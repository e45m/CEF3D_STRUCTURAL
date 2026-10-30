namespace CEF;

// Bar internal actions view.
// Reads the precomputed stations in App.appData.barStatiosForces (see calculeBarInternalForcesStations).
// Bars: the preselected ones in Selection.getBarsColection() (indices into App.model.bars).
public class viewInternalBarActions : Form
{
    internal Label lblElement, lblCase, lblMin, lblMax, lbl_J, lbl_I, lblAction, lblPanel;
    internal TextBox EleTipo, EleNudos, EleLong, EleId, minVal, maxVal, valJ, valI;
    internal Button btnTable;
    internal ComboBox cb_selElem, loadCase, cb_internalAction;
    internal Button exit;
    internal PictureBox pbDiagramCanvas;
    internal GroupBox gbElData;

    public static int CifDec = 9;

    // ---------------- configuration ----------------
    static readonly string[] ActionNames = { "N", "Vy", "Vz", "Mx", "My", "Mz" };
    static readonly Color[] ActionColors =
        { Color.RoyalBlue, Color.SeaGreen, Color.Teal, Color.MediumPurple, Color.DarkOrange, Color.Crimson };
    // Drawing sign per action (use -1 to draw it on the opposite side)
    static readonly double[] ySign = { 1d, 1d, 1d, 1d, 1d, 1d };
    // Action sets; mask: bit0=N, bit1=Vy, bit2=Vz, bit3=Mx, bit4=My, bit5=Mz
    static readonly string[] SetNames =
    {
        I18n.ActionSetAll,
        I18n.ActionSetPlaneXY,
        I18n.ActionSetPlaneXZ,
        I18n.ActionSetShears,
        I18n.ActionSetMoments,
        I18n.ActionAxial,
        I18n.ActionShearY,
        I18n.ActionShearZ,
        I18n.ActionTorsion,
        I18n.ActionMomentY,
        I18n.ActionMomentZ
    };
    static readonly int[] SetMasks = { 63, 1 | 2 | 32, 1 | 4 | 16, 2 | 4, 16 | 32, 1, 2, 4, 8, 16, 32 };
    const int DefaultSet = 1;

    // User units (Uc). Internally everything is in kN, m.
    string forceUnit = "", lenUnit = "", momUnit = "";

    // ---------------- data ----------------
    private sealed class ActionCurve
    {
        public int a;
        public double[] px, py;
        public double maxV, maxX, minV, minX;
    }

    int[] barIdx = Array.Empty<int>();
    bool loading;
    readonly List<ActionCurve> curves = new();
    double barL;

    // hover
    int hoverPanel = -1, activePanel = -1;
    double hoverX;
    Point hoverMouse;

    // canvas layout
    float plotL, plotW, panelH;
    float[] panelTop = Array.Empty<float>();
    double[] panelHi = Array.Empty<double>(), panelLo = Array.Empty<double>();

    // graphic resources
    Font fontSmall, fontBold;
    Pen[] pens;
    Brush[] fills;
    Pen dashPen;
    SolidBrush tipBack;

    // ---------------- UI construction ----------------
    private Label addLabel(string text, int x, int y) =>
        new Label { Text = text, Location = new Point(x, y), AutoSize = true };

    private TextBox addTextBox(int x, int y, int w = 140, bool enabled = false) =>
        new TextBox { Location = new Point(x, y), Size = new Size(w, 23), ReadOnly = !enabled };

