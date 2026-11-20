using System.Data.Common;
using System.Diagnostics;

namespace CEF;

static public class AreaCadTools
{
    // ------------------------------------------------------------------
    // Area <-> Node access helpers.
    // ASSUMPTION: Area exposes node1..node4 (int node IDs), label,
    // SectionID and SectionName. If your field names differ, change ONLY
    // AreaNodeIds, SetAreaNodeIds and NewAreaLike.
    // ------------------------------------------------------------------
    internal static int[] AreaNodeIds(Area area)
    {
        return new[] { area.Node1, area.Node2, area.Node3, area.Node4 };
    }

    private static void SetAreaNodeIds(Area area, int[] ids)
    {
        area.Node1 = ids[0];
        area.Node2 = ids[1];
        area.Node3 = ids[2];
        area.Node4 = ids[3];
    }

    // Creates a new area (new ID) that keeps the properties of src.
    private static Area NewAreaLike(ModelDB db, Area src, int[] ids)
    {
        var a = new Area();
        a.ID = db.areas.NextID();
        a.label = $"{a.ID}";
        a.SectionID = src.SectionID;
        a.SectionName = src.SectionName;
        SetAreaNodeIds(a, ids);
        return a;
    }

    private static Node[] AreaNodes(ModelDB db, Area area)
    {
        var ids = AreaNodeIds(area);
        var nodes = new Node[4];
        for (int i = 0; i < 4; i++)
        {
            int id = ids[i];
            nodes[i] = db.nodes.FindByID( id);
        }
        return nodes;
    }

    // Returns the node at (x, y, z); creates it (with a free restraint) if it does not exist.
    private static Node GetOrCreateNode(ModelDB db, double x, double y, double z, out bool created)
    {
        var p = new Node { X = x, Y = y, Z = z };
        var exists = NodeCadTools.IsNode(db, p);
        if (exists != -1)
        {
            created = false;
            return db.nodes.FindByID(exists);
        }
        p.ID = db.nodes.NextID();
        p.label = $"{p.ID}";
        db.nodes.Add(p);
        db.nodalRestraints.Add(new nodalRestraint { ID = p.ID });
        created = true;
        return p;
    }

    // Copies the six restraint flags (1 = restrained, 0 = free) from one node to another.
    private static void CopyRestraint(ModelDB db, int fromNodeId, int toNodeId)
    {
        var src = db.nodalRestraints.FindByID( fromNodeId);
        var dst = db.nodalRestraints.FindByID( toNodeId);
        if (src == null || dst == null) return;
        dst.ux = src.ux;
        dst.uy = src.uy;
        dst.uz = src.uz;
        dst.rx = src.rx;
        dst.ry = src.ry;
        dst.rz = src.rz;
    }

    // Replaces an area with a grid of sub-areas.
    // uCuts / vCuts: sorted parameters in [0,1], both including 0 and 1.
    // u runs from node1 to node2, v runs from node1 to node4.
    private static bool ReplaceAreaByGrid(ModelDB db, Area area, List<double> uCuts, List<double> vCuts)
    {
        int idx = db.areas.FindIndexByID(area.ID);
        if (idx == -1) return false;

        var n = AreaNodes(db, area);
        int nu = uCuts.Count;
        int nv = vCuts.Count;
        var grid = new int[nv, nu];

        for (int r = 0; r < nv; r++)
        {
            double v = vCuts[r];
            for (int c = 0; c < nu; c++)
            {
                double u = uCuts[c];
                double w1 = (1 - u) * (1 - v), w2 = u * (1 - v), w3 = u * v, w4 = (1 - u) * v;
                double x = w1 * n[0].X + w2 * n[1].X + w3 * n[2].X + w4 * n[3].X;
                double y = w1 * n[0].Y + w2 * n[1].Y + w3 * n[2].Y + w4 * n[3].Y;
                double z = w1 * n[0].Z + w2 * n[1].Z + w3 * n[2].Z + w4 * n[3].Z;
                grid[r, c] = GetOrCreateNode(db, x, y, z, out _).ID;
            }
        }

        var newAreas = new List<Area>();
        for (int r = 0; r < nv - 1; r++)
            for (int c = 0; c < nu - 1; c++)
            {
                var ids = new[] { grid[r, c], grid[r, c + 1], grid[r + 1, c + 1], grid[r + 1, c] };
                newAreas.Add(NewAreaLike(db, area, ids));
            }

        // Area loads are not supported yet: nothing to redistribute here.
        db.areas.RemoveAt(idx);
        foreach (var a in newAreas)
        {
            a.ID = db.areas.NextID();
            a.label = $"{a.ID}";
            db.areas.Add(a);
        }
        return true;
    }

