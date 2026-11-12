using System.Globalization;
using ControlMarket.Negocio;

namespace ControlMarket.Presentacion;

/// <summary>Utilidades comunes de la capa de presentación.</summary>
internal static class Ui
{
    public const string FormatoFecha = "dd/MM/yyyy";
    public const string FormatoFechaHora = "dd/MM/yyyy HH:mm";

    /// <summary>Ejecuta una acción mostrando los errores de negocio como advertencia.</summary>
    public static bool Ejecutar(Action accion)
    {
        try
        {
            Cursor.Current = Cursors.WaitCursor;
            accion();
            return true;
        }
        catch (Exception ex)
        {
            MostrarError(ex);
            return false;
        }
        finally
        {
            Cursor.Current = Cursors.Default;
        }
    }

    public static void MostrarError(Exception ex)
    {
        if (ex is NegocioException)
            MessageBox.Show(ex.Message, "ControlMarket", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        else
            MessageBox.Show("Ocurrió un error inesperado:\n" + (ex.InnerException?.Message ?? ex.Message),
                "ControlMarket", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void Info(string mensaje) =>
        MessageBox.Show(mensaje, "ControlMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static decimal Decimal(string texto, string campo)
    {
        var t = (texto ?? "").Trim();
        if (decimal.TryParse(t, NumberStyles.Number, CultureInfo.CurrentCulture, out var v) ||
            decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
            return v;
        throw new NegocioException($"Ingrese un valor numérico válido en {campo}.");
    }

    public static int Entero(string texto, string campo)
    {
        if (int.TryParse((texto ?? "").Trim(), out var v)) return v;
        throw new NegocioException($"Ingrese un número entero válido en {campo}.");
    }

    public static string Fecha(DateTime? f) => f?.ToString(FormatoFechaHora) ?? "";

    /// <summary>Muestra los campos de auditoría de una entidad en los 4 TextBox de solo lectura.</summary>
    public static void MostrarAuditoria(Entidades.EntidadAuditable e, TextBox createdBy, TextBox createdAt, TextBox modifiedBy, TextBox modifiedAt)
    {
        createdBy.Text = e?.CreatedBy ?? "";
        createdAt.Text = e == null ? "" : Fecha(e.CreatedAt);
        modifiedBy.Text = e?.ModifiedBy ?? "";
        modifiedAt.Text = e == null ? "" : Fecha(e.ModifiedAt);
        foreach (var t in new[] { createdBy, createdAt, modifiedBy, modifiedAt }) t.ReadOnly = true;
    }

    /// <summary>Configura una grilla de solo lectura y la enlaza a una lista con columnas explícitas.</summary>
    public static void Enlazar<T>(DataGridView grid, IEnumerable<T> datos, params (string Propiedad, string Titulo, string Formato)[] columnas)
    {
        grid.SuspendLayout();
        grid.AutoGenerateColumns = false;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.RowHeadersVisible = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.DataSource = null;
        grid.Columns.Clear();
        foreach (var (prop, titulo, formato) in columnas)
        {
            var col = new DataGridViewTextBoxColumn { DataPropertyName = prop, HeaderText = titulo, Name = prop };
            if (!string.IsNullOrEmpty(formato))
            {
                col.DefaultCellStyle.Format = formato;
                if (formato.StartsWith("N")) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            grid.Columns.Add(col);
        }
        grid.DataSource = datos.ToList();
        grid.ClearSelection();
        grid.ResumeLayout();
    }

    public static (string, string, string) Col(string propiedad, string titulo, string formato = null) => (propiedad, titulo, formato);

    /// <summary>Índice de la fila seleccionada (o -1).</summary>
    public static int FilaSeleccionada(DataGridView grid) =>
        grid.CurrentRow != null && grid.SelectedRows.Count > 0 ? grid.CurrentRow.Index : -1;

    public static void Colorear(DataGridView grid, Func<int, Color?> colorDeFila)
    {
        foreach (DataGridViewRow row in grid.Rows)
        {
            var c = colorDeFila(row.Index);
            if (c.HasValue) row.DefaultCellStyle.BackColor = c.Value;
        }
    }

    public static readonly Color Rojo = Color.FromArgb(255, 214, 214);
    public static readonly Color Amarillo = Color.FromArgb(255, 243, 205);
}
