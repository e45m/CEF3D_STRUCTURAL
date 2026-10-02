using System;
using System.Windows.Forms;
using static System.Math;
using static CEF.ModelDB;
namespace CEF;
public class adddBarLoad:Form
{

    #region UI
    internal GroupBox GroupBox1;
    internal CheckBox Iguales;
    internal ComboBox Caso;
    internal Label Label9;
    internal RadioButton RBGlobal, RBLocal,RDDist, RDFrac;
    internal TextBox Fz, Fy, Fx, b, a;
    internal Label Label1, Label2, Label3, Label4, Label5;
    internal GroupBox GroupBox3, groupBox2;
    internal Button ok,  No, btnDel, btnReplace;

    private void InitializeComponent()
    {
        GroupBox1 = new GroupBox();
        GroupBox3 = new GroupBox();
        RDFrac = new RadioButton();
        RDDist = new RadioButton();
        b = new TextBox();
        Label4 = new Label();
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
        Iguales = new CheckBox();
        ok = new Button();
        No = new Button();
        btnDel = new Button();
        btnReplace = new Button();
        groupBox2 = new GroupBox();
        GroupBox1.SuspendLayout();
        GroupBox3.SuspendLayout();
        groupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // GroupBox1
        // 
        GroupBox1.Controls.Add(groupBox2);
        GroupBox1.Controls.Add(GroupBox3);
        GroupBox1.Controls.Add(b);
        GroupBox1.Controls.Add(Label4);
        GroupBox1.Controls.Add(a);
        GroupBox1.Controls.Add(Label5);
        GroupBox1.Controls.Add(Fz);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(Fy);
        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(Fx);
        GroupBox1.Controls.Add(Label3);
        GroupBox1.Controls.Add(Label9);
        GroupBox1.Controls.Add(Caso);
        GroupBox1.Controls.Add(Iguales);
        GroupBox1.Location = new Point(14, 14);
        GroupBox1.Margin = new Padding(4, 3, 4, 3);
        GroupBox1.Padding = new Padding(4, 3, 4, 3);
        GroupBox1.Size = new Size(383, 309);
        GroupBox1.TabIndex = 2;
        GroupBox1.TabStop = false;
        GroupBox1.Text = I18n.Loads;
        // 
        // GroupBox3
        // 
        GroupBox3.Controls.Add(RDFrac);
        GroupBox3.Controls.Add(RDDist);
        GroupBox3.Location = new Point(176, 153);
        GroupBox3.Margin = new Padding(4, 3, 4, 3);
        GroupBox3.Padding = new Padding(4, 3, 4, 3);
        GroupBox3.Size = new Size(188, 63);
        GroupBox3.TabIndex = 45;
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
        // b
        // 
        b.Location = new Point(201, 111);
        b.Margin = new Padding(4, 3, 4, 3);
        b.Size = new Size(90, 23);
        b.TabIndex = 39;
        b.Text = "1";
        // 
        // Label4
        // 
        Label4.AutoSize = true;
        Label4.Location = new Point(173, 114);
        Label4.Margin = new Padding(4, 0, 4, 0);
        Label4.Size = new Size(14, 15);
        Label4.TabIndex = 38;
        Label4.Text = "b";
        // 
        // a
        // 
        a.Location = new Point(201, 83);
        a.Margin = new Padding(4, 3, 4, 3);
        a.Size = new Size(90, 23);
        a.TabIndex = 37;
        a.Text = "0";
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
        RBGlobal.Location = new Point(81, 22);
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
        RBLocal.Location = new Point(8, 22);
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
        Fz.Name = "Fz";
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
        Fy.Name = "Fy";
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
        Fx.Name = "Fx";
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
        Caso.Size = new Size(257, 23);
        Caso.TabIndex = 19;
        // 
        // Iguales
        // 
        Iguales.AutoSize = true;
        Iguales.Checked = true;
        Iguales.CheckState = CheckState.Checked;
        Iguales.Enabled = false;
        Iguales.Location = new Point(17, 284);
        Iguales.Margin = new Padding(4, 3, 4, 3);
        Iguales.Size = new Size(169, 19);
        Iguales.TabIndex = 16;
        Iguales.Text = "";
        Iguales.UseVisualStyleBackColor = true;
        // 
        // ok
        // 
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
        // groupBox2
        // 
        groupBox2.Controls.Add(RBGlobal);
        groupBox2.Controls.Add(RBLocal);
        groupBox2.Location = new Point(400, 218);
        groupBox2.Margin = new Padding(4, 3, 4, 3);
        groupBox2.Padding = new Padding(4, 3, 4, 3);
        groupBox2.Size = new Size(188, 63);
        groupBox2.TabIndex = 46;
        groupBox2.TabStop = false;
        // 
        // adddBarLoad
        // 
        AcceptButton = ok;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(415, 383);
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(btnDel);
        Controls.Add(btnReplace);
        Controls.Add(GroupBox1);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "adddBarLoad";
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = I18n.barDistrLoads;
        Load += loadForm;
        GroupBox1.ResumeLayout(false);
        GroupBox1.PerformLayout();
        GroupBox3.ResumeLayout(false);
        GroupBox3.PerformLayout();
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        ResumeLayout(false);
    }

