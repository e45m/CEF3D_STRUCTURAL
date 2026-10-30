using CEF.matrixImpl;
using System.Runtime.Intrinsics.X86;
using static CEF.modelView;
using static CEF.viewFrame;
using static System.Math;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.LinkLabel;

namespace CEF;
public partial class MainWindow
{
    const string sc = I18n.SplitChar;
    const string gc = I18n.GroupingKey;

    internal void clearSelectionBtn(object sender, EventArgs e)
    {
        Selection.Clear();
        reloadAllCanvasWindows();
    }

    internal void selectAllBtn(object sender, EventArgs e)
    {

        var barCount = App.model.bars.Count;
        var nodeCount = App.model.nodes.Count;
        //other elements
        Selection.Clear();
        for (int i = 0; i < barCount; i++)
            Selection.barAdd(i);
        for (int i = 0; i < nodeCount; i++)
            Selection.nodeAdd(i);

        reloadAllCanvasWindows();
    }

    internal void addNewNode(object sender, EventArgs e)
    {
        var ib = Mis.InputBox(I18n.NewNode, I18n.ImputInstr1 );

        var coords = ib.Split(sc).Select(c => {if( double.TryParse(c, out double r)) return r;return 0d; }).ToArray();
        if (coords.Length != 3) return;
        App.takeUndoSnapShot();

        var id = App.model.nodes.NextID();
        var np = new Node() { ID = id,label=$"{id}", X= Uc.lenFromUserUnits(coords[0]), 
                                                     Y = Uc.lenFromUserUnits(coords[1]),
                                                     Z= Uc.lenFromUserUnits(coords[2])};
        App.model.nodes.Add(np);
        var nr = new nodalRestraint { ID = id };
        App.model.nodalRestraints.Add(nr);

        reloadAllCanvasWindows();

    }
    internal void addNewNodeOnScreen(object sender, EventArgs e)
    {
        App.onDrawingNewNode = true;

    }
    internal void moveNodeOnScreen(object sender, EventArgs e)
    {
        App.onMovingNode = true;

    }


    internal void addNewFrame(object sender, EventArgs e)
    {
        App.onDrawingNewLine = true ;
       
    }
    internal void measureDist2Nodes(object sender, EventArgs e)
    {
       

        var n1 = Selection.nodesMaxIndx + 1;
        var S1 = Selection.getNodesColection();

        var text = "";

        if (n1 == 2)
        {
                text += $"{I18n.Nodes}: \n";
                var na = App.model.nodes[S1[0]];
                var nb = App.model.nodes[S1[1]];
                var dis = Math.Round(Mis.calcDist(na, nb), 3);
                text += $"n1: {na.ID} n2: {nb.ID} l = {Uc.lenToUserUnits(dis)} \n";

            MessageBox.Show(text, I18n.Distance);
        }
        else
        {
            MessageBox.Show(I18n.OutPutInstr1, I18n.Distance);
        }

    }
    internal void showSelectBarsInfo(object sender, EventArgs e)
    {
        var n = Selection.barsMaxIndx + 1;
        var S = Selection.getBarsColection();

        var n1 = Selection.nodesMaxIndx + 1;
        var S1 = Selection.getNodesColection();

        var text = "";

        if (n > 0)
        {
            text += $"{I18n.Info}: \n";
            foreach (int i in S)
            {
                var ba = App.model.bars[i];
                var na = App.model.nodes.First(p => p.ID == ba.startNode);
                var nb = App.model.nodes.First(p => p.ID == ba.endNode);
                var dis = Math.Round(Mis.calcDist(na, nb), 3);
                text += $"{I18n.Bar}  {ba.ID} n1:  {na.ID} n2:  {nb.ID}   l = {Uc.lenToUserUnits(dis)} \n" ;

            }
                MessageBox.Show(text, I18n.Info);
        }

    }


