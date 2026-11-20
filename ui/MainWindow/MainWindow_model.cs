using CEF.matrixImpl;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using static CEF.modelView;
using static System.Math;
namespace CEF;
public partial class MainWindow
{
    internal void nodalLoadDlg(object sender, EventArgs e)
    {
        var mcw = new addNodLoad();
        mcw.Show(this);
    }
    internal void restraintDlg(object sender, EventArgs e)
    {
        var mcw = new setContournRestr();
        mcw.Show(this);
    }
    internal async void runLinearAnalysis(object sender, EventArgs e)
    {
        App.stopUndoRecording();
        deleteResultsFromDB();
        tbStatusText2.Text = "---⏱"; 
        enableDisable(false);
        var calc = new Calculo();
         await Task.Run(()=>calc.Calcular(Engine.getModel(),Engine.getAppData()));
        Engine.getModel().setParamValue(paramDefaults.analysed, 1);
        tbStatusText2.Text = "";   
        enableDisable(true); 
        enableDisable_calc(false);
        App.startUndoRecording();
       
    }
    internal async void runLinearPDAnalysis(object sender, EventArgs e)
    {
        App.stopUndoRecording();
        deleteResultsFromDB();
        tbStatusText2.Text = "⏱⏱⏱⏱⏱";
        enableDisable(false);
        var calc = new Calculo();
        var pdIter = Engine.getModel().getParamValue(paramDefaults.pDeltaIters);
        var pdConf = Engine.getModel().getParamText(paramDefaults.pDeltaLoadComb);
        Mis.parceStringToIntDoubArray(pdConf, out int[] casesId, out double[] loadFactors);
        calc.setPdParams(pdIter, casesId, loadFactors);
        await Task.Run(()=>calc.Calcular(Engine.getModel(),Engine.getAppData(), true/*<--TRUE*/));
        Engine.getModel().setParamValue(paramDefaults.analysed, 1);
        enableDisable(true);
        tbStatusText2.Text = "";
        enableDisable_calc(false);
        App.startUndoRecording();
    }
    internal void analysisOptionsDlg(object sender, EventArgs e)
    {  
        var vw = new analysisOptions();       
       vw.Show(this);
    }
    internal void setBarsSectionDlg(object sender, EventArgs e)
    {
        var mcw = new setBarSection();
        mcw.Show(this);
    }
    internal void openMaterialDlg(object sender, EventArgs e)
    {
        var mcw = new addMaterialToModel();
        mcw.Show(this);
    }
    internal void openSectionDlg(object sender, EventArgs e)
    {
        var mcw = new addSectionsToModel();
        mcw.Show(this);
    }
        internal void openAreaSectionDlg(object sender, EventArgs e)
    {
        var mcw = new defineAreaSection();
        mcw.Show(this);
    }

	 internal void nodalDisplDlg(object sender, EventArgs e)
 {
     var vbd = new setNodDipla();
     vbd.Show(this);
 }
     internal void setupNewCoordSys(object sender, EventArgs e)
    {
        App.stopUndoRecording();
        var CSC = new defineNewCoordSys(this);
        CSC.ShowDialog();
        App.startUndoRecording();

    }
    internal void barInertnalForcesGraphView(object sender, EventArgs e)
   {
       var vw = new viewInternalBarActions();       
       vw.Show(this);
   }
    internal void setDistrBarLoadsDlg(object sender, EventArgs e)
    {
        var vw = new adddBarLoad();
        vw.Show(this);
    }
    internal void setPunctBarLoadsDlg(object sender, EventArgs e)
    {
        var vw = new addpBarLoad();
        vw.Show(this);
    }
    internal void setupNewLoadCase(object sender, EventArgs e)
    {
        var vw = new defineLoadCase();
        vw.Show(this);
    }

  internal void OpcionesVerTSMI_Click(object sender, EventArgs e)
  {
  }
  internal void mfconstraintsDlg(object sender, EventArgs e)
  {
      var vbd = new addNodalMFC();
      vbd.Show(this);
  }
  internal void setBarReleasedDOF(object sender, EventArgs e)
  {
      var vbd = new addBarReleases();
      vbd.Show(this);
  }
  internal void showModelTree(object sender, EventArgs e)
  {
        var vbd = new viewModelDBTree(); 
        vbd.Show(this);        
     
  }
  internal void showModelInTable(object sender, EventArgs e)
  {
      var vbd = new viewModelDBTable(["*"]);
      vbd.Show(this);
  }
    internal void setBarInsertionPoint(object sender, EventArgs e)
  {
      /* Asignable y revisable en el dialogo de asignación desasignación de liberaciones*/
      var vbd = new setInsertBarPoint();
      vbd.Show(this);
  }

    internal void InvertBarAxes(object sender, EventArgs e)
    {   App.takeUndoSnapShot();
        BarAxes.reverseBarDirection(Engine.getModel(),Selection.getBarsColection());

    }
    internal void AligneBarWithFirst(object sender, EventArgs e)
    {   App.takeUndoSnapShot();
        BarAxes.alignBarAxes(Engine.getModel(),Selection.getBarsColection());

    }


   internal void setUnitsFromMenu(object sender, EventArgs e)
 {
     //App.takeUndoSnapShot();

     var send = (ToolStripComboBox)sender;
       
     Uc.ChangeUc(send.Text);
     App.currUnits = send.Text;
     Engine.getModel().setParamText(paramDefaults.units, send.Text);
  
 }



    internal void loadCombDlg(object sender, EventArgs e)
   {
       var vw = new loadCombDlg();       
       vw.Show(this);
   }

    internal void LoadCasesList(object sender, EventArgs e)
    {   if(Engine.getModel()==null) return;
        var send = (ToolStripComboBox)sender;
        send.Items.Clear();
        var cases = Engine.getModel().loadCases.        
            Select(lc => lc.Name).Distinct().ToArray();
        send.Items.AddRange(cases);
        send.SelectedIndex = 0;


    }

    internal void chooseActiveLoadCase(object sender, EventArgs e)
    {

        var clcIndx = ((ToolStripComboBox)sender).SelectedIndex;
        if ( clcIndx<0) clcIndx=0;
        var clcId = Engine.getModel().loadCases[clcIndx].ID;

        App.currLoadCase = clcId;
        Engine.getModel().setParamValue(paramDefaults.ActiveCase, App.currLoadCase);
        reloadAllCanvasWindows();
    }
    internal void deleteResultsFromMenu(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteResultsFromDB();        
        enableDisable_calc(true);
    }
    internal static void deleteResultsFromDB()
    {
        if(Engine.getModel() == null) return;

        Engine.getModel().nodalReactions = [];
        Engine.getModel().nodalDisplacements = [];
        Engine.getModel().barInternForces = [];
        Engine.getModel().areaInternalForces = [];

        Engine.getAppData().biFSeted = false;
        Engine.getAppData().barStatiosForces = [];
        
        var prefix = App.baseLabCombCases;        

        var ids = Engine.getModel().loadCases.Where(x=>x.Type == (int)LoadCaseType.Combination && x.Name.StartsWith(prefix)).ToArray();
        foreach(var i in ids)
        {
            var pos = Engine.getModel().loadCases.FindIndexByID(i.ID);            
            Engine.getModel().loadCases.RemoveAt(pos);
        }
        Engine.getModel().setParamValue(paramDefaults.analysed, 0);
    }


}