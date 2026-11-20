using System.Reflection;

namespace CEF;

public static class parseMetadata
{
    private enum UnitKind { None, Force, Moment, Press, Len, Area, Len3, Len4,Mass }

    // Fields that identify a row. They are never treated as an "action" when testing for all-zero rows.
    static readonly HashSet<string> idFields = new HashSet<string> { "ID", "Case", "BarID", "NodeID", "ST", "Pos" };

    // Tables whose "Type" column is stored as int but shown as the enum name.
    static readonly (Type table, string[] names)[] enumTables = new (Type, string[])[]
    {
        (typeof(loadCase), Enum.GetNames<LoadCaseType>()),
        (typeof(Section), Enum.GetNames<sectType>()),
        (typeof(Material), Enum.GetNames<materialType>()),
        (typeof(nodalConstraint), Enum.GetNames<MFCType>()),
    };

    static readonly Dictionary<Type, FieldInfo[]> fieldCache = new();

    #region Fields and enums

    public static FieldInfo[] getFields(Type t)
    {
        if (!fieldCache.TryGetValue(t, out FieldInfo[] f))
        {
            f = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            fieldCache[t] = f;
        }
        return f;
    }

    public static string[] enumNamesOf(Type rowType, string fieldName)
    {
        if (fieldName != "Type") return null;
        for (int i = 0; i < enumTables.Length; i++)
            if (enumTables[i].table == rowType) return enumTables[i].names;
        return null;
    }

    public static bool isEnumField(Type rowType, FieldInfo f)
    {
        return enumNamesOf(rowType, f.Name) != null;
    }

    public static bool isReadOnly(Type rowType, FieldInfo f)
    {
        // TODO: text -> enum conversion to allow editing enum columns
        return f.Name == "ID" || f.IsInitOnly || f.IsLiteral || isEnumField(rowType, f);
    }

    public static bool getEnumFieldDisplayValue(Type tableType, string[] enumNames, string colName, FieldInfo row, object rowObject, ref object valor)
    {
        if (rowObject.GetType() != tableType || row.Name != colName) return false;
        if (row.GetValue(rowObject) is not int k || k < 0 || k >= enumNames.Length) return false;
        valor = enumNames[k];
        return true;
    }

    #endregion

    #region Cell formatting and parsing

    // Value ready to display: enum name or user units.
    public static object formatCell(object row, FieldInfo f)
    {
        object v = f.GetValue(row);
        if (v == null) return null;
        string[] names = enumNamesOf(row.GetType(), f.Name);
        if (names != null)
        {
            if (v is int k && k >= 0 && k < names.Length) return names[k];
            return v;
        }
        useUserUnits(ref v, f);
        return v;
    }

