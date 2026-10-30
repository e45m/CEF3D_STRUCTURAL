using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CEF;

public class defineNewCoordSys : Form
{
    private const string defaultName = "Default";
    private const string newName = "NCS";
    private const int nAxes = 3;
    private static readonly string[] axisNames = { "X", "Y", "Z" };

    private Button btOK, btCancel, btRemove;
    private PictureBox Imagen;
    private Label lblName;
    internal ComboBox cbSCName;

    private readonly DataGridView[] grids = new DataGridView[nAxes];
    private readonly Button[] spaceBtns = new Button[nAxes];
    private readonly DataTable[] tables = new DataTable[nAxes];

    private CEF.MainWindow _parent = null;

    #region Model access (single place to adjust)

    // Assumption: the three grid tables share the same type
    private static DtaTable<Coordinate> coordTable(int axis)
    {
        return axis == 0 ? App.model.gridCoordsX : axis == 1 ? App.model.gridCoordsY : App.model.gridCoordsZ;
    }

    // Coordinates are lengths: same conversion in both directions
    private static double toInternal(double v) { return Uc.lenFromUserUnits(v); }
    private static double toUser(double v) { return Uc.lenToUserUnits(v); }

    #endregion

    private string scName { get { return cbSCName.Text.Trim(); } }

    #region UI

    private void InitializeComponent()
    {
        dlgStyle.applyForm(this);
        ClientSize = new Size(450, 368);
        Text = I18n.coordSys;
        TopMost = true;

        // --- Header: axes image + system name
        var bar = dlgStyle.actionBar();

        Imagen = new PictureBox
        {
            Image = CEF.Properties.Resources.Ejes,
            Left = 10,
            Top = 6,
            Width = 80,
            Height = 68,
            SizeMode = PictureBoxSizeMode.Zoom,
            TabStop = false
        };
        lblName = new Label { Text = I18n.coordSys, Left = 100, Top = 12, Width = 340 };
        cbSCName = new ComboBox { Left = 100, Top = 36, Width = 340 };
        cbSCName.TextChanged += CbSCNameChanged;

        bar.Controls.AddRange(new Control[] { Imagen, lblName, cbSCName });

        // --- One group per axis: grid + batch input button
        int groupTop = dlgStyle.BarHeight + 8;
        int groupH = 232;

        for (int a = 0; a < nAxes; a++)
        {
            var gb = dlgStyle.group(axisNames[a], 10 + a * 145, groupTop, 140, groupH);

            var g = new DataGridView { Left = 8, Top = 20, Width = 124, Height = 170, Tag = a };
            styleGrid(g);
            g.CellEndEdit += Grid_CellEndEdit;
            g.DataError += (_, e) => e.ThrowException = false;   // no default error dialog on bad input
            grids[a] = g;

            var b = dlgStyle.button(I18n.BatchImput, 8, 196, 124);
            b.Tag = a;
            b.Click += Space_Click;
            spaceBtns[a] = b;

            gb.Controls.Add(g);
            gb.Controls.Add(b);
            Controls.Add(gb);
        }

        // --- Bottom buttons
        int by = groupTop + groupH + 10;

        btRemove = dlgStyle.button(I18n.Delete, 10, by, 90);
        btCancel = dlgStyle.button(I18n.DlgCancelText, 260, by, 85);
        btOK = dlgStyle.button(I18n.DlgOKText, 355, by, 85, true);

        btCancel.DialogResult = DialogResult.Cancel;
        btRemove.Click += BtRemove;
        btOK.Click += OK_Button_Click;

        AcceptButton = btOK;
        CancelButton = btCancel;

        Controls.AddRange(new Control[] { btRemove, btCancel, btOK, bar });
    }

    private static void styleGrid(DataGridView g)
    {
        g.AllowUserToResizeRows = false;
        g.RowHeadersVisible = false;
        g.ColumnHeadersVisible = false;   // the group title already says which axis it is
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        g.BackgroundColor = SystemColors.Window;
        g.BorderStyle = BorderStyle.FixedSingle;
    }

    #endregion

