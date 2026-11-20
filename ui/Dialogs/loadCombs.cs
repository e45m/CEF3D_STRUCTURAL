using System;
namespace CEF;
public class loadCombDlg : Form
{
    private Button BttOK;
    private Button BttCancel;
    private DataGridView DataGridView1;
    private Button BttAdd, BttDel;          // add / delete a row (case + factor) of the current combination
    private Button BttAddComb, BttDelComb;  // add / delete a whole combination
    private ComboBox CombList;

    private string currCombo = "";
    private bool loading = false;

    private void InitializeComponent()
    {
        BttOK = dlgStyle.button(I18n.DlgOKText, 180, 242, 80, true);
        BttOK.Click += BttOK_Click;

        BttCancel = dlgStyle.button(I18n.DlgCancelText, 260, 242, 80);
        CancelButton = BttCancel;
        BttCancel.Click += BttCancel_Click;

        BttAdd = dlgStyle.button(I18n.Add, 20, 242, 80);
        BttAdd.Click += BttAdd_Click;

        BttDel = dlgStyle.button(I18n.Delete, 100, 242, 80);
        BttDel.Click += BttDel_Click;

        DataGridView1 = new DataGridView()
        {
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            Location = new Point(31, 62),
            Margin = new Padding(4, 3, 4, 3),
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            Size = new Size(286, 173),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            ColumnHeadersVisible = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.FixedSingle,
            ReadOnly = false,
        };
        DataGridView1.CellBeginEdit += Grid_CellBeginEdit;
        DataGridView1.CellValueChanged += Grid_CellValueChanged;
        DataGridView1.CurrentCellDirtyStateChanged += Grid_DirtyStateChanged;
        DataGridView1.DataError += (_, e) => e.ThrowException = false;

        CombList = dlgStyle.combo(this, I18n.loadCases, DataGridView1.Left, DataGridView1.Top - 25, 80, 100, []);
        CombList.Enabled = true;
        CombList.SelectedIndexChanged += comboChange;

        BttAddComb = dlgStyle.button("+", 252, DataGridView1.Top - 27, 30);
        BttAddComb.Click += BttAddComb_Click;
        BttDelComb = dlgStyle.button("−", 287, DataGridView1.Top - 27, 30);
        BttDelComb.Click += BttDelComb_Click;

        ClientSize = new Size(348, 309);
        Controls.Add(BttAdd);
        Controls.Add(BttDel);
        Controls.Add(BttAddComb);
        Controls.Add(BttDelComb);
        Controls.Add(DataGridView1);
        Controls.Add(BttCancel);
        Controls.Add(BttOK);

        Text = I18n.LoadCombinations;
        Load += FormLoad;

        dlgStyle.applyForm(this);
    }

    public loadCombDlg()
    {
        InitializeComponent();
    }

    // ---------- Helpers ----------

    void updateComboNames()
    {
        var combNames = Engine.getModel().loadCombs.Select(lc => lc.Name).Distinct().ToArray();
        CombList.Items.Clear();
        CombList.Items.AddRange(combNames);
        int idx = CombList.Items.IndexOf(currCombo);
        if (idx < 0 && CombList.Items.Count > 0) idx = 0;
        loading = true;
        CombList.SelectedIndex = idx;
        loading = false;
        currCombo = idx >= 0 ? CombList.Items[idx].ToString() : "";
    }

    string nextCombName()
    {
        int n = 1;
        while (Engine.getModel().loadCombs.Any(c => c.Name == $"Comb{n}")) n++;
        return $"Comb{n}";
    }

    // Asks for the name of a new combination (suggests a consecutive one); returns null if cancelled
    string askCombName()
    {
        string result = null;
        using var form = new Form()
        {
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedToolWindow,
            Width = 250,
            Height = 100,
            Text = I18n.LoadCombinations
        };
        var tbName = new TextBox() { Left = 20, Top = 10, Width = 190, Text = nextCombName() };
        var btNO = new Button() { Text = I18n.DlgCancelText, DialogResult = DialogResult.Cancel, Top = 35, Left = 50 };
        var btOK = new Button() { Text = I18n.DlgOKText, Top = 35, Left = 130 };
        form.Controls.AddRange([tbName, btNO, btOK]);
        form.AcceptButton = btOK;
        form.CancelButton = btNO;
        btOK.Click += (_, _) =>
        {
            var name = tbName.Text.Trim();
            if (name == "" || Engine.getModel().loadCombs.Any(c => c.Name == name))
            {
                tbName.Focus();
                tbName.SelectAll();
                return; // empty or duplicated name: keep the dialog open
            }
            result = name;
            form.Close();
        };
        form.ShowDialog(this);
        return result;
    }

    int defaultCaseID()
    {
        var cases = Engine.getModel().loadCases;
        return cases.Count > 0 ? cases[0].ID : -1;
    }

    int newID()
    {
        var combs = Engine.getModel().loadCombs;
        int id = combs.NextID();
        if (id <= 0) id = combs.Count == 0 ? 1 : combs.Max(c => c.ID) + 1; // fallback if NextID() gives an invalid value
        return id;
    }

    loadComb newRow(string name)
    {
        var row = new loadComb
        {
            ID = newID(),
            Name = name,
            Type = 0,
            Case = defaultCaseID(),
            Factor = 1
        };
        Engine.getModel().loadCombs.Add(row);
        return row;
    }

    // ---------- Form load ----------

