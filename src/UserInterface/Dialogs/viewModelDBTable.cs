using System.Reflection;
using System.Text;

namespace CEF;

public class viewModelDBTable : Form
{
    // ASSUMPTION: model table names where rows with all actions = 0 are hidden by default. Edit only this line.
    static readonly string[] zeroHideTables = { "nodalReactions", "nodalRestraints", "nodalDisplacements" };

    class TableState
    {
        public Type RowType;
        public FieldInfo[] Fields;
        public DataGridView Grid;
        public int CaseCol = -1;
        public int IdCol = -1;
        public bool ZeroTable;
    }

    class RowRef
    {
        public object Obj;
        public bool Zero;
    }

    internal TabControl tabControl1;
    internal FlowLayoutPanel pnlTop;
    internal TextBox txtSearch;
    internal TextBox txtIds;
    internal ComboBox cmbCase;
    internal CheckBox chkHideZeros;
    internal Button btnCopy;
    internal Label lblStatus;

    readonly List<string> tableNames;
    readonly bool editable;
    int[] caseIds = Array.Empty<int>();
    bool building;

    public viewModelDBTable(List<string> Tables, bool Editable = true)
    {
        tableNames = Tables;
        editable = Editable;
        InitializeComponent();
    }

    #region UI construction

    void InitializeComponent()
    {
        tabControl1 = new TabControl { Alignment = TabAlignment.Bottom, Dock = DockStyle.Fill };
        tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;

        pnlTop = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Padding = new Padding(4, 2, 4, 2)
        };
        addLabel(I18n.TableSearch);
        txtSearch = addTextBox(150);
        txtSearch.TextChanged += filterChanged;
        addLabel(I18n.TableCase);
        cmbCase = addComboBox(150, filterChanged);
        addLabel(I18n.TableIds);
        txtIds = addTextBox(110);
        txtIds.TextChanged += filterChanged;
        chkHideZeros = new CheckBox { Text = I18n.TableHideZeros, AutoSize = true, Checked = true, Margin = new Padding(12, 6, 3, 3) };
        chkHideZeros.CheckedChanged += filterChanged;
        pnlTop.Controls.Add(chkHideZeros);
        btnCopy = new Button { Text = I18n.TableCopy, AutoSize = true, Margin = new Padding(12, 1, 3, 1) };
        btnCopy.Click += btnCopy_Click;
        pnlTop.Controls.Add(btnCopy);

        lblStatus = new Label { Dock = DockStyle.Bottom, Height = 22, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(6, 0, 0, 0) };

