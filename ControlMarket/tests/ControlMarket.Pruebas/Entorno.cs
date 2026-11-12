using ControlMarket.Datos;
using ControlMarket.Entidades;
using ControlMarket.Negocio.Seguridad;
using ControlMarket.Negocio.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ControlMarket.Pruebas;

/// <summary>Base de datos en memoria con datos iniciales y servicios listos para usar.</summary>
public class Entorno
{
    public static readonly DateTime Hoy = new(2026, 10, 4, 11, 0, 0);

    public Sesion Sesion { get; } = new();
    public ContextFactory Factory { get; }
    public UsuarioService Usuarios { get; }
    public CatalogoService Catalogo { get; }
    public InventarioService Inventario { get; }
    public VentaService Ventas { get; }
    public ReporteService Reportes { get; }
    public ParametroService Parametros { get; }

    public Entorno()
    {
        var options = new DbContextOptionsBuilder<ControlMarketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        Factory = new ContextFactory(options, Sesion);
        using (var db = Factory.Crear()) DatosIniciales.Inicializar(db, aplicarMigraciones: false);

        Usuarios = new UsuarioService(Factory, Sesion);
        Catalogo = new CatalogoService(Factory, Sesion);
        Inventario = new InventarioService(Factory, Sesion) { Reloj = () => Hoy };
        Ventas = new VentaService(Factory, Sesion) { Reloj = () => Hoy };
        Reportes = new ReporteService(Factory, Sesion) { Reloj = () => Hoy };
        Parametros = new ParametroService(Factory, Sesion);
    }

    public void LoginAdmin() => Usuarios.IniciarSesion(DatosIniciales.AdminUsername, DatosIniciales.AdminPasswordInicial);

    /// <summary>Crea un usuario con el rol indicado (logueado como admin) y luego inicia sesión con él.</summary>
    public Usuario CrearYLoguear(string username, int idRol)
    {
        LoginAdmin();
        var u = Usuarios.Guardar(0, username, username, "clave123", idRol, true);
        Usuarios.IniciarSesion(username, "clave123");
        return u;
    }

    /// <summary>Catálogo con un producto (stock en dos lotes: 5 que vence pronto y 10 que vence después).</summary>
    public Producto PrepararProductoConStock()
    {
        LoginAdmin();
        var cat = Catalogo.GuardarCategoria(0, "Lácteos", "Leche, yogurt, quesos");
        var prov = Catalogo.GuardarProveedor(0, "20100190797", "Gloria S.A.", "987654321");
        var prod = Catalogo.GuardarProducto(0, "7751271011234", "Leche Gloria 400 g", cat.IdCategoria, 3.10m, 3.80m, 4, true);
        Inventario.RegistrarIngreso(prod.IdProducto, prov.IdProveedor, 10, Hoy.AddDays(-1), Hoy.AddDays(60));
        Inventario.RegistrarIngreso(prod.IdProducto, prov.IdProveedor, 5, Hoy.AddDays(-2), Hoy.AddDays(3));
        return prod;
    }
}
