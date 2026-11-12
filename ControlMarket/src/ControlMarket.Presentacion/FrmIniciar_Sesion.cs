namespace ControlMarket.Presentacion;

public partial class FrmIniciar_Sesion : Form
{
    public FrmIniciar_Sesion()
    {
        InitializeComponent();
        txtContrasenia.UseSystemPasswordChar = true;
        AcceptButton = btnIngresar;
        CancelButton = btnSalida;
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void btnIngresar_Click(object sender, EventArgs e)
    {
        var ok = Ui.Ejecutar(() => App.Usuarios.IniciarSesion(txtUsuario.Text, txtContrasenia.Text));
        if (ok)
        {
            MonitorInactividad.Instancia.Reiniciar();
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            txtContrasenia.Clear();
            txtContrasenia.Focus();
        }
    }

    private void btnSalida_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
