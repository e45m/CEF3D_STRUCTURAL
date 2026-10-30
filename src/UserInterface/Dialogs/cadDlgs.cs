using System;

namespace CEF;

// Input dialogs of the CAD tools in MainWindow (offsets, rotation, replication, division).
// Each one validates before closing (invalid boxes are marked and the dialog stays open)
// and returns its values in internal units.
internal static class cadDialogs
{
    private const int W = 320;   // client width of every dialog

    private static readonly Color badColor = Color.MistyRose;

    #region Offset (copy / move)

    internal static bool askOffset(IWin32Window owner, out double dx, out double dy, out double dz)
    {
        double x = 0, y = 0, z = 0;
        dx = dy = dz = 0;

        using var f = newForm(I18n.ImputOffests);
        var gb = dlgStyle.group(I18n.ImputOffests, 10, 10, W - 20, 118);
        f.Controls.Add(gb);

        var tx = row(gb, I18n.offsetX2, 24);
        var ty = row(gb, I18n.offsetY2, 52);
        var tz = row(gb, I18n.offsetZ2, 80);

        bool ok = run(f, owner, 140, () =>
        {
            bool good = num(tx, out x);
            good &= num(ty, out y);
            good &= num(tz, out z);
            return good;
        });
        if (!ok) return false;

        dx = Uc.lenFromUserUnits(x);
        dy = Uc.lenFromUserUnits(y);
        dz = Uc.lenFromUserUnits(z);
        return true;
    }

    #endregion

    #region Rotation

    // Angles in radians (typed in degrees), center in internal length units
    internal static bool askRotation(IWin32Window owner,
        out double ax, out double ay, out double az,
        out double cx, out double cy, out double cz)
    {
        double rx = 0, ry = 0, rz = 0, x = 0, y = 0, z = 0;
        ax = ay = az = cx = cy = cz = 0;

        using var f = newForm(I18n.ImputRotation);

        var gbAng = dlgStyle.group(I18n.ImputRotation, 10, 10, W - 20, 118);
        var tax = row(gbAng, I18n.RotationX, 24);
        var tay = row(gbAng, I18n.RotationY, 52);
        var taz = row(gbAng, I18n.RotationZ, 80);

        var gbCen = dlgStyle.group(I18n.ImputRotationCent, 10, 136, W - 20, 118);
        var tcx = row(gbCen, I18n.RotCenX, 24);
        var tcy = row(gbCen, I18n.RotCenY, 52);
        var tcz = row(gbCen, I18n.RotCenZ, 80);

        f.Controls.AddRange(new Control[] { gbAng, gbCen });

        bool ok = run(f, owner, 266, () =>
        {
            bool good = num(tax, out rx);
            good &= num(tay, out ry);
            good &= num(taz, out rz);
            good &= num(tcx, out x);
            good &= num(tcy, out y);
            good &= num(tcz, out z);

            // A rotation of 0 on every axis does nothing: flag the angles
            if (good && rx == 0 && ry == 0 && rz == 0)
            {
                tax.BackColor = tay.BackColor = taz.BackColor = badColor;
                good = false;
            }
            return good;
        });
        if (!ok) return false;

        double toRad = Math.PI / 180.0;
        ax = rx * toRad;
        ay = ry * toRad;
        az = rz * toRad;
        cx = Uc.lenFromUserUnits(x);
        cy = Uc.lenFromUserUnits(y);
        cz = Uc.lenFromUserUnits(z);
        return true;
    }

    #endregion

    #region Replication

    // Direction vector and spacings (steps, not cumulative) in internal length units
    internal static bool askReplicate(IWin32Window owner,
        out double dx, out double dy, out double dz, out double[] steps)
    {
        double x = 0, y = 0, z = 0;
        double[] st = null;
        dx = dy = dz = 0;
        steps = Array.Empty<double>();

        using var f = newForm(I18n.ImputDireVect);

        var gbDir = dlgStyle.group(I18n.ImputDireVect, 10, 10, W - 20, 118);
        var tdx = row(gbDir, I18n.DireVectX, 24);
        var tdy = row(gbDir, I18n.DireVectY, 52);
        var tdz = row(gbDir, I18n.DireVectZ, 80);

        var gbSp = dlgStyle.group(I18n.ImputSpacing, 10, 136, W - 20, 92);
        dlgStyle.label(gbSp, I18n.ImputSpacingExample, 10, 20, W - 40);
        var tsp = row(gbSp, I18n.Spacing, 52);

        f.Controls.AddRange(new Control[] { gbDir, gbSp });

        bool ok = run(f, owner, 240, () =>
        {
            bool good = num(tdx, out x);
            good &= num(tdy, out y);
            good &= num(tdz, out z);

            // The direction needs a length
            if (good && x == 0 && y == 0 && z == 0)
            {
                tdx.BackColor = tdy.BackColor = tdz.BackColor = badColor;
                good = false;
            }

            st = parseSteps(tsp.Text);
            if (st == null)
            {
                tsp.BackColor = badColor;
                good = false;
            }
            return good;
        });
        if (!ok) return false;

        dx = Uc.lenFromUserUnits(x);
        dy = Uc.lenFromUserUnits(y);
        dz = Uc.lenFromUserUnits(z);

        steps = new double[st.Length];
        for (int i = 0; i < st.Length; i++)
            steps[i] = Uc.lenFromUserUnits(st[i]);   // spacings are typed in user units
        return true;
    }

