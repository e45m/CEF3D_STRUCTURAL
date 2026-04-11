using System;
namespace CEF;
public  class setContournRestr:Form
{
    #region UI
    internal Button No;
    internal Button ok;
    internal GroupBox GroupBox1;
    internal GroupBox GroupBox3;
    internal GroupBox GroupBox2;
    internal CheckBox GiroY;
    internal CheckBox GiroZ;
    internal CheckBox GiroX;
    internal Label Label1;
    internal CheckBox DespY;
    internal CheckBox DespZ;
    internal CheckBox DespX;
    internal Button BttEmp;
    internal Button BttLib;
    internal Button BttBis;
    internal Button BttPatin;
    private void InitializeComponent()
    {
        No = new Button();
        ok = new Button();
        GroupBox1 = new GroupBox();
        Label1 = new Label();
        GroupBox3 = new GroupBox();
        DespY = new CheckBox();
        DespZ = new CheckBox();
        DespX = new CheckBox();
        GroupBox2 = new GroupBox();
        GiroY = new CheckBox();
        GiroZ = new CheckBox();
        GiroX = new CheckBox();
        BttEmp = new Button();
        BttLib = new Button();
        BttBis = new Button();
        BttPatin = new Button();
        GroupBox1.SuspendLayout();
        GroupBox3.SuspendLayout();
        GroupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // No
        // 
        No.DialogResult = DialogResult.Cancel;
        No.Location = new Point(290, 197);
        No.Margin = new Padding(4, 3, 4, 3);
        No.Size = new Size(88, 31);
        No.TabIndex = 10;
        No.Text = I18n.DlgCancelText;
        No.UseVisualStyleBackColor = true;
        No.Click += No_Click;
        // 
        // ok
        // 
        ok.Location = new Point(198, 197);
        ok.Margin = new Padding(4, 3, 4, 3);
        ok.Size = new Size(88, 31);
        ok.TabIndex = 9;
        ok.Text = I18n.DlgOKText;
        ok.UseVisualStyleBackColor = true;
        ok.Click += ok_Click;
        // 
        // GroupBox1
        // 
        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(GroupBox3);
        GroupBox1.Controls.Add(GroupBox2);
        GroupBox1.Location = new Point(113, 14);
        GroupBox1.Margin = new Padding(4, 3, 4, 3);
        GroupBox1.Padding = new Padding(4, 3, 4, 3);
        GroupBox1.Size = new Size(267, 177);
        GroupBox1.TabIndex = 8;
        GroupBox1.TabStop = false;
        GroupBox1.Text = I18n.nodalRestraints;
        // 
        // Label1
        // 
        Label1.AutoSize = true;
        Label1.BorderStyle = BorderStyle.FixedSingle;
        Label1.Location = new Point(111, 12);
        Label1.Margin = new Padding(4, 0, 4, 0);
        Label1.Size = new Size(59, 17);
        Label1.TabIndex = 2;
        Label1.Text = I18n.Restrain;
        // 
        // GroupBox3
        // 
        GroupBox3.Controls.Add(DespY);
        GroupBox3.Controls.Add(DespZ);
        GroupBox3.Controls.Add(DespX);
        GroupBox3.Location = new Point(21, 32);
        GroupBox3.Margin = new Padding(4, 3, 4, 3);
        GroupBox3.Padding = new Padding(4, 3, 4, 3);
        GroupBox3.Size = new Size(120, 123);
        GroupBox3.TabIndex = 1;
        GroupBox3.TabStop = false;
        GroupBox3.Text = I18n.Displacements;
        // 
        // DespY
        // 
        DespY.AutoSize = true;
        DespY.Checked = true;
        DespY.CheckState = CheckState.Checked;
        DespY.Location = new Point(37, 51);
        DespY.Margin = new Padding(4, 3, 4, 3);
        DespY.Size = new Size(33, 19);
        DespY.TabIndex = 5;
        DespY.Text = "Y";
        DespY.UseVisualStyleBackColor = true;
        // 
        // DespZ
        // 
        DespZ.AutoSize = true;
        DespZ.Checked = true;
        DespZ.CheckState = CheckState.Checked;
        DespZ.Location = new Point(37, 77);
        DespZ.Margin = new Padding(4, 3, 4, 3);
        DespZ.Size = new Size(33, 19);
        DespZ.TabIndex = 4;
        DespZ.Text = "Z";
        DespZ.UseVisualStyleBackColor = true;
        // 
        // DespX
        // 
        DespX.AutoSize = true;
        DespX.Checked = true;
        DespX.CheckState = CheckState.Checked;
        DespX.Location = new Point(37, 24);
        DespX.Margin = new Padding(4, 3, 4, 3);
        DespX.Size = new Size(36, 19);
        DespX.TabIndex = 3;
        DespX.Text = "X";
        DespX.UseVisualStyleBackColor = true;
        // 
        // GroupBox2
        // 
        GroupBox2.Controls.Add(GiroY);
        GroupBox2.Controls.Add(GiroZ);
        GroupBox2.Controls.Add(GiroX);
        GroupBox2.Location = new Point(152, 32);
        GroupBox2.Margin = new Padding(4, 3, 4, 3);
        GroupBox2.Padding = new Padding(4, 3, 4, 3);
        GroupBox2.Size = new Size(102, 123);
        GroupBox2.TabIndex = 0;
        GroupBox2.TabStop = false;
        GroupBox2.Text = I18n.Rotation;
        // 
        // GiroY
        // 
        GiroY.AutoSize = true;
        GiroY.Checked = true;
        GiroY.CheckState = CheckState.Checked;
        GiroY.Location = new Point(26, 51);
        GiroY.Margin = new Padding(4, 3, 4, 3);
        GiroY.Size = new Size(33, 19);
        GiroY.TabIndex = 2;
        GiroY.Text = "Y";
        GiroY.UseVisualStyleBackColor = true;
        // 
        // GiroZ
        // 
        GiroZ.AutoSize = true;
        GiroZ.Checked = true;
        GiroZ.CheckState = CheckState.Checked;
        GiroZ.Location = new Point(26, 77);
        GiroZ.Margin = new Padding(4, 3, 4, 3);
        GiroZ.Size = new Size(33, 19);
        GiroZ.TabIndex = 1;
        GiroZ.Text = "Z";
        GiroZ.UseVisualStyleBackColor = true;
        // 
        // GiroX
        // 
        GiroX.AutoSize = true;
        GiroX.Checked = true;
        GiroX.CheckState = CheckState.Checked;
        GiroX.Location = new Point(26, 24);
        GiroX.Margin = new Padding(4, 3, 4, 3);
        GiroX.Size = new Size(36, 19);
        GiroX.TabIndex = 0;
        GiroX.Text = "X";
        GiroX.UseVisualStyleBackColor = true;
        // 
        // BttEmp
        // 
        BttEmp.Location = new Point(14, 45);
        BttEmp.Margin = new Padding(4, 3, 4, 3);
        BttEmp.Size = new Size(78, 31);
        BttEmp.TabIndex = 11;
        BttEmp.Text = I18n.Fixed;
        BttEmp.UseVisualStyleBackColor = true;
        BttEmp.Click += BttEmp_Click;
        // 
        // BttLib
        // 
        BttLib.Location = new Point(14, 83);
        BttLib.Margin = new Padding(4, 3, 4, 3);
        BttLib.Size = new Size(78, 31);
        BttLib.TabIndex = 12;
        BttLib.Text = I18n.Free;
        BttLib.UseVisualStyleBackColor = true;
        BttLib.Click += BttLib_Click;
        // 
        // BttBis
        // 
        BttBis.Location = new Point(14, 121);
        BttBis.Margin = new Padding(4, 3, 4, 3);
        BttBis.Size = new Size(78, 31);
        BttBis.TabIndex = 13;
        BttBis.Text = I18n.Hinge;
        BttBis.UseVisualStyleBackColor = true;
        BttBis.Click += BttBis_Click;
        // 
        // BttPatin
        // 
        BttPatin.Location = new Point(14, 159);
        BttPatin.Margin = new Padding(4, 3, 4, 3);
        BttPatin.Size = new Size(78, 31);
        BttPatin.TabIndex = 14;
        BttPatin.Text = I18n.Roller;
        BttPatin.UseVisualStyleBackColor = true;
        BttPatin.Click += BttPatin_Click;
        // 
        // AsRestrContorno
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AcceptButton = ok;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = No;
        ClientSize = new Size(394, 240);
        Controls.Add(BttPatin);
        Controls.Add(BttBis);
        Controls.Add(BttLib);
        Controls.Add(BttEmp);
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(GroupBox1);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = I18n.BoundaryConditions;
        GroupBox1.ResumeLayout(false);
        GroupBox1.PerformLayout();
        GroupBox3.ResumeLayout(false);
        GroupBox3.PerformLayout();
        GroupBox2.ResumeLayout(false);
        GroupBox2.PerformLayout();
        ResumeLayout(false);
    }
   
