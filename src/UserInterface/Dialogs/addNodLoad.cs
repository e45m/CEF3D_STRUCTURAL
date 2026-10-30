using System;
namespace CEF;
public class addNodLoad: Form
{
    #region UI

    internal Button ok, No, btnDel, btnReplace;
    internal GroupBox GroupBox1;
    internal TextBox Fz, Fy, Fx,  Mz, My, Mx;
    
    internal ComboBox Caso;

    private void InitializeComponent()
    {
        ok = dlgStyle.button(I18n.DlgOKText,10,250,90,true);
        ok.Click += ok_Click;

        No= dlgStyle.button(I18n.DlgCancelText,105,250,90);
        No.Click += No_Click;
 
        btnDel= dlgStyle.button(I18n.Delete,200,250,90);
        btnDel.Click += BtnDel_Click;

        btnReplace = dlgStyle.button(I18n.Replace,295,250,90);
        btnReplace.Click += BtnReplace_Click; 
 
         GroupBox1 = dlgStyle.group(I18n.Loads,14,8,322,233);
        Fx = dlgStyle.editableField(this,"Fx",20,90,23,90);
        Fy = dlgStyle.editableField(this,"Fy",20,118,23,90);
        Fz = dlgStyle.editableField(this,"Fz",20,148,23,90);

        Mx = dlgStyle.editableField(this,"Mx",170,90,24,90);
        My = dlgStyle.editableField(this,"My",170,118,24,90);
        Mz = dlgStyle.editableField(this,"Mz",170,148,24,90);
        
        Caso = dlgStyle.combo(this, I18n.Case,44,25,48,180,[]);  
       
        dlgStyle.applyForm(this);
        Text = I18n.SetNodalLoads;
        Controls.Add(No);
        Controls.Add(ok);
        Controls.Add(btnDel);
        Controls.Add(btnReplace);
        Controls.Add(GroupBox1);        
        Load += FormLoad;
        ClientSize = new Size(402, 300);
        AcceptButton = ok;
        CancelButton = No;
        ShowIcon = false;
        

    }
   


    #endregion


    public addNodLoad()
    {
        InitializeComponent();
    }
    private void FormLoad(object sender, EventArgs e)
    {
        int n = App.model.loadCases.Count;
        if(n>0) Caso.Enabled = true;
        for (int i = 0; i <n; i++)
            Caso.Items.Add(App.model.loadCases[i].ID + ": " + App.model.loadCases[i].Name);
        
    }
    private void ok_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        setLoad();
            App.model.setParamValue(paramDefaults.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
            if (this.Owner is MainWindow cedm)
                cedm.reloadAllCanvasWindows();
        Close();  
    }
    private void No_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void BtnReplace_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteDLoad();
        setLoad();
        App.model.setParamValue(paramDefaults.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
        if (this.Owner is MainWindow cedm)
            cedm.reloadAllCanvasWindows();
        Close();

    }

    private void BtnDel_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteDLoad();
        App.model.setParamValue(paramDefaults.ActiveCase, App.model.loadCases[Caso.SelectedIndex].ID);
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
            Node Node = App.model.nodes[indNod];
            var nodID = Node.ID;

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

        var fx = Uc.forceFromUserUnits(Fx.Text);
        var fy = Uc.forceFromUserUnits(Fy.Text);
        var fz = Uc.forceFromUserUnits(Fz.Text);
        var mx = Uc.momentFromUserUnits(Mx.Text);
        var my = Uc.momentFromUserUnits(My.Text);
        var mz = Uc.momentFromUserUnits(Mz.Text);

        for (i = 0; i < N; i++)
        {

 
            var nl = new nodalLoad()
            {
                ID = App.model.nodalLoads.NextID(),
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