    public defineNewCoordSys(CEF.MainWindow _Parent)
    {
        InitializeComponent();
        _parent = _Parent;
        initData();

        cbSCName.Items.AddRange(App.model.gridCoordsX.Select(c => c.coordSys).Distinct().ToArray());

        string initial = newName;
        if (!App.initialized)
        {
            initial = defaultName;
            App.initialized = true;
        }

        cbSCName.Text = initial;   // fires CbSCNameChanged: loads the system if it already exists
        updateButtons();
    }

    private void initData()
    {
        for (int a = 0; a < nAxes; a++)
        {
            var t = new DataTable(axisNames[a] + "_Grid");
            t.Columns.Add("Coord", typeof(double));
            tables[a] = t;
            grids[a].DataSource = t;
        }
    }

    #region Systems

    private static bool systemExists(string name)
    {
        return App.model.gridCoordsX.Any(c => c.coordSys == name);
    }

    private static bool canRemove(string name)
    {
        if (name == defaultName || !systemExists(name)) return false;

        return App.model.gridCoordsX.Select(c => c.coordSys).Distinct().Count() >= 2;
    }

    private void updateButtons()
    {
        btOK.Enabled = scName.Length > 0;
        btRemove.Enabled = canRemove(scName);
    }

    // An existing system is loaded as soon as its name is picked or typed;
    // a new name keeps the current grid as a starting point ("save as")
    private void CbSCNameChanged(object sender, EventArgs e)
    {
        string name = scName;

        if (systemExists(name))
            loadSystem(name);

        updateButtons();
    }

    private void loadSystem(string name)
    {
        for (int a = 0; a < nAxes; a++)
        {
            var tbl = tables[a];
            tbl.Clear();

            foreach (var c in coordTable(a))
                if (c.coordSys == name)
                    tbl.Rows.Add(toUser(c.Coord));
        }
    }

    public void removeSC(string name)
    {
        for (int a = 0; a < nAxes; a++)
            replaceSystem(a, name, Array.Empty<double>());
    }

    public void guardar_datos_grilla()
    {
        string name = scName;

        for (int a = 0; a < nAxes; a++)
        {
            grids[a].EndEdit();   // commit a cell still being edited
            replaceSystem(a, name, axisValues(a));
        }
    }

    // Replaces the rows of one system in place (keeps the order of systems).
    // IDs are renumbered = row index, by design (exception to NextID use)
    private static void replaceSystem(int axis, string name, double[] userValues)
    {
        var t = coordTable(axis);
        var old = t.ToList();
        t.Clear();

        bool placed = false;
        int id = 0;

        foreach (var c in old)
        {
            if (c.coordSys != name)
            {
                c.ID = id++;
                t.Add(c);
                continue;
            }

            if (placed) continue;
            placed = true;
            id = addValues(axis, name, userValues, id);
        }

        if (!placed)
            addValues(axis, name, userValues, id);
    }

    private static int addValues(int axis, string name, double[] userValues, int id)
    {
        var t = coordTable(axis);

        for (int i = 0; i < userValues.Length; i++)
            t.Add(new Coordinate { ID = id++, Coord = toInternal(userValues[i]), coordSys = name });

        return id;
    }

    #endregion

    #region Buttons

    private void BtRemove(object sender, EventArgs e)
    {
        string name = scName;
        if (!canRemove(name)) return;

        App.takeUndoSnapShot();
        removeSC(name);
        space3d.createGrid(_parent);
        DialogResult = DialogResult.OK;
    }

    private void OK_Button_Click(object sender, EventArgs e)
    {
        if (scName.Length == 0) return;

        App.takeUndoSnapShot();
        guardar_datos_grilla();
        space3d.createGrid(_parent);
        DialogResult = DialogResult.OK;
    }

    #endregion

    #region Grids

    // Values of one axis in user units, sorted and without duplicates
    private double[] axisValues(int axis)
    {
        var v = new List<double>();

        foreach (DataRow r in tables[axis].Rows)
            if (r[0] is double d) v.Add(d);

        return v.Distinct().OrderBy(x => x).ToArray();
    }

    private void Grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        int axis = (int)((DataGridView)sender).Tag;

