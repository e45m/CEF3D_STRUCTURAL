using System.Data.Common;
using System.Drawing.Drawing2D;
using static CEF.ModelDB;
using static System.Math;
using Font = System.Drawing.Font;
namespace CEF;
public partial class viewFrame
{


private void drawBarsInternalDiagrams(Brush brush, Graphics g, Pen pen,
    BarActions actions = BarActions.All, double diagHeight = 1.0, double[] refMax = null,
    double[] plotSign = null)
{
    if (!Engine.getAppData().biFSeted ) return;
    var list = Engine.getAppData().barStatiosForces;
    int cnt = list.Count;
    int activeCase = Engine.getModel().getParamValue(paramDefaults.ActiveCase);
    int[] offsetAxis = { 0, 0, 1, 1, 1, 0 }; // 0 = eje local Y, 1 = eje local Z
 
    // Escala por acción
    var fre = new double[6];
    var mx = new double[6];
    if (refMax != null) Array.Copy(refMax, mx, 6);
    else
    {
        for (int i = 0; i < cnt; i++)
        {
            var r = list[i];
            if (r.Case != activeCase) continue;
            for (int a = 0; a < 6; a++)
            {
                if (((int)actions & (1 << a)) == 0) continue;
                double v = Math.Max(Math.Abs(barForceVal(r, a, false)), Math.Abs(barForceVal(r, a, true)));
                if (v > mx[a]) mx[a] = v;
            }
        }
    }
    for (int a = 0; a < 6; a++) fre[a] = mx[a] > 1e-12 ? diagHeight / mx[a] : 0d;
 
    int curBar = -1;
    bool barOk = false;
    var o = new double[3];
    var ax = new double[9]; // ex, ey, ez globales (unitarios)
    var seg = new double[16];
 
    for (int i = 0; i < cnt - 1; i++)
    {
        var r0 = list[i];
        if (r0.Case != activeCase) continue;
        var r1 = list[i + 1];
        if (r1.Case != activeCase || r1.BarID != r0.BarID || r1.St != r0.St + 1)
            continue;
 
        if (r0.BarID != curBar)
        {
            curBar = r0.BarID;
            int bi = Engine.getModel().bars.FindIndexByID(curBar);
            int ni = Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[bi].startNode);
            int nj = Engine.getModel().nodes.FindIndexByID(Engine.getModel().bars[bi].endNode);
            double sx = Engine.getModel().nodes[ni].X, sy = Engine.getModel().nodes[ni].Y, sz = Engine.getModel().nodes[ni].Z;
            double ex = Engine.getModel().nodes[nj].X, ey = Engine.getModel().nodes[nj].Y, ez = Engine.getModel().nodes[nj].Z;
            barOk = levelValidation(sx, sy, sz) && levelValidation(ex, ey, ez);
            if (barOk)
            {
                // ex: de i a j, sin depender del sentido del eje x que entrega space3d
                double bx = ex - sx, by = ey - sy, bz = ez - sz;
                double bl = Math.Sqrt(bx * bx + by * by + bz * bz);
                if (bl <= 0d) barOk = false;
                else
                {
                    bx /= bl; by /= bl; bz /= bl;
 
                    // ey de space3d (origen local y +1 en y local), llevado a global
                    double[] globRef = { sx, sy, sz, ex, ey, ez };
                    double[] gn = { sx, sy, sz, sx, sy, sz, ex, ey, ez, ex, ey, ez };
                    var loc = new double[12];
                    space3d.glob2loc(globRef, gn, ref loc);
                    double lx = loc[0], ly = loc[1], lz = loc[2];
                    double[] l2 = { lx, ly, lz, lx, ly + 1d, lz, lx, ly, lz, lx, ly + 1d, lz };
                    space3d.loc2glob(globRef, ref gn, l2);
                    double yx = gn[3] - gn[0], yy = gn[4] - gn[1], yz = gn[5] - gn[2];
 
                    // ortogonalizar ey respecto a ex y normalizar
                    double dp = yx * bx + yy * by + yz * bz;
                    yx -= dp * bx; yy -= dp * by; yz -= dp * bz;
                    double yl = Math.Sqrt(yx * yx + yy * yy + yz * yz);
                    if (yl < 1e-9) barOk = false;
                    else
                    {
                        yx /= yl; yy /= yl; yz /= yl;
                        o[0] = sx; o[1] = sy; o[2] = sz;
                        ax[0] = bx; ax[1] = by; ax[2] = bz;                    // ex
                        ax[3] = yx; ax[4] = yy; ax[5] = yz;                    // ey
                        ax[6] = by * yz - bz * yy;                             // ez = ex x ey
                        ax[7] = bz * yx - bx * yz;
                        ax[8] = bx * yy - by * yx;
                    }
                }
            }
        }
        if (!barOk) continue;
 
        double xa = r0.Pos, xb = r1.Pos;
        for (int a = 0; a < 6; a++)
        {
            if (((int)actions & (1 << a)) == 0 || fre[a] == 0d) continue;
            double sg = plotSign == null ? 1d : plotSign[a];
            double va = barForceVal(r0, a, true) * fre[a] * sg;   // derecha de la estación k
            double vb = barForceVal(r1, a, false) * fre[a] * sg;  // izquierda de la estación k+1
            int d = offsetAxis[a] == 0 ? 3 : 6;              // índice de ey (3) o ez (6) en ax
            for (int k = 0; k < 3; k++)
            {
                double b0 = o[k] + ax[k] * xa;
                double b1 = o[k] + ax[k] * xb;
                seg[0 * 4 + 1 + k] = b0;                      // base inicio
                seg[1 * 4 + 1 + k] = b0 + ax[d + k] * va;     // desplazado inicio
                seg[2 * 4 + 1 + k] = b1 + ax[d + k] * vb;     // desplazado fin
                seg[3 * 4 + 1 + k] = b1;                      // base fin
            }
            seg[0] = seg[4] = seg[8] = seg[12] = 0d;
            drawSegment(seg, brush, g, pen, va,drawArrow:false);
        }
    }
}
 
