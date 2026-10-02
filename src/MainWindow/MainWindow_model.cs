using CEF.matrixImpl;
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
    internal void runLinearAnalysis(object sender, EventArgs e)
    {
        //CEFglobal.restartUndo();

        deleteResultsFromDB();
        ProgressStatus.Value = 0;
        tbStatusText2.Text = "Empezando...";
        var progress = new Progress<(int, string)>(v => { ProgressStatus.Value = v.Item1; tbStatusText2.Text = v.Item2; });
        var calc = new Calculo();
        calc.Calcular(App.model, false, progress);
        App.model.setParamValue(Mdta.analysed, 1);
    }
    internal void runLinearPDAnalysis(object sender, EventArgs e)
    {
        //CEFglobal.restartUndo();
        deleteResultsFromDB();
        ProgressStatus.Value = 0;
        tbStatusText2.Text = "Empezando...";
        var progress = new Progress<(int, string)>(v => { ProgressStatus.Value = v.Item1; tbStatusText2.Text = v.Item2; });
        var calc = new Calculo();
        var pdIter = App.model.getParamValue(Mdta.pDeltaIters);
        var pdConf = App.model.getParamText(Mdta.pDeltaLoadComb);
        Mis.parceStringToIntDoubArray(pdConf, out int[] casesId, out double[] loadFactors);
        calc.setPdParams(1, casesId, loadFactors);
        calc.Calcular(App.model, true, progress);
        App.model.setParamValue(Mdta.analysed, 1);
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
    internal void setBarsMaterialDlg(object sender, EventArgs e)
    {
        var mcw = new setBarMaterial();
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
       
     uc.ChangeUc(send.Text);
     App.model.setParamText(Mdta.units, send.Text);
     modelView A; 
     List<Form> listChild;
     listChild = OwnedForms.Where(x => x is modelView).ToList(); 
     foreach (var currentA in listChild)
     {
         A = (modelView)currentA;
         A.ViewFrame.setUpGridCoords();
     }
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
        App.currLoadCase = ((ToolStripComboBox)sender).SelectedIndex;
        App.model.setParamValue(Mdta.ActiveCase, App.currLoadCase);
        reloadAllCanvasWindows();
    }
    internal void deleteResultsFromMenu(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        deleteResultsFromDB();
    }
    internal static void deleteResultsFromDB()
    {
        App.model.nodalReactions = [];
        App.model.nodalDisplacements = [];
        App.model.barInternForces = [];
        App.model.setParamValue(Mdta.analysed, 0);
    }
}