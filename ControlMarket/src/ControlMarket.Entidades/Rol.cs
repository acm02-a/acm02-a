namespace ControlMarket.Entidades;

public class Rol : EntidadAuditable
{
    public const int Administrador = 1;
    public const int JefeAlmacen = 2;
    public const int Cajero = 3;

    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
