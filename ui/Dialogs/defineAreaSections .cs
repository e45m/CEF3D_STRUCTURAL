using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CEF.AreaElement;
namespace CEF;

public class defineAreaSection : Form
{
    private Button btCreate, btEdit, btPrev, btNext, btSave, btCancel, btMaterial;
    private TextBox  txtNombre, txtMaterial, txtThickness;
    private ComboBox cbKind;
    private Label lblIndex;
    private int[] kindVals;

    private int _currentIndex = 0;
    private int _materialId = -1;   // material of the area section being created/edited
    private int _editId = -1;       // -1 = new section, otherwise ID of the section being edited

    // Collection of area sections in the model (adjust the name if it differs)
    private static DtaTable<areaSection> list => Engine.getModel().areaSections;

    private static void setMatId(areaSection s, int id)
    {
        int idx = Engine.getModel().materials.FindIndexByID(id);

        s.MaterialID = id;
        s.MaterialName = idx < 0 ? "" : Engine.getModel().materials[idx].Name;   // keep name and ID in sync
    }

    // Hole and None carry no material or thickness
    private static bool needsMaterial(int kind) =>
        (areaKind)kind != areaKind.Hole && (areaKind)kind != areaKind.None;

    #region UI

    private void InitializeComponent()
    {
        dlgStyle.applyForm(this);
        Text = I18n.sections;

        // --- Action bar
        var bar = dlgStyle.actionBar();

        btCreate = dlgStyle.button(I18n.New, 10, 8, 110);
        btEdit = dlgStyle.button(I18n.Edit, 130, 8, 110);

        btPrev = dlgStyle.button(I18n.leftArrow, 10, 44, 40);
        btNext = dlgStyle.button(I18n.rightArrow, 55, 44, 40);
        lblIndex = new Label { Left = 105, Top = 49, Width = 120 };

        btSave = dlgStyle.button(I18n.Save, 250, 44, 100, true);
        btCancel = dlgStyle.button(I18n.DlgCancelText, 360, 44, 100);
        btSave.Visible = false;
        btCancel.Visible = false;

        bar.Controls.AddRange(new Control[] { btCreate, btEdit, btPrev, btNext, lblIndex, btSave, btCancel });

        // --- Properties group
        int top = dlgStyle.BarHeight + 8;
        var gb = dlgStyle.group(I18n.sections, 10, top, 480, 170);

       
        txtNombre = dlgStyle.field(gb, I18n.Name, 10, 52, 95, 355);
        cbKind = dlgStyle.combo(gb, I18n.Type, 10, 80, 95, 355, Enum.GetNames(typeof(areaKind)));
        kindVals = Enum.GetValues<areaKind>().Select(v => (int)v).ToArray();

        dlgStyle.label(gb, I18n.MaterialLabel, 10, 108, 95);
        txtMaterial = dlgStyle.textBox(gb, 105, 108, 315);
        btMaterial = dlgStyle.button("…", 428, 107, 32);
        gb.Controls.Add(btMaterial);

        txtThickness = dlgStyle.field(gb, "t", 10, 136, 95, 130);

        Controls.Add(gb);
        Controls.Add(bar);

        ClientSize = new Size(500, top + 170 + 10);

        // --- Events
        btCreate.Click += OnCreateClick;
        btEdit.Click += OnEditClick;
        btSave.Click += OnSaveClick;
        btCancel.Click += OnCancelClick;
        btMaterial.Click += OnMaterialClick;
        cbKind.SelectedIndexChanged += (_, _) => updateKindState();
        btPrev.Click += (_, _) => Navigate(-1);
        btNext.Click += (_, _) => Navigate(1);
    }

    #endregion

    public defineAreaSection()
    {
        InitializeComponent();
        setEnabledCtrls(false);
        Navigate(0);
    }

    #region Browse

    private void Navigate(int step)
    {
        if (list.Count == 0)
        {
            clearFields();
            lblIndex.Text = "0 / 0";
            btEdit.Enabled = false;
            return;
        }

        int newIndex = _currentIndex + step;
        if (newIndex < 0 || newIndex >= list.Count)
            return;

        _currentIndex = newIndex;
        ShowSection(_currentIndex);
    }

