using System.Runtime.InteropServices;
namespace CEF.matrixImpl
{
    internal class Tests
    {
        class ConsoleOut
        {
            const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;
            [DllImport("kernel32.dll")]
            static extern bool AttachConsole(uint dwProcessId);
            [DllImport("kernel32.dll")]
            static extern bool AllocConsole();
            public static void AttachOrCreate()
            {
                if (!AttachConsole(ATTACH_PARENT_PROCESS))
                    AllocConsole();
                var stdout = Console.OpenStandardOutput();
                var writer = new StreamWriter(stdout) { AutoFlush = true };
                Console.SetOut(writer);
                Console.SetError(writer);
            }
        }
        public async static void setUptest()
        {
            ConsoleOut.AttachOrCreate();
            Console.WriteLine("=== Console for test ===");
        }
        //==========================
        //   HERRAMIENTAS
        //==========================
        static BandedMatrix BM(double[] v, int rows, int cols)
        {
            var M = new BandedMatrix(rows, cols);
            M.makeZero();
            int k = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    M[i, j] = v[k++];
            M.setProfile();
            return M;
        }
        static BandedMatrix Transpose(BandedMatrix A)
        {
            var T = new BandedMatrix(A.getCols(), A.getRows());
            T.makeZero();
            for (int i = 0; i < A.getRows(); i++)
                for (int j = 0; j < A.getCols(); j++)
                    T[j, i] = A[i, j];
            T.setProfile();
            return T;
        }
        static BandedMatrix DenseMult(BandedMatrix A, BandedMatrix B)
        {
            var R = new BandedMatrix(A.getRows(), B.getCols());
            R.makeZero();
            for (int i = 0; i < A.getRows(); i++)
                for (int j = 0; j < B.getCols(); j++)
                    for (int k = 0; k < A.getCols(); k++)
                        R[i, j] += A[i, k] * B[k, j];
            R.setProfile();
            return R;
        }
        static void Print(BandedMatrix M)
        {
            for (int i = 0; i < M.getRows(); i++)
            {
                for (int j = 0; j < M.getCols(); j++)
                    Console.Write($"{M[i, j],6:0.###};");
                Console.WriteLine();
            }
        }
        static void Print(double[] M)
        {
            for (int i = 0; i < M.Length; i++)
            {
                Console.Write($"{M[i],6:0.###};");
                Console.WriteLine();
            }
        }
       public  static void Test_LU_Solve()
        {
            Console.WriteLine("=== TEST LU / solveCase ===");
            int n = 6;
            var A = new BandedMatrix(n);
            A.makeZero();
            // SPD simple y bien condicionada
            for (int i = 0; i < n; i++)
            {
                A[i, i] = 4.0;
                if (i > 0) A[i, i - 1] = -1.0;
                if (i < n - 1) A[i, i + 1] = -1.0;
            }
            A.setProfile();
            var AClone = new BandedMatrix(n);
            AClone.makeZero();
            // SPD simple y bien condicionada
            for (int i = 0; i < n; i++)
            {
                AClone[i, i] = 4.0;
                if (i > 0) AClone[i, i - 1] = -1.0;
                if (i < n - 1) AClone[i, i + 1] = -1.0;
            }
            AClone.setProfile();
            var ve = new int[]
                            { 2 ,2, 1, 2, 16, 2 };
            var cancel1 = 2;
            A[cancel1, 0] = A[cancel1, 1] = A[cancel1, cancel1] = A[cancel1, 3] = A[cancel1, 4] = A[cancel1, 5] = A[0, cancel1]
                = A[1, cancel1] = A[3, cancel1] = A[4, cancel1] = A[5, cancel1] = 0d;
            cancel1 = 4;
            A[cancel1, 0] = A[cancel1, 1] = A[cancel1, cancel1] = A[cancel1, 3] = A[cancel1, 4] = A[cancel1, 5] = A[0, cancel1]
    = A[1, cancel1] = A[3, cancel1] = A[4, cancel1] = A[5, cancel1] = 0d;
            double[] xRef = { 1, 2, 3, 4, 5, 6 };
            double[] b = BandedMatrix.M_mult(A, xRef);
            var solver = new LUSolver.LUSolver(A, ve,1E-12);
            solver.factorizeLU();
            double[] x = new double[n];
            double[] d = new double[n];
            solver.solveCase(b, x);
            solver.solveDual(d);
            // Residuo
            var r = BandedMatrix.M_mult(AClone, x);
            double err = 0.0;
            for (int i = 0; i < n; i++){
                if ((ve[i]&(1|16))==0)
                err = Math.Max(err, Math.Abs(r[i] - b[i]));
            }
            Console.WriteLine($"State - 2 to solve, 1 known values, 16 inactive");
            Print(ve.Select(i=>(double)i).ToArray());
            Console.WriteLine($"Xref");
            Print(xRef);
            Console.WriteLine($"b = A*Xref");
            Print(b);
            Console.WriteLine($"d");
            Print(d);
            Console.WriteLine($"x Calc");
            Print(x);
            Console.WriteLine($"r");
            Print(r);
            Console.WriteLine($"Max residual |Ax-b|(skiping 1/16) = {err:e3}");
        }
    }
}