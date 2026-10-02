using System;
using System.Linq;
using System.Windows.Forms;

namespace CEF;

public class addMaterialToModel : Form
{
 

    private Button btLoad;
    private Button btImport;
    private Button btCreate;
    private Button btPrev;
    private Button btNext;

    private TextBox txtNombre, txtE, txtG, txtV, txtW, txtM, txtFc, txtFy, txtFu;
    private ComboBox cbTipo;
    private Label lblIndex;

    private void InitializeComponent()
    {
        // --- Top actions
        btLoad = new Button { Text = I18n.LoadDB, Left = 10, Top = 10, Width = 100, Enabled = false };
        btImport = new Button { Text = I18n.Import, Left = 120, Top = 10, Width = 100, Visible = true };
        btCreate = new Button { Text = I18n.EditCreate, Left = 230, Top = 10, Width = 100 };

        // --- Navigation
        btPrev = new Button { Text = I18n.leftArrow, Left = 10, Top = 50, Width = 40, Visible = true };
        btNext = new Button { Text = I18n.rightArrow, Left = 60, Top = 50, Width = 40, Visible = true };
        lblIndex = new Label { Left = 110, Top = 55, Width = 200 };


        // --- Material form
        int y = 90;

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

        // --- Events
        btLoad.Click += OnLoadClick;
        btImport.Click += OnImportClick;
        btCreate.Click += OnCreateClick;
        btPrev.Click += (_, _) => Navigate(-1);
        btNext.Click += (_, _) => Navigate(1);

        Controls.AddRange(new Control[] {
            btLoad, btImport, btCreate,
            btPrev, btNext, lblIndex
        });

        Text = I18n.Materials;
        Width = 360;
        Height = y + 60;
    }

    private TextBox AddField(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Left = 10, Top = y + 3, Width = 100 });
        var tb = new TextBox { Left = 120, Top = y, Width = 200, Enabled = false };
        Controls.Add(tb);
        y += 28;
        return tb;
    }

    private ComboBox AddCombo(string label, ref int y, string[] items)
    {
        Controls.Add(new Label { Text = label, Left = 10, Top = y + 3, Width = 100 });
        var cb = new ComboBox { Left = 120, Top = y, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
        cb.Items.AddRange(items);
        Controls.Add(cb);
        y += 28;
        return cb;
    }



    private int _currentIndex = 0;

    public addMaterialToModel()
    {
        InitializeComponent();
        loadDataBase("dbApp\\Materials.cefdb");
        Navigate(0);
    }



    private void loadDataBase(string path)
    {
        App.appData.LoadModelFileReflection(path, false);

        if (!App.appData.libMaterials.Any())
        {
            MessageBox.Show(I18n.CannotLoadDB);
            return;
        }
    }


    private void OnLoadClick(object sender, EventArgs e)
    {
        //TODO: Open files....
        var file = "";

        loadDataBase(file);

        _currentIndex = 0;
        ShowMaterial(_currentIndex);

        btImport.Visible = true;
        btPrev.Visible = true;
        btNext.Visible = true;
    }

    private void Navigate(int step)
    {
        int newIndex = _currentIndex + step;

        if (newIndex < 0 || newIndex >= App.appData.libMaterials.Count)
            return;

        _currentIndex = newIndex;
        ShowMaterial(_currentIndex);
    }

    private void ShowMaterial(int index)
    {
        var m = App.appData.libMaterials[index];

        txtNombre.Text = m.Name;
        cbTipo.SelectedIndex = m.Type;

        txtE.Text = Uc.forcePerUnitAreaToUserUnits(m.E).ToString();
        txtG.Text = Uc.forcePerUnitAreaToUserUnits(m.G).ToString();
        txtV.Text = m.v.ToString();//adimentional
        txtW.Text =Uc.forceToUserUnits( m.W).ToString();
        txtM.Text = Uc.massToUserUnits(m.M).ToString();
        txtFc.Text = Uc.forcePerUnitAreaToUserUnits(m.fc).ToString();
        txtFy.Text = Uc.forcePerUnitAreaToUserUnits(m.fy).ToString();
        txtFu.Text = Uc.forcePerUnitAreaToUserUnits(m.fu).ToString();

        lblIndex.Text = $"{index + 1} / {App.appData.libMaterials.Count}";
    }



    private void OnImportClick(object sender, EventArgs e)
    {
        var src = App.appData.libMaterials[_currentIndex];
        var mat = materialClone(src);

        mat.ID = ResolveId(mat.ID);

        App.model.Materials.Add(mat);

    }

    private void OnCreateClick(object sender, EventArgs e)
    {
        setEnabledCtrls();



        var btnSave = new Button()
        {
            Top = btCreate.Top+25,
            Left = btCreate.Left,
            Size = btCreate.Size,
            Text = I18n.Save
        };

        var btnCancel = new Button()
        {
            Top = btCreate.Top+50,
            Left = btCreate.Left,
            Size = btCreate.Size,
            Text = I18n.DlgCancelText
        };
        btnCancel.Click += (_, _) => {
            setEnabledCtrls(false);
            Controls.Remove(btnSave);
            Controls.Remove(btnCancel);
        
        };

        btnSave.Click += (_, _) => {
            setEnabledCtrls(false);
            Controls.Remove(btnSave);
            Controls.Remove(btnCancel);
            var mat = ReadFromUI();
            mat.ID = ResolveId(mat.ID);
            ApplyRules(mat);
            App.model.Materials.Add(mat);
           
        };

        Controls.Add(btnSave);
        Controls.Add(btnCancel);

    }


    private void setEnabledCtrls(bool state = true)
    {

        txtNombre.Enabled = state;
        cbTipo.Enabled = state;
        txtE.Enabled = state;
        txtG.Enabled = state;
        txtV.Enabled = state;
        txtW.Enabled = state;
        txtM.Enabled = state;
        txtFc.Enabled = state;
        txtFy.Enabled = state;
        txtFu.Enabled = state;
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
            v = Parse(txtV),//v is adimentional
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
        if (App.model.Materials.Any(m => m.ID == id))
            return App.model.Materials.Max(m => m.ID) + 1;

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

    
}