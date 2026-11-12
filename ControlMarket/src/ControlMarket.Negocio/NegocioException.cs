namespace ControlMarket.Negocio;

/// <summary>Error de regla de negocio o validación: su mensaje se muestra tal cual al usuario.</summary>
public class NegocioException : Exception
{
    public NegocioException(string mensaje) : base(mensaje) { }
}
