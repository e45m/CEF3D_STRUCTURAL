using System.Windows.Forms;

namespace CEF;
public partial class viewFrame
{
	    private void CanvasPaint(object sender, PaintEventArgs e)
    {
        e.Graphics.DrawImageUnscaled(canvasBMP, 0, 0);
    }
    internal Point p0SelRect;
    internal Rectangle selRect;
    bool selecting = false;
    bool onZoomBox = false;
    bool onZoomBoxEnga = false;
    internal VFTyes activeVF ;

   

    internal enum VFTyes { Active, NoActive, Single };

    private void modelViewLoad(object sender, EventArgs e)
    {
        InvalidateScene();
        canvasBMP = backStageDrawScene(Canvas.Width, Canvas.Height);
    }
    private void CanvasMouseMove(object sender, MouseEventArgs e)
    {
        currSnapedPoint = new() { Id = -1 };

        if (selecting || onZoomBoxEnga)
        {
            selRect = new Rectangle(
                Math.Min(p0SelRect.X, e.X),
                Math.Min(p0SelRect.Y, e.Y),
                Math.Abs(e.X - p0SelRect.X),
                Math.Abs(e.Y - p0SelRect.Y));
            Container.Refresh();
            Canvas.CreateGraphics().DrawRectangle(new Pen(ms.gridLinesColor), selRect);
        }
        int X_ = e.X;
        int Y_ = e.Y;
        bool Punt = false;
        bool Bar = false;
        int nodeIndx;
        int barIndx;
        ballonTip.Active = true; 
        ballonTip.BackColor = Color.Beige;
        ballonTip.IsBalloon = true;
        ballonTip.UseAnimation = true;
        ballonTip.ForeColor = Color.Red;
        nodeIndx = space3d.isPoint(SnapPoints, X_, Y_, currentView, currentCutCoord);
        if (!drawingLineFlag)
        {
            Container.Refresh();
            Canvas.CreateGraphics().DrawLine(Pens.Black, firstLinePoint.x2d, firstLinePoint.y2d, oldXLine, oldYLine);
            oldXLine = X_;
            oldYLine = Y_;
        }
        if (nodeIndx != -1)
        {
            Punt = true;
        }
        barIndx = space3d.isBar(barConecty, SnapPoints, X_, Y_, currentView, currentCutCoord); 
        if (barIndx != -1)
        {
            Bar = true;
        }

        if (Punt & !Bar)
        {
            if(snapToPoints)
            {
                Container.Refresh();
                currSnapedPoint = SnapPoints[nodeIndx];
                Canvas.CreateGraphics().DrawEllipse(Pens.Red, currSnapedPoint.x2d, currSnapedPoint.y2d, 5, 5);
            }

            if (SnapPoints[nodeIndx].IsNode)
                {
                    var labelTxt = App.model.nodes[nodeIndx].label;
                    var text = $"Node {SnapPoints[nodeIndx].Id} \n";
                    if (App.model.nodalDisplacements.Count > 0)
                    {
                        int k = App.model.nodalRestraints.FindIndexByID(SnapPoints[nodeIndx].Id);
                        text += "";
                    }
                    else if (App.model.nodalRestraints.Count > 0)
                    {
                        int k = App.model.nodalRestraints.FindIndexByID(SnapPoints[nodeIndx].Id);
                        text += "";
                    }
                }
                else
                {
                    ballonTip.Show(
                        $"Grid Point ({SnapPoints[nodeIndx].x3d}, {SnapPoints[nodeIndx].y3d}, {SnapPoints[nodeIndx].z3d})",
                       Container, X_, Y_, 1000);
                }
            
        }
        else if (Bar)
        {
            //Caputre snap points here.
			processSnaps(barIndx,X_,Y_);

        }
        if (rotaMouse)
        {
            double xx = e.X - oldXLine;
            double yy = e.Y - oldYLine;
            var flag = false;
            projection.p -= (yy / Canvas.Width) * Math.PI / 16;
            projection.t -= (xx / Canvas.Height) * Math.PI / 16; 
               flag = true;
            if (flag)
            {
                projection.setM();
                canvasRefreh();
            }
        }
        else if (verMouse)
        {
            projection.Y0 += (int)((e.Y - oldYLine) / 20d);
            projection.X0 += (int)((e.X - oldXLine) / 20d);
            projection.setM();
            canvasRefreh();
        }
    }
 
