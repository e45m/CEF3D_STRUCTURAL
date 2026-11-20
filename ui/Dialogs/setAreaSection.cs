using System;
using System.Linq;
using System.Windows.Forms;

namespace CEF;

public class setAreaSection : Form
{
    #region UI

    private ComboBox cbSections;
    private Button btAccept;
    private Button btCancel;

    private TextBox txName, txThick, txType;   

    private void InitializeComponent()
    {


        cbSections = dlgStyle.combo(this, "", 10,10,1,340,[]);
        cbSections.Enabled = true;

        int y = 45;

        txName = dlgStyle.field ( this,I18n.Name, 10, y+=26,80,80);
        txThick = dlgStyle.field(this, "t", 10, y+=26,80,80);
        txType = dlgStyle.field(this, I18n.Type, 10, y+=26,80,80);


        y += 52;


        btAccept = dlgStyle.button(I18n.DlgOKText,180,y,80,true);
        btCancel = dlgStyle.button( I18n.DlgCancelText,270,y,80);

        CancelButton = btCancel;
        AcceptButton = btAccept;

        cbSections.SelectedIndexChanged += OnSelectionChanged;
        btAccept.Click += OnAccept;
        btCancel.Click += (_, _) => Close();

        Text = I18n.AreaSections;
        Width = 380;
        Height = y + 90;
      

        Controls.Add(cbSections);
        Controls.Add(btAccept);
        Controls.Add(btCancel);

        dlgStyle.applyForm(this);

        
    }


    #endregion

    public setAreaSection()
    {
        InitializeComponent();
        LoadSections();
    }





    private void LoadSections()
    {
        cbSections.Items.Clear();

        foreach (var s in Engine.getModel().areaSections)
            cbSections.Items.Add($"{s.ID}: {s.Name}");



        if (cbSections.Items.Count > 0)
            cbSections.SelectedIndex = 0;
      

        ShowSection(Engine.getModel().areaSections[0]);
    }



    private void OnSelectionChanged(object sender, EventArgs e)
    {
        if (cbSections.SelectedIndex < 0) return;

        var s = Engine.getModel().areaSections[cbSections.SelectedIndex];

        ShowSection(s);
    }

    private void OnAccept(object sender, EventArgs e)
    {
        if (cbSections.SelectedIndex < 0)
        {
            return;
        }

        App.takeUndoSnapShot();

        var selected = Engine.getModel().areaSections[cbSections.SelectedIndex];

        
        var sel =  Selection.getAreasColection();

        App.areaSectionDefaultID = selected.ID;

        foreach (var i in sel)
        {
            var Area = Engine.getModel().areas[i];

            Area.SectionID = selected.ID;
            Area.SectionName = selected.Name;
        }

        Close();
    }



    private void ShowSection(areaSection s)
    {
       
        txName.Text = s.Name;
        txThick.Text = Uc.lenToUserUnits(s.Thickness).ToString();
        txType.Text =Enum.GetName(typeof(areaKind),s.Kind);     

    }
}