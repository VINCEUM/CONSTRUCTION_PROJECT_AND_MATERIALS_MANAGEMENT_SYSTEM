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
            Size = new Size(240, 100)
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

        // a list that is already a BindingList is bound as it is, so rows added or removed
        // after binding show up; anything else is copied into one
        grid.DataSource = data as BindingList<T> ?? new BindingList<T>(data.ToList());
    }

    // ------------------------------------------------------------ dialog layout
    // The dialogs lay their fields out with a FlowLayoutPanel instead of fixed pixel
    // positions, so a label can never overlap the box after it and nothing is cut off,
    // whatever the text size or the Windows display scaling.

    /// <summary>A dialog size that never exceeds the screen (small laptops), whatever the wanted size.</summary>
    public static Size FitSize(int width, int height)
    {
        var area = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1366, 768);
        return new Size(Math.Min(width, area.Width - 60), Math.Min(height, area.Height - 100));
    }

    public static Label FormTitle(string text) => new()
    {
        Text = text, Font = Theme.H1, ForeColor = Theme.Ink, Dock = DockStyle.Top,
        Height = 48, AutoSize = false, TextAlign = ContentAlignment.MiddleLeft
    };

    public static Label FormSubtitle(string text) => new()
    {
        Text = text, Font = Theme.Body, ForeColor = Theme.Muted, Dock = DockStyle.Top,
        Height = 32, AutoSize = false, TextAlign = ContentAlignment.TopLeft
    };

    /// <summary>Gives a text box, drop-down or date picker the larger input font and a visible border.</summary>
    public static void StyleInput(Control input)
    {
        switch (input)
        {
            case TextBox tb: tb.Font = Theme.Input; tb.BorderStyle = BorderStyle.FixedSingle; break;
            case ComboBox or DateTimePicker or NumericUpDown: input.Font = Theme.Input; break;
            case Label l: l.Font = Theme.Input; l.ForeColor = Theme.Ink; l.AutoSize = true; break;
        }
    }

    /// <summary>One line of "label + input" pairs. A pair with an empty label is just the control (a button).</summary>
    public static FlowLayoutPanel FieldRow(params (string Label, Control Input)[] fields)
    {
        var row = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, Height = 54, WrapContents = false, AutoSize = false,
            FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 6, 0, 6)
        };
        foreach (var (text, input) in fields)
        {
            if (!string.IsNullOrEmpty(text))
                row.Controls.Add(new Label
                {
                    Text = text, Font = Theme.Body, ForeColor = Theme.InkSoft, AutoSize = true,
                    Margin = new Padding(0, 10, 10, 0)
                });
            StyleInput(input);
            input.Margin = input is Button ? new Padding(0, 2, 32, 0) : new Padding(0, input is Label ? 8 : 4, 32, 0);
            row.Controls.Add(input);
        }
        return row;
    }

    /// <summary>The white top block of a dialog. List the parts top to bottom; the height follows.</summary>
    public static Panel HeaderPanel(params Control[] topToBottom)
    {
        var panel = new Panel { Dock = DockStyle.Top, BackColor = Color.White, Padding = new Padding(28, 16, 28, 12) };
        for (var i = topToBottom.Length - 1; i >= 0; i--) panel.Controls.Add(topToBottom[i]);   // docking: last added sits on top
        panel.Height = panel.Padding.Vertical + topToBottom.Sum(c => c.Height);
        return panel;
    }

    /// <summary>
    /// Lays controls out left to right with an even gap, using each control's own
    /// measured width, so toolbar buttons never touch or overlap. Hidden controls
    /// take no space. Returns the x where the next thing can start.
    /// </summary>
    public static int LayoutRow(int x, int y, int gap, params Control[] controls)
    {
        foreach (var c in controls)
        {
            if (!c.Visible) continue;
            c.Location = new Point(x, c is CheckBox ? y + 3 : y);
            x += c.GetPreferredSize(Size.Empty).Width + gap;
        }
        return x;
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
