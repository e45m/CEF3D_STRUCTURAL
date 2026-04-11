using System;
using System.Linq;
using System.Windows.Forms;

namespace CEF;

public class setBarSection : Form
{
    #region UI

    private ComboBox cbSections;
    private Button btAccept;
    private Button btCancel;

    private TextBox txtNombre, txtH, txtB, txtTw, txtTf;
    private TextBox txtAg, txtIyy, txtIzz, txtIyz, txtTor;
    private TextBox txtAcyy, txtAczz, txtSyy, txtSzz;
    private TextBox txtPyy, txtPzz, txtRyy, txtRzz;
    private ComboBox cbTipo;

    private void InitializeComponent()
    {
        cbSections = new ComboBox
        {
            Left = 10,
            Top = 10,
            Width = 340,
            DropDownStyle = ComboBoxStyle.DropDownList
        };



        int y = 45;

        txtNombre = AddField(I18n.Name, ref y);
        cbTipo = AddCombo(I18n.Type, ref y, Enum.GetNames(typeof(sectType)));

        txtH = AddField("h", ref y);
        txtB = AddField("b", ref y);
        txtTw = AddField("tw", ref y);
        txtTf = AddField("tf", ref y);

        txtAg = AddField("Ag", ref y);
        txtIyy = AddField("Iyy", ref y);
        txtIzz = AddField("Izz", ref y);
        txtIyz = AddField("Iyz", ref y);
        txtTor = AddField("Tor", ref y);

        txtAcyy = AddField("Acyy", ref y);
        txtAczz = AddField("Aczz", ref y);
        txtSyy = AddField("Syy", ref y);
        txtSzz = AddField("Szz", ref y);

        txtPyy = AddField("Pyy", ref y);
        txtPzz = AddField("Pzz", ref y);
        txtRyy = AddField("ryy", ref y);
        txtRzz = AddField("rzz", ref y);

        y += 52;

        btAccept = new Button { Text = I18n.DlgOKText, Left = 180, Top = y, Width = 80 };
        btCancel = new Button { Text = I18n.DlgCancelText, Left = 270, Top = y, Width = 80 };


        cbSections.SelectedIndexChanged += OnSelectionChanged;
        btAccept.Click += OnAccept;
        btCancel.Click += (_, _) => Close();

        Text = I18n.AssignSectionToBars;
        Width = 380;
        Height = y + 70;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;

        Controls.Add(cbSections);
        Controls.Add(btAccept);
        Controls.Add(btCancel);

        
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

    public setBarSection()
    {
        InitializeComponent();
        LoadSections();
    }





    private void LoadSections()
    {
        cbSections.Items.Clear();

        foreach (var s in App.model.sections)
            cbSections.Items.Add($"{s.ID}: {s.Name}");

        if (cbSections.Items.Count > 0)
            cbSections.SelectedIndex = 0;
      

        ShowSection(App.model.sections[0]);
    }



    private void OnSelectionChanged(object sender, EventArgs e)
    {
        if (cbSections.SelectedIndex < 0) return;

        var s = App.model.sections[cbSections.SelectedIndex];

        ShowSection(s);
    }

    private void OnAccept(object sender, EventArgs e)
    {
        if (cbSections.SelectedIndex < 0)
        {
            return;
        }

        App.takeUndoSnapShot();

        var selected = App.model.sections[cbSections.SelectedIndex];

        int nBar = Selection.barMaxIndex;
        var sel =  Selection.getBarsColection();

        App.sectionDefaultID = selected.ID;

        foreach (var i in sel)
        {
            var bar = App.model.bars[i];

            bar.SectionID = selected.ID;
            bar.SectionName = selected.Name;
        }

        Close();
    }



    private void ShowSection(section s)
    {
        txtNombre.Text = s.Name;
        cbTipo.SelectedIndex = Array.FindIndex(
            Enum.GetValues<sectType>().Select(v => (int)v).ToArray(),
            v => v == s.Type);

        txtH.Text = uc.lenToUserUnits(s.h).ToString();
        txtB.Text = uc.lenToUserUnits(s.b).ToString();
        txtTw.Text = uc.lenToUserUnits(s.tw).ToString();
        txtTf.Text = uc.lenToUserUnits(s.tf).ToString();

        txtAg.Text = uc.areaToUserUnits(s.Ag).ToString();
        txtIyy.Text = uc.len4ToUserUnits(s.Iyy).ToString();
        txtIzz.Text = uc.len4ToUserUnits(s.Izz).ToString();
        txtIyz.Text = uc.len4ToUserUnits(s.Iyz).ToString();
        txtTor.Text = uc.len4ToUserUnits(s.Tor).ToString();

        txtAcyy.Text = uc.areaToUserUnits(s.Acyy).ToString();
        txtAczz.Text = uc.areaToUserUnits(s.Aczz).ToString();
        txtSyy.Text = uc.len3ToUserUnits(s.Syy).ToString();
        txtSzz.Text = uc.len3ToUserUnits(s.Szz).ToString();

        txtPyy.Text = uc.len4ToUserUnits(s.Pyy).ToString();
        txtPzz.Text = uc.len4ToUserUnits(s.Pzz).ToString();
        txtRyy.Text = uc.lenToUserUnits(s.ryy).ToString();
        txtRzz.Text = uc.lenToUserUnits(s.rzz).ToString();

    }
}