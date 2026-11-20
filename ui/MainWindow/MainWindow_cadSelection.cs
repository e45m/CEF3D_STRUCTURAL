using static CEF.modelView;
using static CEF.viewFrame;

namespace CEF;

// Selection helpers and select/clear handlers.
// Selection works with table indices; indices shift when elements are added or removed,
// so every handler captures IDs first and resolves the elements by ID afterwards.
public partial class MainWindow
{
    static int[] idsFromIndices(int[] sel, int count, Func<int, int> idAt)
    {
        if (sel == null) return [];
        var ids = new List<int>(sel.Length);
        for (int i = 0; i < sel.Length; i++)
            if (sel[i] >= 0 && sel[i] < count)
                ids.Add(idAt(sel[i]));
        return ids.ToArray();
    }

    internal static int[] selectedBarIds()
    {
        var t = Engine.getModel().bars;
        return idsFromIndices(Selection.getBarsColection(), t.Count, i => t[i].ID);
    }

    internal static int[] selectedNodeIds()
    {
        var t = Engine.getModel().nodes;
        return idsFromIndices(Selection.getNodesColection(), t.Count, i => t[i].ID);
    }

    internal static int[] selectedAreaIds()
    {
        var t = Engine.getModel().areas;
        return idsFromIndices(Selection.getAreasColection(), t.Count, i => t[i].ID);
    }

    // ASSUMPTION: Selection.areaAdd(int index) exists. Change only this line if the name differs.
    static void selectAreaIndex(int i) => Selection.areaAdd(i);

    internal void clearSelectionBtn(object sender, EventArgs e)
    {
        Selection.Clear();
        reloadAllCanvasWindows();
    }

    internal void selectAllBtn(object sender, EventArgs e)
    {
        var db = Engine.getModel();
        Selection.Clear();
        for (int i = 0; i < db.bars.Count; i++)
            Selection.barAdd(i);
        for (int i = 0; i < db.nodes.Count; i++)
            Selection.nodeAdd(i);
        for (int i = 0; i < db.areas.Count; i++)
            selectAreaIndex(i);
        reloadAllCanvasWindows();
    }
}
