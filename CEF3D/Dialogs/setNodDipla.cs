using System;
namespace CEF;
public  class setNodDipla : Form
{
    #region UI


    internal Button ok, No, btnDel, btnReplace;
    internal GroupBox GroupBox1;
    internal TextBox dx, dy, dz, rx, ry, rz;
    internal Label Label9, Label6, Label3, Label4, Label5, Label2, Label1;
    internal ComboBox Caso;
    private void InitializeComponent()
    {
        rz = new TextBox();
        ry = new TextBox();
        rx = new TextBox();
        No = new Button();
        ok = new Button();
        btnDel = new Button();
        btnReplace = new Button();
        GroupBox1 = new GroupBox();
        Label3 = new Label();
        Label4 = new Label();
        Label5 = new Label();
        dz = new TextBox();
        Label2 = new Label();
        dy = new TextBox();
        Label1 = new Label();
        dx = new TextBox();
        Label9 = new Label();
        Caso = new ComboBox();
        Label6 = new Label();
        GroupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // gz
        // 
        rz.Location = new Point(195, 148);
        rz.Margin = new Padding(4, 3, 4, 3);
        rz.Name = "rz";
        rz.Size = new Size(90, 23);
        rz.TabIndex = 33;
        // 
        // gy
        // 
        ry.Location = new Point(195, 118);
        ry.Margin = new Padding(4, 3, 4, 3);
        ry.Name = "ry";
        ry.Size = new Size(90, 23);
        ry.TabIndex = 31;
        // 
        // gx
        // 
        rx.Location = new Point(195, 90);
        rx.Margin = new Padding(4, 3, 4, 3);
        rx.Name = "rx";
        rx.Size = new Size(90, 23);
        rx.TabIndex = 29;
        // 
        // No
        // 
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
        btnDel.Click += BtnDel_Click; ;

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
        GroupBox1.Controls.Add(rz);
        GroupBox1.Controls.Add(Label3);
        GroupBox1.Controls.Add(ry);
        GroupBox1.Controls.Add(Label4);
        GroupBox1.Controls.Add(rx);
        GroupBox1.Controls.Add(Label5);
        GroupBox1.Controls.Add(dz);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(dy);
        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(dx);
        GroupBox1.Controls.Add(Label9);
        GroupBox1.Controls.Add(Caso);
        GroupBox1.Controls.Add(Label6);
        GroupBox1.Location = new Point(9, 22);
        GroupBox1.Margin = new Padding(4, 3, 4, 3);
        GroupBox1.Padding = new Padding(4, 3, 4, 3);
        GroupBox1.Size = new Size(322, 233);
        GroupBox1.TabIndex = 8;
        GroupBox1.TabStop = false;
        GroupBox1.Text =I18n.Loads;
        // 
        // Label3
        // 
        Label3.AutoSize = true;
        Label3.Location = new Point(167, 151);
        Label3.Margin = new Padding(4, 0, 4, 0);
        Label3.Size = new Size(19, 15);
        Label3.TabIndex = 32;
        Label3.Text = "rz";
        // 
        // Label4
        // 
        Label4.AutoSize = true;
        Label4.Location = new Point(167, 121);
        Label4.Margin = new Padding(4, 0, 4, 0);
        Label4.Size = new Size(20, 15);
        Label4.TabIndex = 30;
        Label4.Text = "ry";
        // 
        // Label5
        // 
        Label5.AutoSize = true;
        Label5.Location = new Point(167, 93);
        Label5.Margin = new Padding(4, 0, 4, 0);
        Label5.Size = new Size(19, 15);
        Label5.TabIndex = 28;
        Label5.Text = "rx";
        // 
        // w
        // 
        dz.Location = new Point(48, 148);
        dz.Margin = new Padding(4, 3, 4, 3);
        dz.Size = new Size(90, 23);
        dz.TabIndex = 27;
        // 
        // Label2
        // 
        Label2.AutoSize = true;
        Label2.Location = new Point(20, 151);
        Label2.Margin = new Padding(4, 0, 4, 0);
        Label2.Size = new Size(16, 15);
        Label2.TabIndex = 26;
        Label2.Text = "dz";
        // 
        // v
        // 
        dy.Location = new Point(48, 118);
        dy.Margin = new Padding(4, 3, 4, 3);
        dy.Size = new Size(90, 23);
        dy.TabIndex = 25;
        // 
        // Label1
        // 
        Label1.AutoSize = true;
        Label1.Location = new Point(20, 121);
        Label1.Margin = new Padding(4, 0, 4, 0);
        Label1.Size = new Size(13, 15);
        Label1.TabIndex = 24;
        Label1.Text = "dy";
        // 
        // u
        // 
        dx.Location = new Point(48, 90);
        dx.Margin = new Padding(4, 3, 4, 3);
        dx.Size = new Size(90, 23);
        dx.TabIndex = 23;
        // 
        // Label9
        // 
        Label9.AutoSize = true;
        Label9.Location = new Point(44, 25);
        Label9.Margin = new Padding(4, 0, 4, 0);
        Label9.Size = new Size(56, 15);
        Label9.TabIndex = 20;
        Label9.Text = I18n.Case;
        // 
        // Caso
        // 
        Caso.FormattingEnabled = true;
        Caso.Location = new Point(48, 44);
        Caso.Margin = new Padding(4, 3, 4, 3);
        Caso.Size = new Size(102, 23);
        Caso.TabIndex = 19;
        // 
        // Label6
        // 
        Label6.AutoSize = true;
        Label6.Location = new Point(20, 93);
        Label6.Margin = new Padding(4, 0, 4, 0);
        Label6.Size = new Size(14, 15);
        Label6.TabIndex = 11;
        Label6.Text = "u";
        // 
        // AsDeplazamientos
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AcceptButton = ok;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = No;
        ClientSize = new Size(415, 300);
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(btnReplace);
        Controls.Add(btnDel);
        Controls.Add(GroupBox1);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Text = I18n.Displacements;
        GroupBox1.ResumeLayout(false);
        GroupBox1.PerformLayout();
        Load += FormLoad;

        ResumeLayout(false);
    }
   