        // Deferred: the grid is still committing the edit
        BeginInvoke((Action)(() => sortColumn(axis)));
    }

    private void sortColumn(int axis)
    {
        var sorted = axisValues(axis);
        var tbl = tables[axis];

        var current = new double[tbl.Rows.Count];
        for (int i = 0; i < current.Length; i++)
            current[i] = tbl.Rows[i][0] is double d ? d : double.NaN;

        if (sorted.SequenceEqual(current)) return;   // already clean: avoid redrawing/jumping

        tbl.Rows.Clear();
        for (int i = 0; i < sorted.Length; i++)
            tbl.Rows.Add(sorted[i]);
    }

    // Appends cumulative positions after the last coordinate of the axis
    private void appendSpacings(int axis, double[] cumulative)
    {
        var tbl = tables[axis];

        double start = 0;
        bool any = false;
        foreach (DataRow r in tbl.Rows)
        {
            if (r[0] is not double d) continue;
            start = any ? Math.Max(start, d) : d;
            any = true;
        }

        if (!any)
            tbl.Rows.Add(0d);

        for (int i = 0; i < cumulative.Length; i++)
            tbl.Rows.Add(cumulative[i] + start);

        sortColumn(axis);   // sorts and drops duplicates
    }

    #endregion

    #region Batch input

    private void Space_Click(object sender, EventArgs e)
    {
        int axis = (int)((Button)sender).Tag;

        var spaces = askSpacings();
        if (spaces == null || spaces.Length < 2) return;   // length < 2: nothing to add (e.g. default "0")

        appendSpacings(axis, spaces);
    }

    // Returns the cumulative positions typed by the user, or null if cancelled
    private double[] askSpacings()
    {
        double[] result = null;

        using var dlg = new Form();
        dlgStyle.applyForm(dlg);
        dlg.ClientSize = new Size(300, 150);
        dlg.Text = I18n.BatchImput;

        dlgStyle.label(dlg, I18n.ImputSpacing, 20, 8, 260);
        dlgStyle.label(dlg, I18n.ImputSpacingExample, 20, 32, 260);
        dlgStyle.label(dlg, I18n.Spacing, 20, 66, 95);

        var tb = new TextBox { Left = 120, Top = 66, Width = 160, Text = "0" };
        var btOk = dlgStyle.button(I18n.DlgOKText, 195, 108, 85, true);
        var btCan = dlgStyle.button(I18n.DlgCancelText, 100, 108, 85);
        btCan.DialogResult = DialogResult.Cancel;

        tb.TextChanged += (_, _) => tb.BackColor = SystemColors.Window;
        btOk.Click += (_, _) =>
        {
            var v = parseSpacings(tb.Text);
            if (v == null)
            {
                tb.BackColor = Color.MistyRose;   // invalid input: stay open and flag it
                tb.Focus();
                return;
            }
            result = v;
            dlg.DialogResult = DialogResult.OK;
        };

        dlg.Controls.AddRange(new Control[] { tb, btOk, btCan });
        dlg.AcceptButton = btOk;
        dlg.CancelButton = btCan;
        dlg.Shown += (_, _) => tb.SelectAll();

        // Next to this dialog, kept on screen
        dlg.StartPosition = FormStartPosition.Manual;
        var wa = Screen.FromControl(this).WorkingArea;
        dlg.Location = new Point(Math.Min(Right, wa.Right - dlg.Width), Top);

        dlg.ShowDialog(this);

        return dlg.DialogResult == DialogResult.OK ? result : null;
    }

    // Parses "n*x, y, ..." (separators from I18n) into cumulative positions; null if invalid or negative
    private static double[] parseSpacings(string text)
    {
        var steps = new List<double>();

        foreach (var part in text.Split(I18n.SplitChar))
        {
            string item = part.Trim();
            int times = 1;

            if (item.Contains(I18n.GroupingKey))
            {
                var g = item.Split(I18n.GroupingKey);
                if (g.Length != 2 || !int.TryParse(g[0], out times) || times < 1) return null;
                item = g[1];
            }

            if (!double.TryParse(item, out double v)) return null;

            for (int i = 0; i < times; i++)
                steps.Add(v);
        }

        var cumulative = new double[steps.Count];
        double sum = 0;
        for (int i = 0; i < cumulative.Length; i++)
        {
            sum += steps[i];
            cumulative[i] = sum;
        }

        return cumulative.Any(x => x < 0) ? null : cumulative;
    }

    #endregion
}