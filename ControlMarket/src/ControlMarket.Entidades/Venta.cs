namespace ControlMarket.Entidades;

public class Venta : EntidadAuditable
{
    public int IdVenta { get; set; }
    public int IdCajero { get; set; }
    public int IdMetodoPago { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal MontoRecibido { get; set; }
    public decimal Vuelto { get; set; }

    public Usuario? Cajero { get; set; }
    public MetodoPago? MetodoPago { get; set; }
    public Comprobante? Comprobante { get; set; }
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

    /// <summary>
    /// Los precios de venta incluyen IGV (como en el mockup de Caja):
    /// Total = suma de importes; Subtotal = Total / (1 + IGV); Igv = Total - Subtotal.
    /// </summary>
    public void CalcularTotal(decimal porcentajeIgv)
    {
        Total = Math.Round(Detalles.Sum(d => d.CalcularSubtotal()), 2);
        Subtotal = Math.Round(Total / (1 + porcentajeIgv / 100m), 2);
        Igv = Total - Subtotal;
    }
}
