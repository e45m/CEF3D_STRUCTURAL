namespace CEF;

internal static class PostPorcess{

 // Stations of superposition combos = sum of factor * stations of each component case.
    // Every component is re-evaluated at the union of all component abscissas (exact, no interpolation),
    // then the values are added. Requires SetSuperpositionCombosResults to have created the combo cases.
    public static void calcComboStations(ModelDB model, DtaTable<barInternForce> outList, barInternForce[] src,
        Dictionary<long, (int s, int n)> grp, Dictionary<long, (int s, int n)> stIdx,
        Dictionary<long, List<barDistrLoad>> dlMap, Dictionary<long, List<barPunctLoad>> plMap,
        int nStations, double signI, double signJ, bool closeWithEndJ, bool distanceIsLength)
    {
        int ncs = model.loadCases.Count, nb = model.bars.Count, ncomb = model.loadCombs.Count;
        int plen = comboPrefix.Length;
 
        var cc = new List<int>(); var cf = new List<double>();
        var xs = new List<double>();
        var part = new List<barInternForce>();
        var acc = new List<barInternForce>();
 
        for (int ci = 0; ci < ncs; ci++)
        {
            var lc = model.loadCases[ci];
            if (lc.Type != (int)LoadCaseType.Combination) continue;
            if (lc.Name == null || lc.Name.Length <= plen || string.CompareOrdinal(lc.Name, 0, comboPrefix, 0, plen) != 0) continue;
            string name = lc.Name.Substring(plen);
 
            cc.Clear(); cf.Clear();
            for (int k = 0; k < ncomb; k++)
            {
                var cd = model.loadCombs[k];
                if (cd.Type == 0 && cd.Name == name) { cc.Add(cd.Case); cf.Add(cd.Factor); }
            }
            if (cc.Count == 0) continue;
 
            for (int bi = 0; bi < nb; bi++)
            {
                int barId = model.bars[bi].ID;
 
                // Union of component abscissas
                xs.Clear();
                for (int j = 0; j < cc.Count; j++)
                {
                    if (!stIdx.TryGetValue(pairKey(barId, cc[j]), out var si)) continue;
                    for (int r = 0; r < si.n; r++) xs.Add(outList[si.s + r].Distance);
                }
                if (xs.Count == 0) continue;
                xs.Sort();
                double eps = 1e-9 * Math.Max(xs[xs.Count - 1], 1e-12);
                int w = 0;
                for (int k = 0; k < xs.Count; k++)
                    if (w == 0 || xs[k] - xs[w - 1] > eps) xs[w++] = xs[k];
                xs.RemoveRange(w, xs.Count - w);
                var xa = xs.ToArray();
 
                acc.Clear();
                for (int j = 0; j < cc.Count; j++)
                {
                    long key = pairKey(barId, cc[j]);
                    if (!grp.TryGetValue(key, out var g)) continue;
                    dlMap.TryGetValue(key, out var dl);
                    plMap.TryGetValue(key, out var pl);
 
                    part.Clear();
                    calcOneBarStations(model, part, src, g.s, g.n, dl, pl, nStations, signI, signJ, closeWithEndJ, distanceIsLength, xa);
                    if (part.Count != xa.Length) continue;
 
                    if (acc.Count == 0)
                        for (int k = 0; k < xa.Length; k++)
                            acc.Add(new barInternForce { BarID = barId, Case = lc.ID, Subdivision = k + 1, Distance = xa[k] });
 
                    double f = cf[j];
                    for (int k = 0; k < xa.Length; k++)
                    {
                        var o = acc[k]; var p = part[k];
                        o.Fxi += f * p.Fxi; o.Fyi += f * p.Fyi; o.Fzi += f * p.Fzi;
                        o.Mxi += f * p.Mxi; o.Myi += f * p.Myi; o.Mzi += f * p.Mzi;
                        o.Fxj += f * p.Fxj; o.Fyj += f * p.Fyj; o.Fzj += f * p.Fzj;
                        o.Mxj += f * p.Mxj; o.Myj += f * p.Myj; o.Mzj += f * p.Mzj;
                    }
                }
                for (int k = 0; k < acc.Count; k++) outList.Add(acc[k]);
            }
        }
    }