    #endregion


    public setNodDipla()
    {
        InitializeComponent();

        ok.Enabled = false;
        btnDel.Enabled = false;
        btnReplace.Enabled = false;
    }

    private void FormLoad(object sender, EventArgs e)
    {
        int n = App.model.loadCases.Count;
        for (int i = 0; i < n; i++)
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

            var loads = App.model.nodalDisplacements.
                Where(l => l.ID == nodID && l.Case == Case).Select(l => l.ID).ToList();
            for (int j = 0; j < loads.Count; j++)
            {
                var lindx = App.model.nodalDisplacements.FindIndexByID(loads[j]);
                App.model.nodalDisplacements.RemoveAt(lindx);

            }

        }

    }

    private void setLoad()
    {
        //TODO: Load cases... etc
        int i;
        int index;
        int N = Selection.nodesMaxIndx + 1;
        var Sel = Selection.getNodesColection();
        for (i = 0; i < N; i++)
        {
            index = App.model.nodes[Sel[i]].ID;
            App.model.nodalDisplacements[index].ID = Sel[i];
            App.model.nodalDisplacements[index].dx = uc.lenFromUserUnits(dx.Text);
            App.model.nodalDisplacements[index].dy = uc.lenFromUserUnits(dy.Text);
            App.model.nodalDisplacements[index].dz = uc.lenFromUserUnits(dz.Text);
            App.model.nodalDisplacements[index].rx = uc.lenFromUserUnits(rx.Text);
            App.model.nodalDisplacements[index].ry = uc.lenFromUserUnits(ry.Text);
            App.model.nodalDisplacements[index].rz = uc.lenFromUserUnits(rz.Text);
        }
    }
    }