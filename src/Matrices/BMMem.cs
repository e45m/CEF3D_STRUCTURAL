namespace CEF.matrixImpl;
using System.Runtime.CompilerServices;
using T = double;
internal partial class BandedMatrix
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal BandedMatrix Clone()
    {
        var c = new BandedMatrix(Rows, Columns);
        if (Rows < 10000)
        {
            for (int i = 0; i < rawMtrx.Length; i++)
            {
                c.p[i] = p[i];
                c.vp[i] = vp[i];
                var src = rawMtrx[i];
                var dst = new T[src.Length];
                for (int j = 0; j < src.Length; j++){
                    var v = src[j];
                    if(v!=0)
                    dst[j] = v;
                }
                c.rawMtrx[i] = dst;
                dst = null;
            }
        }
        else
        {
            Parallel.For(0, rawMtrx.Length, i =>
            {
                c.p[i] = p[i];
                c.vp[i] = vp[i];
                var src = rawMtrx[i];
                var dst = new T[src.Length];
                for (int j = 0; j < src.Length; j++)
                {
                    var v = src[j];
                    if (v != 0)
                        dst[j] = v;
                }
                c.rawMtrx[i] = dst;
                dst = null;
            });
        }
        return c;
    }
    internal static bool Compare(BandedMatrix A, BandedMatrix  B)
    {
        var eq = true;
        eq = A.RowsCount() == B.RowsCount();
        if (!eq) return eq;
        var rc = A.RowsCount();
        var rawA = A.rawMtrx;
        var rawB = B.rawMtrx;
        for (var i = 0; i < rc; i++)
        {
            var l = rawA[i].Length;
            eq &= l == rawB[i].Length;
            if (!eq) return eq;
            for (var j = 0; j < l; j++) {
                eq &= rawA[i][j] == rawB[i][j];
                if (!eq) return eq;
            }
        }
        return eq;
    }
    internal int getPhysicalIndex(int row, int col)
    {
        return col - p[row].start;
    }
    internal void ensureCapacity(in int row, in int col)
    {
        if (col > p[row].end)
        {
            var currentCap = rawMtrx[row].Length;
            var shift = col - p[row].end;
            var newLen = currentCap + shift;
            var tmp = new T[newLen];
            Array.Copy(rawMtrx[row], 0, tmp, 0, currentCap);
            rawMtrx[row] = null;
            rawMtrx[row] = tmp;
            p[row].end = col;
        }
        if (col < p[row].start)
        {
            var currentCap = rawMtrx[row].Length;
            var shift = p[row].start - col;
            var newLen = currentCap + shift;
            var tmp = new T[newLen];
            Array.Copy(rawMtrx[row], 0, tmp, shift, currentCap);
            rawMtrx[row] = null;
            rawMtrx[row] = tmp;
            p[row].start = col;
        }
    }
    internal void compactMem(in int row, in int newStart, in int newEnd)
    { 
        var newLen = newEnd - newStart + 1;
        var tmp = new T[newLen];
        var shift = newStart - p[row].start;
        Array.Copy(rawMtrx[row], shift, tmp, 0, newLen);
        rawMtrx[row] = null;
        rawMtrx[row] = tmp;
        p[row].start = newStart;
        p[row].end = newEnd;
    }
    internal void setVertProfile()
    {
        for (var i = 0; i < Columns; i++)
        {
            bool flag = false;
            for (var j = 0; j < Rows; j++)
            {
                if ((p[j].start <= i && p[j].end >= i) && !flag) 
                {
                    vp[i].start = j;
                    flag = true;
                }
                if (p[j].start <= i && p[j].end >= i)
                {
                    vp[i].end = j; 
                }
            }
        }
    }
    internal void setProfile()
    {
        for (var i = 0; i < Rows; i++)
        {
            var startShit = 0;
            var endShift = 0;
            var len = rawMtrx[i].Length;
            for (var j = 0; j < len; j++)
                if (rawMtrx[i][j] != 0d)
                {
                    startShit = j;
                    break;
                } else if (j == len - 1)
                { startShit = j; }
            for (var j = len - 1; j >= 0; j--)
                if (rawMtrx[i][j] != 0d)
                {
                    endShift = j;
                    break;
                } else if (j == 0)
                { endShift = j; }
            if (((p[i].end - p[i].start) - (endShift - startShit)) > compactThreshold)
            {
                var newEnd = p[i].start + endShift;
                var newStart = p[i].start + startShit;
                if (newStart > newEnd)
                    newEnd = newStart;
                compactMem(i, newStart, newEnd);
            }
        }
        setVertProfile();
    }
    public struct CSRstruct
    {
        public int[] rowStart;
        public int[] colIndex;
        public int[] rowShift; 
        public Memory<T> data;
        public int rows;
    }
    internal CSRstruct toCRS()
    {
        var mtrix = new CSRstruct();
        mtrix.rowStart = new int[Rows+1];
        mtrix.colIndex = new int[Rows];
        mtrix.rowShift = new int[Rows];
        mtrix.rows = Rows;
        int len = 0;
        for (int i = 0; i < Rows; i++)
        {
            var leni = rawMtrx[i].Length;
            mtrix.rowStart[i] = len;
            mtrix.rowShift[i] = p[i].start;
            len += leni;
        }
        mtrix.rowStart[Rows] = len;
        mtrix.data = new T[len];
        Span<T> dataSpan = mtrix.data.Span;
        for (int i = 0; i < Rows; i++)
        {
            var l = mtrix.rowStart[i+1]-mtrix.rowStart[i];
            var st = mtrix.rowStart[i];
            var sh = mtrix.rowStart[i];
            for (int j = 0; j < l; j++){
                var value = rawMtrx[i][j];
                if (value == 0d) continue;
                dataSpan[st + j] = rawMtrx[i][j];
                mtrix.colIndex[st + j] = j+sh;
            }
        }
        return mtrix;
    }
    internal static BandedMatrix fromCRS(CSRstruct A)
    {
        var maxColRow = A.rows;
        var rowsCount = A.rows;
        var c = new BandedMatrix(rowsCount, rowsCount);
        c.rawMtrx = new T[rowsCount][];
        for (int i = 0; i < rowsCount; i++)
        {
            var rs = A.rowStart[i];
            var sh = A.rowShift[i];
            var cut = rs;
            var l = A.rowStart[i+1]- A.rowStart[i];
            var sp = A.data.Span;
            c.p[i].start = sh;
            c.p[i].end = sh + l - 1;
            c.rawMtrx[i] = new T[l];
            for (int j = 0; j < l; j++)
            {
                c.rawMtrx[i][j] = sp[cut + j];
            }
        }
        return c;
    }
    private bool _disposed = false;
    internal void freeMemory()
    {
        if (rawMtrx != null)
        {
            var rml = rawMtrx.Length;
            for (int i = 0; i < rml; i++)
                rawMtrx[i] = null;
            rawMtrx = null;
        }
        p = null;
        vp = null;
        Rows = -1;
        Columns = -1;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                freeMemory();
            }
            _disposed = true;
        }
    }
    internal SparseMatrix ToSparse()
    {
        var S = new SparseMatrix(Rows, Columns);
        for (int i = 0; i < Rows; i++)
        {
            int j0 =p[i].start;              
            var row = rawMtrx[i];
            for (int k = 0; k < row.Length; k++)
            {
                var v = row[k];
                if (v != 0d)
                {
                    int j = j0 + k;
                    S.setValue(i,j,v);
                }
            }
        }
        return S;
    }
}