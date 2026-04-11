using System;
using static System.Math;
using static CEF.ModelDB;
namespace CEF;
public class addpBarLoad: Form
{
    #region UI
    internal Button ok, No, btnDel, btnReplace;
    internal ComboBox Caso;
    internal TextBox Fz,Fy,Fx,a,Mz,My,Mx;
    internal Label Label2, Label1, Label3, Label5, Label4, Label6, Label9, Label7;
    internal GroupBox GroupBox1, GroupBox3;
    internal RadioButton RDFrac, RDDist, RBGlobal, RBLocal;


    private void InitializeComponent()
    {
        GroupBox1 = new GroupBox();
        GroupBox3 = new GroupBox();
        RDFrac = new RadioButton();
        RDDist = new RadioButton();
        Mz = new TextBox();
        Label4 = new Label();
        My = new TextBox();
        Label6 = new Label();
        Mx = new TextBox();
        Label7 = new Label();
        a = new TextBox();
        Label5 = new Label();
        RBGlobal = new RadioButton();
        RBLocal = new RadioButton();
        Fz = new TextBox();
        Label2 = new Label();
        Fy = new TextBox();
        Label1 = new Label();
        Fx = new TextBox();
        Label3 = new Label();
        Label9 = new Label();
        Caso = new ComboBox();
        ok = new Button();
        No = new Button();
        btnDel = new Button();
        btnReplace = new Button();
        GroupBox1.SuspendLayout();
        GroupBox3.SuspendLayout();
        SuspendLayout();
        // 
        // GroupBox1
        // 
        GroupBox1.Controls.Add(GroupBox3);
        GroupBox1.Controls.Add(Mz);
        GroupBox1.Controls.Add(Label4);
        GroupBox1.Controls.Add(My);
        GroupBox1.Controls.Add(Label6);
        GroupBox1.Controls.Add(Mx);
        GroupBox1.Controls.Add(Label7);
        GroupBox1.Controls.Add(a);
        GroupBox1.Controls.Add(Label5);
        GroupBox1.Controls.Add(RBGlobal);
        GroupBox1.Controls.Add(RBLocal);
        GroupBox1.Controls.Add(Fz);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(Fy);
        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(Fx);
        GroupBox1.Controls.Add(Label3);
        GroupBox1.Controls.Add(Label9);
        GroupBox1.Controls.Add(Caso);
        GroupBox1.Location = new Point(14, 14);
        GroupBox1.Margin = new Padding(4, 3, 4, 3);
        GroupBox1.Padding = new Padding(4, 3, 4, 3);
        GroupBox1.Size = new Size(371, 287);
        GroupBox1.TabIndex = 2;
        GroupBox1.TabStop = false;
        GroupBox1.Text = I18n.Loads;
        // 
        // GroupBox3
        // 
        GroupBox3.Controls.Add(RDFrac);
        GroupBox3.Controls.Add(RDDist);
        GroupBox3.Location = new Point(176, 130);
        GroupBox3.Margin = new Padding(4, 3, 4, 3);
        GroupBox3.Padding = new Padding(4, 3, 4, 3);
        GroupBox3.Size = new Size(188, 63);
        GroupBox3.TabIndex = 44;
        GroupBox3.TabStop = false;
        // 
        // RDFrac
        // 
        RDFrac.AutoSize = true;
        RDFrac.Checked = true;
        RDFrac.Location = new Point(91, 22);
        RDFrac.Margin = new Padding(4, 3, 4, 3);
        RDFrac.Size = new Size(70, 19);
        RDFrac.TabIndex = 37;
        RDFrac.TabStop = true;
        RDFrac.Text = I18n.Fraction;
        RDFrac.UseVisualStyleBackColor = true;
        // 
        // RDDist
        // 
        RDDist.AutoSize = true;
        RDDist.Location = new Point(4, 22);
        RDDist.Margin = new Padding(4, 3, 4, 3);
        RDDist.Size = new Size(73, 19);
        RDDist.TabIndex = 36;
        RDDist.Text = I18n.Distance;
        RDDist.UseVisualStyleBackColor = true;
        // 
        // Mz
        // 
        Mz.Location = new Point(46, 228);
        Mz.Margin = new Padding(4, 3, 4, 3);
        Mz.Size = new Size(90, 23);
        Mz.TabIndex = 43;
        // 
        // Label4
        // 
        Label4.AutoSize = true;
        Label4.Location = new Point(18, 232);
        Label4.Margin = new Padding(4, 0, 4, 0);
        Label4.Size = new Size(23, 15);
        Label4.TabIndex = 42;
        Label4.Text = "Mz";
        // 
        // M_y
        // 
        My.Location = new Point(46, 198);
        My.Margin = new Padding(4, 3, 4, 3);
        My.Size = new Size(90, 23);
        My.TabIndex = 41;
        // 
        // Label6
        // 
        Label6.AutoSize = true;
        Label6.Location = new Point(18, 202);
        Label6.Margin = new Padding(4, 0, 4, 0);
        Label6.Size = new Size(24, 15);
        Label6.TabIndex = 40;
        Label6.Text = "My";
        // 
        // Mx
        // 
        Mx.Location = new Point(46, 171);
        Mx.Margin = new Padding(4, 3, 4, 3);
        Mx.Size = new Size(90, 23);
        Mx.TabIndex = 39;
        // 
        // Label7
        // 
        Label7.AutoSize = true;
        Label7.Location = new Point(18, 174);
        Label7.Margin = new Padding(4, 0, 4, 0);
        Label7.Size = new Size(23, 15);
        Label7.TabIndex = 38;
        Label7.Text = "Mx";
        // 
        // a
        // 
        a.Location = new Point(195, 87);
        a.Margin = new Padding(4, 3, 4, 3);
        a.Size = new Size(90, 23);
        a.TabIndex = 37;
        a.Text = "0,5";
        // 
        // Label5
        // 
        Label5.AutoSize = true;
        Label5.Location = new Point(173, 87);
        Label5.Margin = new Padding(4, 0, 4, 0);
        Label5.Size = new Size(13, 15);
        Label5.TabIndex = 36;
        Label5.Text = "a";
        // 
        // RBGlobal
        // 
        RBGlobal.AutoSize = true;
        RBGlobal.Location = new Point(245, 227);
        RBGlobal.Margin = new Padding(4, 3, 4, 3);
        RBGlobal.Size = new Size(59, 19);
        RBGlobal.TabIndex = 35;
        RBGlobal.Text = I18n.Global;
        RBGlobal.UseVisualStyleBackColor = true;
        // 
        // RBLocal
        // 
        RBLocal.AutoSize = true;
        RBLocal.Checked = true;
        RBLocal.Location = new Point(173, 227);
        RBLocal.Margin = new Padding(4, 3, 4, 3);
        RBLocal.Size = new Size(53, 19);
        RBLocal.TabIndex = 34;
        RBLocal.TabStop = true;
        RBLocal.Text = I18n.Local;
        RBLocal.UseVisualStyleBackColor = true;
        // 
        // Fz
        // 
        Fz.Location = new Point(46, 141);
        Fz.Margin = new Padding(4, 3, 4, 3);
        Fz.Size = new Size(90, 23);
        Fz.TabIndex = 33;
        // 
        // Label2
        // 
        Label2.AutoSize = true;
        Label2.Location = new Point(18, 144);
        Label2.Margin = new Padding(4, 0, 4, 0);
        Label2.Size = new Size(18, 15);
        Label2.TabIndex = 32;
        Label2.Text = "Fz";
        // 
        // Fy
        // 
        Fy.Location = new Point(46, 111);
        Fy.Margin = new Padding(4, 3, 4, 3);
        Fy.Size = new Size(90, 23);
        Fy.TabIndex = 31;
        // 
        // Label1
        // 
        Label1.AutoSize = true;
        Label1.Location = new Point(18, 114);
        Label1.Margin = new Padding(4, 0, 4, 0);
        Label1.Size = new Size(19, 15);
        Label1.TabIndex = 30;
        Label1.Text = "Fy";
        // 
        // Fx
        // 
        Fx.Location = new Point(46, 83);
        Fx.Margin = new Padding(4, 3, 4, 3);
        Fx.Size = new Size(90, 23);
        Fx.TabIndex = 29;
        // 
        // Label3
        // 
        Label3.AutoSize = true;
        Label3.Location = new Point(18, 87);
        Label3.Margin = new Padding(4, 0, 4, 0);
        Label3.Size = new Size(18, 15);
        Label3.TabIndex = 28;
        Label3.Text = "Fx";
        // 
        // Label9
        // 
        Label9.AutoSize = true;
        Label9.Location = new Point(30, 21);
        Label9.Margin = new Padding(4, 0, 4, 0);
        Label9.Size = new Size(33, 15);
        Label9.TabIndex = 20;
        Label9.Text = I18n.Case;
        // 
        // Caso
        // 
        Caso.FormattingEnabled = true;
        Caso.Location = new Point(34, 39);
        Caso.Margin = new Padding(4, 3, 4, 3);
        Caso.Size = new Size(251, 23);
        Caso.TabIndex = 19;
        // 
        // ok
        ok.Location = new Point(10, 329);
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
        No.Location = new Point(105, 329);
        No.Margin = new Padding(4, 3, 4, 3);
        No.Size = new Size(90, 31);
        No.TabIndex = 4;
        No.Text = I18n.DlgCancelText;
        No.UseVisualStyleBackColor = true;
        No.Click += No_Click;


        btnDel.Location = new Point(200, 329);
        btnDel.Margin = new Padding(4, 3, 4, 3);
        btnDel.Size = new Size(90, 31);
        btnDel.TabIndex = 4;
        btnDel.Text = I18n.Delete;
        btnDel.UseVisualStyleBackColor = true;
        btnDel.Click += BtnDel_Click; ;

        btnReplace.Location = new Point(295, 329);
        btnReplace.Margin = new Padding(4, 3, 4, 3);
        btnReplace.Size = new Size(90, 31);
        btnReplace.TabIndex = 4;
        btnReplace.Text = I18n.Replace;
        btnReplace.UseVisualStyleBackColor = true;
        btnReplace.Click += BtnReplace_Click; ;
        // 
        // AsCargaPunt_Barras
        // 
        AcceptButton = ok;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = No;
        ClientSize = new Size(420, 413);
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(btnDel);
        Controls.Add(btnReplace);
        Controls.Add(GroupBox1);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = I18n.SetPunctBarLoads;
        Load += FormLoad;
        GroupBox1.ResumeLayout(false);
        GroupBox1.PerformLayout();
        GroupBox3.ResumeLayout(false);
        GroupBox3.PerformLayout();
        ResumeLayout(false);
    }
    

