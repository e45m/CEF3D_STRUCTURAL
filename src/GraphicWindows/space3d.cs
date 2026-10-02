using System.Numerics;
using static CEF.ModelDB;
using static CEF.modelView;
using static CEF.viewFrame;

using static System.Math;
namespace CEF;

using Line3 = barGeom;
using Point3 = node;
using View = viewFrame.View;

public class space3d
{
    private static int nodesCount_;
    public static int nodesCount
    {
        get
        {
            return nodesCount_;
        }
        set
        {
            nodesCount_ = value;
        }
    }
    public static int isPoint(point3D[] snapPoints, int X_, int Y_, View view, double level)
    {
        if (snapPoints == null) return -1;
        const float TolX = 5f, TolY = 5f; 
        int n = snapPoints.Length;
        Func<point3D, bool> levelFilter = view switch
        {
            View.XY => p => p.z3d == level,
            View.XZ => p => p.y3d == level,
            View.YZ => p => p.x3d == level,
            _ => p => true
        };
        for (int i = 0; i < n; i++)
        {
            var p = snapPoints[i];
            if (!levelFilter(p)) continue;
            if (X_ + TolX >= p.x2d & X_ - TolX <= p.x2d &
                Y_ + TolY >= p.y2d & Y_ - TolY <= p.y2d)
                return i;
        }
        return -1;
    }
    public static int isBar(int[] conect, point3D[] snapPoints, int X_, int Y_, View view, double level)
    {
        if (snapPoints == null) return -1;
        const int TolX = 5, TolY = 5; 

        if (isPoint(snapPoints, X_, Y_, view, level) != -1) return -1;
        if (conect == null) return -1;
        Func<point3D, bool> levelFilter = view switch
        {
            View.XY => p => p.z3d == level,
            View.XZ => p => p.y3d == level,
            View.YZ => p => p.x3d == level,
            _ => p => true
        };
        int UB = conect.GetUpperBound(0);
       
        for (int i = 0; i < UB; i+=3)
        {
            var p1 = snapPoints[conect[i+ 1]];
            var p2 = snapPoints[conect[i+ 2]];
            if (!levelFilter(p1) | !levelFilter(p2)) continue;
            float x1 = p1.x2d, x2 = p2.x2d;
            float y1 = p1.y2d, y2 = p2.y2d;
            bool casiVertical = Math.Abs(x1 - x2) < (TolX / 2f);
            if (!casiVertical)
            {
                float xmin = x1 < x2 ? x1 : x2;
                float xmax = x1 > x2 ? x1 : x2;
                if (X_ >= xmin & X_ <= xmax)
                {
                    float cy = y1 + (y1 - y2) / (x1 - x2) * (X_ - x1);
                    if (Y_ + TolY >= cy & Y_ - TolY <= cy)
                        return (int)i / 3;
                }
            }
            else
            {
                if (Math.Abs(x1 - X_) <= TolX)
                {
                    float ymin = y1 < y2 ? y1 : y2;
                    float ymax = y1 > y2 ? y1 : y2;
                    if (Y_ >= ymin & Y_ <= ymax)
                       return (int)i/3;
                }
            }
        }
        return -1;
    }
    public static bool IsArea(PointF p, List<PointF> poly)
    {
        bool inside = false;
        int n = poly.Count; 
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            var pi = poly[i];
            var pj = poly[j];
            bool intersect =
                ((pi.Y > p.Y) != (pj.Y > p.Y)) &&
                (p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y) + pi.X);
            if (intersect) inside = !inside;
        }
        return inside;
    }
    public static bool IsVolume(Vector3 p, List<List<Vector3>> faces)
    {
        foreach (var face in faces)
        {
            Vector3 a = face[0];
            Vector3 b = face[1];
            Vector3 c = face[2];
            Vector3 n = Vector3.Cross(b - a, c - a);  
            float side = Vector3.Dot(p - a, n);
            if (side > 0f)
                return false;
        }
        return true;
    }
    public static void createGrid(CEF.MainWindow CEF3dMain)
    {
        int i, j, k, i_; 
        var Conect = new object[4]; 


        var Dx = App.model.gridCoordsX.Where(c => c.coordSys ==App.currCoordSist).Select(i => i.Coord).ToArray();
        var Dy = App.model.gridCoordsY.Where(c => c.coordSys ==App.currCoordSist).Select(i => i.Coord).ToArray();
        var Dz = App.model.gridCoordsZ.Where(c => c.coordSys ==App.currCoordSist).Select(i => i.Coord).ToArray();

        var ScX = App.g_gridXPointsCount =  Dx.Length;
        var ScY = App.g_gridYPointsCount =  Dy.Length;
        var ScZ = App.g_gridZPointsCount = Dz.Length;



        App.appData.Puntos = new CEF.DtaTable<node>();
        
        i_ = 0; 
        for (k = 0; k < ScZ; k++) 
        {
            for (j = 0; j < ScY; j++)
            {
                for (i = 0; i < ScX; i++)
                {
                    App.appData.Puntos[i_].ID = i_; 
                    App.appData.Puntos[i_].X = Dx[i]; 
                    App.appData.Puntos[i_].Y = Dy[j]; 
                    App.appData.Puntos[i_].Z = Dz[k]; 
                    i_ += 1; 
                }
            }
        }
    }
    public static double[,] getLineRotMtrxT(double[] loc) 
    {
        double[,] aux = new double[3, 3], TT = new double[3, 3];
        int i, j;
        aux = getLineRotMtrx(loc);
        for (i = 0; i <= 2; i++)
        {
            for (j = 0; j <= 2; j++)
                TT[i, j] = aux[j, i];
        }
        return TT;
    }
    public static double[,] getLineRotMtrx(double[] vect) 
    {
        double L, d, dx, dy, dz;
        var T = new double[3, 3];
        int i, j;
        dx = vect[3] - vect[0];
        dy = vect[4] - vect[1];
        dz = vect[5] - vect[2];
        L = Sqrt(Pow(dx, 2d) + Pow(dy, 2d) + Pow(dz, 2d));
        d = Sqrt(Pow(dx, 2d) + Pow(dy, 2d));
        for (i = 0; i <= 2; i++)
        {
            for (j = 0; j <= 2; j++)
                T[i, j] = 0d;
        }
        if (d != 0d)
        {
            T[0, 0] = dx / L;
            T[0, 1] = -dy / d;
            T[0, 2] = -dz * dx / (L * d);
            T[1, 0] = dy / L;
            T[1, 1] = dx / d;
            T[1, 2] = -dz * dy / (L * d);
            T[2, 0] = dz / L;
            T[2, 1] = 0d;
            T[2, 2] = d / L;
        }
        else if (d > 0d)
        {
            T[0, 2] = -1;
            T[1, 1] = 1d;
            T[2, 0] = 1d;
        }
        else 
        {
            T[0, 2] = 1d;
            T[1, 1] = 1d;
            T[2, 0] = -1;
        }
        return T;
    }
    public static void loc2glob(double[] Vect, ref double[] loc, double[] glob) 
    {
        int i, j, k, l;
        var T = new double[3, 3];
        T = getLineRotMtrx(Vect);
        l = loc.Length - 1;
        for (k = 0; k <= l; k += 3)
        {
            for (i = 0; i <= 2; i++)
            {
                loc[i + k] = 0d;
                for (j = 0; j <= 2; j++)
                    loc[i + k] += T[i, j] * glob[j + k];
            }
        }
       
    }
    public static void glob2loc(double[] Vect, double[] loc, ref double[] glob) 
    {
        int i, j, k, l;
        var T = new double[3, 3];
        T = getLineRotMtrx(Vect);
        l = glob.Length - 1;
        for (k = 0; k <= l; k += 3)
        {
            for (i = 0; i <= 2; i++)
            {
                  glob[i + k] = 0d;
                for (j = 0; j <= 2; j++)
                    glob[i + k] += T[j, i] * loc[j + k];
            }
        }
       
    }
    public  static void modelViewSelbyRectangle(point3D[] SnapPoints, Rectangle selRect, int[] barsIdNiNj,bool leftToRight , viewFrame mv)
    {
        RectangleF R = RectangleF.FromLTRB(
            Math.Min(selRect.Left, selRect.Right),
            Math.Min(selRect.Top, selRect.Bottom),
            Math.Max(selRect.Left, selRect.Right),
            Math.Max(selRect.Top, selRect.Bottom));
        var pc = mv.getPlaneCoord();
        var plane = mv.getPlane();
        Func<point3D, bool> levelFilter = plane switch
        {
            View.XY => p => p.z3d == pc,
            View.XZ => p => p.y3d == pc,
            View.YZ => p => p.x3d == pc,
            _ => p => true
        };
        var selectNodes = SnapPoints
            .Where(p =>
            {
                bool inside = (p.x2d >= R.Left && p.x2d <= R.Right &&
                               p.y2d >= R.Top && p.y2d <= R.Bottom);
                bool r = levelFilter(p);
                return inside && p.IsNode && r
            ;          
            })
            .Distinct()
            .ToArray();
        for (int i = 0; i < selectNodes.Length; i++)
        {
            int idx = App.model.nodes.FindIndexByID(selectNodes[i].Id);
            if (idx >= 0) Selection.nodeAdd(idx);
        }
        var ns = Selection.getNodesColection();
        static float Orient(PointF a, PointF b, PointF c)
            => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        static bool SegIntersects(PointF p, PointF p2, PointF q, PointF q2)
        {
            float o1 = Orient(p, p2, q);
            float o2 = Orient(p, p2, q2);
            float o3 = Orient(q, q2, p);
            float o4 = Orient(q, q2, p2);
            return (o1 * o2 < 0 && o3 * o4 < 0 );
        }
        static bool SegmentIntersectsRect(PointF p1, PointF p2, RectangleF r)
        {
            if (r.Contains(p1) || r.Contains(p2))
                return true;
            PointF a = new(r.Left, r.Top);
            PointF b = new(r.Right, r.Top);
            PointF c = new(r.Right, r.Bottom);
            PointF d = new(r.Left, r.Bottom);
            return SegIntersects(p1, p2, a, b) ||
                   SegIntersects(p1, p2, b, c) ||
                   SegIntersects(p1, p2, c, d) ||
                   SegIntersects(p1, p2, d, a);
        }
       var selectBars = App.model.bars
    .Where(b =>
    {
    int i = App.model.nodes.FindIndexByID(b.startNode);
    int j = App.model.nodes.FindIndexByID(b.endNode);
        point3D p1 = new()
        {
            x3d = App.model.nodes[i].X,
            y3d = App.model.nodes[i].Y,
            z3d = App.model.nodes[i].Z
        };
        point3D p2 = new()
        {
            x3d = App.model.nodes[j].X,
            y3d = App.model.nodes[j].Y,
            z3d = App.model.nodes[j].Z
        };
        mv.p3DSet2DCoord(ref p1);
        mv.p3DSet2DCoord(ref p2);
        PointF p1f = new PointF(p1.x2d, p1.y2d);
        PointF p2f = new PointF(p2.x2d, p2.y2d);
        if (leftToRight)
        {
            var pf = levelFilter(p1) && levelFilter(p2);
            return R.Contains(p1f) && R.Contains(p2f)&&pf;
        }
        else
        {
            var pf = levelFilter(p1)|| levelFilter(p2);
            return SegmentIntersectsRect(p1f, p2f, R)&&pf;
        }
    })
    .Distinct()
    .ToArray();
        foreach(var b in selectBars)
        {
            int indxbar = App.model.bars.FindIndexByID(b.ID);
            Selection.barAdd(indxbar);
        }
        }

    public static Point3 Sub(Point3 a, Point3 b) => new Point3 { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };

   public static Point3 Cross(Point3 a, Point3 b) => new Point3
    {
        X = a.Y * b.Z - a.Z * b.Y,
        Y = a.Z * b.X - a.X * b.Z,
        Z = a.X * b.Y - a.Y * b.X
    };

    public static double Dot(Point3 a, Point3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    public static double Norm2(Point3 a) => Dot(a, a);
    const double EPS = 1e-9;
    public static Point3 Intersect(Line3 b, Line3 br)
    {
        var v1 = Sub(b.P2, b.P1);
        var v2 = Sub(br.P2, br.P1);
        var w = Sub(br.P1, b.P1);

        var n = Cross(v1, v2);
        var denom = Norm2(n);

        // Paralelas o degeneradas
        if (denom < EPS) return null;

        // No coplanares → no se intersectan
        if (Math.Abs(Dot(w, n)) > EPS) return null;

        var wxv2 = Cross(w, v2);
        double t = Dot(wxv2, n) / denom;

        if (t < 0 || t > 1) return null;

        Point3 p;
        p = new Point3() {

          X =  b.P1.X + t * v1.X,
          Y =  b.P1.Y + t * v1.Y,
          Z =  b.P1.Z + t * v1.Z};
        
        return p;
    }

    public static Point3 NodeIntersect(Line3 b, Point3 ip)
    {
        var v1 = Sub(b.P2, b.P1);
       

        var L = Mis.calcDist(b.P2,b.P1);
        var L2 = Mis.calcDist(ip,b.P1);

        if (L2 > L) return null;

   
            var t = L2 / L;
            var test =
             Math.Abs((b.P1.X + t * v1.X) - ip.X) < EPS &&
             Math.Abs((b.P1.Y + t * v1.Y) - ip.Y) < EPS &&
             Math.Abs((b.P1.Z + t * v1.Z) - ip.Z) < EPS;

        if (test) return ip;

        return null;
    }

    public static  node checkNewNode(node nodo, double tol= 0.01)
    {
        if (nodo == null) return null;
       
        var isNode = App.model.nodes.FirstOrDefault(n => (Math.Abs(n.X - nodo.X) < tol &&
                                                             Math.Abs(n.Y - nodo.Y) < tol &&
                                                             Math.Abs(n.Z - nodo.Z) < tol),
                                                             new node() { ID = -1 });

        if (isNode.ID != -1) return isNode;

        int k = App.model.nodes.Max(n => n.ID) + 1;
        nodo.ID = k;
        nodo.label = $"{nodo.ID}Seg-{k}";
        App.model.nodes.Add(nodo);
        App.model.nodalRestraints.Add(new nodalRestraint { ID = nodo.ID });
        return nodo;
    }



}