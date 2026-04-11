using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CEF;

public class addSectionsToModel : Form
{

    private Button btLoad;
    private Button btImport;
    private Button btCreate;
    private Button btPrev;
    private Button btNext;
    private Button btCalc;

    private TextBox txtNombre, txtH, txtB, txtTw, txtTf;
    private TextBox txtAg, txtIyy, txtIzz, txtIyz, txtTor;
    private TextBox txtAcyy, txtAczz, txtSyy, txtSzz;
    private TextBox txtPyy, txtPzz, txtRyy, txtRzz;

    private ComboBox cbTipo;
    private Label lblIndex;    
    int [] secTypeVals ;


    private void InitializeComponent()
    {
        btLoad = new Button { Text = I18n.LoadDB, Left = 10, Top = 10, Width = 100, Enabled = false };
        btCalc = new Button { Text = I18n.CalProps, Left = 10, Top = 10, Width = 100 , Visible = true};
        btImport = new Button { Text = I18n.Import, Left = 120, Top = 10, Width = 100, Visible = true };
        btCreate = new Button { Text = I18n.EditCreateDelete, Left = 230, Top = 10, Width = 100 };

        btPrev = new Button { Text = I18n.leftArrow, Left = 10, Top = 50, Width = 40, Visible = true };
        btNext = new Button { Text = I18n.rightArrow, Left = 60, Top = 50, Width = 40, Visible = true };
        lblIndex = new Label { Left = 110, Top = 55, Width = 200 };

        btLoad.Click += OnLoadClick;
        btImport.Click += OnImportClick;
        btCreate.Click += OnCreateClick;
        btCalc.Click += OnCalcClick;
        btPrev.Click += (_, _) => Navigate(-1);
        btNext.Click += (_, _) => Navigate(1);



        int y = 90;

        txtNombre = AddField(I18n.Name, ref y);
        cbTipo = AddCombo(I18n.Type, ref y, Enum.GetNames(typeof(sectType)));
        cbTipo.Enabled = false;
        secTypeVals =  (Enum.GetValues<sectType>()).Select(v=>(int)v).ToArray();


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


        Text = I18n.Sections;
        Width = 360;
        Height = y + 60;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;

       
        Controls.AddRange(new Control[] { btLoad, btImport, btCreate,btCalc, btPrev, btNext, lblIndex });
    }