    private ComboBox addComboBox(int x, int y, int w, EventHandler ev = null)
    {
        var cb = new ComboBox
        {
            Location = new Point(x, y),
            Size = new Size(w, 23),
            DropDownStyle = ComboBoxStyle.DropDownList,
            FormattingEnabled = true
        };
        if (ev != null) cb.SelectedIndexChanged += ev;
        return cb;
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        fontSmall = new Font(Font.FontFamily, 8f);
        fontBold = new Font(Font.FontFamily, 8.5f, FontStyle.Bold);
        pens = new Pen[6];
        fills = new Brush[6];
        for (int i = 0; i < 6; i++)
        {
            pens[i] = new Pen(ActionColors[i], 1.6f);
            fills[i] = new SolidBrush(Color.FromArgb(70, ActionColors[i]));
        }
        dashPen = new Pen(Color.Gray) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        tipBack = new SolidBrush(Color.FromArgb(235, 255, 255, 255));

        // === Main combos ===
        cb_selElem = addComboBox(477, 30, 340, Selection_Changed);
        loadCase = addComboBox(477, 61, 340, Case_Changed);
        cb_internalAction = addComboBox(477, 90, 340, Action_Changed);

        lblElement = addLabel(I18n.Element + ":", 356, 33);
        lblCase = addLabel(I18n.CaseOrCombination, 356, 64);
        lblAction = addLabel(I18n.Action, 356, 94);

        // === Element summary ===
        gbElData = new GroupBox
        {
            Text = I18n.ElementSummary,
            Location = new Point(12, 12),
            Size = new Size(292, 170)
        };
        gbElData.Controls.AddRange(new Control[]
        {
            addLabel("Id", 24, 30),
            addLabel(I18n.Type, 24, 62),
            addLabel(I18n.Length, 24, 94),
            addLabel(I18n.Nodes + ":", 24, 126),
            EleId    = addTextBox(100, 26),
            EleTipo  = addTextBox(100, 58),
            EleLong  = addTextBox(100, 90),
            EleNudos = addTextBox(100, 122)
        });

        // === Canvas (resizes with the window) ===
        pbDiagramCanvas = new PictureBox
        {
            BackColor = Color.LightYellow,
            Location = new Point(12, 196),
            Size = new Size(640, 392),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            BorderStyle = BorderStyle.FixedSingle
        };
        pbDiagramCanvas.Paint += Canvas_Paint;
        pbDiagramCanvas.MouseMove += Canvas_MouseMove;
        pbDiagramCanvas.MouseLeave += Canvas_MouseLeave;
        pbDiagramCanvas.Resize += (s, e) => pbDiagramCanvas.Invalidate();

        // === Values on the right (active panel / under the cursor) ===
        lblPanel = addLabel("", 668, 198);
        lblPanel.Font = new Font(Font.FontFamily, 9f, FontStyle.Bold);
        lbl_I = addLabel(I18n.ValueAtStartNode, 668, 232);
        valI = addTextBox(668, 250, 220);
        lbl_J = addLabel(I18n.ValueAtEndNode, 668, 284);
        valJ = addTextBox(668, 302, 220);
        lblMax = addLabel(I18n.max, 668, 336);
        maxVal = addTextBox(668, 354, 220);
        lblMin = addLabel(I18n.min, 668, 388);
        minVal = addTextBox(668, 406, 220);
        foreach (var c in new Control[] { lblPanel, lbl_I, valI, lbl_J, valJ, lblMax, maxVal, lblMin, minVal })
            c.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        // === Buttons ===
        exit = new Button
        {
            Text = I18n.Exit,
            Location = new Point(774, 565),
            Size = new Size(114, 23),
            DialogResult = DialogResult.Cancel,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        exit.Click += Exit_Click;
        btnTable = new Button
        {
            Text = I18n.TableView,
            Location = new Point(654, 565),
            Size = new Size(114, 23),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        btnTable.Click += Table_Click;

        // === Form ===
        ClientSize = new Size(900, 600);
        MinimumSize = new Size(780, 520);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = I18n.BarInternalForces;
        CancelButton = exit;

        Controls.AddRange(new Control[]
        {
            cb_selElem, lblElement,
            loadCase, lblCase,
            cb_internalAction, lblAction,
            gbElData,
            pbDiagramCanvas,
            lblPanel, lbl_I, lbl_J, lblMax, lblMin,
            valI, valJ, maxVal, minVal,
            btnTable, exit
        });

        Load += View_Load;

        ResumeLayout(false);
        PerformLayout();
    }

    public viewInternalBarActions()
    {
        InitializeComponent();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            fontSmall?.Dispose();
            fontBold?.Dispose();
            dashPen?.Dispose();
            tipBack?.Dispose();
            if (pens != null) foreach (var p in pens) p?.Dispose();
            if (fills != null) foreach (var b in fills) b?.Dispose();
        }
        base.Dispose(disposing);
    }

    // ---------------- events ----------------
    private void Exit_Click(object sender, EventArgs e) => Close();
    private void Table_Click(object sender, EventArgs e) => showStationsTable();

    // Table of all station rows (every case) of the selected bar, in user units
    private void showStationsTable()
    {
        int bsel = cb_selElem.SelectedIndex;
        if (!App.appData.biFSeted || bsel < 0) return;
        int barId = App.model.bars[barIdx[bsel]].ID;

        string[] act = { "N", "Vy", "Vz", "Mx", "My", "Mz" };
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        };
        grid.Columns.Add("c", I18n.Case);
        grid.Columns.Add("s", "#");
        grid.Columns.Add("x", "x [" + lenUnit + "]");
        for (int side = 0; side < 2; side++)
            for (int a = 0; a < 6; a++)
                grid.Columns.Add("a" + side + a, act[a] + (side == 0 ? " (i-)" : " (j+)") + " [" + unitOf(a) + "]");

        var rows = new List<barInternForce>();
        var tbl = App.appData.barStatiosForces;
        for (int i = 0; i < tbl.Count; i++)
            if (tbl[i].BarID == barId) rows.Add(tbl[i]);
        rows.Sort((p, q) => p.Case != q.Case ? p.Case.CompareTo(q.Case) : p.Subdivision.CompareTo(q.Subdivision));

        var vals = new object[15];
        for (int i = 0; i < rows.Count; i++)
        {
            var r = rows[i];
            vals[0] = r.Case;
            vals[1] = r.Subdivision;
            vals[2] = fmt(Uc.lenToUserUnits(r.Distance));
            for (int side = 0; side < 2; side++)
                for (int a = 0; a < 6; a++)
                {
                    double v = actionVal(r, a, side == 1);
                    vals[3 + side * 6 + a] = fmt(a < 3 ? Uc.forceToUserUnits(v) : Uc.momentToUserUnits(v));
                }
            grid.Rows.Add(vals);
        }

        var f = new Form
        {
            Text = I18n.TableView + " - " + I18n.Element + " " + barId,
            Size = new Size(1100, 500),
            StartPosition = FormStartPosition.CenterParent,
            ShowInTaskbar = false
        };
        f.Controls.Add(grid);
        f.ShowDialog(this);
        f.Dispose();
    }
    private void Selection_Changed(object sender, EventArgs e) => Rebuild();
    private void Case_Changed(object sender, EventArgs e) => Rebuild();
    private void Action_Changed(object sender, EventArgs e) => Rebuild();

