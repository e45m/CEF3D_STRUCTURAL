using System;
using static System.Math;
using static CEF.ModelDB;
namespace CEF;
public class addpBarLoad: Form
{
    #region UI
  internal GroupBox GroupBox1,GroupBox3, GroupBox2;
    internal ComboBox Caso;
    internal RadioButton RBGlobal, RBLocal,RDDist, RDFrac;
    internal TextBox Fz, Fy, Fx,Mx,My,Mz, /*b,*/ a;   
    internal Button ok,  No, btnDel, btnReplace;

    private void InitializeComponent()
    {

        GroupBox1 = dlgStyle.group(I18n.Loads,20,8,398,200);
        GroupBox2 = dlgStyle.group("",20,210,200,63);
        GroupBox3 = dlgStyle.group("",220,210,200,63);

        Fx = dlgStyle.editableField(GroupBox1,"Fx",20,60,23,90);
        Fy = dlgStyle.editableField(GroupBox1,"Fy",20,88,23,90);
        Fz = dlgStyle.editableField(GroupBox1,"Fz",20,118,23,90);

        Mx = dlgStyle.editableField(GroupBox1,"Mx",170,60,24,90);
        My = dlgStyle.editableField(GroupBox1,"My",170,88,24,90);
        Mz = dlgStyle.editableField(GroupBox1,"Mz",170,118,24,90);         

        a = dlgStyle.editableField(GroupBox1,"a",20,148,24,90); 
        //b = dlgStyle.editableField(GroupBox1,"b",170,148,24,90); 
        a.Text = "0.5";
       // b.Text = "1";

        Caso = dlgStyle.combo(GroupBox1, I18n.Case,44,25,48,180,[]);  

        RDFrac = dlgStyle.radioButton(GroupBox2,I18n.Fraction,8,22,70);
        RDDist = dlgStyle.radioButton(GroupBox2,I18n.Distance,91,22,70);

        RBGlobal = dlgStyle.radioButton(GroupBox3,I18n.Global,8,22,70);
        RBLocal = dlgStyle.radioButton(GroupBox3,I18n.Local,91,22,70);
        RDFrac.Checked=RBLocal.Checked = true;


        ok = dlgStyle.button(I18n.DlgOKText,20,280,90,true);
        ok.Click += ok_Click;

        No= dlgStyle.button(I18n.DlgCancelText,115,280,90);
        No.Click += No_Click;
 
        btnDel= dlgStyle.button(I18n.Delete,210,280,90);
        btnDel.Click += BtnDel_Click;

        btnReplace = dlgStyle.button(I18n.Replace,305,280,90);
        btnReplace.Click += BtnReplace_Click; 

        
        AcceptButton = ok;
        CancelButton = No;
        ClientSize = new Size(425, 320);   
        Text = I18n.barDistrLoads;
        Load += loadForm;
        dlgStyle.applyForm(this);

        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Controls.AddRange(new Control[]{GroupBox1,GroupBox2,GroupBox3,ok,No,btnDel,btnReplace}  );
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
            Engine.getModel().setParamValue(paramDefaults.ActiveCase, Engine.getModel().loadCases[Caso.SelectedIndex].ID);
            if (this.Owner is MainWindow cedm)
                cedm.reloadAllCanvasWindows();
        Close();  
    }
    private void loadForm(object sender, EventArgs e)
    {
        int n = Engine.getModel().loadCases.Count;
        if(n>0) Caso.Enabled = true;
        for (int i = 0; i < n; i++)
            Caso.Items.Add(Engine.getModel().loadCases[i].ID + ": " + Engine.getModel().loadCases[i].Name);
    }


    private void deleteDLoad()
    {
        int N = Selection.barMaxIndex + 1;
        var Sel = Selection.getBarsColection();
        if (Sel == null) return;

        var Case = Engine.getModel().loadCases[Caso.SelectedIndex].ID;
        var idselBars = Sel.Select(i => Engine.getModel().bars[i].ID).ToList();
        if (idselBars == null) return;

        for (int i = 0; i < N; i++)        {
   
            var barID = idselBars[i];

            var loads = Engine.getModel().barPunctLoads.
                Where(l => l.BarID == barID && l.Case == Case).Select(l => l.ID).ToList();
            for (int j = 0; j < loads.Count; j++)
            {
                var lindx = Engine.getModel().barPunctLoads.FindIndexByID(loads[j]);
                Engine.getModel().barPunctLoads.RemoveAt(lindx);

            }

        }

    }

    private void setLoad()
    {
        int N = Selection.barMaxIndex + 1;
        var Sel = Selection.getBarsColection();
        int Ni, Nj, indB;
        double  L;

        var fx = Uc.forceFromUserUnits(Fx.Text);
        var fy = Uc.forceFromUserUnits(Fy.Text);
        var fz = Uc.forceFromUserUnits(Fz.Text);
        var mx = Uc.momentFromUserUnits(Mx.Text);
        var my = Uc.momentFromUserUnits(My.Text);
        var mz = Uc.momentFromUserUnits(Mz.Text);

        for (int i = 0; i < N; i++)
        {
            indB = Sel[i];
            var barra = Engine.getModel().bars[indB];
            Ni = barra.startNode;
            Nj = barra.endNode;
            var nodoI = Engine.getModel().nodes.FindByID(Ni);
            var nodoJ = Engine.getModel().nodes.FindByID(Nj);
            L = space3d.calcDist(nodoI, nodoJ);            
            var id = Engine.getModel().barPunctLoads.NextID();
            var carga = new barPunctLoad()
            {
                ID = id,
                BarID = barra.ID,
                Case = Engine.getModel().loadCases[Caso.SelectedIndex].ID,
                Type = RBGlobal.Checked ? 0 : 1,
                l1 = RDDist.Checked ? Mis.stringToDouble(a.Text, L) / L : Mis.stringToDouble(a.Text, 1d),
                Fx =fx,
                Fy =fy,
                Fz =fz,
                Mx =mx,
                My =my,
                Mz =mz
            };
            Engine.getModel().barPunctLoads.Add(carga);
        }
    }

    private void BtnReplace_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteDLoad();
        setLoad();
        Engine.getModel().setParamValue(paramDefaults.ActiveCase, Engine.getModel().loadCases[Caso.SelectedIndex].ID);
        if (this.Owner is MainWindow cedm)
            cedm.reloadAllCanvasWindows();
        Close();

    }

    private void BtnDel_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteDLoad();
        Engine.getModel().setParamValue(paramDefaults.ActiveCase, Engine.getModel().loadCases[Caso.SelectedIndex].ID);
        if (this.Owner is MainWindow cedm)
            cedm.reloadAllCanvasWindows();
        Close();


    }



}