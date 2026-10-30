using System;

namespace CEF;

public class addNodalMFC : Form
{
    internal ComboBox cbRestr;
    internal ComboBox RestType;
    internal TextBox TbListaNodos;
    internal Button BtDelete;
    internal Button btNewR;
    internal Button ok;
    internal Button No;

    Dictionary<MFCType, string> tipoMFC;

    #region UI

    private void InitializeComponent()
    {
        dlgStyle.applyForm(this);
        Text = I18n.KinematicConstraints;
        ClientSize = new Size(460, 326);

        // --- Constraint selector
        dlgStyle.label(this, I18n.Name, 10, 10, 430);
        cbRestr = new ComboBox { Left = 10, Top = 34, Width = 440, DropDownStyle = ComboBoxStyle.DropDownList };
        cbRestr.SelectedIndexChanged += cbRestr_SelectedIndexChanged;

        // --- Type
        dlgStyle.label(this, I18n.Type, 10, 68, 430);
        RestType = new ComboBox { Left = 10, Top = 92, Width = 440, DropDownStyle = ComboBoxStyle.DropDownList };

        // --- Nodes
        dlgStyle.label(this, I18n.Nodes, 10, 126, 430);
        TbListaNodos = new TextBox
        {
            Left = 10,
            Top = 150,
            Width = 440,
            Height = 120,
            Multiline = true,
            AcceptsReturn = true,
            ScrollBars = ScrollBars.Vertical
        };

        // --- Buttons
        int by = 284;
        BtDelete = dlgStyle.button(I18n.Delete, 10, by, 90);
        btNewR = dlgStyle.button(I18n.New, 110, by, 90);
        No = dlgStyle.button(I18n.DlgCancelText, 265, by, 85);
        ok = dlgStyle.button(I18n.DlgOKText, 365, by, 85, true);

        BtDelete.Click += BtDelete_Click;
        btNewR.Click += btNewR_Click;
        No.Click += No_Click;
        ok.Click += ok_Click;

        AcceptButton = ok;
        CancelButton = No;

        Controls.AddRange(new Control[] { cbRestr, RestType, TbListaNodos, BtDelete, btNewR, No, ok });

        Load += AsMFC_Load;
    }

    #endregion

    public addNodalMFC()
    {
        InitializeComponent();
    }

    private void AsMFC_Load(object sender, EventArgs e)
    {
        dataLoad();

        // Start from the nodes currently selected in the canvas, if any
        var sel = Selection.getNodesColection();
        if (sel != null && sel.Any())
            TbListaNodos.Text = Mis.joinIntList(sel.Select(n => App.model.nodes[n].ID));
    }

    private void dataLoad(int select = 0)
    {
        if (tipoMFC == null)
            tipoMFC = new Dictionary<MFCType, string>()
            {
                { MFCType.diaphragm,   I18n.DiaphragmZ },
                { MFCType.diaphragmXX, I18n.DiaphragmXX },
                { MFCType.diaphragmYY, I18n.DiaphragmYY },
                { MFCType.plate,   I18n.PlateZ },
                { MFCType.plateXX, I18n.PlateXX },
                { MFCType.plateYY, I18n.PlateYY },
                { MFCType.body, I18n.Body },
                { MFCType.weld, I18n.Weld },
                { MFCType.beamX, I18n.BeamX },
                { MFCType.beamY, I18n.BeamY },
                { MFCType.beamZ, I18n.BeamZ },
                { MFCType.rodX, I18n.RodX },
                { MFCType.rodY, I18n.RodY },
                { MFCType.rodZ, I18n.RodZ },
                { MFCType.equal, I18n.Equal }
            };

        RestType.Items.Clear();
        cbRestr.Items.Clear();

        foreach (var it in tipoMFC)
            RestType.Items.Add(it.Value);
        RestType.SelectedIndex = 0;

        foreach (var r in App.model.nodalConstraints)
            cbRestr.Items.Add(r.label);

        if (cbRestr.Items.Count > 0)
            cbRestr.SelectedIndex = Math.Clamp(select, 0, cbRestr.Items.Count - 1);   // fires cbRestr_SelectedIndexChanged
        else
            TbListaNodos.Clear();

        updateButtons();
    }

    private void updateButtons()
    {
        bool has = App.model.nodalConstraints.Count > 0;
        BtDelete.Enabled = has;
        ok.Enabled = has;
    }

    private void cbRestr_SelectedIndexChanged(object sender, EventArgs e)
    {
        int r = cbRestr.SelectedIndex;
        if (r < 0 || r >= App.model.nodalConstraints.Count) return;

        var c = App.model.nodalConstraints[r];
        RestType.SelectedIndex = c.Type;   // assumes Type = position in tipoMFC (same as when creating)
        TbListaNodos.Text = Mis.joinIntList(Mis.parseIntList(c.nodesIDList));   // shown normalized
    }

    // Validates the node list typed by the user; shows a message and returns false if wrong
    private bool readNodes(out int[] ids)
    {
        if (!Mis.tryParseIntList(TbListaNodos.Text, out ids, out string bad))
        {
            MessageBox.Show($"{I18n.CheckInput}\n\"{bad}\"");
            TbListaNodos.Focus();
            return false;
        }

        if (ids.Length == 0)
        {
            MessageBox.Show(I18n.CheckInput);
            TbListaNodos.Focus();
            return false;
        }

        var missing = ids.Where(id => App.model.nodes.FindIndexByID(id) < 0).ToArray();
        if (missing.Length > 0)
        {
            MessageBox.Show($"{I18n.CheckInput}\n{I18n.Nodes}: {Mis.joinIntList(missing)}");
            TbListaNodos.Focus();
            return false;
        }

        return true;
    }

    private void ok_Click(object sender, EventArgs e)
    {
        int idx = cbRestr.SelectedIndex;
        if (idx < 0) return;

        if (!readNodes(out int[] ids)) return;

        App.takeUndoSnapShot();

        var c = App.model.nodalConstraints[idx];
        c.nodesIDList = Mis.joinIntList(ids);   // normalized form on disk
        c.Type = RestType.SelectedIndex;
        Close();
    }

    private void No_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void BtDelete_Click(object sender, EventArgs e)
    {
        int idx = cbRestr.SelectedIndex;
        if (idx < 0) return;

        App.takeUndoSnapShot();

        App.model.nodalConstraints.RemoveAt(idx);
        dataLoad(idx);   // selects the neighbour
    }

    private void btNewR_Click(object sender, EventArgs e)
    {
        var name = Mis.InputBox($"{I18n.New} {RestType.SelectedItem}", $"{I18n.Name}: ")?.ToString();
        if (string.IsNullOrWhiteSpace(name)) return;
        name = name.Trim();

        if (App.model.nodalConstraints.Any(c => c.label == name))
        {
            MessageBox.Show($"{I18n.CheckInput}\n\"{name}\"");
            return;
        }

        // Carry over the typed nodes if they are well formed; otherwise start empty
        string nodes = Mis.tryParseIntList(TbListaNodos.Text, out int[] ids, out _) ? Mis.joinIntList(ids) : "";

        App.takeUndoSnapShot();

        var r = new nodalConstraint()
        {
            ID = App.model.nodalConstraints.NextID(),
            label = name,
            Type = RestType.SelectedIndex,
            nodesIDList = nodes
        };
        App.model.nodalConstraints.Add(r);

        dataLoad(App.model.nodalConstraints.Count - 1);   // select the new one
    }
}