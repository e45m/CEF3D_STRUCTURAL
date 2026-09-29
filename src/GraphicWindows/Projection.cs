using static System.Math;
namespace CEF;
public class Projection
{
    private double p_, t_,A_,B_,C_;
    private const double po = -0.304402, to = -0.39634;
    private double far, near;
    internal double X0 { get; set; }
    internal double Y0 { get; set; }
    public Projection()
    {
        M_ = new double[] {  1, 0, 0 , 0, 1, 0 , 0, 0, 1  };
        M_1 = new double[] {  1, 0, 0 , 0, 1, 0 , 0, 0, 1  };
        X0 = 0;
        Y0 = 0;
        resetProjection();
    }
    public void resetProjection()
    {
        A = 0d;
        B = 0d;
        C = 0d;
        x = 0d;
        y = 0d;
        z = 0d;
        p = po;
        t = to;
        Scale = 100 * screenDiag / (modelDiag * Tan(projFocusAng / 2));
    }
    public double Scale { get; set; }
    public double p
    {
        get => p_; set
        {
            p_ = ((value > (-PI ) && value < PI )) ? value : p_;
            Sv = Sin(p_);
            Cv = Cos(p_);
        }
    }
    public double t { get => t_; set
        {
            t_ = ((value > (-PI * 2) && value < PI * 2)) ? value : 0d;
            Sh = Sin(t_);
            Ch = Cos(t_);
        }
    }
    public double modelDiag { get; set; }
    public double screenDiag { get; set; }
    public double projFocusAng { get; set; }
    public double A { get=>A_; set { A_ = value; Ca = Cos(A); Sa = Sin(A); } }
    public double B { get=>B_; set { B_ = value; Cb = Cos(B); Sb = Sin(B); } }
    public double C { get=>C_; set { C_ = value; Cc = Cos(C); Sc = Sin(C); } }
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
    private double[] M_;
    private double[] M_1;
    private double[] I;
    private bool isIvalid;
    double Ca; 
    double Sa ; 
    double Cb ;
    double Sb ;
    double Cc ;
    double Sc ;
    double Cv ;
    double Sv ;
    double Ch ;
    double Sh ;
    public double[] M { get { return M_; } }
    static double[] LookAt(double ex, double ey, double ez,
                            double tx, double ty, double tz,
                            double ux, double uy, double uz)
    {
        double fx = tx - ex;
        double fy = ty - ey;
        double fz = tz - ez;
        double flen = Math.Sqrt(fx * fx + fy * fy + fz * fz);
        if (flen == 0) flen = 1;
        fx /= flen; fy /= flen; fz /= flen;
        double sx = fy * uz - fz * uy;
        double sy = fz * ux - fx * uz;
        double sz = fx * uy - fy * ux;
        double slen = Math.Sqrt(sx * sx + sy * sy + sz * sz);
        if (slen == 0) slen = 1;
        sx /= slen; sy /= slen; sz /= slen;
        double ux2 = sy * fz - sz * fy;
        double uy2 = sz * fx - sx * fz;
        double uz2 = sx * fy - sy * fx;
        double[] V = {
          sx,   sy,   sz,  -(sx*ex + sy*ey + sz*ez) ,
         ux2,  uy2,  uz2,  -(ux2*ex + uy2*ey + uz2*ez) ,
         -fx,  -fy,  -fz,   (fx*ex + fy*ey + fz*ez)  ,
          0.0,  0.0,  0.0,   1.0 
    };
        return V;
    }
    double[] M__;
    public void setM()
    {
        double[] M_Tras = new double[] {    1d, 0d, 0d, x ,
                                            0d, 1d, 0d, y ,
                                            0d, 0d,1d , z,
                                            0d, 0d, 0d ,1d };
        double aspect = 1.0;
        double f = 1 / Tan(projFocusAng / 2);
         far =10f* modelDiag * f;
         near = far / 2;
        double nf = near - far;
        var rad = far;
        var ze = rad * Sv;
        var xe = rad * Cv * Ch;
        var ye = rad * Cv * Sh;
        var viewM = LookAt(-xe, -ye, -ze,
                            0, 0, 0,
                            0, 0, -1);
        double[] EscxScreenxP = new double[] {
            -(Scale * f / aspect), 0, -X0, 0,
            0, Scale * f, -Y0, 0,
            0, 0, (Scale * (far + near) / nf), (Scale * 2 * far * near / nf),
            0, 0, -1, 0
                };
        M__ = MatrixMath.M_mult(viewM,4,4, M_Tras, 4,4); 
        M__ = MatrixMath.M_mult(EscxScreenxP, 4, 4, M__, 4, 4); 
        M_ = M__;
    }
    public double[] getT() { return M_; }
    public double[] getTinv()  { return M_1; }
    public double getFar() { return far; }
    public double getNear() { return near; }
    public void project(double[] worldCoord, ref double[] screenCoord)
    {
        screenCoord = MatrixMath.M_mult(M_,4,4, new double[] {  
                                            worldCoord[0]  ,
                                            worldCoord[1] ,
                                            worldCoord[2] ,
                                            1d     },4,1);
        screenCoord[0] = screenCoord[0] /screenCoord[3]; 
        screenCoord[1] = screenCoord[1] /screenCoord[3];
        screenCoord[2] = screenCoord[2] /screenCoord[3];
    }
    public void unProject(ref double[] worldCoord,  double[] screenCoord)
    {
        M_1 = MatrixMath.Invert(M_, 4); 
        worldCoord = MatrixMath.M_mult(M_1, 4,4,new double[] {  
                                            screenCoord[0]  ,
                                            screenCoord[1] ,
                                            screenCoord[2] ,
                                            1d     },4,1 );
        worldCoord[0] = worldCoord[0] / worldCoord[3];
        worldCoord[1] = worldCoord[1] /worldCoord[3];
        worldCoord[2] = worldCoord[2] /worldCoord[3];
    }
}