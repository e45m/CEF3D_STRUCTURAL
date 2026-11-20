using System;
using System.ComponentModel;
using static CEF.ModelDB;
namespace CEF;
public partial class viewFrame
{
    public CEF.MainWindow mainWindow;
    

    private point3D[] SnapPoints;
    private point3D[] point3DColection;
    private ViewFrameView currentView;
    private int currentCutPlane;
    private double currentCutCoord;
    private int pointsCount;
    private bool graphsChange;
    private static bool  drawingLineFlag = true;
    private static bool  drawingAreaFlag = true;
    private bool  DrBarrBttOn;
    private Selection Sel = new();
    private int[] barConecty;
    private int[] areaConecty;
    private int gridXPointsCount;
    private int gridYPointsCount;
    private int gridZPointsCount;
    private double screenDiagonal;
    private double modelDiagonal;
    private double scaleFactor3;
    public Projection projection = new ();
    private float fontScale;
    public double maxLoadValue, MaxDesp, MaxAng;
    private double loadEscFact;
    private int deformedStations = 25;
    private int oldXLine, oldYLine;
    private bool verMouse;
    private bool rotaMouse;
    public int defaultColorStyle;
    internal UISettings ms;
    Bitmap canvasBMP;
    Graphics g;

    public double getPlaneCoord() => currentCutCoord;
    public void calc2DCoords(ref point3D p)
    {
        double[] coords = { 0, p.x3d, p.y3d, p.z3d };
        float[] cord2d = new float[4];
        calc2DCoords(coords, cord2d);
        p.x2d = cord2d[1];
        p.y2d = cord2d[2];
        p.z2d = cord2d[3];
    }
    public void calc2DCoords(ref point3D[] nodes3d)
    {
        var Temp1 = new double[3];
		var Temp2 = new float[3];
        int nodesCount = nodes3d.GetLength(0);
        int i;
        for (i = 0; i < nodesCount; i++)
        {
            Temp1[0] = nodes3d[i].x3d;
            Temp1[1] = nodes3d[i].y3d;
            Temp1[2] = nodes3d[i].z3d;
            projection.project(Temp1, ref Temp2);
            nodes3d[i].x2d = (Temp2[0]);
            nodes3d[i].y2d = (Temp2[1]);
            nodes3d[i].z2d = (Temp2[2]);
        }
    }
    public void calc2DCoords(double[] nodes3d, float[] points2d)
    {
        var  Temp1 = new double[3]; 
		var Temp2 = new float [3];
        int nodesCount = nodes3d.GetLength(0);
        int i;
        for (i = 0; i < nodesCount; i += 4)
        {
            Temp1[0] = nodes3d[i + 1];
            Temp1[1] = nodes3d[i + 2];
            Temp1[2] = nodes3d[i + 3];
            projection.project(Temp1, ref Temp2);
            points2d[i] = (float)nodes3d[i];
            points2d[i + 1] = (Temp2[0]);
            points2d[i + 2] = (Temp2[1]);
            points2d[i + 3] = (Temp2[2]);
        }
    }
    public void parallelCalc2DCoords(double[] nodes3d, float[] points2d)
    {
        int nodesCount = nodes3d.Length / 4;
        Parallel.For(0, nodesCount, idx =>
        {
			var  Temp1 = new double[3]; 
			var Temp2 = new float [3];
            int i = idx * 4;
            Temp1[0] = nodes3d[i + 1];
            Temp1[1] = nodes3d[i + 2];
            Temp1[2] = nodes3d[i + 3];
            projection.project(Temp1, ref Temp2);
            points2d[i] = (float)nodes3d[i];
            points2d[i + 1] = Temp2[0];
            points2d[i + 2] = Temp2[1];
            points2d[i + 3] = Temp2[2];
        });
    }

