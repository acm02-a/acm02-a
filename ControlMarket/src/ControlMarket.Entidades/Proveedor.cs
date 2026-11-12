namespace ControlMarket.Entidades;

public class Proveedor : EntidadAuditable
{
    public int IdProveedor { get; set; }
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? Telefono { get; set; }

    public ICollection<IngresoMercaderia> Ingresos { get; set; } = new List<IngresoMercaderia>();
}
