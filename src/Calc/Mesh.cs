namespace CEF;

internal static  class Mesh
{
   // private  ModelDB  mDB;
   static ModelDB MeshedModel=null;

   public static void setModel(ModelDB Model){MeshedModel = Model;}//.Clone();}
   public static ModelDB getModel() =>MeshedModel;

   public static void MeshFrames(int parts=-1) {   
    //Divide intersection with points and other bars
    MeshByNparts(parts);
    MeshIntesectNodes();
    MeshIntesectBars();
    }

    static void MeshByNparts(int parts)
    {
    if (parts<1) return; //Only if there are some parameter set for division of the elements

    var bars = MeshedModel.bars;
    var n = bars.Count;
    var sid = bars.Select(b=>b.ID).ToArray();    

    for (var i = 0; i < n; i++)
    {
        var b = bars.FirstOrDefault(
            b => b.ID == sid[i],
            new Bar { ID = -1 });
        if (sid[i] == -1 || b.ID == -1) break;



        cadTools.barDivide(ref MeshedModel ,
            b,
            -1,
            parts);
    }

    }

    static void MeshIntesectNodes() 
    {

        var n = MeshedModel.bars.Count;
        var m = MeshedModel.nodes.Count;

        var selectedBars  = Enumerable.Range(0, n).ToArray();  
        var selectedNods = Enumerable.Range(0, m).ToArray();

        var selBars = new barGeom[n];
        var selNods = new Node[m];      
  

        Dictionary<int, List<Node>> Puntos = [];
        int j = 0;
        foreach (int i in selectedBars)
        {
            var b = MeshedModel.bars[i];
            var na = MeshedModel.nodes[MeshedModel.nodes.FindIndexByID(b.startNode)];
            var nb = MeshedModel.nodes[MeshedModel.nodes.FindIndexByID(b.endNode)];
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
            selNods[j++] = MeshedModel.nodes[i];


        for (var i = 0; i < n; i++)
        {
            var b = selBars[i];
            Puntos[b.Bar.ID] = [];

           // Puntos[b.Bar.ID].Add(b.P1);
            foreach (var nd in selNods)
            {
                var r = space3d.NodeIntersect(b, nd);
                if (r == null) continue;

                Puntos[b.Bar.ID].Add(nd);
            }
           // Puntos[b.Bar.ID].Add(b.P2);
            Puntos[b.Bar.ID] = Puntos[b.Bar.ID].Distinct().ToList();
            Puntos[b.Bar.ID].Sort((x, y) => Mis.calcDist(b.P1, x).CompareTo(Mis.calcDist(b.P1, y)));
        }

        for (var i = 0; i < n; i++)
        {

            if (Puntos[selBars[i].Bar.ID].Count > 2) {

            cadTools.divideBarInPoinList(ref MeshedModel, selBars[i].Bar, Puntos[selBars[i].Bar.ID]);
            }
        }



    }
    static void MeshIntesectBars() 
    {
        var n = MeshedModel.bars.Count;
        var m = MeshedModel.nodes.Count;

        var selectedBars  = Enumerable.Range(0, n).ToArray();  
        var selectedNods = Enumerable.Range(0, m).ToArray();

        var selBars = new barGeom[n];
        var selNods = new Node[m];      

        Dictionary<int , List<Node>> Puntos = [];

        int j = 0;
        foreach(int i in selectedBars)
        {
            var b = MeshedModel.bars[i];
            var na = MeshedModel.nodes[MeshedModel.nodes.FindIndexByID(b.startNode)];
            var nb = MeshedModel.nodes[MeshedModel.nodes.FindIndexByID(b.endNode)];
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
            selNods[j++] = MeshedModel.nodes[i];

        int k = MeshedModel.nodes.Max(n => n.ID) + 1;

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
            
            cadTools.divideBarInPoinList(ref MeshedModel, selBars[i].Bar,  Puntos[selBars[i].Bar.ID]);            
                }

    }

}