    // "n*x, y, ..." (separators from I18n) -> steps in typed units; null if any token is invalid
    private static double[] parseSteps(string text)
    {
        var list = new List<double>();

        foreach (var part in text.Split(I18n.SplitChar))
        {
            string item = part.Trim();
            int times = 1;

            if (item.Length == 0) continue;

            if (item.Contains(I18n.GroupingKey))
            {
                var g = item.Split(I18n.GroupingKey);
                if (g.Length != 2 || !int.TryParse(g[0], out times) || times < 1) return null;
                item = g[1];
            }

            if (!double.TryParse(item, out double v)) return null;

            for (int i = 0; i < times; i++)
                list.Add(v);
        }

        return list.ToArray();
    }

    #endregion

    #region Division

    // byLength: false = number of parts (parts >= 2), true = target element length (internal units)
    internal static bool askDivide(IWin32Window owner, out bool byLength, out int parts, out double length)
    {
        bool dist = false;
        int n = 2;
        double len = 0;
        byLength = false;
        parts = 2;
        length = 0;

        string lenUn = App.currUnits.Split(",")[2];   // "," is always ","

        using var f = newForm("");

        var gb = dlgStyle.group("", 10, 10, W - 20, 82);
        var rbDivs = new RadioButton { Text = I18n.DivideByElemsCuant, Checked = true, Left = 10, Top = 20, AutoSize = true };
        var rbDist = new RadioButton { Text = $"{I18n.DivideByEleemLen} {lenUn}:", Left = 10, Top = 48, AutoSize = true };
        gb.Controls.AddRange(new Control[] { rbDivs, rbDist });

        var msg = new Label { Left = 20, Top = 109, Width = 205, AutoEllipsis = true, Text = I18n.ImputMsgDivideByElemsCuant };
        var tb = new TextBox { Left = 230, Top = 106, Width = 70, Text = "2" };
        tb.TextChanged += (_, _) => tb.BackColor = SystemColors.Window;
        rbDivs.CheckedChanged += (_, _) =>
            msg.Text = rbDivs.Checked ? I18n.ImputMsgDivideByElemsCuant : I18n.ImputMsgDivideByEleemLen;

        f.Controls.AddRange(new Control[] { gb, msg, tb });

        bool ok = run(f, owner, 146, () =>
        {
            dist = rbDist.Checked;
            bool good = dist ? (num(tb, out len) && len > 0)
                             : (int.TryParse(tb.Text, out n) && n >= 2);
            if (!good) tb.BackColor = badColor;
            return good;
        });
        if (!ok) return false;

        byLength = dist;
        parts = n;
        length = dist ? Uc.lenFromUserUnits(len) : 0;
        return true;
    }

    #endregion

    #region Helpers

    private static Form newForm(string title)
    {
        var f = new Form();
        dlgStyle.applyForm(f);
        f.ClientSize = new Size(W, 200);   // final height is set in run()
        f.Text = title;
        return f;
    }

    // Label + text box inside a group (x fixed: 10 / 160)
    private static TextBox row(Control parent, string label, int y, string text = "0")
    {
        dlgStyle.label(parent, label, 10, y, 150);

        var tb = new TextBox { Left = 160, Top = y, Width = 130, Text = text };
        tb.TextChanged += (_, _) => tb.BackColor = SystemColors.Window;
        parent.Controls.Add(tb);
        return tb;
    }

    // Empty = 0; invalid text marks the box and returns false
    private static bool num(TextBox tb, out double v)
    {
        v = 0;
        string t = tb.Text.Trim();

        if (t.Length == 0) return true;
        if (double.TryParse(t, out v)) return true;

        v = 0;
        tb.BackColor = badColor;
        return false;
    }

    // Adds OK/Cancel at btnY and shows the dialog; OK only closes it when validate() is true
    private static bool run(Form f, IWin32Window owner, int btnY, Func<bool> validate)
    {
        f.ClientSize = new Size(W, btnY + 40);

        var ok = dlgStyle.button(I18n.DlgOKText, W - 185, btnY, 85, true);
        var cancel = dlgStyle.button(I18n.DlgCancelText, W - 95, btnY, 75);
        cancel.DialogResult = DialogResult.Cancel;
        ok.Click += (_, _) =>
        {
            if (validate()) f.DialogResult = DialogResult.OK;
        };

        f.Controls.AddRange(new Control[] { ok, cancel });
        f.AcceptButton = ok;
        f.CancelButton = cancel;

        return f.ShowDialog(owner) == DialogResult.OK;
    }

    #endregion
}