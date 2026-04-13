using System;
namespace CEF;
public class addNodLoad: Form
{
    #region UI

    internal Button ok, No, btnDel, btnReplace;
    internal GroupBox GroupBox1;
    internal TextBox Fz, Fy, Fx, a, Mz, My, Mx;
    internal Label Label9, Label6,  Label3,  Label4,  Label5,  Label2,  Label1;
    internal ComboBox Caso;

    private void InitializeComponent()
    {
        No = new Button();
        ok = new Button();
        GroupBox1 = new GroupBox();
        Mz = new TextBox();
        Label3 = new Label();
        My = new TextBox();
        Label4 = new Label();
        Mx = new TextBox();
        Label5 = new Label();
        Fz = new TextBox();
        Label2 = new Label();
        Fy = new TextBox();
        Label1 = new Label();
        Fx = new TextBox();
        Label9 = new Label();
        Caso = new ComboBox();
        Label6 = new Label();
        btnDel = new Button();
        btnReplace = new Button();
        GroupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // No
        ok.Location = new Point(10, 250);
        ok.Margin = new Padding(4, 3, 4, 3);
        ok.Size = new Size(90, 31);
        ok.TabIndex = 3;
        ok.Text = I18n.DlgOKText;
        ok.UseVisualStyleBackColor = true;
        ok.Click += ok_Click;
        // 
        // No
        // 
        No.DialogResult = DialogResult.Cancel;
        No.Location = new Point(105, 250);
        No.Margin = new Padding(4, 3, 4, 3);
        No.Size = new Size(90, 31);
        No.TabIndex = 4;
        No.Text = I18n.DlgCancelText;
        No.UseVisualStyleBackColor = true;
        No.Click += No_Click;

        btnDel.Location = new Point(200, 250);
        btnDel.Margin = new Padding(4, 3, 4, 3);
        btnDel.Size = new Size(90, 31);
        btnDel.TabIndex = 4;
        btnDel.Text = I18n.Delete;
        btnDel.UseVisualStyleBackColor = true;
        btnDel.Click += BtnDel_Click;

        btnReplace.Location = new Point(295, 250);
        btnReplace.Margin = new Padding(4, 3, 4, 3);
        btnReplace.Size = new Size(90, 31);
        btnReplace.TabIndex = 4;
        btnReplace.Text = I18n.Replace;
        btnReplace.UseVisualStyleBackColor = true;
        btnReplace.Click += BtnReplace_Click; 
        // 
        // GroupBox1
        // 
        GroupBox1.Controls.Add(Mz);
        GroupBox1.Controls.Add(Label3);
        GroupBox1.Controls.Add(My);
        GroupBox1.Controls.Add(Label4);
        GroupBox1.Controls.Add(Mx);
        GroupBox1.Controls.Add(Label5);
        GroupBox1.Controls.Add(Fz);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(Fy);
        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(Fx);
        GroupBox1.Controls.Add(Label9);
        GroupBox1.Controls.Add(Caso);
        GroupBox1.Controls.Add(Label6);
        GroupBox1.Location = new Point(14, 8);
        GroupBox1.Margin = new Padding(4, 3, 4, 3);
        GroupBox1.Padding = new Padding(4, 3, 4, 3);
        GroupBox1.Size = new Size(322, 233);
        GroupBox1.TabIndex = 5;
        GroupBox1.TabStop = false;
        GroupBox1.Text = I18n.Loads;
        // 
        // Mz
        // 
        Mz.Location = new Point(195, 148);
        Mz.Margin = new Padding(4, 3, 4, 3);
        Mz.Size = new Size(90, 23);
        Mz.TabIndex = 33;
        // 
        // Label3
        // 
        Label3.AutoSize = true;
        Label3.Location = new Point(167, 151);
        Label3.Margin = new Padding(4, 0, 4, 0);
        Label3.Size = new Size(23, 15);
        Label3.TabIndex = 32;
        Label3.Text = "Mz";
        // 
        // My
        // 
        My.Location = new Point(195, 118);
        My.Margin = new Padding(4, 3, 4, 3);
        My.Size = new Size(90, 23);
        My.TabIndex = 31;
        // 
        // Label4
        // 
        Label4.AutoSize = true;
        Label4.Location = new Point(167, 121);
        Label4.Margin = new Padding(4, 0, 4, 0);
        Label4.Size = new Size(24, 15);
        Label4.TabIndex = 30;
        Label4.Text = "My";
        // 
        // Mx
        // 
        Mx.Location = new Point(195, 90);
        Mx.Margin = new Padding(4, 3, 4, 3);
        Mx.Size = new Size(90, 23);
        Mx.TabIndex = 29;
        // 
        // Label5
        // 
        Label5.AutoSize = true;
        Label5.Location = new Point(167, 93);
        Label5.Margin = new Padding(4, 0, 4, 0);
        Label5.Size = new Size(23, 15);
        Label5.TabIndex = 28;
        Label5.Text = "Mx";
        // 
        // Fz
        // 
        Fz.Location = new Point(48, 148);
        Fz.Margin = new Padding(4, 3, 4, 3);
        Fz.Size = new Size(90, 23);
        Fz.TabIndex = 27;
        // 
        // Label2
        // 
        Label2.AutoSize = true;
        Label2.Location = new Point(20, 151);
        Label2.Margin = new Padding(4, 0, 4, 0);
        Label2.Size = new Size(18, 15);
        Label2.TabIndex = 26;
        Label2.Text = "Fz";
        // 
        // Fy
        // 
        Fy.Location = new Point(48, 118);
        Fy.Margin = new Padding(4, 3, 4, 3);
        Fy.Size = new Size(90, 23);
        Fy.TabIndex = 25;
        // 
        // Label1
        // 
        Label1.AutoSize = true;
        Label1.Location = new Point(20, 121);
        Label1.Margin = new Padding(4, 0, 4, 0);
        Label1.Size = new Size(19, 15);
        Label1.TabIndex = 24;
        Label1.Text = "Fy";
        // 
        // Fx
        // 
        Fx.Location = new Point(48, 90);
        Fx.Margin = new Padding(4, 3, 4, 3);
        Fx.Size = new Size(90, 23);
        Fx.TabIndex = 23;
        // 
        // Label9
        // 
        Label9.AutoSize = true;
        Label9.Location = new Point(44, 25);
        Label9.Margin = new Padding(4, 0, 4, 0);
        Label9.Size = new Size(33, 15);
        Label9.TabIndex = 20;
        Label9.Text = I18n.Case;
        // 
        // Caso
        // 
        Caso.FormattingEnabled = true;
        Caso.Location = new Point(48, 44);
        Caso.Margin = new Padding(4, 3, 4, 3);
        Caso.Size = new Size(237, 23);
        Caso.TabIndex = 19;
        // 
        // Label6
        // 
        Label6.AutoSize = true;
        Label6.Location = new Point(20, 93);
        Label6.Margin = new Padding(4, 0, 4, 0);
        Label6.Size = new Size(18, 15);
        Label6.TabIndex = 11;
        Label6.Text = "Fx";
        // 
        // modNodeLoads
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(415, 300);
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(btnDel);
        Controls.Add(btnReplace);
        Controls.Add(GroupBox1);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Text = I18n.SetNodalLoads;
        Load += FormLoad;
        GroupBox1.ResumeLayout(false);
        GroupBox1.PerformLayout();
        ResumeLayout(false);
    }
   


