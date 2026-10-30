using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CEF;

public class addSectionsToModel : Form
{
    private const string defaultDb = "dbApp\\sections.cefdb";

    private Button btLoad, btImport, btCreate, btCalc, btPrev, btNext, btSave, btCancel, btMaterial;

    private TextBox txtNombre, txtMaterial, txtH, txtB, txtTw, txtTf;
    private TextBox txtAg, txtIyy, txtIzz, txtIyz, txtTor;
    private TextBox txtAcyy, txtAczz, txtSyy, txtSzz;
    private TextBox txtPyy, txtPzz, txtRyy, txtRzz;

    private ComboBox cbTipo;
    private Label lblIndex;
    private int[] secTypeVals;

    private int _currentIndex = 0;
    private int _materialId = -1;   // material of the section being created

    // The only two places that touch the Section <-> material link: adjust here if the field differs
    private static int matIdOf(Section s) { return s.MaterialID; }
    private static void setMatId(Section s, int id)
    {
        int idx = App.model.materials.FindIndexByID(id);

        s.MaterialID = id;
        s.MaterialName = idx < 0 ? "" : App.model.materials[idx].Name;   // keep name and ID in sync
    }

    #region UI

    private void InitializeComponent()
    {
        dlgStyle.applyForm(this);
        Text = I18n.sections;
        ClientSize = new Size(500, 444);

        // --- Action bar
        var bar = dlgStyle.actionBar();

        btLoad = dlgStyle.button(I18n.LoadDB, 10, 8, 110);
        btCalc = dlgStyle.button(I18n.CalProps, 10, 8, 110);
        btCalc.Visible = false;
        btImport = dlgStyle.button(I18n.Import, 130, 8, 110);
        btCreate = dlgStyle.button(I18n.EditCreateDelete, 250, 8, 160);

        btPrev = dlgStyle.button(I18n.leftArrow, 10, 44, 40);
        btNext = dlgStyle.button(I18n.rightArrow, 55, 44, 40);
        lblIndex = new Label { Left = 105, Top = 49, Width = 120 };

        btSave = dlgStyle.button(I18n.Save, 250, 44, 100, true);
        btCancel = dlgStyle.button(I18n.DlgCancelText, 360, 44, 100);
        btSave.Visible = false;
        btCancel.Visible = false;

        bar.Controls.AddRange(new Control[] { btLoad, btCalc, btImport, btCreate, btPrev, btNext, lblIndex, btSave, btCancel });

        // --- Geometry group
        int top = dlgStyle.BarHeight + 8;
        var gbGeom = dlgStyle.group(I18n.sections, 10, top, 480, 170);

        txtNombre = dlgStyle.field(gbGeom, I18n.Name, 10, 24, 95, 355);
        cbTipo = dlgStyle.combo(gbGeom, I18n.Type, 10, 52, 95, 355, Enum.GetNames(typeof(sectType)));
        secTypeVals = Enum.GetValues<sectType>().Select(v => (int)v).ToArray();

        dlgStyle.label(gbGeom, I18n.MaterialLabel, 10, 80, 95);
        txtMaterial = dlgStyle.textBox(gbGeom, 105, 80, 315);
        btMaterial = dlgStyle.button("…", 428, 79, 32);
        gbGeom.Controls.Add(btMaterial);

        txtH = dlgStyle.field(gbGeom, "h", 10, 108, 95, 130);
        txtB = dlgStyle.field(gbGeom, "b", 250, 108, 75, 135);
        txtTw = dlgStyle.field(gbGeom, "tw", 10, 136, 95, 130);
        txtTf = dlgStyle.field(gbGeom, "tf", 250, 136, 75, 135);

        // --- Properties group (three columns)
        var gbProps = dlgStyle.group(I18n.CalProps, 10, top + 178, 480, 168);

        txtAg = dlgStyle.field(gbProps, "Ag", 10, 24, 45, 90);
        txtIyy = dlgStyle.field(gbProps, "Iyy", 10, 50, 45, 90);
        txtIzz = dlgStyle.field(gbProps, "Izz", 10, 76, 45, 90);
        txtIyz = dlgStyle.field(gbProps, "Iyz", 10, 102, 45, 90);
        txtTor = dlgStyle.field(gbProps, "Tor", 10, 128, 45, 90);

        txtAcyy = dlgStyle.field(gbProps, "Acyy", 165, 24, 45, 90);
        txtAczz = dlgStyle.field(gbProps, "Aczz", 165, 50, 45, 90);
        txtSyy = dlgStyle.field(gbProps, "Syy", 165, 76, 45, 90);
        txtSzz = dlgStyle.field(gbProps, "Szz", 165, 102, 45, 90);

        txtPyy = dlgStyle.field(gbProps, "Pyy", 320, 24, 45, 90);
        txtPzz = dlgStyle.field(gbProps, "Pzz", 320, 50, 45, 90);
        txtRyy = dlgStyle.field(gbProps, "ryy", 320, 76, 45, 90);
        txtRzz = dlgStyle.field(gbProps, "rzz", 320, 102, 45, 90);

        Controls.Add(gbGeom);
        Controls.Add(gbProps);
        Controls.Add(bar);

        // --- Events
        btLoad.Click += OnLoadClick;
        btImport.Click += OnImportClick;
        btCreate.Click += OnCreateClick;
        btCalc.Click += OnCalcClick;
        btSave.Click += OnSaveClick;
        btCancel.Click += OnCancelClick;
        btMaterial.Click += OnMaterialClick;
        btPrev.Click += (_, _) => Navigate(-1);
        btNext.Click += (_, _) => Navigate(1);
    }

