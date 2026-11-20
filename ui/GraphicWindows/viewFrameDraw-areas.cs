using System.Data.Common;
using System.Drawing.Drawing2D;
using System.Net.PeerToPeer;
using static CEF.ModelDB;
using static System.Math;
using Font = System.Drawing.Font;
namespace CEF;
public partial class viewFrame
{
   
private void drawAreas(Brush brush,int [] conect,  Graphics g, Pen pen )
    
    {
        var selectedAreas = Selection.getAreasColection();
        var selectedAreasId = new HashSet<int>();

        if (selectedAreas != null)
         selectedAreasId = selectedAreas.Select(s=>Engine.getModel().areas[s].ID).ToHashSet();
  
            if(conect == null) return;
            var nAreas = conect.Length;
            if (nAreas == 0 ) return;


            switch (ms.viewRenderMode)
                {
                case 1:
                
                ///break;
                case 2:
                
                //break;

                case 0:
                default:                   


                    for(int i= 0;i<nAreas;i+=5) {
                        

                    var pen1 = pen;
                    var c =  ms.areaColor;
                    if(selectedAreasId.Contains(conect[i]))
                    
                    {   pen1 = new Pen(ms.areaSelectedColor);
                        c= ms.areaSelectedColor;
                    }

                draw3DFace( point3DColection[conect[i+1]],
                            point3DColection[conect[i+2]],
                            point3DColection[conect[i+3]],
                            point3DColection[conect[i+4]],
                            g,pen1, c);
               


                    }  
                    break;
                }
 
     }


   private void draw3DFace( double x1, double y1, double z1,
                            double x2, double y2, double z2,
                            double x3, double y3, double z3,
                            double x4, double y4, double z4,
                            Graphics g,Pen pen,Color c)
    {


        point3D p1,p2,p3,p4 ;
        p1=new();p2=new();p3=new();p4=new();
        p1.x3d=x1;p1.y3d=y1;p1.z3d=z1;
        p2.x3d=x2;p2.y3d=y2;p2.z3d=z2;
        p3.x3d=x3;p3.y3d=y3;p3.z3d=z3;
        p4.x3d=x4;p4.y3d=y4;p4.z3d=z4;
        
        p3DSet2DCoord(ref p1);
        p3DSet2DCoord(ref p2);
        p3DSet2DCoord(ref p3);
        p3DSet2DCoord(ref p4);

        
        Color cTrans = Color.FromArgb(128, c.R, c.G, c.B); 
        using (SolidBrush sb = new SolidBrush(cTrans))
        {
                    PointF[] quad =
                    {
                    new PointF(p1.x2d, p1.y2d),
                    new PointF(p2.x2d, p2.y2d),
                    new PointF(p3.x2d, p3.y2d),
                    new PointF(p4.x2d, p4.y2d)
                };
                    g.FillPolygon(sb, quad);
                    g.DrawPolygon(pen, quad); 
                
            
        }

    }

     private void draw3DFace(point3D p1, 
                             point3D p2, 
                             point3D p3, 
                             point3D p4, 
                            Graphics g,Pen pen,Color c)
    {

        Color cTrans = Color.FromArgb(128, c.R, c.G, c.B); 
        using (SolidBrush sb = new SolidBrush(cTrans))
        {
                    PointF[] quad =
                    {
                    new PointF(p1.x2d, p1.y2d),
                    new PointF(p2.x2d, p2.y2d),
                    new PointF(p3.x2d, p3.y2d),
                    new PointF(p4.x2d, p4.y2d)
                };
                    g.FillPolygon(sb, quad);
                    g.DrawPolygon(pen, quad);                
            
        }

    }

     // Draws the local axes (x, y, z) of the given areas at their origin (node 1).
    // Axis length = lenFactor * min(side 1-2, side 1-4), in model units.
    // Method of the canvas class, sibling of drawEdges (uses calc2DCoords, drawEdgesArrow, fontScale).
    private void drawAreaAxes(Graphics g, int[] areaIdx, double lenFactor = 0.3)
    {
        if (areaIdx == null || areaIdx.Length == 0) return;
        var model = Engine.getModel();

        using var penX = new Pen(Color.YellowGreen, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        using var penY = new Pen(Color.Cyan, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        using var penZ = new Pen(Color.Red, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        using var brush = new SolidBrush(Color.Cyan);
        using var font = new Font("ARIAL", fontScale);
        float head = (float)(fontScale / 2);

        var n = new int[4];
        var p = new double[12];
        var f = new double[9];
        var identity = new[] { 0, 1, 2, 3 };

        point3D pt(double x, double y, double z)
        {
            var q = new point3D();
            q.x3d = x; q.y3d = y; q.z3d = z;
            calc2DCoords(ref q);
            return q;
        }

        for (int i = 0; i < areaIdx.Length; i++)
        {
            AreaAxes.getNodes(model.areas[areaIdx[i]], n);
            if (!AreaAxes.getCoords(model, n, p)) continue;
            if (!AreaAxes.getFrame(p, identity, f)) continue;
            if (!levelValidation(p[0], p[1], p[2])) continue;

            double dx1 = p[3] - p[0], dy1 = p[4] - p[1], dz1 = p[5] - p[2];
            double dx2 = p[9] - p[0], dy2 = p[10] - p[1], dz2 = p[11] - p[2];
            double s1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1 + dz1 * dz1);
            double s2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2 + dz2 * dz2);
            double c = lenFactor * Math.Min(s1, s2);

            var e0 = pt(p[0], p[1], p[2]);
            var eX = pt(p[0] + c * f[0], p[1] + c * f[1], p[2] + c * f[2]);
            var eY = pt(p[0] + c * f[3], p[1] + c * f[4], p[2] + c * f[5]);
            var eZ = pt(p[0] + c * f[6], p[1] + c * f[7], p[2] + c * f[8]);

            drawEdgesArrow(eX, e0, head, penX, g);
            drawEdgesArrow(eY, e0, head, penY, g);
            drawEdgesArrow(eZ, e0, head, penZ, g);
            g.DrawString("x", font, brush, eX.x2d, eX.y2d);
            g.DrawString("y", font, brush, eY.x2d, eY.y2d);
            g.DrawString("z", font, brush, eZ.x2d, eZ.y2d);
        }

    }

}