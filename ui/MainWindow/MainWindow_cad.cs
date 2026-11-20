using static CEF.modelView;
using static CEF.viewFrame;

namespace CEF;

// Edit/draw handlers: nodes, bars and areas. Geometry logic lives in the CadTools classes (engine).
public partial class MainWindow
{
    const string sc = I18n.SplitChar;
    const string gc = I18n.GroupingKey;

    internal void addNewNode(object sender, EventArgs e)
    {
        var ib = Mis.InputBox(I18n.NewNode, I18n.ImputInstr1);
        if (string.IsNullOrEmpty(ib)) return;

        var coords = ib.Split(sc).Select(c => double.TryParse(c, out double r) ? r : 0d).ToArray();
        if (coords.Length != 3) return;
        App.takeUndoSnapShot();

        var db = Engine.getModel();
        var id = db.nodes.NextID();
        db.nodes.Add(new Node
        {
            ID = id,
            label = $"{id}",
            X = Uc.lenFromUserUnits(coords[0]),
            Y = Uc.lenFromUserUnits(coords[1]),
            Z = Uc.lenFromUserUnits(coords[2])
        });
        db.nodalRestraints.Add(new nodalRestraint { ID = id });

        reloadAllCanvasWindows();
    }

    internal void addNewNodeOnScreen(object sender, EventArgs e) { App.onDrawingNewNode = true; }
    internal void moveNodeOnScreen(object sender, EventArgs e) { App.onMovingNode = true; }
    internal void addNewFrame(object sender, EventArgs e) { App.onDrawingNewLine = true; }
    internal void addNewArea(object sender, EventArgs e) { App.onDrawingNewArea = true; }

    // ---- copy / replicate ----

    static void copyElements(int[] barIds, int[] nodeIds, int[] areaIds, double ofx, double ofy, double ofz)
    {
        var db = Engine.getModel();
        for (int i = 0; i < barIds.Length; i++)
        {
            var b = db.bars.FindByID(barIds[i]);
            if (b != null) BarCadTools.barCopy(db, b, ofx, ofy, ofz);
        }
        for (int i = 0; i < areaIds.Length; i++)
        {
            var a = db.areas.FindByID(areaIds[i]);
            if (a != null) AreaCadTools.AreaCopy(db, a, ofx, ofy, ofz);
        }
        for (int i = 0; i < nodeIds.Length; i++)
        {
            var n = db.nodes.FindByID(nodeIds[i]);
            if (n != null) NodeCadTools.nodeCopy(db, n, ofx, ofy, ofz);
        }
    }

    internal void copySelected(object sender, EventArgs e)
    {
        int[] barIds = selectedBarIds(), nodeIds = selectedNodeIds(), areaIds = selectedAreaIds();
        if (!cadDialogs.askOffset(this, out double ofx, out double ofy, out double ofz)) return;

        App.takeUndoSnapShot();
        copyElements(barIds, nodeIds, areaIds, ofx, ofy, ofz);
        Selection.Clear();
        reloadAllCanvasWindows();
    }

    internal void replicateSelected(object sender, EventArgs e)
    {
        replicateSelectedElems();
    }

    internal void replicateSelectedElems()
    {
        int[] barIds = selectedBarIds(), nodeIds = selectedNodeIds(), areaIds = selectedAreaIds();
        if (!cadDialogs.askReplicate(this,
                out double dx, out double dy, out double dz, out double[] spaces)) return;

        var l = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        if (l == 0) return;

        App.takeUndoSnapShot();
        var cl = 0.0;
        for (int k = 0; k < spaces.Length; k++)
        {
            if (spaces[k] == 0) continue;
            cl += spaces[k];
            copyElements(barIds, nodeIds, areaIds, dx / l * cl, dy / l * cl, dz / l * cl);
        }
        Selection.Clear();
        reloadAllCanvasWindows();
    }

    // ---- move / rotate / delete ----

    internal void moveSelected(object sender, EventArgs e)
    {
        int[] barIds = selectedBarIds(), nodeIds = selectedNodeIds(), areaIds = selectedAreaIds();
        if (!cadDialogs.askOffset(this, out double ofx, out double ofy, out double ofz)) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();

        for (int i = 0; i < barIds.Length; i++)
        {
            var b = db.bars.FindByID(barIds[i]);
            if (b != null) BarCadTools.barMove(db, b, ofx, ofy, ofz);
        }
        for (int i = 0; i < areaIds.Length; i++)
        {
            var a = db.areas.FindByID(areaIds[i]);
            if (a != null) AreaCadTools.AreaMove(db, a, ofx, ofy, ofz);
        }
        // A selected node can disappear while moving (merged with an existing one): FindByID returns null
        for (int i = 0; i < nodeIds.Length; i++)
        {
            var n = db.nodes.FindByID(nodeIds[i]);
            if (n != null) NodeCadTools.nodeMove(db, n, ofx, ofy, ofz);
        }
        Selection.Clear();
        reloadAllCanvasWindows();
    }

    internal void rotateSelected(object sender, EventArgs e)
    {
        //TODO: Fix Bug: nodes shared by several selected elements are rotated more than once
        int[] barIds = selectedBarIds(), areaIds = selectedAreaIds();
        if (barIds.Length == 0 && areaIds.Length == 0) return;

        if (!cadDialogs.askRotation(this,
                out double afx, out double afy, out double afz,
                out double x, out double y, out double z)) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();
        var center = new double[] { x, y, z };

        for (int i = 0; i < barIds.Length; i++)
        {
            var b = db.bars.FindByID(barIds[i]);
            if (b != null) BarCadTools.barRotate(db, b, afx, afy, afz, center);
        }
        for (int i = 0; i < areaIds.Length; i++)
        {
            var a = db.areas.FindByID(areaIds[i]);
            if (a != null) AreaCadTools.AreaRotate(db, a, afx, afy, afz, center);
        }
        Selection.Clear();
        reloadAllCanvasWindows();
    }

    internal void deleteSelected(object sender, EventArgs e)
    {
        int[] barIds = selectedBarIds(), nodeIds = selectedNodeIds(), areaIds = selectedAreaIds();
        if (barIds.Length + nodeIds.Length + areaIds.Length == 0) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();

        // Order matters: NodeDelete only removes nodes that no bar/area uses
        for (int i = 0; i < areaIds.Length; i++)
            AreaCadTools.AreaDelete(db, areaIds[i]);
        for (int i = 0; i < barIds.Length; i++)
            if (db.bars.FindIndexByID(barIds[i]) != -1)
                BarCadTools.BarDelete(db, barIds[i]);
        for (int i = 0; i < nodeIds.Length; i++)
            if (db.nodes.FindIndexByID(nodeIds[i]) != -1)
                NodeCadTools.NodeDelete(db, nodeIds[i]);

        Selection.Clear();
        reloadAllCanvasWindows();
    }
}
