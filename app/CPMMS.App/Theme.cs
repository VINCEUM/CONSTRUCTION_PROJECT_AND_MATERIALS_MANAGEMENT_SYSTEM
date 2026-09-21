using System.Drawing;

namespace CPMMS.App;

/// <summary>One place for the colours and fonts, so every screen matches.</summary>
public static class Theme
{
    public static readonly Color Ink        = ColorTranslator.FromHtml("#122A33");
    public static readonly Color InkSoft    = ColorTranslator.FromHtml("#3E555E");
    public static readonly Color Muted      = ColorTranslator.FromHtml("#7E9099");
    public static readonly Color Surface    = ColorTranslator.FromHtml("#F7F9F9");
    public static readonly Color Line       = ColorTranslator.FromHtml("#E1E7E9");
    public static readonly Color Nav        = ColorTranslator.FromHtml("#16262E");
    public static readonly Color NavHover   = ColorTranslator.FromHtml("#223A44");
    public static readonly Color Accent     = ColorTranslator.FromHtml("#1F7A6B");
    public static readonly Color AccentSoft = ColorTranslator.FromHtml("#E7F4EF");
    public static readonly Color Good       = ColorTranslator.FromHtml("#2A7B50");
    public static readonly Color Warn       = ColorTranslator.FromHtml("#B4801D");
    public static readonly Color Danger     = ColorTranslator.FromHtml("#C05248");

    public static readonly Font H1    = new("Segoe UI Semibold", 16F);
    public static readonly Font H2    = new("Segoe UI Semibold", 11F);
    public static readonly Font Body  = new("Segoe UI", 9.75F);
    public static readonly Font Big   = new("Segoe UI Semibold", 20F);
    public static readonly Font Small = new("Segoe UI", 8.25F);

    /// <summary>Consistent look for every data grid in the app.</summary>
    public static void Style(DataGridView grid)
    {
        grid.BorderStyle = BorderStyle.None;
        grid.BackgroundColor = Color.White;
        grid.GridColor = Line;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Surface;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 8.5F);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 6, 6, 6);
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersHeight = 34;
        grid.RowTemplate.Height = 30;
        grid.DefaultCellStyle.SelectionBackColor = AccentSoft;
        grid.DefaultCellStyle.SelectionForeColor = Ink;
        grid.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 252);
        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }
}