    internal void loadCoordSysNamesList(object sender, EventArgs e)
   {
       string dumm;
        var send = (ToolStripComboBox)sender;
       send.Items.Clear();
        dumm = App.model.gridCoordsX[0].coordSys;
        send.Items.Add(dumm);
        

        var cases = App.model.gridCoordsX.
            Select(c => c.coordSys).Distinct().ToArray();
        
        send.Items.Clear();
        send.Items.AddRange(cases);

   }
    internal void choosedCoordSysCNChanged(object sender, EventArgs e)
    {
        List<Form> listChild;
        App.currCoordSist = ((ToolStripComboBox)sender).Text;
        listChild = OwnedForms.Where(x => x is modelView).ToList(); 
        foreach (var currentA in listChild)
        {
            ((modelView)currentA).ViewFrame.coordSystemChanged();
        }
        space3d.createGrid(this);
        reloadAllCanvasWindows();
    }
        internal void copySelected(object sender, EventArgs e)
    {
        var n = Selection.barsMaxIndx + 1;
        var S = new int[n];
        var sid = Selection.getBarsColection();

        if (!cadDialogs.askOffset(this, out double ofx, out double ofy, out double ofz)) return;

        App.takeUndoSnapShot();

        for (int i = 0; i < n; i++)
        {
            if (S[i] == -1) break;
            sid[i] = App.model.bars[S[i]].ID;
        }
        for (var i = 0; i < n; i++)
        {
            if (S[i] == -1) break;
            cadTools.barCopy(ref App.model, App.model.bars[S[i]], ofx, ofy, ofz);
        }
        n = Selection.nodesMaxIndx + 1;
        S = new int[n];
        sid = Selection.getNodesColection();
        for (int i = 0; i < n; i++)
        {
            if (S[i] == -1) break;
            sid[i] = App.model.nodes[S[i]].ID;
        }
        for (var i = 0; i < n; i++)
        {
            if (S[i] == -1) break;
            cadTools.nodeCopy(ref App.model, App.model.nodes[S[i]], ofx, ofy, ofz);
        }
        Selection.Clear();
        reloadAllCanvasWindows();
    }

  
    internal void moveSelected(object sender, EventArgs e)
    {
        if (!cadDialogs.askOffset(this, out double ofx, out double ofy, out double ofz)) return;

        App.takeUndoSnapShot();

        var nb = Selection.barsMaxIndx + 1;
        var sidb = new List<Bar>();
        var Sb = Selection.getBarsColection();
        var excludedIDNodes = new HashSet<int>();

        for (int i = 0; i < nb; i++)
        {
            if (Sb[i] == -1) break;
            sidb.Add(App.model.bars[Sb[i]]);

        }
        foreach(var b in sidb)
       {
            cadTools.barMove(ref App.model, b, ofx, ofy, ofz);
        }
        var nn = Selection.nodesMaxIndx + 1;
        var sidn = new List<Node>();
        var Sn = Selection.getNodesColection();
        for (int i = 0; i < nn; i++)
        {
            if (Sn[i] == -1||excludedIDNodes.Contains(App.model.nodes[Sn[i]].ID)) break;
            sidn.Add( App.model.nodes[Sn[i]]);
        }
        foreach (var n in sidn)
        {            
            cadTools.nodeMove(ref App.model, n, ofx, ofy, ofz);
        }
        Selection.Clear();
        reloadAllCanvasWindows();
    }
    internal void deleteSelected(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        var n = Selection.barsMaxIndx + 1;
        var S = Selection.getBarsColection();
        var sid = new int[n];
        
        for (int i = 0; i < n; i++)
        {
            if (S[i] == -1) break;
            sid[i] = App.model.bars[S[i]].ID;
        }
        for (var i = 0; i < n; i++)
        {
            if (sid[i] == -1) break;
            cadTools.BarDelete(ref App.model, sid[i]);
        }
        n = Selection.nodesMaxIndx + 1;
        S = Selection.getNodesColection();        
        sid = new int[n];
        for (int i = 0; i < n; i++)
        {
            if (S[i] == -1) break;
            sid[i] = App.model.nodes[S[i]].ID;
        }
        for (var i = 0; i < n; i++)
        {
            if (sid[i] == -1) break;
            cadTools.NodeDelete(ref App.model, sid[i]);
        }
        Selection.Clear();
       
        reloadAllCanvasWindows();
    }
     internal void rotateSelected(object sender, EventArgs e)
    {
        //TODO: Fix Bug
        var n = Selection.barsMaxIndx + 1;
        var S = Selection.getBarsColection();
        var sid = new Bar[n];
        Selection.Clear();

        if (!cadDialogs.askRotation(this,
                out double afx, out double afy, out double afz,
                out double x, out double y, out double z)) return;

        App.takeUndoSnapShot();

        for (int i = 0; i < n; i++)
        {
            if (S[i] == -1) break;
            sid[i] = App.model.bars[S[i]];
        }
        for (var i = 0; i < n; i++)
        {           
            cadTools.barRotate(ref App.model, sid[i], afx, afy, afz,[ x, y, z ]);
        }

        reloadAllCanvasWindows();
    }

