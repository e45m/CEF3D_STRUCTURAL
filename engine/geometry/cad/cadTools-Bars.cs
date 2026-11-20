using System.Data.Common;
using System.Diagnostics;

namespace CEF;

static public  class BarCadTools
{

    //createNewNode()---
    //createNewBar()---
    //createNewArea()---


    //Bar 
    public static bool BarDelete(ModelDB db, Bar Bar) {
        var isFree = true;
        if (isFree)
        {
            var idx1 = db.bars.FindIndexByID(Bar.ID);
            var idx = -1;
            var id = Bar.ID;
            if (idx1 != -1){
            var cdb = db.barDistrLoads.Where(b => b.BarID == Bar.ID).ToList();
            var cpb = db.barPunctLoads.Where(b => b.BarID == Bar.ID).ToList();
            var ctb = db.barThermalLoads.Where(b => b.BarID == Bar.ID).ToList();
            foreach (var i in cdb)
            {
                idx = db.barDistrLoads.FindIndexByID(i.ID);
                db.barDistrLoads.RemoveAt(idx);
            }
            foreach (var i in cpb)
            {
                idx = db.barPunctLoads.FindIndexByID(i.ID);
                db.barPunctLoads.RemoveAt(idx);
            }
            foreach (var i in ctb)
            {
                idx = db.barThermalLoads.FindIndexByID(i.ID);
                db.barThermalLoads.RemoveAt(idx);
            }
                db.bars.RemoveAt(idx1);
}
        }
        return true;
    }
    public static bool BarDelete(ModelDB db, int id)
    {
        var idx = db.bars.FindIndexByID(id);
        var Bar = db.bars[idx];
        return BarDelete( db, Bar);
    }


