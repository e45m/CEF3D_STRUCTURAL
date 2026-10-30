using System;
namespace CEF;
public  class setNodDipla : Form
{
    #region UI


 
    internal Button ok, No, btnDel, btnReplace;
    internal GroupBox GroupBox1;
    internal TextBox dz, dy, dx,  rz, ry, rx;
    
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
        dx = dlgStyle.editableField(this,"dx",20,90,23,90);
        dy = dlgStyle.editableField(this,"dy",20,118,23,90);
        dz = dlgStyle.editableField(this,"dz",20,148,23,90);

        rx = dlgStyle.editableField(this,"rx",170,90,24,90);
        ry = dlgStyle.editableField(this,"ry",170,118,24,90);
        rz = dlgStyle.editableField(this,"rz",170,148,24,90);
        
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

            var loads = App.model.nodalDisplacements.
                Where(l => l.NodeID == nodID && l.Case == Case).Select(l => l.NodeID).ToList();
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
            App.model.nodalDisplacements[index].ID = App.model.nodalDisplacements.NextID();
            App.model.nodalDisplacements[index].NodeID = Sel[i];
            App.model.nodalDisplacements[index].dx = Uc.lenFromUserUnits(dx.Text);
            App.model.nodalDisplacements[index].dy = Uc.lenFromUserUnits(dy.Text);
            App.model.nodalDisplacements[index].dz = Uc.lenFromUserUnits(dz.Text);
            App.model.nodalDisplacements[index].rx = Uc.lenFromUserUnits(rx.Text);
            App.model.nodalDisplacements[index].ry = Uc.lenFromUserUnits(ry.Text);
            App.model.nodalDisplacements[index].rz = Uc.lenFromUserUnits(rz.Text);
        }
    }
    }