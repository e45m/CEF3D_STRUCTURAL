namespace CEF;

// Area internal actions view.
// Reads the solved vertex values in Engine.getModel().areaInternalForces (one row per area/case, 4 vertex values per action).
// Areas: the preselected ones in Selection.getAreasColection() (indices into Engine.getModel().areas).
public class viewInternalAreaActions : Form
{
    internal Label lblElement, lblCase, lblAction, lblPanel, lblMin, lblMax;
    internal Label[] lblVert = new Label[4];
    internal TextBox EleTipo, EleNudos, EleId, minVal, maxVal;
    internal TextBox[] valVert = new TextBox[4];
    internal Button btnTable, exit;
    internal ComboBox cb_selElem, loadCase, cb_internalAction;
    internal PictureBox pbDiagramCanvas;
    internal GroupBox gbElData;

    // ---------------- configuration ----------------
    static readonly string[] ActionNames = { "Fx", "Fy", "Fz", "Mx", "My", "Mz" };
    static readonly Color lowerColor = Color.RoyalBlue, upperColor = Color.Crimson;
    const int Alpha = 190;
    const int Subdivisions = 12;   // grid cells per side used to paint the bilinear field
    const int DefaultAction = 0;

    // ASSUMPTIONS (change only these members if the real names differ)
    static int[] areaNodeIds(Area a) => new[] { a.Node1, a.Node2, a.Node3, a.Node4 };
    static string areaTypeText(Area a) => a.SectionName + "";
    static int[] selectedAreas() => Selection.getAreasColection();

    // User units (Uc). Internally everything is in kN, m.
    string forceUnit = "", lenUnit = "", momUnit = "";

    // ---------------- data ----------------
    int[] areaIdx = Array.Empty<int>();
    bool loading;
    bool hasData;
    int curAction;
    int[] nodeIds = new int[4];
    double[] vals = new double[4];          // vertex values in user units
    double[] qx = new double[4], qy = new double[4];  // vertex coordinates in the area plane (model units)
    double maxV, minV;
    int maxK, minK;

    // hover
    bool hovering;
    PointF hoverPt;
    double /*hoverU, hoverV,*/ hoverVal;

    // canvas layout
    PointF[] scr = new PointF[4];