    // Parses text typed by the user (user units) and stores it in the field. False if rejected.
    public static bool tryParseCell(object row, FieldInfo f, string text)
    {
        if (string.IsNullOrWhiteSpace(text) || isReadOnly(row.GetType(), f)) return false;
        try
        {
            object v = text.Trim();
            useDefautlDBUnits(ref v, f);
            f.SetValue(row, Convert.ChangeType(v, f.FieldType));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // True when every numeric non-identifier field of the row is exactly 0.
    public static bool isZeroRow(object row, FieldInfo[] fields)
    {
        for (int i = 0; i < fields.Length; i++)
        {
            if (idFields.Contains(fields[i].Name)) continue;
            object v = fields[i].GetValue(row);
            if (v is double d) { if (d != 0) return false; }
            else if (v is int n) { if (n != 0) return false; }
            else if (v is float s) { if (s != 0) return false; }
            else if (v is long l) { if (l != 0) return false; }
        }
        return true;
    }

    #endregion

    #region Display names

    public static string tableText(string name)
    {
        return I18n.translateTableNames.TryGetValue(name, out string t) ? t : name;
    }

    // Translated field name plus unit, e.g. "Fx [kN]".
    public static string headerText(FieldInfo f)
    {
        string name = tableText(f.Name);
        string unit = unitLabel(f.Name);
        return unit == "" ? name : $"{name} [{unit}]";
    }

    static string unitLabel(string name)
    {
        // UNITS: label text comes from the active unit enums. If ToString() is not "kN"/"m", change only these two lines.
        // Pressure label is approximated as force/length^2.
        string fu = Uc.activeForceU.ToString();
        string lu = Uc.activeLenU.ToString();
        string mu = Uc.activeMassU.ToString();
        switch (kindOf(name))
        {
            case UnitKind.Force: return fu;
            case UnitKind.Moment: return fu + "·" + lu;
            case UnitKind.Press: return fu + "/" + lu + "^2";
            case UnitKind.Len: return lu;
            case UnitKind.Area: return lu + "^2";
            case UnitKind.Len3: return lu + "^3";
            case UnitKind.Len4: return lu + "^4";
            case UnitKind.Mass: return mu;
            
            default: return "";
        }
    }

    #endregion

    #region Units

    static UnitKind kindOf(string name)
    {
        if (fieldTypes.forceFields.Contains(name)) return UnitKind.Force;
        if (fieldTypes.momentFields.Contains(name)) return UnitKind.Moment;
        if (fieldTypes.pressFields.Contains(name)) return UnitKind.Press;
        if (fieldTypes.lenFields.Contains(name)) return UnitKind.Len;
        if (fieldTypes.areaFields.Contains(name)) return UnitKind.Area;
        if (fieldTypes.len3Fields.Contains(name)) return UnitKind.Len3;
        if (fieldTypes.len4Fields.Contains(name)) return UnitKind.Len4;
        if (fieldTypes.massFields.Contains(name)) return UnitKind.Mass;
        return UnitKind.None;
    }

    public static bool useUserUnits(ref object valor, FieldInfo dataCellObj)
    {
        if (valor == null) return false;
        string vc = valor.ToString();
        switch (kindOf(dataCellObj.Name))
        {
            case UnitKind.Force: valor = Uc.forceToUserUnits(vc); return true;
            case UnitKind.Moment: valor = Uc.momentToUserUnits(vc); return true;
            case UnitKind.Press: valor = Uc.pressToUserUnits(vc); return true;
            case UnitKind.Len: valor = Uc.lenToUserUnits(vc); return true;
            case UnitKind.Area: valor = Uc.areaToUserUnits(vc); return true;
            case UnitKind.Len3: valor = Uc.len3ToUserUnits(vc); return true;
            case UnitKind.Len4: valor = Uc.len4ToUserUnits(vc); return true;
            case UnitKind.Mass: valor = Uc.massToUserUnits(vc); return true;
            default: return false;
        }
    }

    public static bool useDefautlDBUnits(ref object valor, FieldInfo dataCellObj)
    {
        if (valor == null) return false;
        string vc = valor.ToString();
        switch (kindOf(dataCellObj.Name))
        {
            case UnitKind.Force: valor = Uc.forceFromUserUnits(vc); return true;
            case UnitKind.Moment: valor = Uc.momentFromUserUnits(vc); return true;
            case UnitKind.Press: valor = Uc.pressFromUserUnits(vc); return true;
            case UnitKind.Len: valor = Uc.lenFromUserUnits(vc); return true;
            case UnitKind.Area: valor = Uc.areaFromUserUnits(vc); return true;
            case UnitKind.Len3: valor = Uc.len3FromUserUnits(vc); return true;
            case UnitKind.Len4: valor = Uc.len4FromUserUnits(vc); return true;
            case UnitKind.Mass: valor = Uc.massFromUserUnits(vc); return true;
            default: return false;
        }
    }

    #endregion

    public static class fieldTypes // metadata, used only in tree and table views
    {
        public static readonly IReadOnlySet<string> forceFields = new HashSet<string>() { "Fx", "Fy", "Fz", "Fxi", "Fyi", "Fzi", "Fxj", "Fyj", "Fzj", "W",
                                                                                          "Fx1", "Fy1", "Fz1","Fx2", "Fy2", "Fz2","Fx3", "Fy3", "Fz3",
                                                                                          "Fx4", "Fy4", "Fz4","Fx5", "Fy5", "Fz5","Fx6", "Fy6", "Fz6" };
        public static readonly IReadOnlySet<string> momentFields = new HashSet<string>() { "Mx", "My", "Mz", "Mxi", "Myi", "Mzi", "Mxj", "Myj", "Mzj",
                                                                                          "Mx1", "My1", "Mz1","Mx2", "My2", "Mz2","Mx3", "My3", "Mz3",
                                                                                          "Mx4", "My4", "Mz4","Mx5", "My5", "Mz5","Mx6", "My6", "Mz6"
                                                                                             };
        public static readonly IReadOnlySet<string> lenFields = new HashSet<string>() { "a","b", "dx", "dy", "dz", "Pos", "h", "tw", "tf", "ryy", "rzz", "X", "Y", "Z", 
                                                                                        "offsetY", "offsetZ", "Coord" ,"Thickness"};
        public static readonly IReadOnlySet<string> areaFields = new HashSet<string>() { "Ag", "Acyy", "Aczz" };
        public static readonly IReadOnlySet<string> pressFields = new HashSet<string>() { "E", "G", "fc", "fy", "fu" };
        public static readonly IReadOnlySet<string> len3Fields = new HashSet<string>() { "Szz", "Syy" };
        public static readonly IReadOnlySet<string> massFields = new HashSet<string>() { "M" };
        public static readonly IReadOnlySet<string> len4Fields = new HashSet<string>() { "Iyy", "Izz", "Iyz", "Tor", "Pyy", "Pzz" };
        public static readonly IReadOnlySet<string> themperatureFields = new HashSet<string>() { "extTheperature", "intThemperature" };
    }
}