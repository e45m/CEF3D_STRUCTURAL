namespace CEF;

static class cadTools
{
    public static bool NodeDelete(ref ModelDB db, node p)
    {
        var isFree = !db.bars.Any(b => b.endNode == p.ID || b.startNode == p.ID);
        if (isFree) {
            var idx = db.nodes.FindIndexByID(p.ID);
            var id = p.ID;
 
            var ridx = db.nodalRestraints.FindIndexByID(id);
            db.nodalRestraints.RemoveAt(ridx);
            var cns = db.nodalLoads.Where(c => c.NodeID == id);
            foreach (var c in cns)
            {
                var cidx = db.nodalLoads.FindIndexByID(c.ID);
                db.nodalLoads.RemoveAt(cidx);
            }
            var idlist = db.nodalConstraints.Where(rc => {
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
    public static bool NodeDelete(ref ModelDB db, int id)
    {
        var idx = db.nodes.FindIndexByID(id);
        var p = db.nodes[idx];
        return NodeDelete(ref db, p);
    }
    public static bool BarDelete(ref ModelDB db, bar bar) {
        var isFree = true;
        if (isFree)
        {
            var idx1 = db.bars.FindIndexByID(bar.ID);
            var idx = -1;
            var id = bar.ID;
            if (idx1 != -1){
            var cdb = db.barDistrLoads.Where(b => b.BarID == bar.ID).ToList();
            var cpb = db.barPunctLoads.Where(b => b.BarID == bar.ID).ToList();
            var ctb = db.barThermalLoads.Where(b => b.barID == bar.ID).ToList();
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
    public static bool BarDelete(ref ModelDB db, int id)
    {
        var idx = db.bars.FindIndexByID(id);
        var bar = db.bars[idx];
        return BarDelete(ref db, bar);
    }
    public static bool NodeLoadDelete(ref ModelDB db, nodalLoad p)
    {
        var cidx = db.nodalLoads.FindIndexByID(p.ID);
        db.nodalLoads.RemoveAt(cidx);
        return true;
    }
    public static bool NodeLoadDelete(ref ModelDB db, int id)
    {
        var idx = db.nodalLoads.FindIndexByID(id);
        var l = db.nodalLoads[idx];
        return NodeLoadDelete(ref db, l);
    }
    public static bool BarDistrLoadDelete(ref ModelDB db, barDistrLoad bar)
    {
        return BarDistrLoadDelete(ref db, bar.ID);
    }
    public static bool BarDistrLoadDelete(ref ModelDB db, int id)
    {
        var idx = db.barDistrLoads.FindIndexByID(id);
        db.barDistrLoads.RemoveAt(idx);
        return true;
    }
    public static bool BarPunLoadDelete(ref ModelDB db, barPunctLoad bar)
    {
        BarPunLoadDelete(ref db, bar.ID);
        return false;
    }
    public static bool BarPunLoadDelete(ref ModelDB db, int id)
    {
        var idx = db.barDistrLoads.FindIndexByID(id);
        db.barPunctLoads.RemoveAt(idx);
        return true;
    }
    public static int  IsNode(ModelDB db, node p, double tol = 0.01)
    {
        var id = -1;
        var n = db.nodes.FirstOrDefault(p1 =>
        {
            var x = Math.Abs(p.X - p1.X) < tol;
            var y = Math.Abs(p.Y - p1.Y) < tol;
            var z = Math.Abs(p.Z - p1.Z) < tol;
            return x && y && z;
        },/*Default*/new node () { ID=-1});
        if (n is node mn)
            id = mn.ID;
        return id;
    }
    public static bool NodeEqual(node p1, node p, double tol = 0.01)
    {
        var x = Math.Abs(p.X - p1.X) < tol;
        var y = Math.Abs(p.Y - p1.Y) < tol;
        var z = Math.Abs(p.Z - p1.Z) < tol;
        return x && y && z;
    }
    public static bool nodeCopy(ref ModelDB db, node nudo, double ofx, double ofy, double ofz,
        bool copyLoads=true,bool copyRestr = true, bool copyConstraints = true )
    {

        var idx = db.nodes.FindIndexByID(nudo.ID);
        var id = nudo.ID;

        var nnudo = new node()
        {
            ID = -2,
            label = "",
            X = nudo.X + ofx,
            Y = nudo.Y + ofy,
            Z = nudo.Z + ofz

        };

        var x = IsNode( db, nnudo);
        if (x != -1)
        {
            var indx = db.nodes.FindIndexByID(x);

            nnudo = db.nodes[indx];
        }
        else
        {
            var nid = db.nodes.Max(x => x.ID) + 1;
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
            }else
            {
                db.nodalRestraints.Add(
                   new nodalRestraint()
                   { ID = nnudo.ID }
                   );
            }

        }

        if(copyLoads){
            var cns = db.nodalLoads.Where(c => c.NodeID == id);
            foreach (var c in cns)
            {
                var cid = db.nodalLoads.Max(c => c.ID) + 1;
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
           
        if(copyConstraints){
            var idlist = db.nodalConstraints.Where(rc => {
                return rc.nodesIDList.Split(",").ToList().Any(i => i == $"{id}");
            });
            foreach (var rc in idlist)
            {
                if (rc.nodesIDList.Last() == ',')
                {
                    rc.nodesIDList += $"{nnudo.ID}";

                }
                else
                {
                    rc.nodesIDList += $",{nnudo.ID}";

                }


            }
}
        
        return true;
       
    }
    public static bool nodeMove(ref ModelDB db, node nudo, double ofx, double ofy, double ofz)
    {
        node p1 = new();
        p1.X = nudo.X + ofx;
        p1.Y = nudo.Y + ofy;
        p1.Z = nudo.Z + ofz;
        var existe1 = IsNode(db, p1);
        if (existe1 != -1)
        {
            
            foreach (var b in db.bars)
            {
                if (b.startNode == nudo.ID) b.startNode = existe1; 
                if (b.endNode == nudo.ID) b.endNode = existe1;
            }
            foreach (var l in db.nodalLoads)
                if (l.NodeID == nudo.ID) l.NodeID = existe1;
            NodeDelete(ref db, nudo);
                


        }
        else
        {
            var n = db.nodes.First(n=>n.ID==nudo.ID);
            n.X += ofx;
            n.Y += ofy;
            n.Z += ofz;
        }
        return true;
    }
    public static bool barMove(ref ModelDB db, bar bar, double ofx, double ofy, double ofz)
    {
        node p1 = new();
        node p2 = new();
        var p01 = db.nodes.First(p => p.ID == bar.startNode);
        var p02 = db.nodes.First(p => p.ID == bar.endNode);
        p1.X = p01.X + ofx;
        p1.Y = p01.Y + ofy;
        p1.Z = p01.Z + ofz;
        p2.X = p02.X + ofx;
        p2.Y = p02.Y + ofy;
        p2.Z = p02.Z + ofz;
        var existe1 = IsNode(db, p1);
        var existe2 = IsNode(db, p2);
        if (existe1 != -1)
        {
            p1 = db.nodes.First(p => p.ID == existe1);
        }
        else
        {
            p1.ID = db.nodes.Max(p => p.ID) + 1;
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
            p2.ID = db.nodes.Max(p => p.ID) + 1;
            p2.label = $"{p2.ID}";
            db.nodes.Add(p2);
            db.nodalRestraints.Add(new nodalRestraint { ID = p2.ID, });
        }
        var idx = db.bars.FindIndexByID(bar.ID);
        db.bars[idx].startNode = p1.ID;
        db.bars[idx].endNode = p2.ID;
        return true;
    }
    public static bool barRotate(ref ModelDB db, bar bar, double afx, double afy, double afz, double[] rp)
    {
        node p1 = new();
        node p2 = new();
        var p01 = db.nodes.First(p => p.ID == bar.startNode);
        var p02 = db.nodes.First(p => p.ID == bar.endNode);
        var v1 = new double[,]{ {p01.X -  rp[0] }, { p01.Y - rp[1] }, { p01.Z - rp[2] },{ 1 } };
        var v2 = new double[,]{ {p02.X -  rp[0] }, { p02.Y - rp[1] }, { p02.Z - rp[2] },{ 1 } };
        var MT = getTMatrix(afx, afy, afz);
        var v3 = MatrixMath.MT_mult(MT, v1);
        var v4 = MatrixMath.MT_mult(MT, v2);
        p1.X = v3[0, 0] + rp[0];
        p1.Y = v3[1, 0] + rp[1];
        p1.Z = v3[2, 0] + rp[2];

        p2.X = v4[0, 0] + rp[0];
        p2.Y = v4[1, 0] + rp[1];
        p2.Z = v4[2, 0] + rp[2];



        nodeCopy(ref db, p01, p1.X - p01.X, p1.Y - p01.Y, p1.Z - p01.Z);
        nodeCopy(ref db, p02, p2.X - p02.X, p2.Y - p02.Y, p2.Z - p02.Z);

        var indx1 = db.nodes.FindIndexByID(IsNode(db, p1));
        var indx2 = db.nodes.FindIndexByID(IsNode(db, p2));

        var idx = db.bars.FindIndexByID(bar.ID);
        db.bars[idx].startNode = db.nodes[indx1].ID;
        db.bars[idx].endNode = db.nodes[indx2].ID;
        return true;
    }
    public static bool barCopy(ref ModelDB db, bar bar, double ofx, double ofy, double ofz)
    {
        var nbar = new bar();
        node p1 = new();
        node p2 = new();
        var p01 = db.nodes.First(p => p.ID == bar.startNode);
        var p02 = db.nodes.First(p => p.ID == bar.endNode);
        p1.X = p01.X + ofx;
        p1.Y = p01.Y + ofy;
        p1.Z = p01.Z + ofz;
        p2.X = p02.X + ofx;
        p2.Y = p02.Y + ofy;
        p2.Z = p02.Z + ofz;
        var existe1 = IsNode(db, p1);
        var existe2 = IsNode(db, p2);
        if (existe1 != -1)
        {
            p1 = db.nodes.First(p => p.ID == existe1);
        } else
        {
            p1.ID = db.nodes.Max(p => p.ID) + 1;
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
            p2.ID = db.nodes.Max(p => p.ID) + 1;
            p2.label = $"{p2.ID}";
            db.nodes.Add(p2);
            db.nodalRestraints.Add(new nodalRestraint { ID = p2.ID, });
        }
        nbar.ID = db.bars.Max(b => b.ID) + 1; ; 
        nbar.startNode = p1.ID;
        nbar.endNode = p2.ID;
        nbar.SectionID = bar.SectionID;
        nbar.label = $"{nbar.ID}";
        nbar.SectionName = bar.SectionName;
        nbar.MaterialID = bar.MaterialID;
        nbar.MaterialName = bar.MaterialName;
        nbar.releases = bar.releases;
        nbar.offsetZ = bar.offsetZ;
        nbar.offsetY = bar.offsetY;
        nbar.Rotation = bar.Rotation;
        nbar.unused = bar.unused;
        db.bars.Add(nbar);
        return true;
    }
    public static bool barDivide(ref ModelDB db, bar bar,
        double maxLen = -1, int minSegments = -1)
    {
        var p01 = db.nodes.First(p => p.ID == bar.startNode);
        var p02 = db.nodes.First(p => p.ID == bar.endNode);
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
        List<node> pList = [];
        pList.Add(p01);
        for (int i = 1; i < n; i++)
        {
            var nod = new node {
                ID = db.nodes.Max(n => n.ID) + 1 + i,
                label = $"{p01.ID}Seg-{i}",
                X = p01.X + vec[0] * i,
                Y = p01.Y + vec[1] * i,
                Z = p01.Z + vec[2] * i,
            };
            pList.Add(nod);
        }
        pList.Add(p02);

        divideBarInPoinList(ref db,bar,pList);
       
        return true;

        }


    public static bool barDivideAB(ref ModelDB db, bar bar,
        double a_L)
    {
        var p01 = db.nodes.First(p => p.ID == bar.startNode);
        var p02 = db.nodes.First(p => p.ID == bar.endNode);
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
        List<node> pList = [];
        pList.Add(p01);

            var nod = new node
            {
                ID = db.nodes.Max(n => n.ID) + 1 ,
                label = $"{p01.ID}Seg-{1}",
                X = p01.X + vec[0] *a_L,
                Y = p01.Y + vec[1] *a_L,
                Z = p01.Z + vec[2] *a_L,
            };
            pList.Add(nod);
        
        pList.Add(p02);

        divideBarInPoinList(ref db, bar, pList);

        return true;

    }
   public static void divideBarInPoinList(ref ModelDB db, bar bar, List<node> pList)
    {

        // List<Nodo> pList = [];
        List<bar> bList = [];
        List<barDistrLoad> lcdb = [];
        List<barPunctLoad> lcpb = [];
        List<barThermalLoad> lctb = [];
        var cdb = db.barDistrLoads.Where(c => c.BarID == bar.ID).ToList();
        var cpb = db.barPunctLoads.Where(c => c.BarID == bar.ID).ToList();
        var ctb = db.barThermalLoads.Where(c => c.barID == bar.ID).ToList();
        var n = pList.Count-1;
        for (int i = 1; i < n; i++)
        {
            
            pList[i] = space3d.checkNewNode(pList[i]) ;
        }

        for (int i = 0; i < n; i++)
        {
            var nbar = new bar();
            nbar.ID = db.bars.Max(n => n.ID) + 1 + i;
            nbar.label = $"{nbar.ID}Seg-{i}";
            nbar.startNode = pList[i].ID;
            nbar.endNode = pList[i + 1].ID;
            nbar.SectionID = bar.SectionID;
            nbar.label = $"{nbar.ID}";
            nbar.SectionName = bar.SectionName;
            nbar.MaterialID = bar.MaterialID;
            nbar.MaterialName = bar.MaterialName;
            nbar.offsetZ = bar.offsetZ;
            nbar.offsetY = bar.offsetY;
            nbar.Rotation = bar.Rotation;
            nbar.unused = bar.unused;
            bList.Add(nbar);
            foreach (var dl in cdb)
            {
                var a = dl.a;
                var b = dl.b;
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
                    a = newA,
                    b = newB,
                    Fx = dl.Fx,
                    Fy = dl.Fy,
                    Fz = dl.Fz,
                    Type = dl.Type
                };
                lcdb.Add(nl);
            }
            foreach (var dl in cpb)
            {
                var a = dl.a;
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
                    a = newA,
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
                    barID = nbar.ID,
                    intThemperature = dl.intThemperature,
                    extTheperature = dl.extTheperature,
                    h = dl.h,
                    b = dl.b,
                };
                lctb.Add(nl);
            }
        }
        var rel = bar.releases;
        int mask = (1 << 6) - 1;
        var der = rel & mask;
        var izq = rel & (mask << 6);
        bList[0].releases = der;
        bList[n - 1].releases = izq;
        int idx = db.bars.FindIndexByID(bar.ID);
        if (idx == -1) throw new Exception($"Barra {bar.ID}ID = -1");
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
            l.ID = db.barDistrLoads.Max(c => c.ID) + 1;
            db.barDistrLoads.Add(l);
        }
        foreach (var l in lcpb)
        {
            l.ID = db.barPunctLoads.Max(c => c.ID) + 1;
            db.barPunctLoads.Add(l);
        }
        foreach (var l in lctb)
        {
            l.ID = db.barThermalLoads.Max(c => c.ID) + 1;
            db.barThermalLoads.Add(l);
        }
    }

    private static double[,] getTMatrix(double angX, double angY,double angZ) { 
    double Ca = Math.Cos(angX); 
    double Sa = Math.Sin(angX); 
    double Cb = Math.Cos(angY);
    double Sb = Math.Sin(angY);
    double Cc = Math.Cos(angZ);
    double Sc = Math.Sin(angZ);
    double[,] M_RotaX = new double[,] { { 1d, 0d, 0d, 0d },
                                            {0d, Ca, -Sa, 0d },
                                            {0d, Sa, Ca ,0d},
                                            {0d, 0d, 0d ,1d} };
    double[,] M_RotaY = new double[,] { {Cb, 0d, Sb, 0d },
                                            {0d, 1d, 0d, 0d },
                                            {-Sb, 0d, Cb ,0d},
                                            {0d, 0d, 0d ,1d} };
    double[,] M_RotaZ = new double[,] { { Cc, -Sc, 0d, 0d },
                                            {Sc, Cc, 0d, 0d },
                                            {0d, 0d, 1d ,0d},
                                            {0d, 0d, 0d ,1d} };
        var M = MatrixMath.M_mult(M_RotaX,M_RotaY);          
            M = MatrixMath.M_mult(M_RotaZ, M);
        return M;
}


    public static double[] calSecProfile(section secc, int segms = 1, int offsety = 0, int offsetz = 0)
    {
        double[] pv = Array.Empty<double>();
        var tipo = (sectType)secc.Type;
        double dy = 0;
        double dz = 0;
        switch (offsety)
        {
            case 0:
                dy = 0.0;
                break;
            case 1:
                dy = -secc.b / 2;
                break;
            case 2:
                dy = secc.b / 2;
                break;
        }
        switch (offsetz)
        {
            case 0:
                dz = 0.0;
                break;
            case 1:
                dz = -secc.h / 2;
                break;
            case 2:
                dz = secc.h / 2;
                break;
        }
        void moveOffsets(double[] v)
        {
            if (dy == 0 && dz == 0) return;
            var l = v.Length;
            for (var i = 0; i < l; i += 3)
            {
                if (v[i] == double.MaxValue) continue;
                v[i + 1] += dy;
                v[i + 2] += dz;
            }
        }
        switch (tipo)
        {
            case sectType.Rect:
            case sectType.HolRec:
                {
                    double b = secc.b;
                    double h = secc.h;
                    pv = new double[]
                    {
                    0, -b/2, h/2,
                    0, b/2, h/2,
                    0, b/2, -h/2,
                    0, -b/2, -h/2,
                    0, -b/2, h/2,
                    double.MaxValue,double.MaxValue,double.MaxValue
                    };
                    moveOffsets(pv);
                    pv = InterpolarPerfil(pv, 3).ToArray();
                    break;
                }
            case sectType.Circ:
            case sectType.HolCirc:
                {
                    double r = secc.b / 2;
                    int n = 16;
                    pv = new double[n * 3 + 3];
                    for (int i = 0; i < n; i++)
                    {
                        double angle = 2 * Math.PI * i / n;
                        pv[i * 3 + 0] = 0;
                        pv[i * 3 + 1] = r * Math.Cos(angle);
                        pv[i * 3 + 2] = r * Math.Sin(angle);
                    }
                    pv[n * 3 + 0] = 0;
                    pv[n * 3 + 1] = pv[0 + 1];
                    pv[n * 3 + 2] = pv[0 + 2];
                    moveOffsets(pv);
                    if (segms > 1)
                        pv = InterpolarPerfil(pv, segms).ToArray();
                    break;
                }
            case sectType.IShape:
            case sectType.WShape:
                {
                    double b = secc.b, h = secc.h, tw = secc.tw, tf = secc.tf;
                    pv = new double[]
                    {
                    0.0,-b/2,h/2,
                    0.0, b/2,h/2,
                    0.0, b/2,h/2-tf,
                    0.0, tf/2,h/2-tf,
                    0.0, tw/2,h/2-tf,
                    0.0, tw/2,-h/2+tf,
                    0.0, b/2,-h/2+tf,
                    0.0, b/2,-h/2,
                    0.0, -b/2,-h/2,
                    0.0, -b/2,-h/2+tf,
                    0.0, -tw/2,-h/2+tf,
                    0.0, -tw/2,h/2-tf,
                    0.0, -b/2,h/2-tf,
                    0.0,-b/2,h/2,
                    double.MaxValue,double.MaxValue,double.MaxValue
                    };
                    moveOffsets(pv);
                    pv = InterpolarPerfil(pv, 3).ToArray();
                    break;
                }
            case sectType.Angle:
            case sectType.CProfile:
                {
                    double b = secc.b, h = secc.h, tw = secc.tw, tf = secc.tf;
                    pv = new double[]
                    {
                    0.0, -b/2, h/2,
                    0.0, b/2, h/2,
                    0.0, b/2, -h/2+tf,
                    0.0, tw/2, -h/2+tf,
                    0.0, tw/2, h/2-tf,
                    0.0, -b/2, h/2-tf,
                    0.0, -b/2, h/2,
                    double.MaxValue,double.MaxValue,double.MaxValue
                    };
                    moveOffsets(pv);
                    pv = InterpolarPerfil(pv, 3).ToArray();
                    break;
                }
        }
        return pv;
    }
   private static List<double> InterpolarPerfil(double[] src, int n)
    {
        var result = new List<double>();
        var grupo = new List<(double x, double y, double z)>();
        void cerrarGrupo()
        {
            if (grupo.Count < 2)
                return;
            for (int k = 0; k < grupo.Count - 1; k++)
            {
                var p0 = grupo[k];
                var p1 = grupo[k + 1];
                for (int i = 0; i < n; i++)
                {
                    double t = (double)i / (n - 1);
                    double x = p0.x + (p1.x - p0.x) * t;
                    double y = p0.y + (p1.y - p0.y) * t;
                    double z = p0.z + (p1.z - p0.z) * t;
                    result.Add(x);
                    result.Add(y);
                    result.Add(z);
                }
            }
            result.Add(double.MaxValue);
            result.Add(double.MaxValue);
            result.Add(double.MaxValue);
            grupo.Clear();
        }
        for (int i = 0; i < src.Length; i += 3)
        {
            if (src[i] == double.MaxValue)
            {
                cerrarGrupo();
                continue;
            }
            grupo.Add((src[i], src[i + 1], src[i + 2]));
        }
        cerrarGrupo();
        return result;
    }


}