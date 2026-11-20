using System.Data.Common;
using System.Diagnostics;

namespace CEF;

static public class CadToolsUtils
{


///**************
    public static double[,] getTMatrix(double angX, double angY,double angZ) { 
    double Ca = Math.Cos(angX); 
    double Sa = Math.Sin(angX); 
    double Cb = Math.Cos(angY);
    double Sb = Math.Sin(angY);
    double Cc = Math.Cos(angZ);
    double Sc = Math.Sin(angZ);
    double[,] M_RotaX = new double[,] { { 1d, 0d, 0d, 0d },
                                            {0d, Ca, -Sa, 0d },
                                            {0d, Sa, Ca ,0d},
                                            {0d, 0d, 0d ,1d} };
    double[,] M_RotaY = new double[,] { {Cb, 0d, Sb, 0d },
                                            {0d, 1d, 0d, 0d },
                                            {-Sb, 0d, Cb ,0d},
                                            {0d, 0d, 0d ,1d} };
    double[,] M_RotaZ = new double[,] { { Cc, -Sc, 0d, 0d },
                                            {Sc, Cc, 0d, 0d },
                                            {0d, 0d, 1d ,0d},
                                            {0d, 0d, 0d ,1d} };
        var M = MatrixMath.M_mult(M_RotaX,M_RotaY);          
            M = MatrixMath.M_mult(M_RotaZ, M);
        return M;
}


    public static double[] calSecProfile(Section secc, int segms = 1, int offsety = 0, int offsetz = 0)
    {
        double[] pv = Array.Empty<double>();
        var tipo = (sectType)secc.Type;
        double dy = 0;
        double dz = 0;
        switch (offsety)
        {
            case 0:
                dy = 0.0;
                break;
            case 1:
                dy = -secc.b / 2;
                break;
            case 2:
                dy = secc.b / 2;
                break;
        }
        switch (offsetz)
        {
            case 0:
                dz = 0.0;
                break;
            case 1:
                dz = -secc.h / 2;
                break;
            case 2:
                dz = secc.h / 2;
                break;
        }
        void moveOffsets(double[] v)
        {
            if (dy == 0 && dz == 0) return;
            var l = v.Length;
            for (var i = 0; i < l; i += 3)
            {
                if (v[i] == double.MaxValue) continue;
                v[i + 1] += dy;
                v[i + 2] += dz;
            }
        }
        switch (tipo)
        {
            case sectType.Rect:
            case sectType.HolRec:
                {
                    double b = secc.b;
                    double h = secc.h;
                    pv = new double[]
                    {
                    0, -b/2, h/2,
                    0, b/2, h/2,
                    0, b/2, -h/2,
                    0, -b/2, -h/2,
                    0, -b/2, h/2,
                    double.MaxValue,double.MaxValue,double.MaxValue
                    };
                    moveOffsets(pv);
                    pv = InterpolarPerfil(pv, 3).ToArray();
                    break;
                }
            case sectType.Circ:
            case sectType.HolCirc:
                {
                    double r = secc.b / 2;
                    int n = 16;
                    pv = new double[n * 3 + 3];
                    for (int i = 0; i < n; i++)
                    {
                        double angle = 2 * Math.PI * i / n;
                        pv[i * 3 + 0] = 0;
                        pv[i * 3 + 1] = r * Math.Cos(angle);
                        pv[i * 3 + 2] = r * Math.Sin(angle);
                    }
                    pv[n * 3 + 0] = 0;
                    pv[n * 3 + 1] = pv[0 + 1];
                    pv[n * 3 + 2] = pv[0 + 2];
                    moveOffsets(pv);
                    if (segms > 1)
                        pv = InterpolarPerfil(pv, segms).ToArray();
                    break;
                }
            case sectType.IShape:
            case sectType.WShape:
                {
                    double b = secc.b, h = secc.h, tw = secc.tw, tf = secc.tf;
                    pv = new double[]
                    {
                    0.0,-b/2,h/2,
                    0.0, b/2,h/2,
                    0.0, b/2,h/2-tf,
                    0.0, tf/2,h/2-tf,
                    0.0, tw/2,h/2-tf,
                    0.0, tw/2,-h/2+tf,
                    0.0, b/2,-h/2+tf,
                    0.0, b/2,-h/2,
                    0.0, -b/2,-h/2,
                    0.0, -b/2,-h/2+tf,
                    0.0, -tw/2,-h/2+tf,
                    0.0, -tw/2,h/2-tf,
                    0.0, -b/2,h/2-tf,
                    0.0,-b/2,h/2,
                    double.MaxValue,double.MaxValue,double.MaxValue
                    };
                    moveOffsets(pv);
                    pv = InterpolarPerfil(pv, 3).ToArray();
                    break;
                }
            case sectType.Angle:
            case sectType.CProfile:
                {
                    double b = secc.b, h = secc.h, tw = secc.tw, tf = secc.tf;
                    pv = new double[]
                    {
                    0.0, -b/2, h/2,
                    0.0, b/2, h/2,
                    0.0, b/2, -h/2+tf,
                    0.0, tw/2, -h/2+tf,
                    0.0, tw/2, h/2-tf,
                    0.0, -b/2, h/2-tf,
                    0.0, -b/2, h/2,
                    double.MaxValue,double.MaxValue,double.MaxValue
                    };
                    moveOffsets(pv);
                    pv = InterpolarPerfil(pv, 3).ToArray();
                    break;
                }
        }
        return pv;
    }
   private static List<double> InterpolarPerfil(double[] src, int n)
    {
        var result = new List<double>();
        var grupo = new List<(double x, double y, double z)>();
        void cerrarGrupo()
        {
            if (grupo.Count < 2)
                return;
            for (int k = 0; k < grupo.Count - 1; k++)
            {
                var p0 = grupo[k];
                var p1 = grupo[k + 1];
                for (int i = 0; i < n; i++)
                {
                    double t = (double)i / (n - 1);
                    double x = p0.x + (p1.x - p0.x) * t;
                    double y = p0.y + (p1.y - p0.y) * t;
                    double z = p0.z + (p1.z - p0.z) * t;
                    result.Add(x);
                    result.Add(y);
                    result.Add(z);
                }
            }
            result.Add(double.MaxValue);
            result.Add(double.MaxValue);
            result.Add(double.MaxValue);
            grupo.Clear();
        }
        for (int i = 0; i < src.Length; i += 3)
        {
            if (src[i] == double.MaxValue)
            {
                cerrarGrupo();
                continue;
            }
            grupo.Add((src[i], src[i + 1], src[i + 2]));
        }
        cerrarGrupo();
        return result;
    }


}