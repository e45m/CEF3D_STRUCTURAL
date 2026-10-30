using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CEF;

// Shared look and small control factories for the data dialogs
internal static class dlgStyle
{
    public const int BarHeight = 80;   // height of the top action bar

    private static readonly Font baseFont = new Font("Segoe UI", 9f);
    private static readonly Color barBack = Color.FromArgb(243, 243, 243);
    private static readonly Color accent = Color.FromArgb(0, 103, 192);
    private static readonly Color readOnlyBack = Color.FromArgb(245, 245, 245);

    public static void applyForm(Form f, bool resizable = false)
    {
        f.Font = baseFont;
        f.AutoScaleMode = AutoScaleMode.Dpi;
        f.StartPosition = FormStartPosition.CenterParent;
        f.FormBorderStyle = resizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
        f.MaximizeBox = resizable;
        f.MinimizeBox = false;
        f.ShowInTaskbar = false;
    }

    public static Panel actionBar()
    {
        return new Panel { Dock = DockStyle.Top, Height = BarHeight, BackColor = barBack };
    }

    public static GroupBox group(string text, int x, int y, int w, int h)
    {
        return new GroupBox { Text = text, Left = x, Top = y, Width = w, Height = h };
    }

    public static Button button(string text, int x, int y, int w = 100, bool primary = false)
    {
        var b = new Button
        {
            Text = text,
            Left = x,
            Top = y,
            Width = w,
            Height = 28,
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false,
            BackColor = primary ? accent : Color.White,
            ForeColor = primary ? Color.White : SystemColors.ControlText
        };
        b.FlatAppearance.BorderColor = primary ? accent : Color.Silver;
        return b;
    }

    public static Label label(Control parent, string text, int x, int y, int w)
    {
        var l = new Label { Text = text, Left = x, Top = y + 3, Width = w, AutoEllipsis = true };
        parent.Controls.Add(l);
        return l;
    }

    public static RadioButton radioButton(Control parent, string text, int x, int y, int w)
    {   var r = new RadioButton() {
        AutoSize = true,
        Left=x,
        Top=y+3,
        Width = w,
        Text = text,
        UseVisualStyleBackColor = true,
    };
    parent.Controls.Add(r);
    return r;
    }

    public static TextBox textBox(Control parent, int x, int y, int w)
    {
        var tb = new TextBox { Left = x, Top = y, Width = w, ReadOnly = true, BackColor = readOnlyBack };
        parent.Controls.Add(tb);
        return tb;
    }

        public static TextBox editableTextBox(Control parent, int x, int y, int w)
    {
        var tb = new TextBox { Left = x, Top = y, Width = w };
        parent.Controls.Add(tb);
        return tb;
    }

    // Label + read-only text box on the same row
    public static TextBox field(Control parent, string label, int x, int y, int lblW, int tbW)
    {
        dlgStyle.label(parent, label, x, y, lblW);
        return textBox(parent, x + lblW, y, tbW);
    }

        public static TextBox editableField(Control parent, string label, int x, int y, int lblW, int tbW)
    {
        dlgStyle.label(parent, label, x, y, lblW);
        return editableTextBox(parent, x + lblW, y, tbW);
    }

    public static ComboBox combo(Control parent, string label, int x, int y, int lblW, int w, string[] items)
    {
        dlgStyle.label(parent, label, x, y, lblW);
        var cb = new ComboBox { Left = x + lblW, Top = y, Width = w, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
        cb.Items.AddRange(items);
        parent.Controls.Add(cb);
        return cb;
    }

    public static void setEditable(TextBox tb, bool state)
    {
        tb.ReadOnly = !state;
        tb.BackColor = state ? SystemColors.Window : readOnlyBack;
    }

    // Returns the chosen database file or "" if cancelled
    public static string openDbFile(string initialDir = "dbApp")
    {
        using var dlg = new OpenFileDialog
        {
            Filter = $"{I18n.DBFilter} (*.cefdb)|*.cefdb",
            InitialDirectory = Path.GetFullPath(initialDir),
            CheckFileExists = true,
            RestoreDirectory = true   // keeps the relative default DB paths valid
        };
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : "";
    }
}