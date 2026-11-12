namespace ControlMarket.Entidades;

public class Producto : EntidadAuditable
{
    public int IdProducto { get; set; }
    public int IdCategoria { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockMinimo { get; set; }
    public bool Activo { get; set; } = true;

    public Categoria? Categoria { get; set; }
    public ICollection<Lote> Lotes { get; set; } = new List<Lote>();
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

    /// <summary>Stock real = suma de las cantidades de sus lotes no vencidos (CProducto.ObtenerStock).</summary>
    public int ObtenerStock(DateTime hoy) =>
        Lotes.Where(l => l.FechaVencimiento.Date >= hoy.Date).Sum(l => l.Cantidad);

    public bool EstaBajoMinimo(DateTime hoy) => ObtenerStock(hoy) <= StockMinimo;
}
