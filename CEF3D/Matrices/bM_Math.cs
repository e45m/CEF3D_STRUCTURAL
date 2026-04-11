using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace CEF.matrixImpl;
using T = double;

internal partial class BandedMatrix
{

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SparseMatrix TtxSxT(in BandedMatrix S, in SparseMatrix T)
    {             
         return multABoutSym(multAtBSym(T, S), T);     
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] 
    public static SparseMatrix multAtBSym(in SparseMatrix A, in BandedMatrix B)
    {
        var R = new SparseMatrix(A.getCols(), B.Columns);
        var Bpstart = B.getpStart();
        var Bpvstart = B.getpVStart();
        var ks = A.Keys;
        int ksCount = ks.Count;
        ks.Sort();
        var noZero = getNonZerosPos(B);
        for (int key_indx = 0; key_indx < ksCount; key_indx++)
        {
            int k = (int)(ks[key_indx] >> 24);
            int i = (int)(ks[key_indx] & ((1L << 24) - 1));
            double aki = A.getValue(k, i);
            int jstart = Bpstart[k];
            int jend = B.Rows - Bpvstart[k] - 1;
            var nz = noZero[k];
            if (ks == null) continue;
            if (k > jend) break;
            foreach(int j in nz)
            {
                double bkj = B.getValue(k, j);
                if (bkj == 0.0d) continue;
                double current = R.getValue(i, j);
                R.setValue(i, j, current + aki * bkj);
            }
        }
        return R;    
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SparseMatrix multABoutSym(in SparseMatrix A, in SparseMatrix B)
    {
        var R = new SparseMatrix(A.getRows(), B.getCols());
        var bRows = new  List<int>[B.getRows()];        
        var bkeys = B.Keys.ToList();
        bkeys.Sort();
        int curRow = -1;
        foreach (long key in bkeys)
        {
            int row = (int)(key >> 24);
            int col = (int)(key & ((1L << 24) - 1));
            if (row != curRow)
            {
                bRows[row] = new List<int>();
                curRow = row;
             }
            bRows[row].Add(col);           
        }
        var aRows = new List<int>[A.getRows()];
        var akeys = A.Keys.ToList();
        akeys.Sort();
        curRow = -1;
        foreach (long key in akeys)
        {
            int row = (int)(key >> 24);
            int col = (int)(key & ((1L << 24) - 1));
            if (row != curRow)
            {
                aRows[row] = new List<int>();
                curRow = row;
            }
            aRows[row].Add(col);
        }
        for (int i = 0; i < A.getRows(); i++)
        {
            var ks = aRows[i];
            if (ks == null) continue;
            foreach (int k in ks)
            {
                var brow = bRows[k];
                if (brow == null || brow.Count == 0) continue;
                double aik = A.getValue(i, k);
                if (aik == 0.0) continue;
                foreach (int j in brow)
                {
                    if (i < j) continue; 
                    R.setValue(i, j,
                        R.getValue(i, j) + aik * B.getValue(k, j));
                }
            }
        }
        return R;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static List<int>[] getNonZerosPos(in BandedMatrix A)
    {
        var rows = A.getRows();
        var raw = A.rawMtrx;
        var sts = A.getpStart();
        var nz = new List<int>[rows];
            Parallel.For(0, rows, i =>
            {
                var row = raw[i];
                var st = sts[i];
                if (row == null) return;
                nz[i] = [];
                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j] == 0) continue;
                    nz[i].Add(st + j);
                }
            });
            return nz;
    }
    public static T[] M_mult(in BandedMatrix A, in T[] B)
    {
        var rowsA = A.Rows;
        A.setProfile();
        var R = new T[rowsA];
        for (int i = 0; i < rowsA; i++)
        {
            T sum = 0.0d;
            int kstart = A.getRStart(i);
            int kend = A.getREnd(i);
            for (int k = kstart; k <= kend; k++)
            {
                sum += A[i, k] * B[k];
            }
            R[i] = sum;
        }
        return R;
    }
    public static T[] MT_mult(in BandedMatrix A, in T[] B)
    {
        var cols = A.Columns;
        var rows = A.Rows;
        var R = new T[cols];
        A.setProfile();
        for (int col = 0; col < cols; col++)
        {
            var kstart = A.getRStart(col);
            var kend = A.getREnd(col);
            for (int row = kstart; row <= kend; row++)
            {
                R[row] += A[col, row] * B[col];  
            }
        }
        return R;
    }
    public static T[] M_mult(in SparseMatrix A, in T[] B)
    {
        var rowsA = A.getRows();
        var R = new T[rowsA];
        foreach (var key in A.Keys)
        {
            int i = SparseMatrix.getRowFromKey(key);
            int j = SparseMatrix.getColFromKey(key);
            T valA = A.getValue(i,j);
            R[i] += valA * B[j];
        }
        return R;
    }
    public static T[] MT_mult(in SparseMatrix A, in T[] B)
    {
        var colsA = A.getCols();
        var R = new T[colsA];
        foreach (var key in A.Keys)
        {
            int i = SparseMatrix.getRowFromKey(key);
            int j = SparseMatrix.getColFromKey(key);
            T valA = A.getValue(i,j);
            R[j] += valA * B[i];
        }
        return R;
    }
    internal void makeI()
    {
        var I = this;
        var rows = this.Rows;
        if (rawMtrx == null)
            rawMtrx = new T[rows][];
        for (int j = 0; j < rows; j++)
        {
            rawMtrx[j] = [1.0d];
            p[j].start = j;
            p[j].end =  j;
        }
    }
    internal void makeZero()
    {
        var I = this;
        var rows = this.Rows;
        if (rawMtrx == null)
            rawMtrx = new T[rows][];
        for (int j = 0; j < rows; j++)
        {
            rawMtrx[j] = [0.0d];
            p[j].start = j;
            p[j].end = j;
        }
    }
    bool HasLinearDependence(BandedMatrix T, double eps = 1e-10)
    {
        return true;
    }
}