    private void View_Load(object sender, EventArgs e)
    {
        loading = true;

        // User units (Uc); internally the data is in kN, m
        forceUnit = Uc.activeForceU.ToString();
        lenUnit = Uc.activeLenU.ToString();
        momUnit = forceUnit + "·" + lenUnit;

        var sel = Selection.getBarsColection();
        barIdx = sel ?? Array.Empty<int>();
        cb_selElem.Items.Clear();
        for (int i = 0; i < barIdx.Length; i++)
            cb_selElem.Items.Add(App.model.bars[barIdx[i]].ID.ToString());

        // Cases come straight from the model; the selected index maps to App.model.loadCases[i]
        loadCase.Items.Clear();
        for (int i = 0; i < App.model.loadCases.Count; i++)
            loadCase.Items.Add(App.model.loadCases[i].ID + ": " + App.model.loadCases[i].Name);

        cb_internalAction.Items.Clear();
        for (int i = 0; i < SetNames.Length; i++) cb_internalAction.Items.Add(SetNames[i]);

        if (cb_selElem.Items.Count > 0) cb_selElem.SelectedIndex = 0;
        int active = App.model.loadCases.FindIndexByID(App.model.getParamValue(paramDefaults.ActiveCase));
        if (loadCase.Items.Count > 0) loadCase.SelectedIndex = active >= 0 ? active : 0;
        cb_internalAction.SelectedIndex = DefaultSet;

        loading = false;
        Rebuild();
    }

    // ---------------- diagram data ----------------
    private void clearSideBoxes()
    {
        lblPanel.Text = "";
        valI.Text = valJ.Text = maxVal.Text = minVal.Text = "";
    }

    private void setSideBoxes(int p)
    {
        if (p < 0 || p >= curves.Count) { clearSideBoxes(); return; }
        var c = curves[p];
        lblPanel.Text = ActionNames[c.a] + " [" + unitOf(c.a) + "]   x [" + lenUnit + "]";
        valI.Text = fmt(c.py[0]);
        valJ.Text = fmt(c.py[c.py.Length - 1]);
        maxVal.Text = fmt(c.maxV) + "   (x = " + fmt(c.maxX) + ")";
        minVal.Text = fmt(c.minV) + "   (x = " + fmt(c.minX) + ")";
    }

