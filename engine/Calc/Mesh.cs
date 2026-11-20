namespace CEF;

internal static  class Mesh
{
   // private  ModelDB  mDB;

//TODO: Capture group mesh info... the parent element and their chindren
//table format for easy indexing like idParente. IdCHild---All Main DB elements can be parent, but a child cannot
//a subdivision of a child becme also chid parent`s child

   static ModelDB MeshedModel=null;

   public static void setModel(ModelDB Model){MeshedModel = Model;}//.Clone();}
   public static ModelDB getModel() =>MeshedModel;

   public static void MeshFrames(int parts=-1) {   
    //Divide intersection with points and other bars
    MeshBarByNparts(parts);
    MeshBarIntesectNodes();
    MeshIntesectBars();
    }

   // Area counterpart of MeshFrames.
   // Order matters: bars are split where they pierce an area first, so the
   // nodes created there are then used to cut the areas.
   public static void MeshAreas(int parts=-1) {
    MeshAreaByNparts(parts);
    MeshIntesectBarsAreas();
    MeshAreaIntesectNodes();
    }

    static void MeshBarByNparts(int parts)
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



        BarCadTools.barDivide( MeshedModel ,
            b,
            -1,
            parts);
    }

    }

    static void MeshBarIntesectNodes() 
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
            var na = MeshedModel.nodes.FindByID(b.startNode);
            var nb = MeshedModel.nodes.FindByID(b.endNode);
            selBars[j++] = new barGeom()
            {
                Bar = b,
                P1 = na,
                P2 = nb,
                len = space3d.calcDist(na, nb)
            };
        }
        j = 0;
        foreach (int i in selectedNods)
            selNods[j++] = MeshedModel.nodes[i];


        for (var i = 0; i < n; i++)
        {
            var b = selBars[i];
            Puntos[b.Bar.ID] = [];

            foreach (var nd in selNods)
            {
                var r = space3d.NodeIntersect(b, nd);
                if (r == null) continue;

                Puntos[b.Bar.ID].Add(nd);
            }
         
            Puntos[b.Bar.ID] = Puntos[b.Bar.ID].Distinct().ToList();
            Puntos[b.Bar.ID].Sort((x, y) => space3d.calcDist(b.P1, x).CompareTo(space3d.calcDist(b.P1, y)));
        }

        for (var i = 0; i < n; i++)
        {

            if (Puntos[selBars[i].Bar.ID].Count > 2) {

            BarCadTools.divideBarInPoinList( MeshedModel, selBars[i].Bar, Puntos[selBars[i].Bar.ID]);
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
            var na = MeshedModel.nodes.FindByID(b.startNode);
            var nb = MeshedModel.nodes.FindByID(b.endNode);
            selBars[j++] = new barGeom()
            {
                Bar = b,
                P1 = na,
                P2 = nb,
                len = space3d.calcDist(na, nb)
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
                intersect = space3d.checkNewNode(intersect,MeshedModel );
                if (intersect == null) continue;
                Puntos[b.Bar.ID].Add(intersect);
            }
            Puntos[b.Bar.ID].Add(b.P2);
            Puntos[b.Bar.ID] = Puntos[b.Bar.ID].Distinct().ToList();
            Puntos[b.Bar.ID].Sort((x, y) => space3d.calcDist(b.P1, x).CompareTo(space3d.calcDist(b.P1, y)));
        }

        for (var i = 0; i < n; i++){

            if (Puntos[selBars[i].Bar.ID].Count <= 2) continue;
            
            BarCadTools.divideBarInPoinList( MeshedModel, selBars[i].Bar,  Puntos[selBars[i].Bar.ID]);            
                }

    }

    //Areas

    // Divides every area in parts x parts sub-areas.
    static void MeshAreaByNparts(int parts)
    {
        if (parts < 1) return; // Only if a division parameter is set

        var areas = MeshedModel.areas;
        var n = areas.Count;
        var sid = areas.Select(a => a.ID).ToArray();

        for (var i = 0; i < n; i++)
        {
            // Snapshot of IDs: areas created by the division are not divided again
            int idx = areas.FindIndexByID(sid[i]);
            if (idx == -1) continue;

            AreaCadTools.AreaDivide(MeshedModel, areas[idx], parts, parts);
        }
    }

    // Cuts each area with the nodes that lie inside it (area counterpart of MeshBarIntesectNodes).
    // AreaDivideInPoinList ignores nodes outside the area or off its plane.
    // Several passes: the nodes created on the edges of a divided area can
    // cut its neighbours, so it repeats until no area is divided.
    static void MeshAreaIntesectNodes(int maxPasses = 8)
    {
        for (int pass = 0; pass < maxPasses; pass++)
        {
            var n = MeshedModel.areas.Count;
            var m = MeshedModel.nodes.Count;

            var selectedAreas = Enumerable.Range(0, n).ToArray();
            var selectedNods = Enumerable.Range(0, m).ToArray();

            var selAreas = new Area[n];
            var selNods = new Node[m];

            int j = 0;
            foreach (int i in selectedAreas)
                selAreas[j++] = MeshedModel.areas[i];
            j = 0;
            foreach (int i in selectedNods)
                selNods[j++] = MeshedModel.nodes[i];

            var Puntos = new List<Node>(selNods);

            for (var i = 0; i < n; i++)
            {
                var a = selAreas[i];
                if (MeshedModel.areas.FindIndexByID(a.ID) == -1) continue;

                AreaCadTools.AreaDivideInPoinList(MeshedModel, a, Puntos);
            }

            if (MeshedModel.areas.Count == n) break; // nothing was divided
        }
    }

    // Splits the bars that pierce an area (area counterpart of MeshIntesectBars).
    // Bars lying in the area plane or touching it only at an end node are not handled here.
    static void MeshIntesectBarsAreas()
    {
        const double tol = 1e-6;

        var n = MeshedModel.bars.Count;
        var k = MeshedModel.areas.Count;

        var selectedBars = Enumerable.Range(0, n).ToArray();
        var selectedAreas = Enumerable.Range(0, k).ToArray();

        var selBars = new barGeom[n];
        var selAreas = new Area[k];

        int j = 0;
        foreach (int i in selectedBars)
        {
            var b = MeshedModel.bars[i];
            var na = MeshedModel.nodes.FindByID(b.startNode);
            var nb = MeshedModel.nodes.FindByID(b.endNode);
            selBars[j++] = new barGeom()
            {
                Bar = b,
                P1 = na,
                P2 = nb,
                len = space3d.calcDist(na, nb)
            };
        }
        j = 0;
        foreach (int i in selectedAreas)
            selAreas[j++] = MeshedModel.areas[i];

        // Corners and plane frame of every area (computed once)
        var corners = new Node[k][];
        var e1s = new double[k][];
        var e2s = new double[k][];
        var nrms = new double[k][];
        var okFrame = new bool[k];
        for (int i = 0; i < k; i++)
        {
            var ids = AreaCadTools.AreaNodeIds(selAreas[i]);
            corners[i] = new Node[4];
            for (int c = 0; c < 4; c++)
                corners[i][c] = MeshedModel.nodes.FindByID(ids[c]);
            okFrame[i] = AreaFrame(corners[i], out e1s[i], out e2s[i], out nrms[i]);
        }

        Dictionary<int, List<Node>> Puntos = [];

        for (var i = 0; i < n; i++)
        {
            var b = selBars[i];
            Puntos[b.Bar.ID] = [];
            Puntos[b.Bar.ID].Add(b.P1);

            double dx = b.P2.X - b.P1.X, dy = b.P2.Y - b.P1.Y, dz = b.P2.Z - b.P1.Z;

            for (int a = 0; a < k; a++)
            {
                if (!okFrame[a]) continue;
                var c0 = corners[a][0];
                var nr = nrms[a];

                // Signed distances of both bar ends to the area plane
                double da = (b.P1.X - c0.X) * nr[0] + (b.P1.Y - c0.Y) * nr[1] + (b.P1.Z - c0.Z) * nr[2];
                double db = (b.P2.X - c0.X) * nr[0] + (b.P2.Y - c0.Y) * nr[1] + (b.P2.Z - c0.Z) * nr[2];
                if (Math.Abs(da) < tol || Math.Abs(db) < tol || da * db > 0) continue;

                double t = da / (da - db);
                double x = b.P1.X + t * dx, y = b.P1.Y + t * dy, z = b.P1.Z + t * dz;

                if (!PointInArea(corners[a], e1s[a], e2s[a], nr, x, y, z, tol)) continue;

                // Avoid duplicates (area edges shared by neighbours give the same point)
                bool dup = Math.Abs(x - b.P2.X) < tol && Math.Abs(y - b.P2.Y) < tol && Math.Abs(z - b.P2.Z) < tol;
                foreach (var q in Puntos[b.Bar.ID])
                    if (Math.Abs(x - q.X) < tol && Math.Abs(y - q.Y) < tol && Math.Abs(z - q.Z) < tol) { dup = true; break; }
                if (dup) continue;

                Puntos[b.Bar.ID].Add(new Node { X = x, Y = y, Z = z });
            }

            Puntos[b.Bar.ID].Add(b.P2);
            Puntos[b.Bar.ID].Sort((p, q) => space3d.calcDist(b.P1, p).CompareTo(space3d.calcDist(b.P1, q)));
        }

        for (var i = 0; i < n; i++)
        {
            if (Puntos[selBars[i].Bar.ID].Count <= 2) continue;

            BarCadTools.divideBarInPoinList(MeshedModel, selBars[i].Bar, Puntos[selBars[i].Bar.ID]);
        }
    }

    // Plane frame of an area: e1 = node1->node2, e2 = node1->node4, unit normal.
    static bool AreaFrame(Node[] c, out double[] e1, out double[] e2, out double[] nrm)
    {
        e1 = new[] { c[1].X - c[0].X, c[1].Y - c[0].Y, c[1].Z - c[0].Z };
        e2 = new[] { c[3].X - c[0].X, c[3].Y - c[0].Y, c[3].Z - c[0].Z };
        nrm = new[] {
            e1[1] * e2[2] - e1[2] * e2[1],
            e1[2] * e2[0] - e1[0] * e2[2],
            e1[0] * e2[1] - e1[1] * e2[0]
        };
        double nl = Math.Sqrt(nrm[0] * nrm[0] + nrm[1] * nrm[1] + nrm[2] * nrm[2]);
        if (nl < 1e-12) return false; // degenerate area
        nrm[0] /= nl; nrm[1] /= nl; nrm[2] /= nl;
        return true;
    }

    // True if (x,y,z) lies on the area plane and inside its (parallelogram) bounds.
    static bool PointInArea(Node[] c, double[] e1, double[] e2, double[] nrm,
        double x, double y, double z, double tol)
    {
        double[] d = { x - c[0].X, y - c[0].Y, z - c[0].Z };
        if (Math.Abs(d[0] * nrm[0] + d[1] * nrm[1] + d[2] * nrm[2]) > tol) return false;

        double a11 = e1[0] * e1[0] + e1[1] * e1[1] + e1[2] * e1[2];
        double a12 = e1[0] * e2[0] + e1[1] * e2[1] + e1[2] * e2[2];
        double a22 = e2[0] * e2[0] + e2[1] * e2[1] + e2[2] * e2[2];
        double det = a11 * a22 - a12 * a12;
        if (Math.Abs(det) < 1e-12) return false;

        double b1 = d[0] * e1[0] + d[1] * e1[1] + d[2] * e1[2];
        double b2 = d[0] * e2[0] + d[1] * e2[1] + d[2] * e2[2];
        double u = (b1 * a22 - b2 * a12) / det;
        double v = (b2 * a11 - b1 * a12) / det;

        return u >= -tol && u <= 1 + tol && v >= -tol && v <= 1 + tol;
    }

}