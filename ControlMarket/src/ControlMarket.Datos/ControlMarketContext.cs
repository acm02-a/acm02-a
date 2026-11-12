using ControlMarket.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Datos;

public class ControlMarketContext : DbContext
{
    private readonly IUsuarioActual _usuarioActual;

    public ControlMarketContext(DbContextOptions<ControlMarketContext> options, IUsuarioActual? usuarioActual = null)
        : base(options)
    {
        _usuarioActual = usuarioActual ?? UsuarioSistema.Instancia;
    }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<IngresoMercaderia> IngresosMercaderia => Set<IngresoMercaderia>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();
    public DbSet<Comprobante> Comprobantes => Set<Comprobante>();
    public DbSet<Parametro> Parametros => Set<Parametro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlMarketContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AplicarAuditoria();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AplicarAuditoria();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>Llena CreatedBy/CreatedAt al insertar y ModifiedBy/ModifiedAt al actualizar.</summary>
    private void AplicarAuditoria()
    {
        var ahora = DateTime.Now;
        var usuario = _usuarioActual.Username;

        foreach (var entry in ChangeTracker.Entries<EntidadAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = string.IsNullOrWhiteSpace(entry.Entity.CreatedBy) ? usuario : entry.Entity.CreatedBy;
                entry.Entity.CreatedAt = ahora;
                entry.Entity.ModifiedBy = null;
                entry.Entity.ModifiedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedBy).IsModified = false;
                entry.Property(e => e.CreatedAt).IsModified = false;
                entry.Entity.ModifiedBy = usuario;
                entry.Entity.ModifiedAt = ahora;
            }
        }
    }
}
