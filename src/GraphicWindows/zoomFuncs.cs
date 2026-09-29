using static System.Math;
namespace CEF;
public partial class viewFrame {
    public void onZoom()
    {
        onZoomBox = true;
    }
    public void zoomWindowFunction(Rectangle SelRect)
    {     
        var rec = SelRect;
        var recCenX = (rec.Left + rec.Right) / 2d;
        var recCenY = (rec.Top + rec.Bottom) / 2d;
        var canvasCenX = (double)(Canvas.Left + Canvas.Right) / 2;
        var canvasCenY = (double)(Canvas.Top + Canvas.Bottom) / 2;
        var currentCanvasDiagonal = Sqrt(Pow(rec.Left - rec.Right, 2) + Pow(rec.Top - rec.Bottom, 2));
        var worldViewPoint = new double[4];
        var screenViewPointCoord = new double[4];
        projection.unProject(ref worldViewPoint, [recCenX, recCenY, projection.getFar()]);
        var projPrevScrrenDiagonal = projection.screenDiag;
        var scaleFactor = (float)(projPrevScrrenDiagonal / (currentCanvasDiagonal));
        scaleFactor = scaleFactor > 10f ? 10f : scaleFactor;
        projection.Scale *= scaleFactor;
        projection.setM();
        projection.project(worldViewPoint, ref screenViewPointCoord);
        var reprX = (float)screenViewPointCoord[0];
        var reprY = (float)screenViewPointCoord[1];
        recCenX = (screenViewPointCoord[0]);
        recCenY = (screenViewPointCoord[1]);
        var diplacemente2dX = recCenX;
        var displacement2dY = recCenY;
        projection.X0 -= (float)(recCenX - canvasCenX);
        projection.Y0 -= (float)(recCenY - canvasCenY);
        projection.setM();
    }
    public void zoomObjectFunction(List<point3D> points3d) 
    {
        if (points3d.Count < 2) return;
        var canvasCenX = (double)(Canvas.Left + Canvas.Right) / 2;
        var canvasCenY = (double)(Canvas.Top + Canvas.Bottom) / 2;
        var maxX2d = points3d.Max(p => p.x2d);
        var maxY2d = points3d.Max(p => p.y2d);
        var minX2d = points3d.Min(p => p.x2d);
        var minY2d = points3d.Min(p => p.y2d);
        var x3dmean = points3d.Sum(p => p.x3d) / points3d.Count;
        var y3dmean = points3d.Sum(p => p.y3d) / points3d.Count;
        var z3dmean = points3d.Sum(p => p.z3d) / points3d.Count;
        var currentCanvasDiagonal = Sqrt(Pow(maxX2d - minX2d, 2) + Pow(maxY2d - minY2d, 2));
        var worldViewPoint = new double[4] { x3dmean, y3dmean, z3dmean, 1 };
        var screenViewPointCoord = new double[4];
        var projPrevScrrenDiagonal = projection.screenDiag;
        if (currentCanvasDiagonal <= 0) return;
        var scaleFactor = (float)(projPrevScrrenDiagonal / (currentCanvasDiagonal));
        if (scaleFactor <= 0.5) return;
        scaleFactor = scaleFactor > 10f ? 10f : scaleFactor;
        projection.Scale *= scaleFactor;
        projection.setM();
        projection.project(worldViewPoint, ref screenViewPointCoord);
        var reprX = (float)screenViewPointCoord[0];
        var reprY = (float)screenViewPointCoord[1];
        var recCenX = (screenViewPointCoord[0]);
        var recCenY = (screenViewPointCoord[1]);
        var diplacemente2dX = recCenX;
        var displacement2dY = recCenY;
        projection.X0 -= (float)(recCenX - canvasCenX);
        projection.Y0 -= (float)(recCenY - canvasCenY);
        projection.setM();
    }
}