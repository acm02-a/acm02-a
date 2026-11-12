using System.Windows.Forms.DataVisualization.Charting;

namespace ControlMarket.Presentacion;

public partial class FrmReportesCriticos : Form
{
    public FrmReportesCriticos()
    {
        InitializeComponent();
        Load += (s, e) => Cargar();
    }

    /// <summary>RF Reporte de productos críticos y Reporte estadístico gráfico (Top 5).</summary>
    private void Cargar()
    {
        Ui.Ejecutar(() =>
        {
            var criticos = App.Reportes.ProductosCriticos();
            Ui.Enlazar(dataGridView1, criticos,
                Ui.Col("Producto", "Producto"), Ui.Col("Motivo", "Motivo"),
                Ui.Col("Stock", "Stock", "N0"), Ui.Col("Vence", "Vence", Ui.FormatoFecha));
            Ui.Colorear(dataGridView1, i => criticos[i].Vence.HasValue ? Ui.Amarillo : Ui.Rojo);

            var top = App.Reportes.TopProductos(5);
            chart1.Series.Clear();
            chart1.Legends.Clear();
            var serie = new Series("Unidades vendidas")
            {
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true,
                Color = Color.FromArgb(68, 114, 196)
            };
            // Bar dibuja de abajo hacia arriba: se invierte para que el más vendido quede arriba.
            foreach (var t in Enumerable.Reverse(top)) serie.Points.AddXY(t.Producto, t.Unidades);
            chart1.Series.Add(serie);
            var area = chart1.ChartAreas[0];
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.Interval = 1;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
        });
    }
}
