using static CEF.modelView;
using static CEF.viewFrame;

namespace CEF;

// Bar splitting, node cleanup and overlap selection handlers.
public partial class MainWindow
{
    const double minSplitLength = 0.3;  // minimum accepted max-length when splitting by length
    const double nearNodeTol = 0.1;     // distance under which two nodes are considered overlapping

    static barGeom barGeomOf(ModelDB db, Bar b)
    {
        var na = db.nodes.FindByID(b.startNode);
        var nb = db.nodes.FindByID(b.endNode);
        return new barGeom { Bar = b, P1 = na, P2 = nb, len = space3d.calcDist(na, nb) };
    }

    static barGeom[] buildBarGeoms(ModelDB db, int[] barIds)
    {
        var res = new List<barGeom>(barIds.Length);
        for (int i = 0; i < barIds.Length; i++)
        {
            var b = db.bars.FindByID(barIds[i]);
            if (b != null) res.Add(barGeomOf(db, b));
        }
        return res.ToArray();
    }

    // Splits a bar at the given intermediate points (ends are added and everything is ordered along the bar)
    static void splitBarAtPoints(ModelDB db, barGeom g, List<Node> mid)
    {
        var pts = new List<Node> { g.P1 };
        pts.AddRange(mid);
        pts.Add(g.P2);
        pts = pts.Distinct().ToList();
        pts.Sort((x, y) => space3d.calcDist(g.P1, x).CompareTo(space3d.calcDist(g.P1, y)));
        if (pts.Count <= 2) return;
        BarCadTools.divideBarInPoinList(db, g.Bar, pts);
    }

    internal void divideByLenOrPartsSelectedBars(object sender, EventArgs e)
    {
        //TODO: More options.
        var ids = selectedBarIds();
        if (ids.Length == 0) return;

        if (!cadDialogs.askDivide(this, out bool byLength, out int parts, out double val)) return;
        if (byLength && val < minSplitLength) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();
        Selection.Clear();

        for (int i = 0; i < ids.Length; i++)
        {
            var b = db.bars.FindByID(ids[i]);
            if (b == null) continue;
            if (byLength)
                parts = (int)(barGeomOf(db, b).len / val) + 1;
            BarCadTools.barDivide(db, b, -1, parts);
        }
        reloadAllCanvasWindows();
    }

    internal void divideByIntersectsSelectedBars(object sender, EventArgs e)
    {
        //TODO: More options.
        var ids = selectedBarIds();
        if (ids.Length == 0) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();
        var geoms = buildBarGeoms(db, ids);
        Selection.Clear();

        // Phase 1: collect cut points for every bar; phase 2: split (splitting changes the model)
        var mids = new List<Node>[geoms.Length];
        for (int i = 0; i < geoms.Length; i++)
        {
            mids[i] = [];
            for (int j = 0; j < geoms.Length; j++)
            {
                if (geoms[i].Bar.ID == geoms[j].Bar.ID) continue;
                var p = space3d.Intersect(geoms[i], geoms[j]);
                p = space3d.checkNewNode(p, db);
                if (p == null) continue;
                mids[i].Add(p);
            }
        }
        for (int i = 0; i < geoms.Length; i++)
            splitBarAtPoints(db, geoms[i], mids[i]);

        reloadAllCanvasWindows();
    }

    internal void divideByIntersectsSelectedNodes(object sender, EventArgs e)
    {
        var barIds = selectedBarIds();
        var nodeIds = selectedNodeIds();
        if (barIds.Length == 0 || nodeIds.Length == 0) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();
        var geoms = buildBarGeoms(db, barIds);
        var nodes = new List<Node>();
        for (int i = 0; i < nodeIds.Length; i++)
        {
            var n = db.nodes.FindByID(nodeIds[i]);
            if (n != null) nodes.Add(n);
        }
        Selection.Clear();

        var mids = new List<Node>[geoms.Length];
        for (int i = 0; i < geoms.Length; i++)
        {
            mids[i] = [];
            for (int j = 0; j < nodes.Count; j++)
                if (space3d.NodeIntersect(geoms[i], nodes[j]) != null)
                    mids[i].Add(nodes[j]);
        }
        for (int i = 0; i < geoms.Length; i++)
            splitBarAtPoints(db, geoms[i], mids[i]);

        reloadAllCanvasWindows();
    }