    #endregion


    public setContournRestr()
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

        int i;
        int Pos;
        int Nnudos = Selection.nodesMaxIndx;
        var Sel = Selection.getNodesColection();
        
        for (i = 0; i <= Nnudos; i++)
        {
            int id = Selection.getIdFromIndex((Sel[i]));
            Pos = App.model.nodalRestraints.FindIndexByID(id);
            if (Pos != -1)
            {
                App.model.nodalRestraints[Pos].ux = DespX.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].uy = DespY.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].uz = DespZ.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].rx = GiroX.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].ry = GiroY.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].rz = GiroZ.Checked ? 1 : 0;
            }
        }
        Close();
    }
    private void BttEmp_Click(object sender, EventArgs e)
    {
        DespX.Checked = true;
        DespY.Checked = true;
        DespZ.Checked = true;
        GiroX.Checked = true;
        GiroY.Checked = true;
        GiroZ.Checked = true;
    }
    private void BttLib_Click(object sender, EventArgs e)
    {
        DespX.Checked = false;
        DespY.Checked = false;
        DespZ.Checked = false;
        GiroX.Checked = false;
        GiroY.Checked = false;
        GiroZ.Checked = false;
    }
    private void BttBis_Click(object sender, EventArgs e)
    {
        DespX.Checked = true;
        DespY.Checked = true;
        DespZ.Checked = true;
        GiroX.Checked = false;
        GiroY.Checked = false;
        GiroZ.Checked = false;
    }
    private void BttPatin_Click(object sender, EventArgs e)
    {
        DespX.Checked = false;
        DespY.Checked = false;
        DespZ.Checked = true;
        GiroX.Checked = false;
        GiroY.Checked = false;
        GiroZ.Checked = false;
    }
}