    #endregion

    public double MaxQ;
    public addpBarLoad()
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

        setLoad();
            App.model.setParamValue(Mdta.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
            if (this.Owner is MainWindow cedm)
                cedm.reloadAllCanvasWindows();
        Close();  
    }
    private void FormLoad(object sender, EventArgs e)
    {
        int n = App.model.loadCases.Count;
        for (int i = 0; i < n; i++)
            Caso.Items.Add(App.model.loadCases[i].ID + ": " + App.model.loadCases[i].Name);
    }


    private void deleteDLoad()
    {
        int N = Selection.barMaxIndex + 1;
        double L;
        var Sel = Selection.getBarsColection();
        if (Sel == null) return;

        var Case = App.model.loadCases[Caso.SelectedIndex].ID;
        var idselBars = Sel.Select(i => App.model.bars[i].ID).ToList();
        if (idselBars == null) return;

        for (int i = 0; i < N; i++)
        {
            var indB = idselBars[i];
            bar barra = App.model.bars[indB];
            var barID = barra.ID;

            var loads = App.model.barPunctLoads.
                Where(l => l.BarID == barID && l.Case == Case).Select(l => l.ID).ToList();
            for (int j = 0; j < loads.Count; j++)
            {
                var lindx = App.model.barPunctLoads.FindIndexByID(loads[j]);
                App.model.barPunctLoads.RemoveAt(lindx);

            }

        }

    }