    #endregion

    public addSectionsToModel()
    {
        InitializeComponent();
        loadDataBase(defaultDb);
        Navigate(0);
        refreshMaterial();
    }

    #region Library

    private bool loadDataBase(string path)
    {
        if (!File.Exists(path))
        {
            MessageBox.Show(I18n.CannotLoadDB);
            return false;
        }

        App.appData.LoadSectionsDataBase(path);

        if (!App.appData.libSections.Any())
        {
            MessageBox.Show(I18n.CannotLoadDB);
            return false;
        }
        return true;
    }

    // Opens an additional sections database
    private void OnLoadClick(object sender, EventArgs e)
    {
        string file = dlgStyle.openDbFile();
        if (file == "") return;

        if (!loadDataBase(file))
            loadDataBase(defaultDb);   // fall back to the default library

        _currentIndex = 0;
        if (App.appData.libSections.Count > 0)
            ShowSection(_currentIndex);
    }

    private void Navigate(int step)
    {
        int newIndex = _currentIndex + step;

        if (newIndex < 0 || newIndex >= App.appData.libSections.Count)
            return;

        _currentIndex = newIndex;
        ShowSection(_currentIndex);
    }

    private void ShowSection(int index)
    {
        var s = App.appData.libSections[index];

        txtNombre.Text = s.Name;
        cbTipo.SelectedIndex = Array.FindIndex(secTypeVals, v => v == s.Type);
        refreshMaterial();   // material is a user choice, not taken from the library entry

        fillFields(s);

        lblIndex.Text = $"{index + 1} / {App.appData.libSections.Count}";
    }

    private void fillFields(Section s)
    {
        txtH.Text = Uc.lenToUserUnits(s.h).ToString();
        txtB.Text = Uc.lenToUserUnits(s.b).ToString();
        txtTw.Text = Uc.lenToUserUnits(s.tw).ToString();
        txtTf.Text = Uc.lenToUserUnits(s.tf).ToString();

        txtAg.Text = Uc.areaToUserUnits(s.Ag).ToString();
        txtIyy.Text = Uc.len4ToUserUnits(s.Iyy).ToString();
        txtIzz.Text = Uc.len4ToUserUnits(s.Izz).ToString();
        txtIyz.Text = Uc.len4ToUserUnits(s.Iyz).ToString();
        txtTor.Text = Uc.len4ToUserUnits(s.Tor).ToString();

        txtAcyy.Text = Uc.areaToUserUnits(s.Acyy).ToString();
        txtAczz.Text = Uc.areaToUserUnits(s.Aczz).ToString();
        txtSyy.Text = Uc.len3ToUserUnits(s.Syy).ToString();
        txtSzz.Text = Uc.len3ToUserUnits(s.Szz).ToString();

        txtPyy.Text = Uc.len4ToUserUnits(s.Pyy).ToString();
        txtPzz.Text = Uc.len4ToUserUnits(s.Pzz).ToString();
        txtRyy.Text = Uc.lenToUserUnits(s.ryy).ToString();
        txtRzz.Text = Uc.lenToUserUnits(s.rzz).ToString();
    }

    private void OnImportClick(object sender, EventArgs e)
    {
        var src = App.appData.libSections[_currentIndex];
        if (!ensureMaterial()) return;

        var sec = sectClone(src);
        setMatId(sec, _materialId);

        sec.ID = ResolveId(sec.ID);
        App.model.sections.Add(sec);
    }

