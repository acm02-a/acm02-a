using ControlMarket.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlMarket.Datos.Configuraciones;

// Cada clase mapea una tabla del diccionario de datos (nombres, tipos, PK, FK, UNIQUE, NOT NULL).

internal class RolConfig : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> b)
    {
        b.ToTable("Rol");
        b.HasKey(e => e.IdRol);
        b.Property(e => e.IdRol).ValueGeneratedNever(); // 1=Administrador, 2=JefeAlmacen, 3=Cajero
        b.Property(e => e.NombreRol).HasColumnType("varchar(50)").IsRequired();
        b.HasIndex(e => e.NombreRol).IsUnique();
        b.ConfigurarAuditoria();
    }
}

internal class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("Usuario");
        b.HasKey(e => e.IdUsuario);
        b.Property(e => e.Nombre).HasColumnType("varchar(100)").IsRequired();
        b.Property(e => e.Username).HasColumnType("varchar(50)").IsRequired();
        b.HasIndex(e => e.Username).IsUnique();
        b.Property(e => e.PasswordHash).HasColumnType("varchar(256)").IsRequired();
        b.Property(e => e.Activo).HasDefaultValue(true);
        b.HasOne(e => e.Rol).WithMany(r => r.Usuarios).HasForeignKey(e => e.IdRol).OnDelete(DeleteBehavior.Restrict);
        b.ConfigurarAuditoria();
    }
}

internal class CategoriaConfig : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> b)
    {
        b.ToTable("Categoria");
        b.HasKey(e => e.IdCategoria);
        b.Property(e => e.Nombre).HasColumnType("varchar(50)").IsRequired();
        b.HasIndex(e => e.Nombre).IsUnique();
        b.Property(e => e.Descripcion).HasColumnType("varchar(150)");
        b.ConfigurarAuditoria();
    }
}

internal class ProductoConfig : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> b)
    {
        b.ToTable("Producto", t =>
        {
            t.HasCheckConstraint("CK_Producto_Precios", "PrecioCompra >= 0 AND PrecioVenta >= 0");
            t.HasCheckConstraint("CK_Producto_StockMinimo", "StockMinimo >= 0");
        });
        b.HasKey(e => e.IdProducto);
        b.Property(e => e.CodigoBarras).HasColumnType("varchar(50)").IsRequired();
        b.HasIndex(e => e.CodigoBarras).IsUnique();
        b.Property(e => e.Nombre).HasColumnType("varchar(100)").IsRequired();
        b.Property(e => e.PrecioCompra).HasColumnType("decimal(10,2)");
        b.Property(e => e.PrecioVenta).HasColumnType("decimal(10,2)");
        b.Property(e => e.Activo).HasDefaultValue(true);
        b.HasOne(e => e.Categoria).WithMany(c => c.Productos).HasForeignKey(e => e.IdCategoria).OnDelete(DeleteBehavior.Restrict);
        b.ConfigurarAuditoria();
    }
}

internal class ProveedorConfig : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> b)
    {
        b.ToTable("Proveedor");
        b.HasKey(e => e.IdProveedor);
        b.Property(e => e.Ruc).HasColumnType("char(11)").IsRequired();
        b.HasIndex(e => e.Ruc).IsUnique();
        b.Property(e => e.RazonSocial).HasColumnType("varchar(150)").IsRequired();
        b.Property(e => e.Telefono).HasColumnType("varchar(15)");
        b.ConfigurarAuditoria();
    }
}

internal class IngresoMercaderiaConfig : IEntityTypeConfiguration<IngresoMercaderia>
{
    public void Configure(EntityTypeBuilder<IngresoMercaderia> b)
    {
        b.ToTable("IngresoMercaderia");
        b.HasKey(e => e.IdIngreso);
        b.Property(e => e.Fecha).HasColumnType("datetime");
        b.HasOne(e => e.Proveedor).WithMany(p => p.Ingresos).HasForeignKey(e => e.IdProveedor).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.JefeAlmacen).WithMany(u => u.Ingresos).HasForeignKey(e => e.IdJefeAlmacen).OnDelete(DeleteBehavior.Restrict);
        b.ConfigurarAuditoria();
    }
}

internal class LoteConfig : IEntityTypeConfiguration<Lote>
{
    public void Configure(EntityTypeBuilder<Lote> b)
    {
        b.ToTable("Lote", t =>
        {
            t.HasCheckConstraint("CK_Lote_Cantidad", "Cantidad >= 0 AND Cantidad <= CantidadInicial");
            t.HasCheckConstraint("CK_Lote_Fechas", "FechaVencimiento >= FechaIngreso");
        });
        b.HasKey(e => e.IdLote);
        b.Property(e => e.FechaIngreso).HasColumnType("datetime");
        b.Property(e => e.FechaVencimiento).HasColumnType("datetime");
        b.HasIndex(e => new { e.IdProducto, e.FechaVencimiento });
        b.HasOne(e => e.Producto).WithMany(p => p.Lotes).HasForeignKey(e => e.IdProducto).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.Ingreso).WithMany(i => i.Lotes).HasForeignKey(e => e.IdIngreso).OnDelete(DeleteBehavior.Restrict);
        b.ConfigurarAuditoria();
    }
}

