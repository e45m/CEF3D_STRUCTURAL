using System;
namespace CEF;
public class analysisOptions: Form
{
    #region UI

     Button ok, No;
     CheckBox  AutoMeshBars, AutoMeshAreas;
    TextBox Iters, PdCom;         
    
    private void InitializeComponent()
    {
        int x0,y0;
        x0=25;y0=25;
        AutoMeshBars = dlgStyle.checkBox(this,I18n.BarAutomesh,x0,y0+=25,80,true);
        AutoMeshAreas = dlgStyle.checkBox(this,I18n.AreaAutomesh,x0,y0+=25,80,true);
        //Use EditableField to allow modifications
        Iters = dlgStyle.field(this,paramDefaults.pDeltaIters,x0,y0+=25,150,50);
        dlgStyle.label(this,$"1.0D+0.25L -> {{0,1}}{{1.0,0.25}}", x0,y0+=35,200);
        PdCom = dlgStyle.editableField(this,paramDefaults.pDeltaLoadComb,x0,y0+=25,150,150);

        ok = dlgStyle.button(I18n.DlgOKText,10,250,90,true);
        ok.Click += ok_Click;
        No= dlgStyle.button(I18n.DlgCancelText,105,250,90);
        No.Click += No_Click;     
               
        dlgStyle.applyForm(this);
        Text = I18n.AnalysisOptions;
        Controls.AddRange([No,ok]);   
             
       // Load += FormLoad;
        ClientSize = new Size(402, 300);
        AcceptButton = ok;
        CancelButton = No;
        ShowIcon = false;
        Load+= DlgLoad;
        

    }
   


    #endregion


    public analysisOptions()
    {
        InitializeComponent();
    }

    private void ok_Click(object sender, EventArgs e)
    {
        Engine.getModel().setParamValue(paramDefaults.BarAutoMesh,AutoMeshBars.Checked?1:0 , true);
        Engine.getModel().setParamValue(paramDefaults.AreaAutoMeh,AutoMeshAreas.Checked?1:0, true ); 

        Engine.getModel().setParamText(paramDefaults.pDeltaIters   ,Iters.Text);
        Engine.getModel().setParamText(paramDefaults.pDeltaLoadComb,PdCom.Text);

        Close();
    }
    private void No_Click(object sender, EventArgs e)
    {
        Close();
    }

   public void DlgLoad(object sender, EventArgs e)   
   {    
    AutoMeshBars.Checked=Engine.getModel().getParamValue(paramDefaults.BarAutoMesh)==1;
    AutoMeshAreas.Checked=Engine.getModel().getParamValue(paramDefaults.AreaAutoMeh)==1;

    Iters.Text = Engine.getModel().getParamText(paramDefaults.pDeltaIters);
    PdCom.Text = Engine.getModel().getParamText(paramDefaults.pDeltaLoadComb);

   }


   

}