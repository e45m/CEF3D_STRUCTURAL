using static System.Math;
namespace CEF;
public class Projection
{
    private float p_, t_,A_,B_,C_;
    private const float po = -0.304402f, to = -0.39634f;
    private float far, near;
    internal float X0 { get; set; }
    internal float Y0 { get; set; }
	internal const float  fPI = (float)PI;
	private  int Farfactor = 1;
	
    public Projection()
    {
        M_ = new float[] {  1, 0, 0 , 0, 1, 0 , 0, 0, 1  };
        M_1 = new float[] {  1, 0, 0 , 0, 1, 0 , 0, 0, 1  };
        X0 = 0;
        Y0 = 0;
        resetProjection();
    }
	
	public void  setOtho() {Farfactor = 10;}
	public void  setIso() {Farfactor = 1;}
	
	
	
    public void resetProjection()
    {
        A = 0f;
        B = 0f;
        C = 0f;
        x = 0f;
        y = 0f;
        z = 0f;
        p = po;
        t = to;
        Scale = 100*Farfactor * screenDiag / (modelDiag * (float)Tan(projFocusAng / 2));
    }
    public float Scale { get; set; }
    public float p
    {
        get => p_; set
        {
            p_ = ((value > (-fPI ) && value < fPI )) ? value : p_;
            Sv =(float) Sin(p_);
            Cv =(float) Cos(p_);
        }
    }
    public float t { get => t_; set
        {
            t_ = ((value > (-fPI * 2) && value < fPI * 2)) ? value : 0f;
            Sh = (float)Sin(t_);
            Ch = (float)Cos(t_);
        }
    }
    public float modelDiag { get; set; }
    public float screenDiag { get; set; }
    public float projFocusAng { get; set; }
    public float A { get=>A_; set { A_ = value; Ca = (float)Cos(A); Sa = (float)Sin(A); } }
    public float B { get=>B_; set { B_ = value; Cb = (float)Cos(B); Sb = (float)Sin(B); } }
    public float C { get=>C_; set { C_ = value; Cc = (float)Cos(C); Sc = (float)Sin(C); } }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    private float[] M_;
    private float[] M_1;

    float Ca; 
    float Sa ; 
    float Cb ;
    float Sb ;
    float Cc ;
    float Sc ;
    float Cv ;
    float Sv ;
    float Ch ;
    float Sh ;
    public float[] M { get { return M_; } }
	
    static float[] LookAt(float ex, float ey, float ez,
                            float tx, float ty, float tz,
                            float ux, float uy, float uz)
    {
        float fx = tx - ex;
        float fy = ty - ey;
        float fz = tz - ez;
        float flen =(float) Math.Sqrt(fx * fx + fy * fy + fz * fz);
        if (flen == 0) flen = 1;
        fx /= flen; fy /= flen; fz /= flen;
        float sx = fy * uz - fz * uy;
        float sy = fz * ux - fx * uz;
        float sz = fx * uy - fy * ux;
        float slen =(float) Math.Sqrt(sx * sx + sy * sy + sz * sz);
        if (slen == 0) slen = 1;
        sx /= slen; sy /= slen; sz /= slen;
        float ux2 = sy * fz - sz * fy;
        float uy2 = sz * fx - sx * fz;
        float uz2 = sx * fy - sy * fx;
        float[] V = {
          sx,   sy,   sz,  -(sx*ex + sy*ey + sz*ez) ,
         ux2,  uy2,  uz2,  -(ux2*ex + uy2*ey + uz2*ez) ,
         -fx,  -fy,  -fz,   (fx*ex + fy*ey + fz*ez)  ,
          0.0f,  0.0f,  0.0f,   1.0f
    };
        return V;
    }
    float[] M__;
    public void setM()
    {
		//These parameter tunning controls de z-order preition. 
		//Todo togle betewwn ortho and iso. far*=10 esc*=10
        float[] M_Tras = new float[] {      1f, 0f, 0f, x ,
                                            0f, 1f, 0f, y ,
                                            0f, 0f, 1f , z,
                                            0f, 0f, 0f ,1f };
        float aspect = 1.0f;
		
		//projFocusAng = 45*fPI/180;
		
        float f = 1 / (float)Tan(projFocusAng / 2);
         far =10f*Farfactor* modelDiag * f;
         near = far*0.2f;
        float nf = near - far;
        var rad = far;
        var ze = rad * Sv;
        var xe = rad * Cv * Ch;
        var ye = rad * Cv * Sh;
        var viewM = LookAt(-xe, -ye, -ze,
                            0, 0, 0,
                            0, 0, -1);
        float[] EscxScreenxP = new float[] {
            -(Scale * f / aspect), 0, -X0, 0,
            0, Scale * f, -Y0, 0,
            0, 0, (Scale * (far + near) / nf), (Scale * 2 * far * near / nf),
            0, 0, -1, 0
                };
        M__ = MatrixMath.M_mult(viewM,4,4, M_Tras, 4,4); 
        M__ = MatrixMath.M_mult(EscxScreenxP, 4, 4, M__, 4, 4); 
        M_ = M__;
    }
    public float[] getT() { return M_; }
    public float[] getTinv()  { return M_1; }
    public float getFar() { return far; }
    public float getNear() { return near; }
	
    public void project(double[] worldCoord, ref float[] screenCoord)
    {
        screenCoord = MatrixMath.M_mult(M_,4,4, new float[] {  
                                            (float)worldCoord[0]  ,
                                            (float)worldCoord[1] ,
                                            (float)worldCoord[2] ,
                                            1f     },4,1);
        screenCoord[0] = screenCoord[0] /screenCoord[3]; 
        screenCoord[1] = screenCoord[1] /screenCoord[3];
        screenCoord[2] = screenCoord[2] /screenCoord[3];
    }
    public void unProject(ref double[] worldCoord,  float[] screenCoord)
    {
		//Can use Linq becuse this function is only used for zoom in 
        M_1 = MatrixMath.Invert(M_.Select(m=>((double)m)).ToArray(), 4).Select(m=>((float)m)).ToArray(); 
        worldCoord = MatrixMath.M_mult(M_1, 4,4,new float[] {  
                                            (float)screenCoord[0]  ,
                                            (float)screenCoord[1] ,
                                            (float)screenCoord[2] ,
                                            1f     },4,1 )
											.Select(m=>((double)m)).ToArray();
        worldCoord[0] = worldCoord[0] /worldCoord[3];
        worldCoord[1] = worldCoord[1] /worldCoord[3];
        worldCoord[2] = worldCoord[2] /worldCoord[3];
    }
}