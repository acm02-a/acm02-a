using ControlMarket.Negocio;

namespace ControlMarket.Pruebas;

public class AuditoriaYCatalogoTests
{
    [Fact]
    public void Campos_de_auditoria_se_llenan_al_crear_y_modificar()
    {
        var e = new Entorno();
        e.LoginAdmin();
        var cat = e.Catalogo.GuardarCategoria(0, "Abarrotes", "Arroz, azúcar");
        Assert.Equal("admin", cat.CreatedBy);
        Assert.NotEqual(default, cat.CreatedAt);
        Assert.Null(cat.ModifiedBy);

        e.CrearYLoguear("rhuaman", ControlMarket.Entidades.Rol.JefeAlmacen);
        var mod = e.Catalogo.GuardarCategoria(cat.IdCategoria, "Abarrotes", "Arroz, azúcar, fideos");
        Assert.Equal("admin", mod.CreatedBy);      // no cambia
        Assert.Equal("rhuaman", mod.ModifiedBy);
        Assert.NotNull(mod.ModifiedAt);
    }

    [Fact]
    public void Producto_valida_codigo_unico_y_precios()
    {
        var e = new Entorno();
        var p = e.PrepararProductoConStock();
        Assert.Throws<NegocioException>(() =>
            e.Catalogo.GuardarProducto(0, p.CodigoBarras, "Otro", p.IdCategoria, 1, 2, 0, true));
        Assert.Throws<NegocioException>(() =>
            e.Catalogo.GuardarProducto(0, "123", "Barato", p.IdCategoria, 5, 4, 0, true));
    }

    [Fact]
    public void Proveedor_requiere_ruc_de_11_digitos()
    {
        var e = new Entorno();
        e.LoginAdmin();
        Assert.Throws<NegocioException>(() => e.Catalogo.GuardarProveedor(0, "123", "X", null));
    }

    [Fact]
    public void Parametros_se_actualizan()
    {
        var e = new Entorno();
        e.LoginAdmin();
        e.Parametros.Guardar("Mi Market", "20601234567", "Av. 1", 18, 10);
        var p = e.Parametros.Obtener();
        Assert.Equal(10, p.DiasAlertaVencimiento);
        Assert.Equal("admin", p.ModifiedBy);
        Assert.Throws<NegocioException>(() => e.Parametros.Guardar("X", "20601234567", null, 150, 7));
    }
}

public class CompatibilidadBdIntegrante2Tests
{
    [Fact]
    public void Categorias_reciben_id_correlativo_porque_la_columna_no_es_identity()
    {
        var e = new Entorno();
        e.LoginAdmin();
        var a = e.Catalogo.GuardarCategoria(0, "Abarrotes", null);
        var b = e.Catalogo.GuardarCategoria(0, "Bebidas", null);
        Assert.Equal(1, a.IdCategoria);
        Assert.Equal(2, b.IdCategoria);
    }

    [Fact]
    public void Producto_creado_inactivo_se_guarda_inactivo()
    {
        var e = new Entorno();
        e.LoginAdmin();
        var cat = e.Catalogo.GuardarCategoria(0, "Limpieza", null);
        var p = e.Catalogo.GuardarProducto(0, "111", "Lejía", cat.IdCategoria, 2, 3, 1, activo: false);
        using var db = e.Factory.Crear();
        Assert.False(db.Productos.Find(p.IdProducto)!.Activo);
    }

    [Fact]
    public void Roles_y_metodos_de_pago_iniciales_tienen_los_ids_esperados()
    {
        var e = new Entorno();
        using var db = e.Factory.Crear();
        Assert.Equal("Administrador", db.Roles.Find(ControlMarket.Entidades.Rol.Administrador)!.NombreRol);
        Assert.Equal("Cajero", db.Roles.Find(ControlMarket.Entidades.Rol.Cajero)!.NombreRol);
        Assert.Equal("Efectivo", db.MetodosPago.Find(ControlMarket.Entidades.MetodoPago.Efectivo)!.Nombre);
    }
}