        FormBorderStyle = FormBorderStyle.Sizable;
        ClientSize = new Size(1000, 700);
        MinimumSize = new Size(640, 400);
        KeyPreview = true;
        Text = I18n.TableView;
        Controls.Add(tabControl1);
        Controls.Add(pnlTop);
        Controls.Add(lblStatus);
        KeyDown += onKeyDown;
        Load += (s, e) => buildTables(0);
    }

    void addLabel(string text)
    {
        pnlTop.Controls.Add(new Label { Text = text, AutoSize = true, Margin = new Padding(8, 7, 2, 0) });
    }

    TextBox addTextBox(int w)
    {
        var t = new TextBox { Width = w, Margin = new Padding(2, 4, 2, 0) };
        pnlTop.Controls.Add(t);
        return t;
    }

    ComboBox addComboBox(int w, EventHandler ev)
    {
        var c = new ComboBox { Width = w, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(2, 4, 2, 0) };
        c.SelectedIndexChanged += ev;
        pnlTop.Controls.Add(c);
        return c;
    }

    #endregion

    #region Tables

    void buildTables(int selectIndex)
    {
        building = true;
        fillCaseCombo();
        while (tabControl1.TabPages.Count > 0)
        {
            TabPage old = tabControl1.TabPages[0];
            tabControl1.TabPages.RemoveAt(0);
            old.Dispose();
        }
        foreach (FieldInfo tbl in App.model.GetType().GetFields())
        {
            if (!tableNames.Any(t => t == "*" || t == tbl.Name)) continue;
            object table = tbl.GetValue(App.model);
            if (table == null) continue;
            PropertyInfo itemsProp = table.GetType().GetProperty("Items");
            if (itemsProp == null) continue;
            if (itemsProp.GetValue(table) is not IEnumerable<object> items) continue;
            addTable(tbl.Name, table, items);
        }
        if (tabControl1.TabPages.Count > 0)
            tabControl1.SelectedIndex = Math.Min(selectIndex, tabControl1.TabPages.Count - 1);
        building = false;
        updateControlsForTab();
        applyFilters();
    }

    void fillCaseCombo()
    {
        int prevId = cmbCase.SelectedIndex > 0 ? caseIds[cmbCase.SelectedIndex - 1] : int.MinValue;
        int n = App.model.loadCases.Count;
        caseIds = new int[n];
        cmbCase.Items.Clear();
        cmbCase.Items.Add(I18n.TableAll);
        int sel = 0;
        for (int i = 0; i < n; i++)
        {
            loadCase lc = App.model.loadCases[i];
            caseIds[i] = lc.ID;
            cmbCase.Items.Add($"{lc.ID}: {lc.Name}");
            if (lc.ID == prevId) sel = i + 1;
        }
        cmbCase.SelectedIndex = sel;
    }

    void addTable(string name, object table, IEnumerable<object> items)
    {
        List<object> rows = new List<object>(items);
        Type rowType = rows.Count > 0 ? rows[0].GetType() : null;
        if (rowType == null)
        {
            Type[] args = table.GetType().GetGenericArguments();
            if (args.Length > 0) rowType = args[0];
        }

        var st = new TableState();
        st.RowType = rowType;
        st.Fields = rowType == null ? new FieldInfo[0] : parseMetadata.getFields(rowType);
        st.ZeroTable = Array.IndexOf(zeroHideTables, name) >= 0;

        // Filter columns: case column, and id column by priority BarID > NodeID > ID
        int idRank = 99;
        for (int c = 0; c < st.Fields.Length; c++)
        {
            switch (st.Fields[c].Name)
            {
                case "Case": st.CaseCol = c; break;
                case "BarID": if (idRank > 0) { idRank = 0; st.IdCol = c; } break;
                case "NodeID": if (idRank > 1) { idRank = 1; st.IdCol = c; } break;
                case "ID": if (idRank > 2) { idRank = 2; st.IdCol = c; } break;
            }
        }

        DataGridView grid = createGrid(name, st);
        st.Grid = grid;
        grid.Tag = st;
        fillGrid(grid, st, rows);

        var tab = new TabPage(parseMetadata.tableText(name)) { Name = name, Tag = st };
        tab.Controls.Add(grid);
        tabControl1.TabPages.Add(tab);
    }

    DataGridView createGrid(string name, TableState st)
    {
        Color headBack = Color.FromArgb(230, 234, 240);
        var grid = new DataGridView
        {
            Name = name,
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            BorderStyle = BorderStyle.None,
            BackgroundColor = SystemColors.Window,
            GridColor = Color.FromArgb(224, 228, 234),
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight = 28
        };
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = headBack,
            ForeColor = Color.FromArgb(30, 30, 30),
            SelectionBackColor = headBack,
            SelectionForeColor = Color.FromArgb(30, 30, 30),
            Font = new Font(Font, FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleCenter
        };
        grid.RowTemplate.Height = 22;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 248, 251);
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 245);
        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        grid.CellEndEdit += grid_CellEndEdit;

        for (int c = 0; c < st.Fields.Length; c++)
        {
            FieldInfo f = st.Fields[c];
            bool readOnly = !editable || parseMetadata.isReadOnly(st.RowType, f);
            Type t = f.FieldType;
            bool numeric = (t == typeof(double) || t == typeof(float) || t == typeof(int) || t == typeof(long))
                           && !parseMetadata.isEnumField(st.RowType, f);
            var col = new DataGridViewTextBoxColumn
            {
                Name = f.Name,
                HeaderText = parseMetadata.headerText(f),
                ToolTipText = f.Name,
                ReadOnly = readOnly,
                MinimumWidth = 50
            };
            if (f.Name == "ID") col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            else if (numeric) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (readOnly) col.DefaultCellStyle.ForeColor = Color.DimGray;
            grid.Columns.Add(col);
        }
        return grid;
    }

    void fillGrid(DataGridView grid, TableState st, List<object> rows)
    {
        int nCols = st.Fields.Length;
        if (nCols == 0) return;
        grid.SuspendLayout();
        object[] vals = new object[nCols];
        for (int r = 0; r < rows.Count; r++)
        {
            object obj = rows[r];
            for (int c = 0; c < nCols; c++) vals[c] = parseMetadata.formatCell(obj, st.Fields[c]);
            int idx = grid.Rows.Add(vals);
            grid.Rows[idx].Tag = new RowRef { Obj = obj, Zero = st.ZeroTable && parseMetadata.isZeroRow(obj, st.Fields) };
        }
        // Autosize after filling (faster). Wide tables scroll horizontally.
        grid.AutoSizeColumnsMode = nCols <= 8 ? DataGridViewAutoSizeColumnsMode.Fill : DataGridViewAutoSizeColumnsMode.DisplayedCells;
        grid.ResumeLayout();
    }

    void grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (!editable || e.RowIndex < 0 || e.ColumnIndex < 0) return;
        var grid = (DataGridView)sender;
        var st = (TableState)grid.Tag;
        DataGridViewRow row = grid.Rows[e.RowIndex];
        var rr = (RowRef)row.Tag;
        FieldInfo f = st.Fields[e.ColumnIndex];
        DataGridViewCell cell = row.Cells[e.ColumnIndex];

        string text = cell.Value?.ToString();
        string before = parseMetadata.formatCell(rr.Obj, f)?.ToString();
        if (text == before) return;

        bool ok = parseMetadata.tryParseCell(rr.Obj, f, text);
        cell.Value = parseMetadata.formatCell(rr.Obj, f);
        if (!ok)
        {
            lblStatus.Text = I18n.TableInvalidValue;
            return;
        }
        if (st.ZeroTable)
        {
            bool zero = parseMetadata.isZeroRow(rr.Obj, st.Fields);
            if (zero != rr.Zero)
            {
                rr.Zero = zero;
                applyFilters();
            }
        }
    }

    #endregion

    #region Filters

    TableState currentState()
    {
        return tabControl1.SelectedTab?.Tag as TableState;
    }

    void updateControlsForTab()
    {
        TableState st = currentState();
        chkHideZeros.Enabled = st != null && st.ZeroTable;
        cmbCase.Enabled = st != null && st.CaseCol >= 0;
        txtIds.Enabled = st != null && st.IdCol >= 0;
    }

    void applyFilters()
    {
        TableState st = currentState();
        if (st == null)
        {
            lblStatus.Text = "";
            return;
        }
        DataGridView grid = st.Grid;

        string search = txtSearch.Text.Trim();
        bool hideZeros = chkHideZeros.Checked && st.ZeroTable;
        int caseId = int.MinValue;
        if (st.CaseCol >= 0 && cmbCase.SelectedIndex > 0) caseId = caseIds[cmbCase.SelectedIndex - 1];

        int[] lo = null, hi = null;
        bool useIds = false;
        if (st.IdCol >= 0 && txtIds.Text.Trim().Length > 0)
        {
            useIds = tryParseRanges(txtIds.Text, out lo, out hi);
            txtIds.BackColor = useIds ? SystemColors.Window : Color.MistyRose;
        }
        else txtIds.BackColor = SystemColors.Window;

        // The current row cannot be hidden: clear it first
        grid.CurrentCell = null;
        grid.SuspendLayout();
        int visible = 0;
        foreach (DataGridViewRow row in grid.Rows)
        {
            var rr = (RowRef)row.Tag;
            bool ok = true;
            if (hideZeros && rr.Zero) ok = false;
            if (ok && caseId != int.MinValue) ok = readInt(rr.Obj, st.Fields[st.CaseCol]) == caseId;
            if (ok && useIds) ok = inRanges(readInt(rr.Obj, st.Fields[st.IdCol]), lo, hi);
            if (ok && search.Length > 0) ok = rowContains(row, search);
            row.Visible = ok;
            if (ok) visible++;
        }
        grid.ResumeLayout();
        lblStatus.Text = $"{I18n.TableRows}: {visible} / {grid.Rows.Count}";
    }

    static int readInt(object obj, FieldInfo f)
    {
        return Convert.ToInt32(f.GetValue(obj));
    }

    static bool inRanges(int v, int[] lo, int[] hi)
    {
        for (int i = 0; i < lo.Length; i++)
            if (v >= lo[i] && v <= hi[i]) return true;
        return false;
    }

    static bool rowContains(DataGridViewRow row, string text)
    {
        for (int c = 0; c < row.Cells.Count; c++)
        {
            string s = row.Cells[c].Value?.ToString();
            if (s != null && s.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0) return true;
        }
        return false;
    }

    // "1,5,10-20" -> ranges. Negative ids are not supported.
    static bool tryParseRanges(string text, out int[] lo, out int[] hi)
    {
        string[] parts = text.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        lo = new int[parts.Length];
        hi = new int[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            string p = parts[i];
            int d = p.IndexOf('-');
            if (d < 0)
            {
                if (!int.TryParse(p, out lo[i])) return false;
                hi[i] = lo[i];
            }
            else
            {
                if (!int.TryParse(p.Substring(0, d), out lo[i])) return false;
                if (!int.TryParse(p.Substring(d + 1), out hi[i])) return false;
                if (hi[i] < lo[i]) (lo[i], hi[i]) = (hi[i], lo[i]);
            }
        }
        return parts.Length > 0;
    }

    void filterChanged(object sender, EventArgs e)
    {
        if (building) return;
        applyFilters();
    }

    void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (building) return;
        updateControlsForTab();
        applyFilters();
    }

    #endregion

    #region Clipboard and keys

    void btnCopy_Click(object sender, EventArgs e)
    {
        copyActiveTable();
    }

    // Copies headers and visible rows of the active tab, tab-separated (pastes into Excel).
    void copyActiveTable()
    {
        TableState st = currentState();
        if (st == null) return;
        DataGridView grid = st.Grid;
        int nCols = grid.Columns.Count;
        var sb = new StringBuilder();
        for (int c = 0; c < nCols; c++)
        {
            if (c > 0) sb.Append('\t');
            sb.Append(clean(grid.Columns[c].HeaderText));
        }
        foreach (DataGridViewRow row in grid.Rows)
        {
            if (!row.Visible) continue;
            sb.Append(Environment.NewLine);
            for (int c = 0; c < nCols; c++)
            {
                if (c > 0) sb.Append('\t');
                sb.Append(clean(row.Cells[c].Value?.ToString()));
            }
        }
        if (sb.Length == 0) return;
        Clipboard.SetDataObject(sb.ToString(), true, 10, 100);
    }

    static string clean(string s)
    {
        return s == null ? "" : s.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
    }

    void onKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F5)
        {
            buildTables(tabControl1.SelectedIndex);
            e.Handled = true;
        }
        else if (e.Control && e.KeyCode == Keys.W) Close();
        else if (e.Control && e.KeyCode == Keys.F)
        {
            txtSearch.Focus();
            txtSearch.SelectAll();
            e.Handled = true;
        }
    }

    #endregion
}