    private void ShowSection(int index)
    {
        var s = list[index];

        
        txtNombre.Text = s.Name;
        cbKind.SelectedIndex = Array.FindIndex(kindVals, v => v == s.Kind);
        _materialId = s.MaterialID;
        refreshMaterial();
        txtThickness.Text = Uc.lenToUserUnits(s.Thickness).ToString();

        lblIndex.Text = $"{index + 1} / {list.Count}";
        btEdit.Enabled = true;
    }

    private void clearFields()
    {
        
        txtNombre.Text = "";
        cbKind.SelectedIndex = -1;
        _materialId = -1;
        txtMaterial.Text = "-";
        txtThickness.Text = "";
    }

    #endregion

    #region Material

    private string materialNameOf(int id)
    {
        int idx = Engine.getModel().materials.FindIndexByID(id);
        return idx < 0 ? "-" : $"{id}: {Engine.getModel().materials[idx].Name}";
    }

    // Keeps the user's choice if it still exists in the model, otherwise clears it (-1). Never assigns a default.
    private void refreshMaterial()
    {
        if (Engine.getModel().materials.FindIndexByID(_materialId) < 0)
            _materialId = -1;

        txtMaterial.Text = materialNameOf(_materialId);
    }

    // Guarantees the user has explicitly chosen a material before saving
    private bool ensureMaterial()
    {
        refreshMaterial();
        if (_materialId >= 0) return true;

        int id = selectMaterial.Pick(this);   // its "New..." button lets the user create the first one
        if (id >= 0) _materialId = id;

        refreshMaterial();
        return _materialId >= 0;
    }

    private void OnMaterialClick(object sender, EventArgs e)
    {
        int id = selectMaterial.Pick(this, _materialId);
        if (id < 0) return;

        _materialId = id;
        txtMaterial.Text = materialNameOf(id);
    }

    #endregion

    #region Create / Edit

    private void OnCreateClick(object sender, EventArgs e)
    {
        _editId = -1;
        clearFields();
        cbKind.SelectedIndex = 0;
        setEnabledCtrls(true);
        updateKindState();
    }

    private void OnEditClick(object sender, EventArgs e)
    {
        if (list.Count == 0) return;

        _editId = list[_currentIndex].ID;
        setEnabledCtrls(true);
        updateKindState();
    }

    private void OnSaveClick(object sender, EventArgs e)
    {
        if (cbKind.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtNombre.Text))
            return;

        var sec = ReadFromUI();

        if (needsMaterial(sec.Kind))
        {
            if (sec.Thickness <= 0) return;
            if (!ensureMaterial()) return;
            setMatId(sec, _materialId);
        }
        else
        {
            sec.Thickness = 0;
            setMatId(sec, -1);
        }

        if (_editId < 0)
        {
            sec.ID = list.NextID();   // new section: never reuse the unset ID (0)
            list.Add(sec);
            _currentIndex = list.Count - 1;
        }
        else
        {
            sec.ID = _editId;
            int idx = list.FindIndexByID(_editId);
            list[idx] = sec;
            _currentIndex = idx;
        }

        setEnabledCtrls(false);
        ShowSection(_currentIndex);
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        setEnabledCtrls(false);
        Navigate(0);
    }

    private areaSection ReadFromUI()
    {
        return new areaSection
        {
            
            Name = txtNombre.Text,
            Kind = kindVals[cbKind.SelectedIndex],
            Thickness = Uc.lenFromUserUnits(txtThickness.Text)
        };
    }

    // Material and thickness only apply to kinds that have them
    private void updateKindState()
    {
        if (cbKind.SelectedIndex < 0) return;

        bool on = btSave.Visible && needsMaterial(kindVals[cbKind.SelectedIndex]);
        dlgStyle.setEditable(txtThickness, on);
        btMaterial.Enabled = on;
    }

    private void setEnabledCtrls(bool state)
    {
        dlgStyle.setEditable(txtNombre, state);
        dlgStyle.setEditable(txtThickness, state);
        cbKind.Enabled = state;
        btMaterial.Enabled = state;

        btSave.Visible = state;
        btCancel.Visible = state;
        btCreate.Enabled = !state;
        btEdit.Enabled = !state && list.Count > 0;

        btNext.Visible = !state;
        btPrev.Visible = !state;
        lblIndex.Visible = !state;
    }

    #endregion
}