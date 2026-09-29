using System.Windows.Forms;

namespace CEF;
public partial class viewFrame
{
	internal static bool snapToEnds   = false;
	internal static bool snapToCenter = false;
	internal static bool snapToPerp   = false;
	internal static bool snapToNear   = false;
	internal static bool snapToPoints   = false;

    private static point3D currSnapedPoint;
    private static point3D firstLinePoint;



    private void processSnaps(int barIndx, int X_, int Y_) 
	
	{
		    var localPrevNode = currSnapedPoint;
            currSnapedPoint = new() { Id=-1};

            if (snapToEnds)
            {
                var bar = App.model.bars[barIndx];

                var na = App.model.nodes.First(n => n.ID == bar.startNode);
                var nb = App.model.nodes.First(n => n.ID == bar.endNode);

                var p1 = new point3D() { Id = -2, x3d = na.X, y3d = na.Y, z3d = na.Z };
                var p2 = new point3D() { Id = -2, x3d = nb.X, y3d = nb.Y, z3d = nb.Z };

                p3DSet2DCoord(ref p1);
                p3DSet2DCoord(ref p2);

                var d1 = (p1.x2d - X_) * (p1.x2d - X_) + (p1.y2d - Y_) * (p1.y2d - Y_);
                var d2 = (p2.x2d - X_) * (p2.x2d - X_) + (p2.y2d - Y_) * (p2.y2d - Y_);
                Container.Refresh();

                currSnapedPoint = d2 > d1 ? p1 : p2;
                Canvas.CreateGraphics().DrawEllipse(Pens.Red, currSnapedPoint.x2d, currSnapedPoint.y2d, 5, 5);
            }

            if (snapToCenter)
            {
                var bar = App.model.bars[barIndx];
                var na = App.model.nodes.First(n => n.ID == bar.startNode);
                var nb = App.model.nodes.First(n => n.ID == bar.endNode);
                var p1 = new point3D() { Id = -2, x3d = (na.X+ nb.X)/2, y3d = (na.Y+ nb.Y)/2, z3d = (na.Z+ nb.Z)/2 };

                p3DSet2DCoord(ref p1);
                var d1 = (p1.x2d - X_) * (p1.x2d - X_) + (p1.y2d - Y_) * (p1.y2d - Y_);
                Container.Refresh();

                Canvas.CreateGraphics().DrawEllipse(Pens.Red, p1.x2d, p1.y2d, 5, 5);
                currSnapedPoint =p1; 
            }

            if (snapToNear)
            {
                var bar = App.model.bars[barIndx];
                var na = App.model.nodes.First(n => n.ID == bar.startNode);
                var nb = App.model.nodes.First(n => n.ID == bar.endNode);

                var p1 = new point3D() { Id = -2, x3d = na.X, y3d = na.Y, z3d = na.Z };
                var p2 = new point3D() { Id = -2, x3d = nb.X, y3d = nb.Y, z3d = nb.Z };
                var p3 = new point3D();

                p3DSet2DCoord(ref p1);
                p3DSet2DCoord(ref p2);

                var d1 = (p1.x2d - X_) * (p1.x2d - X_) + (p1.y2d - Y_) * (p1.y2d - Y_);
                var d2 = (p2.x2d - p1.x2d) * (p2.x2d - p1.x2d) + (p2.y2d - p1.y2d) * (p2.y2d - p1.y2d);
                Container.Refresh();

                if (d2 >= d1 && d2>0)
                {
                    var r = Math.Sqrt(d1 / d2);
                    p3.x3d =  p1.x3d+(p2.x3d - p1.x3d) * r;
                    p3.y3d =  p1.y3d+(p2.y3d - p1.y3d) * r;
                    p3.z3d =  p1.z3d+(p2.z3d - p1.z3d) * r;
                    p3DSet2DCoord(ref p3);

                    Canvas.CreateGraphics().DrawEllipse(Pens.Red, p3.x2d, p3.y2d, 5, 5);                    
                    currSnapedPoint = p3;
                }               
            }

            if (snapToPerp && firstLinePoint.Id != -1)
            {
                var bar = App.model.bars[barIndx];

                var na = App.model.nodes.First(n => n.ID == bar.startNode);
                var nb = App.model.nodes.First(n => n.ID == bar.endNode);

            var v = new point3D() { x3d = nb.X - na.X, y3d = nb.Y - na.Y, z3d = nb.Z - na.Z };
            var nc = firstLinePoint;
            var w = new point3D() { x3d = nc.x3d - na.X, y3d = nc.y3d - na.Y, z3d = nc.z3d - na.Z };

            var t = (w.x3d * v.x3d + w.y3d * v.y3d + w.z3d * v.z3d) / (v.x3d*v.x3d+ v.y3d * v.y3d+ v.z3d * v.z3d);

            var p1 = new point3D() {Id=-2, x3d =na.X+t*(nb.X-na.X), y3d = na.Y + t * (nb.Y - na.Y), z3d = na.Z + t * (nb.Z - na.Z) };
                p3DSet2DCoord(ref p1);
                var d1 = (p1.x2d - X_) * (p1.x2d - X_) + (p1.y2d - Y_) * (p1.y2d - Y_);
                Container.Refresh();
                Canvas.CreateGraphics().DrawEllipse(Pens.Red, p1.x2d, p1.y2d, 5, 5);
                currSnapedPoint = p1;
            }		
	}

    }