    //Area

    public static bool AreaDelete(ModelDB db, int id)
    {
        var idx = db.areas.FindIndexByID(id);
        if (idx == -1) return false; // Maybe already deleted or bad ID
        db.areas.RemoveAt(idx);
        return true;
    }

    // NOTE: areas do not support loads yet. These stay as placeholders
    // until area load collections exist in ModelDB.
    public static bool AreaDistrLoadDelete(ModelDB db, int Id)
    {
        return false;
    }
    public static bool AreaPunLoadDelete(ModelDB db, int Id)
    {
        return false;
    }
    public static bool AreaCoordPatLoadDelete(ModelDB db, int Id)
    {
        return false;
    }

    public static bool AreaCopy(ModelDB db, Area area, double ofx, double ofy, double ofz,
    bool copyLoads = true, bool copyConstraints = true)
    {
        if (db.areas.FindIndexByID(area.ID) == -1) return false; // Maybe deleted or bad send

        var old = AreaNodes(db, area);
        var ids = new int[4];
        for (int i = 0; i < 4; i++)
        {
            var nn = GetOrCreateNode(db, old[i].X + ofx, old[i].Y + ofy, old[i].Z + ofz, out bool created);
            ids[i] = nn.ID;
            // Only freshly created nodes get the restraints of their source node;
            // pre-existing nodes keep their own.
            if (created && copyConstraints) CopyRestraint(db, old[i].ID, nn.ID);
        }

        // TODO: when area loads exist, copy them here if copyLoads is true.

        db.areas.Add(NewAreaLike(db, area, ids));
        return true;
    }

    public static bool AreaMove(ModelDB db, Area area, double ofx, double ofy, double ofz)
    {
        int idx = db.areas.FindIndexByID(area.ID);
        if (idx == -1) return false; // Maybe deleted or bad send

        var old = AreaNodes(db, area);
        var ids = new int[4];
        for (int i = 0; i < 4; i++)
            ids[i] = GetOrCreateNode(db, old[i].X + ofx, old[i].Y + ofy, old[i].Z + ofz, out _).ID;

        // Original nodes are left untouched (same criterion as barMove).
        SetAreaNodeIds(db.areas[idx], ids);
        return true;
    }

    public static bool AreaRotate(ModelDB db, Area area, double afx, double afy, double afz, double[] rp)
    {
        int idx = db.areas.FindIndexByID(area.ID);
        if (idx == -1) return false;

        var old = AreaNodes(db, area);
        var MT = CadToolsUtils.getTMatrix(afx, afy, afz);

        // Target positions, computed before moving anything
        var target = new Node[4];
        for (int i = 0; i < 4; i++)
        {
            var v = new double[,] { { old[i].X - rp[0] }, { old[i].Y - rp[1] }, { old[i].Z - rp[2] }, { 1 } };
            var r = MatrixMath.MT_mult(MT, v);
            target[i] = new Node
            {
                X = r[0, 0] + rp[0],
                Y = r[1, 0] + rp[1],
                Z = r[2, 0] + rp[2]
            };
        }

        // Same criterion as barRotate: move the nodes, then relink by position.
        for (int i = 0; i < 4; i++)
            NodeCadTools.nodeMove(db, old[i], target[i].X - old[i].X, target[i].Y - old[i].Y, target[i].Z - old[i].Z);

        var ids = new int[4];
        for (int i = 0; i < 4; i++)
        {
            int nid = NodeCadTools.IsNode(db, target[i]);
            if (nid == -1) return false;
            ids[i] = nid;
        }
        SetAreaNodeIds(db.areas[idx], ids);
        return true;
    }

    // nX: divisions along node1 -> node2 (local u).
    // nY: divisions along node1 -> node4 (local v).
    // Produces nX * nY new areas.
    public static bool AreaDivide(ModelDB db, Area area, double nX, double nY)
    {
        int nx = (int)Math.Round(nX);
        int ny = (int)Math.Round(nY);
        if (nx < 1 || ny < 1) return true;
        if (nx == 1 && ny == 1) return true;

        var uCuts = new List<double>();
        var vCuts = new List<double>();
        for (int i = 0; i <= nx; i++) uCuts.Add((double)i / nx);
        for (int j = 0; j <= ny; j++) vCuts.Add((double)j / ny);

        return ReplaceAreaByGrid(db, area, uCuts, vCuts);
    }