    #endregion


    public addNodLoad()
    {
        InitializeComponent();
    }
    private void FormLoad(object sender, EventArgs e)
    {
        int n = App.model.loadCases.Count;
        for (int i = 0; i <n; i++)
            Caso.Items.Add(App.model.loadCases[i].ID + ": " + App.model.loadCases[i].Name);
    }
    private void ok_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        setLoad();
            App.model.setParamValue(Mdta.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
            if (this.Owner is MainWindow cedm)
                cedm.reloadAllCanvasWindows();
        Close();  
    }
    private void No_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void BtnReplace_Click(object? sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteDLoad();
        setLoad();
        App.model.setParamValue(Mdta.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
        if (this.Owner is MainWindow cedm)
            cedm.reloadAllCanvasWindows();
        Close();

    }

    private void BtnDel_Click(object? sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteDLoad();
        App.model.setParamValue(Mdta.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
        if (this.Owner is MainWindow cedm)
            cedm.reloadAllCanvasWindows();
        Close();


    }
    private void deleteDLoad()
    {
        int N = Selection.nodesMaxIndx + 1;
        var Sel = Selection.getNodesColection();
        if (Sel == null) return;

        var Case = App.model.loadCases[Caso.SelectedIndex].ID;
        var idselNods = Sel.Select(i => App.model.nodes[i].ID).ToList();
        if (idselNods == null) return;

        for (int i = 0; i < N; i++)
        {
            var indNod = idselNods[i];
            node node = App.model.nodes[indNod];
            var nodID = node.ID;

            var loads = App.model.nodalLoads.
                Where(l => l.NodeID == nodID && l.Case == Case).Select(l => l.ID).ToList();
            for (int j = 0; j < loads.Count; j++)
            {
                var lindx = App.model.nodalLoads.FindIndexByID(loads[j]);
                App.model.nodalLoads.RemoveAt(lindx);

            }

        }

    }

    private void setLoad()
    {
        int i;
        int N = Selection.nodesMaxIndx + 1;
        var Sel = Selection.getNodesColection();

        var fx = uc.forceFromUserUnits(Fx.Text);
        var fy = uc.forceFromUserUnits(Fy.Text);
        var fz = uc.forceFromUserUnits(Fz.Text);
        var mx = uc.momentFromUserUnits(Mx.Text);
        var my = uc.momentFromUserUnits(My.Text);
        var mz = uc.momentFromUserUnits(Mz.Text);

        for (i = 0; i < N; i++)
        {
            var nl = new nodalLoad()
            {
                ID = App.model.nodalLoads.Max(l => l.ID) + 1,
                NodeID = App.model.nodes[Sel[i]].ID,
                Case = App.model.loadCases[Caso.SelectedIndex].ID,
                Fx = fx,
                Fy = fy,
                Fz = fz,
                Mx = mx,
                My = my,
                Mz = mz,
            };
            App.model.nodalLoads.Add(nl);
        }

    }

   

}