using ControlMarket.Entidades;
using ControlMarket.Negocio;

namespace ControlMarket.Presentacion;

public partial class FrmProductos : Form
{
    private List<Producto> _productos = new();
    private int _idSeleccionado;

    public FrmProductos()
    {
        InitializeComponent();
        cbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
        btnBuscar.Click += (s, e) => Cargar();
        txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Cargar(); e.SuppressKeyPress = true; } };
        btnGuardar.Click += (s, e) => Guardar();
        dataGridView1.SelectionChanged += (s, e) => MostrarSeleccionado();
        Load += (s, e) =>
        {
            Ui.Ejecutar(() =>
            {
                cbCategoria.Items.Clear();
                cbCategoria.DisplayMember = nameof(Categoria.Nombre);
                cbCategoria.ValueMember = nameof(Categoria.IdCategoria);
                cbCategoria.DataSource = App.Catalogo.ListarCategorias();
            });
            Cargar();
            LimpiarCampos();
        };
    }

    private void Cargar()
    {
        Ui.Ejecutar(() =>
        {
            _productos = App.Catalogo.ListarProductos(txtBuscar.Text);
            Ui.Enlazar(dataGridView1, _productos.Select(p => new
                {
                    p.CodigoBarras, p.Nombre, Categoria = p.Categoria?.Nombre, p.PrecioCompra, p.PrecioVenta,
                    p.StockMinimo, Activo = p.Activo ? "Sí" : "No"
                }),
                Ui.Col("CodigoBarras", "Código de barras"), Ui.Col("Nombre", "Nombre"), Ui.Col("Categoria", "Categoría"),
                Ui.Col("PrecioCompra", "P. compra", "N2"), Ui.Col("PrecioVenta", "P. venta", "N2"),
                Ui.Col("StockMinimo", "Stock mín.", "N0"), Ui.Col("Activo", "Activo"));
        });
    }

    private void MostrarSeleccionado()
    {
        var i = Ui.FilaSeleccionada(dataGridView1);
        if (i < 0 || i >= _productos.Count) return;
        var p = _productos[i];
        _idSeleccionado = p.IdProducto;
        txtCodigoBarra.Text = p.CodigoBarras;
        txtNombre.Text = p.Nombre;
        cbCategoria.SelectedValue = p.IdCategoria;
        txtPrecio.Text = p.PrecioCompra.ToString("0.00");
        txtPrecioVenta.Text = p.PrecioVenta.ToString("0.00");
        txtStock.Text = p.StockMinimo.ToString();
        chkActivo.Checked = p.Activo;
        Ui.MostrarAuditoria(p, txtCreatedBy, txtCreatedAt, txtModifiedBy, txtModifiedAt);
    }

    private void Guardar()
    {
        var ok = Ui.Ejecutar(() =>
        {
            if (cbCategoria.SelectedValue is not int idCategoria) throw new NegocioException("Seleccione una categoría.");
            var p = App.Catalogo.GuardarProducto(_idSeleccionado, txtCodigoBarra.Text, txtNombre.Text, idCategoria,
                Ui.Decimal(txtPrecio.Text, "Precio compra"), Ui.Decimal(txtPrecioVenta.Text, "Precio venta"),
                Ui.Entero(txtStock.Text, "Stock mínimo"), chkActivo.Checked);
            _idSeleccionado = p.IdProducto;
        });
        if (!ok) return;
        Ui.Info("Producto guardado correctamente.");
        var id = _idSeleccionado;
        Cargar();
        var idx = _productos.FindIndex(p => p.IdProducto == id);
        if (idx >= 0)
        {
            dataGridView1.Rows[idx].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[idx].Cells[0];
        }
    }

    private void LimpiarCampos()
    {
        _idSeleccionado = 0;
        txtCodigoBarra.Clear();
        txtNombre.Clear();
        cbCategoria.SelectedIndex = -1;
        txtPrecio.Clear();
        txtPrecioVenta.Clear();
        txtStock.Clear();
        chkActivo.Checked = true;
        Ui.MostrarAuditoria(null, txtCreatedBy, txtCreatedAt, txtModifiedBy, txtModifiedAt);
        dataGridView1.ClearSelection();
    }

    private void btnCancelar_Click(object sender, EventArgs e) => LimpiarCampos();

    private void btnNuevo_Click(object sender, EventArgs e)
    {
        LimpiarCampos();
        txtCodigoBarra.Focus();
    }
}