    #endregion

    #region Material

    private string materialNameOf(int id)
    {
        int idx = App.model.materials.FindIndexByID(id);
        return idx < 0 ? "-" : $"{id}: {App.model.materials[idx].Name}";
    }

    // Keeps the user's choice if it still exists in the model, otherwise clears it (-1). Never assigns a default.
    private void refreshMaterial()
    {
        if (App.model.materials.FindIndexByID(_materialId) < 0)
            _materialId = -1;

        txtMaterial.Text = materialNameOf(_materialId);
    }

    // Guarantees the user has explicitly chosen a material before importing/saving
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

    #region Create

    private void OnCreateClick(object sender, EventArgs e)
    {
        setEnabledCtrls(true);
        setNATb();
        refreshMaterial();
    }

    private void OnSaveClick(object sender, EventArgs e)
    {
        var sec = ReadFromUI();
        ApplyRules(sec);

        // Reject empty/incomplete records (no name or no computable area)
        if (string.IsNullOrWhiteSpace(sec.Name) || sec.Ag <= 0)
            return;

        if (!ensureMaterial()) return;
        setMatId(sec, _materialId);

        sec.ID = App.model.sections.NextID();   // new section: never reuse the unset ID (0)
        App.model.sections.Add(sec);

        setEnabledCtrls(false);
        ShowSection(_currentIndex);
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        setEnabledCtrls(false);
        ShowSection(_currentIndex);
    }

    private void setEnabledCtrls(bool state = true)
    {
        dlgStyle.setEditable(txtNombre, state);
        dlgStyle.setEditable(txtH, state);
        dlgStyle.setEditable(txtB, state);
        dlgStyle.setEditable(txtTw, state);
        dlgStyle.setEditable(txtTf, state);
        cbTipo.Enabled = state;

        btCalc.Visible = state;
        btSave.Visible = state;
        btCancel.Visible = state;
        btCreate.Enabled = !state;

        btImport.Visible = !state;
        btNext.Visible = !state;
        btPrev.Visible = !state;
        lblIndex.Visible = !state;
        btLoad.Visible = !state;
    }

    private void setNATb()
    {
        txtAg.Text = "-";
        txtIyy.Text = "-";
        txtIzz.Text = "-";
        txtIyz.Text = "-";
        txtTor.Text = "-";

        txtAcyy.Text = "-";
        txtAczz.Text = "-";
        txtSyy.Text = "-";
        txtSzz.Text = "-";

        txtPyy.Text = "-";
        txtPzz.Text = "-";
        txtRyy.Text = "-";
        txtRzz.Text = "-";
    }

    private void OnCalcClick(object sender, EventArgs e)
    {
        var s = ReadFromUI();
        ApplyRules(s);
        fillFields(s);
    }

    private Section ReadFromUI()
    {
        var s = new Section
        {
            Name = txtNombre.Text,
                

            Type = secTypeVals[cbTipo.SelectedIndex],

            h = Uc.lenFromUserUnits(txtH.Text),
            b = Uc.lenFromUserUnits(txtB.Text),
            tw = Uc.lenFromUserUnits(txtTw.Text),
            tf = Uc.lenFromUserUnits(txtTf.Text),
            Ag = Uc.areaFromUserUnits(txtAg.Text),
            Iyy = Uc.len4FromUserUnits(txtIyy.Text),
            Izz = Uc.len4FromUserUnits(txtIzz.Text),
            Iyz = Uc.len4FromUserUnits(txtIyz.Text),
            Tor = Uc.len4FromUserUnits(txtTor.Text),
            Acyy = Uc.areaFromUserUnits(txtAcyy.Text),
            Aczz = Uc.areaFromUserUnits(txtAczz.Text),
            Syy = Uc.len3FromUserUnits(txtSyy.Text),
            Szz = Uc.len3FromUserUnits(txtSzz.Text),
            Pyy = Uc.len4FromUserUnits(txtPyy.Text),
            Pzz = Uc.len4FromUserUnits(txtPzz.Text),
            ryy = Uc.lenFromUserUnits(txtRyy.Text),
            rzz = Uc.lenFromUserUnits(txtRzz.Text)
        };
        setMatId(s, _materialId);
        return s;
    }

