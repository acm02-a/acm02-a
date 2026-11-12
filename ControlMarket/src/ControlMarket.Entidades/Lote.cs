namespace ControlMarket.Entidades;

public class Lote : EntidadAuditable
{
    public int IdLote { get; set; }
    public int IdProducto { get; set; }
    public int IdIngreso { get; set; }
    /// <summary>Unidades disponibles actualmente en el lote.</summary>
    public int Cantidad { get; set; }
    /// <summary>Unidades recibidas originalmente (trazabilidad).</summary>
    public int CantidadInicial { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime FechaVencimiento { get; set; }

    public Producto? Producto { get; set; }
    public IngresoMercaderia? Ingreso { get; set; }

    public bool EstaVencido(DateTime hoy) => FechaVencimiento.Date < hoy.Date;

    public bool EstaPorVencer(DateTime hoy, int dias) =>
        !EstaVencido(hoy) && FechaVencimiento.Date <= hoy.Date.AddDays(dias);

    /// <summary>Descuenta unidades del lote y devuelve cuántas se pudieron descontar.</summary>
    public int Descontar(int cantidad)
    {
        var descontado = Math.Min(cantidad, Cantidad);
        Cantidad -= descontado;
        return descontado;
    }
}