    internal void replicateSelected(object sender, EventArgs e)
    {
        replicateSelectedElems();
    }
    internal void replicateSelectedElems()
    {
        var n = Selection.barsMaxIndx + 1;
        var S = Selection.getBarsColection();
        var sid = new int[n];        
        var n1 = Selection.nodesMaxIndx + 1;
        var S1 = Selection.getNodesColection();
        var sid1 = new int[n1];       

        if (!cadDialogs.askReplicate(this,
                out double dx, out double dy, out double dz, out double[] spaces)) return;

        App.takeUndoSnapShot();

        var l = Sqrt(dx * dx + dy * dy + dz * dz);
           
        if (l == 0) return;
        var ofxv = dx / l;
        var ofyv = dy / l;
        var ofzv = dz / l;
        var cl = 0.0;
        foreach (var s in spaces)
        {
            if (s == 0) continue;
            cl += s;
            var ofx = ofxv * cl;
            var ofy = ofyv * cl;
            var ofz = ofzv * cl;
            for (int i = 0; i < n; i++)
            {
                if (S[i] == -1) break;
                sid[i] = App.model.bars[S[i]].ID;
            }
            for (var i = 0; i < n; i++)
            {
                if (S[i] == -1) break;
                cadTools.barCopy(ref App.model, App.model.bars[S[i]], ofx, ofy, ofz);
            }
            for (int i = 0; i < n1; i++)
            {
                if (S1[i] == -1) break;
                sid1[i] = App.model.nodes[S1[i]].ID;
            }
            for (var i = 0; i < n1; i++)
            {
                if (S1[i] == -1) break;
                cadTools.nodeCopy(ref App.model, App.model.nodes[S1[i]], ofx, ofy, ofz);
            }
        }
        Selection.Clear();
        reloadAllCanvasWindows();
    }

    internal void zoomBox(object sender, EventArgs e)
    {
        var mv = getActiveModelView();
        if (mv is null) return;
        mv.onZoom();
    }
    internal void zommAll(object sender, EventArgs e)
    {
        var mv = getActiveModelView();
        if (mv is null) return;
        mv.zoomAll();
        mv.projection.setM();
        mv.reloadCanvas();
    }
    internal void zoomToSelected(object sender, EventArgs e)
    {
        var mv = getActiveModelView();
        if (mv is null) return;
        var np = Selection.nodesMaxIndx + 1;
        var nb = Selection.barMaxIndex + 1;
        int[] points = Selection.getNodesColection();

        int[] bars =  Selection.getBarsColection();
        var pointBars = bars?.ToList().SelectMany(b =>
        {
            var Bar = App.model.bars.First(br => br.ID == b);
            return new[] { Bar.startNode, Bar.endNode };
        }).ToList()?? null;

        if (points == null && pointBars == null) return;

        List<int> points2 = [];
        if (points == null)
            points2 = pointBars;
        if (pointBars == null)
            points2 = points?.ToList()??[0];
        if(points!=null && pointBars!=null)
            points2 = points.Concat(pointBars).Distinct().ToList();
        
        var vP = points2.Select(p =>
        {
            var n = App.model.nodes.First(n => n.ID == p);
            var p3d = new point3D() { x3d = n.X, y3d = n.Y, z3d = n.Z };
            mv.p3DSet2DCoord(ref p3d); 
            return p3d;
        }).ToList();
        mv.zoomObjectFunction(vP);        
        mv.reloadCanvas();
    }
  
