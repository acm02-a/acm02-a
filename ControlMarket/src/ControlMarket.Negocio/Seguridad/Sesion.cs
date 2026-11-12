using ControlMarket.Datos;
using ControlMarket.Entidades;

namespace ControlMarket.Negocio.Seguridad;

/// <summary>Usuario logueado. También alimenta los campos de auditoría (IUsuarioActual).</summary>
public class Sesion : IUsuarioActual
{
    public Perfil? Perfil { get; private set; }
    public Usuario? Usuario => Perfil?.Usuario;
    public bool Activa => Perfil != null;

    public string Username => Usuario?.Username ?? "sistema";

    public void Iniciar(Usuario usuario) => Perfil = Perfil.Crear(usuario);

    public void Cerrar() => Perfil = null;

    /// <summary>Verifica en la capa de negocio que el usuario tenga permiso para el módulo.</summary>
    public void Exigir(Modulo modulo)
    {
        if (Perfil == null)
            throw new NegocioException("Debe iniciar sesión.");
        if (!Perfil.PuedeAcceder(modulo))
            throw new NegocioException($"Su rol ({Perfil.NombreRol}) no tiene acceso al módulo {modulo}.");
    }
}
