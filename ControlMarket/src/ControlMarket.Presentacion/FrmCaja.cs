using ControlMarket.Entidades;
using ControlMarket.Negocio;
using ControlMarket.Negocio.Servicios;

namespace ControlMarket.Presentacion;

/// <summary>Punto de venta. Atajos: F2 Buscar · F4 Quitar · F8 Cobrar · F10 Cancelar.</summary>
public partial class FrmCaja : Form
{
    private readonly Carrito _carrito = new();
    private decimal _igv = 18m;

    public FrmCaja()
    {
        InitializeComponent();
        KeyPreview = true;
        cmbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
        foreach (var t in new[] { txtSubtotal, txtIGV, txtTotal, txtVuelto }) { t.ReadOnly = true; t.TextAlign = HorizontalAlignment.Right; }
        txtCantidad.Text = "1";
        btnCobrar.Text = "Cobrar (F8)";

        btnAgregar.Click += (s, e) => Agregar();
        btnQuitarProducto.Click += (s, e) => Quitar();
        btnCobrar.Click += (s, e) => Cobrar();
        txtCodigo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Agregar(); e.SuppressKeyPress = true; } };
        txtCantidad.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Agregar(); e.SuppressKeyPress = true; } };
        txtMontoRecibido.TextChanged += (s, e) => CalcularVuelto();
        cmbMetodoPago.SelectedIndexChanged += (s, e) => ActualizarCobro();
        rbtnBoleta.CheckedChanged += (s, e) => ActualizarCobro();
        KeyDown += Atajos;

        Load += (s, e) =>
        {
            lblUsuario.Text = $"Cajero: {App.Sesion.Usuario?.Username}";
            lblFechaHora.Text = DateTime.Now.ToString(Ui.FormatoFechaHora);
            Ui.Ejecutar(() =>
            {
                _igv = App.Ventas.PorcentajeIgv;
                label2.Text = $"IGV ({_igv:0.##}%):";
                cmbMetodoPago.Items.Clear();
                cmbMetodoPago.DisplayMember = nameof(MetodoPago.Nombre);
                cmbMetodoPago.ValueMember = nameof(MetodoPago.IdMetodoPago);
                cmbMetodoPago.DataSource = App.Ventas.ListarMetodosPago();
            });
            rbtnBoleta.Checked = true;
            Refrescar();
            txtCodigo.Focus();
        };
    }

    private void Atajos(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.F2: txtCodigo.Focus(); txtCodigo.SelectAll(); break;
            case Keys.F4: Quitar(); break;
            case Keys.F8: Cobrar(); break;
            case Keys.F10: CancelarVenta(); break;
            default: return;
        }
        e.Handled = true;
    }

    /// <summary>RF Registro de productos en caja + Restricción de stock.</summary>
    private void Agregar()
    {
        var ok = Ui.Ejecutar(() =>
        {
            var cantidad = string.IsNullOrWhiteSpace(txtCantidad.Text) ? 1 : Ui.Entero(txtCantidad.Text, "Cantidad");
            App.Ventas.AgregarProducto(_carrito, txtCodigo.Text, cantidad);
        });
        if (ok)
        {
            txtCodigo.Clear();
            txtCantidad.Text = "1";
            Refrescar();
        }
        txtCodigo.Focus();
        txtCodigo.SelectAll();
    }

    private void Quitar()
    {
        var i = Ui.FilaSeleccionada(dataGridView1);
        if (i < 0 || i >= _carrito.Items.Count) return;
        _carrito.Quitar(_carrito.Items[i].IdProducto);
        Refrescar();
    }

    private void CancelarVenta()
    {
        if (_carrito.EstaVacio) return;
        if (MessageBox.Show("¿Cancelar la venta en curso?", "Caja", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        _carrito.Vaciar();
        LimpiarCobro();
        Refrescar();
    }

    private void btnCancelarVenta_Click(object sender, EventArgs e) => CancelarVenta();

    /// <summary>RF Cálculo de importes en tiempo real.</summary>
    private void Refrescar()
    {
        Ui.Enlazar(dataGridView1, _carrito.Items,
            Ui.Col("CodigoBarras", "Código"), Ui.Col("Producto", "Producto"), Ui.Col("Cantidad", "Cantidad", "N0"),
            Ui.Col("PrecioUnitario", "P. unitario", "N2"), Ui.Col("Importe", "Importe", "N2"));
        if (dataGridView1.Rows.Count > 0)
        {
            var ultima = dataGridView1.Rows[^1];
            ultima.Selected = true;
            dataGridView1.CurrentCell = ultima.Cells[0];
        }
        var (sub, igv, total) = _carrito.Totales(_igv);
        txtSubtotal.Text = sub.ToString("N2");
        txtIGV.Text = igv.ToString("N2");
        txtTotal.Text = total.ToString("N2");
        CalcularVuelto();
    }

    private bool EsEfectivo => cmbMetodoPago.SelectedValue is int id && id == MetodoPago.Efectivo;

    private void ActualizarCobro()
    {
        txtMontoRecibido.Enabled = EsEfectivo;
        label5.Text = rbtnBoleta.Checked ? "DNI:" : "RUC:";
        txtDNI_RUC.MaxLength = rbtnBoleta.Checked ? 8 : 11;
        CalcularVuelto();
    }

    /// <summary>RF Métodos de pago y vuelto.</summary>
    private void CalcularVuelto()
    {
        if (!EsEfectivo) { txtVuelto.Text = "0.00"; return; }
        try
        {
            var recibido = Ui.Decimal(txtMontoRecibido.Text, "Monto recibido");
            var vuelto = recibido - _carrito.Total;
            txtVuelto.Text = vuelto >= 0 ? vuelto.ToString("N2") : "";
        }
        catch (NegocioException) { txtVuelto.Text = ""; }
    }

    /// <summary>RF Emisión de comprobantes + Descuento de stock (FEFO). Todo o nada (transacción).</summary>
    private void Cobrar()
    {
        Venta venta = null;
        var ok = Ui.Ejecutar(() =>
        {
            if (cmbMetodoPago.SelectedValue is not int idMetodo) throw new NegocioException("Seleccione un método de pago.");
            var recibido = EsEfectivo ? Ui.Decimal(txtMontoRecibido.Text, "Monto recibido") : 0m;
            var tipo = rbtnFactura.Checked ? Comprobante.Factura : Comprobante.Boleta;
            venta = App.Ventas.Cobrar(_carrito, idMetodo, tipo, txtDNI_RUC.Text, recibido);
        });
        if (!ok) return;

        string texto = null;
        Ui.Ejecutar(() => texto = App.Ventas.GenerarTextoComprobante(venta.IdVenta));
        using (var f = new FrmComprobante(texto ?? $"Venta {venta.IdVenta} registrada."))
            f.ShowDialog(this);

        LimpiarCobro();
        Refrescar();
        txtCodigo.Focus();
    }

    private void LimpiarCobro()
    {
        txtDNI_RUC.Clear();
        txtMontoRecibido.Clear();
        txtVuelto.Clear();
        rbtnBoleta.Checked = true;
        if (cmbMetodoPago.Items.Count > 0) cmbMetodoPago.SelectedIndex = 0;
    }
}
