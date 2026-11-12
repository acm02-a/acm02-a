namespace ControlMarket.Presentacion;

public partial class FrmParametros : Form
{
    public FrmParametros()
    {
        InitializeComponent();
        btnGuardar.Click += (s, e) => Guardar();
        Load += (s, e) => Cargar();
    }

    private void Cargar()
    {
        Ui.Ejecutar(() =>
        {
            var p = App.Parametros.Obtener();
            txtRazonSocial.Text = p.RazonSocial;
            txtRUC.Text = p.Ruc;
            txtDireccion.Text = p.Direccion;
            txtIGV.Text = p.PorcentajeIgv.ToString("0.##");
            txtDiasVencimiento.Text = p.DiasAlertaVencimiento.ToString();
            Ui.MostrarAuditoria(p.IdParametro == 0 ? null : p, txtCreatedByPar, txtCreatedAtPar, txtModifiedByPar, txtModifiedAtPar);
        });
    }

    private void Guardar()
    {
        var ok = Ui.Ejecutar(() => App.Parametros.Guardar(
            txtRazonSocial.Text, txtRUC.Text, txtDireccion.Text,
            Ui.Decimal(txtIGV.Text, "IGV"), Ui.Entero(txtDiasVencimiento.Text, "Días de aviso")));
        if (!ok) return;
        Ui.Info("Parámetros guardados correctamente.");
        Cargar();
    }

    private void LimpiarCampos()
    {
        txtRazonSocial.Clear();
        txtRUC.Clear();
        txtDireccion.Clear();
        txtIGV.Clear();
        txtDiasVencimiento.Clear();
    }

    /// <summary>Cancelar descarta los cambios no guardados y vuelve a mostrar los valores actuales.</summary>
    private void btnCancelar_Click(object sender, EventArgs e)
    {
        LimpiarCampos();
        Cargar();
    }
}
