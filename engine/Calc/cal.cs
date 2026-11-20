using CEF.Dictionary;
using CEF.LinearAnalysis; 
using CEF.LinearAnalysisPlusPdelta;

using System.Diagnostics;
namespace CEF;
public class Calculo
{
    private ModelDB m_mDB;
    private AppDB m_AppData;
    private LinearAnal LinearModel;
    private LinearAnalPlusPdelta pdeltaModel;
   
    public static void CalcBarInternalActions(ModelDB model, AppDB appData) 
{

        if(model.barInternForces==null) return;
        if(appData.barStatiosForces==null) appData.barStatiosForces = [];
        
        PostPorcess.calculeBarInternalForcesStations(model,appData);
}

    public  void Calcular(ModelDB mDB, AppDB mAD,
    bool PdOn = false
) 
    {
       
        m_AppData = mAD;
        m_mDB = mDB;

        var mA = Engine.getModel().getParamValue(paramDefaults.AreaAutoMeh)==1;
        var mB = Engine.getModel().getParamValue(paramDefaults.BarAutoMesh)==1;
        if(mA||mB)  Mesh.setModel(m_mDB);
        if(mB)      Mesh.MeshFrames();
        if(mA)      Mesh.MeshAreas();
        
        m_mDB.SortNodesbyCoordinates();
        ; 
        if (!PdOn)
        {
           OnlyLinearAnalysis();
        }
        else {
            LinearAnalysisPlusPd();
        }
        PostPorcess.SetSuperpositionCombosResults(m_mDB);
        PostPorcess.calculeBarInternalForcesStations( m_mDB, m_AppData);
    }
    public void OnlyLinearAnalysis() {
     
        LinearModel = new LinearAnal(m_mDB);
        LinearModel.solveSystem();
        for (int i = 0; i<m_mDB.loadCases.Count;i++)
        {
            LinearModel.solveLoadCase(m_mDB.loadCases[i].ID);
            LinearModel.calcInternalForces();
        }
       
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
    public void LinearAnalysisPlusPd()
    {
        
        pdeltaModel = new LinearAnalPlusPdelta( m_mDB);
        if(pdeltaIters>1){
            pdeltaIters=1;
            MessageBox.Show("Iterative Pdelta not yet implemented, Analisys continues with 1 iteration");
        }

        for (int i =0;i< pdeltaIters; i++){
            pdeltaModel.solveForKG(pdLoadCases, pdLoadCasesFactor);
        }
           pdeltaModel.solveSystem();
        for (int i = 0; i < m_mDB.loadCases.Count; i++)
        {
             pdeltaModel.solveLoadCase(m_mDB.loadCases[i].ID);
             pdeltaModel.calcInternalForces();
        }       
    }   
         
 }
    