// Valor de la acción a (0=N,1=Vy,2=Vz,3=Mx,4=My,5=Mz) a la izquierda (i) o derecha (j) de la estación
private static double barForceVal(barInternForce r, int a, bool right)
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

private void drawNodeReactions(Brush Paleta, Graphics g, Pen pen, double arrowLen = 0.5)
{
    string[] names = { "Fx", "Fy", "Fz", "Mx", "My", "Mz" };
    string forceU = Uc.activeForceU.ToString();
    string momentU = forceU + "·" + Uc.activeLenU.ToString();
    int activeLoadCase = Engine.getModel().getParamValue(paramDefaults.ActiveCase);

    var p1 = new point3D();
    var p2 = new point3D();
    var v = new double[6];
    var placed = new List<RectangleF>(); // text blocks already drawn (overlap avoidance)

    // Fixed font, same approach as drawNodes (independent of arrow length)
    Font textFont = ms.defaultFont;
    var labels = new List<(string Text, RectangleF Rect)>(); // collected labels, drawn after the loop

    foreach (var r in Engine.getModel().nodalReactions.Items)
    {
        if (r.Case != activeLoadCase) continue;

        int ni = Engine.getModel().nodes.FindIndexByID(r.NodeID);
        if (ni < 0) continue;

        var node = Engine.getModel().nodes[ni];
        if (!levelValidation(node.X, node.Y, node.Z)) continue;

        v[0] = r.Fx; v[1] = r.Fy; v[2] = r.Fz;
        v[3] = r.Mx; v[4] = r.My; v[5] = r.Mz;

        // Node position on screen (projected once)
        p1.x3d = node.X; p1.y3d = node.Y; p1.z3d = node.Z;
        p3DSet2DCoord(ref p1);

        string text = "";
        for (int k = 0; k < 6; k++)
        {
            if (v[k] == 0) continue; // free dof => zero reaction

            int axis = k % 3; // same axis mapping as drawNodeLoads
            p2.x3d = node.X + (axis == 0 ? arrowLen : 0);
            p2.y3d = node.Y + (axis == 1 ? arrowLen : 0);
            p2.z3d = node.Z + (axis == 2 ? arrowLen : 0);
            p3DSet2DCoord(ref p2);

            g.DrawLine(pen, p1.x2d, p1.y2d, p2.x2d, p2.y2d);
            if (v[k] > 0) drawArrowhead(p1, p2, 6, pen, g);
            else drawArrowhead(p2, p1, 6, pen, g);

            double val = k < 3 ? Uc.forceToUserUnits(v[k]) : Uc.momentToUserUnits(v[k]);
            text += (text.Length > 0 ? "\n" : "") + names[k] + " = " + val.ToString("0.00") + " " + (k < 3 ? forceU : momentU);
        }
        if (text.Length == 0) continue;

// Only collect the label here; drawing is decided once all nodes are processed
var size = g.MeasureString(text, textFont);
labels.Add((text, new RectangleF(p1.x2d, p1.y2d, size.Width, size.Height)));
    }
// If any two labels overlap, hide all of them
for (int i = 0; i < labels.Count; i++)
    for (int j = i + 1; j < labels.Count; j++)
        if (labels[i].Rect.IntersectsWith(labels[j].Rect)) return;

foreach (var label in labels)
    g.DrawString(label.Text, textFont, Paleta, label.Rect.X, label.Rect.Y);

}
  //Draw colored interna actions.

