using System.Data.Common;
using System.Drawing.Drawing2D;
using static CEF.ModelDB;
using static System.Math;
using Font = System.Drawing.Font;
namespace CEF;
public partial class viewFrame
{
    private void drawSelectBars(Graphics g, Color barSelectedColor, float barLineW, Pen barLineStyle, Font drawFont )
    {
        int[] Barras_Seleccionadas = Selection.getBarsColection();
        if (Barras_Seleccionadas == null) return;
        var barCount = Selection.barMaxIndex;
        barLineStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        barLineStyle.Color = barSelectedColor;
        barLineStyle.Width = barLineW * 1.2f;
		
		// Font drawFont = new Font("Arial", 9);
		SolidBrush drawBrush = new SolidBrush(barSelectedColor);
		
        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
        {
            for (int i = 0; i <= barCount; i++)
            {
                int idx = Barras_Seleccionadas[i];
                int p1_idx = barConecty[idx * 3 + 1];
                int p2_idx = barConecty[idx * 3 + 2];
                var p1 = SnapPoints[p1_idx];
                var p2 = SnapPoints[p2_idx];
                if (levelValidation(p1.x3d, p1.y3d, p1.z3d) &&
                    levelValidation(p2.x3d, p2.y3d, p2.z3d))
                {
					var text = $"{barConecty[idx * 3 + 0]}";
					g.DrawString(text,drawFont, drawBrush, (p1.x2d+p2.x2d)/2, (p1.y2d+p2.y2d)/2);
                    path.AddLine(p1.x2d, p1.y2d, p2.x2d, p2.y2d);
                    path.StartFigure(); 
                }
            }
            if (path.PointCount > 0)
            {
                g.DrawPath(barLineStyle, path);
            }
        }
    }

 
    private void drawBars(Graphics g, Pen pen)
    {
      
            resetCanvasGeometry();

                switch (ms.viewRenderMode)
                {
                case 1:
                draw3DPathFacesBatch( g, pen);
                break;
                case 2:
                draw3DPathWireBatch(g, pen);
                break;

                case 0:
                default:
                drawLinesBatch(g, pen);
                break;
                }
            
        }

    


	private void drawLinesBatch(Graphics g, Pen pen)
	{
		var barCount = Engine.getModel().bars.Count ;
		
        for (int i = 0; i < barCount; i++)
        {
            
			var IndxI = barConecty[i*3+ 1];
            var IndxJ = barConecty[i*3+ 2];
			var barra = Engine.getModel().bars[i];
            if (levelValidation(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d) &&
                levelValidation(point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d))
            {
             g.DrawLine(pen, point3DColection[IndxI].x2d, point3DColection[IndxI].y2d, point3DColection[IndxJ].x2d, point3DColection[IndxJ].y2d);
				
				
			}		
			
		}
		
	}



	private void draw3DPathWireBatch(Graphics g, Pen pen)
	{
		var barCount = Engine.getModel().bars.Count ;

		
        for (int i = 0; i < barCount; i++)
        {
			var IndxI = barConecty[i*3+ 1];
            var IndxJ = barConecty[i*3+ 2];
			var barra = Engine.getModel().bars[i];
			
            if (levelValidation(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d) &&
                levelValidation(point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d))
            {
                var secc = Engine.getModel().sections.First(s => s.ID == barra.SectionID);				
				var pv4 = CadToolsUtils.calSecProfile(secc,6, barra.offsetY, barra.offsetZ);
				draw3DPathWire(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d,
				point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d, barra.Rotation, pv4, ms.sectionsPerElement, g, pen);
			}		
			
		}
		
	}

	

    private void draw3DPathFacesBatch(Graphics g, Pen pen)
	{
	var barCount = Engine.getModel().bars.Count ;

        for (int i = 0; i < barCount; i++)        
        {	
			var IndxI = barConecty[i*3+ 1];
            var IndxJ = barConecty[i*3+ 2];
			var barra = Engine.getModel().bars[i];
            if (levelValidation(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d) &&
                levelValidation(point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d))
            {
                var secc = Engine.getModel().sections.First(s => s.ID == barra.SectionID);				
                var pv1 = CadToolsUtils.calSecProfile(secc, 1,barra.offsetY, barra.offsetZ);
				
				draw3DPathFaces(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d,
                point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d, barra.Rotation, pv1, ms.sectionsPerElement, g, pen);
				
			}		
			
		}
		
	}