    private static Section sectClone(Section s)
    {
        var c = new Section
        {
            ID = s.ID,
            Name = s.Name,
            Type = s.Type,
            MaterialID = s.MaterialID,
            MaterialName = s.MaterialName,
            h = s.h,
            b = s.b,
            tw = s.tw,
            tf = s.tf,
            Ag = s.Ag,
            Iyy = s.Iyy,
            Izz = s.Izz,
            Iyz = s.Iyz,
            Tor = s.Tor,
            Acyy = s.Acyy,
            Aczz = s.Aczz,
            Syy = s.Syy,
            Szz = s.Szz,
            Pyy = s.Pyy,
            Pzz = s.Pzz,
            ryy = s.ryy,
            rzz = s.rzz
        };
        setMatId(c, matIdOf(s));
        return c;
    }

    private int ResolveId(int id)
    {
        if (App.model.sections.Any(s => s.ID == id))
            return App.model.sections.NextID();

        return id;
    }

    private void ApplyRules(Section s)
    {
        switch ((sectType)s.Type)
        {
            case sectType.Rect:
                RectProps(s);
                break;

            case sectType.Circ:
                CircProps(s);
                break;

            case sectType.HolRec:
                HolRectProps(s);
                break;

            case sectType.HolCirc:
                HolCircProps(s);
                break;

            case sectType.Angle:
                AngleProps(s);
                break;

            case sectType.IShape:
            case sectType.WShape:
                IShapeProps(s);
                break;

            case sectType.CProfile:
                CShapeProps(s);
                break;
        }

        // --- DERIVED ---
        if (s.ryy <= 0 && s.Iyy > 0 && s.Ag > 0)
            s.ryy = Math.Sqrt(s.Iyy / s.Ag);

        if (s.rzz <= 0 && s.Izz > 0 && s.Ag > 0)
            s.rzz = Math.Sqrt(s.Izz / s.Ag);

        if (s.Syy <= 0 && s.Iyy > 0 && s.h > 0)
            s.Syy = s.Iyy / (s.h / 2.0);

        if (s.Szz <= 0 && s.Izz > 0 && s.b > 0)
            s.Szz = s.Izz / (s.b / 2.0);

        if (s.Pyy <= 0 && s.Syy > 0)
            s.Pyy = 1.5 * s.Syy;

        if (s.Pzz <= 0 && s.Szz > 0)
            s.Pzz = 1.5 * s.Szz;

        if (double.IsNaN(s.Iyz))
            s.Iyz = 0;

        if (s.Ag < 0) s.Ag = 0;
    }

    #endregion

    #region HELPERS

    private void RectProps(Section s)
    {
        if (s.b <= 0 || s.h <= 0) return;

        if (s.Ag <= 0)
            s.Ag = s.b * s.h;

        if (s.Iyy <= 0)
            s.Iyy = s.b * Math.Pow(s.h, 3) / 12.0;

        if (s.Izz <= 0)
            s.Izz = s.h * Math.Pow(s.b, 3) / 12.0;

        if (s.Tor <= 0)
            s.Tor = s.b * Math.Pow(s.h, 3) / 3.0;

        if (s.Acyy <= 0) s.Acyy = 5.0 / 6.0 * s.Ag;
        if (s.Aczz <= 0) s.Aczz = 5.0 / 6.0 * s.Ag;

        s.Iyz = 0;
    }

    private void CircProps(Section s)
    {
        if (s.b <= 0) return;

        double R = s.b / 2.0;

        if (s.Ag <= 0)
            s.Ag = Math.PI * R * R;

        if (s.Iyy <= 0)
            s.Iyy = Math.PI * Math.Pow(s.b, 4) / 64.0;

        if (s.Izz <= 0)
            s.Izz = s.Iyy;

        if (s.Tor <= 0)
            s.Tor = Math.PI * Math.Pow(s.b, 4) / 32.0;

        if (s.Acyy <= 0) s.Acyy = 0.9 * s.Ag;
        if (s.Aczz <= 0) s.Aczz = 0.9 * s.Ag;

        s.Iyz = 0;
    }

    private void HolRectProps(Section s)
    {
        if (s.b <= 0 || s.h <= 0 || s.tw <= 0 || s.tf <= 0) return;

        double bi = s.b - 2 * s.tw;
        double hi = s.h - 2 * s.tf;
        if (bi <= 0 || hi <= 0) return;

        if (s.Ag <= 0)
            s.Ag = s.b * s.h - bi * hi;

        if (s.Iyy <= 0)
            s.Iyy = (s.b * Math.Pow(s.h, 3) - bi * Math.Pow(hi, 3)) / 12.0;

        if (s.Izz <= 0)
            s.Izz = (s.h * Math.Pow(s.b, 3) - hi * Math.Pow(bi, 3)) / 12.0;

        if (s.Tor <= 0)
            s.Tor = 2 * s.tw * s.tf * Math.Pow((s.b - s.tw), 2);

        if (s.Acyy <= 0) s.Acyy = 2 * s.tw * s.h;
        if (s.Aczz <= 0) s.Aczz = 2 * s.tf * s.b;

        s.Iyz = 0;
    }

