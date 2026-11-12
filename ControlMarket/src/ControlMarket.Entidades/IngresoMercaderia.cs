namespace ControlMarket.Entidades;

public class IngresoMercaderia : EntidadAuditable
{
    public int IdIngreso { get; set; }
    public int IdProveedor { get; set; }
    public int IdJefeAlmacen { get; set; }
    public DateTime Fecha { get; set; }

    public Proveedor? Proveedor { get; set; }
    public Usuario? JefeAlmacen { get; set; }
    public ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}
