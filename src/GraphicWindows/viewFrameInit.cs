using System.Drawing.Drawing2D;
using static System.Math;
namespace CEF;
public partial class viewFrame
{
    public viewFrame(Form _container,MainWindow mainApp, int Heigth, int Width, int Left=0, int Top=0)
    {
        Container = _container;        
        mainWindow = mainApp; //This control requires a valid cef3d app and database by design
        vfHeight = Heigth;
        vfWidth = Width;
        vfLeft = Left;
        vfTtop = Top;

        initVfControls();
        setupVfControls();

        currentCutCoord = 0d;
        setPlane( View.ThreeDimensional);
        currentCutPlane = 0;
        projection.X0 = (float)(Canvas.Left + Canvas.Right) / 2f;
        projection.Y0 = (float)(Canvas.Bottom + Canvas.Top) / 2f ;

        DrBarrBttOn = false;
        graphsChange = true;
        drawingLineFlag = true;
        projection.Scale = 1d;
        projection.setM();
        mainWindow.setMSglob(projection.Scale);
        ms = App.visualModelStyles.Clone();
        setUpGridCoords();        
        InvalidateScene();
        canvasBMP = backStageDrawScene(Canvas.Width, Canvas.Height);

    }

    private void setupVfControls()
    {
        Canvas.Left = vfLeft;
        Canvas.Top = vfTtop;
        Canvas.Width = vfWidth ;
        Canvas.Height = vfHeight ;
        var W = Canvas.Size.Width;
        var H = Canvas.Size.Height;
        if (W > 0 && H > 0)
        {
            canvasBMP = new Bitmap(W, H);
            g = Graphics.FromImage(canvasBMP);
            g.SmoothingMode = SmoothingMode.None;
        }
    }
    private void clearSelection(object sender, EventArgs e)
    {
        Selection.Clear();
        canvasRefreh();
    }
    public void coordSystemChanged()
    {
        this.gridXPointsCount = (int)App.g_gridXPointsCount;
        this.gridYPointsCount = (int)App.g_gridYPointsCount;
        this.gridZPointsCount = (int)App.g_gridZPointsCount;
    }
    public void setUpGridCoords()
    {
        firstLinePoint = new point3D() { Id=-1};
        gridXPointsCount = (int)App.g_gridXPointsCount;
        gridYPointsCount = (int)App.g_gridYPointsCount;
        gridZPointsCount = (int)App.g_gridZPointsCount;
        screenDiagonal = Sqrt( Pow(Canvas.Width, 2d) + Pow(Canvas.Height, 2d));
        modelDiagonal = Sqrt(Pow(App.model.gridCoordsX[0].Coord - App.model.gridCoordsX[gridXPointsCount - 1].Coord, 2d) +
                    Pow(App.model.gridCoordsY[0].Coord - App.model.gridCoordsY[gridYPointsCount - 1].Coord, 2d) +
                    Pow(App.model.gridCoordsZ[0].Coord - App.model.gridCoordsZ[gridZPointsCount - 1].Coord, 2d));
        if (modelDiagonal == 0) modelDiagonal = 10;
        projection.x = (App.model.gridCoordsX[0].Coord - App.model.gridCoordsX[gridXPointsCount - 1].Coord) / 2;
        projection.y = (App.model.gridCoordsY[0].Coord - App.model.gridCoordsY[gridYPointsCount - 1].Coord) / 2;
        projection.z = (App.model.gridCoordsZ[0].Coord - App.model.gridCoordsZ[gridZPointsCount - 1].Coord) / 2;
        scaleFactor3 = Pow(Pow(gridXPointsCount, 2d) + Pow(gridYPointsCount, 2d) + Pow(gridZPointsCount, 2d), 0.5d);
        projection.modelDiag = modelDiagonal;
        projection.screenDiag = screenDiagonal;
        projection.projFocusAng = PI/2;
        projection.Scale = 100  * screenDiagonal / (modelDiagonal*Tan(projection.projFocusAng/2)); 
        projection.setM();
        space3d.createGrid(mainWindow);
    }
    public void resetView()
    {
        projection.resetProjection();
        projection.X0 = (Canvas.Left + Canvas.Right) / 2;
        projection.Y0 = (Canvas.Top + Canvas.Bottom) / 2;
        projection.x = (App.model.gridCoordsX[0].Coord - App.model.gridCoordsX[gridXPointsCount - 1].Coord) / 2;
        projection.y = (App.model.gridCoordsY[0].Coord - App.model.gridCoordsY[gridYPointsCount - 1].Coord) / 2;
        projection.z = (App.model.gridCoordsZ[0].Coord - App.model.gridCoordsZ[gridZPointsCount - 1].Coord) / 2;
    }
    public void zoomAll()
    {
        projection.Scale =  100 * projection.screenDiag / (projection.modelDiag * Tan(projection.projFocusAng / 2));
        projection.X0 = (Canvas.Left + Canvas.Right) / 2;
        projection.Y0 = (Canvas.Top + Canvas.Bottom) / 2;
        projection.x = (App.model.gridCoordsX[0].Coord - App.model.gridCoordsX[gridXPointsCount - 1].Coord) / 2;
        projection.y = (App.model.gridCoordsY[0].Coord - App.model.gridCoordsY[gridYPointsCount - 1].Coord) / 2;
        projection.z = (App.model.gridCoordsZ[0].Coord - App.model.gridCoordsZ[gridZPointsCount - 1].Coord) / 2;
    }
}