    // Divides the area with a cut line through each node of pList (one along u, one along v).
    // Nodes outside the area (or off its plane) are ignored.
    // The local (u,v) mapping assumes a planar, parallelogram-like area.
   public static bool AreaDivideInPoinList(ModelDB db, Area area, List<Node> pList, double minSize = 0.5)
{
    if (db.areas.FindIndexByID(area.ID) == -1) return false;
    if (pList == null || pList.Count == 0) return true;

    const double tol = 1e-6;
    var n = AreaNodes(db, area);

    double[] e1 = { n[1].X - n[0].X, n[1].Y - n[0].Y, n[1].Z - n[0].Z }; // u direction
    double[] e2 = { n[3].X - n[0].X, n[3].Y - n[0].Y, n[3].Z - n[0].Z }; // v direction

    double a11 = e1[0] * e1[0] + e1[1] * e1[1] + e1[2] * e1[2];
    double a12 = e1[0] * e2[0] + e1[1] * e2[1] + e1[2] * e2[2];
    double a22 = e2[0] * e2[0] + e2[1] * e2[1] + e2[2] * e2[2];
    double det = a11 * a22 - a12 * a12;
    if (Math.Abs(det) < 1e-6) return false; // degenerate area

    // Plane normal
    double[] nrm = {
        e1[1] * e2[2] - e1[2] * e2[1],
        e1[2] * e2[0] - e1[0] * e2[2],
        e1[0] * e2[1] - e1[1] * e2[0]
    };
    double nl = Math.Sqrt(nrm[0] * nrm[0] + nrm[1] * nrm[1] + nrm[2] * nrm[2]);
    if (nl < tol) return false; // degenerate normal
    nrm[0] /= nl; nrm[1] /= nl; nrm[2] /= nl;

    // Edge lengths, used to convert minSize into parametric units
    double lenU = Math.Sqrt(a11);
    double lenV = Math.Sqrt(a22);
    if (lenU < tol || lenV < tol) return false;

    double effMin = minSize > tol ? minSize : tol;
    double minU = effMin / lenU;
    double minV = effMin / lenV;

    var uCuts = new List<double>();
    var vCuts = new List<double>();

    for (int i = 0; i < pList.Count; i++)
    {
        Node p = pList[i];
        double[] d = { p.X - n[0].X, p.Y - n[0].Y, p.Z - n[0].Z };

        // Off-plane distance
        double dist = Math.Abs(d[0] * nrm[0] + d[1] * nrm[1] + d[2] * nrm[2]);
        if (dist > tol) continue;

        double b1 = d[0] * e1[0] + d[1] * e1[1] + d[2] * e1[2];
        double b2 = d[0] * e2[0] + d[1] * e2[1] + d[2] * e2[2];
        double u = (b1 * a22 - b2 * a12) / det;
        double v = (b2 * a11 - b1 * a12) / det;

        if (u < -tol || u > 1 + tol || v < -tol || v > 1 + tol) continue; // outside

        if (u > tol && u < 1 - tol) uCuts.Add(u);
        if (v > tol && v < 1 - tol) vCuts.Add(v);
    }

    // Drop cuts that would create strips thinner than minSize (also removes duplicates)
    List<double> uFinal = FilterCuts(uCuts, minU);
    List<double> vFinal = FilterCuts(vCuts, minV);

    if (uFinal.Count == 2 && vFinal.Count == 2) return true; // nothing to cut

    return ReplaceAreaByGrid(db, area, uFinal, vFinal);
}

// Sorts the cuts and keeps only those leaving every strip (including both borders) >= minParam
private static List<double> FilterCuts(List<double> cuts, double minParam)
{
    cuts.Sort();

    var result = new List<double>();
    result.Add(0d);
    double last = 0d;

    for (int i = 0; i < cuts.Count; i++)
    {
        double c = cuts[i];
        if (c - last < minParam) continue;  // too close to previous cut
        if (1d - c < minParam) continue;    // too close to far border
        result.Add(c);
        last = c;
    }

    result.Add(1d);
    return result;
}

}