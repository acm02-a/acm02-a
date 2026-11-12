using ControlMarket.Entidades;

namespace ControlMarket.Presentacion;

public partial class FrmCategoriasProveedores : Form
{
    private List<Categoria> _categorias = new();
    private List<Proveedor> _proveedores = new();
    private int _idCategoriaSeleccionada;
    private int _idProveedorSeleccionado;

    public FrmCategoriasProveedores()
    {
        InitializeComponent();
        btnGuardarCategorias.Click += (s, e) => GuardarCategoria();
        btnGuardarProveedores.Click += (s, e) => GuardarProveedor();
        dgvCategorias.SelectionChanged += (s, e) => MostrarCategoria();
        dgvProveedores.SelectionChanged += (s, e) => MostrarProveedor();
        Load += (s, e) =>
        {
            CargarCategorias();
            CargarProveedores();
            LimpiarCamposCategoria();
            LimpiarCamposProveedor();
        };
    }

    // ---------------- Categorías ----------------
    private void CargarCategorias()
    {
        Ui.Ejecutar(() =>
        {
            _categorias = App.Catalogo.ListarCategorias();
            Ui.Enlazar(dgvCategorias, _categorias,
                Ui.Col("IdCategoria", "ID"), Ui.Col("Nombre", "Nombre"), Ui.Col("Descripcion", "Descripción"));
        });
    }

    private void MostrarCategoria()
    {
        var i = Ui.FilaSeleccionada(dgvCategorias);
        if (i < 0 || i >= _categorias.Count) return;
        var c = _categorias[i];
        _idCategoriaSeleccionada = c.IdCategoria;
        txtNombre.Text = c.Nombre;
        txtDescripcion.Text = c.Descripcion;
        Ui.MostrarAuditoria(c, txtCreatedByCat, txtCreatedAtCat, txtModifiedByCat, txtModifiedAtCat);
    }

    private void GuardarCategoria()
    {
        if (!Ui.Ejecutar(() => App.Catalogo.GuardarCategoria(_idCategoriaSeleccionada, txtNombre.Text, txtDescripcion.Text))) return;
        Ui.Info("Categoría guardada correctamente.");
        CargarCategorias();
        LimpiarCamposCategoria();
    }

    private void LimpiarCamposCategoria()
    {
        _idCategoriaSeleccionada = 0;
        txtNombre.Clear();
        txtDescripcion.Clear();
        Ui.MostrarAuditoria(null, txtCreatedByCat, txtCreatedAtCat, txtModifiedByCat, txtModifiedAtCat);
        dgvCategorias.ClearSelection();
    }

    // ---------------- Proveedores ----------------
    private void CargarProveedores()
    {
        Ui.Ejecutar(() =>
        {
            _proveedores = App.Catalogo.ListarProveedores();
            Ui.Enlazar(dgvProveedores, _proveedores,
                Ui.Col("Ruc", "RUC"), Ui.Col("RazonSocial", "Razón social"), Ui.Col("Telefono", "Teléfono"));
        });
    }

    private void MostrarProveedor()
    {
        var i = Ui.FilaSeleccionada(dgvProveedores);
        if (i < 0 || i >= _proveedores.Count) return;
        var p = _proveedores[i];
        _idProveedorSeleccionado = p.IdProveedor;
        txtRUC.Text = p.Ruc;
        txtRazonSocial.Text = p.RazonSocial;
        txtTelefono.Text = p.Telefono;
        Ui.MostrarAuditoria(p, txtCreatedByProv, txtCreatedAtProv, txtModifiedByProv, txtModifiedAtProv);
    }

    private void GuardarProveedor()
    {
        if (!Ui.Ejecutar(() => App.Catalogo.GuardarProveedor(_idProveedorSeleccionado, txtRUC.Text, txtRazonSocial.Text, txtTelefono.Text))) return;
        Ui.Info("Proveedor guardado correctamente.");
        CargarProveedores();
        LimpiarCamposProveedor();
    }

    private void LimpiarCamposProveedor()
    {
        _idProveedorSeleccionado = 0;
        txtRUC.Clear();
        txtRazonSocial.Clear();
        txtTelefono.Clear();
        Ui.MostrarAuditoria(null, txtCreatedByProv, txtCreatedAtProv, txtModifiedByProv, txtModifiedAtProv);
        dgvProveedores.ClearSelection();
    }

    private void btnNuevoCategorias_Click(object sender, EventArgs e)
    {
        LimpiarCamposCategoria();
        txtNombre.Focus();
    }

    private void btnNuevoProveedores_Click(object sender, EventArgs e)
    {
        LimpiarCamposProveedor();
        txtRUC.Focus();
    }
}
