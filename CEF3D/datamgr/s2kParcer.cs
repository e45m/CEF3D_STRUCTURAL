using System.Text.RegularExpressions;
namespace CEF.datamgr
{
    internal class s2kparser
    {
       private  ModelDB modelDB;
        private string filePath;
        private Dictionary<int,string> sectMaterial;
        private Dictionary<int,string> loadPatterns;
        public s2kparser(ModelDB db, string file)
        {
            modelDB = db;
            filePath = file;
            preLine = "";
        }
        string tables;
        private  void captureTables()
        {
            Dictionary<string, string> tableDic = [];            
            tableDic.Add("PROGRAM CONTROL", "");
            tableDic.Add("CASE - MODAL 1 - GENERAL", "");
            tableDic.Add("CASE - STATIC 1 - LOAD ASSIGNMENTS", "");
            tableDic.Add("LOAD PATTERN DEFINITIONS", "");
            tableDic.Add("COMBINATION DEFINITIONS", "");
            tableDic.Add("CONNECTIVITY - FRAME", "");
            tableDic.Add("FRAME SECTION ASSIGNMENTS", "");
            tableDic.Add("FRAME LOADS - DISTRIBUTED", "");
            tableDic.Add("FRAME SECTION PROPERTIES 01 - GENERAL", "");
            tableDic.Add("JOINT COORDINATES", "");
            tableDic.Add("JOINT RESTRAINT ASSIGNMENTS", "");
            tableDic.Add("GRID LINES", "");
            tableDic.Add("LOAD CASE DEFINITIONS", "");
            tableDic.Add("MATERIAL PROPERTIES 01 - GENERAL", "");
            tableDic.Add("MATERIAL PROPERTIES 02 - BASIC MECHANICAL PROPERTIES", "");
            tableDic.Add("MATERIAL PROPERTIES 03A - STEEL DATA", "");
            tableDic.Add("MATERIAL PROPERTIES 03B - CONCRETE DATA", "");
            tableDic.Add("CONSTRAINT DEFINITIONS - DIAPHRAGM", "");
            tableDic.Add("JOINT CONSTRAINT ASSIGNMENTS", "");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Archivo no encontrado", filePath);
            string[] textLines = File.ReadAllLines(filePath);
            var sap2kTable = "";
            foreach (string line in textLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.Contains("TABLE:"))
                {
                    sap2kTable = Regex.Match(line, @"TABLE:\s*""([^""]+)""").Groups[1].Value;
                    if (tableDic.Keys.Contains(sap2kTable))
                        tableDic[sap2kTable] += $"{line}\n";
                    continue;
                }
                if (tableDic.Keys.Contains(sap2kTable))
                    tableDic[sap2kTable] += $"{line}\n";
            }
            tables = "";
            tables += tableDic["PROGRAM CONTROL"];
            tables += tableDic["GRID LINES"];
            tables += tableDic["JOINT COORDINATES"];
            tables += tableDic["JOINT RESTRAINT ASSIGNMENTS"];
            tables += tableDic["CONSTRAINT DEFINITIONS - DIAPHRAGM"]; 
            tables += tableDic["JOINT CONSTRAINT ASSIGNMENTS"]; 
            tables += tableDic["MATERIAL PROPERTIES 01 - GENERAL"];
            tables += tableDic["MATERIAL PROPERTIES 02 - BASIC MECHANICAL PROPERTIES"];
            tables += tableDic["MATERIAL PROPERTIES 03A - STEEL DATA"];
            tables += tableDic["MATERIAL PROPERTIES 03B - CONCRETE DATA"];
            tables += tableDic["FRAME SECTION ASSIGNMENTS"];
            tables += tableDic["FRAME SECTION PROPERTIES 01 - GENERAL"];
            tables += tableDic["CONNECTIVITY - FRAME"];
            tables += tableDic["CASE - MODAL 1 - GENERAL"];
            tables += tableDic["CASE - STATIC 1 - LOAD ASSIGNMENTS"];
            tables += tableDic["LOAD PATTERN DEFINITIONS"];
            tables += tableDic["LOAD CASE DEFINITIONS"];
            tables += tableDic["COMBINATION DEFINITIONS"];
            tables += tableDic["FRAME LOADS - DISTRIBUTED"];
        }
        public void ImprtS2kFile_ordeningTables()
        {
            captureTables();
            int indx = 0;
            bool fl = true;
            int nf = 0;
            modelDB.InitThisClassProperies();
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Archivo no encontrado", filePath);
            string[] lines = tables.Split("\n").ToArray();
            parseLines(lines);
        }
        public void ImprtS2kFileScuencial()
        {
            modelDB.InitThisClassProperies();
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found!!", filePath);
            string[] lines = File.ReadAllLines(filePath);
            parseLines(lines);
        }
        void parseLines(string[] lines)
        {
            sectMaterial = [];
            loadPatterns = [];
            sectMaterial.Add(-1, "None");
            loadPatterns.Add(-1, "None");
            var Table = "";
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.Contains("TABLE:"))
                {
                    Table = Regex.Match(line, @"TABLE:\s*""([^""]+)""").Groups[1].Value;
                    continue;
                }
                switch (Table)
                {
                    case "PROGRAM CONTROL":
                        parse_Table_PROGRAM_CONTROL(line);
                        break;
                    case "CASE - MODAL 1 - GENERAL":
                        parse_Table_CASE_MODAL_1_GENERAL(line);
                        break;
                    case "CASE - STATIC 1 - LOAD ASSIGNMENTS":
                        parse_Table_CASE_STATIC_1_LOAD_ASSIGNMENTS(line);
                        break;
                    case "LOAD PATTERN DEFINITIONS":
                        parse_Table_LOAD_PATTERN_DEFINITIONS(line);
                        break;
                    case "COMBINATION DEFINITIONS":
                        parse_Table_COMBINATION_DEFINITIONS(line);
                        break;
                    case "CONNECTIVITY - FRAME":
                        parse_Table_CONNECTIVITY_FRAME(line);
                        break;
                    case "FRAME SECTION ASSIGNMENTS":
                        parse_Table_FRAME_SECTION_ASSIGNMENTS(line);
                        break;
                    case "FRAME LOADS - DISTRIBUTED":
                        parse_Table_FRAME_LOADS_DISTRIBUTED(line);
                        break;
                    case "FRAME SECTION PROPERTIES 01 - GENERAL":
                        parse_Table_FRAME_SECTION_PROPERTIES_01_GENERAL(line);
                        break;
                    case "JOINT COORDINATES":
                        parse_Table_JOINT_COORDINATES(line);
                        break;
                    case "JOINT RESTRAINT ASSIGNMENTS":
                        parse_Table_JOINT_RESTRAINT_ASSIGNMENTS(line);
                        break;
                    case "GRID LINES":
                        parse_Table_GRID_LINES(line);
                        break;
                    case "LOAD CASE DEFINITIONS":
                        break;
                    case "MATERIAL PROPERTIES 01 - GENERAL":
                        break;
                    case "MATERIAL PROPERTIES 02 - BASIC MECHANICAL PROPERTIES":
                        parse_Table_MATERIAL_PROPERTIES_02_BASIC_MECHANICAL_PROPERTIES(line);
                        break;
                    case "MATERIAL PROPERTIES 03A - STEEL DATA":
                        break;
                    case "MATERIAL PROPERTIES 03B - CONCRETE DATA":
                        break;
                    case "JOINT CONSTRAINT ASSIGNMENTS":
                        parse_Table_JOINT_CONSTRAINT_ASSIGNMENTS(line);
                        break;
                    case "CONSTRAINT DEFINITIONS - DIAPHRAGM":
                        parse_Table_CONSTRAINT_DEFINITIONS_DIAPHRAGM(line);
                        break;
                    default:
                        break;
                }
                continue;
            }
            foreach (var bar in modelDB.bars)
            {
                bar.SectionID = modelDB.sections.FirstOrDefault(m => m.Name == bar.SectionName, new section { ID = 0 }).ID;
                bar.MaterialName = sectMaterial[bar.SectionID];
                bar.MaterialID = modelDB.materials.FirstOrDefault(m => m.Name == bar.MaterialName, new material { ID = 0 }).ID;
            }
            foreach (var load in modelDB.barDistrLoads)
            {
                load.Case = modelDB.loadCases.FirstOrDefault(c => c.Name == loadPatterns[load.ID], new loadCase { ID = -1 }).ID;
            }
        }
        void parse_Table_PROGRAM_CONTROL(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)"); 
            var m = rx.Matches(line);
            var flag = true; 
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                modelDB.programParams.Add(new programParam { Name = varName, Value = value });
                if(varName == "CurrUnits")
                {
                    var sapCurrUnits =value.Replace(" ", "").Split(",").ToArray();
                    if (sapCurrUnits[0] == "lbf" || sapCurrUnits[0] == "kpi")
                    {
                        App.currUnits = string.Join(",", new[] { sapCurrUnits[0], "lb", sapCurrUnits[1], sapCurrUnits[2] });
                    }
                    else
                    {
                        App.currUnits = string.Join(",", new[] { sapCurrUnits[0], "kg", sapCurrUnits[1], sapCurrUnits[2] });
                    }

                    uc.setUc(App.currUnits);
                    flag = false;

                }

            }
                if (flag) throw new Exception("No units entry finded!!! model canno be imported");
        }
        void parse_Table_CASE_MODAL_1_GENERAL(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Case":
                        break;
                    case "ModeType":
                        break;
                    case "MaxNumModes":
                        break;
                    case "MinNumModes":
                        break;
                    case "EigenShift":
                        break;
                    case "EigenCutoff":
                        break;
                    case "EigenTol":
                        break;
                    case "AutoShift":
                        break;
                }
            }
        }
        void parse_Table_LOAD_PATTERN_DEFINITIONS(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            var caso = new loadCase();
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "LoadPat":
                        caso.ID = modelDB.loadCases.Count;
                       caso.Name = value;
                        break;
                    case "DesignType":
                        var res = value switch
                        {
                            "Dead" => LoadCaseType.Dead,
                            "Live"=>LoadCaseType.Live,
                            "Roof Live"=>LoadCaseType.Roof,
                            "Wind"=>LoadCaseType.Wind,
                            "Quake"=>LoadCaseType.Quake,
                            _=>LoadCaseType.Other,
                        };
                        caso.Type = (int)res;
                        break;
                }
            }
            modelDB.loadCases.Add(caso);
        }
        void parse_Table_CASE_STATIC_1_LOAD_ASSIGNMENTS(string line)
        {
            return;
        }
        void parse_Table_COMBINATION_DEFINITIONS(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            var c = new loadComb();
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "ComboName":
                        c.Name = value;
                        break;
                    case "ComboType":
                        break;
                    case "AutoDesign":
                        break;
                    case "CaseName":
                        c.Case = modelDB.loadCases.FirstOrDefault(cc=>cc.Name == value,new loadCase { ID=-1}).ID;
                        break;
                    case "ScaleFactor":
                        c.Factor = double.Parse(value);
                        break;
                    case "SteelDesign":
                        break;
                    case "ConcDesign":
                        break;
                    case "AlumDesign":
                        break;
                    case "ColdDesign":
                        break;
                    case "GUID":
                        break;
                }
            }
            c.ID = modelDB.loadCombs.Count + 1;
            modelDB.loadCombs.Add(c);
        }
        void parse_Table_CONNECTIVITY_FRAME(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            var b= new bar();
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Frame":
                        b.ID = modelDB.bars.Count ; ;
                        b.label = value;
                        break;
                    case "JointI":
                        b.startNode = modelDB.nodes.FirstOrDefault(p => p.label == value, new node {ID = -1 }).ID;
                        if (b.startNode == -1) throw new Exception($"Error parsing frama {b.ID}, node {value} does not have al ID");
                        break;
                    case "JointJ":
                        b.endNode = modelDB.nodes.FirstOrDefault(p => p.label == value, new node { ID = -1 }).ID;
                        if (b.endNode == -1) throw new Exception($"Error parsing frama {b.ID}, node {value} does not have al ID");
                        break;
                    case "IsCurved":
                        break;
                    case "GUID":
                        break;
                }
            }
            var res = modelDB.bars.FirstOrDefault(ba => ba.label == b.label, new bar { ID = -1 });
            if(res.ID==-1){
                modelDB.bars.Add(b);
            }
            else
            {
                int idx = modelDB.bars.FindIndexByID(res.ID);
                res.label = b.label;
                res.startNode = b.startNode;
                res.endNode = b.endNode;
            }
        }
      void   parse_Table_FRAME_SECTION_ASSIGNMENTS(string line) {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            var b = new bar();
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Frame":
                        b.ID = modelDB.bars.Count ; 
                        b.label = value;
                        break;
                    case "AnalSect":
                       b.SectionName = value;
                       b.SectionID = modelDB.sections.FirstOrDefault(s=>s.Name==value,new section { ID=-1}).ID;
                       b.MaterialName =modelDB.materials.FirstOrDefault(m=>m.ID==0,new material { ID = -1, Name=""}).Name;
                       b.MaterialID = 0;
                        break;
                }
            }
            var res = modelDB.bars.FirstOrDefault(ba => ba.ID == b.ID, new bar { ID = -1 });
            if (res.ID == -1)
            {
                modelDB.bars.Add(b);
            }
            else
            {
                res.ID = b.ID;
                res.label = b.label;
                res.SectionName = b.SectionName;
                res.SectionID = b.SectionID;
                res.MaterialID = b.MaterialID;
                res.MaterialName = b.MaterialName;
            }
        }
        void   parse_Table_FRAME_LOADS_DISTRIBUTED(string line) {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var s = new barDistrLoad();
            string dir = "";
            if (line.EndsWith("_"))
            {
                preLine += line;
                return;
            }
            line = preLine + line;
            var m = rx.Matches(line);
            preLine = "";
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Frame":
                        s.ID = modelDB.barDistrLoads.Count ;
                        s.BarID =modelDB.bars.FirstOrDefault(b=>b.label == value, new bar { ID=-1}).ID ;
                        if (s.BarID == -1) throw new Exception($"Error parsing frama {value}, does not have al ID");
                        break;
                    case "LoadPat":
                        loadPatterns.Add(s.ID, value);
                        break;
                    case "CoordSys":
                        s.Type = value=="GLOBAL"?0:1;
                        break;
                    case "Dir":
                        dir = value;
                        break;
                    case "DistType":
                        break;
                    case "RelDistA":
                        s.a = uc.lenFromUserUnits(value);
                        break;
                    case "RelDistB":
                        s.b = uc.lenFromUserUnits(value);
                        break;
                    case "Area":
                        s.b = uc.lenFromUserUnits(value);
                        break;
                    case "FOverLA":
                        switch (dir)
                        {
                            case "X":
                                s.Fx = uc.forceFromUserUnits(value);
                                break;
                            case "Y":
                                s.Fy = uc.forceFromUserUnits(value);
                                break;
                            case "Z":
                                s.Fz = uc.forceFromUserUnits(value);
                                break;
                            case "Gravity":
                                s.Fz = -uc.forceFromUserUnits(value);
                                break;
                        }
                        break;
                }
            }
            modelDB.barDistrLoads.Add(s);
        }
        string preLine;
        void parse_Table_FRAME_SECTION_PROPERTIES_01_GENERAL(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var s = new section();
            if (line.EndsWith("_"))
            {
                preLine += line;
                return;
            }
            line = preLine+line;
            var m = rx.Matches(line);
            preLine = "";
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "SectionName":
                        s.ID = modelDB.sections.Count;
                        s.Name = value;
                        break;
                    case "Shape":
                        var t= value switch
                        {
                            "Box/Tube"=> sectType.Rect,
                            "W"=>sectType.WShape,
                            "Circular"=>sectType.Circ,
                            "Angle"=>sectType.Angle,
                            "Pipe"=>sectType.Circ,
                            "General"=>sectType.Rect,
                            _ =>sectType.Rect
                        };
                        s.Type = (int)t;
                        break;
                    case "t3":
                        s.h = uc.lenToUserUnits(value);
                        break;
                    case "t2":
                        s.b = uc.lenToUserUnits(value);
                        break;
                    case "tf":
                        s.tf = uc.lenToUserUnits(value);
                        break;
                    case "tw":
                        s.tw = uc.lenToUserUnits(value);
                        break;
                    case "Area":
                        s.Ag = uc.areaToUserUnits(value);
                        break;
                    case "I23":
                        s.Iyz = uc.len4ToUserUnits(value);
                        break;
                    case "I22":
                        s.Izz = uc.len4ToUserUnits(value);
                        break;
                    case "I33":
                        s.Iyy = uc.len4ToUserUnits(value);
                        break;
                    case "AS3":
                        s.Acyy = uc.areaToUserUnits(value);
                        break;
                    case "AS2":
                        s.Aczz = uc.areaToUserUnits(value);
                        break;
                    case "S22":
                        s.Szz = uc.len3ToUserUnits(value);
                        break;
                    case "S33":
                        s.Syy = uc.len3ToUserUnits(value);
                        break;
                    case "R22":
                        s.rzz = uc.lenToUserUnits(value);
                        break;
                    case "R33":
                        s.ryy = uc.lenToUserUnits(value);
                        break;
                    case "TorsConst":
                        s.Tor = uc.len4ToUserUnits(value);
                        break;
                    case "Material":
                        sectMaterial.Add(s.ID,value);
                        break;
                }
            }
            modelDB.sections.Add(s);
        }
        void parse_Table_JOINT_COORDINATES(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            var n = new node();
            var r = new nodalRestraint();
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Joint":
                        n.ID = modelDB.nodes.Count; ;
                        n.label = value;
                        break;
                    case "XorR":
                        n.X = uc.lenToUserUnits(value);
                        break;
                    case "Y":
                        n.Y = uc.lenToUserUnits(value);
                        break;
                    case "Z":
                        n.Z = uc.lenToUserUnits(value);
                        break;
                    case "CoordSys":
                        break;
                    case "CoordType":
                        break;
                    case "SpecialJt":
                        break;
                    case "GUID":
                        break;
                }
            }
            modelDB.nodes.Add(n);
            r.ID = n.ID;
            modelDB.nodalRestraints.Add(r);
        }
        void parse_Table_JOINT_RESTRAINT_ASSIGNMENTS(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            int id = -1, idx = -1;
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Joint":
                        id = modelDB.nodes.FirstOrDefault(n => n.label == value, new node { ID = -1 }).ID;
                        idx = modelDB.nodalRestraints.FindIndexByID(id);
                        if (id == -1 || idx == -1) throw new Exception("Restraint index does not exists");
                        break;
                    case "U1":
                       modelDB.nodalRestraints[idx].ux = value == "Yes" ? 1 : 0;
                        break;
                    case "U2":
                        modelDB.nodalRestraints[idx].uy = value == "Yes" ? 1 : 0;
                        break;
                    case "U3":
                        modelDB.nodalRestraints[idx].uz = value == "Yes" ? 1 : 0;
                        break;
                    case "R1":
                        modelDB.nodalRestraints[idx].rx = value == "Yes" ? 1 : 0;
                        break;
                    case "R2":
                        modelDB.nodalRestraints[idx].ry = value == "Yes" ? 1 : 0;
                        break;
                    case "R3":
                        modelDB.nodalRestraints[idx].rz = value == "Yes" ? 1 : 0;
                        break;
                }
            }
        }
            int  xid = 1, yid = 1, zid = 1;
        void parse_Table_GRID_LINES(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
            var sc = "default";
            var ad = "";
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "CoordSys":
                        sc = value;
                        if (value == "GLOBAL") sc = "Default";
                        break;
                    case "AxisDir":
                        ad = value;
                        break;
                    case "XRYZCoord":
                        switch (ad)
                        {
                            case "X":
                                modelDB.gridCoordsX.Add(new coordinate(){ ID= xid, Coord = uc.lenToUserUnits(value),coordSys=sc});
                                xid++;
                                break;
                            case "Y":
                                modelDB.gridCoordsY.Add(new coordinate() { ID = yid, Coord = uc.lenToUserUnits(value), coordSys = sc });
                                yid++;
                                break;
                            case "Z":
                                modelDB.gridCoordsZ.Add(new coordinate() { ID = zid, Coord = uc.lenToUserUnits(value), coordSys = sc });
                                zid++;
                                break;
                        }
                        break;
                }
            }
        }
        void parse_Table_MATERIAL_PROPERTIES_02_BASIC_MECHANICAL_PROPERTIES(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var s = new material();
            if (line.EndsWith("_"))
            {
                preLine += line;
                return;
            }
            line = preLine + line;
            var m = rx.Matches(line);
            preLine = "";
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Material":
                        s.ID = modelDB.materials.Count;
                        s.Name = value;
                        break;
                    case "UnitWeight":
                        s.W = uc.forceToUserUnits(value);
                        break;
                    case "UnitMass":
                        s.M = uc.massToUserUnits(value);
                        break;
                    case "E1":
                        s.E = uc.pressToUserUnits(value);
                        break;
                    case "G12":
                        s.G = uc.pressToUserUnits(value);
                        break;
                    case "U12":
                        s.v = double.Parse(value);//adimentional
                        break;
                }
            }
            modelDB.materials.Add(s);
        }
        void parse_Table_CONSTRAINT_DEFINITIONS_DIAPHRAGM(string line) 
        {
                var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
                var m = rx.Matches(line);
                foreach (Match x in m)
                {
                    string varName = x.Groups[1].Value;
                    string value = x.Groups[2].Value;
                    switch (varName)
                    {
                        case "Name":
                        int id = modelDB.nodalConstraints.Count;
                        modelDB.nodalConstraints.Add(new nodalConstraint() { ID = id, label = value, Type = 0 });
                            break;
                    }
                }
            }
        void parse_Table_JOINT_CONSTRAINT_ASSIGNMENTS(string line)
        {
            var rx = new Regex(@"(\w+)=(""[^""]+""|[^\s]+)");
            var m = rx.Matches(line);
    int joint = -1;
            foreach (Match x in m)
            {
                string varName = x.Groups[1].Value;
                string value = x.Groups[2].Value;
                switch (varName)
                {
                    case "Joint":
                        joint = modelDB.nodes.First(n=>n.label == value).ID;
                        break;
                    case "Constraint":
                        var r = modelDB.nodalConstraints.First(r => r.label == value);
                        r.nodesIDList += $"{joint},";
                        break;
                }
            }
        }
    }
    }