    // graphic resources
    Font fontSmall, fontBold;
    Pen outlinePen, vertPen;
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
        outlinePen = new Pen(Color.Black, 1.4f);
        vertPen = new Pen(Color.Black, 1f);
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
            Size = new Size(292, 140)
        };
        gbElData.Controls.AddRange(new Control[]
        {
            addLabel("Id", 24, 30),
            addLabel(I18n.Type, 24, 62),
            addLabel(I18n.Nodes + ":", 24, 94),
            EleId    = addTextBox(100, 26),
            EleTipo  = addTextBox(100, 58),
            EleNudos = addTextBox(100, 90)
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

        // === Values on the right ===
        var side = new List<Control>();
        lblPanel = addLabel("", 668, 198);
        lblPanel.Font = new Font(Font.FontFamily, 9f, FontStyle.Bold);
        side.Add(lblPanel);
        for (int k = 0; k < 4; k++)
        {
            lblVert[k] = addLabel("", 668, 232 + k * 52);
            valVert[k] = addTextBox(668, 250 + k * 52, 220);
            side.Add(lblVert[k]); side.Add(valVert[k]);
        }
        lblMax = addLabel(I18n.max, 668, 232 + 4 * 52);
        maxVal = addTextBox(668, 250 + 4 * 52, 220);
        lblMin = addLabel(I18n.min, 668, 232 + 5 * 52);
        minVal = addTextBox(668, 250 + 5 * 52, 220);
        side.AddRange(new Control[] { lblMax, maxVal, lblMin, minVal });
        foreach (var c in side) c.Anchor = AnchorStyles.Top | AnchorStyles.Right;

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
        Text = I18n.AreaInternalForces;   
        CancelButton = exit;

        Controls.AddRange(new Control[]
        {
            cb_selElem, lblElement,
            loadCase, lblCase,
            cb_internalAction, lblAction,
            gbElData,
            pbDiagramCanvas
        });
        Controls.AddRange(side.ToArray());
        Controls.AddRange(new Control[] { btnTable, exit });

        Load += View_Load;

        ResumeLayout(false);
        PerformLayout();
    }

    public viewInternalAreaActions()
    {
        InitializeComponent();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            fontSmall?.Dispose();
            fontBold?.Dispose();
            outlinePen?.Dispose();
            vertPen?.Dispose();
            tipBack?.Dispose();
        }
        base.Dispose(disposing);
    }

    // ---------------- events ----------------
    private void Exit_Click(object sender, EventArgs e) => Close();
    private void Table_Click(object sender, EventArgs e) => showResultsTable();
    private void Selection_Changed(object sender, EventArgs e) => Rebuild();
    private void Case_Changed(object sender, EventArgs e) => Rebuild();
    private void Action_Changed(object sender, EventArgs e) => Rebuild();

    private void View_Load(object sender, EventArgs e)
    {
        loading = true;

        forceUnit = Uc.activeForceU.ToString();
        lenUnit = Uc.activeLenU.ToString();
        momUnit = forceUnit + "·" + lenUnit;

        areaIdx = selectedAreas() ?? Array.Empty<int>();
        cb_selElem.Items.Clear();
        for (int i = 0; i < areaIdx.Length; i++)
            cb_selElem.Items.Add(Engine.getModel().areas[areaIdx[i]].ID.ToString());

        loadCase.Items.Clear();
        for (int i = 0; i < Engine.getModel().loadCases.Count; i++)
            loadCase.Items.Add(Engine.getModel().loadCases[i].ID + ": " + Engine.getModel().loadCases[i].Name);

        cb_internalAction.Items.Clear();
        for (int i = 0; i < ActionNames.Length; i++) cb_internalAction.Items.Add(ActionNames[i]);

        if (cb_selElem.Items.Count > 0) cb_selElem.SelectedIndex = 0;
        int active = Engine.getModel().loadCases.FindIndexByID(Engine.getModel().getParamValue(paramDefaults.ActiveCase));
        if (loadCase.Items.Count > 0) loadCase.SelectedIndex = active >= 0 ? active : 0;
        cb_internalAction.SelectedIndex = DefaultAction;

        loading = false;
        Rebuild();
    }

    // ---------------- data access ----------------
    private static double areaVal(areaInternalForce r, int a, int k)
    {
        switch (a)
        {
            case 0: return k == 0 ? r.Fx1 : k == 1 ? r.Fx2 : k == 2 ? r.Fx3 : r.Fx4;
            case 1: return k == 0 ? r.Fy1 : k == 1 ? r.Fy2 : k == 2 ? r.Fy3 : r.Fy4;
            case 2: return k == 0 ? r.Fz1 : k == 1 ? r.Fz2 : k == 2 ? r.Fz3 : r.Fz4;
            case 3: return k == 0 ? r.Mx1 : k == 1 ? r.Mx2 : k == 2 ? r.Mx3 : r.Mx4;
            case 4: return k == 0 ? r.My1 : k == 1 ? r.My2 : k == 2 ? r.My3 : r.My4;
            default: return k == 0 ? r.Mz1 : k == 1 ? r.Mz2 : k == 2 ? r.Mz3 : r.Mz4;
        }
    }

    private string unitOf(int a) => a < 3 ? forceUnit : momUnit;
    private static double toUser(int a, double v) => a < 3 ? Uc.forceToUserUnits(v) : Uc.momentToUserUnits(v);
    private static string fmt(double v) => Math.Abs(v) < 1e-12 ? "0" : v.ToString("G5");

    // Table of all cases of the selected area, in user units
    private void showResultsTable()
    {
        int asel = cb_selElem.SelectedIndex;
        if (asel < 0) return;
        int areaId = Engine.getModel().areas[areaIdx[asel]].ID;

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
        for (int a = 0; a < 6; a++)
            for (int k = 0; k < 4; k++)
                grid.Columns.Add("a" + a + k, ActionNames[a] + (k + 1) + " [" + unitOf(a) + "]");

        var rows = new List<areaInternalForce>();
        var tbl = Engine.getModel().areaInternalForces;
        for (int i = 0; i < tbl.Count; i++)
            if (tbl[i].AreaID == areaId) rows.Add(tbl[i]);
        rows.Sort((p, q) => p.Case.CompareTo(q.Case));

        var cells = new object[25];
        for (int i = 0; i < rows.Count; i++)
        {
            cells[0] = rows[i].Case;
            for (int a = 0; a < 6; a++)
                for (int k = 0; k < 4; k++)
                    cells[1 + a * 4 + k] = fmt(toUser(a, areaVal(rows[i], a, k)));
            grid.Rows.Add(cells);
        }

        var f = new Form
        {
            Text = I18n.TableView + " - " + I18n.Element + " " + areaId,
            Size = new Size(1300, 500),
            StartPosition = FormStartPosition.CenterParent,
            ShowInTaskbar = false
        };
        f.Controls.Add(grid);
        f.ShowDialog(this);
        f.Dispose();
    }

    // ---------------- diagram data ----------------
    private void clearSideBoxes()
    {
        lblPanel.Text = "";
        for (int k = 0; k < 4; k++) { lblVert[k].Text = ""; valVert[k].Text = ""; }
        maxVal.Text = minVal.Text = "";
    }

    private void setSideBoxes()
    {
        lblPanel.Text = ActionNames[curAction] + " [" + unitOf(curAction) + "]";
        for (int k = 0; k < 4; k++)
        {
            lblVert[k].Text = (k + 1) + " [" + nodeIds[k] + "]";
            valVert[k].Text = fmt(vals[k]);
        }
        maxVal.Text = fmt(maxV) + "   (" + (maxK + 1) + ")";
        minVal.Text = fmt(minV) + "   (" + (minK + 1) + ")";
    }

    private void Rebuild()
    {
        if (loading) return;
        hasData = false;
        hovering = false;
        EleId.Text = EleTipo.Text = EleNudos.Text = "";
        clearSideBoxes();

        int asel = cb_selElem.SelectedIndex, csel = loadCase.SelectedIndex, actSel = cb_internalAction.SelectedIndex;
        if (asel < 0 || csel < 0 || actSel < 0)
        {
            pbDiagramCanvas.Invalidate();
            return;
        }

        var area = Engine.getModel().areas[areaIdx[asel]];
        int areaId = area.ID;
        int cas = Engine.getModel().loadCases[csel].ID;
        curAction = actSel;

        // Vertex coordinates
        nodeIds = areaNodeIds(area);
        var px = new double[4]; var py = new double[4]; var pz = new double[4];
        for (int k = 0; k < 4; k++)
        {
            int ni = Engine.getModel().nodes.FindIndexByID(nodeIds[k]);
            if (ni < 0) { pbDiagramCanvas.Invalidate(); return; }
            px[k] = Engine.getModel().nodes[ni].X;
            py[k] = Engine.getModel().nodes[ni].Y;
            pz[k] = Engine.getModel().nodes[ni].Z;
        }
        EleId.Text = areaId.ToString();
        EleTipo.Text = areaTypeText(area);
        EleNudos.Text = nodeIds[0] + " - " + nodeIds[1] + " - " + nodeIds[2] + " - " + nodeIds[3];

        // Local plane frame: e1 along 1->2, normal = e1 x (1->4), e2 = n x e1
        double e1x = px[1] - px[0], e1y = py[1] - py[0], e1z = pz[1] - pz[0];
        double l1 = Math.Sqrt(e1x * e1x + e1y * e1y + e1z * e1z);
        if (l1 < 1e-12) { pbDiagramCanvas.Invalidate(); return; }
        e1x /= l1; e1y /= l1; e1z /= l1;
        double wx = px[3] - px[0], wy = py[3] - py[0], wz = pz[3] - pz[0];
        double nx = e1y * wz - e1z * wy, ny = e1z * wx - e1x * wz, nz = e1x * wy - e1y * wx;
        double ln = Math.Sqrt(nx * nx + ny * ny + nz * nz);
        if (ln < 1e-12) { pbDiagramCanvas.Invalidate(); return; }
        nx /= ln; ny /= ln; nz /= ln;
        double e2x = ny * e1z - nz * e1y, e2y = nz * e1x - nx * e1z, e2z = nx * e1y - ny * e1x;
        for (int k = 0; k < 4; k++)
        {
            double dx = px[k] - px[0], dy = py[k] - py[0], dz = pz[k] - pz[0];
            qx[k] = dx * e1x + dy * e1y + dz * e1z;
            qy[k] = dx * e2x + dy * e2y + dz * e2z;
        }

        // Vertex values for the case (single pass, no assumption about table order)
        areaInternalForce row = null;
        var tbl = Engine.getModel().areaInternalForces;
        for (int i = 0; i < tbl.Count; i++)
            if (tbl[i].AreaID == areaId && tbl[i].Case == cas) { row = tbl[i]; break; }
        if (row == null) { pbDiagramCanvas.Invalidate(); return; }

        maxV = double.NegativeInfinity; minV = double.PositiveInfinity; maxK = minK = 0;
        for (int k = 0; k < 4; k++)
        {
            vals[k] = toUser(curAction, areaVal(row, curAction, k));
            if (vals[k] > maxV) { maxV = vals[k]; maxK = k; }
            if (vals[k] < minV) { minV = vals[k]; minK = k; }
        }

        hasData = true;
        setSideBoxes();
        pbDiagramCanvas.Invalidate();
    }

    // ---------------- drawing ----------------
    private Color colorOf(double v)
    {
        double range = maxV - minV;
        double t = range < 1e-12 ? 0.5 : Math.Clamp((v - minV) / range, 0d, 1d);
        return Color.FromArgb(Alpha,
            (int)(lowerColor.R + (upperColor.R - lowerColor.R) * t),
            (int)(lowerColor.G + (upperColor.G - lowerColor.G) * t),
            (int)(lowerColor.B + (upperColor.B - lowerColor.B) * t));
    }

    // Fit the area plane into the canvas (aspect preserved, v axis up)
    private void computeLayout()
    {
        const float margin = 48f, barH = 34f;
        float W = pbDiagramCanvas.ClientSize.Width, H = pbDiagramCanvas.ClientSize.Height - barH;
        double xmin = double.MaxValue, xmax = double.MinValue, ymin = double.MaxValue, ymax = double.MinValue;
        for (int k = 0; k < 4; k++)
        {
            if (qx[k] < xmin) xmin = qx[k];
            if (qx[k] > xmax) xmax = qx[k];
            if (qy[k] < ymin) ymin = qy[k];
            if (qy[k] > ymax) ymax = qy[k];
        }
        double sx = Math.Max(xmax - xmin, 1e-12), sy = Math.Max(ymax - ymin, 1e-12);
        double s = Math.Min((W - 2 * margin) / sx, (H - 2 * margin) / sy);
        if (s <= 0d) s = 1d;
        double ox = (W - s * sx) / 2d, oy = (H - s * sy) / 2d;
        for (int k = 0; k < 4; k++)
            scr[k] = new PointF((float)(ox + (qx[k] - xmin) * s), (float)(oy + (ymax - qy[k]) * s));
    }

    private PointF mapUV(double u, double v)
    {
        double w1 = (1 - u) * (1 - v), w2 = u * (1 - v), w3 = u * v, w4 = (1 - u) * v;
        return new PointF(
            (float)(w1 * scr[0].X + w2 * scr[1].X + w3 * scr[2].X + w4 * scr[3].X),
            (float)(w1 * scr[0].Y + w2 * scr[1].Y + w3 * scr[2].Y + w4 * scr[3].Y));
    }

    private double bilinear(double u, double v) =>
        (1 - u) * (1 - v) * vals[0] + u * (1 - v) * vals[1] + u * v * vals[2] + (1 - u) * v * vals[3];

    // Inverse bilinear map (Newton) from a canvas point; false if outside the quad
    private bool inverseMap(PointF p, out double u, out double v)
    {
        u = 0.5d; v = 0.5d;
        for (int it = 0; it < 12; it++)
        {
            var c = mapUV(u, v);
            double rx = p.X - c.X, ry = p.Y - c.Y;
            double dux = (1 - v) * (scr[1].X - scr[0].X) + v * (scr[2].X - scr[3].X);
            double duy = (1 - v) * (scr[1].Y - scr[0].Y) + v * (scr[2].Y - scr[3].Y);
            double dvx = (1 - u) * (scr[3].X - scr[0].X) + u * (scr[2].X - scr[1].X);
            double dvy = (1 - u) * (scr[3].Y - scr[0].Y) + u * (scr[2].Y - scr[1].Y);
            double det = dux * dvy - duy * dvx;
            if (Math.Abs(det) < 1e-9) return false;
            u += (rx * dvy - ry * dvx) / det;
            v += (dux * ry - duy * rx) / det;
        }
        const double tol = 1e-3;
        return u >= -tol && u <= 1d + tol && v >= -tol && v <= 1d + tol;
    }

    private void Canvas_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        if (!hasData)
        {
            g.DrawString(I18n.NoResultsForSelection, fontSmall, Brushes.Gray, 10f, 10f);
            return;
        }
        computeLayout();

        // Bilinear field, painted without antialiasing so the semi-transparent cells show no seams
        var oldSm = g.SmoothingMode;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        var cell = new PointF[4];
        for (int r = 0; r < Subdivisions; r++)
        {
            double v0 = (double)r / Subdivisions, v1 = (double)(r + 1) / Subdivisions;
            for (int c = 0; c < Subdivisions; c++)
            {
                double u0 = (double)c / Subdivisions, u1 = (double)(c + 1) / Subdivisions;
                cell[0] = mapUV(u0, v0); cell[1] = mapUV(u1, v0);
                cell[2] = mapUV(u1, v1); cell[3] = mapUV(u0, v1);
                using var br = new SolidBrush(colorOf(bilinear((u0 + u1) / 2d, (v0 + v1) / 2d)));
                g.FillPolygon(br, cell);
            }
        }
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.DrawPolygon(outlinePen, scr);

        // Vertex markers and values, pushed outwards from the centroid
        float cx = (scr[0].X + scr[1].X + scr[2].X + scr[3].X) / 4f;
        float cy = (scr[0].Y + scr[1].Y + scr[2].Y + scr[3].Y) / 4f;
        for (int k = 0; k < 4; k++)
        {
            g.FillEllipse(Brushes.White, scr[k].X - 3.5f, scr[k].Y - 3.5f, 7f, 7f);
            g.DrawEllipse(vertPen, scr[k].X - 3.5f, scr[k].Y - 3.5f, 7f, 7f);
            string s = (k + 1) + ": " + fmt(vals[k]);
            var sz = g.MeasureString(s, fontBold);
            float tx = scr[k].X + (scr[k].X >= cx ? 8f : -8f - sz.Width);
            float ty = scr[k].Y + (scr[k].Y >= cy ? 6f : -6f - sz.Height);
            g.DrawString(s, fontBold, Brushes.Black, tx, ty);
        }

        // Color bar at the bottom: min .. max
        float by = pbDiagramCanvas.ClientSize.Height - 26f, bx0 = 58f, bw = Math.Max(10f, pbDiagramCanvas.ClientSize.Width - 116f);
        for (int i = 0; i < (int)bw; i++)
            using (var p = new Pen(colorOf(minV + (maxV - minV) * i / bw)))
                g.DrawLine(p, bx0 + i, by, bx0 + i, by + 10f);
        g.DrawRectangle(Pens.Gray, bx0, by, bw, 10f);
        string smin = fmt(minV), smax = fmt(maxV) + " " + unitOf(curAction);
        g.DrawString(smin, fontSmall, Brushes.Black, bx0 - g.MeasureString(smin, fontSmall).Width - 2f, by - 1f);
        g.DrawString(smax, fontSmall, Brushes.Black, bx0 + bw + 2f, by - 1f);

        // Hover: value at the cursor
        if (hovering)
        {
            g.FillEllipse(Brushes.White, hoverPt.X - 3f, hoverPt.Y - 3f, 6f, 6f);
            g.DrawEllipse(vertPen, hoverPt.X - 3f, hoverPt.Y - 3f, 6f, 6f);
            string tip = ActionNames[curAction] + " = " + fmt(hoverVal) + " " + unitOf(curAction);
            var tsz = g.MeasureString(tip, fontSmall);
            float tx = hoverPt.X + 14f, ty = hoverPt.Y + 14f;
            if (tx + tsz.Width + 6f > pbDiagramCanvas.ClientSize.Width) tx = hoverPt.X - 14f - tsz.Width - 6f;
            if (ty + tsz.Height + 4f > pbDiagramCanvas.ClientSize.Height) ty = hoverPt.Y - 14f - tsz.Height - 4f;
            g.FillRectangle(tipBack, tx, ty, tsz.Width + 6f, tsz.Height + 4f);
            g.DrawRectangle(Pens.Gray, tx, ty, tsz.Width + 6f, tsz.Height + 4f);
            g.DrawString(tip, fontSmall, Brushes.Black, tx + 3f, ty + 2f);
        }
        g.SmoothingMode = oldSm;
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!hasData) return;
        computeLayout();
        var p = new PointF(e.X, e.Y);
        if (inverseMap(p, out double u, out double v))
        {
            u = Math.Clamp(u, 0d, 1d); v = Math.Clamp(v, 0d, 1d);
            hovering = true;
            hoverPt = p;
            hoverVal = bilinear(u, v);
        }
        else if (!hovering) return;
        else hovering = false;
        pbDiagramCanvas.Invalidate();
    }

    private void Canvas_MouseLeave(object sender, EventArgs e)
    {
        if (!hovering) return;
        hovering = false;
        pbDiagramCanvas.Invalidate();
    }
}