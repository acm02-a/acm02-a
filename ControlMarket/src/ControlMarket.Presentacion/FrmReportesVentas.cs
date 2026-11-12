using ControlMarket.Entidades;

namespace ControlMarket.Presentacion;

public partial class FrmReportesVentas : Form
{
    public FrmReportesVentas()
    {
        InitializeComponent();
        cmbCajero.DropDownStyle = ComboBoxStyle.DropDownList;
        dateTimePicker1.Format = DateTimePickerFormat.Short;
        dateTimePicker2.Format = DateTimePickerFormat.Short;
        btnGenera.Text = "Generar";
        btnGenera.Click += (s, e) => Generar();
        rbtnProducto.CheckedChanged += (s, e) => { if (rbtnProducto.Checked) Generar(); };
        rbtnCategoria.CheckedChanged += (s, e) => { if (rbtnCategoria.Checked) Generar(); };

        Load += (s, e) =>
        {
            dateTimePicker1.Value = DateTime.Today.AddDays(-6);
            dateTimePicker2.Value = DateTime.Today;
            Ui.Ejecutar(() =>
            {
                var cajeros = new List<Usuario> { new() { IdUsuario = 0, Username = "Todos" } };
                cajeros.AddRange(App.Usuarios.ListarPorRol(Rol.Cajero));
                cmbCajero.Items.Clear();
                cmbCajero.DisplayMember = nameof(Usuario.Username);
                cmbCajero.ValueMember = nameof(Usuario.IdUsuario);
                cmbCajero.DataSource = cajeros;
            });
            rbtnCategoria.Checked = true;
            Generar();
        };
    }

    private int? CajeroSeleccionado => cmbCajero.SelectedValue is int id && id > 0 ? id : null;

    /// <summary>RF Reporte de ventas por fechas y Reporte de rentabilidad.</summary>
    private void Generar()
    {
        Ui.Ejecutar(() =>
        {
            var desde = dateTimePicker1.Value;
            var hasta = dateTimePicker2.Value;

            var ventas = App.Reportes.VentasPorFecha(desde, hasta, CajeroSeleccionado);
            var filasVentas = ventas.Select(v => new { Fecha = v.Fecha.ToString(Ui.FormatoFecha), v.Cajero, v.NumeroVentas, v.Total }).ToList();
            if (ventas.Count > 0)
                filasVentas.Add(new { Fecha = "Total", Cajero = "", NumeroVentas = ventas.Sum(v => v.NumeroVentas), Total = ventas.Sum(v => v.Total) });
            Ui.Enlazar(dataGridView1, filasVentas,
                Ui.Col("Fecha", "Fecha"), Ui.Col("Cajero", "Cajero"),
                Ui.Col("NumeroVentas", "N.º ventas", "N0"), Ui.Col("Total", "Total (S/)", "N2"));
            NegritaUltimaFila(dataGridView1, ventas.Count > 0);

            var rent = App.Reportes.Rentabilidad(desde, hasta, rbtnCategoria.Checked, CajeroSeleccionado);
            var filasRent = rent.ToList();
            if (rent.Count > 0)
                filasRent.Add(new Entidades.Dtos.RentabilidadDto("Total", rent.Sum(r => r.Ingreso), rent.Sum(r => r.Costo), rent.Sum(r => r.GananciaNeta)));
            Ui.Enlazar(dataGridView2, filasRent,
                Ui.Col("Grupo", rbtnCategoria.Checked ? "Categoría" : "Producto"),
                Ui.Col("Ingreso", "Ingreso", "N2"), Ui.Col("Costo", "Costo", "N2"), Ui.Col("GananciaNeta", "Ganancia neta", "N2"));
            NegritaUltimaFila(dataGridView2, rent.Count > 0);
        });
    }

    private static void NegritaUltimaFila(DataGridView grid, bool hayTotal)
    {
        if (!hayTotal || grid.Rows.Count == 0) return;
        grid.Rows[^1].DefaultCellStyle.Font = new Font(grid.Font, FontStyle.Bold);
    }
}
