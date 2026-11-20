namespace CEF;

// Shared helpers for bar local-axes operations.
// The local frame comes from space3d (the local x may run from j to i, so it is always read from space3d, never assumed).
internal static partial class BarAxes
{
    // ASSUMPTION: Bar.Angle = roll of the local axes about x, in degrees (change only these two methods if it differs)
    internal static double getRoll(Bar b) => b.Rotation;
    internal static void setRoll(Bar b, double deg) { b.Rotation = deg; }

    // Swaps the end nodes and the end releases (bits 0-5 = end i, bits 6-11 = end j)
    internal static void swapEnds(Bar b)
    {
        int t = b.startNode; b.startNode = b.endNode; b.endNode = t;
        b.releases = ((b.releases & 63) << 6) | ((b.releases >> 6) & 63);

    }

    // Local frame (without roll) of a bar going from node ni to node nj: f = ex(0..2), ey(3..5), ez(6..8)
    internal static bool getFrame(ModelDB model, int ni, int nj, double[] f)
    {
        int a = model.nodes.FindIndexByID(ni), b = model.nodes.FindIndexByID(nj);
        if (a < 0 || b < 0) return false;

        var globRef = new double[]
        {
            model.nodes[a].X, model.nodes[a].Y, model.nodes[a].Z,
            model.nodes[b].X, model.nodes[b].Y, model.nodes[b].Z
        };
        // Local origin and unit points of each axis, mapped to global
        var loc = new double[] { 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1 };
        var glob = new double[12];
        space3d.loc2glob(globRef, ref glob, loc);

        for (int k = 0; k < 3; k++)
        {
            double x = glob[3 * (k + 1)] - glob[0];
            double y = glob[3 * (k + 1) + 1] - glob[1];
            double z = glob[3 * (k + 1) + 2] - glob[2];
            double l = Math.Sqrt(x * x + y * y + z * z);
            if (l < 1e-12) return false;
            f[3 * k] = x / l; f[3 * k + 1] = y / l; f[3 * k + 2] = z / l;
        }
        return true;
    }

    // Rotates ey and ez about ex by the roll angle (degrees)
    internal static void applyRoll(double[] f, double deg)
    {
        if (Math.Abs(deg) < 1e-12) return;
        double c = Math.Cos(deg * Math.PI / 180d), s = Math.Sin(deg * Math.PI / 180d);
        for (int k = 0; k < 3; k++)
        {
            double y = f[3 + k], z = f[6 + k];
            f[3 + k] = c * y + s * z;
            f[6 + k] = -s * y + c * z;
        }
    }


    // Aligns the local axes of the selected bars with those of the first valid bar of the selection.
    // For each bar both end orders (i-j, j-i) are tried and the one whose axes are closest to the reference is kept.
    // Roll is not considered. Returns the number of bars modified.
    // NOTE: results and loads (a, b fractions, local components) of modified bars are no longer valid.
    internal static int alignBarAxes(ModelDB model, int[] barIdx)
    {
        if (barIdx == null || barIdx.Length == 0) return 0;

        var f = new double[9]; var g = new double[9]; var r = new double[9];
        bool haveRef = false;
        int changed = 0;

        for (int i = 0; i < barIdx.Length; i++)
        {
            var bar = model.bars[barIdx[i]];
            if (!getFrame(model, bar.startNode, bar.endNode, f)) continue;

            if (!haveRef)
            {
                Array.Copy(f, r, 9);
                haveRef = true;
                continue;
            }
            if (!getFrame(model, bar.endNode, bar.startNode, g)) continue;

            // Score = ex.rx + ey.ry + ez.rz (same layout)
            double keep = 0d, swap = 0d;
            for (int k = 0; k < 9; k++) { keep += f[k] * r[k]; swap += g[k] * r[k]; }
            if (swap > keep + 1e-9)
            {
                swapEnds(bar);
                changed++;
            }
        }
        return changed;
    }


    // Reverses the direction of the selected bars (swaps start/end nodes and end releases).
    // This is the bar equivalent of inverting the local axes. Returns the number of bars modified.
    // NOTE: results and loads (a, b fractions, local components) of modified bars are no longer valid.
    internal static int reverseBarDirection(ModelDB model, int[] barIdx)
    {
        if (barIdx == null || barIdx.Length == 0) return 0;
        for (int i = 0; i < barIdx.Length; i++) swapEnds(model.bars[barIdx[i]]);
        return barIdx.Length;
    }

    // Rotates the local axes of the selected bars about their x axis (adds angleDeg to the roll). Returns the number of bars modified.
    // NOTE: results and local-axis loads of modified bars are no longer valid.
    internal static int rotateBarAxes(ModelDB model, int[] barIdx, double angleDeg = 90d)
    {
        if (barIdx == null || barIdx.Length == 0) return 0;
        for (int i = 0; i < barIdx.Length; i++)
        {
            var bar = model.bars[barIdx[i]];
            setRoll(bar, getRoll(bar) + angleDeg);
        }
        return barIdx.Length;
    }


}