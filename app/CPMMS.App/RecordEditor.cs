namespace CPMMS.App;

public enum FieldKind { Text, Multiline, Number, Money, Date, Combo, Password }

public sealed class FieldSpec
{
    public string Key { get; init; } = "";
    public string Label { get; init; } = "";
    public FieldKind Kind { get; init; } = FieldKind.Text;
    public object? Value { get; init; }
    public IEnumerable<object>? Options { get; init; }
    public string? Hint { get; init; }
    public bool ReadOnly { get; init; }
}

/// <summary>
/// One dialog that builds itself from a list of fields, so suppliers,
/// materials, projects and users all edit through the same screen instead of
/// four near-identical hand-built forms.
/// </summary>
public sealed class RecordEditor : Form
{
    private readonly Dictionary<string, Control> _controls = new();
    private readonly Label _error = new();

    public Dictionary<string, object?> Values { get; } = new();

    public RecordEditor(string title, IReadOnlyList<FieldSpec> fields, string saveText = "Save")
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = Theme.Body;

        var head = new Label
        {
            Text = title,
            Font = Theme.H1,
            ForeColor = Theme.Ink,
            Dock = DockStyle.Top,
            Height = 42,
            Padding = new Padding(20, 10, 20, 0),
            AutoSize = false
        };

        var body = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20, 6, 20, 6) };

        var y = 6;
        foreach (var field in fields)
        {
            var label = new Label
            {
                Text = field.Label,
                Font = new Font("Segoe UI Semibold", 9F),
                ForeColor = Theme.InkSoft,
                Location = new Point(0, y),
                Size = new Size(380, 18),
                AutoSize = false
            };
            body.Controls.Add(label);
            y += 20;

            Control input = field.Kind switch
            {
                FieldKind.Multiline => new TextBox
                {
                    Multiline = true, Height = 62, ScrollBars = ScrollBars.Vertical,
                    Text = field.Value?.ToString() ?? ""
                },
                FieldKind.Date => new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Value = field.Value is DateTime d && d > new DateTime(1900, 1, 1) ? d : DateTime.Today
                },
                FieldKind.Combo => BuildCombo(field),
                FieldKind.Password => new TextBox { UseSystemPasswordChar = true, Text = "" },
                _ => new TextBox { Text = FormatValue(field) }
            };

            input.Location = new Point(0, y);
            input.Width = 380;
            if (input is TextBox tb) tb.BorderStyle = BorderStyle.FixedSingle;
            if (field.ReadOnly)
            {
                input.Enabled = false;
                input.BackColor = Theme.Surface;
            }

            _controls[field.Key] = input;
            body.Controls.Add(input);
            y += input.Height + 4;

            if (!string.IsNullOrWhiteSpace(field.Hint))
            {
                var hint = new Label
                {
                    Text = field.Hint,
                    Font = Theme.Small,
                    ForeColor = Theme.Muted,
                    Location = new Point(0, y),
                    Size = new Size(380, 30),
                    AutoSize = false
                };
                body.Controls.Add(hint);
                y += 32;
            }
            else y += 8;
        }

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 76, Padding = new Padding(20, 6, 20, 12) };

        _error.ForeColor = Theme.Danger;
        _error.Dock = DockStyle.Top;
        _error.Height = 20;
        _error.AutoSize = false;

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 38, FlowDirection = FlowDirection.RightToLeft };

        var save = new Button
        {
            Text = saveText,
            BackColor = Theme.Accent, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 9.75F),
            Width = 110, Height = 32
        };
        save.FlatAppearance.BorderSize = 0;
        save.Click += (_, _) => Commit();

        var cancel = UiKit.Secondary("Cancel");
        cancel.Width = 90;
        cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        buttons.Controls.Add(save);
        buttons.Controls.Add(cancel);
        footer.Controls.Add(buttons);
        footer.Controls.Add(_error);

        Controls.Add(body);
        Controls.Add(footer);
        Controls.Add(head);

        ClientSize = new Size(424, Math.Min(660, 42 + y + 90));
        AcceptButton = save;
        CancelButton = cancel;
    }

    private static ComboBox BuildCombo(FieldSpec field)
    {
        var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        foreach (var option in field.Options ?? Enumerable.Empty<object>())
            combo.Items.Add(option);

        if (field.Value is not null)
        {
            var match = combo.Items.Cast<object>()
                .FirstOrDefault(i => i.ToString() == field.Value.ToString()
                                     || (i is IIdentified id && field.Value is int v && id.Id == v));
            if (match is not null) combo.SelectedItem = match;
        }
        if (combo.SelectedIndex < 0 && combo.Items.Count > 0) combo.SelectedIndex = 0;
        return combo;
    }

    private static string FormatValue(FieldSpec field) => field.Value switch
    {
        null => "",
        decimal d => d.ToString("0.##"),
        _ => field.Value.ToString() ?? ""
    };

    private void Commit()
    {
        _error.Text = "";
        Values.Clear();

        foreach (var (key, control) in _controls)
        {
            Values[key] = control switch
            {
                ComboBox combo => combo.SelectedItem,
                DateTimePicker picker => picker.Value.Date,
                TextBox text => text.Text.Trim(),
                _ => null
            };
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    public string Text_(string key) => Values.TryGetValue(key, out var v) ? v?.ToString() ?? "" : "";

    public decimal Decimal_(string key, decimal fallback = 0) =>
        decimal.TryParse(Text_(key), out var d) ? d : fallback;

    public DateTime Date_(string key) =>
        Values.TryGetValue(key, out var v) && v is DateTime d ? d : DateTime.Today;

    public T? Pick<T>(string key) where T : class =>
        Values.TryGetValue(key, out var v) ? v as T : null;

    /// <summary>True when a numeric box holds something that is not a number.</summary>
    public bool IsNotANumber(string key) =>
        !string.IsNullOrWhiteSpace(Text_(key)) && !decimal.TryParse(Text_(key), out _);
}

/// <summary>Lets the editor match a combo item back to an id.</summary>
public interface IIdentified
{
    int Id { get; }
}
