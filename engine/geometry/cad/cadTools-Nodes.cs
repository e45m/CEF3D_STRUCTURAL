namespace CEF;

// Engine: node operations only. Bars -> BarCadTools, areas -> AreaCadTools, shared math -> CadToolsUtils.
static public class NodeCadTools
{
    public static bool NodeDelete(ModelDB db, Node p)
    {
        // A node is free when no bar and no area uses it
        var isFree = !db.bars.Any(b => b.endNode == p.ID || b.startNode == p.ID)
                  && !db.areas.Any(a => Array.IndexOf(AreaCadTools.AreaNodeIds(a), p.ID) >= 0);
        if (isFree)
        {
            var idx = db.nodes.FindIndexByID(p.ID);
            var id = p.ID;

            var ridx = db.nodalRestraints.FindIndexByID(id);
            if (ridx != -1)
                db.nodalRestraints.RemoveAt(ridx);

            var cns = db.nodalLoads.Where(c => c.NodeID == id).ToList();
            foreach (var c in cns)
            {
                var cidx = db.nodalLoads.FindIndexByID(c.ID);
                db.nodalLoads.RemoveAt(cidx);
            }
            var idlist = db.nodalConstraints.Where(rc =>
            {
                return rc.nodesIDList.Split(",").ToList().Any(i => i == $"{id}");
            });
            foreach (var rc in idlist)
            {
                var sepp = rc.nodesIDList.Split(",").Where(i => i != $"{id}");
                rc.nodesIDList = string.Join(",", sepp);
            }
            if (idx != -1)
                db.nodes.RemoveAt(idx);
        }
        return true;
    }

    public static bool NodeDelete(ModelDB db, int id)
    {
        var idx = db.nodes.FindIndexByID(id);
        if (idx == -1) return false;
        return NodeDelete(db, db.nodes[idx]);
    }

    public static bool NodeLoadDelete(ModelDB db, nodalLoad p)
    {
        var cidx = db.nodalLoads.FindIndexByID(p.ID);
        db.nodalLoads.RemoveAt(cidx);
        return true;
    }

    public static bool NodeLoadDelete(ModelDB db, int id)
    {
        var idx = db.nodalLoads.FindIndexByID(id);
        var l = db.nodalLoads[idx];
        return NodeLoadDelete(db, l);
    }

    public static int IsNode(ModelDB db, Node p, double tol = 0.01)
    {
        var id = -1;
        var n = db.nodes.FirstOrDefault(p1 =>
        {
            var x = Math.Abs(p.X - p1.X) < tol;
            var y = Math.Abs(p.Y - p1.Y) < tol;
            var z = Math.Abs(p.Z - p1.Z) < tol;
            return x && y && z;
        },/*Default*/new Node() { ID = -1 });
        if (n is Node mn)
            id = mn.ID;
        return id;
    }

    public static bool NodeEqual(Node p1, Node p, double tol = 0.01)
    {
        var x = Math.Abs(p.X - p1.X) < tol;
        var y = Math.Abs(p.Y - p1.Y) < tol;
        var z = Math.Abs(p.Z - p1.Z) < tol;
        return x && y && z;
    }

    public static bool nodeCopy(ModelDB db, Node nudo, double ofx, double ofy, double ofz,
        bool copyLoads = true, bool copyRestr = true, bool copyConstraints = true)
    {
        if (-1 == db.nodes.FindIndexByID(nudo.ID)) return false;//Maybe deleted or bad send...

        var id = nudo.ID;

        var nnudo = new Node()
        {
            ID = -2,
            label = "",
            X = nudo.X + ofx,
            Y = nudo.Y + ofy,
            Z = nudo.Z + ofz
        };

        var x = IsNode(db, nnudo);
        if (x != -1)
        {
            var indx = db.nodes.FindIndexByID(x);
            nnudo = db.nodes[indx];
        }
        else
        {
            var nid = db.nodes.NextID();
            nnudo.ID = nid;
            nnudo.label = $"{nid}";
            db.nodes.Add(nnudo);

            var ridx = db.nodalRestraints.FindIndexByID(id);
            var ra = db.nodalRestraints[ridx];
            if (copyRestr)
            {
                db.nodalRestraints.Add(
                    new nodalRestraint()
                    { ID = nnudo.ID, rx = ra.rx, ry = ra.ry, rz = ra.rz, ux = ra.ux, uy = ra.uy, uz = ra.uz }
                    );
            }
            else
            {
                db.nodalRestraints.Add(
                   new nodalRestraint()
                   { ID = nnudo.ID }
                   );
            }
        }

        if (copyLoads)
        {
            var cns = db.nodalLoads.Where(c => c.NodeID == id).ToList();
            foreach (var c in cns)
            {
                var cid = db.nodalLoads.NextID();
                var nc = new nodalLoad()
                {
                    ID = cid,
                    NodeID = nnudo.ID,
                    Fx = c.Fx,
                    Fy = c.Fy,
                    Fz = c.Fz,
                    Mx = c.Mx,
                    My = c.My,
                    Mz = c.Mz,
                    Case = c.Case
                };
                db.nodalLoads.Add(nc);
            }
        }

        if (copyConstraints)
        {
            var idlist = db.nodalConstraints.Where(rc =>
            {
                return rc.nodesIDList.Split(",").ToList().Any(i => i == $"{id}");
            });
            foreach (var rc in idlist)
            {
                if (rc.nodesIDList.Last() == ',')
                    rc.nodesIDList += $"{nnudo.ID}";
                else
                    rc.nodesIDList += $",{nnudo.ID}";
            }
        }

        return true;
    }

    public static bool nodeMove(ModelDB db, Node nudo, double ofx, double ofy, double ofz)
    {
        if (-1 == db.nodes.FindIndexByID(nudo.ID)) return false;//Maybe deleted or bad send...

        Node p1 = new();
        p1.X = nudo.X + ofx;
        p1.Y = nudo.Y + ofy;
        p1.Z = nudo.Z + ofz;
        var existe1 = IsNode(db, p1);
        if (existe1 != -1)//In any node with this ID in the destiny position
        {
            foreach (var b in db.bars)
            {
                if (b.startNode == nudo.ID) b.startNode = existe1;
                if (b.endNode == nudo.ID) b.endNode = existe1;
            }
            foreach (var l in db.nodalLoads)
                if (l.NodeID == nudo.ID) l.NodeID = existe1;
            // TODO: re-link area corners too (Area.Node1..Node4) before deleting the node
            NodeDelete(db, nudo);
        }
        else
        {
            var n = db.nodes.First(n => n.ID == nudo.ID);
            n.X += ofx;
            n.Y += ofy;
            n.Z += ofz;
        }
        return true;
    }
}
