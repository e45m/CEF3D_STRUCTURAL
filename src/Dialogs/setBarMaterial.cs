using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CEF;

public class setBarMaterial : Form
{
    #region UI

    private ComboBox cbMaterials;
    private Button btAccept;
    private Button btCancel;

    private TextBox txtNombre, txtE, txtG, txtV, txtW, txtM, txtFc, txtFy, txtFu;
    private ComboBox cbTipo;

    private void InitializeComponent()
    {
        // --- Selector
        cbMaterials = new ComboBox
        {
            Left = 10,
            Top = 10,
            Width = 330,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        btAccept = new Button { Text = "Aceptar", Left = 160, Top = 300, Width = 80 };
        btCancel = new Button { Text = "Cancelar", Left = 250, Top = 300, Width = 80 };

        cbMaterials.SelectedIndexChanged += OnMaterialChanged;
        btAccept.Click += OnAccept;
        btCancel.Click += (_, _) => Close();

        Controls.Add(cbMaterials);
        Controls.Add(btAccept);
        Controls.Add(btCancel);

        // --- Material detail form
        int y = 45;

        txtNombre = AddField(I18n.Name, ref y);
        cbTipo = AddCombo(I18n.Type, ref y, Enum.GetNames(typeof(materialType)));

        txtE = AddField("E", ref y);
        txtG = AddField("G", ref y);
        txtV = AddField("ν", ref y);
        txtW = AddField("W", ref y);
        txtM = AddField("M", ref y);
        txtFc = AddField("fc", ref y);
        txtFy = AddField("Fy", ref y);
        txtFu = AddField("Fu", ref y);

        Text = I18n.AssignMaterialToBars;
        Width = 370;
        Height = y + 80;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
    }

    private TextBox AddField(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Left = 10, Top = y + 3, Width = 100 });
        var tb = new TextBox { Left = 120, Top = y, Width = 220, ReadOnly = true };
        Controls.Add(tb);
        y += 26;
        return tb;
    }

    private ComboBox AddCombo(string label, ref int y, string[] items)
    {
        Controls.Add(new Label { Text = label, Left = 10, Top = y + 3, Width = 100 });
        var cb = new ComboBox
        {
            Left = 120,
            Top = y,
            Width = 220,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Enabled = false
        };
        cb.Items.AddRange(items);
        Controls.Add(cb);
        y += 26;
        return cb;
    }

    #endregion

    public setBarMaterial()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        LoadMaterials();
    }

    #region Load

    private void LoadMaterials()
    {
        cbMaterials.Items.Clear();

        foreach (var m in App.model.Materials)
            cbMaterials.Items.Add($"{m.ID}: {m.Name}");

        if (cbMaterials.Items.Count > 0)
            cbMaterials.SelectedIndex = 0;
    }

    #endregion

    #region Events

    private void OnMaterialChanged(object sender, EventArgs e)
    {
        if (cbMaterials.SelectedIndex < 0) return;

        var m = App.model.Materials[cbMaterials.SelectedIndex];

        ShowMaterial(m);
    }

    private void OnAccept(object sender, EventArgs e)
    {
        if (cbMaterials.SelectedIndex < 0)
        {
            return;
        }

        App.takeUndoSnapShot();

        var selectedMaterial = App.model.Materials[cbMaterials.SelectedIndex];

        int nBar = Selection.barMaxIndex;
        var sel = Selection.getBarsColection();

        App. materialDefaultID = selectedMaterial.ID;

        foreach (var i in sel)
        {
            var Bar = App.model.bars[i];

            Bar.MaterialID = selectedMaterial.ID;
            Bar.MaterialName = selectedMaterial.Name;
        }

        Close();
    }

    #endregion

    #region UI Helpers

    private void ShowMaterial(Material m)
    {
        txtNombre.Text = m.Name;
        cbTipo.SelectedIndex = m.Type;

        txtE.Text =Uc.forcePerUnitAreaToUserUnits(m.E).ToString();
        txtG.Text =Uc.forcePerUnitAreaToUserUnits(m.G).ToString();
        txtV.Text = m.v.ToString();//adimentional
        txtW.Text =Uc.forceToUserUnits(m.W).ToString();
        txtM.Text = Uc.massToUserUnits(m.M).ToString();
        txtFc.Text =Uc.forcePerUnitAreaToUserUnits(m.fc).ToString();
        txtFy.Text =Uc.forcePerUnitAreaToUserUnits(m.fy).ToString();
        txtFu.Text =Uc.forcePerUnitAreaToUserUnits(m.fu).ToString();
    }

    #endregion
}