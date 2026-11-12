using System.Data;
using ControlMarket.Datos;
using ControlMarket.Entidades;
using ControlMarket.Entidades.Dtos;
using ControlMarket.Negocio.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Negocio.Servicios;

/// <summary>
/// RF Registro de productos en caja, Cálculo de importes, Métodos de pago y vuelto,
/// Emisión de comprobantes, Descuento de stock (FEFO) y Restricción de stock.
/// </summary>
public class VentaService : ServicioBase
{
    private readonly InventarioService _inventario;
    private readonly ParametroService _parametros;

    public VentaService(IContextFactory factory, Sesion sesion) : base(factory, sesion)
    {
        _inventario = new InventarioService(factory, sesion) { Reloj = () => Reloj() };
        _parametros = new ParametroService(factory, sesion);
    }

    public decimal PorcentajeIgv => _parametros.Obtener().PorcentajeIgv;

    public List<MetodoPago> ListarMetodosPago()
    {
        using var db = Factory.Crear();
        return db.MetodosPago.AsNoTracking().OrderBy(m => m.IdMetodoPago).ToList();
    }

    /// <summary>Agrega un producto al carrito validando que haya stock suficiente.</summary>
    public ItemCarritoDto AgregarProducto(Carrito carrito, string? codigoBarras, int cantidad)
    {
        Sesion.Exigir(Modulo.Caja);
        if (string.IsNullOrWhiteSpace(codigoBarras)) throw new NegocioException("Ingrese el código de barras.");
        if (cantidad <= 0) throw new NegocioException("La cantidad debe ser mayor a cero.");

        Producto producto;
        using (var db = Factory.Crear())
        {
            var codigo = codigoBarras.Trim();
            producto = db.Productos.AsNoTracking().FirstOrDefault(p => p.CodigoBarras == codigo)
                       ?? throw new NegocioException($"No existe un producto con código {codigo}.");
        }
        if (!producto.Activo) throw new NegocioException($"El producto {producto.Nombre} está desactivado.");

        var disponible = _inventario.ObtenerStock(producto.IdProducto) - carrito.CantidadDe(producto.IdProducto);
        if (cantidad > disponible)
            throw new NegocioException($"Stock insuficiente: {producto.Nombre} solo tiene {Math.Max(disponible, 0)} unidades disponibles.");

        var item = new ItemCarritoDto(producto.IdProducto, producto.CodigoBarras, producto.Nombre, cantidad, producto.PrecioVenta);
        carrito.Agregar(item);
        return item;
    }

    /// <summary>
    /// Confirma la venta en una sola transacción: crea Venta + Detalles + Comprobante y
    /// descuenta stock del lote con vencimiento más próximo. Si algo falla, no se guarda nada.
    /// </summary>
    public Venta Cobrar(Carrito carrito, int idMetodoPago, string tipoComprobante, string? documentoCliente, decimal montoRecibido)
    {
        Sesion.Exigir(Modulo.Caja);
        if (carrito.EstaVacio) throw new NegocioException("Agregue al menos un producto a la venta.");

        var tipo = tipoComprobante == Comprobante.Factura ? Comprobante.Factura : Comprobante.Boleta;
        string? documento = documentoCliente?.Trim();
        if (tipo == Comprobante.Factura)
            documento = Digitos(documento, "RUC del cliente", 11);
        else if (!string.IsNullOrEmpty(documento))
            documento = Digitos(documento, "DNI del cliente", 8);
        else
            documento = null;

        var igv = PorcentajeIgv;
        var hoy = Reloj();

        using var db = Factory.Crear();
        if (!db.MetodosPago.Any(m => m.IdMetodoPago == idMetodoPago))
            throw new NegocioException("Seleccione un método de pago.");

        var estrategia = db.Database.CreateExecutionStrategy();
        return estrategia.Execute(() =>
        {
            db.ChangeTracker.Clear();
            using var tx = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;

            var venta = new Venta { IdCajero = Sesion.Usuario!.IdUsuario, IdMetodoPago = idMetodoPago, Fecha = hoy };

            foreach (var item in carrito.Items)
            {
                var producto = db.Productos.Find(item.IdProducto)
                               ?? throw new NegocioException($"El producto {item.Producto} ya no existe.");

                // FEFO: primero el lote que vence antes (ignorando los vencidos).
                var lotes = db.Lotes
                    .Where(l => l.IdProducto == item.IdProducto && l.Cantidad > 0 && l.FechaVencimiento >= hoy.Date)
                    .OrderBy(l => l.FechaVencimiento).ThenBy(l => l.IdLote)
                    .ToList();

                if (lotes.Sum(l => l.Cantidad) < item.Cantidad)
                    throw new NegocioException($"Stock insuficiente para {producto.Nombre}. La venta no se registró.");

                var pendiente = item.Cantidad;
                foreach (var lote in lotes)
                {
                    if (pendiente == 0) break;
                    pendiente -= lote.Descontar(pendiente);
                }

                venta.Detalles.Add(new DetalleVenta
                {
                    IdProducto = producto.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.PrecioVenta,
                    CostoUnitario = producto.PrecioCompra
                });
            }

            venta.CalcularTotal(igv);

            if (idMetodoPago == MetodoPago.Efectivo)
            {
                if (montoRecibido < venta.Total)
                    throw new NegocioException($"El monto recibido (S/ {montoRecibido:N2}) es menor al total (S/ {venta.Total:N2}).");
                venta.MontoRecibido = montoRecibido;
                venta.Vuelto = montoRecibido - venta.Total;
            }
            else
            {
                venta.MontoRecibido = venta.Total;
                venta.Vuelto = 0;
            }

            var serie = tipo == Comprobante.Factura ? "F001" : "B001";
            var numero = (db.Comprobantes.Where(c => c.Serie == serie).Max(c => (int?)c.Numero) ?? 0) + 1;
            venta.Comprobante = new Comprobante { Tipo = tipo, Serie = serie, Numero = numero, DocumentoCliente = documento };

            db.Ventas.Add(venta);
            db.SaveChanges();
            tx?.Commit();
            carrito.Vaciar();
            return venta;
        });
    }