    public static bool BarDistrLoadDelete(ModelDB db, barDistrLoad Bar)
    {
        return BarDistrLoadDelete( db, Bar.ID);
    }
    public static bool BarDistrLoadDelete(ModelDB db, int id)
    {
        var idx = db.barDistrLoads.FindIndexByID(id);
        db.barDistrLoads.RemoveAt(idx);
        return true;
    }
    public static bool BarPunLoadDelete(ModelDB db, barPunctLoad Bar)
    {
        BarPunLoadDelete( db, Bar.ID);
        return false;
    }
    public static bool BarPunLoadDelete(ModelDB db, int id)
    {
        var idx = db.barDistrLoads.FindIndexByID(id);
        db.barPunctLoads.RemoveAt(idx);
        return true;
    }

    
    public static bool barMove(ModelDB db, Bar Bar, double ofx, double ofy, double ofz)
    {

        if(-1== db.bars.FindIndexByID(Bar.ID)) return false;//Maybe deleted or bad send...

        Node p1 = new();
        Node p2 = new();
        var p01 = db.nodes.First(p => p.ID == Bar.startNode);
        var p02 = db.nodes.First(p => p.ID == Bar.endNode);
        p1.X = p01.X + ofx;
        p1.Y = p01.Y + ofy;
        p1.Z = p01.Z + ofz;
        p2.X = p02.X + ofx;
        p2.Y = p02.Y + ofy;
        p2.Z = p02.Z + ofz;
        var existe1 = NodeCadTools.IsNode(db, p1);
        var existe2 = NodeCadTools.IsNode(db, p2);
        if (existe1 != -1)
        {
            p1 = db.nodes.First(p => p.ID == existe1);
        }
        else
        {
            p1.ID = db.nodes.NextID();
            p1.label = $"{p1.ID}";
            db.nodes.Add(p1);
            db.nodalRestraints.Add(new nodalRestraint { ID = p1.ID, });
        }
        if (existe2 != -1)
        {
            p2 = db.nodes.First(p => p.ID == existe2);
        }
        else
        {
            p2.ID = db.nodes.NextID();
            p2.label = $"{p2.ID}";
            db.nodes.Add(p2);
            db.nodalRestraints.Add(new nodalRestraint { ID = p2.ID, });
        }
        var idx = db.bars.FindIndexByID(Bar.ID);
        db.bars[idx].startNode = p1.ID;
        db.bars[idx].endNode = p2.ID;
        return true;
    }
    public static bool barRotate(ModelDB db, Bar Bar, double afx, double afy, double afz, double[] rp)
    {
        Node p1 = new();
        Node p2 = new();
        var p01 = db.nodes.First(p => p.ID == Bar.startNode);
        var p02 = db.nodes.First(p => p.ID == Bar.endNode);
        var v1 = new double[,]{ {p01.X -  rp[0] }, { p01.Y - rp[1] }, { p01.Z - rp[2] },{ 1 } };
        var v2 = new double[,]{ {p02.X -  rp[0] }, { p02.Y - rp[1] }, { p02.Z - rp[2] },{ 1 } };
        var MT = CadToolsUtils.getTMatrix(afx, afy, afz);
        var v3 = MatrixMath.MT_mult(MT, v1);
        var v4 = MatrixMath.MT_mult(MT, v2);
        p1.X = v3[0, 0] + rp[0];
        p1.Y = v3[1, 0] + rp[1];
        p1.Z = v3[2, 0] + rp[2];

        p2.X = v4[0, 0] + rp[0];
        p2.Y = v4[1, 0] + rp[1];
        p2.Z = v4[2, 0] + rp[2];

        NodeCadTools.nodeMove( db, p01, p1.X - p01.X, p1.Y - p01.Y, p1.Z - p01.Z);
        NodeCadTools.nodeMove( db, p02, p2.X - p02.X, p2.Y - p02.Y, p2.Z - p02.Z);
        // nodeCopy(ref db, p01, p1.X - p01.X, p1.Y - p01.Y, p1.Z - p01.Z);
        // nodeCopy(ref db, p02, p2.X - p02.X, p2.Y - p02.Y, p2.Z - p02.Z);

        var indx1 = db.nodes.FindIndexByID(NodeCadTools.IsNode(db, p1));
        var indx2 = db.nodes.FindIndexByID(NodeCadTools.IsNode(db, p2));

        var idx = db.bars.FindIndexByID(Bar.ID);
        db.bars[idx].startNode = db.nodes[indx1].ID;
        db.bars[idx].endNode = db.nodes[indx2].ID;
        return true;
    }
    public static bool barCopy( ModelDB db, Bar Bar, double ofx, double ofy, double ofz)
    {
        var nbar = new Bar();
        Node p1 = new();
        Node p2 = new();
        var p01 = db.nodes.First(p => p.ID == Bar.startNode);
        var p02 = db.nodes.First(p => p.ID == Bar.endNode);
        p1.X = p01.X + ofx;
        p1.Y = p01.Y + ofy;
        p1.Z = p01.Z + ofz;
        p2.X = p02.X + ofx;
        p2.Y = p02.Y + ofy;
        p2.Z = p02.Z + ofz;
        var existe1 = NodeCadTools.IsNode(db, p1);
        var existe2 = NodeCadTools.IsNode(db, p2);
        if (existe1 != -1)
        {
            p1 = db.nodes.First(p => p.ID == existe1);
        } else
        {
            p1.ID = db.nodes.NextID();
            p1.label = $"{p1.ID}";
            db.nodes.Add(p1);
            db.nodalRestraints.Add(new nodalRestraint { ID = p1.ID, });
        }
        if (existe2 != -1)
        {
            p2 = db.nodes.First(p => p.ID == existe2);
        }
        else
        {
            p2.ID = db.nodes.NextID();
            p2.label = $"{p2.ID}";
            db.nodes.Add(p2);
            db.nodalRestraints.Add(new nodalRestraint { ID = p2.ID, });
        }
        nbar.ID = db.bars.NextID() ; 
        nbar.startNode = p1.ID;
        nbar.endNode = p2.ID;
        nbar.SectionID = Bar.SectionID;
        nbar.label = $"{nbar.ID}";
        nbar.SectionName = Bar.SectionName;
        nbar.releases = Bar.releases;
        nbar.offsetZ = Bar.offsetZ;
        nbar.offsetY = Bar.offsetY;
        nbar.Rotation = Bar.Rotation;
        nbar.unused = Bar.unused;
        db.bars.Add(nbar);
        return true;
    }
    public static bool barDivide(ModelDB db, Bar Bar,
        double maxLen = -1, int minSegments = -1)
    {
        var p01 = db.nodes.First(p => p.ID == Bar.startNode);
        var p02 = db.nodes.First(p => p.ID == Bar.endNode);
        var L = Math.Sqrt(Math.Pow(p02.X - p01.X, 2) +
            Math.Pow(p02.Y - p01.Y, 2) +
            Math.Pow(p02.Z - p01.Z, 2));

        if ( (minSegments == -1 && maxLen == -1 || minSegments <= 0)) return true;
        var n = minSegments;
        var lenSeg = L / n;
        var space = 1 / n;

        var vec = new double[] {
        (p02.X-p01.X)/n,
        (p02.Y-p01.Y)/n,
        (p02.Z-p01.Z)/n,
        };
        List<Node> pList = [];
        pList.Add(p01);
        for (int i = 1; i < n; i++)
        {
            var nod = new Node {
                ID = db.nodes.NextID(),
                
                X = p01.X + vec[0] * i,
                Y = p01.Y + vec[1] * i,
                Z = p01.Z + vec[2] * i,
            };
            nod.label = $"{nod.ID}";
            pList.Add(nod);
        }
        pList.Add(p02);

        divideBarInPoinList( db,Bar,pList);
       
        return true;

        }


