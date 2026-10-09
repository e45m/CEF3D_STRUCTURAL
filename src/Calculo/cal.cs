using CEF.LinearAnalysis; 
using CEF.LinearAnalysisPlusPdelta;
using System.Diagnostics;
namespace CEF;
internal class Calculo
{
    private ModelDB m_mDB;
    private LinearAnal LinearModel;
    private LinearAnalPlusPdelta pdeltaModel;
   
   public async  void Calcular(ModelDB mDB, 
        bool PdOn = false, 
    IProgress<(int, string)> progress =null) 
    {
       var sw = Stopwatch.StartNew();
       m_mDB = mDB;
       m_mDB.SortNodesbyCoordinates();
        ; 
        if (!PdOn)
        {
           await Task.Run(() => OnlyLinearAnalysis(progress));
            sw.Stop();
        }
        else {
            LinearAnalysisPlusPd(progress);
        }
    }
    public void OnlyLinearAnalysis(IProgress<(int, string)> progress) {
        App.stopUndoRecording();
        LinearModel = new LinearAnal(m_mDB);
        LinearModel.solveSystem(progress);
        for (int i = 0; i<App.model.loadCases.Count;i++)
        {
            LinearModel.solveLoadCase(i);
            LinearModel.calcBarsInternalForces();
        }
        App.startUndoRecording();
    }
    int pdeltaIters = 1;
    int [] pdLoadCases = null;
    double [] pdLoadCasesFactor = null;
    public void setPdParams(in int iters,in int[] Cases, in double[] Factors)
    {
        Debug.Assert(Cases.Length==Factors.Length);
        pdeltaIters = iters;
        pdLoadCases = Cases;
        pdLoadCasesFactor = Factors;
    }
    public void LinearAnalysisPlusPd(IProgress<(int, string)> progress)
    {
        App.stopUndoRecording();
        pdeltaModel = new LinearAnalPlusPdelta( m_mDB);
        for (int i =0;i< pdeltaIters; i++){
            pdeltaModel.solveForKG(pdLoadCases, pdLoadCasesFactor, progress);
        }
        pdeltaModel.solveSystem(progress);
        for (int i = 0; i < App.model.loadCases.Count; i++)
        {
             pdeltaModel.solveLoadCase(i);
             pdeltaModel.calcBarsInternalForces();
        }
        App.startUndoRecording();
    }
}