    private void canvasMouseDown(object sender, MouseEventArgs e)
    {
        var s = (modelView)((PictureBox)sender).FindForm().Parent;
        s.setActiveVF(this); 
       
        switch (e.Button)
        {
            case MouseButtons.Left:
                {
                    if(onZoomBox ){
                        p0SelRect = e.Location;
                        onZoomBoxEnga = true;

                        break;
                     }
                    if (!App.onDrawingNewLine && !App.onDrawingNewNode && !App.onMovingNode && !onZoomBox)
                    {
                        selecting = true;
                        p0SelRect = e.Location;
                        trySelect(e.X, e.Y);
                        break;
                    }
                    if (App.onDrawingNewLine && !onZoomBox)
                    {
                        addPointToNewBarQueue(e.X,e.Y);
                        break;
                    }
                    if(App.onDrawingNewNode && currSnapedPoint.Id!=-1 && !currSnapedPoint.IsNode)
                    {
                        App.takeUndoSnapShot();
                        var id = App.model.nodes.Count > 0 ? App.model.nodes.Max(n => n.ID) + 1 : 1;
                        var np = new node() { ID = id, label = $"{id}", 
                            X = currSnapedPoint.x3d, Y = currSnapedPoint.y3d, Z = currSnapedPoint.z3d };
                        App.model.nodes.Add(np);
                        reloadCanvas();
                        break;
                    }

                    if (App.onMovingNode && !App.onDrawingNewNode )
                    {
                        App.takeUndoSnapShot();
                        var id = App.model.nodes.Count > 0 ? App.model.nodes.Max(n => n.ID) + 1 : 1;
                        var np = new node()
                        {
                            ID = id,
                            label = $"{id}",
                            X = currSnapedPoint.x3d,
                            Y = currSnapedPoint.y3d,
                            Z = currSnapedPoint.z3d
                        };

                        int[] n=Selection.getNodesColection();
                        if (n.Length == 0) break;
                        var selNode = App.model.nodes[n[0]];

                        cadTools.nodeMove(ref App.model, selNode, np.X - selNode.X, np.Y - selNode.Y, np.Z - selNode.Z);

                        reloadCanvas();
                        break;
                    }


                    break;
                }
            case MouseButtons.Right:
                {
                    App.onDrawingNewLine = false;
                    App.onDrawingNewNode = false;
                    App.onMovingNode = false;
                   drawingLineFlag = true;
                    reloadCanvas();
                    break;
                }
            case MouseButtons.Middle:
                {
                    if (ctrlKeypress == false)
                    {
                        verMouse = true;
                        oldYLine = e.Y;
                        oldXLine = e.X;
                    }
                    else
                    {
                        verMouse = false;
                        rotaMouse = true;
                        oldYLine = e.Y;
                        oldXLine = e.X;
                    }
                    break;
                }
        }
    }

    private void canvasMouseWheel(object sender, MouseEventArgs e)
    {
        var sign = (e.Delta / 120);
        var dh = 10 * sign;
        var dv = 10 * sign;
        Rectangle rec = Rectangle.FromLTRB(Canvas.Left + dh, Canvas.Top + dv, Canvas.Right - dh, Canvas.Bottom - dv);
        zoomWindowFunction(rec);
        reloadCanvas();
    }
    private void canvasMouseUp(object sender, MouseEventArgs e)
    {
        if (onZoomBoxEnga)
        {
            zoomWindowFunction(selRect);
            canvasRefreh();
        }
        if (selecting && drawingLineFlag && e.Button == MouseButtons.Left)
        {
            space3d.modelViewSelbyRectangle(SnapPoints, selRect, barConecty, p0SelRect.X - (int)e.X < 0, this);
        }
        selecting = false;
        onZoomBox = false;
        onZoomBoxEnga = false;
        verMouse = false;
        rotaMouse = false;
    }


    private void canvasMove(object sender, EventArgs e)
    {
        setupVfControls();
    }
    private void canvasResize(object sender, EventArgs e)
    {
        projection.screenDiag = 
            Math.Sqrt(Canvas.Width* Canvas.Width + Canvas.Height* Canvas.Height);
        setupVfControls();
    }
    private void containerResize(object sender, EventArgs e)
    {
        vfHeight = Container.Height;
        vfWidth = Container.Width;
        canvasResize(sender, e);
    }

    private static bool ctrlKeypress=false;
    private static bool shiftKeypress=false;//currently not used
    private static bool AltKeypress=false;//currently not used
    private void canvasKeyDown(object sender, KeyEventArgs e)
    {
        var md = e.Modifiers;
        ctrlKeypress = md.HasFlag(Keys.Control);
        shiftKeypress = md.HasFlag(Keys.Shift);
        AltKeypress = md.HasFlag(Keys.Alt);

        switch (e.KeyCode)
        {
            //Only for mod keys with no other keys, like in ctr+mouse
            case Keys.ControlKey:
                //Nothing
                break;
            case Keys.ShiftKey:
                Selection.Unselect = true;
                break;
            case Keys.Alt:
                //Nothing
                break;
            //Any other keys combination, like Ctr+Alt+C
            default:

                switch (ctrlKeypress, shiftKeypress, AltKeypress)
                {
                    case (false, false, false):
                        KeysParse(sender, e);
                        break;
                    case (true, false, false):
                        CtrKeysParse(sender, e);
                        break;
                    case (false, true, false):
                        ShiftKeysParse(sender, e);
                        break;
                    case (true, true, false):
                        CtrShiftKeysParse(sender, e);
                        break;
                    case (false,false, true):
                        altKeysParse(sender, e);
                        break;
                }
                break;
        }
    }

