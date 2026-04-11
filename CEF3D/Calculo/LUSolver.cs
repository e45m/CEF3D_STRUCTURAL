using CEF.matrixImpl;
using System.Runtime.CompilerServices;
namespace CEF.LUSolver;
[Flags]
enum CalcState
{
    None = 0,
    Primal = 1,
    Dual=2,
    Master=4,
    Slave = 8,
    Inactive=16,
    OK=32,
    Error=64,
    Pivot=128,
    Absent=256,
    Negative=512,
    MFCConstr=1024,
};
internal class LUSolver
{
    private  int totalRows;  
    private  BandedMatrix m_A;
    private int[] calcStatev;
    private double[] x;
    private double[] b;
    double eps = 10e-12;
    internal LUSolver(BandedMatrix A, int[] stateVect, double minDiagVal = 10e-12)
    {
        m_A = A.Clone();
        totalRows = A.RowsCount();
        calcStatev = stateVect;
        eps = minDiagVal;
    }
    internal BandedMatrix getMatrix() {
        return m_A;
    }
    internal BandedMatrix getCloneMatrix()
    {
        return m_A.Clone();
    }
    internal   void factorizeLU()
    {
        CalcState res;
        res = factorize();
    }
    internal bool solveCase(double[] b, double[] x)
    {
        CalcState res;
        this.b = b;
        this.x = x;
        res = solveBackandForward();
        return res == CalcState.OK;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isSlave(int i) =>
     (calcStatev[i] & (int)CalcState.Slave) != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isMaster(int i) =>
     (calcStatev[i] & (int)CalcState.Master) != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isInactive(int i) =>
     (calcStatev[i] & (int)CalcState.Inactive) != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isPrimal(int i) =>
        (calcStatev[i] & (int)CalcState.Primal) != 0 ;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isNegative(int i) =>
        (calcStatev[i] & (int)CalcState.Negative) != 0 ;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isDual(int i) =>
        (calcStatev[i] & (int)CalcState.Dual) != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isDualNonNegative(int i) =>
        isDual(i) && !isNegative(i);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isDualNegative(int i) =>
        isDual(i) && isNegative(i);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isToBeFactorize(int i) =>
        isDual(i) && !isInactive(i) && !isSlave(i) && !isPrimal(i);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isToBeSolvedBF(int i) =>
     (!isInactive(i) && !isSlave(i));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool isToBeSolvedDual(int i) => 
      (!isInactive(i) && !isSlave(i));
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private CalcState factorize()
    {
        var S_ = m_A.getS();
        var str = m_A.getpStart();
        int row, col, /*0,*/ start;
        double diag, rowCol;
        object sync = new();
        for (row = 0; row < totalRows; row++)
        {
            if (!isToBeFactorize(row))
                continue;
            for (col = str[row]; col < row; col++)
            {
                if (!isToBeFactorize(col))
                    continue;
                start = str[row] > str[col] ? str[row] : str[col];
                rowCol = S_[row][col - str[row]];
                for (int i = start; i < col; i++)
                {
                    if (!isToBeFactorize(i)) continue;
                    if (isDualNonNegative(i))
                    {
                        rowCol -= S_[row][i - str[row]] * S_[col][i - str[col]];
                    }
                    else if (isDualNegative(i))
                    {
                        rowCol += S_[row][i - str[row]] * S_[col][i - str[col]];
                    }
                }
                S_[row][col - str[row]] = rowCol;
                if (isDualNonNegative(col))
                {
                    S_[row][col - str[row]] *= S_[col][col - str[col]];
                    S_[row][row - str[row]] -= S_[row][col - str[row]]* S_[row][col - str[row]];
                }
                else if (isDualNegative(col))
                {
                    S_[row][col - str[row]] *= S_[col][col - str[col]];
                    S_[row][row - str[row]] += S_[row][col - str[row]]* S_[row][col - str[row]];
                }
            }
            diag = S_[row][row - str[row]];
            if (diag<eps && diag>-eps)
            {
                throw new Exception();
            }
            if (diag < 0.0)
            {
                calcStatev[row] |= (int)CalcState.Negative;
                S_[row][row - str[row]] = -S_[row][row - str[row]];
            }
            m_A[row, row] =
                1 / Math.Sqrt(S_[row][row - str[row]]);
        }
        return CalcState.OK;
    }
    private CalcState solveBackandForward()
    {
        int row, col;
        var b_aux = new double[totalRows];
        var S_ = m_A.getS();
        var str = m_A.getpStart();
        for (row = 0; row < totalRows; row++)
            if (isDual(row) && isToBeSolvedBF(row) )               
                b_aux[row] = b[row];
        for (col = 0; col < totalRows; col++)
            if (isPrimal(col) && isToBeSolvedBF(col))
                for (row = str[col]; row < col; row++)
                    if (isDual(row) && isToBeSolvedBF(row) )                       
                        b_aux[row] -= S_[col][row - str[col]] * x[col];
        for (row = 0; row < totalRows; row++)
            if (isDual(row) && isToBeSolvedBF(row))
                for (col = str[row]; col < row; col++)
                    if (isPrimal(col) && isToBeSolvedBF(col))                       
                        b_aux[row] -= S_[row][col - str[row]] * x[col];
        for (row = 0; row < totalRows; row++)          
            if (isDual(row) && isToBeSolvedBF(row))
            {
                for (col = str[row]; col < row; col++)
                    if (isDual(col) && isToBeSolvedBF(col))
                        b_aux[row] -= S_[row][col - str[row]] * b_aux[col];
                    b_aux[row] *= S_[row][row - str[row]];
            }
        for (row = 0; row < totalRows; row++)
            if (isDualNegative(row) && isToBeSolvedBF(row))
                b_aux[row] = -b_aux[row];
        for (row = 0; row < totalRows; row++)
            if (isDual(row) && isToBeSolvedBF(row))
                x[row] = b_aux[row];
        for (col = totalRows - 1; col >= 0; col--)
            if (isDual(col) && isToBeSolvedBF(col))
            {
                x[col] *= S_[col][col - str[col]];
                for (row = str[col]; row < col; row++)
                    if (isDual(row) && isToBeSolvedBF(row))
                        x[row] -= S_[col][row - str[col]] * x[col];
            }
        return CalcState.OK;
    }
    internal CalcState solveDual(in double[] d)
    {
        int col, row;
        var S_ = m_A.getS();
        var str = m_A.getpStart();
        for (row = 0; row < totalRows; row++)
            if (isPrimal(row) && isToBeSolvedDual(row))
            {
                d[row] = -b[row];
            }
            else if (isDual(row) && isToBeSolvedDual(row))
            {
                d[row] = 0.0;
            }
        for (col = 0; col < totalRows; col++)
            if (isToBeSolvedDual(col))
                for (row = str[col]; row < col; row++)
                    if (isPrimal(row) && isToBeSolvedDual(row))
                        d[row] += S_[col][row - str[col]] * x[col];
        for (row = 0; row < totalRows; row++)
            if (isPrimal(row) && isToBeSolvedDual(row))
                for (col = str[row]; col < row; col++)
                    if (isToBeSolvedDual(row))
                        d[row] += S_[row][col - str[row]] * x[col];
        return CalcState.OK;
    }
}