    int createNewNode(point3D point)
    {

        if ( Engine.getModel().nodes.Any(n=>point.IsNear(n.X,n.Y,n.Z))) return-1;       

        var id = Engine.getModel().nodes.NextID();
        point.Id = id;
        var newNode = new Node()
        {
            ID =  point.Id,
            label = $"{id}",
            X = point.x3d,
            Y = point.y3d,
            Z = point.z3d
        };
        Engine.getModel().nodes.Add(newNode);
        var newRestr = new nodalRestraint()
        {
            ID = point.Id,
            ux = 0,
            uy = 0,
            uz = 0,
            rx = 0,
            ry = 0,
            rz = 0
        };
        Engine.getModel().nodalRestraints.Add(newRestr);        
        return id;
    }

    void createNewBar(int IDStartNode, int IDEndNode)
    {

      
        int ID_Seccion_Por_Omision = App.barSectionDefaultID;

        if (ID_Seccion_Por_Omision == -1)
        {
            if (Engine.getModel().sections.Count == 0) 
            Engine.getModel().sections.Add(new Section() { Name = "None", ID = 0});
            ID_Seccion_Por_Omision = Engine.getModel().sections[0].ID;
        }
        
        string Nombre_Seccion_Por_Omision = Engine.getModel().sections.First(s => s.ID == ID_Seccion_Por_Omision).Name;
        int DivBarra = Engine.getModel().getParamValue(paramDefaults.divideIntoSections);

        var _id = Engine.getModel().bars.NextID();
        

        var newBar = new Bar() 
        {
            ID = _id,
            label = $"{_id} ",
            startNode = IDStartNode,
            endNode = IDEndNode,
            SectionID = ID_Seccion_Por_Omision,            
            SectionName = Nombre_Seccion_Por_Omision,
            unused = DivBarra,
            Rotation = 0.0,
            releases = 0b0,
            offsetY = 0,
            offsetZ = 0
        };
        Engine.getModel().bars.Add(newBar);
        resetCanvasGeometry();
    }

     void createNewArea(int n1, int n2, int n3, int n4)    {

        
        int ID_Seccion_Por_Omision = App.areaSectionDefaultID;

        if (ID_Seccion_Por_Omision == -1)
        {
            if (Engine.getModel().areaSections.Count == 0) 
            Engine.getModel().areaSections.Add(new areaSection() { Name = "None", ID = 0});
            ID_Seccion_Por_Omision = Engine.getModel().areaSections[0].ID;
        }
       
        string Nombre_Seccion_Por_Omision = Engine.getModel().areaSections.First(s => s.ID == ID_Seccion_Por_Omision).Name;
       
        var _id = Engine.getModel().areas.NextID();        

        var newArea = new Area() 
        {
            ID = _id,
            label = $"{_id} ",
            Node1 = n1,
            Node2 = n2,
            Node3 = n3,
            Node4 = n4,            
            SectionID = ID_Seccion_Por_Omision,            
            SectionName = Nombre_Seccion_Por_Omision,
            sign = 1
        };
        Engine.getModel().areas.Add(newArea);
        resetCanvasGeometry();
    }

    point3D getPointFromDataBase(Node pp)
    {
       
       var p =  new point3D() {

           Id = pp.ID,
           IsNode = true,
           x3d = pp.X,
           y3d = pp.Y,
           z3d = pp.Z,
           x2d = 0f,
           y2d = 0f,
           z2d = 0f };

        p3DSet2DCoord(ref p);
        return p;

    }

