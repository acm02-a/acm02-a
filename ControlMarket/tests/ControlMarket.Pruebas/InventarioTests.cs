using ControlMarket.Negocio;

namespace ControlMarket.Pruebas;

public class InventarioTests
{
    [Fact]
    public void Stock_es_la_suma_de_lotes_vigentes()
    {
        var e = new Entorno();
        var p = e.PrepararProductoConStock();
        Assert.Equal(15, e.Inventario.ObtenerStock(p.IdProducto));
    }

    [Fact]
    public void Ingreso_registra_jefe_de_almacen_y_valida_fechas()
    {
        var e = new Entorno();
        var p = e.PrepararProductoConStock();
        var prov = e.Catalogo.ListarProveedores().First();
        Assert.Throws<NegocioException>(() =>
            e.Inventario.RegistrarIngreso(p.IdProducto, prov.IdProveedor, 5, Entorno.Hoy, Entorno.Hoy.AddDays(-1)));
        Assert.Throws<NegocioException>(() =>
            e.Inventario.RegistrarIngreso(p.IdProducto, prov.IdProveedor, 0, Entorno.Hoy, Entorno.Hoy.AddDays(5)));
    }

    [Fact]
    public void Alerta_de_vencimiento_lista_lotes_dentro_del_plazo()
    {
        var e = new Entorno();
        e.PrepararProductoConStock();
        var alertas = e.Inventario.AlertasVencimiento();
        var a = Assert.Single(alertas);           // solo el lote que vence en 3 días (plazo = 7)
        Assert.Equal(5, a.Cantidad);
    }

    [Fact]
    public void Alerta_de_stock_minimo_aparece_cuando_stock_baja()
    {
        var e = new Entorno();
        e.LoginAdmin();
        var cat = e.Catalogo.GuardarCategoria(0, "Limpieza", null);
        e.Catalogo.GuardarProducto(0, "7500435126789", "Detergente Ariel 750 g", cat.IdCategoria, 8, 10, 15, true);
        var alerta = Assert.Single(e.Inventario.AlertasStockMinimo());
        Assert.Equal(0, alerta.StockActual);
        Assert.Equal("Bajo mínimo", e.Inventario.ConsultarStock().Single().Estado);
    }
}
