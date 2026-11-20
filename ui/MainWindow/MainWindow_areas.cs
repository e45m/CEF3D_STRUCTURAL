using static CEF.modelView;
using static CEF.viewFrame;

namespace CEF;

// Area handlers. Real ones call AreaCadTools; the rest are TODO placeholders
// (their menu items are disabled in Cef3DWindowBase.menu.cs: switch todoItem -> menuItem when ready).
public partial class MainWindow
{
    internal void divideAreasXY(object sender, EventArgs e)
    {
        var ids = selectedAreaIds();
        if (ids.Length == 0) return;
        if (!cadAreaDialogs.askDivideArea(this, out int nx, out int ny)) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();
        Selection.Clear();

        for (int i = 0; i < ids.Length; i++)
        {
            var a = db.areas.FindByID(ids[i]);
            if (a != null) AreaCadTools.AreaDivide(db, a, nx, ny);
        }
        reloadAllCanvasWindows();
    }

    internal void divideAreasAtSelectedNodes(object sender, EventArgs e)
    {
        var areaIds = selectedAreaIds();
        var nodeIds = selectedNodeIds();
        if (areaIds.Length == 0 || nodeIds.Length == 0) return;

        App.takeUndoSnapShot();
        var db = Engine.getModel();
        var nodes = new List<Node>();
        for (int i = 0; i < nodeIds.Length; i++)
        {
            var n = db.nodes.FindByID(nodeIds[i]);
            if (n != null) nodes.Add(n);
        }
        Selection.Clear();

        for (int i = 0; i < areaIds.Length; i++)
        {
            var a = db.areas.FindByID(areaIds[i]);
            if (a != null) AreaCadTools.AreaDivideInPoinList(db, a, nodes);
        }
        reloadAllCanvasWindows();
    }

    // ---- TODO placeholders ----

    // TODO: areas sharing the same four nodes, or a corner lying inside another area
    internal void selectOverlapedAreas(object sender, EventArgs e) {

        //Thas because UI shows a difference in color
        //May be unnecesary.

   }


    // TODO: pick an area section and assign it to the selected areas (Area.SectionID / SectionName)
    internal void setAreasSectionDlg(object sender, EventArgs e) {
            var w = new setAreaSection();
            w.Show(this);

     }
/*
    // TODO: area loads need new tables in ModelDB (see AreaCadTools.AreaDistrLoadDelete and related)
    internal void setDistrAreaLoadsDlg(object sender, EventArgs e) { }
    internal void setPunctAreaLoadsDlg(object sender, EventArgs e) { }
    internal void setCoordPatAreaLoadsDlg(object sender, EventArgs e) { }

    // TODO: edge releases for areas
    internal void setAreaReleasesDlg(object sender, EventArgs e) { }*/
    
    internal void areaForcesTable(object sender, EventArgs e) { 
        var m = Engine.getModel();
        var tablename = nameof(m.areaInternalForces);
        var vbd = new viewModelDBTable([tablename], false);
        vbd.Show(this);

    }
    internal void areaForcesGraphView(object sender, EventArgs e) {

       var vw = new viewInternalAreaActions();       
       vw.Show(this);
     }
    internal void InverAreaAxes(object sender, EventArgs e) {
        App.takeUndoSnapShot();
        AreaAxes.flipAreaZ(Engine.getModel(), Selection.getAreasColection());

     }
    internal void AligneAtraWithFist(object sender, EventArgs e) {

        App.takeUndoSnapShot();
        AreaAxes.alignAreaAxes(Engine.getModel(), Selection.getAreasColection());
     }
    internal void ShiftAreaNodes(object sender, EventArgs e) {
        App.takeUndoSnapShot();
        AreaAxes.rotateAreaOrigin(Engine.getModel(), Selection.getAreasColection());

     }
}
