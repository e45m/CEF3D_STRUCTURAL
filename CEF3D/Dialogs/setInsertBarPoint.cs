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
        GroupBox1 = new GroupBox();
        cBOffsetY = new ComboBox();
        cBOffsetZ = new ComboBox();
        ok = new Button();
        No = new Button();
        label1 = new Label();
        tBAng = new TextBox();
        GroupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // GroupBox1
        // 
        GroupBox1.Controls.Add(cBOffsetY);
        GroupBox1.Controls.Add(cBOffsetZ);
        GroupBox1.Location = new Point(14, 14);
        GroupBox1.Margin = new Padding(4, 3, 4, 3);
        GroupBox1.Padding = new Padding(4, 3, 4, 3);
        GroupBox1.Size = new Size(364, 162);
        GroupBox1.TabIndex = 2;
        GroupBox1.TabStop = false;
        GroupBox1.Text = I18n.InsertionPoints;
        // 
        // cBOffsetY
        // 
        cBOffsetY.FormattingEnabled = true;
        cBOffsetY.Location = new Point(47, 115);
        cBOffsetY.Size = new Size(263, 23);
        cBOffsetY.TabIndex = 1;
        // 
        // cBOffsetZ
        // 
        cBOffsetZ.FormattingEnabled = true;
        cBOffsetZ.Location = new Point(47, 48);
        cBOffsetZ.Size = new Size(263, 23);
        cBOffsetZ.TabIndex = 0;
        // 
        // ok
        // 
        ok.Location = new Point(140, 333);
        ok.Margin = new Padding(4, 3, 4, 3);
        ok.Size = new Size(88, 31);
        ok.TabIndex = 3;
        ok.Text = I18n.DlgOKText;
        ok.UseVisualStyleBackColor = true;
        ok.Click += ok_Click;
        // 
        // No
        // 
        No.DialogResult = DialogResult.Cancel;
        No.Location = new Point(234, 333);
        No.Margin = new Padding(4, 3, 4, 3);
        No.Size = new Size(88, 31);
        No.TabIndex = 4;
        No.Text = I18n.DlgCancelText;
        No.UseVisualStyleBackColor = true;
        No.Click += No_Click;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(23, 193);
        label1.Size = new Size(103, 15);
        label1.TabIndex = 5;
        label1.Text =I18n.RotationAngle;
        // 
        // tBAng
        // 
        tBAng.Location = new Point(61, 221);
        tBAng.Size = new Size(263, 23);
        tBAng.TabIndex = 6;
        // 
        // AsInsertPointAngBar
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AcceptButton = ok;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(411, 401);
        Controls.Add(tBAng);
        Controls.Add(label1);
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(GroupBox1);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = I18n.InsertionPoints;
        Load += FormLoad;
        GroupBox1.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
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
            bar barra = App.model.bars[indB];
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