    void KeysParse(object sender, KeyEventArgs e)
    {
        
        switch (e.KeyCode)
        {
            case Keys.ControlKey:
               // ctrlKeypress = true;
                break;
            case Keys.ShiftKey:
                Selection.Unselect = true;
                break;
            case Keys.Delete:
                mainWindow.deleteSelected(sender, e);
                break;
            case Keys.Escape:
                mainWindow.clearSelectionBtn(sender, e);
                break;
            case Keys.R:
                mainWindow.rotateSelected(sender, e);
                break;

        }
    }

    void CtrKeysParse(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.A:

                mainWindow.selectAllBtn(sender, e);
                break;
            case Keys.C:

                mainWindow.copySelected(sender, e);
                break;
            case Keys.Z:
                mainWindow.undoLastAction(sender, e);
                break;
            case Keys.R:
                mainWindow.replicateSelected(sender, e);
                break;
            case Keys.M:
                mainWindow.moveSelected(sender, e);
                break;
            case Keys.D:
                mainWindow.divideByLenOrPartsSelectedBars(sender, e);
                break;
            case Keys.S:
                mainWindow.saveFile(sender, e)
;                break;
            case Keys.T:
                mainWindow.showModelInTreeView(sender, e);
                break;

        }
    }


    void ShiftKeysParse(object sender, KeyEventArgs e)
    {
        
        switch (e.KeyCode)
        {
            case Keys.D:
                mainWindow.divideByIntersectsSelectedBars(sender, e);
                break;
        }
    }

    void CtrShiftKeysParse(object sender, KeyEventArgs e)
    {
       
        switch (e.KeyCode)
        {
            case Keys.S:
                mainWindow.saveFileAs(sender, e);
                break;
            case Keys.ControlKey:
                ;
                break;
        }
    }

    void altKeysParse(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.F4:
                mainWindow.Close();
                break;

        }
    }



    private void canvasKeyUp(object sender, KeyEventArgs e)
    {
            ctrlKeypress = false;
            Selection.Unselect = false;
    }
    private void canvasKeyPress(object sender, KeyPressEventArgs e)
    {
        char Key = e.KeyChar;


        switch (Key)
        {
            case 'z':
                projection.z -= 1;
                projection.setM();
                break;
            case 'Z':
                projection.z += 1;
                projection.setM();
                break;
            case 'x':
                projection.x -= 1;
                projection.setM();
                break;
            case 'X':
                projection.x += 1;
                projection.setM();
                break;
            case 'y':
                projection.y -= 1;
                projection.setM();
                break;
            case 'Y':
                projection.y += 1;
                projection.setM();
                break;
            case 'T':
                projection.t -= 0.1;
                projection.setM();
                break;
            case 't':
                projection.t += 0.1;
                projection.setM();
                break;
            case 'p':
                projection.p -= 0.1;
                projection.setM();
                break;
            case 'P':
                projection.p += 0.1;
                projection.setM();
                break;
            case 'a':
                projection.A -= 0.1;
                projection.setM();
                break;
            case 'A':
                projection.A += 0.1;
                projection.setM();
                break;
            case 'b':
                projection.B -= 0.1;
                projection.setM();
                break;
            case 'B':
                projection.B += 0.1;
                projection.setM();
                break;
            case 'c':
                projection.C -= 0.1;
                projection.setM();
                break;
            case 'C':
                projection.C += 0.1;
                projection.setM();
                break;
        }
        reloadCanvas();
        return;
    }
    public void canvasRefreh()
    {
        InvalidateScene();
        canvasBMP = backStageDrawScene(Canvas.Width,Canvas.Height);
        Canvas.Refresh();

    }
    public void planeChange()
    {
        switch (currentView)
        {
            case View.XY:
                {
                projection.p = -Math.PI / 2;
                projection.t = Math.PI / 2; 
                projection.projFocusAng = Math.PI / 2;
                break;
                }
            case View.XZ:
                {
                projection.p = 0;
                projection.t = Math.PI / 2; 
                projection.projFocusAng = Math.PI / 2;
                break;
                }
            case View.YZ:
                {                 
                 projection.p = 0;
                 projection.t = 0;
                 projection.projFocusAng = Math.PI / 2;

                    break;
                }
            case View.ThreeDimensional:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }
        setLevel(0);
        projection.setM();        
        
    }

    private bool levelValidation(double x, double y, double z)
    {
            switch (currentView )
            {
                case View.XY:
                    {
                        if (currentCutPlane < gridZPointsCount)
                        {
                            if (z == currentCutCoord)
                            {
                                return true;
                            }
                        }
                        break;
                    }
                case View.XZ:
                    {
                        if (currentCutPlane < gridYPointsCount)
                        {
                            if (y == currentCutCoord)
                            {
                                return true;
                            }
                        }
                        break;
                    }
                case View.YZ:
                    {
                        if (currentCutPlane < gridXPointsCount)
                        {
                            if (x == currentCutCoord)
                            {
                                return true;
                            }
                        }
                        break;
                    }
                case View.ThreeDimensional:
                    {
                        return true;
                    }
                default:
                    {
                        return true;
                    }
            }
        return false;
        }
    }