    private void BtnReplace_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteDLoad();
        setLoad();
        App.model.setParamValue(Mdta.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
        if (this.Owner is MainWindow cedm)
            cedm.reloadAllCanvasWindows();
        Close();

    }

    private void BtnDel_Click(object sender, EventArgs e)
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
        int N = Selection.barMaxIndex + 1;
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

            var loads = App.model.barDistrLoads.
                Where(l => l.BarID == barID && l.Case == Case).Select(l => l.ID).ToList();
            for (int j = 0; j < loads.Count; j++)
            {
                var lindx = App.model.barDistrLoads.FindIndexByID(loads[j]);
                App.model.barDistrLoads.RemoveAt(lindx);

            }

        }

    }

    private void setLoad()
    {
        int N = Selection.barMaxIndex + 1;
        int Ni, Nj, indB;
        double L;
        double f1, f2, f3;
        var Sel = Selection.getBarsColection();
        f1 = uc.forceFromUserUnits(Fx.Text);
        f2 = uc.forceFromUserUnits(Fy.Text);
        f3 = uc.forceFromUserUnits(Fz.Text);
        for (int i = 0; i < N; i++)
        {
            indB = Sel[i];
            bar bar = App.model.bars[indB];
            Ni = bar.startNode;
            Nj = bar.endNode;
            node nodoI = App.model.nodes[Ni];
            node nodoJ = App.model.nodes[Nj];
            L = Mis.calcDist(nodoI, nodoJ);
            var load = new barDistrLoad()
            {
                ID = App.model.barDistrLoads.Count,
                BarID = bar.ID,
                Case = App.model.loadCases[Caso.SelectedIndex].ID,
                Type = RBGlobal.Checked ? 0 : 1,
                a = RDDist.Checked ? Mis.stringToDouble(a.Text) / L : Mis.stringToDouble(a.Text),
                b = RDDist.Checked ? Mis.stringToDouble(b.Text, L) / L : Mis.stringToDouble(b.Text, 1d),
                Fx = f1,
                Fy = f2,
                Fz = f3,
                Mx = 0d,
                My = 0d,
                Mz = 0d
            };
            App.model.barDistrLoads.Add(load);
        }
    }


    #endregion


    public double MaxQ;
    public adddBarLoad()
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
    private void loadForm(object sender, EventArgs e)
    {
        int n = App.model.loadCases.Count;
        for (int i = 0; i < n; i++)
            Caso.Items.Add(App.model.loadCases[i].ID + ": " + App.model.loadCases[i].Name);
    }
}