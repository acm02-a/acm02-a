namespace ControlMarket.Entidades;

public class DetalleVenta : EntidadAuditable
{
    public int IdDetalleVenta { get; set; }
    public int IdVenta { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    /// <summary>Precio de venta congelado al momento de facturar.</summary>
    public decimal PrecioUnitario { get; set; }
    /// <summary>Precio de compra congelado al momento de facturar (para el reporte de rentabilidad).</summary>
    public decimal CostoUnitario { get; set; }

    public Venta? Venta { get; set; }
    public Producto? Producto { get; set; }

    public decimal CalcularSubtotal() => Cantidad * PrecioUnitario;
}
