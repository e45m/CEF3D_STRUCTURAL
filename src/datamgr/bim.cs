//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//TODO: IFC IMPORT EXPORT

//using Xbim.Ifc;
//using Xbim.Ifc4.Interfaces;
//using static CEF.ModelDB;

//namespace CEF
//{
//    internal class BIMcef
//    {
//        // Requiere: Xbim.Essentials y Xbim.Ifc4 o Xbim.Ifc2x3

//        public void ExportToIFC(string rutaIFC)
//        {
//            using (var model = IfcStore.Create(IfcSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
//            {
//                model.Initialise(ProjectName: "CEF-Export");

//                using (var txn = model.BeginTransaction("Export"))
//                {
//                    var proj = model.Instances.New<IfcProject>(p => {
//                        p.Initialize(ProjectUnits.SIUnits());
//                    });

//                    var site = model.Instances.New<IfcSite>();
//                    var bldg = model.Instances.New<IfcBuilding>();
//                    var lvl = model.Instances.New<IfcBuildingStorey>();
//                    bldg.BuildingAddress = model.Instances.New<IfcPostalAddress>();

//                    proj.AddSite(site);
//                    site.AddBuilding(bldg);
//                    bldg.AddToSpatialDecomposition(lvl);

//                    //---------------------------------------------------
//                    // 1) NODOS → IfcCartesianPoint + IfcLocalPlacement
//                    //---------------------------------------------------
//                    var dicNodos = new Dictionary<int, IfcLocalPlacement>();

//                    if (this.Nodos != null)
//                    {
//                        foreach (var n in this.Nodos.Items)
//                        {
//                            if (n == null) continue;

//                            var p = model.Instances.New<IfcCartesianPoint>(cp =>
//                            {
//                                cp.SetXYZ(
//                                    n.X,
//                                    n.Y,
//                                    n.Z
//                                );
//                            });

//                            var lp = model.Instances.New<IfcLocalPlacement>(pl =>
//                            {
//                                pl.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>(ax =>
//                                {
//                                    ax.Location = p;
//                                });
//                            });

//                            dicNodos[n.ID] = lp;
//                        }
//                    }

//                    //---------------------------------------------------
//                    // 2) MATERIALES
//                    //---------------------------------------------------
//                    var dicMat = new Dictionary<int, IfcMaterial>();

//                    if (this.Materiales != null)
//                    {
//                        foreach (var m in this.Materiales.Items)
//                        {
//                            if (m == null) continue;

//                            var mat = model.Instances.New<IfcMaterial>(mm =>
//                            {
//                                mm.Name = string.IsNullOrWhiteSpace(m.Nombre)
//                                          ? $"MAT_{m.ID}"
//                                          : m.Nombre;
//                            });

//                            dicMat[m.ID] = mat;
//                        }
//                    }

//                    //---------------------------------------------------
//                    // 3) SECCIONES → perfiles IFC
//                    //---------------------------------------------------
//                    var dicPerf = new Dictionary<int, IfcProfileDef>();

//                    if (this.Secciones != null)
//                    {
//                        foreach (var s in this.Secciones.Items)
//                        {
//                            if (s == null) continue;

//                            IfcProfileDef perfil = null;

//                            // Ejemplo: IPE, W, etc
//                            if (s.Tipo == 1) // tu tipo I
//                            {
//                                perfil = model.Instances.New<IfcIShapeProfileDef>(pf =>
//                                {
//                                    pf.ProfileName = s.Nombre ?? ("SEC_" + s.ID);
//                                    pf.OverallDepth = s.h;
//                                    pf.OverallWidth = s.b;
//                                    pf.WebThickness = s.tw;
//                                    pf.FlangeThickness = s.tf;
//                                });
//                            }
//                            else
//                            {
//                                // fallback genérico
//                                perfil = model.Instances.New<IfcRectangleProfileDef>(pf =>
//                                {
//                                    pf.ProfileName = s.Nombre ?? ("SEC_" + s.ID);
//                                    pf.XDim = Math.Max(0.01, s.b);
//                                    pf.YDim = Math.Max(0.01, s.h);
//                                });
//                            }

//                            dicPerf[s.ID] = perfil;
//                        }
//                    }

//                    //---------------------------------------------------
//                    // 4) BARRAS → IfcBeam
//                    //---------------------------------------------------
//                    if (this.Barras != null)
//                    {
//                        foreach (var b in this.Barras.Items)
//                        {
//                            if (b == null) continue;
//                            if (!dicNodos.ContainsKey(b.NudoI)) continue;
//                            if (!dicNodos.ContainsKey(b.NudoJ)) continue;

//                            var pi = this.Nodos.Items.FirstOrDefault(x => x.ID == b.NudoI);
//                            var pj = this.Nodos.Items.FirstOrDefault(x => x.ID == b.NudoJ);
//                            if (pi == null || pj == null) continue;

//                            // vector de dirección
//                            var dx = pj.X - pi.X;
//                            var dy = pj.Y - pi.Y;
//                            var dz = pj.Z - pi.Z;
//                            var L = Math.Sqrt(dx * dx + dy * dy + dz * dz);
//                            if (L < 1e-6) continue;

//                            var beam = model.Instances.New<IfcBeam>(bb =>
//                            {
//                                bb.Name = b.label ?? ("B_" + b.ID);
//                            });

//                            //---------------------------------------------------
//                            // Colocación: origen en NudoI
//                            //---------------------------------------------------
//                            beam.ObjectPlacement = dicNodos[b.NudoI];

//                            //---------------------------------------------------
//                            // Geometría → IfcExtrudedAreaSolid
//                            //---------------------------------------------------
//                            if (dicPerf.TryGetValue(b.Seccion_ID, out var perfil))
//                            {
//                                var body = model.Instances.New<IfcExtrudedAreaSolid>(ex =>
//                                {
//                                    ex.SweptArea = perfil;

