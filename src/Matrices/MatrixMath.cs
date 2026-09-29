
namespace CEF;
internal static class MatrixMath
{
    public static void M_mult(double[,] M1_axm, double[,] M2_mxb, ref double[,] M3_axb)
    {
        int i;
        int j;
        int k;
        int a = M1_axm.GetUpperBound(0); int b = M2_mxb.GetUpperBound(1); int c = M2_mxb.GetUpperBound(0); for (i = 0; i <= a; i++)
        {
            for (j = 0; j <= b; j++)
                M3_axb[i, j] = 0.0d;
        }
        for (i = 0; i <= a; i++)
        {
            for (j = 0; j <= b; j++)
            {
                for (k = 0; k <= c; k++)
                    M3_axb[i, j] += M1_axm[i, k] * M2_mxb[k, j];
            }
        }
    }
    public static double[,] M_mult(double[,] M1_axm, double[,] M2_mxb)
    {
        int a = M1_axm.GetUpperBound(0); int b = M2_mxb.GetUpperBound(1); var M3_axb = new double[a + 1, b + 1];
        M_mult(M1_axm, M2_mxb, ref M3_axb);
        return M3_axb;
    }
    public static double[,] MT_mult(double[,] M1_mxa, double[,] M2_mxb)
    {
        int i;
        int j;
        int k;
        int a = M1_mxa.GetUpperBound(1); int b = M2_mxb.GetUpperBound(1); int m = M2_mxb.GetUpperBound(0); var M3_axb = new double[(a + 1), (b + 1)];
        for (i = 0; i <= a; i++)
        {
            for (j = 0; j <= b; j++)
                M3_axb[i, j] = 0.0d;
        }
        for (i = 0; i <= a; i++)
        {
            for (j = 0; j <= b; j++)
            {
                for (k = 0; k <= m; k++)
                    M3_axb[i, j] += M1_mxa[k, i] * M2_mxb[k, j];
            }
        }
        return M3_axb;
    }
    public static double[,] M_multT(double[,] M1_axm, double[,] M2_bxm)
    {
        int i;
        int j;
        int k;
        int a = M1_axm.GetUpperBound(0); 
        int b = M2_bxm.GetUpperBound(0); 
        int m = M2_bxm.GetUpperBound(1); 
        var M3_axb = new double[(a + 1), (b + 1)];
        for (i = 0; i <= a; i++)
        {
            for (j = 0; j <= b; j++)
                M3_axb[i, j] = 0.0d;
        }
        for (i = 0; i <= a; i++)
        {
            for (j = 0; j <= b; j++)
            {
                for (k = 0; k <= m; k++)
                    M3_axb[i, j] += M1_axm[i, k] * M2_bxm[j, k];
            }
        }
        return M3_axb;
    }
    public static double[,] InvertCholesky(double[,] A)
    {
        int n = A.GetLength(0);
        if (n != A.GetLength(1))
            throw new Exception("Matrix must be square");
        double[,] L = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = A[i, j];
                for (int k = 0; k < j; k++)
                    sum -= L[i, k] * L[j, k];
                if (i == j)
                {
                    if (sum <= 0.0)
                        throw new Exception("Matrix is not positive definitive");
                    L[i, j] = Math.Sqrt(sum);
                }
                else
                {
                    L[i, j] = sum / L[j, j];
                }
            }
        }
        double[,] Linv = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            Linv[i, i] = 1.0 / L[i, i];
            for (int j = 0; j < i; j++)
            {
                double sum = 0.0;
                for (int k = j; k < i; k++)
                    sum -= L[i, k] * Linv[k, j];
                Linv[i, j] = sum / L[i, i];
            }
        }
        double[,] inv = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = 0.0;
                for (int k = i; k < n; k++)
                    sum += Linv[k, i] * Linv[k, j];
                inv[i, j] = sum;
                inv[j, i] = sum;
            }
        }
        return inv;
    }
    public static double[,] Invert(double[,] A)
    {
        int n = A.GetLength(0);
        int m = A.GetLength(1);
        if (n != m) throw new ArgumentException("Matrix must be square.");
        var a = (double[,])A.Clone();
        var inv = new double[n, n];
        for (int i = 0; i < n; i++)
            inv[i, i] = 1.0;
        for (int i = 0; i < n; i++)
        {
            int pivotRow = i;
            double maxVal = Math.Abs(a[i, i]);
            for (int k = i + 1; k < n; k++)
            {
                if (Math.Abs(a[k, i]) > maxVal)
                {
                    maxVal = Math.Abs(a[k, i]);
                    pivotRow = k;
                }
            }
            if (pivotRow != i)
            {
                for (int j = 0; j < n; j++)
                {
                    double temp = a[i, j];
                    a[i, j] = a[pivotRow, j];
                    a[pivotRow, j] = temp;
                    temp = inv[i, j];
                    inv[i, j] = inv[pivotRow, j];
                    inv[pivotRow, j] = temp;
                }
            }
            double diag = a[i, i];
            const double Tolerance = 1e-10;
            if (Math.Abs(diag) < Tolerance)
            {
                throw new Exception("Cannot invert Matrix after pivot");
            }
            for (int j = 0; j < n; j++)
            {
                a[i, j] /= diag;
                inv[i, j] /= diag;
            }
            for (int k = 0; k < n; k++)
            {
                if (k == i) continue; double factor = a[k, i];
                for (int j = 0; j < n; j++)
                {
                    a[k, j] -= factor * a[i, j];
                    inv[k, j] -= factor * inv[i, j];
                }
            }
        }
        return inv;
    }
    public static void M_mult(double[] M1, int rows1, int cols1,
              double[] M2, int rows2, int cols2,
              ref double[] M3)
    {
        Array.Clear(M3, 0, M3.Length);
        for (int i = 0; i < rows1; i++)
        {
            int iOff1 = i * cols1;
            int iOff3 = i * cols2;
            for (int k = 0; k < cols1; k++)
            {
                double v1 = M1[iOff1 + k];
                int kOff2 = k * cols2;
                for (int j = 0; j < cols2; j++)
                {
                    M3[iOff3 + j] += v1 * M2[kOff2 + j];
                }
            }
        }
    }
    public static double[] M_mult(double[] M1, int rows1, int cols1, double[] M2, int rows2, int cols2)
    {
        var M3 = new double[rows1 * cols2];
        M_mult(M1, rows1, cols1, M2, rows2, cols2, ref M3);
        return M3;
    }
    public static double[] MT_mult(double[] M1, int rows1, int cols1, double[] M2, int rows2, int cols2)
    {
        int a = cols1;
        int b = cols2;
        int m = rows1;
        var M3 = new double[a * b];
        for (int i = 0; i < a; i++)
        {
            for (int k = 0; k < m; k++)
            {
                double v1 = M1[k * cols1 + i]; int kOff2 = k * b;
                int iOff3 = i * b;
                for (int j = 0; j < b; j++)
                {
                    M3[iOff3 + j] += v1 * M2[kOff2 + j];
                }
            }
        }
        return M3;
    }
    public static double[] M_multT(double[] M1, int rows1, int cols1, double[] M2, int rows2, int cols2)
    {
        int a = rows1;
        int b = rows2;
        int m = cols1;
        var M3 = new double[a * b];
        for (int i = 0; i < a; i++)
        {
            int iOff1 = i * m;
            int iOff3 = i * b;
            for (int j = 0; j < b; j++)
            {
                int jOff2 = j * m;
                double sum = 0;
                for (int k = 0; k < m; k++)
                {
                    sum += M1[iOff1 + k] * M2[jOff2 + k];
                }
                M3[iOff3 + j] = sum;
            }
        }
        return M3;
    }
    public static double[] InvertCholesky(double[] A, int n)
    {
        double[] L = new double[n * n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = A[i * n + j];
                for (int k = 0; k < j; k++)
                    sum -= L[i * n + k] * L[j * n + k];
                if (i == j)
                {
                    if (sum <= 0.0) throw new Exception("Matrix is not positive definitive.");
                    L[i * n + j] = Math.Sqrt(sum);
                }
                else L[i * n + j] = sum / L[j * n + j];
            }
        }
        double[] Linv = new double[n * n];
        for (int i = 0; i < n; i++)
        {
            Linv[i * n + i] = 1.0 / L[i * n + i];
            for (int j = 0; j < i; j++)
            {
                double sum = 0.0;
                for (int k = j; k < i; k++)
                    sum -= L[i * n + k] * Linv[k * n + j];
                Linv[i * n + j] = sum / L[i * n + i];
            }
        }
        double[] inv = new double[n * n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = 0.0;
                for (int k = i; k < n; k++)
                    sum += Linv[k * n + i] * Linv[k * n + j];
                inv[i * n + j] = inv[j * n + i] = sum;
            }
        }
        return inv;
    }
    public static double[] Invert(double[] A, int n)
    {
        double[] a = (double[])A.Clone();
        double[] inv = new double[n * n];
        for (int i = 0; i < n; i++)
            inv[i * n + i] = 1.0;
        for (int i = 0; i < n; i++)
        {
            int pivotRow = i;
            double maxVal = Math.Abs(a[i * n + i]);
            for (int k = i + 1; k < n; k++)
            {
                double val = Math.Abs(a[k * n + i]);
                if (val > maxVal)
                {
                    maxVal = val;
                    pivotRow = k;
                }
            }
            if (pivotRow != i)
            {
                int row1 = i * n;
                int row2 = pivotRow * n;
                for (int j = 0; j < n; j++)
                {
                    double tempA = a[row1 + j];
                    a[row1 + j] = a[row2 + j];
                    a[row2 + j] = tempA;
                    double tempInv = inv[row1 + j];
                    inv[row1 + j] = inv[row2 + j];
                    inv[row2 + j] = tempInv;
                }
            }
            double diag = a[i * n + i];
            const double Tolerance = 1e-10;
            if (Math.Abs(diag) < Tolerance)
                throw new Exception("Matrix is singular");
            int currentRowOff = i * n;
            for (int j = 0; j < n; j++)
            {
                a[currentRowOff + j] /= diag;
                inv[currentRowOff + j] /= diag;
            }
            for (int k = 0; k < n; k++)
            {
                if (k == i) continue;
                int targetRowOff = k * n;
                double factor = a[targetRowOff + i];
                for (int j = 0; j < n; j++)
                {
                    a[targetRowOff + j] -= factor * a[currentRowOff + j];
                    inv[targetRowOff + j] -= factor * inv[currentRowOff + j];
                }
            }
        }
        return inv;
    }
}