using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;
namespace CEF;

internal class UISettings()
{
    private const string APPDATAFILE = "cef.config";
    

    // ───────────── General options ─────────────
    internal string lang { get; set; }
    public bool applyToAllWindows { get; set; }
    public bool applyToNewWindows { get; set; }
    public int sectionsPerElement { get; set; }
    public bool showNodesID { get; set; }
    public bool showGrid { get; set; }
    public bool showBarsLoads { get; set; }
    public bool showBarsDiagrams { get; set; }
    public bool showAreasDiagrams { get; set; }
    public bool showBarsAxes { get; set; }
    public bool showAreasAxes{ get; set; }
    public bool showDeformedShape { get; set; }
    public bool showNodalLoads { get; set; }
    public bool showNodalReactions { get; set; }
    public bool showNodes { get; set; }
    public bool showAreas { get; set; }//
    public bool showSelectedNodes { get; set; }
    public bool showNodesConstraints { get; set; }
    public bool showBarReleases { get; set; }
    internal int viewRenderMode { get; set; }
    public double deformScale { get; set; }
    public BarActions barAction { get; set; }
    public AreaAction areaAction { get; set; }
    public double rotScale { get; set; }

    // ───────────── Theme (color) options ─────────────
    public Color canvColor { get; set; }
    public Color nodeBrushColor { get; set; }
    public Color barLineSColor { get; set; }
    public Color barDiagramsSColor { get; set; }
    public Color areaColor { get; set; }
    public Color areaActionUpperColor { get; set; }
    public Color areaActionLowerColor { get; set; }

    public float barLineSSize { get; set; }
    public Color barColor { get; set; }
    public float barLineW { get; set; }
    public Color barSelectedColor { get; set; }
    public Color areaSelectedColor { get; set; }
    public Color loadColorBrush { get; set; }
    public Color loadPenColor { get; set; }
    public Color reactionPenColor { get; set; }
    public Color deformedShapeColor { get; set; }
    public Color SuppColorBrush { get; set; }
    public Font defaultFont { get; set; }
    public Color gridLinesColor { get; set; }
    public float gridLineW { get; set; }
    public DashStyle GridLineStyle { get; set; }

    public enum RenderMode
    {
        Normal,
        Extruded,
        WireFrame,
    }
    public enum CanvasTheme
    {
        Tecnic,
        BluePrint,
        NotePad,
    }

    public enum SuppLangs {en, es,ca,de,eu,fr,it,ja,pt,ru,zh }

    private float fontScale;
    private string themeName = nameof(CanvasTheme.Tecnic);

    internal UISettings(float FontScale, int dfc) : this()
    {
        Theme_ = (CanvasTheme)dfc;
        themeName = Theme_.ToString();
        lang = ""; //""=eng
        fontScale = FontScale;
        sectionsPerElement = 3;
        showNodesID = false;
        showGrid = true;
        showBarsLoads = true;
        showAreas=false;
        showNodalReactions = false;
        showDeformedShape = false;
        showBarsDiagrams = false;
        showAreasDiagrams = false;
        showBarsAxes = false;
        showAreasAxes = false;
        showNodalLoads = true;
        showNodesConstraints = true;
        showBarReleases = true;
        showNodes = false;
        showSelectedNodes = true;
        applyToAllWindows = false;
        applyToNewWindows = true;
        barAction = BarActions.Vz;
        areaAction = AreaAction.Fz;
        deformScale = 100.0;
        rotScale = 10.0;
        applyTheme();
    }

    // ───────────── Themes registry ─────────────
    // Name -> raw key/value pairs. Filled with the built-in themes and then with
    // every [Theme:xxx] section found in the config file (no limit on count).
    private static readonly Dictionary<string, Dictionary<string, string>> Themes = new(StringComparer.OrdinalIgnoreCase);

    static UISettings() => EnsureBuiltInThemes();