    internal void divideByLenOrPartsSelectedBars(object sender, EventArgs e)
    {
        //TODO: More options.
                
        var n = Selection.barsMaxIndx + 1;
        var S = Selection.getBarsColection();
        var sid = new int[n];

        if (!cadDialogs.askDivide(this, out bool dist, out int div_Section, out double val)) return;

        if (dist && val < 0.3) return;//0.01 units no matters the real unit is relative 

        App.takeUndoSnapShot();
     
        for (int i = 0; i < n; i++)
            sid[i] = App.model.bars[S[i]].ID;
        Selection.Clear();
        for (var i = 0; i < n; i++)
        {
            var b = App.model.bars.FirstOrDefault(
                b => b.ID == sid[i],
                new Bar { ID = -1 });
            if (sid[i] == -1 || b.ID == -1) break;

            if (dist)
            {
                var na = App.model.nodes[App.model.nodes.FindIndexByID(b.startNode)];
                var nb = App.model.nodes[App.model.nodes.FindIndexByID(b.endNode)];
                var len = Mis.calcDist(na,nb);

                div_Section =(int) (len / val)+1;

            }

            cadTools.barDivide(ref App.model,
                b,
                -1,
                div_Section);
        }
    }

    internal void divideByIntersectsSelectedNodes(object sender, EventArgs e)
    {
        var n = Selection.barsMaxIndx + 1;
        var m = Selection.nodesMaxIndx + 1;
        int[] selectedBars = Selection.getBarsColection();
        int[] selectedNods = Selection.getNodesColection();

        var selBars = new barGeom[n];
        var selNods = new Node[m];

        App.takeUndoSnapShot();

        Dictionary<int, List<Node>> Puntos = [];
        int j = 0;
        foreach (int i in selectedBars)
        {
            var b = App.model.bars[i];
            var na = App.model.nodes[App.model.nodes.FindIndexByID(b.startNode)];
            var nb = App.model.nodes[App.model.nodes.FindIndexByID(b.endNode)];
            selBars[j++] = new barGeom()
            {
                Bar = b,
                P1 = na,
                P2 = nb,
                len = Mis.calcDist(na, nb)
            };
        }
        j = 0;
        foreach (int i in selectedNods)
            selNods[j++] = App.model.nodes[i];

        Selection.Clear();

        for (var i = 0; i < n; i++)
        {
            var b = selBars[i];
            Puntos[b.Bar.ID] = [];

            Puntos[b.Bar.ID].Add(b.P1);
            foreach (var nd in selNods)
            {
                var r = space3d.NodeIntersect(b, nd);
                if (r == null) continue;

                Puntos[b.Bar.ID].Add(nd);
            }
            Puntos[b.Bar.ID].Add(b.P2);
            Puntos[b.Bar.ID] = Puntos[b.Bar.ID].Distinct().ToList();
            Puntos[b.Bar.ID].Sort((x, y) => Mis.calcDist(b.P1, x).CompareTo(Mis.calcDist(b.P1, y)));
        }

        for (var i = 0; i < n; i++)
        {

            if (Puntos[selBars[i].Bar.ID].Count <= 2) continue;

            cadTools.divideBarInPoinList(ref App.model, selBars[i].Bar, Puntos[selBars[i].Bar.ID]);
        }


    }