// Colors areas by the active internal action (per-vertex interpolation).
// RGBColor: [0..2] = color for the minimum, [3..5] = color for the maximum.
// conect: blocks of 5 -> [AreaID, idx1, idx2, idx3, idx4] (indices into point3DColection).
// Colors areas by the active internal action (per-vertex interpolation).
// RGBColor: [0..2] = color for the minimum, [3..5] = color for the maximum.
// conect: blocks of 5 -> [AreaID, idx1, idx2, idx3, idx4] (indices into point3DColection).
private void drawAreaInternActions(Color lowerColor, Color upperColor, Graphics g, Pen pen, int[] conect,
    AreaAction action = AreaAction.Fx)
{
    var forces = Engine.getModel().areaInternalForces;
    if (forces == null || forces.Count == 0) return;
    if (conect == null || conect.Length < 5) return;

    int activeCase = Engine.getModel().getParamValue(paramDefaults.ActiveCase);

    // Select the four vertex values of the requested action for the active case
    List<(int AreaID, double v1, double v2, double v3, double v4)> actionList = action switch
    {
        AreaAction.Fx => forces.Where(a => a.Case == activeCase).Select(a => (a.AreaID, a.Fx1, a.Fx2, a.Fx3, a.Fx4)).ToList(),
        AreaAction.Fy => forces.Where(a => a.Case == activeCase).Select(a => (a.AreaID, a.Fy1, a.Fy2, a.Fy3, a.Fy4)).ToList(),
        AreaAction.Fz => forces.Where(a => a.Case == activeCase).Select(a => (a.AreaID, a.Fz1, a.Fz2, a.Fz3, a.Fz4)).ToList(),
        AreaAction.Mx => forces.Where(a => a.Case == activeCase).Select(a => (a.AreaID, a.Mx1, a.Mx2, a.Mx3, a.Mx4)).ToList(),
        AreaAction.My => forces.Where(a => a.Case == activeCase).Select(a => (a.AreaID, a.My1, a.My2, a.My3, a.My4)).ToList(),
        AreaAction.Mz => forces.Where(a => a.Case == activeCase).Select(a => (a.AreaID, a.Mz1, a.Mz2, a.Mz3, a.Mz4)).ToList(),
        _ => null
    };
    if (actionList == null || actionList.Count == 0) return;
// Lookup by area ID (only to know which areas have results for the active case)
var valuesByArea = new Dictionary<int, (double v1, double v2, double v3, double v4)>(actionList.Count);
foreach (var item in actionList)
    valuesByArea[item.AreaID] = (item.v1, item.v2, item.v3, item.v4);
// Nodal averaging: a node shared by several areas gets one value (continuous colors across areas)
var nodeAccum = new Dictionary<int, (double sum, int count)>();
for (int i = 0; i + 4 < conect.Length; i += 5)
{
    if (!valuesByArea.TryGetValue(conect[i], out var vals)) continue;
    for (int k = 0; k < 4; k++)
    {
        int idx = conect[i + 1 + k];
        double val = k == 0 ? vals.v1 : k == 1 ? vals.v2 : k == 2 ? vals.v3 : vals.v4;
        nodeAccum.TryGetValue(idx, out var acc);
        nodeAccum[idx] = (acc.sum + val, acc.count + 1);
    }
}
if (nodeAccum.Count == 0) return;

// Global range over the averaged nodal values
double minValue = double.MaxValue, maxValue = double.MinValue;
foreach (var acc in nodeAccum.Values)
{
    double val = acc.sum / acc.count;
    if (val < minValue) minValue = val;
    if (val > maxValue) maxValue = val;
}
double valueRange = maxValue - minValue;


const int alpha = 160;
const int subdivisions = 8;      // grid cells per side inside each area
const float minPixelArea = 1f;   // below this the quad is seen edge-on on screen

Color ColorOf(double value)
{
    double t = valueRange < 1e-12 ? 0.5 : (value - minValue) / valueRange;
    t = Math.Clamp(t, 0d, 1d);
    return Color.FromArgb(alpha,
        (int)(lowerColor.R + (upperColor.R - lowerColor.R) * t),
        (int)(lowerColor.G + (upperColor.G - lowerColor.G) * t),
        (int)(lowerColor.B + (upperColor.B - lowerColor.B) * t));
}

double NodeValue(int pointIndex)
{
    var acc = nodeAccum[pointIndex];
    return acc.sum / acc.count;
}

var oldSmoothing = g.SmoothingMode;
var cornerX = new float[subdivisions + 1, subdivisions + 1];
var cornerY = new float[subdivisions + 1, subdivisions + 1];
var cornerV = new double[subdivisions + 1, subdivisions + 1];
var cell = new PointF[4];

for (int i = 0; i + 4 < conect.Length; i += 5)
{
    if (!nodeAccum.ContainsKey(conect[i + 1]) || !valuesByArea.ContainsKey(conect[i])) continue;

    var p1 = point3DColection[conect[i + 1]];
    var p2 = point3DColection[conect[i + 2]];
    var p3 = point3DColection[conect[i + 3]];
    var p4 = point3DColection[conect[i + 4]];

    // Level validation: all four vertices must belong to the visible level
    if (!levelValidation(p1.x3d, p1.y3d, p1.z3d) ||
        !levelValidation(p2.x3d, p2.y3d, p2.z3d) ||
        !levelValidation(p3.x3d, p3.y3d, p3.z3d) ||
        !levelValidation(p4.x3d, p4.y3d, p4.z3d)) continue;

    PointF[] quad =
    {
        new PointF(p1.x2d, p1.y2d),
        new PointF(p2.x2d, p2.y2d),
        new PointF(p3.x2d, p3.y2d),
        new PointF(p4.x2d, p4.y2d)
    };

    // Screen-space area (shoelace). Near zero => plane rotated edge-on, collapses to a line
    float screenArea = 0f;
    for (int k = 0; k < 4; k++)
    {
        var a = quad[k];
        var b = quad[(k + 1) % 4];
        screenArea += a.X * b.Y - b.X * a.Y;
    }
    screenArea = Math.Abs(screenArea) * 0.5f;

    if (screenArea < minPixelArea)
    {
        g.DrawPolygon(pen, quad);
        continue;
    }

    double n1 = NodeValue(conect[i + 1]), n2 = NodeValue(conect[i + 2]);
    double n3 = NodeValue(conect[i + 3]), n4 = NodeValue(conect[i + 4]);

    // Bilinear grid: position and value at each corner of the sub-cells
    for (int r = 0; r <= subdivisions; r++)
    {
        double v = (double)r / subdivisions;
        for (int c = 0; c <= subdivisions; c++)
        {
            double u = (double)c / subdivisions;
            double w1 = (1 - u) * (1 - v), w2 = u * (1 - v), w3 = u * v, w4 = (1 - u) * v;
            cornerX[r, c] = (float)(w1 * quad[0].X + w2 * quad[1].X + w3 * quad[2].X + w4 * quad[3].X);
            cornerY[r, c] = (float)(w1 * quad[0].Y + w2 * quad[1].Y + w3 * quad[2].Y + w4 * quad[3].Y);
            cornerV[r, c] = w1 * n1 + w2 * n2 + w3 * n3 + w4 * n4;
        }
    }

    // No antialiasing for fills, so semi-transparent cells do not show seams
    g.SmoothingMode = SmoothingMode.None;
    for (int r = 0; r < subdivisions; r++)
    {
        for (int c = 0; c < subdivisions; c++)
        {
            cell[0] = new PointF(cornerX[r, c], cornerY[r, c]);
            cell[1] = new PointF(cornerX[r, c + 1], cornerY[r, c + 1]);
            cell[2] = new PointF(cornerX[r + 1, c + 1], cornerY[r + 1, c + 1]);
            cell[3] = new PointF(cornerX[r + 1, c], cornerY[r + 1, c]);
            double cellValue = (cornerV[r, c] + cornerV[r, c + 1] + cornerV[r + 1, c + 1] + cornerV[r + 1, c]) / 4d;
            using var cellBrush = new SolidBrush(ColorOf(cellValue));
            g.FillPolygon(cellBrush, cell);
        }
    }
    g.SmoothingMode = oldSmoothing;

    g.DrawPolygon(pen, quad);
}

bool isMoment = action == AreaAction.Mx || action == AreaAction.My || action == AreaAction.Mz;
drawColorBar(g, pen, lowerColor, upperColor, minValue, maxValue, isMoment);

 
}