    /// <summary>Names of all known themes (built-in + user defined).</summary>
    internal static IReadOnlyCollection<string> ThemeNames => Themes.Keys;

    private static void EnsureBuiltInThemes()
    {
        foreach (var t in Enum.GetValues<CanvasTheme>())
        {
            if (Themes.ContainsKey(t.ToString())) continue;
            var tmp = new UISettings { fontScale = 3 };
            tmp.LoadBuiltIn(t);
            Themes[t.ToString()] = tmp.CaptureTheme();
        }
    }

    /// <summary>Applies the active theme: built-in baseline first, then stored values on top.</summary>
    internal void applyTheme()
    {
        if (fontScale < 1) fontScale = 3;
        LoadBuiltIn(Theme_); // baseline, so missing keys in a theme never leave empty values
        if (Themes.TryGetValue(themeName, out var values)) ApplyThemeValues(values);
    }

    /// <summary>Hard-coded defaults for the built-in themes.</summary>
    private void LoadBuiltIn(CanvasTheme theme)
    {
        switch (theme)
        {
            case CanvasTheme.BluePrint:
                canvColor = Color.FromArgb(15, 35, 65);
                gridLinesColor = Color.FromArgb(90, 140, 190);
                gridLineW = 0.7f;
                GridLineStyle = DashStyle.Dot;
                nodeBrushColor = Color.FromArgb(235, 240, 255);
                barColor = Color.FromArgb(200, 215, 40);
                areaColor = Color.FromArgb(200, 215, 60);
                areaActionUpperColor =Color.FromArgb(235,50,50); 
                areaActionLowerColor =Color.FromArgb(0, 50, 245); 
                barLineW = 1.25f;
                barLineSColor = Color.FromArgb(150, 200, 255);
                barLineSSize = 1.0f;
                barSelectedColor = Color.FromArgb(255, 225, 120);
                areaSelectedColor = Color.FromArgb(255, 225, 140);
                deformedShapeColor = Color.FromArgb(240, 160, 110);
                barDiagramsSColor = Color.FromArgb(240, 160, 110);
                SuppColorBrush = Color.FromArgb(130, 185, 225);
                loadPenColor = Color.FromArgb(145, 170, 200);
                loadColorBrush = Color.FromArgb(130, 185, 225);                
                reactionPenColor=Color.FromArgb(240, 160, 110);
                sectionsPerElement = 3;
                defaultFont = new("Consolas", fontScale);
                break;
            case CanvasTheme.Tecnic:
                canvColor = Color.FromArgb(238, 238, 235);
                gridLinesColor = Color.FromArgb(200, 200, 200);
                gridLineW = 0.6f;
                GridLineStyle = DashStyle.Dot;
                nodeBrushColor = Color.FromArgb(45, 45, 45);
                barColor = Color.FromArgb(30, 30, 30);
                areaColor = Color.FromArgb(30, 30, 30);
                areaActionUpperColor =Color.FromArgb(235,50,50); 
                areaActionLowerColor =Color.FromArgb(0, 50, 245); 
                barLineW = 1.4f;
                barLineSColor = Color.FromArgb(120, 120, 120);
                barLineSSize = 1.1f;
                barSelectedColor = Color.FromArgb(70, 120, 200);
                areaSelectedColor = Color.FromArgb(70, 120, 200);
                deformedShapeColor = Color.FromArgb(180, 120, 80);
                barDiagramsSColor = Color.FromArgb(190, 80, 60);
                SuppColorBrush = Color.FromArgb(90, 110, 130);
                loadPenColor = Color.FromArgb(100, 130, 160);
                loadColorBrush = Color.FromArgb(100, 130, 160);
                sectionsPerElement = 3;
                reactionPenColor=Color.FromArgb(240, 160, 110);
                defaultFont = new("Segoe UI", fontScale);
                break;
            case CanvasTheme.NotePad:
                canvColor = Color.FromArgb(250, 245, 220);
                gridLinesColor = Color.FromArgb(180, 180, 170);
                gridLineW = 0.6f;
                GridLineStyle = DashStyle.Dot;
                nodeBrushColor = Color.FromArgb(70, 70, 70);
                barColor = Color.FromArgb(80, 80, 80);
                areaColor = Color.FromArgb(80, 80, 80);
                areaActionUpperColor =Color.FromArgb(235,50,50); 
                areaActionLowerColor =Color.FromArgb(0, 50, 245); 
                barLineW = 1.4f;
                barLineSColor = Color.FromArgb(140, 140, 140);
                barLineSSize = 1.2f;
                barSelectedColor = Color.FromArgb(210, 70, 70);
                areaSelectedColor = Color.FromArgb(210, 70, 70);
                deformedShapeColor = Color.FromArgb(180, 110, 80);
                barDiagramsSColor = Color.FromArgb(60, 120, 150);
                SuppColorBrush = Color.FromArgb(90, 130, 90);
                loadPenColor = Color.FromArgb(70, 110, 140);
                loadColorBrush = Color.FromArgb(70, 110, 140);
                sectionsPerElement = 3;
                reactionPenColor=Color.FromArgb(240, 160, 110);
                defaultFont = new("Arial", fontScale);
                break;
        }
    }

