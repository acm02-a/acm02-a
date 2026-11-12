using ControlMarket.Entidades;

namespace ControlMarket.Negocio.Seguridad;

public enum Modulo
{
    Usuarios,
    Parametros,
    Productos,
    CategoriasProveedores,
    LotesStock,
    Caja,
    Reportes
}

/// <summary>
/// Implementa la jerarquía CUsuario del diagrama de clases: clase abstracta con el
/// método polimórfico ObtenerMenu(); cada rol define a qué módulos tiene acceso.
/// </summary>
public abstract class Perfil
{
    protected Perfil(Usuario usuario) => Usuario = usuario;

    public Usuario Usuario { get; }
    public abstract string NombreRol { get; }
    public abstract IReadOnlyCollection<Modulo> Modulos { get; }

    public bool PuedeAcceder(Modulo modulo) => Modulos.Contains(modulo);

    public string ObtenerMenu() => $"{NombreRol}: {string.Join(", ", Modulos)}";

    public static Perfil Crear(Usuario usuario) => usuario.IdRol switch
    {
        Rol.Administrador => new PerfilAdministrador(usuario),
        Rol.JefeAlmacen => new PerfilJefeAlmacen(usuario),
        Rol.Cajero => new PerfilCajero(usuario),
        _ => throw new NegocioException("El usuario no tiene un rol válido.")
    };
}

public sealed class PerfilAdministrador : Perfil
{
    public PerfilAdministrador(Usuario u) : base(u) { }
    public override string NombreRol => "Administrador";
    public override IReadOnlyCollection<Modulo> Modulos { get; } = Enum.GetValues<Modulo>();
}

public sealed class PerfilJefeAlmacen : Perfil
{
    public PerfilJefeAlmacen(Usuario u) : base(u) { }
    public override string NombreRol => "Jefe de Almacén";
    public override IReadOnlyCollection<Modulo> Modulos { get; } =
        new[] { Modulo.Productos, Modulo.CategoriasProveedores, Modulo.LotesStock };
}

public sealed class PerfilCajero : Perfil
{
    public PerfilCajero(Usuario u) : base(u) { }
    public override string NombreRol => "Cajero";
    public override IReadOnlyCollection<Modulo> Modulos { get; } = new[] { Modulo.Caja };
}
