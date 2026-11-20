using System.Numerics;
using System.Drawing;
using static CEF.ModelDB;



using static System.Math;
using System.Security.AccessControl;
namespace CEF;

using Line3 = barGeom;
using Point3 = Node;


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
public static double calcDist(in Node a,in Node b) =>
            Math. Sqrt(Math.Pow(b.X - a.X, 2d) + Math.Pow(b.Y - a.Y, 2d) + Math.Pow(b.Z - a.Z, 2d));

    public static int isPoint(point3D[] snapPoints, int X_, int Y_, ViewFrameView view, double level)
    {
        if (snapPoints == null) return -1;
        const float TolX = 3f, TolY = 3f; 
        int n = snapPoints.Length;
        Func<point3D, bool> levelFilter = view switch
        {
            ViewFrameView.XY => p => p.z3d == level,
            ViewFrameView.XZ => p => p.y3d == level,
            ViewFrameView.YZ => p => p.x3d == level,
            _ => p => true
        };
        for (int i = 0; i < n; i++)
        {
            var p = snapPoints[i];
            if (!levelFilter(p)) continue;
            if (X_ + TolX >= p.x2d && X_ - TolX <= p.x2d &
                Y_ + TolY >= p.y2d && Y_ - TolY <= p.y2d)
                return i;
        }
        return -1;
    }
    public static int isBar(int[] conect, point3D[] snapPoints, int X_, int Y_, ViewFrameView view, double level)
    {//Area conet bar idbar, index p1, index p2
        if (snapPoints == null) return -1;
        const float  TolX = 3, TolY = 3; 

        if (isPoint(snapPoints, X_, Y_, view, level) != -1) return -1;
        if (conect == null) return -1;
        Func<point3D, bool> levelFilter = view switch
        {
            ViewFrameView.XY => p => p.z3d == level,
            ViewFrameView.XZ => p => p.y3d == level,
            ViewFrameView.YZ => p => p.x3d == level,
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
            bool casiVertical = (x1 - x2)*(x1 - x2)/((x1 - x2)*(x1 - x2)+(y1 - y2)*(y1 - y2)) < (0.05);
            if (!casiVertical)
            {
                float xmin = x1 < x2 ? x1 : x2;
                float xmax = x1 > x2 ? x1 : x2;
                if (X_ >= xmin && X_ <= xmax)
                {
                    float cy = y1 + (y1 - y2) / (x1 - x2) * (X_ - x1);
                    if (Y_ + TolY >= cy && Y_ - TolY <= cy)
                        return (int)i / 3;
                }
            }
            else
            {
                if (Math.Abs(x1 - X_) <= TolX)
                {
                    float ymin = y1 < y2 ? y1 : y2;
                    float ymax = y1 > y2 ? y1 : y2;
                    if (Y_ >= ymin && Y_ <= ymax)
                       return (int)i/3;
                }
            }
        }
        return -1;
    }
	
