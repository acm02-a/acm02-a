using ControlMarket.Entidades;
using ControlMarket.Negocio;
using ControlMarket.Negocio.Seguridad;

namespace ControlMarket.Pruebas;

public class SeguridadTests
{
    [Fact]
    public void Login_admin_inicial_funciona_y_tiene_todos_los_modulos()
    {
        var e = new Entorno();
        var perfil = e.Usuarios.IniciarSesion("admin", "Admin123*");
        Assert.IsType<PerfilAdministrador>(perfil);
        Assert.All(Enum.GetValues<Modulo>(), m => Assert.True(perfil.PuedeAcceder(m)));
    }

    [Fact]
    public void Login_con_clave_incorrecta_falla()
    {
        var e = new Entorno();
        var ex = Assert.Throws<NegocioException>(() => e.Usuarios.IniciarSesion("admin", "otra"));
        Assert.Equal("Usuario o contraseña incorrectos.", ex.Message);
        Assert.False(e.Sesion.Activa);
    }

    [Fact]
    public void Usuario_desactivado_no_puede_ingresar()
    {
        var e = new Entorno();
        e.LoginAdmin();
        var u = e.Usuarios.Guardar(0, "Jorge Salas", "jsalas", "clave123", Rol.Cajero, true);
        e.Usuarios.Guardar(u.IdUsuario, "Jorge Salas", "jsalas", null, Rol.Cajero, false);
        Assert.Throws<NegocioException>(() => e.Usuarios.IniciarSesion("jsalas", "clave123"));
    }

    [Fact]
    public void Cajero_solo_accede_a_caja()
    {
        var e = new Entorno();
        e.CrearYLoguear("lparedes", Rol.Cajero);
        Assert.IsType<PerfilCajero>(e.Sesion.Perfil);
        Assert.Throws<NegocioException>(() => e.Usuarios.Listar());
        Assert.Throws<NegocioException>(() => e.Catalogo.GuardarCategoria(0, "X", null));
    }

    [Fact]
    public void Jefe_de_almacen_gestiona_inventario_pero_no_usuarios()
    {
        var e = new Entorno();
        e.CrearYLoguear("rhuaman", Rol.JefeAlmacen);
        var c = e.Catalogo.GuardarCategoria(0, "Bebidas", null);
        Assert.Equal("rhuaman", c.CreatedBy);
        Assert.Throws<NegocioException>(() => e.Usuarios.Listar());
        Assert.Throws<NegocioException>(() => e.Reportes.TopProductos());
    }

    [Fact]
    public void Username_duplicado_es_rechazado()
    {
        var e = new Entorno();
        e.LoginAdmin();
        Assert.Throws<NegocioException>(() => e.Usuarios.Guardar(0, "Otro", "admin", "clave123", Rol.Cajero, true));
    }

    [Fact]
    public void Contraseña_se_guarda_como_hash()
    {
        var e = new Entorno();
        e.LoginAdmin();
        var u = e.Usuarios.Guardar(0, "Ana Quiroz", "aquiroz", "clave123", Rol.Cajero, true);
        Assert.NotEqual("clave123", u.PasswordHash);
        Assert.True(ControlMarket.Datos.PasswordHasher.Verificar("clave123", u.PasswordHash));
    }
}
