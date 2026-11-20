
#if !__CEFTYPES__
#define __CEFTYPES__



public struct point3D
    {
        public int Id;
        public float x2d;
        public float y2d;
        public float z2d;
        public double x3d;
        public double y3d;
        public double z3d;
        public bool IsNode;

            // 1. Sobrecarga del operador ==
    public static bool operator ==(point3D p1, point3D p2)
    {
        return p1.Id == p2.Id;
    }

    // 2. Sobrecarga del operador != (Obligatorio)
    public static bool operator !=(point3D p1, point3D p2)
    {
        return !(p1 == p2);
    }

    // 3. Anular Equals para mantener consistencia (Obligatorio)
    public override bool Equals(object obj)
    {
        return obj is point3D other && this == other;
    }

    // 4. Anular GetHashCode para colecciones como Dictionary o HashSet (Obligatorio)
    public override int GetHashCode()
    {
         return Id.GetHashCode();
    }

        public  bool IsNear(point3D p,double square_tol=0.01)
    {        
        var dx = x3d-p.x3d;
        var dy = y3d-p.y3d;
        var dz = z3d-p.z3d;
        var diff = dx*dx+dy*dy+dz*dz;
        return diff <=square_tol;
    }

      public  bool IsNear(double x, double y, double z,double square_tol=0.01)
    {        
        var dx = x3d-x;
        var dy = y3d-y;
        var dz = z3d-z;
        var diff = dx*dx+dy*dy+dz*dz;
        return diff <=square_tol;
    }

    }
public struct CEFRectangle 
{
public float Top, Left, Right, Bottom, Height,Width;
 public  bool Contains(point3D p) =>
        ( p.x2d<=Right) && (p.x2d>=Left) && (p.y2d>=Top) && (p.y2d<=Bottom) ;
 public  bool Contains(float x, float y) =>
        ( x<=Right) && (x>=Left) && (y>=Top) && (y<=Bottom) ;
}



#endif