// Vertical reference bar on the right side of the canvas.
// Top = maxValue (upperColor), bottom = minValue (lowerColor).
private void drawColorBar(Graphics g, Pen pen, Color lowerColor, Color upperColor,
    double minValue, double maxValue, bool isMoment, int tickCount = 6)
{
    string forceU = Uc.activeForceU.ToString();
    string unit = isMoment ? forceU + "·" + Uc.activeLenU.ToString() : forceU;

    const int margin = 12;
    const int barWidth = 18;
    const int tickLength = 4;
    const int textGap = 4;

    int canvasWidth = Canvas.Width-6*margin;
    int canvasHeight = Canvas.Height;
    int barHeight = Math.Max(40, (int)(canvasHeight * 0.5));

    // Tick labels (converted to user units)
    var labels = new string[tickCount];
    float maxLabelWidth = 0f;
    for (int i = 0; i < tickCount; i++)
    {
        double t = tickCount == 1 ? 0d : (double)i / (tickCount - 1);
        double value = maxValue - (maxValue - minValue) * t; // top = max
        double userValue = isMoment ? Uc.momentToUserUnits(value) : Uc.forceToUserUnits(value);
        labels[i] = userValue.ToString("0.00");
        maxLabelWidth = Math.Max(maxLabelWidth, g.MeasureString(labels[i], ms.defaultFont).Width);
    }

    float unitWidth = g.MeasureString(unit, ms.defaultFont).Width;
    float blockWidth = barWidth + tickLength + textGap + Math.Max(maxLabelWidth, unitWidth);

    float x = canvasWidth - margin - blockWidth;
    float y = (canvasHeight - barHeight) / 2f;
    var barRect = new RectangleF(x, y, barWidth, barHeight);

    // Gradient fill (opaque)
    using (var gradientBrush = new LinearGradientBrush(barRect,
               Color.FromArgb(255, upperColor), Color.FromArgb(255, lowerColor),
               LinearGradientMode.Vertical))
    {
        g.FillRectangle(gradientBrush, barRect);
    }
    g.DrawRectangle(pen, barRect.X, barRect.Y, barRect.Width, barRect.Height);

    // Ticks and labels to the right of the bar
    using (var textBrush = new SolidBrush(pen.Color))
    {
        float textHeight = g.MeasureString("0", ms.defaultFont).Height;
        for (int i = 0; i < tickCount; i++)
        {
            float ty = y + (tickCount == 1 ? 0f : barHeight * i / (float)(tickCount - 1));
            float tx = x + barWidth;
            g.DrawLine(pen, tx, ty, tx + tickLength, ty);
            g.DrawString(labels[i], ms.defaultFont, textBrush,
                tx + tickLength + textGap, ty - textHeight / 2f);
        }

        // Unit title above the bar
        g.DrawString(unit, ms.defaultFont, textBrush, x, y - g.MeasureString(unit, ms.defaultFont).Height - 4);
    }
}





  }