    private void draw3DPathWire(double x1, double y1, double z1,
                             double x2, double y2, double z2,
                             double giro, double[] per, int N,
                             Graphics g, Pen pen)
    {
        var l = per.Length;
        int profileCount = l / 3;
        int validCount = 0;
        for (int i = 0; i < profileCount; i++)
            if (per[i * 3] != double.MaxValue)
                validCount++;
        point3D[] p3list = new point3D[(N + 1) * validCount];
        double[] vectPath = new double[] { x1, y1, z1, x2, y2, z2 };
        Parallel.For(0, N + 1, j =>
        {
            double[] loc = new double[3];
            double[] glob = new double[3];
            int idx = 0; 
            for (int i = 0; i < l; i += 3)
            {
                if (per[i] == double.MaxValue)
                    continue;
                loc[0] = per[i + 0];
                loc[1] = per[i + 1];
                loc[2] = per[i + 2];
                if (giro != 0.0)
                {
                    var loct = new double[3];
                    double ang = giro * Math.PI / 180.0;
                    double[,] tr = {
                    { 1.0, 0.0, 0.0 },
                    { 0.0, Math.Cos(ang),  Math.Sin(ang) },
                    { 0.0, -Math.Sin(ang), Math.Cos(ang) }
                };
                    for (int ii = 0; ii < 3; ii++)
                    {
                        loct[ii] = 0;
                        for (int jj = 0; jj < 3; jj++)
                            loct[ii] += tr[ii, jj] * loc[jj];
                    }
                    loc = loct;
                }
                space3d.loc2glob(vectPath, ref glob, loc);
                var p = new point3D();
                p.x3d = glob[0] + x1 + (x2 - x1) * j / N;
                p.y3d = glob[1] + y1 + (y2 - y1) * j / N;
                p.z3d = glob[2] + z1 + (z2 - z1) * j / N;
                p.x2d = 0;
                p.y2d = 0;
                p3DSet2DCoord(ref p);
                int pos = j * validCount + idx;
                p3list[pos] = p;
                idx++;
            }
        });
        for (int j = 0; j <= N; j++)
        {
            int baseIndex = j * validCount;
            float xs = 0, ys = 0;
            bool first = true;
            for (int k = 0; k < validCount; k++)
            {
                var p = p3list[baseIndex + k];
                float xe = p.x2d;
                float ye = p.y2d;
                if (!first)
                    g.DrawLine(pen, xs, ys, xe, ye);
                xs = xe;
                ys = ye;
                first = false;
            }
        }
        for (int k = 0; k < validCount; k++)
        {
            for (int j = 1; j <= N; j++)
            {
                var p0 = p3list[(j - 1) * validCount + k];
                var p1 = p3list[j * validCount + k];
                g.DrawLine(pen, p0.x2d, p0.y2d, p1.x2d, p1.y2d);
            }
        }
    }
    private void draw3DPathFaces(double x1, double y1, double z1,
                            double x2, double y2, double z2,
                            double giro, double[] per, int N,
                            Graphics g, Pen pen)
    {
        var l = per.Length;
        int profileCount = l / 3;
        int validCount = 0;
        for (int i = 0; i < profileCount; i++)
            if (per[i * 3] != double.MaxValue)
                validCount++;
        point3D[] p3list = new point3D[(N + 1) * validCount];
        double[] vectPath = new double[] { x1, y1, z1, x2, y2, z2 };
        double[] loc = new double[3];
        double[] glob = new double[3];
        int m = 0;
        for (int j = 0; j <= N; j++)
        {
            for (int i = 0; i < l; i += 3)
            {
                if (per[i] == double.MaxValue)
                    continue;
                loc[0] = per[i + 0];
                loc[1] = per[i + 1];
                loc[2] = per[i + 2];
                if (giro != 0.0)
                {
                    var loct = new double[3];
                    double ang = giro * Math.PI / 180.0;
                    double[,] tr = {
                    { 1.0, 0.0, 0.0 },
                    { 0.0, Math.Cos(ang),  Math.Sin(ang) },
                    { 0.0, -Math.Sin(ang), Math.Cos(ang) }
                };
                    for (int ii = 0; ii < 3; ii++)
                    {
                        loct[ii] = 0;
                        for (int jj = 0; jj < 3; jj++)
                            loct[ii] += tr[ii, jj] * loc[jj];
                    }
                    loc = loct;
                }
                space3d.loc2glob(vectPath, ref glob, loc);
                var px = new point3D();
                px.x3d = glob[0] + x1 + (x2 - x1) * j / N;
                px.y3d = glob[1] + y1 + (y2 - y1) * j / N;
                px.z3d = glob[2] + z1 + (z2 - z1) * j / N;
                p3DSet2DCoord(ref px);
                p3list[m++] = px;
            }
        }
        Color c = ms.barColor;
        Color cTrans = Color.FromArgb(128, c.R, c.G, c.B); 
        using (SolidBrush sb = new SolidBrush(cTrans))
        {
            for (int j = 0; j < N; j++)
            {
                int baseA = j * validCount;
                int baseB = (j + 1) * validCount;
                for (int k = 0; k < validCount - 1; k++)
                {
                    var pA1 = p3list[baseA + k];
                    var pA2 = p3list[baseA + k + 1];
                    var pB1 = p3list[baseB + k];
                    var pB2 = p3list[baseB + k + 1];
                    PointF[] quad =
                    {
                    new PointF(pA1.x2d, pA1.y2d),
                    new PointF(pA2.x2d, pA2.y2d),
                    new PointF(pB2.x2d, pB2.y2d),
                    new PointF(pB1.x2d, pB1.y2d)
                };
                    g.FillPolygon(sb, quad);
                    g.DrawPolygon(pen, quad); 
                }
            }
        }
        for (int j = 0; j <= N; j++)
        {
            int baseIndex = j * validCount;
            for (int k = 0; k < validCount - 1; k++)
            {
                var p0 = p3list[baseIndex + k];
                var p1 = p3list[baseIndex + k + 1];
                g.DrawLine(pen, p0.x2d, p0.y2d, p1.x2d, p1.y2d);
            }
        }
        for (int k = 0; k < validCount; k++)
        {
            for (int j = 1; j <= N; j++)
            {
                var p0 = p3list[(j - 1) * validCount + k];
                var p1 = p3list[j * validCount + k];
                g.DrawLine(pen, p0.x2d, p0.y2d, p1.x2d, p1.y2d);
            }
        }
    }
	
	
    private void drawBarsDistrLoads(  Brush brush, Graphics g, Pen pen)
    {
        int i;
        var loadsX = new double[2];
        var loadsY = new double[2];
        var loadsZ = new double[2];
        int activeLoadCase = Engine.getModel().getParamValue(paramDefaults.ActiveCase); ;
        double MaxQ = getMaxQ();
        double FREDC2 = ((double)ms.defaultFont.Size/20 )  / (MaxQ);
        var dloadsCount = Engine.getModel().barDistrLoads.Count;
        for (i = 0; i < dloadsCount; i++)
        {    
            barDistrLoad load = Engine.getModel().barDistrLoads[i];
            
            if (load.Case == activeLoadCase)
            {
                loadsX[0] = load.Fx;
                loadsX[1] = load.Fx;
                loadsY[0] = load.Fy;
                loadsY[1] = load.Fy;
                loadsZ[0] = load.Fz;
                loadsZ[1] = load.Fz;
                switch (load.Type)
                {
                    case 1:
                        {
                            drawBarLocalLoad(load.BarID, FREDC2, 1, loadsX, "X", brush, g, Pens.LightGreen,load.l1,load.l2);
                            drawBarLocalLoad(load.BarID, FREDC2, 1, loadsY, "Y", brush, g, Pens.LightGreen,load.l1,load.l2);
                            drawBarLocalLoad(load.BarID, FREDC2, 1, loadsZ, "Z", brush, g, Pens.LightGreen,load.l1,load.l2);
                            break;                               
                        }                                        
                    case 0:                                      
                        {                                        
                            drawBarGlobalLoad(load.BarID, FREDC2,1, loadsX, "X", brush, g, Pens.LightGreen,load.l1,load.l2);
                            drawBarGlobalLoad(load.BarID, FREDC2,1, loadsY, "Y", brush, g, Pens.LightGreen,load.l1,load.l2);
                            drawBarGlobalLoad(load.BarID, FREDC2,1, loadsZ, "Z", brush, g, Pens.LightGreen,load.l1,load.l2);
                            break;
                        }
                }
            }
        }
    }
    private void drawBarsPunctLoads(  Brush brush, Graphics g, Pen pen)
    {
        int i;
        int indBarra;
        int IndA, IndB;
        double dx, dy, dz;
        point3D auxp = default, auxpX, auxpY, auxpZ;
        double a;
        var dirVect = new double[6];
        int activeLoadCase = Engine.getModel().getParamValue(paramDefaults.ActiveCase);
        double MaxQ = getMaxQ();
        double FREDC2 = 1d / MaxQ;
        var barsLoadsCount = Engine.getModel().barPunctLoads.Count;
        for (i = 0; i < barsLoadsCount; i++)
        {
            barPunctLoad load = Engine.getModel().barPunctLoads[i]; 
            if (load.Case == activeLoadCase)
            {
                indBarra = Engine.getModel().bars.FindIndexByID(load.BarID);
                a = load.l1;
                IndA = Engine.getModel().bars[indBarra].startNode;
                IndB = Engine.getModel().bars[indBarra].endNode;
                IndA = Engine.getModel().nodes.FindIndexByID(IndA);
                IndB = Engine.getModel().nodes.FindIndexByID(IndB);
                dx = Engine.getModel().nodes[IndB].X - Engine.getModel().nodes[IndA].X;
                dy = Engine.getModel().nodes[IndB].Y - Engine.getModel().nodes[IndA].Y;
                dz = Engine.getModel().nodes[IndB].Z - Engine.getModel().nodes[IndA].Z;
                auxp.x3d = Engine.getModel().nodes[IndA].X + dx * a;
                auxp.y3d = Engine.getModel().nodes[IndA].Y + dy * a;
                auxp.z3d = Engine.getModel().nodes[IndA].Z + dz * a;
                p3DSet2DCoord(ref auxp); 
                auxpX = auxp;
                auxpY = auxp;
                auxpZ = auxp;
                if (load.Type == 1) 
                {
                    dirVect[0] = Engine.getModel().nodes[IndA].X;
                    dirVect[1] = Engine.getModel().nodes[IndA].Y;
                    dirVect[2] = Engine.getModel().nodes[IndA].Z;
                    dirVect[3] = Engine.getModel().nodes[IndB].X;
                    dirVect[4] = Engine.getModel().nodes[IndB].Y;
                    dirVect[5] = Engine.getModel().nodes[IndB].Z;
                    auxpX = p3DGlobFromLoc(auxpX, dirVect);
                    auxpY = p3DGlobFromLoc(auxpY, dirVect);
                    auxpZ = p3DGlobFromLoc(auxpZ, dirVect);
                }
                auxpX.x3d -= load.Fx * FREDC2;
                auxpY.y3d -= load.Fy * FREDC2;
                auxpZ.z3d -= load.Fz * FREDC2;
                if (load.Type == 1)
                {
                    auxpX = p3DLocFromGlob(auxpX, dirVect);
                    auxpY = p3DLocFromGlob(auxpY, dirVect);
                    auxpZ = p3DLocFromGlob(auxpZ, dirVect);
                }
                if (load.Type == 0)
                {
                    p3DSet2DCoord(ref auxpX);
                    p3DSet2DCoord(ref auxpY);
                    p3DSet2DCoord(ref auxpZ);
                }
                if (load.Fx != 0)
                    drawArrow(auxp, auxpX, 7f, pen, g);
                if (load.Fy != 0)
                    drawArrow(auxp, auxpY, 7f, pen, g);
                if (load.Fz != 0)
                    drawArrow(auxp, auxpZ, 7f, pen, g);
                auxpX = auxp;
                auxpY = auxp;
                auxpZ = auxp;
                if (load.Type == 1)
                {
                    auxpX = p3DGlobFromLoc(auxpX, dirVect);
                    auxpY = p3DGlobFromLoc(auxpY, dirVect);
                    auxpZ = p3DGlobFromLoc(auxpZ, dirVect);
                }
                auxpX.x3d = auxpX.x3d - load.Mx * FREDC2;
                auxpY.y3d = auxpY.y3d - load.My * FREDC2;
                auxpZ.z3d = auxpZ.z3d - load.Mz * FREDC2;
                if (load.Type == 1)
                {
                    auxpX = p3DLocFromGlob(auxpX, dirVect);
                    auxpY = p3DLocFromGlob(auxpY, dirVect);
                    auxpZ = p3DLocFromGlob(auxpZ, dirVect);
                }
                if (load.Type == 0)
                {
                    p3DSet2DCoord(ref auxpX);
                    p3DSet2DCoord(ref auxpY);
                    p3DSet2DCoord(ref auxpZ);
                }
                if (load.Mx != 0)
                    drawArrow(auxp, auxpX, 7f, pen, g);
                if (load.My != 0)
                    drawArrow(auxp, auxpY, 7f, pen, g);
                if (load.Mz != 0)
                    drawArrow(auxp, auxpZ, 7f, pen, g);
            }
        }
    }
    private void drawBarLocalLoad(int barID, double FREDC, int segments, double[] valu, string edge,
    Brush brush, Graphics g, Pen pen, double a = 0d, double b = 1d)
{
    var barIndx = Engine.getModel().bars.FindIndexByID(barID);
    var startNodeIndx = Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[barIndx].startNode);
    var endNodeIndx = Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[barIndx].endNode);

    double startX = Engine.getModel().nodes[startNodeIndx].X;
    double startY = Engine.getModel().nodes[startNodeIndx].Y;
    double startZ = Engine.getModel().nodes[startNodeIndx].Z;

    double endX = Engine.getModel().nodes[endNodeIndx].X;
    double endY = Engine.getModel().nodes[endNodeIndx].Y;
    double endZ = Engine.getModel().nodes[endNodeIndx].Z;

    if (!levelValidation(startX, startY, startZ) || !levelValidation(endX, endY, endZ))
        return;

    double dx = startX - endX;
    double dy = startY - endY;
    double dz = startZ - endZ;

    double L = Sqrt(dx * dx + dy * dy + dz * dz);
    segments = (int)Min(Max(1, L / 0.5), 10);

    int st = (int)(a * segments);
    int en = (int)(b * segments);

    // Arreglos mínimos requeridos por firma externa de API (space3d)
    double[] globRef = new double[] { startX, startY, startZ, endX, endY, endZ };
    double[] globNodes = new double[] { startX, startY, startZ, startX, startY, startZ, endX, endY, endZ, endX, endY, endZ };
    double[] loc = new double[12];

    for (int ii = st; ii < en; ii++)
    {
        // Reseteo de nodos globales del segmento
        globNodes[0] = startX; globNodes[1] = startY; globNodes[2] = startZ;
        globNodes[3] = startX; globNodes[4] = startY; globNodes[5] = startZ;
        globNodes[6] = endX;   globNodes[7] = endY;   globNodes[8] = endZ;
        globNodes[9] = endX;   globNodes[10] = endY;  globNodes[11] = endZ;

        space3d.glob2loc(globRef, globNodes, ref loc);

        double localL = loc[6] - loc[0];
        double segStep = localL / segments;

        double loc0 = loc[0] + segStep * ii;
        double loc3 = loc0;
        double loc6 = loc0 + segStep;
        double loc9 = loc6;

        double loc1 = loc[1];
        double loc2 = loc[2];
        double loc4 = loc[4];
        double loc5 = loc[5];
        double loc7 = loc[7];
        double loc8 = loc[8];
        double loc10 = loc[10];
        double loc11 = loc[11];

        double of1 = Math.Abs(valu[0]) * FREDC;
        double of2 = Math.Abs(valu[1]) * FREDC;

        switch (edge ?? "")
        {
            case "X":
                loc3 += of1;
                loc6 += of2;
                break;
            case "Y":
                loc4 += of1;
                loc7 += of2;
                break;
            case "Z":
                loc5 += of1;
                loc8 += of2;
                break;
        }

        loc[0] = loc0; loc[1] = loc1; loc[2] = loc2;
        loc[3] = loc3; loc[4] = loc4; loc[5] = loc5;
        loc[6] = loc6; loc[7] = loc7; loc[8] = loc8;
        loc[9] = loc9; loc[10] = loc10; loc[11] = loc11;

        space3d.loc2glob(globRef, ref globNodes, loc);

        var segmentNodes = new double[]
        {
            0d, globNodes[0], globNodes[1], globNodes[2],
            0d, globNodes[3], globNodes[4], globNodes[5],
            0d, globNodes[6], globNodes[7], globNodes[8],
            0d, globNodes[9], globNodes[10], globNodes[11]
        };

        drawSegment(segmentNodes, brush, g, pen, valu[0]);
    }
}
    private void drawBarGlobalLoad(int barID, double FREDC, int segments, double[] valu, string edge,
    Brush brush, Graphics g, Pen pen, double a = 0d, double b = 1d)
{
    var barIndx = Engine.getModel().bars.FindIndexByID(barID);
    var startNodeIndx = Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[barIndx].startNode);
    var endNodeIndx = Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[barIndx].endNode);

    double startX = Engine.getModel().nodes[startNodeIndx].X;
    double startY = Engine.getModel().nodes[startNodeIndx].Y;
    double startZ = Engine.getModel().nodes[startNodeIndx].Z;

    double endX = Engine.getModel().nodes[endNodeIndx].X;
    double endY = Engine.getModel().nodes[endNodeIndx].Y;
    double endZ = Engine.getModel().nodes[endNodeIndx].Z;

    double vx = endX - startX;
    double vy = endY - startY;
    double vz = endZ - startZ;

    double L = Sqrt(Pow(vx, 2d) + Pow(vy, 2d) + Pow(vz, 2d));

    double g0 = startX ;
    double g1 = startY ;
    double g2 = startZ ;

    if (!levelValidation(g0, g1, g2) | !levelValidation(endX, endY, endZ))
        return;

    segments = (int)Min(Max(1, L / 0.5), 10);
    int st = (int)a * segments;
    int en = (int)b * segments;

    double stepX = vx / segments;
    double stepY = vy / segments;
    double stepZ = vz / segments;
	
	g0-=stepX;
	g1-=stepY;
	g2-=stepZ;

    for (int ii = st; ii < en; ii++)
    {
        g0 += stepX;
        g1 += stepY;
        g2 += stepZ;

        double g3 = g0;
        double g4 = g1;
        double g5 = g2;

        double g9 = g0 + stepX;
        double g10 = g1 + stepY;
        double g11 = g2 + stepZ;

        double g6 = g9;
        double g7 = g10;
        double g8 = g11;

        double of1 = Math.Abs(valu[0]) * FREDC;
        double of2 = Math.Abs(valu[1]) * FREDC;

        switch (edge ?? "")
        {
            case "X":
                g3 += of1;
                g6 += of2;
                break;
            case "Y":
                g4 += of1;
                g7 += of2;
                break;
            case "Z":
                g5 += of1;
                g8 += of2;
                break;
        }

        var segmentNodes = new double[]
        {
            0d, g0, g1, g2,
            0d, g3, g4, g5,
            0d, g6, g7, g8,
            0d, g9, g10, g11
        };

        drawSegment(segmentNodes, brush, g, pen, valu[0]);
    }
}
    private readonly float[] _auxPoints = new float[20];
    private readonly PointF[] _points = new PointF[5];
    private void drawSegment(double[] nodes3d, Brush brush, Graphics g, Pen pen, double sign, bool drawArrow= true)
    {
        calc2DCoords(nodes3d, _auxPoints);
        _points[0].X = _auxPoints[1]; _points[0].Y = _auxPoints[2];
        _points[1].X = _auxPoints[5]; _points[1].Y = _auxPoints[6];
        _points[2].X = _auxPoints[9]; _points[2].Y = _auxPoints[10];
        _points[3].X = _auxPoints[13]; _points[3].Y = _auxPoints[14];
        _points[4].X = _auxPoints[1]; _points[4].Y = _auxPoints[2];
        g.DrawLines(pen, _points);
        if(drawArrow)
        if (sign > 0)
            drawArrowOptimized(_auxPoints[1], _auxPoints[2], _auxPoints[5], _auxPoints[6], pen, g);
        else if (sign < 0)
            drawArrowOptimized(_auxPoints[5], _auxPoints[6], _auxPoints[1], _auxPoints[2], pen, g);
    }
    private void drawArrowOptimized(float xStart, float yStart, float xEnd, float yEnd, Pen pen, Graphics g)
    {
        float dx = xEnd - xStart;
        float dy = yEnd - yStart;
        float l = (float)Math.Sqrt(dx * dx + dy * dy);
        if (l > 5f) 
        {
            float sz = 6.0f;
            float ux = dx / l;
            float uy = dy / l;
            float bx = xEnd - ux * sz;
            float by = yEnd - uy * sz;
            float vx = -uy * sz * 0.5f;
            float vy = ux * sz * 0.5f;
            g.DrawLine(pen, xEnd, yEnd, bx + vx, by + vy);
            g.DrawLine(pen, xEnd, yEnd, bx - vx, by - vy);
        }
    }

    int loadedCase = -1;
    Dictionary<int, int> fid;
    private void drawDeformedShape(Graphics g, Pen pen)
    {
        if (Engine.getModel().nodalDisplacements is  null ||( Engine.getModel().nodalDisplacements.Count == 0)) return;
       double deformScale = ms.deformScale;   
       double rotScale = ms.rotScale;   
        var casoAc = Engine.getModel().getParamValue(paramDefaults.ActiveCase);
        var nudos = Engine.getModel().nodes;
        var desp = Engine.getModel().nodalDisplacements.Where(d => d.Case == casoAc).ToArray();
        var barras = Engine.getModel().bars;
        if ( casoAc != loadedCase || fid == null||fid.Count!=desp.Length+1)
        {
            fid = [];
            for (int i = 0; i < desp.Length; i++)
            {
                fid.Add(desp[i].NodeID,i);
            }
            loadedCase = casoAc;
        }
        foreach (var barra in barras)
        {
            int i = nudos.FindIndexByID(barra.startNode);
            int j = nudos.FindIndexByID(barra.endNode);
            if (i < 0 || j < 0) continue;
            var ni = nudos[i];
            var nj = nudos[j];
            var di = desp[fid[ni.ID]];
            var dj = desp[fid[nj.ID]];
            if (di == null || dj == null) continue;
            double x1 = ni.X + di.dx * deformScale;
            double y1 = ni.Y + di.dy * deformScale;
            double z1 = ni.Z + di.dz * deformScale;
            double x2 = nj.X + dj.dx * deformScale;
            double y2 = nj.Y + dj.dy * deformScale;
            double z2 = nj.Z + dj.dz * deformScale;
            double dx1 = di.rx * rotScale;
            double dy1 = di.ry * rotScale;
            double dz1 = di.rz * rotScale;
            double dx2 = dj.rx * rotScale;
            double dy2 = dj.ry * rotScale;
            double dz2 = dj.rz * rotScale;
            double X = nj.X - ni.X;
            double Y = nj.Y - ni.Y;
            double Z = nj.Z - ni.Z;
            draw3DCurveSecment(
                x1, y1, z1,
                dx1, dy1, dz1,
                x2, y2, z2,
                dx2, dy2, dz2,
                X, Y, Z,
                pen, g
            );
        }
    }

    private void drawArrow(point3D head, point3D tail, float size, Pen pen, Graphics g)
    {
        if (levelValidation(head.x3d, head.y3d, head.z3d))
        {
            g.DrawLine(pen, tail.x2d, tail.y2d, head.x2d, head.y2d);
            drawArrowhead(tail, head, size, pen, g);
        }
    }
    private void drawArrowhead(point3D cen, point3D head, float size, Pen pen, Graphics g)
    {
        float dx = head.x2d - cen.x2d;
        float dy = head.y2d - cen.y2d;
        float l = (float)Math.Sqrt(dx * dx + dy * dy);
        if (l > 0)
        {
            float ux = dx / l;
            float uy = dy / l;
            float vx = -uy;
            float vy = ux;
            float base_x = head.x2d - ux * size;
            float base_y = head.y2d - uy * size;
            PointF p1 = new PointF(base_x + vx * size * 0.5f, base_y + vy * size * 0.5f);
            PointF p2 = new PointF(base_x - vx * size * 0.5f, base_y - vy * size * 0.5f);
            PointF p3 = new PointF(head.x2d, head.y2d);
            g.DrawLine(pen, p1, p3);
            g.DrawLine(pen, p2, p3);
        }
    }
    private void drawLink(point3D pA, point3D pB, float size, Pen pen, Graphics g)
    {
        double dx = pB.x2d - pA.x2d;
        double dy = pB.y2d - pA.y2d;
        double len = Math.Sqrt(dx * dx + dy * dy);
        if (len <= double.Epsilon) return;
        double ux = dx / len;
        double uy = dy / len;
        double nx = -uy;
        double ny = ux;
        int Segments = 4;
        double step = size;
        Point prev = ToPoint(pB.x2d, pB.y2d);
        for (int i = 1; i <= Segments; i++)
        {
            double t1 = i * step;
            double sign = (i % 2 == 0) ? 1.0 : -1.0;
            Point p = ToPoint(
                pB.x2d + ux * t1 + nx * size * sign,
                pB.y2d + uy * t1 + ny * size * sign
            );
            g.DrawLine(pen, prev, p);
            prev = p;
        }
        Point basePoint = ToPoint(
            pB.x2d + ux * step,
            pB.y2d + uy * step
        );
        g.DrawRectangle(
            pen,
            basePoint.X - size / 2f,
            basePoint.Y - size / 2f,
            2f * size,
            2f * size
        );
    }
    private static Point ToPoint(double x, double y)
    {
        return new Point(
            (int)Math.Round(x),
            (int)Math.Round(y)
        );
    }
   
    private void draw3DCurveSecment(
        double x1, double y1, double z1,
        double dx1, double dy1, double dz1,
        double x2, double y2, double z2,
        double dx2, double dy2, double dz2,
        double X, double Y, double Z,
        Pen pen, Graphics g)
    {
        int n = deformedStations;
        const int S = 4;
        double[] p3D = new double[(n + 1) * S];
        float[] p2D = new float[(n + 1) * S];
        Point[] pts = new Point[n + 1];
        double L = Sqrt(X * X + Y * Y + Z * Z);
        double fx = X / L, fy = Y / L, fz = Z / L;
        double fi1x = dy1 * fz + dz1 * fy;
        double fi1y = -(dz1 * fx + dx1 * fz);
        double fi1z = dx1 * fy + dy1 * fx;
        double dfx = dy2 * fz + dz2 * fy - fi1x;
        double dfy = -(dz2 * fx + dx2 * fz) - fi1y;
        double dfz = dx2 * fy + dy2 * fx - fi1z;
        double dx = x2 - x1, dy = y2 - y1, dz = z2 - z1;
        double invL2 = 1.0 / (L * L);
        double invL3 = invL2 / L;
        double C4x = x1, C3x = fi1x;
        double C4y = y1, C3y = fi1y;
        double C4z = z1, C3z = fi1z;
        double C2x = -3 * invL2 * (dfx * L / 3 - dx + fi1x * L);
        double C2y = -3 * invL2 * (dfy * L / 3 - dy + fi1y * L);
        double C2z = -3 * invL2 * (dfz * L / 3 - dz + fi1z * L);
        double C1x = invL3 * (dfx * L - 2 * (dx - fi1x * L));
        double C1y = invL3 * (dfy * L - 2 * (dy - fi1y * L));
        double C1z = invL3 * (dfz * L - 2 * (dz - fi1z * L));
        double dt = L / n;
        for (int i = 0; i <= n; ++i)
        {
            double t = dt * i;
            double t2 = t * t;
            double t3 = t2 * t;
            int o = i * S;
            p3D[o + 1] = C1x * t3 + C2x * t2 + C3x * t + C4x;
            p3D[o + 2] = C1y * t3 + C2y * t2 + C3y * t + C4y;
            p3D[o + 3] = C1z * t3 + C2z * t2 + C3z * t + C4z;
        }
        calc2DCoords(p3D, p2D);
        for (int i = 0; i <= n; ++i)
        {
            int o = i * S;
            pts[i].X = (int)Round(p2D[o + 1]);
            pts[i].Y = (int)Round(p2D[o + 2]);
        }
        g.DrawLines(pen, pts);
    }
    private void draw3DLine(point3D Inicio, point3D Fin, Pen estilo, Graphics g)
    {
        p3DSet2DCoord(ref Inicio);
        p3DSet2DCoord(ref Fin);
        g.DrawLine(estilo, Inicio.x2d, Inicio.y2d, Fin.x2d, Fin.y2d);
    }
    static Color colorFormDouble(double val, double max, double min)
    {
        if (val > max) val = max;
        if (val < min) val = min;
        if (max == min) max++;
        var nc = 255 * 255 * 255 * (val - min) / (max - min);
        var r = (int)((nc / 255 * 255) % 255);
        var g = (int)((nc / 255) % 255);
        var b = (int)((nc) % 255);
        var c = Color.FromArgb(255, r, g, b); ;
        return c;
    }
    static SolidBrush setBrightFormDouble(SolidBrush sb, double val, double max, double min)
    {
        var lsb = sb.Color;
        if (max == min) max++;
        var f = (val - min) / (max - min); ;
        var c = Color.FromArgb(lsb.A,
            (int)(lsb.R * f),
            (int)(lsb.G * f),
            (int)(lsb.B * f));
        var b = new SolidBrush(c);
        return b;
    }

    // Draws the local axes (x, y, z) of the given bars, at the bar midpoint (or at node i if atMid is false).
    // Axis length = lenFactor * bar length, in model units.
    // Method of the canvas class, sibling of drawEdges (uses calc2DCoords, drawEdgesArrow, fontScale).
    private void drawBarAxes(Graphics g, int[] barIdx, double lenFactor = 0.15, bool atMid = true)
    {
        if (barIdx == null || barIdx.Length == 0) return;
        var model = Engine.getModel();

        using var penX = new Pen(Color.YellowGreen, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        using var penY = new Pen(Color.Cyan, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        using var penZ = new Pen(Color.Red, 3f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        using var brush = new SolidBrush(Color.Cyan);
        using var font = new Font("ARIAL", fontScale);
        float head = (float)(fontScale / 2);

        var f = new double[9];

        point3D pt(double x, double y, double z)
        {
            var q = new point3D();
            q.x3d = x; q.y3d = y; q.z3d = z;
            calc2DCoords(ref q);
            return q;
        }

        for (int i = 0; i < barIdx.Length; i++)
        {
            var bar = model.bars[barIdx[i]];
            if (!BarAxes.getFrame(model, bar.startNode, bar.endNode, f)) continue;
            BarAxes.applyRoll(f, BarAxes.getRoll(bar));

            int a = model.nodes.FindIndexByID(bar.startNode), b = model.nodes.FindIndexByID(bar.endNode);
            double xi = model.nodes[a].X, yi = model.nodes[a].Y, zi = model.nodes[a].Z;
            double dx = model.nodes[b].X - xi, dy = model.nodes[b].Y - yi, dz = model.nodes[b].Z - zi;
            double c = lenFactor * Math.Sqrt(dx * dx + dy * dy + dz * dz);
            double ox = atMid ? xi + dx / 2d : xi, oy = atMid ? yi + dy / 2d : yi, oz = atMid ? zi + dz / 2d : zi;
            if (!levelValidation(ox, oy, oz)) continue;

            var e0 = pt(ox, oy, oz);
            var eX = pt(ox + c * f[0], oy + c * f[1], oz + c * f[2]);
            var eY = pt(ox + c * f[3], oy + c * f[4], oz + c * f[5]);
            var eZ = pt(ox + c * f[6], oy + c * f[7], oz + c * f[8]);

            drawEdgesArrow(eX, e0, head, penX, g);
            drawEdgesArrow(eY, e0, head, penY, g);
            drawEdgesArrow(eZ, e0, head, penZ, g);
            g.DrawString("x", font, brush, eX.x2d, eX.y2d);
            g.DrawString("y", font, brush, eY.x2d, eY.y2d);
            g.DrawString("z", font, brush, eZ.x2d, eZ.y2d);
        }
    }

void drawBarRels( Brush Paleta, Graphics g, Pen pen)
{
var cz = 12f;
var bs = Engine.getModel().bars;
var node_listIDs = new List<int>();
foreach (var b in bs){

    if(b.releases>0){
        
        var left =(b.releases >> 6)>0;
        var rigth =(b.releases & 0x3F)>0;
        if(left) node_listIDs.Add(b.startNode);
        if(rigth) node_listIDs.Add(b.endNode);

        foreach(var n in node_listIDs){
            var nd = Engine.getModel().nodes.FindByID(n);
            var c3d = new point3D(){x3d=nd.X, y3d=nd.Y, z3d=nd.Z};
            p3DSet2DCoord(ref c3d);
            g.DrawEllipse(pen,c3d.x2d-cz/2,c3d.y2d-cz/2,cz,cz );
            g.DrawEllipse(pen,c3d.x2d-cz/4,c3d.y2d-cz/4,cz/2,cz/2);

        }
        node_listIDs.Clear();
    }
}


}

}