using CEF.matrixImpl;
using static CEF.modelView;
using static CEF.viewFrame;

namespace CEF;

// View handlers: zoom, active coordinate system and information dialogs.
public partial class MainWindow
{
    internal void loadCoordSysNamesList(object sender, EventArgs e)
    {
        var db = Engine.getModel();
        if (db == null) return;

        var send = (ToolStripComboBox)sender;
        var names = db.gridCoordsX.Select(c => c.coordSys).Distinct().ToArray();
        send.Items.Clear();
        send.Items.AddRange(names);
    }

    internal void choosedCoordSysCNChanged(object sender, EventArgs e)
    {
        App.currCoordSist = ((ToolStripComboBox)sender).Text;
        var listChild = OwnedForms.Where(x => x is modelView).ToList();
        foreach (var f in listChild)
            ((modelView)f).ViewFrame.coordSystemChanged();

        space3d.createGrid(Engine.getModel(), Engine.getAppData(), App.currCoordSist,
            ref App.g_gridXPointsCount, ref App.g_gridYPointsCount, ref App.g_gridZPointsCount);

        reloadAllCanvasWindows();
    }
        internal viewFrame getActiveVF()
    {
        
        var listChild = OwnedForms.Where(x => x is modelView).ToList();
        foreach (var f in listChild)          
            if( ((modelView)f).ViewFrame.activeVF== VFTyes.Active ||((modelView)f).ViewFrame.activeVF== VFTyes.Single) 
                return ((modelView)f).ViewFrame;  

        return null;        
    }


        internal void barActionChange(object sender, EventArgs e)
    {
        var mv =getActiveVF();
        if (mv is null) return;
        mv.ms.barAction =(BarActions) ((ToolStripComboBox)sender).SelectedIndex;
        mv.reloadCanvas();

    }
        internal void areaActionChange(object sender, EventArgs e)
    {
       var mv =getActiveVF();
        if (mv is null) return;
        mv.ms.areaAction =(AreaAction) ((ToolStripComboBox)sender).SelectedIndex;
        mv.reloadCanvas();
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
        var db = Engine.getModel();

        // Nodes of the selection: selected nodes + end nodes of selected bars + corners of selected areas
        var nodeIds = new HashSet<int>(selectedNodeIds());
        foreach (var id in selectedBarIds())
        {
            var b = db.bars.FindByID(id);
            if (b == null) continue;
            nodeIds.Add(b.startNode);
            nodeIds.Add(b.endNode);
        }
        foreach (var id in selectedAreaIds())
        {
            var a = db.areas.FindByID(id);
            if (a == null) continue;
            foreach (var nid in AreaCadTools.AreaNodeIds(a))
                nodeIds.Add(nid);
        }
        if (nodeIds.Count == 0) return;

        var pts = new List<point3D>();
        foreach (var nid in nodeIds)
        {
            var n = db.nodes.FindByID(nid);
            if (n == null) continue;
            var p3d = new point3D() { x3d = n.X, y3d = n.Y, z3d = n.Z };
            mv.p3DSet2DCoord(ref p3d);
            pts.Add(p3d);
        }
        mv.zoomObjectFunction(pts);
        mv.reloadCanvas();
    }

    internal void measureDist2Nodes(object sender, EventArgs e)
    {
        var ids = selectedNodeIds();
        if (ids.Length != 2)
        {
            MessageBox.Show(I18n.OutPutInstr1, I18n.Distance);
            return;
        }
        var db = Engine.getModel();
        var na = db.nodes.FindByID(ids[0]);
        var nb = db.nodes.FindByID(ids[1]);
        var dis = Math.Round(space3d.calcDist(na, nb), 3);
        var text = $"{I18n.Nodes}: \n" +
                   $"n1: {na.ID} n2: {nb.ID} l = {Uc.lenToUserUnits(dis)} \n";
        MessageBox.Show(text, I18n.Distance);
    }

    internal void showSelectBarsInfo(object sender, EventArgs e)
    {
        var ids = selectedBarIds();
        if (ids.Length == 0) return;
        var db = Engine.getModel();

        var text = $"{I18n.Info}: \n";
        for (int i = 0; i < ids.Length; i++)
        {
            var ba = db.bars.FindByID(ids[i]);
            if (ba == null) continue;
            var na = db.nodes.FindByID(ba.startNode);
            var nb = db.nodes.FindByID(ba.endNode);
            var dis = Math.Round(space3d.calcDist(na, nb), 3);
            text += $"{I18n.Bar}  {ba.ID} n1:  {na.ID} n2:  {nb.ID}   l = {Uc.lenToUserUnits(dis)} \n";
        }
        MessageBox.Show(text, I18n.Info);
    }

    internal void showSelectAreasInfo(object sender, EventArgs e)
    {
        var ids = selectedAreaIds();
        if (ids.Length == 0) return;
        var db = Engine.getModel();

        var text = $"{I18n.Info}: \n";
        for (int i = 0; i < ids.Length; i++)
        {
            var a = db.areas.FindByID(ids[i]);
            if (a == null) continue;
            var n = AreaCadTools.AreaNodeIds(a);
            text += $"{I18n.Area}  {a.ID} n1: {n[0]} n2: {n[1]} n3: {n[2]} n4: {n[3]}   {I18n.SectionName}: {a.SectionName} \n";
        }
        MessageBox.Show(text, I18n.Info);
    }
}