//                                    ex.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(
//                                        dx / L,
//                                        dy / L,
//                                        dz / L
//                                    ));

//                                    ex.Depth = L;
//                                    ex.Position = model.Instances.New<IfcAxis2Placement3D>();
//                                });

//                                var shape = model.Instances.New<IfcShapeRepresentation>(sh =>
//                                {
//                                    sh.ContextOfItems = proj.RepresentationContexts.FirstOrDefault();
//                                    sh.RepresentationType = "SweptSolid";
//                                    sh.Items.Add(body);
//                                });

//                                var prodShape = model.Instances.New<IfcProductDefinitionShape>(pd =>
//                                {
//                                    pd.Representations.Add(shape);
//                                });

//                                beam.Representation = prodShape;
//                            }

//                            //---------------------------------------------------
//                            // Material
//                            //---------------------------------------------------
//                            if (dicMat.TryGetValue(b.Material_ID, out var mat))
//                            {
//                                var rel = model.Instances.New<IfcRelAssociatesMaterial>(rm =>
//                                {
//                                    rm.RelatedObjects.Add(beam);
//                                    rm.RelatingMaterial = mat;
//                                });
//                            }

//                            lvl.AddElement(beam);
//                        }
//                    }

//                    txn.Commit();
//                }

//                model.SaveAs(rutaIFC);
//            }
//        }



//public void ImportarIFC(string rutaIFC)
//    {
//        using var ifc = IfcStore.Open(rutaIFC);

//        CargarNodos(ifc);
//        CargarMateriales(ifc);
//        CargarSecciones(ifc);
//        CargarBarras(ifc);
//    }


//        void CargarNodos(IfcStore ifc)
//        {
//            var puntos = ifc.Instances.OfType<IIfcCartesianPoint>();

//            foreach (var p in puntos)
//            {
//                var coor = p.Coordinates;

//                this.Nodos.Add(new Nodo
//                {
//                    ID = this.Nodos.Count + 1,
//                    X = coor.Count > 0 ? coor[0] : 0,
//                    Y = coor.Count > 1 ? coor[1] : 0,
//                    Z = coor.Count > 2 ? coor[2] : 0,
//                    label = p.EntityLabel.ToString()
//                });
//            }
//        }

//        void CargarMateriales(IfcStore ifc)
//        {
//            var mats = ifc.Instances.OfType<IIfcMaterial>();

//            foreach (var m in mats)
//            {
//                this.Materiales.Add(new Material
//                {
//                    ID = this.Materiales.Count + 1,
//                    Nombre = m.Name ?? "",
//                    Tipo = 0,
//                    E = 0,
//                    Fy = 0,
//                    Fu = 0
//                });
//            }
//        }


//        void CargarSecciones(IfcStore ifc)
//        {
//            var perfiles = ifc.Instances.OfType<IIfcProfileDef>();

//            foreach (var pf in perfiles)
//            {
//                this.Secciones.Add(new Seccion
//                {
//                    ID = this.Secciones.Count + 1,
//                    Nombre = pf.ProfileName ?? "",
//                    Tipo = 0,
//                    h = ExtraerDoble(pf, "Depth"),
//                    b = ExtraerDoble(pf, "Width"),
//                    tw = ExtraerDoble(pf, "WebThickness"),
//                    tf = ExtraerDoble(pf, "FlangeThickness")
//                });
//            }
//        }

//        double ExtraerDoble(object o, string prop)
//        {
//            var pi = o.GetType().GetProperty(prop);
//            if (pi == null) return 0;
//            var v = pi.GetValue(o);
//            return v is double d ? d : 0;
//        }

//        void CargarBarras(IfcStore ifc)
//        {
//            var barras = ifc.Instances
//                .Where(x =>
//                    x is IIfcBeam ||
//                    x is IIfcColumn ||
//                    x is IIfcMember)
//                .Cast<IIfcProduct>();

//            foreach (var b in barras)
//            {
//                var reps = b.Representation?.Representations;
//                if (reps == null) continue;

//                // Buscar línea directriz
//                var curve = reps
//                    .SelectMany(r => r.Items)
//                    .OfType<IIfcPolyline>()
//                    .FirstOrDefault();

//                if (curve == null) continue;

//                var p0 = curve.Points[0];
//                var p1 = curve.Points[1];

//                int nI = CrearNodoDesdeIFC(p0);
//                int nJ = CrearNodoDesdeIFC(p1);

//                this.Barras.Add(new Barra
//                {
//                    ID = this.Barras.Count + 1,
//                    label = b.Name ?? "",
//                    NudoI = nI,
//                    NudoJ = nJ,
//                    Material_ID = 0,
//                    Seccion_ID = 0,
//                    giro = 0
//                });
//            }
//        }

//        int CrearNodoDesdeIFC(IIfcCartesianPoint p)
//        {
//            double X = p.Coordinates[0];
//            double Y = p.Coordinates.Count > 1 ? p.Coordinates[1] : 0;
//            double Z = p.Coordinates.Count > 2 ? p.Coordinates[2] : 0;

//            var existente = this.Nodos.FirstOrDefault(n =>
//                Math.Abs(n.X - X) < 1e-6 &&
//                Math.Abs(n.Y - Y) < 1e-6 &&
//                Math.Abs(n.Z - Z) < 1e-6);

//            if (existente != null) return existente.ID;

//            var nuevo = new Nodo
//            {
//                ID = this.Ndos.Count + 1,
//                X = X,
//                Y = Y,
//                Z = Z,
//                label = ""
//            };

//            this.Nodos.Add(nuevo);
//            return nuevo.ID;
//        }


//    }
//}
