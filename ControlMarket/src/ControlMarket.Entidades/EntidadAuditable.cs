namespace ControlMarket.Entidades;

/// <summary>
/// Clase base con los cuatro campos de auditoría obligatorios (RNF06).
/// Los valores se llenan automáticamente en ControlMarketContext.SaveChanges().
/// </summary>
public abstract class EntidadAuditable
{
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
