using CEF.matrixImpl;
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
         await Task.Run(()=>calc.Calcular(App.model,App.appData, false));
        App.model.setParamValue(paramDefaults.analysed, 1);
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
        var pdIter = App.model.getParamValue(paramDefaults.pDeltaIters);
        var pdConf = App.model.getParamText(paramDefaults.pDeltaLoadComb);
        Mis.parceStringToIntDoubArray(pdConf, out int[] casesId, out double[] loadFactors);
        calc.setPdParams(1, casesId, loadFactors);
        await Task.Run(()=>calc.Calcular(App.model,App.appData, false));
        App.model.setParamValue(paramDefaults.analysed, 1);
        enableDisable(true);
        tbStatusText2.Text = "";
        enableDisable_calc(false);
        App.startUndoRecording();
    }
    internal void analysisOptionsDlg(object sender, EventArgs e)
    {
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
  internal void showModelTables(object sender, EventArgs e)
  {
        var vbd = new viewModelDBTree(); 
        vbd.Show(this);        
     
  }
  internal void showModelInTreeView(object sender, EventArgs e)
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
   internal void setUnitsFromMenu(object sender, EventArgs e)
 {
     App.takeUndoSnapShot();

     var send = (ToolStripComboBox)sender;
       
     Uc.ChangeUc(send.Text);
     App.currUnits = send.Text;
     App.model.setParamText(paramDefaults.units, send.Text);
  
 }
    internal void loadLoadCasesList(object sender, EventArgs e)
    {
        var send = (ToolStripComboBox)sender;
        send.Items.Clear();
        var cases = App.model.loadCases.
            Select(lc => lc.Name).Distinct().ToArray();
        send.Items.AddRange(cases);


    }

    internal void chooseActiveLoadCase(object sender, EventArgs e)
    {

        var clcIndx = ((ToolStripComboBox)sender).SelectedIndex;
        if ( clcIndx<0) clcIndx=0;
        var clcId = App.model.loadCases[clcIndx].ID;

        App.currLoadCase = clcId;
        App.model.setParamValue(paramDefaults.ActiveCase, App.currLoadCase);
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
        App.model.nodalReactions = [];
        App.model.nodalDisplacements = [];
        App.model.barInternForces = [];

        App.appData.biFSeted = false;
        App.appData.barStatiosForces = [];
        
        var prefix = App.baseLabCombCases;        

        var ids = App.model.loadCases.Where(x=>x.Type == (int)LoadCaseType.Combination && x.Name.StartsWith(prefix)).ToArray();
        foreach(var i in ids)
        {
            var pos = App.model.loadCases.FindIndexByID(i.ID);            
            App.model.loadCases.RemoveAt(pos);
        }
        App.model.setParamValue(paramDefaults.analysed, 0);
    }


}