    private Dictionary<string, string> CaptureTheme()
    {
        var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in ThemeFields) d[f.Key] = f.Get(this);
        return d;
    }

    private void ApplyThemeValues(Dictionary<string, string> values)
    {
        foreach (var f in ThemeFields)
            if (values.TryGetValue(f.Key, out var v)) f.Set(this, v);
    }

    /// <summary>Stores the current colors as a new (or overwritten) theme, selects it and saves.</summary>
    internal bool SaveAsTheme(string name)
    {
        name = name?.Trim() ?? "";
        if (name.Length == 0 || name.IndexOfAny(['[', ']', '\r', '\n']) >= 0) return false;
        Themes[name] = CaptureTheme();
        themeName = name;
        return Save();
    }

    // ───────────── Persistence ─────────────
    // Plain text, INI-like:
    //   [General]
    //   key=value
    //   [Theme:Name]
    //   key=value        (colors as R,G,B or R,G,B,A)
    private static string ConfigPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CEF", APPDATAFILE);

    private sealed record Field(string Key, Func<UISettings, string> Get, Action<UISettings, string> Set);

    private static Field S(string k, Func<UISettings, string> g, Action<UISettings, string> s) => new(k, g, s);
    private static Field B(string k, Func<UISettings, bool> g, Action<UISettings, bool> s) =>
        new(k, o => g(o).ToString(), (o, v) => { if (bool.TryParse(v, out var x)) s(o, x); });
    private static Field I(string k, Func<UISettings, int> g, Action<UISettings, int> s) =>
        new(k, o => g(o).ToString(), (o, v) => { if (int.TryParse(v, out var x)) s(o, x); });
    private static Field F(string k, Func<UISettings, float> g, Action<UISettings, float> s) =>
        new(k, o => g(o).ToString("0.###"), (o, v) => { if (float.TryParse(v, out var x)) s(o, x); });
    private static Field D(string k, Func<UISettings, double> g, Action<UISettings, double> s) =>
        new(k, o => g(o).ToString("0.###"), (o, v) => { if (double.TryParse(v, out var x)) s(o, x); });
    private static Field E<T>(string k, Func<UISettings, T> g, Action<UISettings, T> s) where T : struct, Enum =>
        new(k, o => g(o).ToString(), (o, v) => { if (Enum.TryParse<T>(v, true, out var x) && Enum.IsDefined(x)) s(o, x); });
    private static Field C(string k, Func<UISettings, Color> g, Action<UISettings, Color> s) =>
        new(k, o => FormatColor(g(o)), (o, v) => { if (TryParseColor(v, out var x)) s(o, x); });

    private static string FormatColor(Color c) => c.A == 255 ? $"{c.R},{c.G},{c.B}" : $"{c.R},{c.G},{c.B},{c.A}";

    private static bool TryParseColor(string text, out Color color)
    {
        color = default;
        var p = text.Split(',', StringSplitOptions.TrimEntries);
        if (p.Length is not (3 or 4)) return false;
        var n = new byte[4] { 0, 0, 0, 255 };
        for (int i = 0; i < p.Length; i++)
            if (!byte.TryParse(p[i], out n[i])) return false;
        color = Color.FromArgb(n[3], n[0], n[1], n[2]);
        return true;
    }

    private static readonly Field[] GeneralFields =
    [
        S("lang", o => o.lang, (o, v) => o.lang = v),
        S("theme", o => o.themeName, (o, v) =>
        {
            if (!Themes.ContainsKey(v)) return;
            o.themeName = v;
            if (Enum.TryParse<CanvasTheme>(v, true, out var t)) o.Theme_ = t;
        }),
        B("applyToAllWindows", o => o.applyToAllWindows, (o, v) => o.applyToAllWindows = v),
        B("applyToNewWindows", o => o.applyToNewWindows, (o, v) => o.applyToNewWindows = v),
        B("showNodesID", o => o.showNodesID, (o, v) => o.showNodesID = v),
        B("showGrid", o => o.showGrid, (o, v) => o.showGrid = v),
        B("showBarsLoads", o => o.showBarsLoads, (o, v) => o.showBarsLoads = v),
        B("showBarsDiagrams", o => o.showBarsDiagrams, (o, v) => o.showBarsDiagrams = v),
        B("showAreasDiagrams", o => o.showAreasDiagrams, (o, v) => o.showAreasDiagrams = v),
        B("showBarsAxes", o => o.showBarsAxes, (o, v) => o.showBarsAxes = v),
        B("showAreasAxes", o => o.showAreasAxes, (o, v) => o.showAreasAxes = v),
        B("showDeformedShape", o => o.showDeformedShape, (o, v) => o.showDeformedShape = v),
        B("showNodalLoads", o => o.showNodalLoads, (o, v) => o.showNodalLoads = v),
        B("showNodes", o => o.showNodes, (o, v) => o.showNodes = v),
        B("showSelectedNodes", o => o.showSelectedNodes, (o, v) => o.showSelectedNodes = v),
        B("showNodesConstraints", o => o.showNodesConstraints, (o, v) => o.showNodesConstraints = v),
        B("showBarReleases", o => o.showBarReleases, (o, v) => o.showBarReleases = v),
        I("renderMode", o => o.viewRenderMode, (o, v) => o.viewRenderMode = v),
        D("deformScale", o => o.deformScale, (o, v) => o.deformScale = v),
        D("rotScale", o => o.rotScale, (o, v) => o.rotScale = v),
        B("showNodalReactions",o=>o.showNodalReactions,(o,v)=>o.showNodalReactions = v),
        B("showAreas",o=>o.showAreas,(o,v)=>o.showAreas = v),
        E<BarActions>("barAction", o => o.barAction, (o, v) => o.barAction = v),
        E<AreaAction>("areaAction", o => o.areaAction, (o, v) => o.areaAction = v),
    ];

    private static readonly Field[] ThemeFields =
    [
        C("canvColor", o => o.canvColor, (o, v) => o.canvColor = v),
        C("gridLinesColor", o => o.gridLinesColor, (o, v) => o.gridLinesColor = v),
        F("gridLineW", o => o.gridLineW, (o, v) => o.gridLineW = v),
        E<DashStyle>("gridLineStyle", o => o.GridLineStyle, (o, v) => o.GridLineStyle = v),
        C("nodeBrushColor", o => o.nodeBrushColor, (o, v) => o.nodeBrushColor = v),
        C("barColor", o => o.barColor, (o, v) => o.barColor = v),
        C("areaColor", o => o.areaColor, (o, v) => o.areaColor = v),
        C("areaActionLowerColor", o => o.areaActionLowerColor, (o, v) => o.areaActionLowerColor = v),
        C("areaActionUpperColor", o => o.areaActionUpperColor, (o, v) => o.areaActionUpperColor = v),
        F("barLineW", o => o.barLineW, (o, v) => o.barLineW = v),
        C("barLineSColor", o => o.barLineSColor, (o, v) => o.barLineSColor = v),
        F("barLineSSize", o => o.barLineSSize, (o, v) => o.barLineSSize = v),
        C("barSelectedColor", o => o.barSelectedColor, (o, v) => o.barSelectedColor = v),
        C("areaSelectedColor", o => o.areaSelectedColor, (o, v) => o.areaSelectedColor = v),
        C("deformedShapeColor", o => o.deformedShapeColor, (o, v) => o.deformedShapeColor = v),
        C("barDiagramsSColor", o => o.barDiagramsSColor, (o, v) => o.barDiagramsSColor = v),
        C("suppColorBrush", o => o.SuppColorBrush, (o, v) => o.SuppColorBrush = v),
        C("loadPenColor", o => o.loadPenColor, (o, v) => o.loadPenColor = v),
        C("loadColorBrush", o => o.loadColorBrush, (o, v) => o.loadColorBrush = v),
        C("reactionPenColor", o => o.reactionPenColor, (o, v) => o.reactionPenColor = v),
        I("sectionsPerElement", o => o.sectionsPerElement, (o, v) => o.sectionsPerElement = v),
        S("font", o => o.defaultFont?.Name ?? "", (o, v) =>
        {
            if (!string.IsNullOrWhiteSpace(v)) o.defaultFont = new Font(v, o.fontScale);
        }),
    ];

    /// <summary>Creates the settings and overlays the config file, if any. Never throws.</summary>
    
    internal static bool initialiced;
    internal static UISettings Load(float fontScale, int defaultTheme)
    {
        var s = new UISettings(fontScale, defaultTheme);
        try
        {
            if (!File.Exists(ConfigPath)) return s;

            var general = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var themes = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            ParseFile(ConfigPath, general, themes);

            // Themes found in the file are added (or override the built-in ones).
            foreach (var t in themes) Themes[t.Key] = t.Value;

            foreach (var f in GeneralFields)
                if (general.TryGetValue(f.Key, out var v)) f.Set(s, v);

            s.applyTheme();
           
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
        return s;
    }

    private static void ParseFile(string path, Dictionary<string, string> general,
        Dictionary<string, Dictionary<string, string>> themes)
    {
        Dictionary<string, string> current = null;
        foreach (var raw in File.ReadLines(path, Encoding.UTF8))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] is '#' or ';') continue;

            if (line[0] == '[' && line[^1] == ']')
            {
                var section = line[1..^1].Trim();
                if (section.Equals("General", StringComparison.OrdinalIgnoreCase)) current = general;
                else if (section.StartsWith("Theme:", StringComparison.OrdinalIgnoreCase) && section.Length > 6)
                {
                    var name = section[6..].Trim();
                    if (!themes.TryGetValue(name, out current))
                        themes[name] = current = new(StringComparer.OrdinalIgnoreCase);
                }
                else current = null;
                continue;
            }

            int eq = line.IndexOf('=');
            if (eq <= 0 || current is null) continue;
            current[line[..eq].Trim()] = line[(eq + 1)..].Trim();
        }
    }

    /// <summary>Saves general options and all themes. The active theme keeps its current (edited) colors.</summary>
    internal bool Save()
    {
        try
        {
            Themes[themeName] = CaptureTheme();

            var sb = new StringBuilder();
            sb.AppendLine("# CEF configuration. Colors are R,G,B or R,G,B,A.");
            sb.AppendLine("[General]");
            foreach (var f in GeneralFields) sb.Append(f.Key).Append('=').AppendLine(f.Get(this));

            foreach (var t in Themes)
            {
                sb.AppendLine();
                sb.Append("[Theme:").Append(t.Key).AppendLine("]");
                foreach (var kv in t.Value) sb.Append(kv.Key).Append('=').AppendLine(kv.Value);
            }

            var path = ConfigPath;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var tmp = path + ".tmp"; // write + move, so a crash never leaves a half-written file
            File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
            File.Move(tmp, path, true);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    internal UISettings Clone()
    {
        var c = new UISettings();
        c.Theme_ = Theme_;
        c.themeName = themeName;
        c.lang = lang;
        c.canvColor = canvColor;
        c.gridLinesColor = gridLinesColor;
        c.gridLineW = gridLineW;
        c.GridLineStyle = GridLineStyle;
        c.nodeBrushColor = nodeBrushColor;
        c.barColor = barColor;
        c.barLineW = barLineW;
        c.barLineSColor = barLineSColor;
        c.barLineSSize = barLineSSize;
        c.barSelectedColor = barSelectedColor;
        c.deformedShapeColor = deformedShapeColor;
        c.barDiagramsSColor = barDiagramsSColor;
        c.SuppColorBrush = SuppColorBrush;
        c.loadPenColor = loadPenColor;
        c.loadColorBrush = loadColorBrush;
        c.sectionsPerElement = sectionsPerElement;
        c.fontScale = fontScale;
        c.defaultFont = (Font)defaultFont.Clone();
        c.showNodesID = showNodesID;
        c.showGrid = showGrid;
        c.showBarsLoads = showBarsLoads;
        c.showDeformedShape = showDeformedShape;
        c.showNodalLoads = showNodalLoads;
        c.showNodes = showNodes;
        c.showSelectedNodes = showSelectedNodes;
        c.showBarReleases = showBarReleases;
        c.showNodesConstraints = showNodesConstraints;
        c.applyToAllWindows = applyToAllWindows;
        c.applyToNewWindows = applyToNewWindows;
        c.deformScale = deformScale;
        c.rotScale = rotScale;
        c.barAction = barAction;
        c.areaAction = areaAction;
        c.showBarsDiagrams = showBarsDiagrams;
        c.showAreasDiagrams = showAreasDiagrams;
        c.showBarsAxes = showBarsAxes;
        c.showAreasAxes = showAreasAxes;
        c.viewRenderMode = viewRenderMode;
        c.showNodalReactions = showNodalReactions;
        c.reactionPenColor = reactionPenColor;
        c.showAreas = showAreas;
        c.areaActionLowerColor = areaActionLowerColor;
        c.areaColor = areaColor;
        c.areaActionUpperColor = areaActionUpperColor;

        c.areaSelectedColor = areaSelectedColor;
        return c;
    }

    #region PropertyGrid helpers
    // Everything here exists only to make the PropertyGrid friendly.

    public RenderMode ViewRendeMode { get => (RenderMode)viewRenderMode; set { viewRenderMode = (int)value; } }
    public  SuppLangs Langs{  get => Enum.TryParse(lang, out SuppLangs result) ? result : default; set => lang = value.ToString();}

    /// <summary>Built-in theme enum. Hidden in the grid; ThemeName (dropdown) replaces it.</summary>
    [Browsable(false)]
    public CanvasTheme Theme { get => Theme_; set { Theme_ = value; themeName = value.ToString(); applyTheme(); } }
    internal CanvasTheme Theme_ { get; set; }

    /// <summary>Dropdown with every known theme (built-in + from file).</summary>
    [TypeConverter(typeof(ThemeNameConverter))]
    public string ThemeName
    {
        get => themeName;
        set { if (Themes.ContainsKey(value)) { themeName = value; applyTheme(); } }
    }

    internal sealed class ThemeNameConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) =>
            new(ThemeNames.ToArray());
    }
    #endregion
}