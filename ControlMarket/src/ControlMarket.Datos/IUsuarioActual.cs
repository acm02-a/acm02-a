namespace ControlMarket.Datos;

/// <summary>Provee el username del usuario logueado para llenar CreatedBy / ModifiedBy.</summary>
public interface IUsuarioActual
{
    string Username { get; }
}

public sealed class UsuarioSistema : IUsuarioActual
{
    public static readonly UsuarioSistema Instancia = new();
    public string Username => "sistema";
}
