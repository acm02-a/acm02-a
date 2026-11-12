namespace ControlMarket.Presentacion;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.ThreadException += (_, e) => Ui.MostrarError(e.Exception);

        try
        {
            App.Configurar();
            App.InicializarBaseDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "No se pudo conectar con SQL Server.\n\nRevise la cadena de conexión en appsettings.json.\n\nDetalle: " +
                (ex.InnerException?.Message ?? ex.Message),
                "ControlMarket", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.AddMessageFilter(MonitorInactividad.Instancia);

        // Ciclo de sesión: login -> menú -> (cerrar sesión) -> login ...
        while (true)
        {
            using (var login = new FrmIniciar_Sesion())
            {
                if (login.ShowDialog() != DialogResult.OK) break;
            }

            var menu = new FrmMenuPrincipal();
            Application.Run(menu);
            if (!menu.VolverAlLogin) break;
        }
    }
}