    private TextBox AddField(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Left = 10, Top = y + 3, Width = 100 , Name = label});
        var tb = new TextBox { Left = 120, Top = y, Width = 200, Enabled = false };
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
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cb.Items.AddRange(items);
        Controls.Add(cb);
        y += 26;
        return cb;
    }


    private int _currentIndex = 0;

    public addSectionsToModel()
    {
        InitializeComponent();
        loadDataBase("dbApp\\Sections.cefdb");
        Navigate(0);
    }

    private void loadDataBase(string path)
    {
        App.appData.LoadModelFileReflection(path, false);

        if (!App.appData.libSections.Any())
        {
            MessageBox.Show(I18n.CannotLoadDB);
            return;
        }
    }

    private void OnLoadClick(object sender, EventArgs e)
    {
        var file = "";
        App.appData.LoadModelFileReflection(file, false);

        if (!App.appData.libSections.Any())
        {
            MessageBox.Show(I18n.CannotLoadDB);
            return;
        }

        _currentIndex = 0;
        ShowSection(_currentIndex);

        btImport.Visible = true;
        btPrev.Visible = true;
        btNext.Visible = true;
        btLoad.Enabled = false;
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
        cbTipo.SelectedIndex = Array.FindIndex(secTypeVals,v=>v== s.Type);

        txtH.Text =uc.lenToUserUnits(s.h).ToString();
        txtB.Text = uc.lenToUserUnits(s.b).ToString();
        txtTw.Text = uc.lenToUserUnits(s.tw).ToString();
        txtTf.Text = uc.lenToUserUnits(s.tf).ToString();

        txtAg.Text =uc.areaToUserUnits(s.Ag).ToString();
        txtIyy.Text =uc.len4ToUserUnits(s.Iyy).ToString();
        txtIzz.Text = uc.len4ToUserUnits(s.Izz).ToString();
        txtIyz.Text = uc.len4ToUserUnits(s.Iyz).ToString();
        txtTor.Text = uc.len4ToUserUnits(s.Tor).ToString();

        txtAcyy.Text =uc.areaToUserUnits(s.Acyy).ToString();
        txtAczz.Text =uc.areaToUserUnits(s.Aczz).ToString();
        txtSyy.Text = uc.len3ToUserUnits(s.Syy).ToString();
        txtSzz.Text = uc.len3ToUserUnits(s.Szz).ToString();

        txtPyy.Text =uc.len4ToUserUnits(s.Pyy).ToString();
        txtPzz.Text =uc.len4ToUserUnits(s.Pzz).ToString();
        txtRyy.Text =uc.lenToUserUnits(s.ryy).ToString();
        txtRzz.Text =uc.lenToUserUnits(s.rzz).ToString();

        lblIndex.Text = $"{index + 1} / {App.appData.libSections.Count}";

    }


    private void OnImportClick(object sender, EventArgs e)
    {
        var src = App.appData.libSections[_currentIndex];
        var sec = sectClone(src);

        sec.ID = ResolveId(sec.ID);

        App.model.sections.Add(sec);

    }

    private void OnCreateClick(object sender, EventArgs e)
    {
        setEnabledCtrls(true);
        setNATb();

        var btnSave = new Button()
        {
            Top = btCreate.Top + 25,
            Left = btCreate.Left,
            Size = btCreate.Size,
            Text = I18n.Save
        };

        var btnCancel = new Button()
        {
            Top = btCreate.Top + 50,
            Left = btCreate.Left,
            Size = btCreate.Size,
            Text = I18n.DlgOKText
        };
        btnCancel.Click += (_, _) => {
            setEnabledCtrls(false);
            Controls.Remove(btnSave);
            Controls.Remove(btnCancel);
            ShowSection(_currentIndex);
        };

        btnSave.Click += (_, _) => {
            setEnabledCtrls(false);
            Controls.Remove(btnSave);
            Controls.Remove(btnCancel);
            var sec = ReadFromUI();
            sec.ID = ResolveId(sec.ID);
            ApplyRules(sec);
            App.model.sections.Add(sec);
            ShowSection(_currentIndex);


        };

        Controls.Add(btnSave);
        Controls.Add(btnCancel);

    }

    private void setEnabledCtrls(bool state = true)
    {

        txtNombre.Enabled = state;
        cbTipo.Enabled = state;
        btCalc.Visible = state;


        txtH.Enabled = state;
        txtB.Enabled = state;
        txtTw.Enabled = state;
        txtTf.Enabled = state;

        btImport.Visible = !state;
        btNext.Visible = !state;
        btPrev.Visible = !state;
        lblIndex.Visible = !state;
        btLoad.Visible = !state;

    }

    private void setNATb()
    {
        txtAg.Text =  "-";
        txtIyy.Text = "-";
        txtIzz.Text = "-";
        txtIyz.Text = "-";
        txtTor.Text = "-";

        txtAcyy.Text = "-";
        txtAczz.Text = "-";
        txtSyy.Text =  "-";
        txtSzz.Text =  "-";

        txtPyy.Text = "-";
        txtPzz.Text = "-";
        txtRyy.Text = "-";
        txtRzz.Text = "-";
    }

    private void OnCreateClick_(object sender, EventArgs e)
    {
        var sec = ReadFromUI();

        sec.ID = ResolveId(sec.ID);

        ApplyRules(sec);

        App.model.sections.Add(sec);

    }

    private void OnCalcClick(object sender, EventArgs e)
    {
        var s = ReadFromUI();

        s.ID = ResolveId(s.ID);

        ApplyRules(s);     

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


    private section ReadFromUI()
    {
        return new section
        {
            Name = txtNombre.Text,

            Type = secTypeVals [ cbTipo.SelectedIndex],

            h = uc.lenFromUserUnits(txtH.Text),
            b = uc.lenFromUserUnits(txtB.Text),
            tw =uc.lenFromUserUnits (txtTw.Text),
            tf =uc.lenFromUserUnits(txtTf.Text),
            Ag = uc.areaFromUserUnits(txtAg.Text),
            Iyy = uc.len4FromUserUnits(txtIyy.Text),
            Izz = uc.len4FromUserUnits(txtIzz.Text),
            Iyz = uc.len4FromUserUnits(txtIyz.Text),
            Tor = uc.len4FromUserUnits(txtTor.Text),
            Acyy = uc.areaFromUserUnits(txtAcyy.Text),
            Aczz = uc.areaFromUserUnits(txtAczz.Text),
            Syy = uc.len4FromUserUnits(txtSyy.Text),
            Szz = uc.len4FromUserUnits(txtSzz.Text),
            Pyy = uc.len4FromUserUnits(txtPyy.Text),
            Pzz = uc.len4FromUserUnits(txtPzz.Text),
            ryy = uc.lenFromUserUnits(txtRyy.Text),
            rzz = uc.lenFromUserUnits(txtRzz.Text)
        };
    }

    private static section sectClone(section s)
    {
        return new section
        {
            ID = s.ID,
            Name = s.Name,
            Type = s.Type,
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
    }

    private int ResolveId(int id)
    {
        if (App.model.sections.Any(s => s.ID == id))
            return App.model.sections.Max(s => s.ID) + 1;

        return id == 0 ? App.model.sections.Max(s => s.ID) + 1 : id;
    }

   

    private double Parse(TextBox tb)
    {
        return double.TryParse(tb.Text, out var v) ? v : 0;
    }

    private void ApplyRules(section s)
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

        // --- DERIVADAS ---
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

    #region HELPERS

    private void RectProps(section s)
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

    private void CircProps(section s)
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

    private void HolRectProps(section s)
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

    private void HolCircProps(section s)
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

    private void AngleProps(section s)
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

    private void IShapeProps(section s)
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

    private void CShapeProps(section s)
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

        s.Iyz = -0.1 * s.Ag * s.b * s.h; // aproximación
    }

    #endregion

}