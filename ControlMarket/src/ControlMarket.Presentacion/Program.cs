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
            var detalle = ex.InnerException?.Message ?? ex.Message;
            var sqlEx = ex as Microsoft.Data.SqlClient.SqlException ?? ex.InnerException as Microsoft.Data.SqlClient.SqlException;
            var conexion = sqlEx == null || sqlEx.Number is 2 or 53 or 4060 or 18456 or -1 or 26;
            MessageBox.Show(
                (conexion
                    ? "No se pudo conectar con SQL Server.\n\nRevise la cadena de conexión en appsettings.json."
                    : "Se conectó a SQL Server, pero la base de datos no tiene la estructura esperada.\n\nEjecute database/01_db_ControlMarket_integrante2.sql y luego database/02_ajustes_integrante3.sql.") +
                "\n\nDetalle: " + detalle,
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
