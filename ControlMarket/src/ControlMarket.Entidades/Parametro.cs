namespace ControlMarket.Entidades;

/// <summary>Parámetros generales de la tienda (RF Parámetros generales). Tabla de una sola fila.</summary>
public class Parametro : EntidadAuditable
{
    public int IdParametro { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string Ruc { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public decimal PorcentajeIgv { get; set; } = 18m;
    public int DiasAlertaVencimiento { get; set; } = 7;
}