  private const string comboPrefix = App.baseLabCombCases;
 
    // Collision-free key for (id, case) pairs. Used only as dictionary key, never as IHasID.
    private static long pairKey(int id, int cas) => ((long)id << 32) | (uint)cas;
 
    public static void calculeBarInternalForcesStations(ModelDB model, AppDB appData, int nStations = 10, double signI = -1d, double signJ = 1d,
        bool closeWithEndJ = true, bool distanceIsLength = true)
    {
        if (appData.biFSeted) return;
 
        if (appData.barStatiosForces == null)
            appData.barStatiosForces = new DtaTable<barInternForce>();
        var outList = appData.barStatiosForces;
        outList.Clear();
 
        // Superposition combos are not computed from loads: they are built from their components
        var comboIds = new HashSet<int>();
        int ncs = model.loadCases.Count;
        for (int i = 0; i < ncs; i++)
            if (model.loadCases[i].Type == (int)LoadCaseType.Combination) comboIds.Add(model.loadCases[i].ID);
 
        int total = model.barInternForces.Count;
        var tmpRows = new List<barInternForce>(total);
        for (int i = 0; i < total; i++)
        {
            var r = model.barInternForces[i];
            if (!comboIds.Contains(r.Case)) tmpRows.Add(r);
        }
        var src = tmpRows.ToArray();
        int cnt = src.Length;
        Array.Sort(src, (p, q) =>
        {
            int c = p.Case.CompareTo(q.Case);
            if (c != 0) return c;
            c = p.BarID.CompareTo(q.BarID);
            return c != 0 ? c : p.Subdivision.CompareTo(q.Subdivision);
        });
 
        // Loads grouped by (bar, case)
        var dlMap = new Dictionary<long, List<barDistrLoad>>();
        int nd = model.barDistrLoads.Count;
        for (int i = 0; i < nd; i++)
        {
            var l = model.barDistrLoads[i];
            long key = pairKey(l.BarID, l.Case);
            if (!dlMap.TryGetValue(key, out var lst)) dlMap[key] = lst = new List<barDistrLoad>();
            lst.Add(l);
        }
        var plMap = new Dictionary<long, List<barPunctLoad>>();
        int np = model.barPunctLoads.Count;
        for (int i = 0; i < np; i++)
        {
            var l = model.barPunctLoads[i];
            long key = pairKey(l.BarID, l.Case);
            if (!plMap.TryGetValue(key, out var lst)) plMap[key] = lst = new List<barPunctLoad>();
            lst.Add(l);
        }
 
        // (bar, case) -> row range in src / in outList
        var grp = new Dictionary<long, (int s, int n)>();
        var stIdx = new Dictionary<long, (int s, int n)>();
        var res = new List<barInternForce>();
 
        int s = 0;
        while (s < cnt)
        {
            int e = s + 1;
            while (e < cnt && src[e].Case == src[s].Case && src[e].BarID == src[s].BarID) e++;
            long key = pairKey(src[s].BarID, src[s].Case);
            dlMap.TryGetValue(key, out var dl);
            plMap.TryGetValue(key, out var pl);
 
            res.Clear();
            calcOneBarStations(model, res, src, s, e - s, dl, pl, nStations, signI, signJ, closeWithEndJ, distanceIsLength, null);
            stIdx[key] = (outList.Count, res.Count);
            grp[key] = (s, e - s);
            for (int i = 0; i < res.Count; i++) outList.Add(res[i]);
            s = e;
        }
 
        if (comboIds.Count > 0)
            calcComboStations(model, outList, src, grp, stIdx, dlMap, plMap, nStations, signI, signJ, closeWithEndJ, distanceIsLength);
 
        appData.biFSeted = true;
    }
 