internal class MetodoPagoConfig : IEntityTypeConfiguration<MetodoPago>
{
    public void Configure(EntityTypeBuilder<MetodoPago> b)
    {
        b.ToTable("MetodoPago");
        b.HasKey(e => e.IdMetodoPago);
        b.Property(e => e.IdMetodoPago).ValueGeneratedNever();
        b.Property(e => e.Nombre).HasColumnType("varchar(50)").IsRequired();
        b.HasIndex(e => e.Nombre).IsUnique();
        b.ConfigurarAuditoria();
    }
}

internal class VentaConfig : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> b)
    {
        b.ToTable("Venta", t => t.HasCheckConstraint("CK_Venta_Montos", "Subtotal >= 0 AND Igv >= 0 AND Total >= 0"));
        b.HasKey(e => e.IdVenta);
        b.Property(e => e.Fecha).HasColumnType("datetime");
        b.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
        b.Property(e => e.Igv).HasColumnType("decimal(10,2)");
        b.Property(e => e.Total).HasColumnType("decimal(10,2)");
        b.Property(e => e.MontoRecibido).HasColumnType("decimal(10,2)");
        b.Property(e => e.Vuelto).HasColumnType("decimal(10,2)");
        b.HasIndex(e => e.Fecha);
        b.HasOne(e => e.Cajero).WithMany(u => u.Ventas).HasForeignKey(e => e.IdCajero).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.MetodoPago).WithMany(m => m.Ventas).HasForeignKey(e => e.IdMetodoPago).OnDelete(DeleteBehavior.Restrict);
        b.ConfigurarAuditoria();
    }
}

internal class DetalleVentaConfig : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> b)
    {
        b.ToTable("DetalleVenta", t => t.HasCheckConstraint("CK_DetalleVenta_Cantidad", "Cantidad > 0"));
        b.HasKey(e => e.IdDetalleVenta);
        b.Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)");
        b.Property(e => e.CostoUnitario).HasColumnType("decimal(10,2)");
        b.HasOne(e => e.Venta).WithMany(v => v.Detalles).HasForeignKey(e => e.IdVenta).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Producto).WithMany(p => p.Detalles).HasForeignKey(e => e.IdProducto).OnDelete(DeleteBehavior.Restrict);
        b.ConfigurarAuditoria();
    }
}

internal class ComprobanteConfig : IEntityTypeConfiguration<Comprobante>
{
    public void Configure(EntityTypeBuilder<Comprobante> b)
    {
        b.ToTable("Comprobante", t => t.HasCheckConstraint("CK_Comprobante_Tipo", "Tipo IN ('Boleta','Factura')"));
        b.HasKey(e => e.IdComprobante);
        b.Property(e => e.Tipo).HasColumnType("varchar(20)").IsRequired();
        b.Property(e => e.Serie).HasColumnType("varchar(10)").IsRequired();
        b.Property(e => e.DocumentoCliente).HasColumnType("varchar(11)");
        b.HasIndex(e => new { e.Serie, e.Numero }).IsUnique();
        b.HasIndex(e => e.IdVenta).IsUnique(); // relación 1 a 1 con Venta
        b.HasOne(e => e.Venta).WithOne(v => v.Comprobante).HasForeignKey<Comprobante>(e => e.IdVenta).OnDelete(DeleteBehavior.Cascade);
        b.Ignore(e => e.NumeroCompleto);
        b.ConfigurarAuditoria();
    }
}

internal class ParametroConfig : IEntityTypeConfiguration<Parametro>
{
    public void Configure(EntityTypeBuilder<Parametro> b)
    {
        b.ToTable("Parametro", t =>
        {
            t.HasCheckConstraint("CK_Parametro_Igv", "PorcentajeIgv BETWEEN 0 AND 100");
            t.HasCheckConstraint("CK_Parametro_Dias", "DiasAlertaVencimiento >= 0");
        });
        b.HasKey(e => e.IdParametro);
        b.Property(e => e.RazonSocial).HasColumnType("varchar(150)").IsRequired();
        b.Property(e => e.Ruc).HasColumnType("char(11)").IsRequired();
        b.Property(e => e.Direccion).HasColumnType("varchar(200)");
        b.Property(e => e.PorcentajeIgv).HasColumnType("decimal(5,2)");
        b.ConfigurarAuditoria();
    }
}