    private void HolCircProps(Section s)
    {
        if (s.b <= 0 || s.tw <= 0) return;

        double D = s.b;
        double d = D - 2 * s.tw;
        if (d <= 0) return;

        if (s.Ag <= 0)
            s.Ag = Math.PI * (Math.Pow(D / 2, 2) - Math.Pow(d / 2, 2));

        if (s.Iyy <= 0)
            s.Iyy = Math.PI * (Math.Pow(D, 4) - Math.Pow(d, 4)) / 64.0;

        if (s.Izz <= 0)
            s.Izz = s.Iyy;

        if (s.Tor <= 0)
            s.Tor = Math.PI * (Math.Pow(D, 4) - Math.Pow(d, 4)) / 32.0;

        if (s.Acyy <= 0) s.Acyy = 0.9 * s.Ag;
        if (s.Aczz <= 0) s.Aczz = 0.9 * s.Ag;

        s.Iyz = 0;
    }

    private void AngleProps(Section s)
    {
        if (s.b <= 0 || s.h <= 0 || s.tw <= 0) return;

        if (s.Ag <= 0)
            s.Ag = (s.b + s.h - s.tw) * s.tw;

        if (s.Iyy <= 0)
            s.Iyy = s.tw * Math.Pow(s.h, 3) / 3.0;

        if (s.Izz <= 0)
            s.Izz = s.tw * Math.Pow(s.b, 3) / 3.0;

        s.Iyz = -s.Ag * (s.b / 4.0) * (s.h / 4.0);
        if (s.Tor <= 0 && s.b > 0 && s.h > 0 && s.tw > 0)
            s.Tor = (Math.Pow(s.tw, 3) / 3.0) * (s.b + s.h - s.tw);

        if (s.Acyy <= 0) s.Acyy = s.tw * s.h;
        if (s.Aczz <= 0) s.Aczz = s.tw * s.b;
    }

    private void IShapeProps(Section s)
    {
        if (s.b <= 0 || s.h <= 0 || s.tw <= 0 || s.tf <= 0) return;

        double hw = s.h - 2 * s.tf;

        if (s.Ag <= 0)
            s.Ag = 2 * s.b * s.tf + hw * s.tw;

        if (s.Iyy <= 0)
            s.Iyy = (s.b * Math.Pow(s.h, 3) - (s.b - s.tw) * Math.Pow(hw, 3)) / 12.0;

        if (s.Izz <= 0)
            s.Izz = 2 * (s.tf * Math.Pow(s.b, 3) / 12.0) + hw * Math.Pow(s.tw, 3) / 12.0;

        if (s.Tor <= 0)
            s.Tor = 2 * s.b * Math.Pow(s.tf, 3) / 3.0 + hw * Math.Pow(s.tw, 3) / 3.0;

        if (s.Acyy <= 0) s.Acyy = s.tw * hw;
        if (s.Aczz <= 0) s.Aczz = 2 * s.b * s.tf;

        s.Iyz = 0;
    }

    private void CShapeProps(Section s)
    {
        if (s.b <= 0 || s.h <= 0 || s.tw <= 0 || s.tf <= 0) return;

        double hw = s.h - 2 * s.tf;

        if (s.Ag <= 0)
            s.Ag = 2 * s.b * s.tf + hw * s.tw;

        if (s.Iyy <= 0)
            s.Iyy = (s.b * Math.Pow(s.h, 3) - (s.b - s.tw) * Math.Pow(hw, 3)) / 12.0;

        if (s.Izz <= 0)
            s.Izz = 2 * (s.tf * Math.Pow(s.b, 3) / 12.0) + hw * Math.Pow(s.tw, 3) / 12.0;

        if (s.Acyy <= 0) s.Acyy = s.tw * hw;
        if (s.Aczz <= 0) s.Aczz = 2 * s.b * s.tf;

        s.Iyz = -0.1 * s.Ag * s.b * s.h; // approximation
    }

    #endregion
}