using CEF.Dictionary;
using System.Runtime.CompilerServices;
using static CEF.matrixImpl.BandedMatrix;
namespace CEF.matrixImpl;
using T = double;
internal partial class SparseMatrix : cefDictionary
{
    private int Rows;
    private int Columns;
    public int getRows() => Rows;
    public int getCols() => Columns;
    public SparseMatrix(int SystemSize)
    {
        Rows = SystemSize;
        Columns = Rows;
        setBucketSize(SystemSize);
        buckets = new bucket[sizeBuckets];
        Keys = [];
    }
    public SparseMatrix(int rows, int cols)
    {
        Rows = rows;
        Columns = cols;
        setBucketSize(10 * rows);
        buckets = new bucket[sizeBuckets];
        Keys = [];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double getValue(int n, int m)
    {
        TryGetValue(makeKey(n, m), out double val);
        return val;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void setValue(int n, int m, double value)
    {
        TryAdd(makeKey(n, m), value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void IncrementValue(int n, int m, double value)
    {   
        var key = makeKey(n, m);
        TryGetValue(key, out double val);    
        TryAdd(key, val + value);
    }
    new public double this[int n, int m] //shaded function
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => getValue(n, m);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            setValue(n, m, value);
        }
    }
    public void makeI()
    {
        buckets = new bucket[sizeBuckets];
        Keys = [];
        for (int i = 0; i < Rows; i++)
            TryAdd(makeKey(i, i), 1.0d);
    }
    public void makeZero()
    {
        buckets = new bucket[sizeBuckets];
        Keys = [];
    }
    internal BandedMatrix ToBanded()
    {
        var B = new BandedMatrix(Rows, Columns);
        B.makeZero();
        Keys.Sort();
        int io = -1, jo = -1;
        List<double> row = [];
        foreach (var kv in Keys)
        {
            int i = getRowFromKey(kv);
            int j = getColFromKey(kv);
            if (i != io)
            {
                if (io != -1)
                    B.addRow(io, row.ToArray(), jo);
                io = i;
                jo = j;
                row = [];
                row.EnsureCapacity((int)0.25*Rows);
            }
            int expectedJ = jo + row.Count;
            while (expectedJ < j)
            {
                row.Add(0.0);
                expectedJ++;
            }
            row.Add(getValue(i, j));
        }
        if (io != -1)
            B.addRow(io, row.ToArray(), jo);
        return B;
    }
    internal void __debug_printMatrixInfo(string file)
    {
        file += ".csv";
        var M = this;
        using (var sw = new StreamWriter(file))
        {
            sw.Write($"Size: {Keys.Count}, Rows: {M.Rows}, Cols: {M.Columns}\n");
            sw.Write("Row;Start;End; Content\n");
            for (int i = 0; i < M.Rows; i++)
            {
                sw.Write(i); sw.Write(";");
                sw.Write(""); sw.Write(";");
                sw.Write(""); sw.Write(";");
                for (int j = 0; j < M.Columns; j++)
                {
                    sw.Write(M[i, j]); sw.Write(';');
                }
                sw.WriteLine();
            }
        }
    }
    struct rowCol
    {
        public int row;
        public int col;
        public double value;
    }
    internal CSRstruct toCRS()
    {
        var mtrix = new CSRstruct();
        mtrix.rowStart = new int[Rows + 1];
        mtrix.colIndex = new int[Rows];
        mtrix.rowShift = new int[Rows];
        mtrix.rows = Rows;
        var rowcol = new List<rowCol>(Keys.Count);
        foreach (long key in Keys)
        {
            int r = getRowFromKey(key);
            int c = getColFromKey(key);
            rowcol.Add(new rowCol() { row = r, col = c, value = getValue(r, c) });
        }
        var sortedEntries = rowcol.OrderBy(i => i.row).ThenBy(i => i.col).ToList();
        var groupedByRow = sortedEntries.GroupBy(i => i.row).ToDictionary(g => g.Key, g => g.ToList());
        int currentDataPos = 0;
        for (int i = 0; i < Rows; i++)
        {
            if (groupedByRow.TryGetValue(i, out var elements))
            {
                int minCol = elements[0].col;
                int maxCol = elements[elements.Count - 1].col;
                int rowLength = maxCol - minCol + 1;
                mtrix.rowStart[i] = currentDataPos;
                mtrix.rowShift[i] = minCol;
                currentDataPos += rowLength;
            }
            else
            {
                mtrix.rowStart[i] = currentDataPos;
                mtrix.rowShift[i] = 0;
            }
        }
        mtrix.rowStart[Rows] = currentDataPos;
        mtrix.data = new T[currentDataPos];
        var dataSpan = mtrix.data.Span;
        foreach (var entry in sortedEntries)
        {
            int targetIndex = mtrix.rowStart[entry.row] + (entry.col - mtrix.rowShift[entry.row]);
            dataSpan[targetIndex] = entry.value;
        }
        return mtrix;
    }
}