    private void Rebuild()
    {
        if (loading) return;
        curves.Clear();
        barL = 0d;
        hoverPanel = activePanel = -1;
        EleId.Text = EleTipo.Text = EleLong.Text = EleNudos.Text = "";
        clearSideBoxes();

        int bsel = cb_selElem.SelectedIndex, csel = loadCase.SelectedIndex, asel = cb_internalAction.SelectedIndex;
        if (!App.appData.biFSeted || bsel < 0 || csel < 0 || asel < 0)
        {
            pbDiagramCanvas.Invalidate();
            return;
        }

        int bi = barIdx[bsel];
        var bar = App.model.bars[bi];
        int barId = bar.ID;
        int cas = App.model.loadCases[csel].ID;

        // Element data (in user units)
        int ni = App.model.nodes.FindIndexByID(bar.startNode);
        int nj = App.model.nodes.FindIndexByID(bar.endNode);
        double dx = App.model.nodes[nj].X - App.model.nodes[ni].X;
        double dy = App.model.nodes[nj].Y - App.model.nodes[ni].Y;
        double dz = App.model.nodes[nj].Z - App.model.nodes[ni].Z;
        EleId.Text = barId.ToString();
        EleTipo.Text = barTypeText(bar);
        EleLong.Text = fmt(Uc.lenToUserUnits(Math.Sqrt(dx * dx + dy * dy + dz * dz))) + " " + lenUnit;
        EleNudos.Text = bar.startNode + " - " + bar.endNode;

        // Stations of the bar/case: filter only, no assumption about table order
        var st = new List<barInternForce>();
        var tbl = App.appData.barStatiosForces;
        for (int i = 0; i < tbl.Count; i++)
        {
            var r = tbl[i];
            if (r.Case != cas || r.BarID != barId) continue;
            st.Add(r);
        }
        st.Sort((p, q) => p.Subdivision.CompareTo(q.Subdivision));
        // No results for this bar/case: two zero stations (i, j) so the canvas shows zero effect
        if (st.Count < 2)
        {
            st.Clear();
            double len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            st.Add(new barInternForce { BarID = barId, Case = cas, Subdivision = 1, Distance = 0d });
            st.Add(new barInternForce { BarID = barId, Case = cas, Subdivision = 2, Distance = len });
        }
        if (st.Count >= 2)
        {
            barL = Uc.lenToUserUnits(st[st.Count - 1].Distance);
            int mask = SetMasks[asel];
            for (int a = 0; a < 6; a++)
            {
                if ((mask & (1 << a)) == 0) continue;
                var c = buildCurve(a, st);
                if (c != null) curves.Add(c);
            }
        }
        activePanel = curves.Count > 0 ? 0 : -1;
        setSideBoxes(activePanel);
        pbDiagramCanvas.Invalidate();
    }

    // Element "type": name of the bar section
    // (if the Bar member has a different name, change only this line)
    private static string barTypeText(Bar b) => b.SectionName + "";

    private string unitOf(int a) => a < 3 ? forceUnit : momUnit;

    private static double actionVal(barInternForce r, int a, bool right)
    {
        switch (a)
        {
            case 0: return right ? r.Fxj : r.Fxi;
            case 1: return right ? r.Fyj : r.Fyi;
            case 2: return right ? r.Fzj : r.Fzi;
            case 3: return right ? r.Mxj : r.Mxi;
            case 4: return right ? r.Myj : r.Myi;
            default: return right ? r.Mzj : r.Mzi;
        }
    }