    internal void divideByIntersectsSelectedBars(object sender, EventArgs e)
    {
        //TODO: More options.
    
        var n = Selection.barsMaxIndx + 1;
        var m = Selection.nodesMaxIndx + 1;
        int[] selectedBars = Selection.getBarsColection();
        int[] selectedNods = Selection.getNodesColection();

        var selBars = new barGeom[n];
        var selNods = new Node[m];
      
        App.takeUndoSnapShot();

        Dictionary<int , List<Node>> Puntos = [];

        int j = 0;
        foreach(int i in selectedBars)
        {
            var b = App.model.bars[i];
            var na = App.model.nodes[App.model.nodes.FindIndexByID(b.startNode)];
            var nb = App.model.nodes[App.model.nodes.FindIndexByID(b.endNode)];
            selBars[j++] = new barGeom()
            {
                Bar = b,
                P1 = na,
                P2 = nb,
                len = Mis.calcDist(na, nb)
            };
          }
        j = 0;
        foreach (int i in selectedNods)
            selNods[j++] = App.model.nodes[i];

        Selection.Clear();
        int k = App.model.nodes.Max(n => n.ID) + 1;

        for (var i = 0; i < n; i++)
        {
            var b = selBars[i];
            Puntos[b.Bar.ID] = [];

            Puntos[b.Bar.ID].Add(b.P1);
            foreach (var br in selBars)
            {
                if (b.Bar.ID == br.Bar.ID) continue;

                var intersect = space3d.Intersect(b, br);
                intersect = space3d.checkNewNode(intersect);
                if (intersect == null) continue;
                Puntos[b.Bar.ID].Add(intersect);
            }
            Puntos[b.Bar.ID].Add(b.P2);
            Puntos[b.Bar.ID] = Puntos[b.Bar.ID].Distinct().ToList();
            Puntos[b.Bar.ID].Sort((x, y) => Mis.calcDist(b.P1, x).CompareTo(Mis.calcDist(b.P1, y)));
        }

        for (var i = 0; i < n; i++){

            if (Puntos[selBars[i].Bar.ID].Count <= 2) continue;
            
            cadTools.divideBarInPoinList(ref App.model, selBars[i].Bar, Puntos[selBars[i].Bar.ID]);            
                }
    }

    internal void joinSelectedBars(object sender, EventArgs e)
    {
        //TODO: More options.

        var n = Selection.barsMaxIndx + 1;
        var m = Selection.nodesMaxIndx + 1;
        int[] selectedBars = Selection.getBarsColection();
        int[] selectedNods = Selection.getNodesColection();

        var selBars = new barGeom[n];
        var selNods = new Node[m];

        App.takeUndoSnapShot();

        Dictionary<int, List<Node>> Puntos = [];

        int j = 0;
        foreach (int i in selectedBars)
        {
            var b = App.model.bars[i];
            var na = App.model.nodes[App.model.nodes.FindIndexByID(b.startNode)];
            var nb = App.model.nodes[App.model.nodes.FindIndexByID(b.endNode)];
            selBars[j++] = new barGeom()
            {
                Bar = b,
                P1 = na,
                P2 = nb,
                len = Mis.calcDist(na, nb)
            };
        }
        j = 0;
        foreach (int i in selectedNods)
            selNods[j++] = App.model.nodes[i];

        Selection.Clear();
        int k = App.model.nodes.Max(n => n.ID) + 1;


        for (var i = 0; i < n; i++)
        {
            var b = selBars[i];
            Puntos[b.Bar.ID] = [];

            Puntos[b.Bar.ID].Add(b.P1);
            foreach (var br in selBars)
            {
                if (b.Bar.ID == br.Bar.ID) continue;

                var intersect = space3d.Intersect(b, br);
                intersect = space3d.checkNewNode(intersect);
                if (intersect == null) continue;
                Puntos[b.Bar.ID].Add(intersect);
            }
            Puntos[b.Bar.ID].Add(b.P2);
            Puntos[b.Bar.ID] = Puntos[b.Bar.ID].Distinct().ToList();
            Puntos[b.Bar.ID].Sort((x, y) => Mis.calcDist(b.P1, x).CompareTo(Mis.calcDist(b.P1, y)));
        }

        for (var i = 0; i < n; i++)
        {

            if (Puntos[selBars[i].Bar.ID].Count <= 2) continue;

            cadTools.divideBarInPoinList(ref App.model, selBars[i].Bar, Puntos[selBars[i].Bar.ID]);
        }
    }