    public static bool barDivideAB(ModelDB db, Bar Bar,
        double a_L)
    {
        var p01 = db.nodes.First(p => p.ID == Bar.startNode);
        var p02 = db.nodes.First(p => p.ID == Bar.endNode);
        var L = Math.Sqrt(Math.Pow(p02.X - p01.X, 2) +
            Math.Pow(p02.Y - p01.Y, 2) +
            Math.Pow(p02.Z - p01.Z, 2));

        if (a_L <= 0 || a_L>=1) return true;
        var n =2;
        var lenSeg = L / n;
        var space = 1 / n;

        var vec = new double[] {
        (p02.X-p01.X)/n,
        (p02.Y-p01.Y)/n,
        (p02.Z-p01.Z)/n,
        };
        List<Node> pList = [];
        pList.Add(p01);

            var nod = new Node
            {
                ID = db.nodes.NextID(),
                
                X = p01.X + vec[0] *a_L,
                Y = p01.Y + vec[1] *a_L,
                Z = p01.Z + vec[2] *a_L,
            };
            nod.label = $"{nod.ID}";
            pList.Add(nod);
        
        pList.Add(p02);

        divideBarInPoinList( db, Bar, pList);

        return true;

    }
   public static void divideBarInPoinList(ModelDB db, Bar Bar, List<Node> pList)
    {

        // List<Nodo> pList = [];
        List<Bar> bList = [];
        List<barDistrLoad> lcdb = [];
        List<barPunctLoad> lcpb = [];
        List<barThermalLoad> lctb = [];
        var cdb = db.barDistrLoads.Where(c => c.BarID == Bar.ID).ToList();
        var cpb = db.barPunctLoads.Where(c => c.BarID == Bar.ID).ToList();
        var ctb = db.barThermalLoads.Where(c => c.BarID == Bar.ID).ToList();
        var n = pList.Count-1;
        for (int i = 1; i < n; i++)
        {
            
            pList[i] = space3d.checkNewNode(pList[i],db) ;
        }

        for (int i = 0; i < n; i++)
        {
            var nbar = new Bar();
            nbar.ID = db.bars.NextID();
            nbar.label = $"{nbar.ID}";
            nbar.startNode = pList[i].ID;
            nbar.endNode = pList[i + 1].ID;
            nbar.SectionID = Bar.SectionID;            
            nbar.SectionName = Bar.SectionName;
            nbar.offsetZ = Bar.offsetZ;
            nbar.offsetY = Bar.offsetY;
            nbar.Rotation = Bar.Rotation;
            nbar.unused = Bar.unused;
            bList.Add(nbar);
            foreach (var dl in cdb)
            {
                var a = dl.l1;
                var b = dl.l2;
                var sti = 1 / n * (i);
                var stj = 1 / n * (i + 1);
                var newA = 0d;
                var newB = 1d;
                if (a > stj || b < sti) continue;
                if (a > sti)
                    newA = (a - sti) / (stj - sti);
                if (b < stj)
                    newB = (b - sti) / (stj - sti);
                var nl = new barDistrLoad()
                {
                    ID = db.barDistrLoads.Count,
                    Case = dl.Case,
                    BarID = nbar.ID,
                    l1 = newA,
                    l2 = newB,
                    Fx = dl.Fx,
                    Fy = dl.Fy,
                    Fz = dl.Fz,
                    Type = dl.Type
                };
                lcdb.Add(nl);
            }
            foreach (var dl in cpb)
            {
                var a = dl.l1;
                var sti = 1 / n * (i);
                var stj = 1 / n * (i + 1);
                var newA = 0d;
                if (a > stj) continue;
                if (a > sti)
                    newA = (a - sti) / (stj - sti);
                var nl = new barPunctLoad()
                {
                    ID = db.barPunctLoads.Count,
                    BarID = nbar.ID,
                    l1 = newA,
                    Case = dl.Case,
                    Fx = dl.Fx,
                    Fy = dl.Fy,
                    Fz = dl.Fz,
                    Type = dl.Type
                };
                lcpb.Add(nl);
            }
            foreach (var dl in ctb)
            {
                var b = dl.b;
                var sti = 1 / n * (i);
                var stj = 1 / n * (i + 1);
                var newB = 1d;
                if (b < stj)
                    newB = (b - sti) / (stj - sti);
                var nl = new barThermalLoad()
                {
                    ID = db.barThermalLoads.Count,
                    Case = dl.Case,
                    BarID = nbar.ID,
                    intThemperature = dl.intThemperature,
                    extTheperature = dl.extTheperature,
                    h = dl.h,
                    b = dl.b,
                };
                lctb.Add(nl);
            }
        }
        var rel = Bar.releases;
        int mask = (1 << 6) - 1;
        var der = rel & mask;
        var izq = rel & (mask << 6);
        bList[0].releases = der;
        bList[n - 1].releases = izq;
        int idx = db.bars.FindIndexByID(Bar.ID);
        if (idx == -1) throw new Exception($"Barra {Bar.ID}ID = -1");
        db.bars.RemoveAt(idx
            );
        foreach (var dl in cdb)
        {
            var indx = db.barDistrLoads.FindIndexByID(dl.ID);
            db.barDistrLoads.RemoveAt(indx);
        }
        foreach (var dl in cpb)
        {
            var indx = db.barPunctLoads.FindIndexByID(dl.ID);
            db.barPunctLoads.RemoveAt(indx);
        }
        foreach (var dl in ctb)
        {
            var indx = db.barThermalLoads.FindIndexByID(dl.ID);
            db.barThermalLoads.RemoveAt(indx);
        }

        for (int i = 0; i < n; i++)
            db.bars.Add(bList[i]);
        foreach (var l in lcdb)
        {
            if(db.barDistrLoads.Count>0){
            l.ID = db.barDistrLoads.NextID();
            }else {
               l.ID = 1; 
            }
            db.barDistrLoads.Add(l);
        }
        foreach (var l in lcpb)
        {
            if(db.barPunctLoads.Count>0){
            l.ID = db.barPunctLoads.NextID();            
            }else {
               l.ID = 1; 
            }
            db.barPunctLoads.Add(l);
        }
        foreach (var l in lctb)
        {
           if(db.barThermalLoads.Count>0){
            l.ID = db.barThermalLoads.NextID();
            }else {
               l.ID = 1; 
            }
            db.barThermalLoads.Add(l);
        }
    }



}