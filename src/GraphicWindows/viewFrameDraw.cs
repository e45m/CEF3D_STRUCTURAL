using System.Drawing.Drawing2D;
using static CEF.ModelDB;
using static System.Math;
using Font = System.Drawing.Font;
namespace CEF;
public partial class viewFrame
{
    Bitmap _bmpA;
    Bitmap _bmpB;
    Bitmap _front;          
    bool _useA = true;
    bool _dirty = true;     
    public void InvalidateScene()
    {
        _dirty = true;
    }
    public Bitmap backStageDrawScene(int width, int height)
    {
        if (!_dirty && _front != null)
            return _front;
        if (_bmpA == null || _bmpA.Width != width || _bmpA.Height != height)
        {
            _bmpA?.Dispose();
            _bmpB?.Dispose();
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            _bmpA = new Bitmap(width, height);
            _bmpB = new Bitmap(width, height);
            _dirty = true;
        }
        Bitmap drawBmp = _useA ? _bmpA : _bmpB;
        using (Graphics g = Graphics.FromImage(drawBmp))
        {
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.Clear(Color.White);
            DrawScene(g);
        }
        _useA = !_useA;
        _front = drawBmp;
        _dirty = false;
        return _front;
    }
    private void DrawScene(Graphics currentCanvas)
    {
        currentCanvas.Clear(ms.canvColor);
        if( this.activeVF== VFTyes.Active)
        {
            currentCanvas.DrawRectangle(new Pen(ms.deformedShapeColor,3f), 
                Canvas.Left+5, Canvas.Top+5,
                Canvas.Width-5, Canvas.Height-5);
        }else if(this.activeVF == VFTyes.NoActive)
        {

            currentCanvas.DrawRectangle(new Pen(ms.gridLinesColor, 3f),
                Canvas.Left + 5, Canvas.Top + 5,
                Canvas.Width - 5, Canvas.Height - 5);

        }


        fontScale = (float)(10d * projection.Scale); 
        fontScale = fontScale < 4.0f ? 4.0f : fontScale < 10.0f ? fontScale : 10.0f;
        loadEscFact = fontScale / (screenDiagonal / 2d);        
        Pen loadpen = new Pen(ms.loadColorBrush);
        var nodeBrush = new SolidBrush(ms.nodeBrushColor);
        var barLineStyle = new Pen(ms.barLineSColor, ms.barLineSSize);
        space3d.nodesCount = pointsCount; 
        if (graphsChange) SetSnapPoints();
        Pen gridDefaultStyle;
        gridDefaultStyle = new Pen(ms.gridLinesColor, ms.gridLineW)
        { DashStyle = ms.GridLineStyle };
        if (ms.showGrid)
        {
            drawGrid(SnapPoints, modelDiagonal / 50d, currentCanvas, gridDefaultStyle);
        }
        barLineStyle.Color = ms.barColor;
        barLineStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        barLineStyle.Width = ms.barLineW;
        drawEdges(currentCanvas);
        drawBars(currentCanvas, barLineStyle);
        drawNodes(currentCanvas, ms.defaultFont,
         ms.barSelectedColor, ms.barColor,  nodeBrush,
         ms.barLineSSize,  barLineStyle,
        ms.ShowSelectedNodes, ms.showNodes, ms.showNodesID);
        barLineStyle.Width = ms.barLineSSize / 4; 
        if (ms.showBarsLoads)
        {
            drawBarsDistrLoads(ms.loadColorBrush, currentCanvas, loadpen);
            drawBarsPunctLoads(ms.loadColorBrush, currentCanvas, loadpen);
        }
        float FontSize = ms.defaultFont.Size * 0.6f;
        var barLoadPenStyle = new Pen(ms.loadPenColor, FontSize / 2) { Width = ms.barLineW / 2 };
        if (ms.showNodalLoads)
        {
            drawNodeLoads(ms.loadColorBrush, currentCanvas, barLoadPenStyle);
        }
        var deformedPenStyle = new Pen(ms.deformedShapeColor, ms.barLineW / 1.5f);
        if (ms.showDeformedShape)
        {
            drawDeformedShape(currentCanvas, deformedPenStyle);
        }
        barLineStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        barLineStyle.Width = ms.barLineW / 3;
        var EstiloApoyos = new SolidBrush(ms.SuppColorBrush); 
        drawNodesSupports( currentCanvas, barLineStyle);
        drawSelectBars( currentCanvas, ms.barSelectedColor, ms.barLineW,  barLineStyle);
    }
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
        }
    }
    private void drawSelectBars(Graphics g, Color barSelectedColor, float barLineW, Pen barLineStyle)
    {
        barLineStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        barLineStyle.Color = barSelectedColor;
        barLineStyle.Width = barLineW * 1.2f;
        int[] Barras_Seleccionadas = Selection.getBarsColection();
        if (Barras_Seleccionadas == null) return;
        var barCount = Selection.barMaxIndex;
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
                    path.AddLine((float)p1.x2d, (float)p1.y2d, (float)p2.x2d, (float)p2.y2d);
                    path.StartFigure(); 
                }
            }
            if (path.PointCount > 0)
            {
                g.DrawPath(barLineStyle, path);
            }
        }
    }

    private void drawGrid(point3D[] sP, double delta, Graphics g, Pen gridPen)
    {
        var Cx = new List<Coordinate>();
        var Cy = new List<Coordinate>();
        var Cz = new List<Coordinate>();
        for (int i = 0; i < App.model.gridCoordsX.Count; i++)
        {
            if ((App.model.gridCoordsX[i].coordSys ) == (App.currCoordSist ))
                Cx.Add(App.model.gridCoordsX[i]);
        }
        for (int i = 0; i < App.model.gridCoordsY.Count; i++)
        {
            if ((App.model.gridCoordsY[i].coordSys ) == (App.currCoordSist ))
                Cy.Add(App.model.gridCoordsY[i]);
        }
        for (int i = 0; i < App.model.gridCoordsZ.Count; i++)
        {
            if ((App.model.gridCoordsZ[i].coordSys ) == (App.currCoordSist ))
                Cz.Add(App.model.gridCoordsZ[i]);
        }
        Cx.Sort((a, b) => a.Coord.CompareTo(b.Coord));
        Cy.Sort((a, b) => a.Coord.CompareTo(b.Coord));
        Cz.Sort((a, b) => a.Coord.CompareTo(b.Coord));
        double maxX = Cx[Cx.Count - 1].Coord;
        double minX = Cx[0].Coord;
        double maxY = Cy[Cy.Count - 1].Coord;
        double minY = Cy[0].Coord;
        double maxZ = Cz[Cz.Count - 1].Coord;
        double minZ = Cz[0].Coord;
        point3D p1 = default, p2 = default;
        for (int i = 0; i < Cz.Count; i++)
        {
            for (int j = 0; j < Cy.Count; j++)
            {
                for (int k = 0; k < Cx.Count; k++)
                {
                    p1.x3d = Cx[k].Coord;
                    p1.y3d = maxY;
                    p1.z3d = Cz[i].Coord;
                    p2.x3d = p1.x3d;
                    p2.y3d = minY;
                    p2.z3d = p1.z3d;
                    draw3DLine(p1, p2, gridPen, g);
                    p1.x3d = maxX;
                    p1.y3d = Cy[j].Coord;
                    p1.z3d = Cz[i].Coord;
                    p2.x3d = minX;
                    p2.y3d = p1.y3d;
                    p2.z3d = p1.z3d;
                    draw3DLine(p1, p2, gridPen, g);
                    p1.x3d = Cx[k].Coord;
                    p1.y3d = Cy[j].Coord;
                    p1.z3d = maxZ;
                    p2.x3d = p1.x3d;
                    p2.y3d = p1.y3d;
                    p2.z3d = minZ;
                    draw3DLine(p1, p2, gridPen, g);
                }
            }
        }
    }
    public void resetCanvasGeometry()
    {
        point3DColection = new point3D[App.model.nodes.Count*3];
        var maxIters = App.model.bars.Count*3 ;
        barConecty = new int[maxIters];
        int indx = -1;
        for (int i = 0; i < maxIters; i+=3)
        {
            indx = (int)(i/3);
            barConecty[i+ 0] = App.model.bars[indx].ID;
            barConecty[i + 1] = App.model.nodes.FindIndexByID(App.model.bars[indx].startNode);
            barConecty[i+ 2] =  App.model.nodes.FindIndexByID(App.model.bars[indx].endNode);
        }
        maxIters = App.model.nodes.Count;
        for (int i = 0; i < maxIters; i++)
        {
            point3DColection[i].x3d = App.model.nodes[i].X;
            point3DColection[i].y3d = App.model.nodes[i].Y;
            point3DColection[i].z3d = App.model.nodes[i].Z;
            p3DSet2DCoord(ref point3DColection[i]);
        }
    }
    private void drawBars(Graphics g, Pen pen)
    {
        int i;
            resetCanvasGeometry();
        var barCount = App.model.bars.Count ;
        for (i = 0; i < barCount; i++)
        {
            var IndxI = barConecty[i*3+ 1];
            var IndxJ = barConecty[i*3+ 2];
            if (levelValidation(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d) &&
                levelValidation(point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d))
            {
                var barra = App.model.bars[i];
                switch (ms.viewRenderMode)
                {
                    case 1:
                    case 2:
                        var secc = App.model.Sections.First(s => s.ID == barra.SectionID);
                        switch (ms.viewRenderMode)
                        {
                            case 1:
                                var pv1 = cadTools.calSecProfile(secc, 1,barra.offsetY, barra.offsetZ);
                                draw3DPathFaces(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d,
                                point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d, barra.Rotation, pv1, ms.sectionsPerElement, g, pen);
                                break;
                            case 2:
                                var pv4 = cadTools.calSecProfile(secc,6, barra.offsetY, barra.offsetZ);
                                draw3DPathWire(point3DColection[IndxI].x3d, point3DColection[IndxI].y3d, point3DColection[IndxI].z3d,
                                    point3DColection[IndxJ].x3d, point3DColection[IndxJ].y3d, point3DColection[IndxJ].z3d, barra.Rotation, pv4, ms.sectionsPerElement, g, pen);
                                break;
                        }
                        break;
                    case 0:
                    default:
                        g.DrawLine(pen, point3DColection[IndxI].x2d, point3DColection[IndxI].y2d, point3DColection[IndxJ].x2d, point3DColection[IndxJ].y2d);
                        break;
                }
            }
        }
        if (ms.viewRenderMode == 1)
        {
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
        int activeLoadCase = App.model.getParamValue(Mdta.ActiveCase); ;
        double MaxQ = getMaxQ();
        double FREDC2 = ((double)ms.defaultFont.Size/20 )  / (MaxQ);
        var dloadsCount = App.model.barDistrLoads.Count;
        for (i = 0; i < dloadsCount; i++)
        {    
            barDistrLoad load = App.model.barDistrLoads[i];
            
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
                            drawBarLocalLoad(load.BarID, FREDC2, 1, loadsX, "X", brush, g, Pens.LightGreen,load.a,load.b);
                            drawBarLocalLoad(load.BarID, FREDC2, 1, loadsY, "Y", brush, g, Pens.LightGreen,load.a,load.b);
                            drawBarLocalLoad(load.BarID, FREDC2, 1, loadsZ, "Z", brush, g, Pens.LightGreen,load.a,load.b);
                            break;                               
                        }                                        
                    case 0:                                      
                        {                                        
                            drawBarGlobalLoad(load.BarID, FREDC2,1, loadsX, "X", brush, g, Pens.LightGreen,load.a,load.b);
                            drawBarGlobalLoad(load.BarID, FREDC2,1, loadsY, "Y", brush, g, Pens.LightGreen,load.a,load.b);
                            drawBarGlobalLoad(load.BarID, FREDC2,1, loadsZ, "Z", brush, g, Pens.LightGreen,load.a,load.b);
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
        int activeLoadCase = App.model.getParamValue(Mdta.ActiveCase);
        double MaxQ = getMaxQ();
        double FREDC2 = 1d / MaxQ;
        var barsLoadsCount = App.model.barPunctLoads.Count;
        for (i = 0; i < barsLoadsCount; i++)
        {
            barPunctLoad load = App.model.barPunctLoads[i]; 
            if (load.Case == activeLoadCase)
            {
                indBarra = App.model.bars.FindIndexByID(load.BarID);
                a = load.a;
                IndA = App.model.bars[indBarra].startNode;
                IndB = App.model.bars[indBarra].endNode;
                IndA = App.model.nodes.FindIndexByID(IndA);
                IndB = App.model.nodes.FindIndexByID(IndB);
                dx = App.model.nodes[IndB].X - App.model.nodes[IndA].X;
                dy = App.model.nodes[IndB].Y - App.model.nodes[IndA].Y;
                dz = App.model.nodes[IndB].Z - App.model.nodes[IndA].Z;
                auxp.x3d = App.model.nodes[IndA].X + dx * a;
                auxp.y3d = App.model.nodes[IndA].Y + dy * a;
                auxp.z3d = App.model.nodes[IndA].Z + dz * a;
                p3DSet2DCoord(ref auxp); 
                auxpX = auxp;
                auxpY = auxp;
                auxpZ = auxp;
                if (load.Type == 1) 
                {
                    dirVect[0] = App.model.nodes[IndA].X;
                    dirVect[1] = App.model.nodes[IndA].Y;
                    dirVect[2] = App.model.nodes[IndA].Z;
                    dirVect[3] = App.model.nodes[IndB].X;
                    dirVect[4] = App.model.nodes[IndB].Y;
                    dirVect[5] = App.model.nodes[IndB].Z;
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
    private void drawBarLocalLoad(int barID, double FREDC, int segments, double[] valu, string edge
      , Brush brush, Graphics g, Pen pen, double a=0d, double b = 1d)
    {
        var BarsConect = new int[3];
        int ii;
        var nodes = new double[12];
        var nodes4x4 = new double[4* 4];
        double[] loc = new double[12], glob = new double[6];
        double L, dz, dy;
        BarsConect[0] = App.model.bars.FindIndexByID(barID);
        BarsConect[1] = App.model.nodes.FindIndexByID(App.model.bars[BarsConect[0]].startNode);
        BarsConect[2] = App.model.nodes.FindIndexByID(App.model.bars[BarsConect[0]].endNode);
        glob[0] = App.model.nodes[BarsConect[1]].X;
        glob[1] = App.model.nodes[BarsConect[1]].Y;
        glob[2] = App.model.nodes[BarsConect[1]].Z;
        glob[3] = App.model.nodes[BarsConect[2]].X;
        glob[4] = App.model.nodes[BarsConect[2]].Y;
        glob[5] = App.model.nodes[BarsConect[2]].Z;
        if (!levelValidation(glob[0], glob[1], glob[2]) | !levelValidation(glob[3], glob[4], glob[5]))
            return;
        L = Sqrt((glob[0] - glob[3])* (glob[0] - glob[3]) + (glob[1] - glob[4]) * (glob[1] - glob[4]) + (glob[2] - glob[5]) * (glob[2] - glob[5]));
        segments =(int)Min(Max( 1, L / 0.5),10);
        int st = (int )(a * segments);
        int en = (int )(b * segments);

        for (ii = st; ii < en; ii++)
        {
            nodes[0] = glob[0];
            nodes[1] = glob[1];
            nodes[2] = glob[2];
            nodes[3] = glob[0];
            nodes[4] = glob[1];
            nodes[5] = glob[2];
            nodes[6] = glob[3];
            nodes[7] = glob[4];
            nodes[8] = glob[5];
            nodes[9] = glob[3];
            nodes[10] = glob[4];
            nodes[11] = glob[5];
            space3d.glob2loc(glob, nodes, ref loc);
            L = loc[6] - loc[0];
            dz = loc[5] - loc[2];
            dy = loc[7] - loc[1];
            loc[0] = loc[0] + L / segments * ii;
            loc[3] = loc[0];
            loc[6] = loc[0] + L / segments;
            loc[9] = loc[6];
            var of1 = Math.Abs(valu[0]) * FREDC;
            var of2 = Math.Abs(valu[1]) * FREDC;
            switch (edge )
            {
                case "X":
                    {
                        loc[3] += of1;
                        loc[6] += of2;
                        break;
                    }
                case "Y":
                    {
                        loc[4] += of1;
                        loc[7] += of2;
                        break;
                    }
                case "Z":
                    {
                        loc[5] += of1;
                        loc[8] += of2;
                        break;
                    }
            }
            space3d.loc2glob(glob, ref nodes, loc);
            nodes4x4[0] = 0d;
            nodes4x4[1] = nodes[0];
            nodes4x4[2] = nodes[1];
            nodes4x4[3] = nodes[2];
            nodes4x4[4] = 0d;
            nodes4x4[5] = nodes[3];
            nodes4x4[6] = nodes[4];
            nodes4x4[7] = nodes[5];
            nodes4x4[8] = 0d;
            nodes4x4[9] = nodes[6];
            nodes4x4[10] = nodes[7];
            nodes4x4[11] = nodes[8];
            nodes4x4[12] = 0d;
            nodes4x4[13] = nodes[9];
            nodes4x4[14] = nodes[10];
            nodes4x4[15] = nodes[11];
            drawSegment( nodes4x4, brush, g, pen, valu[0]);
        }
    }
    private void drawBarGlobalLoad(int barID, double FREDC, int segments, double[] valu, string edge,
        Brush brush, Graphics g, Pen pen, double a = 0d, double b = 1d)
    {
        var barsCon = new int[3];
        int ii;
        var nodes = new double[4* 4];
        var glob = new double[12];
        double L;
        double[] V = new double[3];
            barsCon[0] = App.model.bars.FindIndexByID(barID);
            barsCon[1] = App.model.nodes.FindIndexByID(App.model.bars[barsCon[0]].startNode);
            barsCon[2] = App.model.nodes.FindIndexByID(App.model.bars[barsCon[0]].endNode);
            glob[0] = App.model.nodes[barsCon[1]].X;
            glob[1] = App.model.nodes[barsCon[1]].Y;
            glob[2] = App.model.nodes[barsCon[1]].Z;
            glob[3] = glob[0];
            glob[4] = glob[1];
            glob[5] = glob[2];
            glob[6] = App.model.nodes[barsCon[2]].X;
            glob[7] = App.model.nodes[barsCon[2]].Y;
            glob[8] = App.model.nodes[barsCon[2]].Z;
            glob[9] = glob[6];
            glob[10] = glob[7];
            glob[11] = glob[8];
            V[0] = glob[6] - glob[3];
            V[1] = glob[7] - glob[4];
            V[2] = glob[8] - glob[5];
            L = Sqrt(Pow(V[0], 2d) + Pow(V[1], 2d) + Pow(V[2], 2d));
            glob[0] -= V[0] / segments;
            glob[1] -= V[1] / segments;
            glob[2] -= V[2] / segments;
            if (!levelValidation(glob[0], glob[1], glob[2]) | !levelValidation(glob[6], glob[7], glob[8]))
                return;
        segments = (int)Min(Max(1, L / 0.5), 10);
        int st = (int)a * segments;
        int en = (int)b * segments;

        for (ii = st; ii < en; ii++)
        {
                glob[0] += V[0] / segments;
                glob[1] += V[1] / segments;
                glob[2] += V[2] / segments;
                glob[3] = glob[0];
                glob[4] = glob[1];
                glob[5] = glob[2];
                glob[9] = glob[0] + V[0] / segments;
                glob[10] = glob[1] + V[1] / segments;
                glob[11] = glob[2] + V[2] / segments;
                glob[6] = glob[9];
                glob[7] = glob[10];
                glob[8] = glob[11];
                var of1 = Math.Abs(valu[0]) * FREDC;
                var of2 = Math.Abs(valu[1]) * FREDC;
                switch (edge ?? "")
                {
                    case "X":
                        {
                            glob[3] += of1;
                            glob[6] += of2;
                            break;
                        }
                    case "Y":
                        {
                            glob[4] += of1;
                            glob[7] += of2;
                            break;
                        }
                    case "Z":
                        {
                            glob[5] += of1;
                            glob[8] +=of2;
                            break;
                        }
                }
                nodes[0] = 0d;
                nodes[1] = glob[0];
                nodes[2] = glob[1];
                nodes[3] = glob[2];
                nodes[4] = 0d;
                nodes[5] = glob[3];
                nodes[6] = glob[4];
                nodes[7] = glob[5];
                nodes[8] = 0d;
                nodes[9] = glob[6];
                nodes[10] = glob[7];
                nodes[11] = glob[8];
                nodes[12] = 0d;
                nodes[13] = glob[9];
                nodes[14] = glob[10];
                nodes[15] = glob[11];
                drawSegment(nodes, brush, g, pen, valu[0]);
            }
    }
    private readonly float[] _auxPoints = new float[20];
    private readonly PointF[] _points = new PointF[5];
    private void drawSegment(double[] nodes3d, Brush brush, Graphics g, Pen pen, double sign)
    {
        calc2DCoords(nodes3d, _auxPoints);
        _points[0].X = _auxPoints[1]; _points[0].Y = _auxPoints[2];
        _points[1].X = _auxPoints[5]; _points[1].Y = _auxPoints[6];
        _points[2].X = _auxPoints[9]; _points[2].Y = _auxPoints[10];
        _points[3].X = _auxPoints[13]; _points[3].Y = _auxPoints[14];
        _points[4].X = _auxPoints[1]; _points[4].Y = _auxPoints[2];
        g.DrawLines(pen, _points);
        if (sign < 0)
            drawArrowOptimized(_auxPoints[1], _auxPoints[2], _auxPoints[5], _auxPoints[6], pen, g);
        else if (sign > 0)
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
        float delta = (float)(modelDiagonal / scaleFactor3 / 8.0);
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
        foreach (var restriccion in App.model.nodalRestraints)
        {
            int idxNodo = App.model.nodes.FindIndexByID(restriccion.ID);
            if (idxNodo < 0) continue;
            var nodo = App.model.nodes[idxNodo];
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
        int activeLoadCase = App.model.getParamValue(Mdta.ActiveCase);
        maxLoadValue = getMaxQ();
        var p3d1 = new point3D();
        var p3d2= new point3D();
        foreach (var l in App.model.nodalLoads)
        {
            var nudo = App.model.nodes.First(x => x.ID == l.NodeID);
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
    int loadedCase = -1;
    Dictionary<int, int> fid;
    private void drawDeformedShape(Graphics g, Pen pen)
    {
        if (App.model.nodalDisplacements is  null ||( App.model.nodalDisplacements.Count == 0)) return;
       double deformScale = ms.deformScale;   
       double rotScale = ms.rotScale;   
        var casoAc = App.model.getParamValue(Mdta.ActiveCase);
        var nudos = App.model.nodes;
        var desp = App.model.nodalDisplacements.Where(d => d.Case == casoAc).ToArray();
        var barras = App.model.bars;
        if ( casoAc != loadedCase || fid == null)
        {
            fid = [];
            for (int i = 0; i < desp.Length; i++)
            {
                fid.Add(desp[i].ID,i);
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
    private void drawBarDiagrams(int Barra_id, int ind, int caso_, int tramos,
        ref double[] Vzz, ref double[] Vyy, ref double[] Mzz, ref double[] Myy,
        ref double[] phiz, ref double[] phiy, ref double[] defz, ref double[] defy)
    {
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
        float dx = (float)(head.x2d - cen.x2d);
        float dy = (float)(head.y2d - cen.y2d);
        float l = (float)Math.Sqrt(dx * dx + dy * dy);
        if (l > 0)
        {
            float ux = dx / l;
            float uy = dy / l;
            float vx = -uy;
            float vy = ux;
            float base_x = (float)head.x2d - ux * size;
            float base_y = (float)head.y2d - uy * size;
            PointF p1 = new PointF(base_x + vx * size * 0.5f, base_y + vy * size * 0.5f);
            PointF p2 = new PointF(base_x - vx * size * 0.5f, base_y - vy * size * 0.5f);
            PointF p3 = new PointF((float)head.x2d, (float)head.y2d);
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
    private void drawTriangle(point3D center, point3D head, float size, Pen pen, Graphics g)
    {
        Point p1 = default, p2 = default, p3 = default;
        double ang, dx, dy, l;
        dx = (double)(head.x2d - center.x2d);
        dy = (double)(head.y2d - center.y2d);
        l = Sqrt(Pow(dx, 2d) + Pow(dy, 2d));
        if (dx != 0d)
        {
            ang = Atan(dy / dx);
        }
        else if (dy > 0d)
        {
            ang = 90d * PI / 180d;
        }
        else
        {
            ang = -90 * PI / 180d;
        }
        l = l / (double)size;
        p1.X = (int)Round(head.x2d - (double)size * Sin(ang) - dx / l);
        p1.Y = (int)Round(head.y2d + (double)size * Cos(ang) - dy / l);
        p2.X = (int)Round(head.x2d + (double)size * Sin(ang) - dx / l);
        p2.Y = (int)Round(head.y2d - (double)size * Cos(ang) - dy / l);
        p3.X = (int)Round(head.x2d);
        p3.Y = (int)Round(head.y2d);
        g.DrawLine(pen, p1, p2);
        g.DrawLine(pen, p2, p3);
        g.DrawLine(pen, p1, p3);
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
}