public static int isArea(int[] conect, point3D[] snapPoints, int X_, int Y_, ViewFrameView view, double level)
{//Area conect: idArea, index p1, p2, p3, p4 (5 ints por área)
    if (snapPoints == null || conect == null) return -1;
    if (isPoint(snapPoints, X_, Y_, view, level) != -1) return -1;

    const int NPts = 4, Stride = NPts + 1;

    Func<point3D, bool> levelFilter = view switch
    {
        ViewFrameView.XY => p => p.z3d == level,
        ViewFrameView.XZ => p => p.y3d == level,
        ViewFrameView.YZ => p => p.x3d == level,
        _ => p => true
    };

    for (int i = 0; i + Stride <= conect.Length; i += Stride)
    {
        bool ok = true;
        var pts = new point3D[NPts];
        for (int k = 0; k < NPts; k++)
        {
            pts[k] = snapPoints[conect[i + 1 + k]];
            if (!levelFilter(pts[k])) { ok = false; break; }
        }
        if (!ok) continue;

        // Ray casting
        bool inside = false;
        for (int a = 0, b = NPts - 1; a < NPts; b = a++)
        {
            float xa = pts[a].x2d, ya = pts[a].y2d;
            float xb = pts[b].x2d, yb = pts[b].y2d;
            if ((ya > Y_) != (yb > Y_) &&
                X_ < (xb - xa) * (Y_ - ya) / (yb - ya) + xa)
                inside = !inside;
        }
        if (inside) return i / Stride;
    }
    return -1;
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
    public static void createGrid(ModelDB model,AppDB appData,string currCoordSist, ref int g_gridXPointsCount, 
    ref int g_gridYPointsCount , ref int g_gridZPointsCount  )
    {
        int i, j, k, i_; 
        
        var Dx = model.gridCoordsX.Where(c => c.coordSys ==currCoordSist).Select(i => i.Coord).ToArray();
        var Dy = model.gridCoordsY.Where(c => c.coordSys ==currCoordSist).Select(i => i.Coord).ToArray();
        var Dz = model.gridCoordsZ.Where(c => c.coordSys ==currCoordSist).Select(i => i.Coord).ToArray();

        var ScX = g_gridXPointsCount = Dx.Length;
        var ScY = g_gridYPointsCount = Dy.Length;
        var ScZ = g_gridZPointsCount = Dz.Length;

        appData.Puntos = new CEF.DtaTable<Node>();
        
        i_ = 0; 
        for (k = 0; k < ScZ; k++) 
        {
            for (j = 0; j < ScY; j++)
            {
                for (i = 0; i < ScX; i++)
                {
                     appData.Puntos[i_]=new();
                     appData.Puntos[i_].ID = i_; 
                     appData.Puntos[i_].X = Dx[i]; 
                     appData.Puntos[i_].Y = Dy[j]; 
                     appData.Puntos[i_].Z = Dz[k]; 
                    i_ += 1; 
                }
            }
        }
    }
    public static double[,] getLineRotMtrxGlobToLoc(double[] loc) 
    {
        double[,] aux = new double[3, 3], TT = new double[3, 3];
        int i, j;
        aux = getLineRotMtrxLocToGlob(loc);
        for (i = 0; i <= 2; i++)
        {
            for (j = 0; j <= 2; j++)
                TT[i, j] = aux[j, i];
        }
        return TT;
    }
    public static double[,] getLineRotMtrxLocToGlob(double[] vect) 
    {
        double L, d, dx, dy, dz;
        var T = new double[3, 3];
        dx = vect[3] - vect[0];
        dy = vect[4] - vect[1];
        dz = vect[5] - vect[2];
        L = Sqrt(Pow(dx, 2d) + Pow(dy, 2d) + Pow(dz, 2d));
        d = Sqrt(Pow(dx, 2d) + Pow(dy, 2d));
        if (d > 0d)//only false if == 0
        {
            T[0, 0] =  dx/L;
            T[0, 1] = -dy/d;
            T[0, 2] = -dx*dz/(d*L);
            T[1, 0] =  dy/L;
            T[1, 1] =  dx/d;
            T[1, 2] = -dy*dz/(d*L);
            T[2, 0] =  dz/L;
            T[2, 1] =  0d;
            T[2, 2] =  d/L;

        }
        else{
       for (int i = 0; i <= 2; i++)
            for (int j = 0; j <= 2; j++)
                T[i, j] = 0d;
            
        if (dz > 0d)
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
        }}
        return T;
    }
  public static void getLineRotMtrxLocToGlob(double[] vect,  double[,] T) 
    {
        double L, d, dx, dy, dz;     
        dx = vect[3] - vect[0];
        dy = vect[4] - vect[1];
        dz = vect[5] - vect[2];
        L = Sqrt(dx*dx + dy*dy + dz*dz);
        d = Sqrt(dx*dx + dy*dy);   
     
        if (d > 0d)//only false if == 0
        {
            T[0, 0] =  dx/L;
            T[0, 1] = -dy/d;
            T[0, 2] = -dx*dz/(d*L);
            T[1, 0] =  dy/L;
            T[1, 1] =  dx/d;
            T[1, 2] = -dy*dz/(d*L);
            T[2, 0] =  dz/L;
            T[2, 1] =  0d;
            T[2, 2] =  d/L;

        }
        else{
       for (int i = 0; i <= 2; i++)
            for (int j = 0; j <= 2; j++)
                T[i, j] = 0d;
            
        if (dz > 0d)
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
        }}
        
    }
    
    public static void loc2glob(double[] Vect, ref double[] loc, double[] glob) 
    {
        int i, j, k, l;
        
        var T = getLineRotMtrxLocToGlob(Vect);
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
       
        var T = getLineRotMtrxLocToGlob(Vect);
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


public static void modelViewSelbyRectangle(
    point3D[] SnapPoints,
    CEFRectangle selRect,
    int[] barsIdNiNj,
    int[] AreaConect,
    bool leftToRight,
    ViewFrameView view, double pc, ModelDB model)
{
 


    CEFRectangle R = new(){
       Left = Math.Min(selRect.Left, selRect.Right),
       Top= Math.Min(selRect.Top, selRect.Bottom),
       Right =  Math.Max(selRect.Left, selRect.Right),
       Bottom =  Math.Max(selRect.Top, selRect.Bottom)};
       R.Height = R.Bottom-R.Top;
       R.Width = R.Right-R.Left;



    bool levelFilter(point3D p) => view switch
    {
        ViewFrameView.XY => p.z3d == pc,
        ViewFrameView.XZ => p.y3d == pc,
        ViewFrameView.YZ => p.x3d == pc,
        _ => true
    };

    static PointF ToPoint(point3D p) => new(p.x2d, p.y2d);

    

    bool Inside(point3D p) =>
        R.Contains(p) &&levelFilter(p);

    static float Orient(PointF a, PointF b, PointF c) =>
        (b.X - a.X) * (c.Y - a.Y) -
        (b.Y - a.Y) * (c.X - a.X);

    static bool OnSegment(PointF a, PointF b, PointF p) =>
        p.X >= Math.Min(a.X, b.X) &&
        p.X <= Math.Max(a.X, b.X) &&
        p.Y >= Math.Min(a.Y, b.Y) &&
        p.Y <= Math.Max(a.Y, b.Y);

    static bool SegIntersects(PointF a, PointF b, PointF c, PointF d)
    {
        float o1 = Orient(a, b, c);
        float o2 = Orient(a, b, d);
        float o3 = Orient(c, d, a);
        float o4 = Orient(c, d, b);

        return (o1 * o2 < 0 && o3 * o4 < 0) ||
               (o1 == 0 && OnSegment(a, b, c)) ||
               (o2 == 0 && OnSegment(a, b, d)) ||
               (o3 == 0 && OnSegment(c, d, a)) ||
               (o4 == 0 && OnSegment(c, d, b));
    }

    bool SegmentIntersectsRect(PointF a, PointF b)
    {
        if (R.Contains(a.X,a.Y) || R.Contains(b.X,b.Y))
            return true;

        PointF c = new(R.Left, R.Top);
        PointF d = new(R.Right, R.Top);
        PointF e = new(R.Right, R.Bottom);
        PointF f = new(R.Left, R.Bottom);

        return SegIntersects(a, b, c, d) ||
               SegIntersects(a, b, d, e) ||
               SegIntersects(a, b, e, f) ||
               SegIntersects(a, b, f, c);
    }

    bool SelectPolygon(int n1, int n2, int n3, int n4)
    {
        var p1 = SnapPoints[n1];
        var p2 = SnapPoints[n2];
        var p3 = SnapPoints[n3];
        var p4 = SnapPoints[n4];

        if (!levelFilter(p1) || !levelFilter(p2) ||
            !levelFilter(p3) || !levelFilter(p4))
            return false;

        if (leftToRight)
            return Inside(p1) && Inside(p2) &&
                   Inside(p3) && Inside(p4);

        return SegmentIntersectsRect(ToPoint(p1), ToPoint(p2)) ||
               SegmentIntersectsRect(ToPoint(p2), ToPoint(p3)) ||
               SegmentIntersectsRect(ToPoint(p3), ToPoint(p4)) ||
               SegmentIntersectsRect(ToPoint(p4), ToPoint(p1));
    }

    // Select nodes.
   
    for(int i=0;i<SnapPoints.Length;i++){       
        if(SnapPoints[i].IsNode && Inside(SnapPoints[i])){
        var j  =  model.nodes.FindIndexByID(SnapPoints[i].Id);
        Selection.nodeAdd(j);
        }
    }

    // Select bars: [ID, n1, n2].
    for (int i = 0; i < barsIdNiNj.Length; i += 3)
    {
        int id = barsIdNiNj[i];
        var p1 = SnapPoints[barsIdNiNj[i + 1]];
        var p2 = SnapPoints[barsIdNiNj[i + 2]];

        bool selected = leftToRight
            ? Inside(p1) && Inside(p2)
            : (levelFilter(p1) || levelFilter(p2)) &&
              SegmentIntersectsRect(ToPoint(p1), ToPoint(p2));

        if (selected)
        {   
        var j  =  model.bars.FindIndexByID(id);
            Selection.barAdd(j);}
    }

    // Select areas: [ID, n1, n2, n3, n4].
    for (int i = 0; i < AreaConect.Length; i += 5)
    {
        if (SelectPolygon(
            AreaConect[i + 1],
            AreaConect[i + 2],
            AreaConect[i + 3],
            AreaConect[i + 4]))
        {
           var ind  =  model.areas.FindIndexByID(AreaConect[i]);
            Selection.areaAdd(ind);
        }
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
       

        var L =  calcDist(b.P2,b.P1);
        var L2 = calcDist(ip,b.P1);

        if (L2 > L) return null;

   
            var t = L2 / L;
            var test =
             Math.Abs((b.P1.X + t * v1.X) - ip.X) < EPS &&
             Math.Abs((b.P1.Y + t * v1.Y) - ip.Y) < EPS &&
             Math.Abs((b.P1.Z + t * v1.Z) - ip.Z) < EPS;

        if (test) return ip;

        return null;
    }

    public static  Node checkNewNode(Node nodo, ModelDB model, double tol= 0.01)
    {
        if (nodo == null) return null;
       
        var isNode = model.nodes.FirstOrDefault(n => (Math.Abs(n.X - nodo.X) < tol &&
                                                             Math.Abs(n.Y - nodo.Y) < tol &&
                                                             Math.Abs(n.Z - nodo.Z) < tol),
                                                             new Node() { ID = -1 });

        if (isNode.ID != -1) return isNode;

        int k = model.nodes.Max(n => n.ID) + 1;
        nodo.ID = k;
        nodo.label = $"{nodo.ID}Seg-{k}";
        model.nodes.Add(nodo);
        model.nodalRestraints.Add(new nodalRestraint { ID = nodo.ID });
        return nodo;
    }




/// <summary>
/// Global -> local rotation matrix of a plane (rows = local axes expressed in global coords).
/// pts: [x1,y1,z1, x2,y2,z2, x3,y3,z3] (triangle) or 4 points (12 values, quad).
/// Local x' = edge 1-2 projected onto the plane, z' = plane normal, y' = z' x x'.
/// v_local = rot * v_global
/// </summary>
internal static void getPlaneRotMtrxGlobToLoc(double[] pts, double[,] rot)
{
    if (pts.Length != 9 && pts.Length != 12)
        throw new ArgumentException("pts must hold 3 or 4 points (9 or 12 values).");

    // Plane normal
    double nx, ny, nz;
    if (pts.Length == 9)
    {
        // Cross product of edges 1-2 and 1-3
        double ax = pts[3] - pts[0], ay = pts[4] - pts[1], az = pts[5] - pts[2];
        double bx = pts[6] - pts[0], by = pts[7] - pts[1], bz = pts[8] - pts[2];
        nx = ay * bz - az * by;
        ny = az * bx - ax * bz;
        nz = ax * by - ay * bx;
    }
    else
    {
        // Cross product of diagonals 1-3 and 2-4 (robust for non-planar quads)
        double d1x = pts[6] - pts[0], d1y = pts[7] - pts[1], d1z = pts[8] - pts[2];
        double d2x = pts[9] - pts[3], d2y = pts[10] - pts[4], d2z = pts[11] - pts[5];
        nx = d1y * d2z - d1z * d2y;
        ny = d1z * d2x - d1x * d2z;
        nz = d1x * d2y - d1y * d2x;
    }

    double nLen = Math.Sqrt(nx * nx + ny * ny + nz * nz);
    if (nLen < 1e-12) throw new InvalidOperationException("Degenerate plane (zero normal).");
    nx /= nLen; ny /= nLen; nz /= nLen;

    // Local x axis: edge 1-2 projected onto the plane
    double xx = pts[3] - pts[0], xy = pts[4] - pts[1], xz = pts[5] - pts[2];
    double proj = xx * nx + xy * ny + xz * nz;
    xx -= proj * nx; xy -= proj * ny; xz -= proj * nz;
    double xLen = Math.Sqrt(xx * xx + xy * xy + xz * xz);
    if (xLen < 1e-12) throw new InvalidOperationException("Degenerate plane (zero-length edge 1-2).");
    xx /= xLen; xy /= xLen; xz /= xLen;

    // Local y axis = z' x x' (right-handed)
    double yx = ny * xz - nz * xy;
    double yy = nz * xx - nx * xz;
    double yz = nx * xy - ny * xx;

    rot[0, 0] = xx; rot[0, 1] = xy; rot[0, 2] = xz;
    rot[1, 0] = yx; rot[1, 1] = yy; rot[1, 2] = yz;
    rot[2, 0] = nx; rot[2, 1] = ny; rot[2, 2] = nz;
}

/// <summary>
/// Local -> global rotation matrix of a plane (columns = local axes expressed in global coords).
/// Same inputs as getPlaneRotMtrxGlobToLoc. v_global = rot * v_local
/// </summary>
internal static void getPlaneRotMtrxLocToGlob(double[] pts, double[,] rot)
{
        getPlaneRotMtrxGlobToLoc(pts, rot);
        double t;
        t = rot[0, 1]; rot[0, 1] = rot[1, 0]; rot[1, 0] = t;
        t = rot[0, 2]; rot[0, 2] = rot[2, 0]; rot[2, 0] = t;
        t = rot[1, 2]; rot[1, 2] = rot[2, 1]; rot[2, 1] = t;

}


}