    internal void joinSelectedBars(object sender, EventArgs e)
    {
        // TODO: merge collinear selected bars into one. (The previous code only repeated
        // "split at intersections with selected bars", so it was removed.)
    }

    internal void deleteOrfanNodes(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();
        var db = Engine.getModel();

        var used = new HashSet<int>();
        for (int i = 0; i < db.bars.Count; i++)
        {
            used.Add(db.bars[i].startNode);
            used.Add(db.bars[i].endNode);
        }
        for (int i = 0; i < db.areas.Count; i++)
            foreach (var id in AreaCadTools.AreaNodeIds(db.areas[i]))
                used.Add(id);

        var toDelete = new List<int>();
        for (int i = 0; i < db.nodes.Count; i++)
            if (!used.Contains(db.nodes[i].ID)) toDelete.Add(db.nodes[i].ID);

        for (int i = 0; i < toDelete.Count; i++)
            NodeCadTools.NodeDelete(db, toDelete[i]);

        reloadAllCanvasWindows();
    }

    // IDs of nodes that overlap an earlier node (the first node of each group is not included)
    static List<int> findNearNodeIds(ModelDB db)
    {
        var dup = new List<int>();
        var visited = new HashSet<int>();
        for (int i = 0; i < db.nodes.Count; i++)
        {
            var n = db.nodes[i];
            if (!visited.Add(n.ID)) continue;
            for (int j = 0; j < db.nodes.Count; j++)
            {
                var p = db.nodes[j];
                if (visited.Contains(p.ID)) continue;
                if (space3d.calcDist(n, p) >= nearNodeTol) continue;
                dup.Add(p.ID);
                visited.Add(p.ID);
            }
        }
        return dup;
    }

    internal void joinNearNodes(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();
        var db = Engine.getModel();
        var dup = findNearNodeIds(db);
        for (int i = 0; i < dup.Count; i++)
            if (db.nodes.FindIndexByID(dup[i]) != -1)
                NodeCadTools.NodeDelete(db, dup[i]);
        reloadAllCanvasWindows();
    }

    internal void SelectNearNodes(object sender, EventArgs e)
    {
       // App.takeUndoSnapShot();
        var db = Engine.getModel();
        var dup = findNearNodeIds(db);
        for (int i = 0; i < dup.Count; i++)
            Selection.nodeAdd(db.nodes.FindIndexByID(dup[i]));
        reloadAllCanvasWindows();
    }

    internal void selectOverlapedBars(object sender, EventArgs e)
    {
        //App.takeUndoSnapShot();
        var db = Engine.getModel();

        // An end node of one bar lying inside the other bar (end nodes of that same bar do not count)
        Node endInside(barGeom b, Node n) =>
            (b.Bar.startNode == n.ID || b.Bar.endNode == n.ID) ? null : space3d.NodeIntersect(b, n);

        var toSelect = new List<int>();
        var visited = new HashSet<int>();
        for (int i = 0; i < db.bars.Count; i++)
        {
            var b1 = db.bars[i];
            if (visited.Contains(b1.ID)) continue;
            var bg1 = barGeomOf(db, b1);

            var overlaps = new List<int>();
            for (int j = 0; j < db.bars.Count; j++)
            {
                var b2 = db.bars[j];
                if (b2.ID == b1.ID) continue;
                var bg2 = barGeomOf(db, b2);

                bool sameNodes =
                    (b1.startNode == bg2.P1.ID || b1.endNode == bg2.P1.ID) &&
                    (b1.startNode == bg2.P2.ID || b1.endNode == bg2.P2.ID);

                if (sameNodes ||
                    endInside(bg1, bg2.P1) != null || endInside(bg1, bg2.P2) != null ||
                    endInside(bg2, bg1.P1) != null || endInside(bg2, bg1.P2) != null)
                    overlaps.Add(b2.ID);
            }
            toSelect.AddRange(overlaps);
            visited.Add(b1.ID);
            visited.UnionWith(overlaps);
        }

        if (toSelect.Count == 0)
        {
            MessageBox.Show(I18n.NoOverlappedBarsFound);
            return;
        }

        for (int i = 0; i < toSelect.Count; i++)
            Selection.barAdd(db.bars.FindIndexByID(toSelect[i]));

        getActiveModelView().reloadCanvas();
    }
}
