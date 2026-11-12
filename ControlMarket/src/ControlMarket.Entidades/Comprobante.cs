namespace ControlMarket.Entidades;

public class Comprobante : EntidadAuditable
{
    public const string Boleta = "Boleta";
    public const string Factura = "Factura";

    public int IdComprobante { get; set; }
    public int IdVenta { get; set; }
    public string Tipo { get; set; } = Boleta;
    public string Serie { get; set; } = string.Empty;
    public int Numero { get; set; }
    /// <summary>DNI (boleta) o RUC (factura) del cliente.</summary>
    public string? DocumentoCliente { get; set; }

    public Venta? Venta { get; set; }

    public string NumeroCompleto => $"{Serie}-{Numero:D8}";
}
