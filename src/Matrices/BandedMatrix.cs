using System.Runtime.CompilerServices;
namespace CEF.matrixImpl;
using T = double;
internal partial class BandedMatrix 
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref readonly  int getRStart(in int indx)
    {       
        return ref  p[indx].start;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref readonly int getREnd(in int indx)
    {       
        return ref p[indx].end;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref readonly int getCStart(in int indx)
    {       
        return ref vp[indx].start;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref readonly int getCEnd(in int indx)
    {       
        return ref vp[indx].end;
    }
    internal T[][] getS()
    {
        return rawMtrx;
    }
    internal int[] getpStart() => p.Select(i=>i.start).ToArray();
    internal int[] getpVStart() => vp.Select(i=>i.start).ToArray();
    internal int[] getpEnd() => p.Select(i=>i.end).ToArray();
    internal   int RowsCount() =>  Rows;  
    internal BandedMatrix(int SystemSize)
    {
        Rows = SystemSize;
        Columns = Rows;
        rawMtrx = new T[Rows][];
        p = new Profile[Rows];
        vp = new Profile[Columns];
        compactThreshold = (int)0;
    }
    internal BandedMatrix(int rows, int cols)
    {
        Rows = rows;
        Columns = cols;
        rawMtrx = new T[Rows][];
        p = new Profile[Rows];
        vp = new Profile[Columns];
        compactThreshold = (int)0;
    }
    internal void addRow(int indx,  T[] row, int start)
    {
        if (indx >= Rows) throw new Exception($"Index out of range SystemSize {Rows} index {indx}");
        if (row.Length == 0) throw new Exception($"Row size must be more than 0");
        rawMtrx[indx] = row;
        p[indx].start = start;
        p[indx].end = p[indx].start+ row.Length-1 ;
    }
    internal void addRow(int indx, int rowlen)
    {
        if (indx >= Rows) throw new Exception($"Index out of range SystemSize {Rows} index {indx}");
        if (rowlen <= 0)rowlen = 1;
        rawMtrx[indx] = new T[rowlen];
        p[indx].start = indx - rowlen + 1;
        p[indx].end = p[indx].start + rowlen-1;
    }
    internal void addRow(int indx, int rowlen, int profile)
    {
        if (indx >= Rows) throw new Exception($"Index out of range SystemSize {Rows} index {indx}");
        if (rowlen <= 0) rowlen = 1;
        rawMtrx[indx] = new T[rowlen];
        p[indx].start = profile;
        p[indx].end = p[indx].start + rowlen-1;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public  double getValue(int row, int col)
    {
        if ((col >= p[row].start && col <= p[row].end))
        {
            return  rawMtrx[row][getPhysicalIndex(row, col)];
        }
        else
        {            
            return  0d;
          }
        }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void setValue(int row, int col, in T value)
    {
        ensureCapacity(row, col);
        rawMtrx[row][getPhysicalIndex(row, col)] = value;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void incrementValue(int row, int col, in T value)
    {
        ensureCapacity(row, col);
        rawMtrx[row][getPhysicalIndex(row, col)] += value;
    }
    public  ref T getCellRef(int row, int col)
    {
            helper = row; row = col; col = helper;
        ensureCapacity(row, col);
        return ref rawMtrx[row][getPhysicalIndex(row, col)];
    }
    public T this[int n, int m]
    {  
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get=> getValue(n, m);  
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        { setValue(n, m, value);
        }
    }
    public T[,] getMatrix()
    {
        var Sout = new T[Rows, Columns];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Columns; j++)
                Sout[i, j] = rawMtrx[i][getPhysicalIndex(i, j)];
        return Sout;
    }
    public static implicit operator T[,](in BandedMatrix M)
    {
        return M.getMatrix();
    }
    }