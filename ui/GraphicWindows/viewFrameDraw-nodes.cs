using Microsoft.VisualBasic;
using System.Data.Common;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using static CEF.ModelDB;
using static System.Math;
using Font = System.Drawing.Font;
namespace CEF;
public partial class viewFrame
{
      //NODES
    private void drawNodes(Graphics currentCanvas, Font defaultFont, 
        Color barSelectedColor,Color barColor,Brush nodeBrush,
        float barLineSSize, Pen barLineStyle, 
        bool Mostrar_nudos_sel,bool Mostrar_nudos, bool MostrarIDNudos)
    {
        int i;
        int[] selectPoints = Selection.getNodesColection();
        float FontSize = defaultFont.Size * 0.6f;
        barLineStyle.Color = barSelectedColor;
        barLineStyle.Color = barColor;
        barLineStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        barLineStyle.Width = barLineSSize / 2;
        var snapLen = (int)SnapPoints.Length;
        for (i = 0; i < snapLen && (Mostrar_nudos || MostrarIDNudos); i++)
        {
            if (Mostrar_nudos && SnapPoints[i].IsNode && levelValidation(SnapPoints[i].x3d, SnapPoints[i].y3d, SnapPoints[i].z3d))
            {
                currentCanvas.DrawEllipse(barLineStyle, SnapPoints[i].x2d - FontSize / 2f, SnapPoints[i].y2d - FontSize / 2f, FontSize, FontSize);
            }
            if (MostrarIDNudos && SnapPoints[i].IsNode && levelValidation(SnapPoints[i].x3d, SnapPoints[i].y3d, SnapPoints[i].z3d))
            {
                        currentCanvas.DrawString($"{SnapPoints[i].Id}", defaultFont, nodeBrush, SnapPoints[i].x2d, SnapPoints[i].y2d);
            }
        }
        var selectedNodesCount = Selection.nodesMaxIndx;
        for (i = 0; i <= selectedNodesCount && Mostrar_nudos_sel; i++)
        {
            if (Mostrar_nudos_sel &&
                levelValidation(SnapPoints[selectPoints[i]].x3d, SnapPoints[selectPoints[i]].y3d, SnapPoints[selectPoints[i]].z3d))
            {
                currentCanvas.DrawString($"{SnapPoints[selectPoints[i]].Id}", defaultFont,
                    Brushes.DarkRed, SnapPoints[selectPoints[i]].x2d, SnapPoints[selectPoints[i]].y2d);
                currentCanvas.DrawEllipse(barLineStyle, SnapPoints[selectPoints[i]].x2d - FontSize / 2f,
                    SnapPoints[selectPoints[i]].y2d - FontSize /2f, FontSize, FontSize);
            }
        }}
       private void drawEdges(Graphics g)
    {
        var pen1 = new Pen(Color.YellowGreen, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        var pen2 = new Pen(Color.Cyan, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        var pen3 = new Pen(Color.Red, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        pen3.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        var c = 1;
        var eX = new point3D();
        var eY = new point3D();
        var eZ = new point3D();
        var e0 = new point3D();
        eX.x3d = c; eX.y3d = 0; eX.z3d = 0;
        eY.x3d = 0; eY.y3d = c; eY.z3d = 0;
        eZ.x3d = 0; eZ.y3d = 0; eZ.z3d = c;
        e0.x3d = 0; e0.y3d = 0; e0.z3d = 0;
        calc2DCoords(ref eX);
        calc2DCoords(ref eY);
        calc2DCoords(ref eZ);
        calc2DCoords(ref e0);
        drawEdgesArrow(eX, e0, (float)(fontScale / 2), pen1, g);
        drawEdgesArrow(eY, e0, (float)(fontScale / 2), pen2, g);
        drawEdgesArrow(eZ, e0, (float)(fontScale / 2), pen3, g);
        var brush = new SolidBrush(Color.Cyan);
        var font = new Font("ARIAL", fontScale);
        g.DrawString("x", font, brush, eX.x2d, eX.y2d);
        g.DrawString("y", font, brush, eY.x2d, eY.y2d);
        g.DrawString("z", font, brush, eZ.x2d, eZ.y2d);
    }
    private void drawEdgesArrow(point3D punta, point3D cola, float size, Pen estilo, Graphics g)
    {
        g.DrawLine(estilo, cola.x2d, cola.y2d, punta.x2d, punta.y2d);
        drawArrowhead(cola, punta, size, estilo, g);
    }
    private void drawNodesSupports(Graphics g, Pen pen)
    {
        const int STRIDE = 4;
        double[] nodeBuffer = new double[5 * STRIDE];
        float[] proj2DBuffer = new float[5 * STRIDE];
        Point[] polyPoints = new Point[5];
        float delta = 0.035f  ; //metesr //(float)(modelDiagonal / scaleFactor3 / 8.0);
		//TODO: Make an user option this value for supports dimensions. 
        void SetNode(int i, double x, double y, double z)
        {
            int o = i * STRIDE;
            nodeBuffer[o + 0] = 0.0;
            nodeBuffer[o + 1] = x;
            nodeBuffer[o + 2] = y;
            nodeBuffer[o + 3] = z;
        }
        void DrawSupportPolygon()
        {
            calc2DCoords(nodeBuffer, proj2DBuffer);
            for (int i = 0; i < 4; ++i)
            {
                int o = (i + 1) * STRIDE;
                polyPoints[i].X = (int)Round(proj2DBuffer[o + 1]);
                polyPoints[i].Y = (int)Round(proj2DBuffer[o + 2]);
            }
            polyPoints[4] = polyPoints[0];
            g.DrawPolygon(pen, polyPoints);
        }
        foreach (var restriccion in Engine.getModel().nodalRestraints)
        {
            int idxNodo = Engine.getModel().nodes.FindIndexByID(restriccion.ID);
            if (idxNodo < 0) continue;
            var nodo = Engine.getModel().nodes[idxNodo];
            if (!levelValidation(nodo.X, nodo.Y, nodo.Z)) continue;
            SetNode(0, nodo.X, nodo.Y, nodo.Z);
            if ( restriccion.rx == 1)
            {
                SetNode(1, nodo.X - delta, nodo.Y + delta, nodo.Z);
                SetNode(2, nodo.X + delta, nodo.Y + delta, nodo.Z);
                SetNode(3, nodo.X + delta, nodo.Y - delta, nodo.Z);
                SetNode(4, nodo.X - delta, nodo.Y - delta, nodo.Z);
                DrawSupportPolygon();
            }
            if ( restriccion.ry == 1)
            {
                SetNode(1, nodo.X, nodo.Y - delta, nodo.Z + delta);
                SetNode(2, nodo.X, nodo.Y + delta, nodo.Z + delta);
                SetNode(3, nodo.X, nodo.Y + delta, nodo.Z - delta);
                SetNode(4, nodo.X, nodo.Y - delta, nodo.Z - delta);
                DrawSupportPolygon();
            }
            if ( restriccion.rz == 1)
            {
                SetNode(1, nodo.X + delta, nodo.Y, nodo.Z - delta);
                SetNode(2, nodo.X + delta, nodo.Y, nodo.Z + delta);
                SetNode(3, nodo.X - delta, nodo.Y, nodo.Z + delta);
                SetNode(4, nodo.X - delta, nodo.Y, nodo.Z - delta);
                DrawSupportPolygon();
            }
            if (restriccion.rx == 0 &&  restriccion.ux == 1 )
            {
                SetNode(1, nodo.X - delta, nodo.Y + delta, nodo.Z);
                SetNode(2, nodo.X , nodo.Y , nodo.Z);
                SetNode(3, nodo.X , nodo.Y , nodo.Z);
                SetNode(4, nodo.X - delta, nodo.Y - delta, nodo.Z);
                DrawSupportPolygon();
            }
            if (restriccion.ry == 0 && restriccion.uy == 1 )
            {
                SetNode(1, nodo.X, nodo.Y - delta, nodo.Z + delta);
                SetNode(2, nodo.X, nodo.Y , nodo.Z );
                SetNode(3, nodo.X, nodo.Y , nodo.Z );
                SetNode(4, nodo.X, nodo.Y - delta, nodo.Z - delta);
                DrawSupportPolygon();
            }
            if (restriccion.rz == 0 &&  restriccion.uz == 1 )
            {
                SetNode(1, nodo.X + delta, nodo.Y, nodo.Z - delta);
                SetNode(2, nodo.X , nodo.Y, nodo.Z);
                SetNode(3, nodo.X , nodo.Y, nodo.Z);
                SetNode(4, nodo.X - delta, nodo.Y, nodo.Z - delta);
                DrawSupportPolygon();
            }
        }
    }
    private void drawNodeLoads(  Brush Paleta, Graphics g, Pen pen)
    {
        var Puntos = new Point[5];
        var AuxPuntos2D = new float[5, 3];
        var Nodos2 = new double[5, 4];
        var Nudo = new int[2];
        int activeLoadCase = Engine.getModel().getParamValue(paramDefaults.ActiveCase);
        maxLoadValue = getMaxQ();
        var p3d1 = new point3D();
        var p3d2= new point3D();
        foreach (var l in Engine.getModel().nodalLoads)
        {
            var nudo = Engine.getModel().nodes.First(x => x.ID == l.NodeID);
            p3d1.x3d = nudo.X;
            p3d1.y3d = nudo.Y;
            p3d1.z3d = nudo.Z;
            if (levelValidation(p3d1.x3d, p3d1.y3d, p3d1.z3d) )
            {
                if (l.Fx != 0)
                {
                    p3d2.x3d = p3d1.x3d + Math.Abs(l.Fx / maxLoadValue);
                    p3d2.y3d = p3d1.y3d;
                    p3d2.z3d = p3d1.z3d;
                    p3DSet2DCoord(ref p3d1);
                    p3DSet2DCoord(ref p3d2);
                    g.DrawLine(pen, p3d1.x2d, p3d1.y2d, p3d2.x2d, p3d2.y2d);
                    if (l.Fx > 0)
                    {
                        drawArrowhead(p3d1, p3d2, 6, pen, g);
                    }
                    else
                    {
                        drawArrowhead(p3d2, p3d1, 6, pen, g);
                    }
                }
                if (l.Fy != 0)
                {
                    p3d2.x3d = p3d1.x3d;
                    p3d2.y3d = p3d1.y3d + Math.Abs(l.Fy / maxLoadValue);
                    p3d2.z3d = p3d1.z3d;
                    p3DSet2DCoord(ref p3d1);
                    p3DSet2DCoord(ref p3d2);
                    g.DrawLine(pen, p3d1.x2d, p3d1.y2d, p3d2.x2d, p3d2.y2d);
                    if (l.Fy > 0)
                    {
                        drawArrowhead(p3d1, p3d2, 6, pen, g);
                    }
                    else
                    {
                        drawArrowhead(p3d2, p3d1, 6, pen, g);
                    }
                }
                if (l.Fz != 0)
                {
                    p3d2.x3d = p3d1.x3d; 
                    p3d2.y3d = p3d1.y3d;
                    p3d2.z3d = p3d1.z3d + Math.Abs(l.Fz / maxLoadValue);
                    p3DSet2DCoord(ref p3d1);
                    p3DSet2DCoord(ref p3d2);
                    g.DrawLine(pen, p3d1.x2d, p3d1.y2d, p3d2.x2d, p3d2.y2d);
                    if (l.Fz > 0)
                    {
                        drawArrowhead(p3d1, p3d2, 6, pen, g);
                    }
                    else
                    {
                        drawArrowhead(p3d2, p3d1, 6, pen, g);
                    }
                }
                if (l.Mx != 0)
                {
                    p3d2.x3d = p3d1.x3d + Math.Abs(l.Mx / maxLoadValue);
                    p3d2.y3d = p3d1.y3d;
                    p3d2.z3d = p3d1.z3d;
                    p3DSet2DCoord(ref p3d1);
                    p3DSet2DCoord(ref p3d2);
                    g.DrawLine(pen, p3d1.x2d, p3d1.y2d, p3d2.x2d, p3d2.y2d);
                    if (l.Mx > 0)
                    {
                        drawArrowhead(p3d1, p3d2, 6, pen, g);
                    }
                    else
                    {
                        drawArrowhead(p3d2, p3d1, 6, pen, g);
                    }
                }
                if (l.My != 0)
                {
                    p3d2.x3d = p3d1.x3d;
                    p3d2.y3d = p3d1.y3d + Math.Abs(l.My / maxLoadValue);
                    p3d2.z3d = p3d1.z3d;
                    p3DSet2DCoord(ref p3d1);
                    p3DSet2DCoord(ref p3d2);
                    g.DrawLine(pen, p3d1.x2d, p3d1.y2d, p3d2.x2d, p3d2.y2d);
                    if (l.My > 0)
                    {
                        drawArrowhead(p3d1, p3d2, 6, pen, g);
                    }
                    else
                    {
                        drawArrowhead(p3d2, p3d1, 6, pen, g);
                    }
                }
                if (l.Mz != 0)
                {
                    p3d2.x3d = p3d1.x3d;
                    p3d2.y3d = p3d1.y3d;       
                    p3d2.z3d = p3d1.z3d + Math.Abs(l.Mz / maxLoadValue);
                    p3DSet2DCoord(ref p3d1);
                    p3DSet2DCoord(ref p3d2);
                    g.DrawLine(pen, p3d1.x2d, p3d1.y2d, p3d2.x2d, p3d2.y2d);
                    if (l.Mz > 0)
                    {
                        drawArrowhead(p3d1, p3d2, 6, pen, g);
                    }
                    else
                    {
                        drawArrowhead(p3d2, p3d1, 6, pen, g);
                    }
                }
            }
        }
    }
    
void drawMFC( Brush Paleta, Graphics g, Pen pen)
{
var cz = 8f;
var rs = Engine.getModel().nodalConstraints;
foreach (var r in rs){
    var node_listIDs = r.nodesIDList.Replace(" ","")
                .Split(",",StringSplitOptions.RemoveEmptyEntries).
                ToArray().
                Select(int.Parse).ToArray();

        foreach(var n in node_listIDs){
            var nd = Engine.getModel().nodes.FindByID(n);
            var c3d = new point3D(){x3d=nd.X, y3d=nd.Y, z3d=nd.Z};
            p3DSet2DCoord(ref c3d);
            g.DrawEllipse(pen,c3d.x2d-cz/2,c3d.y2d-cz/2,cz,cz );
            g.DrawString(r.label, ms.defaultFont,Paleta,c3d.x2d,c3d.y2d);

        }

}

}



}