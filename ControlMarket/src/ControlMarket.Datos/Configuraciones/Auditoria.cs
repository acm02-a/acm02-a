using ControlMarket.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlMarket.Datos.Configuraciones;

internal static class Auditoria
{
    /// <summary>Mapea los 4 campos de auditoría con los tipos del diccionario de datos.</summary>
    public static void ConfigurarAuditoria<T>(this EntityTypeBuilder<T> b) where T : EntidadAuditable
    {
        b.Property(e => e.CreatedBy).HasColumnType("varchar(50)").IsRequired();
        b.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        b.Property(e => e.ModifiedBy).HasColumnType("varchar(50)");
        b.Property(e => e.ModifiedAt).HasColumnType("datetime");
    }
}
