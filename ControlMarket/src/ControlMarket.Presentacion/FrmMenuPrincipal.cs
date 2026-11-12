using ControlMarket.Negocio.Seguridad;

namespace ControlMarket.Presentacion;

public partial class FrmMenuPrincipal : Form
{
    private readonly System.Windows.Forms.Timer _reloj = new() { Interval = 15_000 };

    /// <summary>true si el usuario cerró sesión (manual o por inactividad): Program vuelve a mostrar el login.</summary>
    public bool VolverAlLogin { get; private set; }

    public FrmMenuPrincipal()
    {
        InitializeComponent();
        StartPosition = FormStartPosition.CenterScreen;

        usuariosToolStripMenuItem.Click += btnUsuarios_Click;
        parámetrosGeneralesToolStripMenuItem.Click += btnParametros_Click;
        productosToolStripMenuItem.Click += btnProductos_Click;
        categoríasToolStripMenuItem.Click += btnCategoriasProveedores_Click;
        lotesYStockToolStripMenuItem.Click += btnLotesStock_Click;
        cajaToolStripMenuItem.Click += btnCaja_Click;
        ventasYRentabilidadToolStripMenuItem.Click += btnReportes_Click;
        productosCríticosYTop5ToolStripMenuItem.Click += (s, e) => AbrirFormulario(Modulo.Reportes, () => new FrmReportesCriticos());

        _reloj.Tick += (s, e) => VerificarInactividad();
        Load += (s, e) => { AplicarPermisos(); _reloj.Start(); };
        Activated += (s, e) => CargarAlertas(); // también refresca al volver de cada módulo
        FormClosed += (s, e) => _reloj.Dispose();
    }

    /// <summary>Muestra solo los módulos permitidos para el rol (polimorfismo de Perfil).</summary>
    private void AplicarPermisos()
    {
        var perfil = App.Sesion.Perfil!;
        lblUsuario.Text = $"Usuario: {perfil.Usuario.Username}";
        lblRol.Text = $"Rol: {perfil.NombreRol}";
        lblInactividad.Text = $"Cierre automático por inactividad: {App.MinutosInactividad} min";
        ActualizarHora();

        void Permitir(Modulo m, params ToolStripItem[] items) { foreach (var i in items) i.Visible = perfil.PuedeAcceder(m); }
        void PermitirBtn(Modulo m, Button b) => b.Visible = perfil.PuedeAcceder(m);

        PermitirBtn(Modulo.Usuarios, btnUsuarios);
        PermitirBtn(Modulo.Parametros, btnParametros);
        PermitirBtn(Modulo.Productos, btnProductos);
        PermitirBtn(Modulo.CategoriasProveedores, btnCategoriasProveedores);
        PermitirBtn(Modulo.LotesStock, btnLotesStock);
        PermitirBtn(Modulo.Caja, btnCaja);
        PermitirBtn(Modulo.Reportes, btnReportes);

        Permitir(Modulo.Usuarios, usuariosToolStripMenuItem);
        Permitir(Modulo.Parametros, parámetrosGeneralesToolStripMenuItem);
        Permitir(Modulo.Productos, productosToolStripMenuItem);
        Permitir(Modulo.CategoriasProveedores, categoríasToolStripMenuItem);
        Permitir(Modulo.LotesStock, lotesYStockToolStripMenuItem);
        Permitir(Modulo.Caja, cajaToolStripMenuItem);
        Permitir(Modulo.Reportes, ventasYRentabilidadToolStripMenuItem, productosCríticosYTop5ToolStripMenuItem);

        seguridadToolStripMenuItem.Visible = perfil.PuedeAcceder(Modulo.Usuarios) || perfil.PuedeAcceder(Modulo.Parametros);
        inventarioToolStripMenuItem.Visible = perfil.PuedeAcceder(Modulo.Productos) || perfil.PuedeAcceder(Modulo.LotesStock);
        ventasToolStripMenuItem.Visible = perfil.PuedeAcceder(Modulo.Caja);
        reportesToolStripMenuItem.Visible = perfil.PuedeAcceder(Modulo.Reportes);

        // Reubica los botones visibles para que no queden huecos.
        var visibles = gbModulos.Controls.OfType<Button>().OrderBy(b => b.Top).ToList();
        var tops = visibles.Select(b => b.Top).ToList();
        var i = 0;
        foreach (var b in visibles.Where(b => b.Visible)) b.Top = tops[i++];
    }