    private void addPointToNewBarQueue(int screenX, int screenY)
    {
        
        int indx;
        point3D nextStartPoint;
              
        if (App.onDrawingNewLine)
        {
            if(currSnapedPoint.Id== -1)
            {
                
                indx = space3d.isPoint(SnapPoints, screenX, screenY, currentView, currentCutCoord);
                if (indx != -1) 
                    currSnapedPoint = getPointFromDataBase(Engine.getModel().nodes[indx]);
            }
            if (currSnapedPoint.Id == -2)
            {
                indx = space3d.isPoint(SnapPoints, (int)currSnapedPoint.x2d, (int)currSnapedPoint.y2d, currentView, currentCutCoord);
                if (indx != -1)
                    currSnapedPoint = getPointFromDataBase(Engine.getModel().nodes[indx]);
            }

            if (currSnapedPoint.Id != -1 && currSnapedPoint.Id != -2)
            {
                if (drawingLineFlag)
                {
                    firstLinePoint = currSnapedPoint;
                    drawingLineFlag = !drawingLineFlag;
                    return;
                }
                else if(!currSnapedPoint.IsNear(firstLinePoint)) 

                {
                    App.takeUndoSnapShot();

                    if (!firstLinePoint.IsNode)
                    {                     
                       var _id =  createNewNode( firstLinePoint);   
                       if(_id>-1) {
                        firstLinePoint.Id = _id;
                        firstLinePoint.IsNode = true;  
                          }                 
                       
                    }
                       // nextStartPoint = firstLinePoint;
                    if (!currSnapedPoint.IsNode)
                    {                      
                       var _id =  createNewNode(currSnapedPoint);   
                       if(_id>-1) {
                        currSnapedPoint.Id = _id;
                        currSnapedPoint.IsNode = true;  
                          }  
                    }
                    nextStartPoint = currSnapedPoint;
                    createNewBar(firstLinePoint.Id, currSnapedPoint.Id);
                    firstLinePoint = nextStartPoint;
                    mainWindow.reloadAllCanvasWindows();
                    return;
                }
            }
        }
    }

        private void addPointToNewAreaQueue(int screenX, int screenY)
    {
        int indx;
        point3D nextStartPoint;
              
        if (App.onDrawingNewArea)
        {
            if(currSnapedPoint.Id== -1)
            {
                
                indx = space3d.isPoint(SnapPoints, screenX, screenY, currentView, currentCutCoord);
                if (indx != -1) 
                    currSnapedPoint = getPointFromDataBase(Engine.getModel().nodes[indx]);
            }
            if (currSnapedPoint.Id == -2)
            {
                indx = space3d.isPoint(SnapPoints, (int)currSnapedPoint.x2d, (int)currSnapedPoint.y2d, currentView, currentCutCoord);
                if (indx != -1)
                    currSnapedPoint = getPointFromDataBase(Engine.getModel().nodes[indx]);
            }

            if (currSnapedPoint.Id != -1 && currSnapedPoint.Id != -2)
            {
 
             if(!currSnapedPoint.IsNear(firstLinePoint)) 

                {
                    App.takeUndoSnapShot();
                    if(drawingAreaFlag )
                    {
                        drawingAreaFlag = !drawingAreaFlag;
                         shapePointsList = [];
                    }

                    if (!firstLinePoint.IsNode)
                    {                     
                       var _id =  createNewNode( firstLinePoint);   
                       if(_id>-1) {
                        firstLinePoint.Id = _id;
                        firstLinePoint.IsNode = true;  
                          }                 
                       
                    }
                       // nextStartPoint = firstLinePoint;
                    if (!currSnapedPoint.IsNode)
                    {                      
                       var _id =  createNewNode(currSnapedPoint);   
                       if(_id>-1) {
                        currSnapedPoint.Id = _id;
                        currSnapedPoint.IsNode = true;  
                          }  
                    }

                    nextStartPoint = currSnapedPoint;
                    shapePointsList.Add( nextStartPoint);

                    if(shapePointsList.Count>=4)
                    {
                    
                    createNewArea(shapePointsList[0].Id, shapePointsList[1].Id,shapePointsList[2].Id,shapePointsList[3].Id);
                    shapePointsList = [];
                    shapePointsList.Add( nextStartPoint);                    

                    }
                    firstLinePoint = nextStartPoint;
                    mainWindow.reloadAllCanvasWindows();
                    return;
                }
            }
        }
    }