    // Curve of action a: polyline joining the station values directly (no interpolation).
    private ActionCurve buildCurve(int a, List<barInternForce> st)
    {
        int m = st.Count;
        if (m < 2) return null;
        var px = new List<double>(m * 2);
        var py = new List<double>(m * 2);
        double maxV = double.NegativeInfinity, minV = double.PositiveInfinity, maxX = 0d, minX = 0d;

        for (int k = 0; k < m - 1; k++)
        {
            var r0 = st[k];
            var r1 = st[k + 1];
            double x0 = r0.Distance, x1 = r1.Distance, h = x1 - x0;
            if (h <= 0d) continue;
            double v0 = actionVal(r0, a, true), v1 = actionVal(r1, a, false);

            px.Add(x0); py.Add(v0);
            if (v0 > maxV) { maxV = v0; maxX = x0; }
            if (v0 < minV) { minV = v0; minX = x0; }

            px.Add(x1); py.Add(v1);
            if (v1 > maxV) { maxV = v1; maxX = x1; }
            if (v1 < minV) { minV = v1; minX = x1; }
        }
        if (px.Count < 2) return null;

        // Everything was computed in internal units (kN, m); converted to user units with Uc
        var cx = px.ToArray();
        var cy = py.ToArray();
        bool isForce = a < 3;
        for (int i = 0; i < cx.Length; i++)
        {
            cx[i] = Uc.lenToUserUnits(cx[i]);
            cy[i] = isForce ? Uc.forceToUserUnits(cy[i]) : Uc.momentToUserUnits(cy[i]);
        }
        return new ActionCurve
        {
            a = a, px = cx, py = cy,
            maxV = isForce ? Uc.forceToUserUnits(maxV) : Uc.momentToUserUnits(maxV),
            minV = isForce ? Uc.forceToUserUnits(minV) : Uc.momentToUserUnits(minV),
            maxX = Uc.lenToUserUnits(maxX),
            minX = Uc.lenToUserUnits(minX)
        };
    }

    private static double valueAt(ActionCurve c, double x)
    {
        int n = c.px.Length;
        if (x <= c.px[0]) return c.py[0];
        for (int i = 1; i < n; i++)
        {
            if (c.px[i] < x) continue;
            double dxs = c.px[i] - c.px[i - 1];
            if (dxs < 1e-12) return c.py[i];
            return c.py[i - 1] + (c.py[i] - c.py[i - 1]) * (x - c.px[i - 1]) / dxs;
        }
        return c.py[n - 1];
    }

    private static string fmt(double v) => Math.Abs(v) < 1e-12 ? "0" : v.ToString("G5");

    // ---------------- layout and drawing ----------------
    private void computeLayout()
    {
        int n = curves.Count;
        if (panelTop.Length != n)
        {
            panelTop = new float[n];
            panelHi = new double[n];
            panelLo = new double[n];
        }
        float W = pbDiagramCanvas.ClientSize.Width, H = pbDiagramCanvas.ClientSize.Height;
        plotL = 58f;
        plotW = Math.Max(10f, W - 58f - 14f);
        panelH = Math.Max(24f, (H - 24f - 18f - 22f * (n - 1)) / n);
        for (int p = 0; p < n; p++)
        {
            panelTop[p] = 24f + p * (panelH + 22f);
            var c = curves[p];
            double sv = ySign[c.a];
            double s1 = c.maxV * sv, s2 = c.minV * sv;
            double hi = Math.Max(Math.Max(s1, s2), 0d), lo = Math.Min(Math.Min(s1, s2), 0d);
            if (hi - lo < 1e-12) { hi = 1d; lo = -1d; }
            panelHi[p] = hi;
            panelLo[p] = lo;
        }
    }

    private float PX(double x) => plotL + (float)(x / barL) * plotW;

    private float PY(int p, double v)
    {
        const float pad = 12f;
        double sv = ySign[curves[p].a];
        return panelTop[p] + pad + (float)((panelHi[p] - v * sv) / (panelHi[p] - panelLo[p])) * (panelH - 2f * pad);
    }

    private void drawMark(Graphics g, int p, double x, double v, int a)
    {
        float px = PX(x), py = PY(p, v);
        g.FillEllipse(Brushes.White, px - 3.5f, py - 3.5f, 7f, 7f);
        g.DrawEllipse(pens[a], px - 3.5f, py - 3.5f, 7f, 7f);
        string s = fmt(v);
        var sz = g.MeasureString(s, fontSmall);
        float tx = px + 6f;
        if (tx + sz.Width > plotL + plotW) tx = px - 6f - sz.Width;
        float ty = Math.Min(Math.Max(py - sz.Height / 2f, panelTop[p] + 1f), panelTop[p] + panelH - sz.Height - 1f);
        g.DrawString(s, fontSmall, Brushes.Black, tx, ty);
    }

    private void Canvas_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        int n = curves.Count;
        if (n == 0 || barL <= 0d)
        {
            g.DrawString(I18n.NoResultsForSelection, fontSmall, Brushes.Gray, 10f, 10f);
            return;
        }
        computeLayout();