    /// <summary>RF Alertas de stock mínimo (filas rojas) y de vencimiento (filas amarillas).</summary>
    private void CargarAlertas()
    {
        if (!App.Sesion.Activa) return;
        Ui.Ejecutar(() =>
        {
            var stock = App.Inventario.AlertasStockMinimo();
            Ui.Enlazar(dataGridView1, stock,
                Ui.Col("CodigoBarras", "Código"), Ui.Col("Producto", "Producto"),
                Ui.Col("StockActual", "Stock actual", "N0"), Ui.Col("StockMinimo", "Stock mínimo", "N0"));
            Ui.Colorear(dataGridView1, _ => Ui.Rojo);
            groupBox1.Text = $"Alertas de stock mínimo ({stock.Count})";

            var dias = App.Parametros.Obtener().DiasAlertaVencimiento;
            var lotes = App.Inventario.AlertasVencimiento();
            Ui.Enlazar(dataGridView2, lotes,
                Ui.Col("Lote", "Lote"), Ui.Col("Producto", "Producto"),
                Ui.Col("Cantidad", "Cantidad", "N0"), Ui.Col("FechaVencimiento", "Vencimiento", Ui.FormatoFecha));
            Ui.Colorear(dataGridView2, _ => Ui.Amarillo);
            groupBox2.Text = $"Lotes próximos a vencer - {dias} días ({lotes.Count})";
        });
    }

    private void ActualizarHora() => lblFechaHora.Text = DateTime.Now.ToString(Ui.FormatoFechaHora);

    private void VerificarInactividad()
    {
        ActualizarHora();
        if (MonitorInactividad.Instancia.TiempoInactivo.TotalMinutes < App.MinutosInactividad) return;

        _reloj.Stop();
        foreach (var f in Application.OpenForms.Cast<Form>().Where(f => f != this).ToList()) f.Close();
        MessageBox.Show("La sesión se cerró por inactividad.", "ControlMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);
        CerrarSesion();
    }

    private void CerrarSesion()
    {
        App.Usuarios.CerrarSesion();
        VolverAlLogin = true;
        Close();
    }

    private void AbrirFormulario(Modulo modulo, Func<Form> crear)
    {
        if (!Ui.Ejecutar(() => App.Sesion.Exigir(modulo))) return;
        using var f = crear();
        f.StartPosition = FormStartPosition.CenterParent;
        f.ShowDialog(this);
    }

    private void btnUsuarios_Click(object sender, EventArgs e) => AbrirFormulario(Modulo.Usuarios, () => new FrmUsuarios());
    private void btnParametros_Click(object sender, EventArgs e) => AbrirFormulario(Modulo.Parametros, () => new FrmParametros());
    private void btnProductos_Click(object sender, EventArgs e) => AbrirFormulario(Modulo.Productos, () => new FrmProductos());
    private void btnCategoriasProveedores_Click(object sender, EventArgs e) => AbrirFormulario(Modulo.CategoriasProveedores, () => new FrmCategoriasProveedores());
    private void btnLotesStock_Click(object sender, EventArgs e) => AbrirFormulario(Modulo.LotesStock, () => new FrmLotesStock());
    private void btnCaja_Click(object sender, EventArgs e) => AbrirFormulario(Modulo.Caja, () => new FrmCaja());
    private void btnReportes_Click(object sender, EventArgs e) => AbrirFormulario(Modulo.Reportes, () => new FrmReportesVentas());

    private void cerrarSesiónToolStripMenuItem_Click(object sender, EventArgs e) => CerrarSesion();

    private void sAlirToolStripMenuItem_Click(object sender, EventArgs e)
    {
        App.Usuarios.CerrarSesion();
        VolverAlLogin = false;
        Close();
    }
}
