namespace CEF;

// Shared helpers for area local-axes operations.
// Local axes of an area: x = node1 -> node2, y in the plane towards node1 -> node4, z = x cross (node1 -> node4).
internal static partial class AreaAxes
{
    // ASSUMPTION: Area node members are Node1..Node4 (change only these two methods if the names differ)
    internal static void getNodes(Area a, int[] n)
    {
        n[0] = a.Node1; n[1] = a.Node2; n[2] = a.Node3; n[3] = a.Node4;
    }

    internal static void setNodes(Area a, int[] n)
    {
        a.Node1 = n[0]; a.Node2 = n[1]; a.Node3 = n[2]; a.Node4 = n[3];
    }

    // Fills p[12] with the vertex coordinates (x,y,z per node); false if a node is missing
    internal static bool getCoords(ModelDB model, int[] n, double[] p)
    {
        for (int k = 0; k < 4; k++)
        {
            int ni = model.nodes.FindIndexByID(n[k]);
            if (ni < 0) return false;
            p[3 * k] = model.nodes[ni].X;
            p[3 * k + 1] = model.nodes[ni].Y;
            p[3 * k + 2] = model.nodes[ni].Z;
        }
        return true;
    }

    // Local frame for the vertex order o (indices 0..3 into p): f = ex(0..2), ey(3..5), ez(6..8). False if degenerate
    internal static bool getFrame(double[] p, int[] o, double[] f)
    {
        int a = 3 * o[0], b = 3 * o[1], d = 3 * o[3];
        double xx = p[b] - p[a], xy = p[b + 1] - p[a + 1], xz = p[b + 2] - p[a + 2];
        double wx = p[d] - p[a], wy = p[d + 1] - p[a + 1], wz = p[d + 2] - p[a + 2];
        double lx = Math.Sqrt(xx * xx + xy * xy + xz * xz);
        if (lx < 1e-12) return false;
        xx /= lx; xy /= lx; xz /= lx;
        double zx = xy * wz - xz * wy, zy = xz * wx - xx * wz, zz = xx * wy - xy * wx;
        double lz = Math.Sqrt(zx * zx + zy * zy + zz * zz);
        if (lz < 1e-12) return false;
        zx /= lz; zy /= lz; zz /= lz;
        f[0] = xx; f[1] = xy; f[2] = xz;
        f[3] = zy * xz - zz * xy; f[4] = zz * xx - zx * xz; f[5] = zx * xy - zy * xx;
        f[6] = zx; f[7] = zy; f[8] = zz;
        return true;
    }

    // Aligns the local axes of the selected areas with those of the first valid area of the selection.
    // For each area the 8 node orders (4 cyclic shifts x 2 senses) are tried and the one whose axes
    // are closest to the reference axes is kept. Returns the number of areas modified.
    // NOTE: results and local-axis loads of modified areas are no longer valid.
    internal static int alignAreaAxes(ModelDB model, int[] areaIdx)
    {
        if (areaIdx == null || areaIdx.Length == 0) return 0;

        var n = new int[4]; var nn = new int[4];
        var p = new double[12];
        var o = new int[4]; var bo = new int[4];
        var identity = new[] { 0, 1, 2, 3 };
        var f = new double[9]; var r = new double[9];
        bool haveRef = false;
        int changed = 0;

        for (int i = 0; i < areaIdx.Length; i++)
        {
            var ar = model.areas[areaIdx[i]];
            getNodes(ar, n);
            if (!getCoords(model, n, p)) continue;
            if (!getFrame(p, identity, f)) continue;

            if (!haveRef)
            {
                Array.Copy(f, r, 9);
                haveRef = true;
                continue;
            }

            // Score = ex.rx + ey.ry + ez.rz (9 products, same layout)
            double best = 0d;
            for (int k = 0; k < 9; k++) best += f[k] * r[k];
            bool found = false;

            for (int s = 0; s < 4; s++)
            {
                for (int rev = 0; rev < 2; rev++)
                {
                    if (s == 0 && rev == 0) continue;
                    for (int k = 0; k < 4; k++)
                        o[k] = rev == 0 ? (s + k) % 4 : ((s - k) % 4 + 4) % 4;
                    if (!getFrame(p, o, f)) continue;
                    double score = 0d;
                    for (int k = 0; k < 9; k++) score += f[k] * r[k];
                    if (score > best + 1e-9)
                    {
                        best = score;
                        Array.Copy(o, bo, 4);
                        found = true;
                    }
                }
            }

            if (!found) continue;
            for (int k = 0; k < 4; k++) nn[k] = n[bo[k]];
            setNodes(ar, nn);
            changed++;
        }
        return changed;
    
}

    // Reverses the node order of the selected areas, which inverts the local z axis.
    // keepOrigin = true : order 1,4,3,2 (origin stays at node 1; x and y swap roles).
    // keepOrigin = false: order 4,3,2,1 (x direction is kept, y is inverted; origin moves to node 4).
    // Returns the number of areas modified.
    // NOTE: results and local-axis loads of modified areas are no longer valid.
    internal static int flipAreaZ(ModelDB model, int[] areaIdx, bool keepOrigin = true)
    {
        if (areaIdx == null || areaIdx.Length == 0) return 0;

        var n = new int[4]; var nn = new int[4];
        for (int i = 0; i < areaIdx.Length; i++)
        {
            var ar = model.areas[areaIdx[i]];
            getNodes(ar, n);
            if (keepOrigin) { nn[0] = n[0]; nn[1] = n[3]; nn[2] = n[2]; nn[3] = n[1]; }
            else { nn[0] = n[3]; nn[1] = n[2]; nn[2] = n[1]; nn[3] = n[0]; }
            setNodes(ar, nn);
        }
        return areaIdx.Length;
    }

    // Moves the origin of the selected areas by cyclic shift of the node order (z is preserved).
    // steps = 1: origin goes to node 2 (x axis turns 90 deg); negative steps rotate the other way.
    // Returns the number of areas modified.
    // NOTE: results and local-axis loads of modified areas are no longer valid.
    internal static int rotateAreaOrigin(ModelDB model, int[] areaIdx, int steps = 1)
    {
        if (areaIdx == null || areaIdx.Length == 0) return 0;
        int s = ((steps % 4) + 4) % 4;
        if (s == 0) return 0;

        var n = new int[4]; var nn = new int[4];
        for (int i = 0; i < areaIdx.Length; i++)
        {
            var ar = model.areas[areaIdx[i]];
            getNodes(ar, n);
            for (int k = 0; k < 4; k++) nn[k] = n[(s + k) % 4];
            setNodes(ar, nn);
        }
        return areaIdx.Length;
    }


}