    private void setLoad()
    {
        int N = Selection.barMaxIndex + 1;
        var Sel = Selection.getBarsColection();
        int Ni, Nj, indB;
        double  L;

        var fx = uc.forceFromUserUnits(Fx.Text);
        var fy = uc.forceFromUserUnits(Fy.Text);
        var fz = uc.forceFromUserUnits(Fz.Text);
        var mx = uc.forceFromUserUnits(Mx.Text);
        var my = uc.forceFromUserUnits(My.Text);
        var mz = uc.forceFromUserUnits(Mz.Text);

        for (int i = 0; i < N; i++)
        {
            indB = Sel[i];
            var barra = App.model.bars[indB];
            Ni = barra.startNode;
            Nj = barra.endNode;
            var nodoI = App.model.nodes[App.model.nodes.FindIndexByID(Ni)];
            var nodoJ = App.model.nodes[App.model.nodes.FindIndexByID(Nj)];
            L = Mis.calcDist(nodoI, nodoJ);
            var id = App.model.barPunctLoads.Count + 1;
            var carga = new barPunctLoad()
            {
                ID = id,
                BarID = barra.ID,
                Case = App.model.loadCases[Caso.SelectedIndex].ID,
                Type = RBGlobal.Checked ? 0 : 1,
                a = RDDist.Checked ? Mis.stringToDouble(a.Text, L) / L : Mis.stringToDouble(a.Text, 1d),
                Fx =fx,
                Fy =fy,
                Fz =fz,
                Mx =mx,
                My =my,
                Mz =mz
            };
            App.model.barPunctLoads.Add(carga);
        }
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



}