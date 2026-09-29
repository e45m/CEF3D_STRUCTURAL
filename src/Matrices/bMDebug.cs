namespace CEF.matrixImpl;
using T = double;
internal partial class BandedMatrix
{
    internal void __debug_printMatrixInfo(string file)
    {
        file += ".csv";
        var M = this;
        using (var sw = new StreamWriter(file))
        {
            sw.Write($"Size: {M.RowsCount()}, Rows: {M.Rows}, Cols: {M.Columns}\n");
            sw.Write("Row;Start;End; Content\n");
            for (int i = 0; i < M.Rows; i++)
            {
                sw.Write(i); sw.Write(";");
                sw.Write(M.getRStart(i)); sw.Write(";");
                sw.Write(M.getREnd(i)); sw.Write(";");
                for (int j = 0; j < M.Columns; j++)
                {
                    sw.Write(M[i, j]); sw.Write(';');
                }
                sw.WriteLine();
            }
        }
    }
    internal void __debug_printMatrixInfo(string file, LUSolver.CalcState[] ve, LUSolver.CalcState skip = LUSolver.CalcState.Inactive )
    {
        file += ".csv";
        var M = this;
        using (var sw = new StreamWriter(file))
        {
            sw.Write($"Size: {M.RowsCount()}, Rows: {M.Rows}, Cols: {M.Columns}\n");
            sw.Write("Row;Start;End; Content\n");
            for (int i = 0; i < M.Rows; i++)
            {
                if ((ve[i] & skip) != 0) continue; 
                sw.Write(i); sw.Write(";");
                sw.Write(M.getRStart(i)); sw.Write(";");
                sw.Write(M.getREnd(i)); sw.Write(";");
                for (int j = 0; j < M.Columns; j++)
                {
                    if ((ve[j] & skip) != 0) continue; 
                    sw.Write(M[i, j]); sw.Write(';');
                }
                sw.WriteLine();
            }
        }
    }
    internal static void __debug_printVectInfo(string file, T[] data)
    {
        using (var sw = new StreamWriter(file))
        {
            sw.Write($"Size: {data.Length}\n");
            sw.Write("Row;Start;End; Content\n");
            for (int i = 0; i < data.Length; i++)
            {
                sw.Write(i); sw.Write(";");
                sw.Write(0); sw.Write(";");
                sw.Write(0); sw.Write(";");
                    sw.Write(data[i]); sw.Write(';');
                sw.WriteLine();
            }
        }
    }
    internal static void __debug_printMatrixInfo(string file, T[,] data)
    {
        var M = data;
        var rows = M.GetUpperBound(0)+1;
        var cols = M.GetUpperBound(1)+1;
        using (var sw = new StreamWriter(file))
        {
            sw.Write($"Size: {M.Length}, Rows: {rows}, Cols: {cols}");
            sw.Write("Row;Start;End; Content\n");
            for (int i = 0; i < rows; i++)
            {
                sw.Write(i); sw.Write(";");
                sw.Write(0); sw.Write(";");
                sw.Write(cols); sw.Write(";");
                for (int j = 0; j < cols; j++)
                {
                    sw.Write(M[i, j]); sw.Write(';');
                }
                sw.WriteLine();
            }
        }
    }
}