    /// <summary>Venta completa con detalle, productos, cajero y comprobante (para imprimir).</summary>
    public Venta ObtenerVenta(int idVenta)
    {
        using var db = Factory.Crear();
        return db.Ventas.AsNoTracking()
                   .Include(v => v.Detalles).ThenInclude(d => d.Producto)
                   .Include(v => v.Cajero)
                   .Include(v => v.MetodoPago)
                   .Include(v => v.Comprobante)
                   .FirstOrDefault(v => v.IdVenta == idVenta)
               ?? throw new NegocioException("La venta no existe.");
    }

    /// <summary>Texto del comprobante electrónico (pantalla «Comprobante de pago»).</summary>
    public string GenerarTextoComprobante(int idVenta)
    {
        var v = ObtenerVenta(idVenta);
        var p = _parametros.Obtener();
        var c = v.Comprobante!;
        var linea = new string('-', 40);
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(Centrar(p.RazonSocial.ToUpperInvariant(), 40));
        sb.AppendLine(Centrar($"RUC {p.Ruc}", 40));
        if (!string.IsNullOrEmpty(p.Direccion)) sb.AppendLine(Centrar(p.Direccion, 40));
        sb.AppendLine(Centrar(c.Tipo == Comprobante.Factura ? "FACTURA ELECTRÓNICA" : "BOLETA DE VENTA ELECTRÓNICA", 40));
        sb.AppendLine(Centrar(c.NumeroCompleto, 40));
        sb.AppendLine(linea);
        sb.AppendLine($"Fecha: {v.Fecha:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"Cajero: {v.Cajero?.Username}" + (c.DocumentoCliente != null ? $"   {(c.Tipo == Comprobante.Factura ? "RUC" : "DNI")}: {c.DocumentoCliente}" : ""));
        sb.AppendLine(linea);
        sb.AppendLine($"{"Cant",-5}{"Descripción",-25}{"Importe",10}");
        foreach (var d in v.Detalles)
        {
            var nombre = d.Producto?.Nombre ?? "";
            if (nombre.Length > 24) nombre = nombre[..24];
            sb.AppendLine($"{d.Cantidad,-5}{nombre,-25}{d.CalcularSubtotal(),10:N2}");
        }
        sb.AppendLine(linea);
        sb.AppendLine($"{"Subtotal",-30}{v.Subtotal,10:N2}");
        sb.AppendLine($"{$"IGV {p.PorcentajeIgv:0.##}%",-30}{v.Igv,10:N2}");
        sb.AppendLine($"{"TOTAL S/",-30}{v.Total,10:N2}");
        sb.AppendLine($"{v.MetodoPago?.Nombre,-30}{v.MontoRecibido,10:N2}");
        sb.AppendLine($"{"Vuelto",-30}{v.Vuelto,10:N2}");
        return sb.ToString();
    }

    private static string Centrar(string texto, int ancho) =>
        texto.Length >= ancho ? texto : texto.PadLeft((ancho + texto.Length) / 2);
}
