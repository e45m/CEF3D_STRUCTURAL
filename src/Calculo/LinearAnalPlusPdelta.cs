using CEF.matrixImpl;
using CEF.BarrElement;
using CEF.LinearAnalysis;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
namespace CEF.LinearAnalysisPlusPdelta;
internal class LinearAnalPlusPdelta : LinearAnal
{
    ModelDB m_mDB;
    SparseMatrix Kg;
    BandedMatrix Ko;
    internal LinearAnalPlusPdelta(ModelDB mDB)
                    : base(mDB)
    {
        m_mDB = mDB;
        Ko = base.getSystemMatrix();
        Kg = new SparseMatrix(Ko.getRows());
    }
    internal void solveForKG(int[] loadCases, double[] factors, IProgress<(int, string)> progress = null)
    {
        Kg.makeZero(); 
        solveSystem(progress);
        Debug.Assert(loadCases.Length == factors.Length);
        for (int i = 0; i < loadCases.Length; i++)
        {
            solveLoadCase(loadCases[i]);
            getKgUpdate(loadCases[i], factors[i]);
        }
        var keys = Kg.Keys;
        keys.Sort();
        foreach (long k in keys)
        {
            var i = SparseMatrix.getRowFromKey(k);
            var j = SparseMatrix.getColFromKey(k);
            Ko.incrementValue(i, j, Kg.getValue(i, j));
        }
    }
    double[,] KgEl = new double[12, 12]; 
    double[,] KgElg;
    internal void getKgUpdate(int Case, double F) 
    {
        var nodos = m_mDB.nodes;
        var reacciones = m_mDB.nodalReactions;
        var desplazamientos = m_mDB.nodalDisplacements;
        int[] index = new int[12];
        int row;
        int col;
        var p = base.getp();
        var d = base.getd();
        var bars = m_mDB.bars;
        var vForces = new double[12];
        foreach (var Bar in bars)
        {
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                   { 
                    KgEl[i, j] = 0d;
                }
            barrElement.getInternalPdForcesVector(Bar, Case, p,
                                                    out double Ax1, out double Ax2, out double L,
                                                    out int idxI, out int idxJ
                                                    );
            Ax1 *= F;
            Ax2 *= F;
            var sectID = Bar.SectionID;
            if (sectID == -1) throw new Exception();
            var secIndx = m_mDB.Sections.FindIndexByID(sectID);
            if (secIndx == -1) throw new Exception();
            var sec = m_mDB.Sections[secIndx];
            var A = sec.Ag;
            var Iy = sec.Iyy;
            var Iz = sec.Izz;
            var P = -(-Ax1+Ax2)/2;
            double a1 = 1.2 * P / L;         
            double a2 = P / 10.0;            
            double a3 = 2.0 * P * L / 15.0;  
            double a4 = -P * L / 30.0;       
            double a5 = P * (Iy + Iz) / (A * L); 
            KgEl[1, 1] = a1;
            KgEl[1, 5] = a2;
            KgEl[1, 7] = -a1;
            KgEl[1, 11] = a2;
            KgEl[2, 2] = a1;
            KgEl[2, 4] = -a2;
            KgEl[2, 8] = -a1;
            KgEl[2, 10] = -a2;
            KgEl[3, 3] = a5;
            KgEl[3, 9] = -a5;
            KgEl[4, 2] = -a2;
            KgEl[4, 4] = a3;
            KgEl[4, 8] = a2;
            KgEl[4, 10] = a4;
            KgEl[5, 1] = a2;
            KgEl[5, 5] = a3;
            KgEl[5, 7] = -a2;
            KgEl[5, 11] = a4;
            KgEl[7, 1] = -a1;
            KgEl[7, 5] = -a2;
            KgEl[7, 7] = a1;
            KgEl[7, 11] = -a2;
            KgEl[8, 2] = -a1;
            KgEl[8, 4] = a2;
            KgEl[8, 8] = a1;
            KgEl[8, 10] = a2;
            KgEl[9, 3] = -a5;
            KgEl[9, 9] = a5;
            KgEl[10, 2] = -a2;
            KgEl[10, 4] = a4;
            KgEl[10, 8] = a2;
            KgEl[10, 10] = a3;
            KgEl[11, 1] = a2;
            KgEl[11, 5] = a4;
            KgEl[11, 7] = -a2;
            KgEl[11, 11] = a3;
            double[,] trans = new double[12,12]; 
            barrElement.getTransMatrix(Bar, trans);
            KgElg = MatrixMath.M_multT(MatrixMath.M_mult(trans, KgEl), trans);
            for (int i = 0; i < 6; i++)
            {
                index[i] = idxI + i; 
                index[i + 6] = idxJ + i;
            }
            for (int i = 0; i < 12; i++)
                for (int j = 0; j <= i; j++)
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
                    double v = KgElg[i, j];
                    Kg.IncrementValue(row, col, v);
                    Kg.setValue(col, row, Kg.getValue(row,col));
                }
        }
    }
}