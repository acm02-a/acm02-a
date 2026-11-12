namespace ControlMarket.Entidades;

public class Usuario : EntidadAuditable
{
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public Rol? Rol { get; set; }
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public ICollection<IngresoMercaderia> Ingresos { get; set; } = new List<IngresoMercaderia>();
}