    private void FormLoad(object sender, EventArgs e)
    {
        var caseList = new List<KeyValuePair<int, string>>();
        var cases = Engine.getModel().loadCases;
        for (int i = 0; i < cases.Count; i++)
            caseList.Add(new KeyValuePair<int, string>(cases[i].ID, cases[i].Name));

        var colName = new DataGridViewTextBoxColumn { HeaderText = I18n.Name, Width = 85 };
        var colCase = new DataGridViewComboBoxColumn
        {
            HeaderText = I18n.Case,
            DataSource = caseList,
            DisplayMember = "Value",
            ValueMember = "Key",
            FlatStyle = FlatStyle.Flat,
            Width = 110
        };
        var colFactor = new DataGridViewTextBoxColumn { HeaderText = I18n.Factor, Width = 65, MaxInputLength = 5 };
        DataGridView1.Columns.AddRange(colName, colCase, colFactor);

        updateComboNames();
        fillDataGrid();
    }

    private void fillDataGrid()
    {
        loading = true;
        DataGridView1.Rows.Clear();
        var combs = Engine.getModel().loadCombs;
        var cases = Engine.getModel().loadCases;
        for (int i = 0; i < combs.Count; i++)
        {
            var cc = combs[i];
            if (cc.Name != currCombo) continue;
            int r = DataGridView1.Rows.Add();
            var row = DataGridView1.Rows[r];
            row.Tag = cc;
            row.Cells[0].Value = cc.Name;
            if (cases.Any(c => c.ID == cc.Case)) row.Cells[1].Value = cc.Case;
            row.Cells[2].Value = cc.Factor;
        }
        loading = false;
    }

    // ---------- Grid editing ----------

    private void Grid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        App.takeUndoSnapShot();
    }

    private void Grid_DirtyStateChanged(object sender, EventArgs e)
    {
        // Only the combo cell commits immediately; text cells commit when the edit ends
        if (DataGridView1.IsCurrentCellDirty && DataGridView1.CurrentCell is DataGridViewComboBoxCell)
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
        if (loading || e.RowIndex < 0) return;
        var row = DataGridView1.Rows[e.RowIndex];
        if (row.Tag is not loadComb cc) return;

        var val = row.Cells[e.ColumnIndex].Value;
        if (e.ColumnIndex == 0)
        {
            renameCombo(Convert.ToString(val)?.Trim() ?? "");
            return;
        }
        if (e.ColumnIndex == 1)
        {
            if (val != null) cc.Case = Convert.ToInt32(val);
        }
        else if (e.ColumnIndex == 2)
        {
            var s = Convert.ToString(val)?.Trim().Replace(',', '.');
            if (double.TryParse(s, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double f))
                cc.Factor = f;
            else
            {
                loading = true;
                row.Cells[2].Value = cc.Factor; // restore previous value
                loading = false;
            }
        }
    }

    // Renames the current combination (all its rows); empty or duplicated names are discarded
    void renameCombo(string newName)
    {
        if (newName == currCombo) return;
        var combs = Engine.getModel().loadCombs;
        if (newName != "" && !combs.Any(c => c.Name == newName))
        {
            for (int i = 0; i < combs.Count; i++)
                if (combs[i].Name == currCombo)
                    combs[i].Name = newName;
            currCombo = newName;
        }
        // refresh after the grid finishes the current edit
        BeginInvoke((Action)(() => { updateComboNames(); fillDataGrid(); }));
    }

    // ---------- Buttons ----------

    private void BttOK_Click(object sender, EventArgs e)
    {
        DataGridView1.EndEdit();
        Close();
    }

    private void BttCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    void comboChange(object sender, EventArgs e)
    {
        if (loading || CombList.SelectedItem == null) return;
        DataGridView1.EndEdit();
        currCombo = CombList.SelectedItem.ToString();
        fillDataGrid();
    }

    // New combination with a consecutive name and one default row
    private void BttAddComb_Click(object sender, EventArgs e)
    {
        if (Engine.getModel().loadCases.Count == 0) return; // a combination needs at least one load case
        var name = askCombName();
        if (name == null) return;
        App.takeUndoSnapShot();
        DataGridView1.EndEdit();
        currCombo = name;
        newRow(currCombo);
        updateComboNames();
        fillDataGrid();
    }

    // Removes the whole combination (all its rows)
    private void BttDelComb_Click(object sender, EventArgs e)
    {
        if (currCombo == "") return;
        App.takeUndoSnapShot();
        DataGridView1.EndEdit();
        var combs = Engine.getModel().loadCombs;
        for (int i = combs.Count - 1; i >= 0; i--)
            if (combs[i].Name == currCombo)
                combs.RemoveAt(i);
        currCombo = "";
        updateComboNames();
        fillDataGrid();
    }

    // Adds a row (case + factor) to the current combination; cases may repeat
    private void BttAdd_Click(object sender, EventArgs e)
    {
        if (Engine.getModel().loadCases.Count == 0) return; // a combination needs at least one load case
        string newName = null;
        if (currCombo == "")
        {
            newName = askCombName();
            if (newName == null) return;
        }
        App.takeUndoSnapShot();
        DataGridView1.EndEdit();
        if (newName != null)
        {
            currCombo = newName;
            newRow(currCombo);
            updateComboNames();
        }
        else
            newRow(currCombo);
        fillDataGrid();
    }

    // Deletes the selected row
    private void BttDel_Click(object sender, EventArgs e)
    {
        if (DataGridView1.CurrentRow?.Tag is not loadComb cc) return;
        App.takeUndoSnapShot();
        DataGridView1.EndEdit();
        var idx = Engine.getModel().loadCombs.FindIndexByID(cc.ID);
        if (idx >= 0) Engine.getModel().loadCombs.RemoveAt(idx);
        updateComboNames();
        fillDataGrid();
    }
}