    internal void deleteOrfanNodes(object sender, EventArgs e)
    {
        var bars = App.model.bars;
        //other elems

        App.takeUndoSnapShot();
        var usedNodes = bars.SelectMany(b => new int[] { b.endNode, b.endNode }).ToHashSet();
        //usedNodes.Sort();
        var toDelete = new List<int>();
        foreach (var n in App.model.nodes)
            if (!usedNodes.Contains(n.ID)) toDelete.Add(n.ID);

        foreach (var n in toDelete)
            cadTools.NodeDelete(ref App.model, n);
    }

    internal void joinNearNodes(object sender, EventArgs e)
    {

        App.takeUndoSnapShot();
        var toDelete = new List<int>();
        var visited = new List<int>();
		double tol = 0.1;
        foreach (var n in App.model.nodes)
        {
            if (visited.Contains(n.ID)) continue;

            var nearNodes = App.model.nodes.Where(p =>
            {
                var ideq = p.ID != n.ID;
                var poseq = Mis.calcDist(n,p);
                return ideq && (poseq< tol);
            }).Select(p=>p.ID).ToList();
            toDelete.AddRange(nearNodes);            
            visited.Add(n.ID);
            visited.AddRange(nearNodes);
            
        }
           

        foreach (var n in toDelete)
           cadTools.NodeDelete(ref App.model, n);
    }
    internal void SelectNearNodes(object sender, EventArgs e)
    {

        App.takeUndoSnapShot();
        var toDelete = new List<int>();
        var visited = new List<int>();
		double tol = 0.1;
        foreach (var n in App.model.nodes)
        {
            if (visited.Contains(n.ID)) continue;

            var nearNodes = App.model.nodes.Where(p =>
            {
                var ideq = p.ID != n.ID;
                var poseq = Mis.calcDist(n, p);
                return ideq && (poseq < tol);
            }).Select(p => p.ID).ToList();
            toDelete.AddRange(nearNodes);
            visited.Add(n.ID);
            visited.AddRange(nearNodes);
        }

        foreach (var n in toDelete)
            Selection.nodeAdd(App.model.nodes.FindIndexByID( n));
    }
    internal void selectOverlapedBars(object sender, EventArgs e)
    {

        App.takeUndoSnapShot();
        var toSelect = new List<int>();
        var visited = new List<int>();
        foreach (var b1 in App.model.bars)
        {
            if (visited.Contains(b1.ID)) continue;
            var ni1 = App.model.nodes.First(n => n.ID == b1.startNode);
            var nj1 = App.model.nodes.First(n => n.ID == b1.endNode);
            var bg1 = new barGeom() { Bar = b1, P1 = ni1, P2 = nj1 };

            var nearNodes = App.model.bars.Where(b2 =>
            {
               if ( b2.ID == b1.ID) return false;

                var ni2 = App.model.nodes.First(n => n.ID == b2.startNode);
                var nj2 = App.model.nodes.First(n => n.ID == b2.endNode);
                var bg2 = new barGeom() { Bar = b2, P1 = ni2, P2 = nj2 };

                if ((bg1.Bar.startNode == ni2.ID || bg1.Bar.endNode == ni2.ID) &&
                    (bg1.Bar.startNode == nj2.ID || bg1.Bar.endNode == nj2.ID)) 
                  return true;

                Node checkIntersect(barGeom b, Node n)
                {
                  return ( b.Bar.startNode == n.ID || b.Bar.endNode == n.ID)? null: space3d.NodeIntersect(b, n);

                }
                var p1 = checkIntersect(bg1, ni2);
                var p2 = checkIntersect(bg1, nj2);
                var p3 = checkIntersect(bg2, ni1);
                var p4 = checkIntersect(bg2, nj1);

                if (p1 != null || p2 != null || p3!=null || p4!= null)
                    return true;

                return false;

            }).Select(p => p.ID).ToList();

            toSelect.AddRange(nearNodes);
            visited.Add(b1.ID);
            visited.AddRange(nearNodes);

        }
        if (toSelect.Count == 0)
        {
            MessageBox.Show(I18n.NoOverlappedBarsFound);
            return;
        }

        foreach (var n in toSelect)
            Selection.barAdd(App.model.bars.FindIndexByID(n));

        getActiveModelView().reloadCanvas();

    }
}