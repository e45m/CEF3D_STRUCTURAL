using System.Data.Common;
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
    {//TODO: Reduce news
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
        ms.showSelectedNodes, ms.showNodes, ms.showNodesID);
        barLineStyle.Width = ms.barLineSSize / 4; 
        var lcbrush = new SolidBrush(ms.loadColorBrush);
        if (ms.showBarsLoads)
        {
            drawBarsDistrLoads(lcbrush, currentCanvas, loadpen);
            drawBarsPunctLoads(lcbrush, currentCanvas, loadpen);
        }
        float FontSize = ms.defaultFont.Size * 0.6f;
        var barLoadPenStyle = new Pen(ms.loadPenColor, FontSize / 2) { Width = ms.barLineW / 2 };
        if (ms.showNodalLoads)
        {
            drawNodeLoads(lcbrush, currentCanvas, barLoadPenStyle);
        }
        var deformedPenStyle = new Pen(ms.deformedShapeColor, ms.barLineW / 1.5f);
        if (ms.showDeformedShape)
        {
            drawDeformedShape(currentCanvas, deformedPenStyle);
        }
        barLineStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        barLineStyle.Width = ms.barLineW / 3;
        var EstiloApoyos = new Pen(ms.SuppColorBrush); 
        drawNodesSupports( currentCanvas, EstiloApoyos);
        drawSelectBars( currentCanvas, ms.barSelectedColor, ms.barLineW,  barLineStyle, ms.defaultFont);

       
  //Draw Internal forces
  MainWindow.tsBarAction.Visible = ms.showBarsDiagrams;
if(ms.showBarsDiagrams)      
    drawBarsInternalDiagrams(new SolidBrush(ms.barDiagramsSColor), currentCanvas, new Pen(ms.barDiagramsSColor),
        actions : ms.barAction , diagHeight : 1.0, refMax: null);

if(ms.showNodalReactions)
    drawNodeReactions(new SolidBrush(ms.reactionPenColor), currentCanvas, new Pen(ms.reactionPenColor));

  MainWindow.tsAreaAction.Visible = ms.showAreasDiagrams;
if(ms.showAreas)  
    drawAreas(new SolidBrush(ms.areaColor),areaConecty ,currentCanvas, new Pen(ms.areaColor) );
      //TODO COLORS
    if(ms.showAreasDiagrams)
      drawAreaInternActions(ms.areaActionLowerColor,ms.areaActionUpperColor, currentCanvas, new Pen(ms.areaColor),  areaConecty,
     ms.areaAction);
     
if(ms.showBarsAxes)
    drawBarAxes(currentCanvas,Enumerable.Range(0,Engine.getModel().bars.Count).ToArray());
  
if(ms.showAreasAxes)
    drawAreaAxes(currentCanvas,Enumerable.Range(0,Engine.getModel().areas.Count).ToArray());



if(ms.showNodesConstraints)
    drawMFC(new SolidBrush(ms.reactionPenColor), currentCanvas, new Pen(ms.reactionPenColor));

if(ms.showBarReleases)
    drawBarRels(new SolidBrush(ms.reactionPenColor), currentCanvas, new Pen(ms.reactionPenColor));

    }


	
//MISC
   private void drawGrid(point3D[] sP, double delta, Graphics g, Pen gridPen)
    {
        var Cx = new List<Coordinate>();
        var Cy = new List<Coordinate>();
        var Cz = new List<Coordinate>();
        for (int i = 0; i < Engine.getModel().gridCoordsX.Count; i++)
        {
            if ((Engine.getModel().gridCoordsX[i].coordSys ) == (App.currCoordSist ))
                Cx.Add(Engine.getModel().gridCoordsX[i]);
        }
        for (int i = 0; i < Engine.getModel().gridCoordsY.Count; i++)
        {
            if ((Engine.getModel().gridCoordsY[i].coordSys ) == (App.currCoordSist ))
                Cy.Add(Engine.getModel().gridCoordsY[i]);
        }
        for (int i = 0; i < Engine.getModel().gridCoordsZ.Count; i++)
        {
            if ((Engine.getModel().gridCoordsZ[i].coordSys ) == (App.currCoordSist ))
                Cz.Add(Engine.getModel().gridCoordsZ[i]);
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
        point3DColection = new point3D[Engine.getModel().nodes.Count*3];
        var maxIters = Engine.getModel().bars.Count*3 ;
        barConecty = new int[maxIters];
        var areasCount = Engine.getModel().areas.Count*5;
        areaConecty = new int[areasCount];
        int indx = -1;
        for (int i = 0; i < maxIters; i+=3)
        {
            indx = (int)(i/3);
            barConecty[i+0] = Engine.getModel().bars[indx].ID;
            barConecty[i+1] = Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[indx].startNode);
            barConecty[i+2] =  Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[indx].endNode);
        }
                for (int i = 0; i < areasCount; i+=5)
        {
            indx = (int)(i/5);
            areaConecty[i+0] = Engine.getModel().areas[indx].ID;
            areaConecty[i+1] = Engine.getModel().nodes.FindIndexByID(Engine.getModel().areas[indx].Node1);
            areaConecty[i+2] = Engine.getModel().nodes.FindIndexByID(Engine.getModel().areas[indx].Node2);
            areaConecty[i+3] = Engine.getModel().nodes.FindIndexByID(Engine.getModel().areas[indx].Node3);
            areaConecty[i+4] = Engine.getModel().nodes.FindIndexByID(Engine.getModel().areas[indx].Node4);
        }




        maxIters = Engine.getModel().nodes.Count;
        for (int i = 0; i < maxIters; i++)
        {
            point3DColection[i].x3d = Engine.getModel().nodes[i].X;
            point3DColection[i].y3d = Engine.getModel().nodes[i].Y;
            point3DColection[i].z3d = Engine.getModel().nodes[i].Z;
            p3DSet2DCoord(ref point3DColection[i]);
        }
    }
	
  

}