using static CEF.modelView;
using static CEF.viewFrame;
namespace CEF;
public class Selection
{
    private static HashSet<int>  selectedNodes;
    private static HashSet<int>  selectedBars;
    private static HashSet<int>  selectedAreas;
    private static HashSet<int>  selectedSolids;
    private static Dictionary<int,int> nodesIDList;
    public static int nodesMaxIndx { get { return selectedNodes.Count-1; } }
    public static int barsMaxIndx { get { return selectedBars.Count-1; }  }
    public static int barMaxIndex { get { return selectedBars.Count-1; }  }
    public static int areaMaxIndx { get { return selectedAreas.Count-1; }  }
    public static int solidMaxIndx { get { return selectedSolids.Count-1; }  }
    private static bool initizalizedGuardFlag = false;
    public static bool Unselect = false;
    public Selection()
    {
        if (!initizalizedGuardFlag)
        {
            selectedBars = new();
            selectedNodes = new();
            selectedAreas = new();
            selectedSolids = new();
            nodesIDList = new();
            initizalizedGuardFlag = true;
            Unselect = false;
        }
    }
    public static void nodeAdd(int Indice)
    {
        if (!selectedNodes.Add(Indice) && Unselect)
            selectedNodes.Remove(Indice);
    }
    public static void barAdd(int Indice)
    {
        if ( !selectedBars.Add(Indice) && Unselect)
            selectedBars.Remove(Indice);
    }
    public static void areaAdd(int Indice)
    {
        if ( !selectedAreas.Add(Indice) && Unselect)
            selectedAreas.Remove(Indice);
    }
    public static void solidAdd(int Indice)
    {
            if ( !selectedSolids.Add(Indice) && Unselect) 
            selectedSolids.Remove(Indice);
    }
    public static void Clear()
    {
        selectedBars = [];
        selectedNodes = [];
        selectedAreas = [];
        selectedSolids = [];
    }
    public static int[]? getNodesColection()
    {
        if (nodesMaxIndx != -1)
        {
            return selectedNodes.ToArray();
        }
        return null;
    }
    public static int[]? getBarsColection()
    {
        if (barsMaxIndx != -1)
        {
            return selectedBars.ToArray();
        }
        return null;

    }
    public static int[]? getAreasColection()
    {
        if (areaMaxIndx != -1)
        {
            return selectedAreas.ToArray();
        }
        return null;

    }
    public static int[]? getSolidsColection()
    {
        if (solidMaxIndx != -1)
        {
            return selectedSolids.ToArray();
        }
        return null;

    }
    public static int setIndexListPoints(point3D[] snappoints)
    {
        int i;
        int n;
        n = snappoints.Length - 1;
        nodesIDList = [];
        for (i = 0; i <= n; i++)
           { nodesIDList.Add(i, snappoints[i].Id); 
        }
        return i;
    }
    public static int getIdFromIndex(int index)
    {
        if (index > nodesIDList.Count- 1)
            return -1;
        return nodesIDList[index];
    }
}