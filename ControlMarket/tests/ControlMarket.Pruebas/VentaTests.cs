using ControlMarket.Entidades;
using ControlMarket.Negocio;
using ControlMarket.Negocio.Servicios;

namespace ControlMarket.Pruebas;

public class VentaTests
{
    private static (Entorno e, Producto p) PrepararCaja()
    {
        var e = new Entorno();
        var p = e.PrepararProductoConStock();
        e.CrearYLoguear("lparedes", Rol.Cajero);
        return (e, p);
    }

    [Fact]
    public void Calcula_subtotal_igv_y_total()
    {
        var (e, p) = PrepararCaja();
        var carrito = new Carrito();
        e.Ventas.AgregarProducto(carrito, p.CodigoBarras, 6);   // 6 x 3.80 = 22.80
        var (sub, igv, total) = carrito.Totales(18);
        Assert.Equal(22.80m, total);
        Assert.Equal(19.32m, sub);
        Assert.Equal(3.48m, igv);
    }

    [Fact]
    public void Restriccion_de_stock_impide_agregar_mas_de_lo_disponible()
    {
        var (e, p) = PrepararCaja();
        var carrito = new Carrito();
        e.Ventas.AgregarProducto(carrito, p.CodigoBarras, 10);
        var ex = Assert.Throws<NegocioException>(() => e.Ventas.AgregarProducto(carrito, p.CodigoBarras, 6));
        Assert.Contains("solo tiene 5 unidades", ex.Message);
    }

    [Fact]
    public void Cobro_descuenta_primero_el_lote_mas_proximo_a_vencer_y_emite_boleta()
    {
        var (e, p) = PrepararCaja();
        var carrito = new Carrito();
        e.Ventas.AgregarProducto(carrito, p.CodigoBarras, 7);

        var venta = e.Ventas.Cobrar(carrito, MetodoPago.Efectivo, Comprobante.Boleta, "45871236", 30m);

        Assert.Equal(26.60m, venta.Total);
        Assert.Equal(3.40m, venta.Vuelto);
        Assert.Equal("B001", venta.Comprobante!.Serie);
        Assert.Equal(1, venta.Comprobante.Numero);
        Assert.True(carrito.EstaVacio);

        using var db = e.Factory.Crear();
        var lotes = db.Lotes.OrderBy(l => l.FechaVencimiento).ToList();
        Assert.Equal(0, lotes[0].Cantidad);   // lote que vencía en 3 días: 5 -> 0
        Assert.Equal(8, lotes[1].Cantidad);   // segundo lote: 10 -> 8
        Assert.Equal(26.60m, db.DetallesVenta.Sum(d => d.Cantidad * d.PrecioUnitario));
        Assert.Equal("lparedes", db.Ventas.Single().CreatedBy);
    }

    [Fact]
    public void Comprobantes_tienen_correlativo_por_serie()
    {
        var (e, p) = PrepararCaja();
        for (var i = 0; i < 2; i++)
        {
            var c = new Carrito();
            e.Ventas.AgregarProducto(c, p.CodigoBarras, 1);
            e.Ventas.Cobrar(c, MetodoPago.Efectivo, Comprobante.Boleta, null, 10m);
        }
        var cf = new Carrito();
        e.Ventas.AgregarProducto(cf, p.CodigoBarras, 1);
        var f = e.Ventas.Cobrar(cf, 2, Comprobante.Factura, "20100190797", 0m);

        Assert.Equal("F001-00000001", f.Comprobante!.NumeroCompleto);
        Assert.Equal(0m, f.Vuelto);
        using var db = e.Factory.Crear();
        Assert.Equal(new[] { 1, 2 }, db.Comprobantes.Where(x => x.Serie == "B001").Select(x => x.Numero).OrderBy(n => n).ToArray());
    }

    [Fact]
    public void Monto_insuficiente_cancela_la_venta_sin_tocar_el_stock()
    {
        var (e, p) = PrepararCaja();
        var carrito = new Carrito();
        e.Ventas.AgregarProducto(carrito, p.CodigoBarras, 2);
        Assert.Throws<NegocioException>(() => e.Ventas.Cobrar(carrito, MetodoPago.Efectivo, Comprobante.Boleta, null, 1m));

        Assert.False(carrito.EstaVacio);
        Assert.Equal(15, e.Inventario.ObtenerStock(p.IdProducto));
        using var db = e.Factory.Crear();
        Assert.Empty(db.Ventas);
    }

    [Fact]
    public void Factura_exige_ruc_valido()
    {
        var (e, p) = PrepararCaja();
        var carrito = new Carrito();
        e.Ventas.AgregarProducto(carrito, p.CodigoBarras, 1);
        Assert.Throws<NegocioException>(() => e.Ventas.Cobrar(carrito, 2, Comprobante.Factura, "4587", 0));
    }

    [Fact]
    public void Texto_de_comprobante_incluye_numero_y_totales()
    {
        var (e, p) = PrepararCaja();
        var carrito = new Carrito();
        e.Ventas.AgregarProducto(carrito, p.CodigoBarras, 1);
        var v = e.Ventas.Cobrar(carrito, MetodoPago.Efectivo, Comprobante.Boleta, null, 5m);
        var texto = e.Ventas.GenerarTextoComprobante(v.IdVenta);
        Assert.Contains("B001-00000001", texto);
        Assert.Contains("3.80", texto.Replace(',', '.'));
    }
}
