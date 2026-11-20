using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CEF;

public class addMaterialToModel : Form
{
    private const string defaultDb = "dbApp\\materials.cefdb";

    private Button btLoad, btImport, btCreate, btPrev, btNext, btSave, btCancel;
    private TextBox txtNombre, txtE, txtG, txtV, txtW, txtM, txtFc, txtFy, txtFu;
    private ComboBox cbTipo;
    private Label lblIndex;

    private int _currentIndex = 0;

    #region UI

    private void InitializeComponent()
    {
        dlgStyle.applyForm(this);
        Text = I18n.materials;
        ClientSize = new Size(440, 300);

        // --- Action bar
        var bar = dlgStyle.actionBar();

        btLoad = dlgStyle.button(I18n.LoadDB, 10, 8, 110);
        btImport = dlgStyle.button(I18n.Import, 130, 8, 110);
        btCreate = dlgStyle.button(I18n.EditCreate, 250, 8, 160);

        btPrev = dlgStyle.button(I18n.leftArrow, 10, 44, 40);
        btNext = dlgStyle.button(I18n.rightArrow, 55, 44, 40);
        lblIndex = new Label { Left = 105, Top = 49, Width = 100 };

        btSave = dlgStyle.button(I18n.Save, 250, 44, 75, true);
        btCancel = dlgStyle.button(I18n.DlgCancelText, 335, 44, 75);
        btSave.Visible = false;
        btCancel.Visible = false;

        bar.Controls.AddRange(new Control[] { btLoad, btImport, btCreate, btPrev, btNext, lblIndex, btSave, btCancel });

        // --- Material form
        var gb = dlgStyle.group(I18n.materials, 10, dlgStyle.BarHeight + 8, 420, 200);

        txtNombre = dlgStyle.field(gb, I18n.Name, 10, 24, 70, 325);
        cbTipo = dlgStyle.combo(gb, I18n.Type, 10, 52, 70, 325, Enum.GetNames(typeof(materialType)));

        txtE = dlgStyle.field(gb, "E", 10, 80, 70, 130);
        txtG = dlgStyle.field(gb, "G", 230, 80, 40, 135);
        txtV = dlgStyle.field(gb, "ν", 10, 108, 70, 130);
        txtW = dlgStyle.field(gb, "W", 230, 108, 40, 135);
        txtM = dlgStyle.field(gb, "M", 10, 136, 70, 130);
        txtFc = dlgStyle.field(gb, "fc", 230, 136, 40, 135);
        txtFy = dlgStyle.field(gb, "Fy", 10, 164, 70, 130);
        txtFu = dlgStyle.field(gb, "Fu", 230, 164, 40, 135);

        Controls.Add(gb);
        Controls.Add(bar);

        // --- Events
        btLoad.Click += OnLoadClick;
        btImport.Click += OnImportClick;
        btCreate.Click += OnCreateClick;
        btSave.Click += OnSaveClick;
        btCancel.Click += OnCancelClick;
        btPrev.Click += (_, _) => Navigate(-1);
        btNext.Click += (_, _) => Navigate(1);
    }

    #endregion

    public addMaterialToModel()
    {
        InitializeComponent();
        loadDataBase(defaultDb);
        Navigate(0);
    }

    #region Library

    private bool loadDataBase(string path)
    {
        if (!File.Exists(path))
        {
            MessageBox.Show(I18n.CannotLoadDB);
            return false;
        }

        Engine.getAppData().LoadMaterialsDataBase(path);

        if (!Engine.getAppData().libMaterials.Any())
        {
            MessageBox.Show(I18n.CannotLoadDB);
            return false;
        }
        return true;
    }

    // Opens an additional materials database
    private void OnLoadClick(object sender, EventArgs e)
    {
        string file = dlgStyle.openDbFile();
        if (file == "") return;

        if (!loadDataBase(file))
            loadDataBase(defaultDb);   // fall back to the default library

        _currentIndex = 0;
        if (Engine.getAppData().libMaterials.Count > 0)
            ShowMaterial(_currentIndex);
    }

    private void Navigate(int step)
    {
        int newIndex = _currentIndex + step;

        if (newIndex < 0 || newIndex >= Engine.getAppData().libMaterials.Count)
            return;

        _currentIndex = newIndex;
        ShowMaterial(_currentIndex);
    }

