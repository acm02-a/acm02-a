using ControlMarket.Entidades;
using ControlMarket.Negocio.Servicios;

namespace ControlMarket.Pruebas;

public class ReporteTests
{
    [Fact]
    public void Reportes_de_ventas_rentabilidad_criticos_y_top5()
    {
        var e = new Entorno();
        var p = e.PrepararProductoConStock();
        var cajero = e.CrearYLoguear("lparedes", Rol.Cajero);
        var c = new Carrito();
        e.Ventas.AgregarProducto(c, p.CodigoBarras, 4);
        e.Ventas.Cobrar(c, MetodoPago.Efectivo, Comprobante.Boleta, null, 20m);

        e.LoginAdmin();
        var ventas = e.Reportes.VentasPorFecha(Entorno.Hoy.AddDays(-1), Entorno.Hoy);
        var fila = Assert.Single(ventas);
        Assert.Equal(1, fila.NumeroVentas);
        Assert.Equal(15.20m, fila.Total);
        Assert.Empty(e.Reportes.VentasPorFecha(Entorno.Hoy.AddDays(-1), Entorno.Hoy, idCajero: 999));
        Assert.Single(e.Reportes.VentasPorFecha(Entorno.Hoy, Entorno.Hoy, cajero.IdUsuario));

        var rent = Assert.Single(e.Reportes.Rentabilidad(Entorno.Hoy, Entorno.Hoy, porCategoria: true));
        Assert.Equal("Lácteos", rent.Grupo);
        Assert.Equal(15.20m, rent.Ingreso);
        Assert.Equal(12.40m, rent.Costo);
        Assert.Equal(2.80m, rent.GananciaNeta);

        var top = Assert.Single(e.Reportes.TopProductos());
        Assert.Equal(4, top.Unidades);

        // quedan 1 unidad en el lote por vencer (5-4) -> aparece como próximo a vencer
        Assert.Contains(e.Reportes.ProductosCriticos(), x => x.Motivo == "Próximo a vencer" && x.Stock == 1);
    }
}
