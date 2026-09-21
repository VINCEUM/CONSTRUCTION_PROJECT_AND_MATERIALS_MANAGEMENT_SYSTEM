using System.ComponentModel;

namespace CPMMS.App;

/// <summary>Small helpers so every screen builds the same kind of card, grid and banner.</summary>
public static class UiKit
{
    public static Panel Card(string label, string value, string? sub = null, Color? valueColor = null)
    {
        var card = new Panel
        {
            BackColor = Color.White,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0, 0, 12, 12),
            Size = new Size(210, 96)
        };
        card.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.Line);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var lblValue = new Label
        {
            Text = value,
            Font = Theme.Big,
            ForeColor = valueColor ?? Theme.Ink,
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 38
        };
        var lblLabel = new Label
        {
            Text = label,
            Font = Theme.Body,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 20
        };
        var lblSub = new Label
        {
            Text = sub ?? "",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 18
        };

        card.Controls.Add(lblSub);
        card.Controls.Add(lblLabel);
        card.Controls.Add(lblValue);
        return card;
    }

    public static Label SectionTitle(string text) => new()
    {
        Text = text,
        Font = Theme.H2,
        ForeColor = Theme.Ink,
        AutoSize = false,
        Dock = DockStyle.Top,
        Height = 30,
        Padding = new Padding(0, 6, 0, 0)
    };

    public static Panel Banner(string text, Color fore, Color back)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = back, Padding = new Padding(12, 0, 12, 0) };
        panel.Controls.Add(new Label
        {
            Text = text,
            ForeColor = fore,
            Font = Theme.Body,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        });
        return panel;
    }

    public static DataGridView Grid()
    {
        var grid = new DataGridView { Dock = DockStyle.Fill };
        Theme.Style(grid);
        return grid;
    }

    /// <summary>Binds a list and declares exactly which columns show, in order.</summary>
    public static void Bind<T>(DataGridView grid, IEnumerable<T> data,
                               params (string Prop, string Header, string? Format)[] columns)
    {
        grid.AutoGenerateColumns = false;
        grid.Columns.Clear();

        foreach (var (prop, header, format) in columns)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                Name = prop,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            if (format is not null)
            {
                column.DefaultCellStyle.Format = format;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            grid.Columns.Add(column);
        }

        grid.DataSource = new BindingList<T>(data.ToList());
    }

    public static Button Secondary(string text)
    {
        var button = new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.InkSoft,
            BackColor = Color.White,
            Height = 30,
            AutoSize = true,
            Padding = new Padding(10, 0, 10, 0),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderColor = Theme.Line;
        return button;
    }
}
