using System;
using System.Windows.Forms;
using static System.Math;
using static CEF.ModelDB;
namespace CEF;
public  class setInsertBarPoint : Form
{
    #region UI
    internal GroupBox GroupBox1;
    internal Button ok;
    internal Button No;
    private ComboBox cBOffsetY;
    private ComboBox cBOffsetZ;
    private Label label1;
    private TextBox tBAng;
    private void InitializeComponent()
    { 

        GroupBox1 = dlgStyle.group(I18n.InsertionPoints,14,14,364,162);
        cBOffsetY = dlgStyle.combo(GroupBox1,"YY",47,115,40,263,[]);
        cBOffsetZ = dlgStyle.combo(GroupBox1,"ZZ",47,48,40,263,[]);
        cBOffsetY.Enabled =  cBOffsetZ.Enabled = true;

        ok = dlgStyle.button(I18n.DlgOKText,140,240,80,true);
        ok.Click += ok_Click;
        No = dlgStyle.button(I18n.DlgCancelText,234,240,80);
        No.Click += No_Click;
        AcceptButton = ok;
        CancelButton = No;

        tBAng = dlgStyle.editableField(this,I18n.RotationAngle,23,193,105,200 );
        dlgStyle.applyForm(this);

        ClientSize = new Size(411, 300);
        Controls.Add(tBAng);
        Controls.Add(label1);
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(GroupBox1);       
        Text = I18n.InsertionPoints;
        Load += FormLoad;

    }

    #endregion

    public setInsertBarPoint()
    {
        InitializeComponent();
    }
    private void No_Click(object sender, EventArgs e)
    {
        Close();   
    }
    private void ok_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        int N = Selection.barMaxIndex;
        int indB;
        var Sel = Selection.getBarsColection();
        var giro = 0d;
        var flagGiro = tBAng.Text != "";
        if(flagGiro)
        if(!double.TryParse(tBAng.Text, out giro)) giro = 0d;
        for (int i = 0; i <= N; i++)
        {
            indB = Sel[i];
            Bar barra = App.model.bars[indB];
            barra.offsetY = cBOffsetY.SelectedIndex;
            barra.offsetZ = cBOffsetZ.SelectedIndex;
            if (flagGiro)
                barra.Rotation = giro;
        }
        if (this.ParentForm is MainWindow cedm)
            cedm.reloadAllCanvasWindows();
        Close();
    }
    private void FormLoad(object sender, EventArgs e)
    {
        cBOffsetZ.Items.Add(I18n.ip_center);
        cBOffsetZ.Items.Add(I18n.ip_up);
        cBOffsetZ.Items.Add(I18n.ip_down);
        cBOffsetZ.SelectedIndex = 0;
        cBOffsetY.Items.Add(I18n.ip_center);
        cBOffsetY.Items.Add(I18n.ip_left);
        cBOffsetY.Items.Add(I18n.ip_right);
        cBOffsetY.SelectedIndex = 0;
    }
}