        for (int p = 0; p < n; p++)
        {
            var c = curves[p];
            int a = c.a;
            float top = panelTop[p];
            float y0 = PY(p, 0d);

            g.DrawRectangle(Pens.Silver, plotL, top, plotW, panelH);
            g.DrawLine(Pens.Gray, plotL, y0, plotL + plotW, y0);
            using (var tb = new SolidBrush(ActionColors[a]))
                g.DrawString(ActionNames[a] + " [" + unitOf(a) + "]", fontBold, tb, plotL, top - 17f);
            g.DrawString("0", fontSmall, Brushes.Gray, plotL - 14f, y0 - 7f);

            int m = c.px.Length;
            var pts = new PointF[m];
            for (int i = 0; i < m; i++) pts[i] = new PointF(PX(c.px[i]), PY(p, c.py[i]));
            var poly = new PointF[m + 2];
            Array.Copy(pts, poly, m);
            poly[m] = new PointF(pts[m - 1].X, y0);
            poly[m + 1] = new PointF(pts[0].X, y0);
            g.FillPolygon(fills[a], poly);
            g.DrawLines(pens[a], pts);

            if (c.maxV - c.minV > 1e-9 * Math.Max(1d, Math.Abs(c.maxV) + Math.Abs(c.minV)))
            {
                drawMark(g, p, c.maxX, c.maxV, a);
                drawMark(g, p, c.minX, c.minV, a);
            }
        }

        // Horizontal scale below the last panel
        float by = panelTop[n - 1] + panelH + 2f;
        g.DrawString("0", fontSmall, Brushes.Gray, plotL - 3f, by);
        string sl = fmt(barL) + " " + lenUnit;
        g.DrawString(sl, fontSmall, Brushes.Gray, plotL + plotW - g.MeasureString(sl, fontSmall).Width + 3f, by);

        // Hover
        if (hoverPanel >= 0)
        {
            float hx = PX(hoverX);
            g.DrawLine(dashPen, hx, panelTop[0], hx, panelTop[n - 1] + panelH);
            var sb = new System.Text.StringBuilder();
            sb.Append("x = ").Append(fmt(hoverX)).Append(' ').Append(lenUnit);
            for (int p = 0; p < n; p++)
            {
                var c = curves[p];
                double v = valueAt(c, hoverX);
                float hy = PY(p, v);
                g.FillEllipse(Brushes.White, hx - 3f, hy - 3f, 6f, 6f);
                g.DrawEllipse(pens[c.a], hx - 3f, hy - 3f, 6f, 6f);
                sb.Append('\n').Append(ActionNames[c.a]).Append(" = ").Append(fmt(v)).Append(' ').Append(unitOf(c.a));
            }
            string tip = sb.ToString();
            var tsz = g.MeasureString(tip, fontSmall);
            float tx = hoverMouse.X + 16f, ty = hoverMouse.Y + 16f;
            if (tx + tsz.Width + 6f > pbDiagramCanvas.ClientSize.Width) tx = hoverMouse.X - 16f - tsz.Width - 6f;
            if (ty + tsz.Height + 4f > pbDiagramCanvas.ClientSize.Height) ty = hoverMouse.Y - 16f - tsz.Height - 4f;
            g.FillRectangle(tipBack, tx, ty, tsz.Width + 6f, tsz.Height + 4f);
            g.DrawRectangle(Pens.Gray, tx, ty, tsz.Width + 6f, tsz.Height + 4f);
            g.DrawString(tip, fontSmall, Brushes.Black, tx + 3f, ty + 2f);
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        int n = curves.Count;
        if (n == 0 || barL <= 0d) return;
        computeLayout();
        if (e.X < plotL || e.X > plotL + plotW || e.Y < panelTop[0] || e.Y > panelTop[n - 1] + panelH)
        {
            if (hoverPanel >= 0) { hoverPanel = -1; pbDiagramCanvas.Invalidate(); }
            return;
        }
        hoverX = (e.X - plotL) / plotW * barL;
        hoverMouse = e.Location;
        int p = n - 1;
        while (p > 0 && e.Y < panelTop[p]) p--;
        hoverPanel = p;
        if (p != activePanel)
        {
            activePanel = p;
            setSideBoxes(p);
        }
        pbDiagramCanvas.Invalidate();
    }

    private void Canvas_MouseLeave(object sender, EventArgs e)
    {
        if (hoverPanel < 0) return;
        hoverPanel = -1;
        pbDiagramCanvas.Invalidate();
    }
}