using System.Numerics;
namespace CEF.FakebandedMatrix;
internal partial class BandedMatrix<T> where T : INumber<T>
{
    private const int MAX_ELEMENTS = 250_000_000;
    private int Rows;
    private int Columns;
    private T[,] M;
    internal bool AllowOutOfBindAccess { get; set; } = true;
    public int getRows() => Rows;
    public int getCols() => Columns;
    internal int getSs() => Rows;
    internal BandedMatrix(int SystemSize, bool symetric = false)
        : this(SystemSize, SystemSize, symetric) { }
    internal BandedMatrix(int rows, int cols, bool symetric = false)
    {
        long elems = (long)rows * cols;
        if (elems > MAX_ELEMENTS)
            throw new Exception($"Matrix too large: {elems}");
        Rows = rows;
        Columns = cols;
        M = new T[rows, cols];
    }
    internal BandedMatrix(T[,] inputMtrx)
    {
        long elems = (long)inputMtrx.GetLength(0) * inputMtrx.GetLength(1);
        if (elems > MAX_ELEMENTS)
            throw new Exception($"Matrix too large: {elems}");
        Rows = inputMtrx.GetLength(0);
        Columns = inputMtrx.GetLength(1);
        M = inputMtrx;
    }
    internal int getRStart(int i) => 0;
    internal int getREnd(int i) => Columns - 1;
    internal int getCStart(int j) => 0;
    internal int getCEnd(int j) => Rows - 1;
    internal void setVertProfile() { }
    internal void setProfile() { }
    internal void addRow(int indx, ref T[] row, int start)
    {
        for (int j = 0; j < row.Length; j++)
            M[indx, start + j] = row[j];
    }
    internal void addRow(int indx, int rowlen)
    {
    }
    internal void addRow(int indx, int rowlen, int profile)
    {
    }
    public T this[int n, int m]
    {
        get
        {
            if (n >= Rows || m >= Columns)
                throw new Exception($"Index out of range ({Rows},{Columns})");
            return M[n, m];
        }
        set
        {
            if (n >= Rows || m >= Columns)
                throw new Exception($"Index out of range ({Rows},{Columns})");
                M[n, m] = value;
        }
    }
    internal void makeI()
    {
        for (int i = 0; i < Rows; i++)
            M[i, i] = T.One;
    }
    internal void makeZero()
    {
        Array.Clear(M);
    }
    internal T[][] getS()
    {
        var S = new T[Rows][];
        for (int i = 0; i < Rows; i++)
        {
            S[i] = new T[Columns];
            for (int j = 0; j < Columns; j++)
                S[i][j] = M[i, j];
        }
        return S;
    }
    public T[,] getMatrix() => M;
    public static implicit operator T[,](in BandedMatrix<T> A)
        => A.M;
}
internal partial class BandedMatrix<T> where T : INumber<T>/* IEnumerable<T> where T : class*/
{
    public static BandedMatrix<T> operator *(in BandedMatrix<T> A, in BandedMatrix<T> B)
    {
        return M_mult(A, B);
    }
    public static BandedMatrix<T> M_mult(in BandedMatrix<T> A, in BandedMatrix<T> B, bool ResAsSymetric = false)
    {
        var rowsA = A.getRows();
        var rowsB = B.getRows();
        var colsA = A.getCols();
        var colsB = B.getCols();
        var Rrows = A.getCols();
        var Rcols = B.getRows();
        A.setVertProfile();
        B.setVertProfile();
        var R = new BandedMatrix<T>(Rrows, Rcols, ResAsSymetric);
        R.AllowOutOfBindAccess = true;
        for (var i = 0; i < Rrows; i++)
            R.addRow(i, 1, i);
        bool A_All = A.AllowOutOfBindAccess;
        bool B_All = B.AllowOutOfBindAccess;
        A.AllowOutOfBindAccess = true;
        B.AllowOutOfBindAccess = true;
        for (var i = 0; i < rowsA; i++)
        {
            for (var j = 0; j < colsB; j++)
            {
                T sum = T.Zero;
                var kstart = Math.Min(A.getRStart(i), B.getCStart(j));
                var kend = Math.Min(A.getREnd(i), B.getCEnd(j));
                for (var k = kstart; k <= kend; k++)
                {
                    sum += A[i, k] * B[k, j];
                }
                if (sum != T.Zero)
                    R[i, j] = sum;
            }
        }
        R.setProfile();
        R.AllowOutOfBindAccess = true;
        A.AllowOutOfBindAccess = A_All;
        B.AllowOutOfBindAccess = B_All;
        return R;
    }
    public static BandedMatrix<T> MT_mult(in BandedMatrix<T> A, in BandedMatrix<T> B, bool ResAsSymetric = false)
    {
        var rowsA = A.getRows();
        var rowsB = B.getRows();
        var colsA = A.getCols();
        var colsB = B.getCols();
        var Rrows = A.getRows();
        var Rcols = B.getRows();
        A.setVertProfile();
        B.setVertProfile();
        var R = new BandedMatrix<T>(Rrows, Rcols, ResAsSymetric);
        R.AllowOutOfBindAccess = true;
        for (var i = 0; i < Rrows; i++)
            R.addRow(i, 1, i);
        bool A_All = A.AllowOutOfBindAccess;
        bool B_All = B.AllowOutOfBindAccess;
        A.AllowOutOfBindAccess = true;
        B.AllowOutOfBindAccess = true;
        for (var i = 0; i < colsA; i++)
        {
            for (var j = 0; j < colsB; j++)
            {
                T sum = T.Zero;
                var kstart = Math.Min(A.getCStart(i), B.getCStart(j));
                var kend = Math.Min(A.getCEnd(i), B.getCEnd(j));
                for (var k = kstart; k <= kend; k++)
                {
                    sum += A[k, i] * B[k, j];
                }
                if (sum != T.Zero)
                    R[i, j] = sum;
            }
        }
        R.setProfile();
        R.AllowOutOfBindAccess = true;
        A.AllowOutOfBindAccess = A_All;
        B.AllowOutOfBindAccess = B_All;
        return R;
    }
    public static BandedMatrix<T> M_multT(in BandedMatrix<T> A, in BandedMatrix<T> B, bool ResAsSymetric = false)
    {
        var rowsA = A.getRows();
        var rowsB = B.getRows();
        var colsA = A.getCols();
        var colsB = B.getCols();
        var Rrows = A.getCols();
        var Rcols = B.getCols();
        A.setVertProfile();
        B.setVertProfile();
        var R = new BandedMatrix<T>(Rrows, Rcols, ResAsSymetric);
        R.AllowOutOfBindAccess = true;
        for (var i = 0; i < Rrows; i++)
            R.addRow(i, 1, i);
        bool A_All = A.AllowOutOfBindAccess;
        bool B_All = B.AllowOutOfBindAccess;
        A.AllowOutOfBindAccess = true;
        B.AllowOutOfBindAccess = true;
        for (var i = 0; i < rowsA; i++)
        {
            for (var j = 0; j < rowsB; j++)
            {
                T sum = T.Zero;
                var kstart = Math.Min(A.getRStart(i), B.getRStart(j));
                var kend = Math.Min(A.getREnd(i), B.getREnd(j));
                for (var k = kstart; k <= kend; k++)
                {
                    sum += A[i, k] * B[j, k];
                }
                if (sum != T.Zero)
                    R[i, j] = sum;
            }
        }
        R.setProfile();
        R.AllowOutOfBindAccess = true;
        A.AllowOutOfBindAccess = A_All;
        B.AllowOutOfBindAccess = B_All;
        return R;
    }
    public static T[] M_mult(in BandedMatrix<T> A, in T[] B)
    {
        var rowsA = A.getRows();
        A.setVertProfile();
        var R = new T[rowsA];
        bool old = A.AllowOutOfBindAccess;
        A.AllowOutOfBindAccess = true;
        for (int i = 0; i < rowsA; i++)
        {
            T sum = T.Zero;
            int kstart = A.getRStart(i);
            int kend = A.getREnd(i);
            for (int k = kstart; k <= kend; k++)
            {
                sum += A[i, k] * B[k];
            }
            R[i] = sum;
        }
        A.AllowOutOfBindAccess = old;
        return R;
    }
    public static T[] MT_mult(in BandedMatrix<T> A, in T[] B)
    {
        var cols = A.getCols();
        var rows = A.getRows();
        var R = new T[rows];
        bool old = A.AllowOutOfBindAccess;
        A.AllowOutOfBindAccess = true;
        A.setProfile();
        A.setVertProfile();
        for (int col = 0; col < cols; col++)
        {
            var kstart = A.getRStart(col);
            var kend = A.getREnd(col);
            for (int row = kstart; row <= kend; row++)
            {
                R[row] += A[col, row] * B[col];  
            }
        }
        A.AllowOutOfBindAccess = old;
        return R;
    }
    internal void __debug_printMatrixInfo(string file)
    {
        var M = this;
        using (var sw = new StreamWriter(file))
        {
            sw.Write($"Size: {M.getSs()}, Rows: {M.getRows()}, Cols: {M.getCols()}\n");
            sw.Write("Row;Start;End; Content\n");
            for (int i = 0; i < M.getRows(); i++)
            {
                sw.Write(i); sw.Write(";");
                sw.Write(M.getRStart(i)); sw.Write(";");
                sw.Write(M.getREnd(i)); sw.Write(";");
                for (int j = 0; j < M.getCols(); j++)
                {
                    sw.Write(M[i, j]); sw.Write(';');
                }
                sw.WriteLine();
            }
        }
    }
}