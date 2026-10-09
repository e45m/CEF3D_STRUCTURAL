using CEF.matrixImpl;
using CEF.BarrElement;
using CEF.LUSolver;
using CEF.MFC;
using System.Diagnostics;
using System.Runtime;
using Mtrx = CEF.matrixImpl.BandedMatrix;
namespace CEF.LinearAnalysis;
internal class LinearAnal
{
    const int DOFbyNode = 6;
    private double[] Q;  
    private double[] p;  
    private double[] d;  
    internal double[] getp() => p;
    internal double[] getQ() => Q;
    internal double[] getd() => d;
    double[] Qred;  
    double[] pred;  
    double[] dred;  
    private int[] e;  
    private   int Ss;  
    private  BandedMatrix S;
    private  SparseMatrix Sred;
    private  readonly ModelDB m_mDB;
    int currLoadCase = 0;
    SparseMatrix T;
    bool AnalysisDone { get; set; }
    bool ModelConformed { get; set; }
    LUSolver.LUSolver solver;
    internal LinearAnal(  ModelDB mDB)
    {
        m_mDB = mDB;
        AnalysisDone = false;
        ModelConformed = false;
        constructSystem();
    }
    private CalcState setUPKMem()
    {
        int numeroDeNudos = m_mDB.nodes.Count;
        Ss = DOFbyNode * numeroDeNudos;
        p = new double[Ss];
        d = new double[Ss];
        Q = new double[Ss];
        e = new int[Ss];
        int[] profile;
        profile = new int[Ss];
        S = new BandedMatrix(Ss);
        S.makeZero();
        return CalcState.OK;
    }