    private void ShowMaterial(int index)
    {
        var m = Engine.getAppData().libMaterials[index];

        txtNombre.Text = m.Name;
        cbTipo.SelectedIndex = m.Type;

        txtE.Text = Uc.forcePerUnitAreaToUserUnits(m.E).ToString();
        txtG.Text = Uc.forcePerUnitAreaToUserUnits(m.G).ToString();
        txtV.Text = m.v.ToString();   // dimensionless
        txtW.Text = Uc.forceToUserUnits(m.W).ToString();
        txtM.Text = Uc.massToUserUnits(m.M).ToString();
        txtFc.Text = Uc.forcePerUnitAreaToUserUnits(m.fc).ToString();
        txtFy.Text = Uc.forcePerUnitAreaToUserUnits(m.fy).ToString();
        txtFu.Text = Uc.forcePerUnitAreaToUserUnits(m.fu).ToString();

        lblIndex.Text = $"{index + 1} / {Engine.getAppData().libMaterials.Count}";
    }

    private void OnImportClick(object sender, EventArgs e)
    {
        var src = Engine.getAppData().libMaterials[_currentIndex];
        var mat = materialClone(src);

        mat.ID = ResolveId(mat.ID);

        Engine.getModel().materials.Add(mat);
    }

    #endregion

    #region Create

    private void OnCreateClick(object sender, EventArgs e)
    {
        setEnabledCtrls(true);
    }

    private void OnSaveClick(object sender, EventArgs e)
    {
        var mat = ReadFromUI();
        ApplyRules(mat);

        // Reject empty/incomplete records (no name or no stiffness)
        if (string.IsNullOrWhiteSpace(mat.Name) || mat.E <= 0)
            return;

        mat.ID = Engine.getModel().materials.NextID();   // new material: never reuse the unset ID (0)
        Engine.getModel().materials.Add(mat);

        setEnabledCtrls(false);
        ShowMaterial(_currentIndex);
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        setEnabledCtrls(false);
        ShowMaterial(_currentIndex);
    }

    private void setEnabledCtrls(bool state = true)
    {
        dlgStyle.setEditable(txtNombre, state);
        dlgStyle.setEditable(txtE, state);
        dlgStyle.setEditable(txtG, state);
        dlgStyle.setEditable(txtV, state);
        dlgStyle.setEditable(txtW, state);
        dlgStyle.setEditable(txtM, state);
        dlgStyle.setEditable(txtFc, state);
        dlgStyle.setEditable(txtFy, state);
        dlgStyle.setEditable(txtFu, state);
        cbTipo.Enabled = state;

        btSave.Visible = state;
        btCancel.Visible = state;
        btCreate.Enabled = !state;

        btImport.Visible = !state;
        btNext.Visible = !state;
        btPrev.Visible = !state;
        lblIndex.Visible = !state;
        btLoad.Visible = !state;
    }

    private Material ReadFromUI()
    {
        return new Material
        {
            Name = txtNombre.Text,
            Type = cbTipo.SelectedIndex,
            E = Uc.pressFromUserUnits(Parse(txtE)),
            G = Uc.pressFromUserUnits(Parse(txtG)),
            v = Parse(txtV),   // dimensionless
            W = Uc.forceFromUserUnits(Parse(txtW)),
            M = Uc.massFromUserUnits(Parse(txtM)),
            fc = Uc.pressFromUserUnits(Parse(txtFc)),
            fy = Uc.pressFromUserUnits(Parse(txtFy)),
            fu = Uc.pressFromUserUnits(Parse(txtFu))
        };
    }

    private Material materialClone(Material m)
    {
        return new Material
        {
            ID = m.ID,
            Name = m.Name,
            Type = m.Type,
            E = m.E,
            G = m.G,
            v = m.v,
            W = m.W,
            M = m.M,
            fc = m.fc,
            fy = m.fy,
            fu = m.fu
        };
    }

    private int ResolveId(int id)
    {
        if (Engine.getModel().materials.Any(m => m.ID == id))
            return Engine.getModel().materials.NextID();

        return id;
    }

    private void ApplyRules(Material m)
    {
        switch ((materialType)m.Type)
        {
            case materialType.Concrete:
                m.fy = 0;
                m.fu = 0;
                if (m.E == 0)
                    m.E = 3700 * Math.Sqrt(m.fc);
                break;

            case materialType.Steel:
                m.fc = 0;
                break;
        }

        if (m.v <= 0) m.v = 0.3;

        if (m.W <= 0)
            m.W = m.Type switch
            {
                (int)materialType.Steel => 79,
                (int)materialType.Concrete => 24,
                (int)materialType.Aluminium => 27,
                (int)materialType.Wood => 5,
                _ => 1
            };

        if (m.M <= 0)
            m.M = m.W * 98.1;
    }

    private double Parse(TextBox tb)
    {
        return double.TryParse(tb.Text, out var v) ? v : 0;
    }

    #endregion
}