using ControlMarket.Entidades;
using ControlMarket.Entidades.Dtos;
using ControlMarket.Negocio;

namespace ControlMarket.Presentacion;

public partial class FrmLotesStock : Form
{
    private List<StockProductoDto> _stock = new();

    public FrmLotesStock()
    {
        InitializeComponent();
        cbProducto.DropDownStyle = ComboBoxStyle.DropDownList;
        cbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
        dtpFechaIngreso.Format = DateTimePickerFormat.Short;
        dtpFechaVencimiento.Format = DateTimePickerFormat.Short;

        btnRegistrar.Click += (s, e) => Registrar();
        btnBuscar.Click += (s, e) => CargarStock();
        txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { CargarStock(); e.SuppressKeyPress = true; } };
        Load += (s, e) =>
        {
            Ui.Ejecutar(() =>
            {
                cbProducto.Items.Clear();
                cbProducto.DisplayMember = nameof(Producto.Nombre);
                cbProducto.ValueMember = nameof(Producto.IdProducto);
                cbProducto.DataSource = App.Catalogo.ListarProductos(soloActivos: true);

                cbProveedor.Items.Clear();
                cbProveedor.DisplayMember = nameof(Proveedor.RazonSocial);
                cbProveedor.ValueMember = nameof(Proveedor.IdProveedor);
                cbProveedor.DataSource = App.Catalogo.ListarProveedores();
            });
            LimpiarCampos();
            CargarStock();
        };
    }

    private void Registrar()
    {
        Lote lote = null;
        var ok = Ui.Ejecutar(() =>
        {
            if (cbProducto.SelectedValue is not int idProducto) throw new NegocioException("Seleccione un producto.");
            if (cbProveedor.SelectedValue is not int idProveedor) throw new NegocioException("Seleccione un proveedor.");
            lote = App.Inventario.RegistrarIngreso(idProducto, idProveedor, Ui.Entero(txtCantidad.Text, "Cantidad"),
                dtpFechaIngreso.Value, dtpFechaVencimiento.Value);
        });
        if (!ok) return;
        Ui.Info($"Lote L-{lote.IdLote:D4} registrado correctamente.");
        LimpiarCampos();
        Ui.MostrarAuditoria(lote, txtCreatedBy, txtCreatedAt, txtModifiedBy, txtModifiedAt);
        CargarStock();
    }

    /// <summary>Consulta de stock: rojo = bajo mínimo, amarillo = por vencer.</summary>
    private void CargarStock()
    {
        Ui.Ejecutar(() =>
        {
            _stock = App.Inventario.ConsultarStock(txtBuscar.Text);
            Ui.Enlazar(dataGridView1, _stock,
                Ui.Col("CodigoBarras", "Código"), Ui.Col("Producto", "Producto"),
                Ui.Col("StockActual", "Stock actual", "N0"), Ui.Col("StockMinimo", "Stock mínimo", "N0"),
                Ui.Col("ProximoVencimiento", "Próximo vencimiento", Ui.FormatoFecha), Ui.Col("Estado", "Estado"));
            Ui.Colorear(dataGridView1, i => _stock[i].Estado switch
            {
                "Bajo mínimo" => Ui.Rojo,
                "Por vencer" => Ui.Amarillo,
                _ => null
            });
        });
    }

    private void LimpiarCampos()
    {
        cbProducto.SelectedIndex = -1;
        cbProveedor.SelectedIndex = -1;
        txtCantidad.Clear();
        dtpFechaIngreso.Value = DateTime.Now;
        dtpFechaVencimiento.Value = DateTime.Now.AddMonths(6);
        Ui.MostrarAuditoria(null, txtCreatedBy, txtCreatedAt, txtModifiedBy, txtModifiedAt);
    }

    private void btnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarCampos();
        cbProducto.Focus();
    }
}