    private bool trySelect(int screenX, int screenY)
    {

        var i = space3d.isPoint(SnapPoints, screenX, screenY, currentView, currentCutCoord);
        if ((i != -1)&& SnapPoints[i].IsNode)
        {   Selection.nodeAdd(i);
            mainWindow.reloadAllCanvasWindows();  
            return true;          
        }
        var j = space3d.isBar(barConecty, SnapPoints, screenX, screenY, currentView, currentCutCoord);  
        if (j != -1)
        {
                Selection.barAdd(j);
                mainWindow.reloadAllCanvasWindows();   
                return true;         
        }
        var k = space3d.isArea(areaConecty, SnapPoints, screenX, screenY, currentView, currentCutCoord);  
        if (k != -1)
        {
                Selection.areaAdd(k);
                mainWindow.reloadAllCanvasWindows();   
                return true;         
        }
        return false;

    }

    public void removeBar()
    {
        int NBar = Selection.barMaxIndex;
        var Sel =  Selection.getBarsColection();
        for (int i = 0; i <NBar; i++)
        {
            int idx = Engine.getModel().bars.FindIndexByID(Sel[i]);
            if (idx >= 0)
            {
                Engine.getModel().bars.RemoveAt(idx);
            }
        }
        Selection.Clear();
        canvasRefreh();
    }
    public void p3DSet2DCoord(ref point3D P)
    {
        var P3d = new double[4];
        var P2d = new float[4];
        P3d[1] = P.x3d;
        P3d[2] = P.y3d;
        P3d[3] = P.z3d;
        calc2DCoords(P3d, P2d);
        P.x2d = P2d[1];
        P.y2d = P2d[2];
        P.z2d = P2d[3];
    }
    public point3D p3DGlobFromLoc(point3D p, double[] vect)
    {
        double[] loc = new double[3], glob = new double[3];
        loc[0] = p.x3d;
        loc[1] = p.y3d;
        loc[2] = p.z3d;
        space3d.glob2loc(vect, loc, ref glob);
        p.x3d = glob[0];
        p.y3d = glob[1];
        p.z3d = glob[2];
        p3DSet2DCoord(ref p);
        return p;
    }
    public point3D p3DLocFromGlob(point3D p, double[] vect)
    {
        double[] loc = new double[3], glob = new double[3];
        glob[0] = p.x3d;
        glob[1] = p.y3d;
        glob[2] = p.z3d;
        space3d.loc2glob(vect, ref loc, glob);
        p.x3d = loc[0];
        p.y3d = loc[1];
        p.z3d = loc[2];
        p3DSet2DCoord(ref p);
        return p;
    }


    public int getLevel() => currentCutPlane;

    public int incLevel(int value = 1)
    {
        setLevel(currentCutPlane + value);
        return currentCutPlane; 
    }

    public void setLevel(int Level)
    {
        int n = 0;
        switch (currentView)
        {
            case ViewFrameView.XY:
                {
                    n = gridZPointsCount;
                    if(Level>=0&&Level<n){
                        currentCutPlane = Level;
                        currentCutCoord = Engine.getModel().gridCoordsZ[currentCutPlane].Coord;
                    }
                    break;
                }
            case ViewFrameView.XZ:
                {
                    n = gridYPointsCount;
                    if (Level >= 0 && Level < n)
                    {
                        currentCutPlane = Level;
                        currentCutCoord = Engine.getModel().gridCoordsY[currentCutPlane].Coord;
                    }
                    break;
                }
            case ViewFrameView.YZ:
                {
                    n = gridXPointsCount;
                    if (Level >= 0 && Level < n)
                    {   currentCutPlane = Level;
                        currentCutCoord = Engine.getModel().gridCoordsX[currentCutPlane].Coord;
                    }
                    break;
                }
            case ViewFrameView.ThreeDimensional:
                {
                    currentCutCoord = 0d;
                    break;
                }
            default:
                {
                    currentCutCoord = 0d;
                    break;
                }
        }
    }
    
    public  ViewFrameView getPlane() => currentView;

