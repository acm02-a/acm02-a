using ControlMarket.Entidades;

namespace ControlMarket.Presentacion;

public partial class FrmUsuarios : Form
{
    private List<Usuario> _usuarios = new();
    private int _idSeleccionado;

    public FrmUsuarios()
    {
        InitializeComponent();
        txtContrasena.UseSystemPasswordChar = true;
        cbRol.DropDownStyle = ComboBoxStyle.DropDownList;

        btnBuscar.Click += (s, e) => Cargar();
        textBox1.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Cargar(); e.SuppressKeyPress = true; } };
        btnNuevo.Click += (s, e) => { LimpiarCampos(); txtNombre.Focus(); };
        btnGuardar.Click += (s, e) => Guardar();
        dgvUsuarios.SelectionChanged += (s, e) => MostrarSeleccionado();
        Load += (s, e) =>
        {
            Ui.Ejecutar(() =>
            {
                cbRol.Items.Clear();
                cbRol.DisplayMember = nameof(Rol.NombreRol);
                cbRol.ValueMember = nameof(Rol.IdRol);
                cbRol.DataSource = App.Usuarios.ListarRoles();
            });
            Cargar();
            LimpiarCampos();
        };
    }

    private void Cargar()
    {
        Ui.Ejecutar(() =>
        {
            _usuarios = App.Usuarios.Listar(textBox1.Text);
            Ui.Enlazar(dgvUsuarios, _usuarios.Select(u => new
                {
                    u.IdUsuario, u.Nombre, u.Username, Rol = u.Rol?.NombreRol, Activo = u.Activo ? "Sí" : "No"
                }),
                Ui.Col("IdUsuario", "ID"), Ui.Col("Nombre", "Nombre"), Ui.Col("Username", "Usuario"),
                Ui.Col("Rol", "Rol"), Ui.Col("Activo", "Activo"));
        });
    }

    private void MostrarSeleccionado()
    {
        var i = Ui.FilaSeleccionada(dgvUsuarios);
        if (i < 0 || i >= _usuarios.Count) return;
        var u = _usuarios[i];
        _idSeleccionado = u.IdUsuario;
        txtNombre.Text = u.Nombre;
        txtUsuario.Text = u.Username;
        txtContrasena.Clear(); // vacío = conservar contraseña actual
        cbRol.SelectedValue = u.IdRol;
        chkActivo.Checked = u.Activo;
        Ui.MostrarAuditoria(u, txtCreatedBy, txtCreatedAt, txtModifiedBy, txtModifiedAt);
    }

    private void Guardar()
    {
        var ok = Ui.Ejecutar(() =>
        {
            if (cbRol.SelectedValue is not int idRol) throw new Negocio.NegocioException("Seleccione un rol.");
            var u = App.Usuarios.Guardar(_idSeleccionado, txtNombre.Text, txtUsuario.Text, txtContrasena.Text, idRol, chkActivo.Checked);
            _idSeleccionado = u.IdUsuario;
        });
        if (!ok) return;
        Ui.Info("Usuario guardado correctamente.");
        var id = _idSeleccionado;
        Cargar();
        SeleccionarPorId(id);
    }

    private void SeleccionarPorId(int id)
    {
        var idx = _usuarios.FindIndex(u => u.IdUsuario == id);
        if (idx < 0) return;
        dgvUsuarios.ClearSelection();
        dgvUsuarios.Rows[idx].Selected = true;
        dgvUsuarios.CurrentCell = dgvUsuarios.Rows[idx].Cells[0];
    }

    private void LimpiarCampos()
    {
        _idSeleccionado = 0;
        txtNombre.Clear();
        txtUsuario.Clear();
        txtContrasena.Clear();
        cbRol.SelectedIndex = -1;
        chkActivo.Checked = true;
        Ui.MostrarAuditoria(null, txtCreatedBy, txtCreatedAt, txtModifiedBy, txtModifiedAt);
        dgvUsuarios.ClearSelection();
    }

    private void btnCancelar_Click(object sender, EventArgs e) => LimpiarCampos();
}