    internal bool constructSystem()
    {
        CalcState res;
        var res1 = Mfc.createMFCMatrix( App.model, ref T);
        res = setUPKMem();
        res = calcStiffnessMtrx();
        res = createStateVect();
        if (res != CalcState.OK) {   /*Error handling*/  }
        return CalcState.OK == res;
    }
    private CalcState calcStiffnessMtrx()
    {
        foreach (var barra in m_mDB.bars)
            addBarsToSystem(barra);
        return CalcState.OK;
    }
    private void addBarsToSystem(Bar Bar)
    {
        const int elemDim = 12;
        int i, j, kK1, kK2,  row, col;
        int[] index = new int[elemDim];
        double[,] trans = new double[elemDim, elemDim]; 
        double[,] elemStiffness = new double[elemDim, elemDim];
        double[,] elemglobal = new double[elemDim, elemDim];
        var nodes = m_mDB.nodes;
        kK1 = nodes.FindIndexByID(Bar.startNode);
        kK2 = nodes.FindIndexByID(Bar.endNode);
        if (kK1 != -1 && kK2 != -1)        
            for (i = 0; i < 6; i++)
            {   index[i] = kK1 * DOFbyNode + i;
                index[i + 6] = kK2 * DOFbyNode + i;
            }
        barrElement.getTransMatrix( Bar,  trans);
        barrElement.getKedMatrix(Bar,  elemStiffness);
        elemglobal = MatrixMath.M_multT(MatrixMath.M_mult(trans, elemStiffness), trans); 
        for (i = 0; i < elemDim; i++)
            for (j = 0; j <= i; j++)
            {
                int a = index[i];
                int b = index[j];
                if (a >= b)
                {
                    row = a; 
                    col = b; 
                }
                else
                {
                    row = b;
                    col = a;
                }
                S[row,col] += elemglobal[i, j];
                S[col, row] = S[row, col];
            }
    }
    private CalcState createStateVect()
    {
        var nodes = m_mDB.nodes;
        Array.Fill(e, (int)CalcState.None);
        setStateWithModelElements();
        setContournRestrictions();
        setMultiFreedomConstraints();
        return CalcState.OK;
    }
    int getNodeStartIndex(int id)
    {
        int NodeIdx, nodeStart;
        var nudos = m_mDB.nodes;
        NodeIdx = nudos.FindIndexByID(id);
        if (NodeIdx == -1) throw new
                Exception();
        nodeStart = NodeIdx * DOFbyNode;
        return nodeStart;
    }
    void setStateWithModelElements()
    {
        var nodes = m_mDB.nodes;
        int nodeStart;
        foreach (var b in m_mDB.bars)
        {
            var nodeI = m_mDB.nodes.FindIndexByID(b.startNode);
            var nodeJ = m_mDB.nodes.FindIndexByID(b.endNode);
            if (nodeI == -1 || nodeJ == -1) throw new
                    Exception();
            nodeStart = nodeI * DOFbyNode;
            e[nodeStart + 0] = (int)CalcState.Dual;
            e[nodeStart + 1] = (int)CalcState.Dual;
            e[nodeStart + 2] = (int)CalcState.Dual;
            e[nodeStart + 3] = (int)CalcState.Dual;
            e[nodeStart + 4] = (int)CalcState.Dual;
            e[nodeStart + 5] = (int)CalcState.Dual;
            nodeStart = nodeJ * DOFbyNode;
            e[nodeStart + 0] = (int)CalcState.Dual;
            e[nodeStart + 1] = (int)CalcState.Dual;
            e[nodeStart + 2] = (int)CalcState.Dual;
            e[nodeStart + 3] = (int)CalcState.Dual;
            e[nodeStart + 4] = (int)CalcState.Dual;
            e[nodeStart + 5] = (int)CalcState.Dual;
        }
    }
    void setContournRestrictions()
    {
        int nodeStart;
        var NR = m_mDB.nodalRestraints;
        foreach (var support in NR)
        {
            nodeStart = getNodeStartIndex(support.ID);
            if ((support.ux | support.uy | support.uz | support.rx | support.ry | support.rz) == 0) continue;
            e[nodeStart + 0] = support.ux == 1 ? (int)CalcState.Primal : (int)CalcState.Dual;
            e[nodeStart + 1] = support.uy == 1 ? (int)CalcState.Primal : (int)CalcState.Dual;
            e[nodeStart + 2] = support.uz == 1 ? (int)CalcState.Primal : (int)CalcState.Dual;
            e[nodeStart + 3] = support.rx == 1 ? (int)CalcState.Primal : (int)CalcState.Dual;
            e[nodeStart + 4] = support.ry == 1 ? (int)CalcState.Primal : (int)CalcState.Dual;
            e[nodeStart + 5] = support.rz == 1 ? (int)CalcState.Primal : (int)CalcState.Dual;
        }
    }
    void setMultiFreedomConstraints()
    {
        int  nodeStart;
        var nodes = m_mDB.nodes;
        foreach (var rest in m_mDB.nodalConstraints)
        {
            nodeStart = getNodeStartIndex(rest.masterNodeID);
            var gdlsrest = getMFCGDL((MFCType)rest.Type);
            for (int i = 0; i < DOFbyNode; i++)
            {
                int idx = nodeStart + i;
                int v = e[idx];
                bool hasDP = (v & ((int)CalcState.Dual | (int)CalcState.Primal)) != 0;
                if (gdlsrest[i] == 1)
                {
                    e[idx] = hasDP
                        ? v | (int)CalcState.Master
                        : ((int)CalcState.Dual | (int)CalcState.Master);
                }
                else
                {
                    e[idx] = hasDP
                       ? v
                       : (int)CalcState.Inactive;
                }
            }
            var nodeslave = rest.nodesIDList.
                Split(",", StringSplitOptions.RemoveEmptyEntries).
                Select(i => int.Parse(i)).ToList();
            foreach (var slave in nodeslave)
            {
                if (slave == rest.masterNodeID) continue;
                nodeStart = getNodeStartIndex(slave);
                for (int i = 0; i < DOFbyNode; i++)
                {
                    int idx = nodeStart + i;
                    int v = e[idx];
                    bool hasDP = (v & ((int)CalcState.Dual | (int)CalcState.Primal)) != 0;
                    if (gdlsrest[i] == 1)
                    {
                        e[idx] = hasDP
                            ? v | (int)CalcState.Slave
                            : (v | (int)CalcState.Slave);
                    }
                    else
                    {
                        e[idx] = hasDP
                           ? v
                           : (int)CalcState.Inactive;
                    }
                }
            }
        }
    }
    int[] getMFCGDL(MFCType Tipo)
    {
        int[] gdls;
        switch (Tipo)
        {
            case MFCType.diaphragm: 
                gdls = new int[] { 1, 1, 0, 0, 0, 1 };
                break;
            case MFCType.diaphragmXX: 
                gdls = new int[] { 0, 1, 1, 1, 0, 0 };
                break;
            case MFCType.diaphragmYY: 
                gdls = new int[] { 1, 0, 1, 0, 1, 0 };
                break;
            case MFCType.plate: 
                gdls = new int[] { 0, 0, 1, 1, 1, 0 };
                break;
            case MFCType.plateXX: 
                gdls = new int[] { 1, 0, 0, 0, 1, 1 };
                break;
            case MFCType.plateYY: 
                gdls = new int[] { 0, 1, 0, 1, 0, 1 };
                break;
            case MFCType.rodX: 
                gdls = new int[] { 1, 0, 0, 0, 0, 0 };
                break;
            case MFCType.rodY: 
                gdls = new int[] { 0, 1, 0, 0, 0, 0 };
                break;
            case MFCType.rodZ: 
                gdls = new int[] { 0, 0, 1, 0, 0, 0 };
                break;
            case MFCType.beamX: 
                gdls = new int[] { 1, 0, 0, 1, 0, 0 };
                break;
            case MFCType.beamY: 
                gdls = new int[] { 0, 1, 0, 0, 1, 0 };
                break;
            case MFCType.beamZ: 
                gdls = new int[] { 0, 0, 1, 0, 0, 1 };
                break;
            case MFCType.body:
            case MFCType.weld:
            case MFCType.equal:
                gdls = new int[] { 1, 1, 1, 1, 1, 1 };
                break;
            default:
                throw new Exception();
        }
        return gdls;
    }
    private CalcState constructLoadVector()
    {
        const int dimElem = 12;
        double[] loadVector = new double[dimElem];
        var nodes = m_mDB.nodes;
        var nodalLoads = m_mDB.nodalLoads.Where(c=>c.Case== currLoadCase);
        foreach (var load in nodalLoads)
        {
            var nodeIdx = nodes.FindIndexByID(load.NodeID);
            if ((nodeIdx == -1))
                throw new Exception();
            var indexA = nodeIdx * DOFbyNode;
            Q[indexA + 0] += load.Fx;
            Q[indexA + 1] += load.Fy;
            Q[indexA + 2] += load.Fz;
            Q[indexA + 3] += load.Mx;
            Q[indexA + 4] += load.My;
            Q[indexA + 5] += load.Mz;
        }
        var basrs = m_mDB.bars;
        foreach(var Bar in basrs)
        {
            var indxI = nodes.FindIndexByID(Bar.startNode);
            var indxJ = nodes.FindIndexByID(Bar.endNode);
            if (indxI == -1 || indxJ == -1) 
                throw new Exception();
            barrElement.getLoadVector(Bar, ref loadVector,currLoadCase);
            indxI *= DOFbyNode;
            indxJ *= DOFbyNode;
          Q[indxI + 0] += loadVector[0];
          Q[indxI + 1] += loadVector[1];
          Q[indxI + 2] += loadVector[2];
          Q[indxI + 3] += loadVector[3];
          Q[indxI + 4] += loadVector[4];
          Q[indxI + 5] += loadVector[5];
          Q[indxJ + 0] += loadVector[6];
          Q[indxJ + 1] += loadVector[7];
          Q[indxJ + 2] += loadVector[8];
          Q[indxJ + 3] += loadVector[9];
          Q[indxJ + 4] += loadVector[10];
          Q[indxJ + 5] += loadVector[11];
        }
        return CalcState.OK;
    }
    internal void calcBarsInternalForces()
    {
        var nodes = m_mDB.nodes;
        var reactions = m_mDB.nodalReactions;
        var displacements =  m_mDB.nodalDisplacements;
        var idx = 0;
        foreach (var Node in nodes)
        {
            idx = nodes.FindIndexByID(Node.ID); ;
            var displ = new nodalDisplacement();
            displ.ID = Node.ID;
            displ.Case = currLoadCase;
            displ.dx = p[DOFbyNode * idx + 0];
            displ.dy = p[DOFbyNode * idx + 1];
            displ.dz = p[DOFbyNode * idx + 2];
            displ.rx = p[DOFbyNode * idx + 3];
            displ.ry = p[DOFbyNode * idx + 4];
            displ.rz = p[DOFbyNode * idx + 5];
            displacements.Add(displ);
            var reac= new nodalReaction();
            reac.ID = Node.ID;
            reac.Case = currLoadCase;
            reac.Fx = d[DOFbyNode * idx + 0];
            reac.Fy = d[DOFbyNode * idx + 1];
            reac.Fz = d[DOFbyNode * idx + 2];
            reac.Mx = d[DOFbyNode * idx + 3];
            reac.My = d[DOFbyNode * idx + 4];
            reac.Mz = d[DOFbyNode * idx + 5];
            reactions.Add(reac);
        }
        var bars = m_mDB.bars;
        foreach (var Bar in bars)
            barrElement.getInternalForcesVector(Bar, currLoadCase);
    }
    internal /*async Task<bool> */void solveSystem(IProgress<(int, string)> progress =null)
      {
        if (App.model.nodalConstraints.Count is 0)       {
            solver = new LUSolver.LUSolver(S, e, 1e-11);
            solver.factorizeLU();
        }
        else
        {
            Sred = Mtrx.TtxSxT(S,T);
            solver = new LUSolver.LUSolver(Sred.ToBanded(), e, 1e-11);
            solver.factorizeLU();
        }
     }
    internal bool solveLoadCase(int caso = 0)
    {
        currLoadCase = caso;
        Array.Fill(d, 0);
        Array.Fill(p, 0);
        Array.Fill(Q, 0);
        barrElement.setUpLoadIndexes();
        constructLoadVector();
        if (App.model.nodalConstraints.Count is 0)
        {
            solver.solveCase(Q,  p);           
            solver.solveDual(d);
        }
        else
        {
            pred = new double[Ss];
            dred = new double[Ss];
            Qred = Mtrx.MT_mult(T, Q);
            solver.solveCase(Qred,  pred);
            solver.solveDual(dred);
           p = Mtrx.M_mult(T, pred); 
           d = Mtrx.M_mult(T, dred); 
        }
        return true;
    }
    internal BandedMatrix getSystemMatrix()
    {
        return S;
    }
    internal SparseMatrix getT_MFC_Matrix()
    {
        return T;
    }
}