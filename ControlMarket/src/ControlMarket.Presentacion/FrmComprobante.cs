namespace ControlMarket.Presentacion;

public partial class FrmComprobante : Form
{
    public FrmComprobante()
    {
        InitializeComponent();
        btnNuevaVenta.Click += (s, e) => Close();
        AcceptButton = btnNuevaVenta;
    }

    public FrmComprobante(string textoComprobante) : this()
    {
        textBox1.Text = textoComprobante.Replace("\n", Environment.NewLine).Replace("\r\r", "\r");
        textBox1.SelectionStart = 0;
    }
}
