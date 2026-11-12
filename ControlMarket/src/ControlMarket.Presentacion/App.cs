using ControlMarket.Datos;
using ControlMarket.Negocio.Seguridad;
using ControlMarket.Negocio.Servicios;
using Microsoft.Extensions.Configuration;

namespace ControlMarket.Presentacion;

/// <summary>
/// Raíz de composición: lee appsettings.json, crea la fábrica de DbContext (SQL Server)
/// y expone los servicios de la capa de negocio a los formularios.
/// </summary>
internal static class App
{
    public static Sesion Sesion { get; } = new();
    public static IConfiguration Config { get; private set; }
    public static IContextFactory Factory { get; private set; }

    public static UsuarioService Usuarios { get; private set; }
    public static ParametroService Parametros { get; private set; }
    public static CatalogoService Catalogo { get; private set; }
    public static InventarioService Inventario { get; private set; }
    public static VentaService Ventas { get; private set; }
    public static ReporteService Reportes { get; private set; }

    public static int MinutosInactividad => int.TryParse(Config["Seguridad:MinutosInactividad"], out var m) && m > 0 ? m : 15;

    public static void Configurar()
    {
        Config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var cs = Config.GetConnectionString("ControlMarket")
                 ?? throw new InvalidOperationException("Falta la cadena de conexión 'ControlMarket' en appsettings.json.");

        Factory = ContextFactory.ParaSqlServer(cs, Sesion);
        Usuarios = new UsuarioService(Factory, Sesion);
        Parametros = new ParametroService(Factory, Sesion);
        Catalogo = new CatalogoService(Factory, Sesion);
        Inventario = new InventarioService(Factory, Sesion);
        Ventas = new VentaService(Factory, Sesion);
        Reportes = new ReporteService(Factory, Sesion);
    }

    /// <summary>Crea/actualiza la BD (migraciones) y carga roles, métodos de pago, parámetros y admin.</summary>
    public static void InicializarBaseDatos()
    {
        var migrar = !bool.TryParse(Config["BaseDatos:AplicarMigraciones"], out var b) || b;
        using var db = Factory.Crear();
        DatosIniciales.Inicializar(db, migrar);
    }
}
