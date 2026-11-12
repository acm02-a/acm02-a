using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ControlMarket.Datos;

/// <summary>
/// Crea un DbContext nuevo por operación (patrón recomendado en aplicaciones de escritorio:
/// contextos de vida corta para no acumular entidades rastreadas).
/// </summary>
public interface IContextFactory
{
    ControlMarketContext Crear();
}

public class ContextFactory : IContextFactory
{
    private readonly DbContextOptions<ControlMarketContext> _options;
    private readonly IUsuarioActual _usuarioActual;

    public ContextFactory(DbContextOptions<ControlMarketContext> options, IUsuarioActual usuarioActual)
    {
        _options = options;
        _usuarioActual = usuarioActual;
    }

    public static ContextFactory ParaSqlServer(string connectionString, IUsuarioActual usuarioActual)
    {
        var options = new DbContextOptionsBuilder<ControlMarketContext>()
            .UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3).CommandTimeout(30))
            .Options;
        return new ContextFactory(options, usuarioActual);
    }

    public ControlMarketContext Crear() => new(_options, _usuarioActual);
}

/// <summary>Usado solo por las herramientas "dotnet ef" (migraciones / script).</summary>
public class DesignTimeFactory : IDesignTimeDbContextFactory<ControlMarketContext>
{
    public ControlMarketContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("CONTROLMARKET_CS")
                 ?? "Server=localhost;Database=ControlMarketDB;Trusted_Connection=True;TrustServerCertificate=True;";
        var options = new DbContextOptionsBuilder<ControlMarketContext>().UseSqlServer(cs).Options;
        return new ControlMarketContext(options);
    }
}
