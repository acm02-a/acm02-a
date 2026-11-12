using ControlMarket.Datos;
using ControlMarket.Entidades;
using ControlMarket.Entidades.Dtos;
using ControlMarket.Negocio.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Negocio.Servicios;

/// <summary>RF Ingreso por lotes, Consulta de stock, Alertas de stock mínimo y de vencimiento.</summary>
public class InventarioService : ServicioBase
{
    private readonly ParametroService _parametros;

    public InventarioService(IContextFactory factory, Sesion sesion) : base(factory, sesion)
    {
        _parametros = new ParametroService(factory, sesion);
    }

    public Lote RegistrarIngreso(int idProducto, int idProveedor, int cantidad, DateTime fechaIngreso, DateTime fechaVencimiento)
    {
        Sesion.Exigir(Modulo.LotesStock);
        if (cantidad <= 0) throw new NegocioException("La cantidad debe ser mayor a cero.");
        if (fechaIngreso.Date > Reloj().Date) throw new NegocioException("La fecha de ingreso no puede ser futura.");
        if (fechaVencimiento.Date <= fechaIngreso.Date)
            throw new NegocioException("La fecha de vencimiento debe ser posterior a la fecha de ingreso.");

        using var db = Factory.Crear();
        var producto = db.Productos.Find(idProducto) ?? throw new NegocioException("Seleccione un producto válido.");
        if (!producto.Activo) throw new NegocioException("El producto está desactivado.");
        if (!db.Proveedores.Any(p => p.IdProveedor == idProveedor))
            throw new NegocioException("Seleccione un proveedor válido.");

        var ingreso = new IngresoMercaderia
        {
            IdProveedor = idProveedor,
            IdJefeAlmacen = Sesion.Usuario!.IdUsuario,
            Fecha = fechaIngreso
        };
        var lote = new Lote
        {
            IdProducto = idProducto,
            Cantidad = cantidad,
            CantidadInicial = cantidad,
            FechaIngreso = fechaIngreso,
            FechaVencimiento = fechaVencimiento.Date
        };
        ingreso.Lotes.Add(lote);
        db.IngresosMercaderia.Add(ingreso);
        db.SaveChanges();
        return lote;
    }

    /// <summary>Stock vendible = lotes con unidades y no vencidos.</summary>
    public int ObtenerStock(int idProducto)
    {
        var hoy = Reloj().Date;
        using var db = Factory.Crear();
        return db.Lotes.Where(l => l.IdProducto == idProducto && l.FechaVencimiento >= hoy).Sum(l => (int?)l.Cantidad) ?? 0;
    }

    public List<StockProductoDto> ConsultarStock(string? filtro = null)
    {
        var hoy = Reloj().Date;
        var limite = hoy.AddDays(_parametros.Obtener().DiasAlertaVencimiento);
        using var db = Factory.Crear();
        var q = db.Productos.AsNoTracking().Where(p => p.Activo);
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var f = filtro.Trim();
            q = q.Where(p => p.Nombre.Contains(f) || p.CodigoBarras.Contains(f));
        }

        var datos = q.Select(p => new
            {
                p.IdProducto, p.CodigoBarras, p.Nombre, p.StockMinimo,
                Stock = p.Lotes.Where(l => l.FechaVencimiento >= hoy).Sum(l => (int?)l.Cantidad) ?? 0,
                Proximo = p.Lotes.Where(l => l.Cantidad > 0 && l.FechaVencimiento >= hoy).Min(l => (DateTime?)l.FechaVencimiento)
            })
            .OrderBy(p => p.Nombre)
            .ToList();

        return datos.Select(d => new StockProductoDto(
            d.IdProducto, d.CodigoBarras, d.Nombre, d.Stock, d.StockMinimo, d.Proximo,
            d.Stock <= d.StockMinimo ? "Bajo mínimo"
            : d.Proximo.HasValue && d.Proximo.Value <= limite ? "Por vencer"
            : "Normal")).ToList();
    }

    public List<AlertaStockDto> AlertasStockMinimo() =>
        ConsultarStock()
            .Where(s => s.StockActual <= s.StockMinimo)
            .Select(s => new AlertaStockDto(s.CodigoBarras, s.Producto, s.StockActual, s.StockMinimo))
            .ToList();

    public List<AlertaVencimientoDto> AlertasVencimiento()
    {
        var hoy = Reloj().Date;
        var limite = hoy.AddDays(_parametros.Obtener().DiasAlertaVencimiento);
        using var db = Factory.Crear();
        return db.Lotes.AsNoTracking()
            .Where(l => l.Cantidad > 0 && l.FechaVencimiento >= hoy && l.FechaVencimiento <= limite)
            .OrderBy(l => l.FechaVencimiento)
            .Select(l => new { l.IdLote, Producto = l.Producto!.Nombre, l.Cantidad, l.FechaVencimiento })
            .AsEnumerable()
            .Select(l => new AlertaVencimientoDto(l.IdLote, $"L-{l.IdLote:D4}", l.Producto, l.Cantidad, l.FechaVencimiento))
            .ToList();
    }
}
