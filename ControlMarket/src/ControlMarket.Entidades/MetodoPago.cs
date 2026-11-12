namespace ControlMarket.Entidades;

public class MetodoPago : EntidadAuditable
{
    public const int Efectivo = 1;

    public int IdMetodoPago { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