    public void setPlane(ViewFrameView Plane)
    {
        currentView = Plane;
        switch (currentView)
        {
            case ViewFrameView.XY:
                {
                    currentCutCoord = Engine.getModel().gridCoordsZ[currentCutPlane].Coord;
                    break;
                }
            case ViewFrameView.XZ:
                {
                    currentCutCoord = Engine.getModel().gridCoordsY[currentCutPlane].Coord;
                    break;
                }
            case ViewFrameView.YZ:
                {
                    currentCutCoord = Engine.getModel().gridCoordsX[currentCutPlane].Coord;
                    break;
                }
            case ViewFrameView.ThreeDimensional:
                {
                    currentCutCoord = 0d;
                    break;
                }
            default:
                {
                    currentCutCoord = 0d;
                    break;
                }
        }
        planeChange();
    }


    private double getMaxQ()
    {
        int casoActivo = Engine.getModel().getParamValue(paramDefaults.ActiveCase);
        if (Engine.getModel().barDistrLoads.Count > 0 ||
            Engine.getModel().barPunctLoads.Count > 0 || 
            Engine.getModel().nodalLoads.Count > 0)
        {
            var maxQ = 1d;
            var MaxQ1 = 0d;
            var MaxQ2 = 0d;
            var MaxQ3 = 0d;
            var mq = Engine.getModel().barDistrLoads.Where(l => l.Case == casoActivo).ToList();
            var mqbp = Engine.getModel().barPunctLoads.Where(l => l.Case == casoActivo).ToList();
            var mqp = Engine.getModel().nodalLoads.Where(l => l.Case == casoActivo).ToList();
            if(mq.Count>0){
            MaxQ1 = mq.Max( l => Math.Max(Math.Abs(l.Fy), Math.Max(Math.Abs(l.Fx), Math.Abs(l.Fz))));
                }
            if (mqp.Count > 0)
            {
             MaxQ2 = mqp.Max(l => Math.Max(Math.Abs(l.Fy), Math.Max(Math.Abs(l.Fx), Math.Abs(l.Fz))));
            }
            if (mqbp.Count > 0)
            {
             MaxQ3 = mqbp.Max(l => Math.Max(Math.Abs(l.Fy), Math.Max(Math.Abs(l.Fx), Math.Abs(l.Fz))));
            }

            maxQ = (new double [] {MaxQ1, MaxQ2, MaxQ3}).Max();
            return maxQ;
        }
        else { 
            return 1;
        }
    }
    private void SetSnapPoints()
    {
        pointsCount = Engine.getAppData().Puntos.Count;
        var nodesCount = Engine.getModel().nodes.Count;
        SnapPoints = new point3D[pointsCount + nodesCount];
        Parallel.For(0, nodesCount, i =>
        {
            ref var sp = ref SnapPoints[i];
            var n = Engine.getModel().nodes[i];
            sp.Id = n.ID;
            sp.x3d = n.X;
            sp.y3d = n.Y;
            sp.z3d = n.Z;
            sp.x2d = 0;
            sp.y2d = 0;
            sp.IsNode = true;
            p3DSet2DCoord(ref sp);
        });
        Parallel.For(0, pointsCount, i =>
        {
            int idx = nodesCount + i;
            ref var sp = ref SnapPoints[idx];
            var p = Engine.getAppData().Puntos[i];
            sp.Id = -10000;
            sp.x3d = p.X;
            sp.y3d = p.Y;
            sp.z3d = p.Z;
            sp.x2d = 0;
            sp.y2d = 0;
            sp.IsNode = false;
            p3DSet2DCoord(ref sp);
        });
    }
    public void reloadCanvas()
    {
        canvasRefreh();
    }
    public void switchDrawingLineFlag()
    {
        drawingLineFlag = !drawingLineFlag;
    }
    public void setdrawingLineFlag(bool val)
    {
        drawingLineFlag = val;
    }
    public void setButtonsLoc( bool DrBarrBttOn_)
    {
        DrBarrBttOn = DrBarrBttOn_;
    }
    public void callSettings()
    {

    }
    public  void  resetProjection()
    {
        projection.resetProjection();
    }
}