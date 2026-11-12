namespace ControlMarket.Presentacion;

/// <summary>
/// RNF Seguridad de acceso: registra la última interacción (teclado/ratón) de toda la aplicación
/// para cerrar la sesión automáticamente tras el periodo de inactividad configurado.
/// </summary>
internal sealed class MonitorInactividad : IMessageFilter
{
    public static readonly MonitorInactividad Instancia = new();

    private const int WM_KEYDOWN = 0x0100, WM_MOUSEMOVE = 0x0200, WM_LBUTTONDOWN = 0x0201,
        WM_RBUTTONDOWN = 0x0204, WM_MOUSEWHEEL = 0x020A;

    private Point _ultimaPosicion;

    public DateTime UltimaActividad { get; private set; } = DateTime.Now;

    public TimeSpan TiempoInactivo => DateTime.Now - UltimaActividad;

    public void Reiniciar() => UltimaActividad = DateTime.Now;

    public bool PreFilterMessage(ref Message m)
    {
        switch (m.Msg)
        {
            case WM_MOUSEMOVE:
                var pos = Cursor.Position;
                if (pos != _ultimaPosicion) { _ultimaPosicion = pos; Reiniciar(); }
                break;
            case WM_KEYDOWN:
            case WM_LBUTTONDOWN:
            case WM_RBUTTONDOWN:
            case WM_MOUSEWHEEL:
                Reiniciar();
                break;
        }
        return false;
    }
}