    private static void calcOneBarStations(ModelDB model, List<barInternForce> res, barInternForce[] rows, int s, int nr,
        List<barDistrLoad> dl, List<barPunctLoad> pl, int nStations,
        double signI, double signJ, bool closeWithEndJ, bool distanceIsLength, double[] fixedXs)
    {
        int barId = rows[s].BarID, cas = rows[s].Case;
        int bi = model.bars.FindIndexByID(barId);
        int ni = model.nodes.FindIndexByID(model.bars[bi].startNode);
        int nj = model.nodes.FindIndexByID(model.bars[bi].endNode);
        double ax = model.nodes[ni].X, ay = model.nodes[ni].Y, az = model.nodes[ni].Z;
        double bx = model.nodes[nj].X, by = model.nodes[nj].Y, bz = model.nodes[nj].Z;
        double L = Math.Sqrt((bx - ax) * (bx - ax) + (by - ay) * (by - ay) + (bz - az) * (bz - az));
        if (L <= 0d) return;
        double eps = 1e-9 * L;
        if (nStations < 1) nStations = 1;
 
        double[] globRef = { ax, ay, az, bx, by, bz };
 
        // Local frame: fx = (j-i)/L, fy from space3d (orthogonalized), fz = fx x fy
        double[] fx = { (bx - ax) / L, (by - ay) / L, (bz - az) / L };
        double[] fy = new double[3];
        {
            var lp = new double[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            var gp = new double[12];
            space3d.loc2glob(globRef, ref gp, lp);
            fy[0] = gp[3] - gp[0]; fy[1] = gp[4] - gp[1]; fy[2] = gp[5] - gp[2];
        }
        double dd = fy[0] * fx[0] + fy[1] * fx[1] + fy[2] * fx[2];
        fy[0] -= dd * fx[0]; fy[1] -= dd * fx[1]; fy[2] -= dd * fx[2];
        double ny = Math.Sqrt(fy[0] * fy[0] + fy[1] * fy[1] + fy[2] * fy[2]);
        if (ny < 1e-9)
        {
            double[] rf = Math.Abs(fx[2]) < 0.99 ? new[] { 0d, 0d, 1d } : new[] { 0d, 1d, 0d };
            dd = rf[0] * fx[0] + rf[1] * fx[1] + rf[2] * fx[2];
            fy[0] = rf[0] - dd * fx[0]; fy[1] = rf[1] - dd * fx[1]; fy[2] = rf[2] - dd * fx[2];
            ny = Math.Sqrt(fy[0] * fy[0] + fy[1] * fy[1] + fy[2] * fy[2]);
        }
        fy[0] /= ny; fy[1] /= ny; fy[2] /= ny;
        double[] fz = { fx[1] * fy[2] - fx[2] * fy[1], fx[2] * fy[0] - fx[0] * fy[2], fx[0] * fy[1] - fx[1] * fy[0] };
 
        // Global vector -> local components
        double[] GlobVecToLoc(double vx, double vy, double vz)
        {
            return new[]
            {
                vx * fx[0] + vy * fx[1] + vz * fx[2],
                vx * fy[0] + vy * fy[1] + vz * fy[2],
                vx * fz[0] + vy * fz[1] + vz * fz[2]
            };
        }
 
        // Loads in local axes: [F(3), M(3)] (Type 1 = local, 0 = global)
        int ndl = dl == null ? 0 : dl.Count, npl = pl == null ? 0 : pl.Count;
        var da = new double[ndl]; var db = new double[ndl];
        var df = new double[ndl][]; var dm = new double[ndl][];
        for (int i = 0; i < ndl; i++)
        {
            var l = dl[i];
            da[i] = l.a * L; db[i] = l.b * L;
            if (l.Type == 1) { df[i] = new[] { l.Fx, l.Fy, l.Fz }; dm[i] = new[] { l.Mx, l.My, l.Mz }; }
            else { df[i] = GlobVecToLoc(l.Fx, l.Fy, l.Fz); dm[i] = GlobVecToLoc(l.Mx, l.My, l.Mz); }
        }
        var px = new double[npl];
        var pf = new double[npl][]; var pm = new double[npl][];
        for (int i = 0; i < npl; i++)
        {
            var l = pl[i];
            px[i] = l.a * L;
            if (l.Type == 1) { pf[i] = new[] { l.Fx, l.Fy, l.Fz }; pm[i] = new[] { l.Mx, l.My, l.Mz }; }
            else { pf[i] = GlobVecToLoc(l.Fx, l.Fy, l.Fz); pm[i] = GlobVecToLoc(l.Mx, l.My, l.Mz); }
        }
 
        // Spans (one barInternForces record = one span)
        var x0s = new double[nr]; var x1s = new double[nr];
        if (nr == 1) { x0s[0] = 0d; x1s[0] = L; }
        else if (distanceIsLength)
        {
            double sum = 0d;
            for (int r = 0; r < nr; r++) sum += rows[s + r].Distance;
            double k = sum > 0d ? L / sum : 1d, acc = 0d;
            for (int r = 0; r < nr; r++) { x0s[r] = acc; acc += rows[s + r].Distance * k; x1s[r] = acc; }
        }
        else
        {
            for (int r = 0; r < nr; r++)
            {
                x0s[r] = rows[s + r].Distance;
                x1s[r] = r + 1 < nr ? rows[s + r + 1].Distance : L;
            }
        }
 
        double Ov(double x, double xa, double xb) => Math.Max(0d, Math.Min(x, xb) - xa);
        double I1(double x, double xa, double xb)
        {
            if (x <= xa) return 0d;
            if (x <= xb) return 0.5 * (x - xa) * (x - xa);
            double w = xb - xa;
            return 0.5 * w * w + w * (x - xb);
        }
 
        // o = [N, Vy, Vz, Mx, My, Mz] at x of span r; right=true includes point loads at x
        void Act(int r, double x, bool right, double[] o)
        {
            var f = rows[s + r];
            double x0 = x0s[r], t = x - x0;
            o[0] = signI * f.Fxi; o[1] = signI * f.Fyi; o[2] = signI * f.Fzi;
            o[3] = signI * f.Mxi;
            o[4] = signI * f.Myi + signI * f.Fzi * t;
            o[5] = signI * f.Mzi - signI * f.Fyi * t;
            for (int i = 0; i < ndl; i++)
            {
                double ov = Ov(x, da[i], db[i]) - Ov(x0, da[i], db[i]);
                double i1 = I1(x, da[i], db[i]) - I1(x0, da[i], db[i]) - Ov(x0, da[i], db[i]) * t;
                o[0] -= df[i][0] * ov;
                o[1] -= df[i][1] * ov;
                o[2] -= df[i][2] * ov;
                o[3] -= dm[i][0] * ov;
                o[5] += df[i][1] * i1 - dm[i][2] * ov;
                o[4] += -df[i][2] * i1 - dm[i][1] * ov;
            }
            for (int i = 0; i < npl; i++)
            {
                if (px[i] <= x0 + eps) continue;
                if (right ? px[i] > x + eps : px[i] >= x - eps) continue;
                double tt = Math.Max(0d, x - px[i]);
                o[0] -= pf[i][0];
                o[1] -= pf[i][1];
                o[2] -= pf[i][2];
                o[3] -= pm[i][0];
                o[5] += pf[i][1] * tt - pm[i][2];
                o[4] += -pf[i][2] * tt - pm[i][1];
            }
        }
 
        // Linear correction of each span so it closes at end j
        var delta = new double[nr * 6];
        var tmp = new double[6];
        if (closeWithEndJ)
        {
            for (int r = 0; r < nr; r++)
            {
                var f = rows[s + r];
                Act(r, x1s[r], false, tmp);
                delta[r * 6 + 0] = signJ * f.Fxj - tmp[0];
                delta[r * 6 + 1] = signJ * f.Fyj - tmp[1];
                delta[r * 6 + 2] = signJ * f.Fzj - tmp[2];
                delta[r * 6 + 3] = signJ * f.Mxj - tmp[3];
                delta[r * 6 + 4] = signJ * f.Myj - tmp[4];
                delta[r * 6 + 5] = signJ * f.Mzj - tmp[5];
            }
        }
 
        // Station abscissas: fixed list (combos) or uniform + loads + span limits
        var xs = new List<double>(nStations + 1 + 2 * (ndl + npl + nr));
        if (fixedXs != null) xs.AddRange(fixedXs);
        else
        {
            for (int k = 0; k <= nStations; k++) xs.Add(L * k / nStations);
            for (int i = 0; i < npl; i++) xs.Add(px[i]);
            for (int i = 0; i < ndl; i++) { xs.Add(da[i]); xs.Add(db[i]); }
            for (int r = 0; r < nr; r++) { xs.Add(x0s[r]); xs.Add(x1s[r]); }
            xs.RemoveAll(v => v < 0d || v > L);
            xs.Sort();
            int w = 0;
            for (int k = 0; k < xs.Count; k++)
                if (w == 0 || xs[k] - xs[w - 1] > eps) xs[w++] = xs[k];
            xs.RemoveRange(w, xs.Count - w);
        }
        int m = xs.Count;
 
        var vl = new double[6];
        var vr = new double[6];
        
        for (int k = 0; k < m; k++)
        {
            double x = xs[k];
            int rl = nr - 1, rr = nr - 1;
            for (int r = 0; r < nr; r++) if (x <= x1s[r] + eps) { rl = r; break; }
            for (int r = 0; r < nr; r++) if (x < x1s[r] - eps) { rr = r; break; }
 
            Act(rl, x, false, vl);
            Act(rr, x, true, vr);
            double lenL = x1s[rl] - x0s[rl], lenR = x1s[rr] - x0s[rr];
            double wl = lenL > eps ? Math.Min(1d, Math.Max(0d, (x - x0s[rl]) / lenL)) : 0d;
            double wr = lenR > eps ? Math.Min(1d, Math.Max(0d, (x - x0s[rr]) / lenR)) : 0d;
            for (int a = 0; a < 6; a++)
            {
                vl[a] += delta[rl * 6 + a] * wl;
                vr[a] += delta[rr * 6 + a] * wr;
            }
            if (k == m - 1) Array.Copy(vl, vr, 6);
 
            res.Add(new barInternForce
            {
                ID = model.barInternForces.NextID(),BarID = barId, Case = cas, Subdivision = k + 1, Distance = x,
                Fxi = vl[0], Fyi = vl[1], Fzi = vl[2], Mxi = vl[3], Myi = vl[4], Mzi = vl[5],
                Fxj = vr[0], Fyj = vr[1], Fzj = vr[2], Mxj = vr[3], Myj = vr[4], Mzj = vr[5]
            });
        }
    }

// Linear superposition of solved cases (loadCombs.Type == 0).
    // Creates one load case "comb_<name>" per combo with summed end forces, reactions and displacements.
    // Must run before calculeBarInternalForcesStations (which builds the combo stations).
    // Already existing combo cases are skipped (remove the case first to rebuild it).
    internal static void SetSuperpositionCombosResults(ModelDB model)
    {
        int ncomb = model.loadCombs.Count;
        int ncs = model.loadCases.Count;
        int nbars = model.bars.Count;
        int nnodes = model.nodes.Count;
 
        // Distinct superposition combo names
        var names = new List<string>();
        for (int k = 0; k < ncomb; k++)
        {
            var c = model.loadCombs[k];
            if (c.Type == 0 && !names.Contains(c.Name)) names.Add(c.Name);
        }
        if (names.Count == 0) return;
 
        int maxId = 0;
        var existing = new HashSet<string>();
        for (int i = 0; i < ncs; i++)
        {
            var lc = model.loadCases[i];
            if (lc.ID > maxId) maxId = lc.ID;
            existing.Add(lc.Name);
        }
 
        // Row indices of the solved results by (id, case); first/last row of a bar gives end i / end j
        var barFirst = new Dictionary<long, int>();
        var barLast = new Dictionary<long, int>();
        int nbf = model.barInternForces.Count;
        for (int i = 0; i < nbf; i++)
        {
            var f = model.barInternForces[i];
            long key = pairKey(f.BarID, f.Case);
            if (!barFirst.ContainsKey(key)) barFirst[key] = i;
            barLast[key] = i;
        }
        var rxIdx = new Dictionary<long, int>();
        int nrx = model.nodalReactions.Count;
        for (int i = 0; i < nrx; i++) rxIdx[pairKey(model.nodalReactions[i].NodeID, model.nodalReactions[i].Case)] = i;
        var dpIdx = new Dictionary<long, int>();
        int ndp = model.nodalDisplacements.Count;
        for (int i = 0; i < ndp; i++) dpIdx[pairKey(model.nodalDisplacements[i].NodeID, model.nodalDisplacements[i].Case)] = i;
 
        var cc = new List<int>(); var cf = new List<double>();
 
        for (int n = 0; n < names.Count; n++)
        {
            string combName = names[n];
            if (existing.Contains(comboPrefix + combName)) continue;
 
            cc.Clear(); cf.Clear();
            for (int k = 0; k < ncomb; k++)
            {
                var cd = model.loadCombs[k];
                if (cd.Type == 0 && cd.Name == combName) { cc.Add(cd.Case); cf.Add(cd.Factor); }
            }
 
            var ncaso = new loadCase()
            {
                ID = ++maxId,
                Type = (int)LoadCaseType.Combination,
                Name = comboPrefix + combName,
            };
            model.loadCases.Add(ncaso);
 
            int bStart = model.barInternForces.Count;
            
            for (int i = 0; i < nbars; i++)
                model.barInternForces.Add(new barInternForce() { ID = model.barInternForces.NextID(),BarID = model.bars[i].ID, Case = ncaso.ID });
 
            int rStart = model.nodalReactions.Count, dStart = model.nodalDisplacements.Count;

   
            for (int i = 0; i < nnodes; i++)
            {
                int nodeId = model.nodes[i].ID;
                model.nodalReactions.Add(new nodalReaction() {ID= model.nodalReactions.NextID(), NodeID = nodeId, Case = ncaso.ID });
                model.nodalDisplacements.Add(new nodalDisplacement() { ID = model.nodalDisplacements.NextID(),NodeID = nodeId, Case = ncaso.ID });
            }
 
            for (int j = 0; j < cc.Count; j++)
            {
                int cas = cc[j];
                double fact = cf[j];
 
                for (int i = 0; i < nbars; i++)
                {
                    long key = pairKey(model.bars[i].ID, cas);
                    if (!barFirst.TryGetValue(key, out int fi)) continue;
                    var a = model.barInternForces[fi];          // end i values
                    var b = model.barInternForces[barLast[key]]; // end j values
                    var t = model.barInternForces[bStart + i];
                    t.Fxi += fact * a.Fxi; t.Fyi += fact * a.Fyi; t.Fzi += fact * a.Fzi;
                    t.Mxi += fact * a.Mxi; t.Myi += fact * a.Myi; t.Mzi += fact * a.Mzi;
                    t.Fxj += fact * b.Fxj; t.Fyj += fact * b.Fyj; t.Fzj += fact * b.Fzj;
                    t.Mxj += fact * b.Mxj; t.Myj += fact * b.Myj; t.Mzj += fact * b.Mzj;
                }
 
                for (int i = 0; i < nnodes; i++)
                {
                    long key = pairKey(model.nodes[i].ID, cas);
                    if (rxIdx.TryGetValue(key, out int ri))
                    {
                        var a = model.nodalReactions[ri];
                        var t = model.nodalReactions[rStart + i];
                        t.Fx += fact * a.Fx; t.Fy += fact * a.Fy; t.Fz += fact * a.Fz;
                        t.Mx += fact * a.Mx; t.My += fact * a.My; t.Mz += fact * a.Mz;
                    }
                    if (dpIdx.TryGetValue(key, out int di))
                    {
                        var a = model.nodalDisplacements[di];
                        var t = model.nodalDisplacements[dStart + i];
                        t.dx += fact * a.dx; t.dy += fact * a.dy; t.dz += fact * a.dz;
                        t.rx += fact * a.rx; t.ry += fact * a.ry; t.rz += fact * a.rz;
                    }
                }
            }
        }
    }

}