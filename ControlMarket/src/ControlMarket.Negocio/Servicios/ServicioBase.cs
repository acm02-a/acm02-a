using ControlMarket.Datos;
using ControlMarket.Negocio.Seguridad;

namespace ControlMarket.Negocio.Servicios;

public abstract class ServicioBase
{
    protected ServicioBase(IContextFactory factory, Sesion sesion)
    {
        Factory = factory;
        Sesion = sesion;
    }

    protected IContextFactory Factory { get; }
    protected Sesion Sesion { get; }

    /// <summary>Fecha "de hoy" (inyectable para pruebas).</summary>
    public Func<DateTime> Reloj { get; set; } = () => DateTime.Now;

    protected static string Requerido(string? valor, string campo, int max)
    {
        var v = valor?.Trim() ?? string.Empty;
        if (v.Length == 0) throw new NegocioException($"El campo {campo} es obligatorio.");
        if (v.Length > max) throw new NegocioException($"El campo {campo} admite como máximo {max} caracteres.");
        return v;
    }

    protected static string? Opcional(string? valor, string campo, int max)
    {
        var v = valor?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        if (v.Length > max) throw new NegocioException($"El campo {campo} admite como máximo {max} caracteres.");
        return v;
    }

    protected static string Digitos(string? valor, string campo, int longitud)
    {
        var v = valor?.Trim() ?? string.Empty;
        if (v.Length != longitud || !v.All(char.IsDigit))
            throw new NegocioException($"El {campo} debe tener {longitud} dígitos.");
        return v;
    }
}
