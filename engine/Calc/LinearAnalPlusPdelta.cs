using CEF.matrixImpl;
using CEF.BarrElement;
using CEF.LinearAnalysis;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.Xml;

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
    internal void solveForKG(int[] loadCases, double[] factors)
    {
        solveSystem();
        Kg.makeZero(); 
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
    double[,] strans = new double [12,12];
    double[,] KgElg;
    double[,] trans = new double[12,12]; 
    internal void getKgUpdate(int Case, double F) 
    {
        // var nodos = m_mDB.nodes;
        // var reacciones = m_mDB.nodalReactions;
        // var desplazamientos = m_mDB.nodalDisplacements;
        int[] index = new int[12];
        int row;
        int col;
        var p = base.getp();
        //var d = base.getd();
        var bars = m_mDB.bars;
        var vForces = new double[12];
        foreach (var Bar in bars)
        {

            Array.Clear(KgEl,0,KgEl.Length);
            barrElement.
            getInternalPdForcesVector(
                Bar, Case, p, 
                 out double Ax1, out double Ax2, out double L,
                  out int idxI, out int idxJ ,m_mDB);
            Ax1 *= F;
            Ax2 *= F;
            var sectID = Bar.SectionID;
            if (sectID == -1) throw new Exception();
            var sec = m_mDB.sections.FindByID(sectID);
            if (sec == null) throw new Exception();
            
            var A = sec.Ag;
            var Iy = sec.Iyy;
            var Iz = sec.Izz;
            var sign = -1; //Ax1>=0
            if(Ax1<0) sign = 1; 
            /// --->##################<--- sign = -1
            /// <---##################---> sign = +1

            var P = sign*(Math.Abs(Ax1)+Math.Abs(Ax2))/2;
                
            double a1 = 1.2 * P / L;//36
            double a2 = P / 10.0;//3L
            double a4 = P * L / 30.0;//L**2
            double a3 = 4*a4;//4*L**2

         

            //TODO: Pasar la matriz

            double a5 = P * (Iy + Iz) / (A * L);
            // Traslación V - rotación Z

            KgEl[1, 1]  = a1;
            KgEl[5, 1]  = a2;
            KgEl[5, 5]  = a3;
            KgEl[7, 1]  = -a1;
            KgEl[7, 5]  = -a2;
            KgEl[7, 7]  = a1;
            KgEl[11,1]  = a2;
            KgEl[11,5]  = -a4; 
            KgEl[11,7]  = -a2;
            KgEl[11, 11] = a3;
            
            KgEl[2, 2]   = a1;
            KgEl[4, 2]   = a2;//-
            KgEl[4, 4]   = -a3;//xz
            KgEl[8, 2]   = -a1;
            KgEl[8, 4]   = -a2;//+
            KgEl[8, 8]   = a1;
            KgEl[10,2]   = -a2;
            KgEl[10,4]   = -a4;
            KgEl[10,8]   = a2;
            KgEl[10, 10] = -a3;//xz

            KgEl[3, 3] = a5;
            KgEl[9, 3] = -a5;
            KgEl[9, 9] = a5;

            // Rigidez geométrica torsional/axial adicional

            // Simetría
            for (int i = 0; i < 12; i++)
                for (int j = i + 1; j < 12; j++)
                    KgEl[i, j] = KgEl[j, i];

      //Rotation about local X
       
      if (Bar.Rotation != 0.0)
        {
            var ang = Bar.Rotation * Math.PI / 180;
            strans[0, 0] = 1.0;
            strans[0, 1] = 0.0;
            strans[0, 2] = 0.0;
            strans[1, 0] = 0.0;
            strans[1, 1] = Math.Cos(ang);
            strans[1, 2] = Math.Sin(ang);
            strans[2, 0] = 0.0;
            strans[2, 1] = -Math.Sin(ang);
            strans[2, 2] = Math.Cos(ang);
            Array.Clear(trans, 0, trans.Length);
            for (int b = 0; b < 4; b++)
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        trans[3 * b + i, 3 * b + j] = strans[i, j];
               // (TtxS)xT
            Array.Copy(MatrixMath.M_multT( 
                MatrixMath.M_mult( trans,KgEl),trans), KgEl ,144);
        }


    
            Array.Clear(trans, 0, trans.Length);           
            barrElement.getTransMatrix(Bar, trans,m_mDB);
            KgElg = MatrixMath.M_multT(MatrixMath.M_mult(trans, KgEl), trans);//TSTt

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