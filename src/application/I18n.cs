//#define ES
#if !ES
#define EN
#endif

using System.Collections.Frozen;

namespace CEF;

//A BIG TODO:

internal static class I18n
{

    //Tables   
    //As soon as there are few entryes (<100) this works fine
    public static readonly FrozenDictionary<string, string> translateTableNames =
            new Dictionary<string, string>
            {   //Table Names
                ["gridCoordsX"] = gridCoordsX,
                ["gridCoordsY"] = gridCoordsY,
                ["gridCoordsZ"] = gridCoordsZ,
                ["barThermalLoads"] = barThermalLoads,
                ["programParams"] = programParams,
                ["loadCombs"] = loadCombs,
                ["loadCases"] = loadCases,
                ["nodalDisplacements"] = nodalDisplacements,
                ["nodalReactions"] = nodalReactions,
                ["nodalRestraints"] = nodalRestraints,
                ["nodes"] = nodes,
                ["nodalLoads"] = nodalLoads,
                ["Materials"] = Materials,
                ["Sections"] = Sections,
                ["bars"] = bars,
                ["barDistrLoads"] = barDistrLoads,
                ["barPunctLoads"] = barPunctLoads,
                ["barInternForces"] = barInternForces,
                ["nodalConstraints"] = nodalConstraints,
                //Table Fields
                ["Name"] = Name,
                ["Type"] = Type,
                ["Value"] = Value,
                ["Coord"] = Coord,
                ["coordSys"] = coordSys,
                ["label"] = label,
                ["startNode"] = startNode,
                ["endNode"] = endNode,
                ["Case"] = Case,
                ["Distance"] = Distance,
                ["MaterialID"] = MaterialID,
                ["SectionID"] = SectionID,
                ["MaterialName"] = MaterialName,
                ["SectionName"] = SectionName,
                ["unused"] = unused,
                ["Rotation"] = Rotation,
                ["releases"] = releases,
                ["offsetY"] = offsetY,
                ["offsetZ"] = offsetZ,
                ["masterNodeID"] = masterNodeID,
                ["nodesIDList"] = nodesIDList,

            }.ToFrozenDictionary();

    public const string SplitChar = ","; //not to be localizable
    public const string GroupingKey = "@"; //not to be localizable

#if ES
    // Spanish (es)
    public const string About = "Acerca de";
    public const string File = "Archivo";
    public const string New = "Nuevo";
    public const string Open = "Abrir";
    public const string Save = "Guardar";
    public const string SaveAs = "Guardar como";
    public const string Export = "Exportar...";
    public const string ExportToDxf = "Exportar a DXF";
    public const string Import = "Importar...";
    public const string ImportSap2000S2k = "Importar SAP2000 S2K";
    public const string Exit = "Salir";
    public const string Edit = "Editar";
    public const string Copy = "Copiar";
    public const string Replicate = "Replicar";
    public const string Move = "Mover";
    public const string Rotate = "Rotar";
    public const string Delete = "Borrar";
    public const string Split = "Dividir";
    public const string SplitByLengthOrElements = "Dividir por longitud o elementos";
    public const string SplitAtIntersectionWithSelectedBars = "Dividir en intersección con barras seleccionadas";
    public const string SplitAtIntersectionWithSelectedNodes = "Dividir en intersección con nudos seleccionados";
    public const string MoveNodeGraphically = "Mover nudo gráficamente";
    public const string RemoveOrphanNodes = "Eliminar nudos huérfanos";
    public const string RemoveOverlappingNodes = "Eliminar nudos sobrepuestos";
    public const string MergeSelectedBars = "Unir barras seleccionadas";
    public const string Undo = "Deshacer";
    public const string Draw = "Dibujar";
    public const string Node = "Nudo";
    public const string NodeGraphically = "Nudo (gráficamente)";
    public const string Bar = "Barra";
    public const string Select = "Seleccionar";
    public const string SelectOverlappingNodes = "Seleccionar nudos sobrepuestos";
    public const string SelectOverlappingBars = "Seleccionar barras sobrepuestas";
    public const string SelectAll = "Seleccionar todo";
    public const string DeselectAll = "Deseleccionar todo";
    public const string Model = "Modelo";
    public const string Materials = "Materiales";
    public const string Sections = "Secciones";
    public const string Nodes = "Nudos";
    public const string BoundaryConditions = "Restricciones de contorno";
    public const string Loads = "Cargas";
    public const string Displacements = "Desplazamientos";
    public const string KinematicConstraints = "Restricciones cinemáticas";
    public const string Bars = "Barras";
    public const string LoadsEllipsis = "Cargas...";
    public const string DistributedLoads = "Cargas distribuidas";
    public const string PointLoads = "Cargas puntuales";
    public const string ReleaseDegreeOfFreedom = "Liberar grado de libertad";
    public const string InsertionPoints = "Puntos de inserción";
    public const string CoordinateSystems = "Sistemas coordenados";
    public const string NewEdit = "Nuevo/Editar...";
    public const string LoadCases = "Casos de carga";
    public const string EditCreateDelete = "Editar/Crear/Eliminar";
    public const string SelectActiveCase = "Seleccionar caso activo";
    public const string LoadCombinations = "Combinaciones de carga";
    public const string Databases = "Bases de datos";
    public const string TreeView = "Vista de árbol";
    public const string TableView = "Vista de tablas";
    public const string Units = "Unidades";
    public const string Analysis = "Análisis";
    public const string LinearElasticAnalysis = "Análisis lineal elástico";
    public const string LinearAnalysisWithPd = "Análisis lineal con P-Δ";
    public const string Options = "Opciones";
    public const string Results = "Resultados";
    public const string ViewTables = "Ver tablas";
    public const string NodeDisplacements = "Desplazamientos de nudos";
    public const string NodeReactions = "Reacciones en nudos";
    public const string BarInternalForces = "Acciones internas en barras";
    public const string Diagrams = "Diagramas";
    public const string ClearResults = "Borrar resultados";
    public const string View = "Ver";
    public const string DisplayOptions = "Opciones de visualización";
    public const string RestoreView = "Restaurar vista";
    public const string Viewframes = "Viewframes";
    public const string View1 = "1";
    public const string View2 = "2";
    public const string View3 = "3";
    public const string View4 = "4";
    public const string Views = "Vistas";
    public const string XY = "XY";
    public const string XZ = "XZ";
    public const string YZ = "YZ";
    public const string View3D = "3D";
    public const string Zoom = "Zoom";
    public const string ZoomWindow = "Zoom ventana";
    public const string ZoomExtents = "Zoom general";
    public const string ZoomToSelection = "Zoom a selección";
    public const string Info = "Información";
    public const string MeasureDistanceBetweenTwoNodes = "Medir distancia entre dos nodos";
    public const string SelectedBarsInfo = "Información de barras seleccionadas";
    public const string Windows = "Ventanas";
    public const string NewWindow = "Nueva ventana";
    public const string Help = "Ayuda";
    public const string ReferenceManual = "Manual de referencia";
    public const string License = "Licencia";
    public const string Test = "Test";
    public const string SnapPoint = "Pnt";
    public const string SnapEnd = "End";
    public const string SnapCenter = "Cen";
    public const string SnapNear = "Nea";
    public const string SnapPerpendicular = "Per";

    //Table names to display
    public const string gridCoordsX = "Coordenadas X";
    public const string gridCoordsY = "Coordenadas Y";
    public const string gridCoordsZ = "Coordenadas Z";
    public const string barThermalLoads = "Cargas Térmicas Barras";
    public const string programParams = "Parámetros del Programa";
    public const string loadCombs = "Combinaciones de Carga";
    public const string loadCases = "Casos de Carga";
    public const string nodalDisplacements = "Desplazamientos Nodales";
    public const string nodalReactions = "Reacciones en Nodos";
    public const string nodalRestraints = "Restricciones en Nodos";
    public const string nodes = "Nodos";
    public const string nodalLoads = "Cargas en Nodos";

    public const string bars = "Barras";
    public const string barDistrLoads = "Cargas Distribuidas en Barras";
    public const string barPunctLoads = "Cargas Puntuales en Barras";
    public const string barInternForces = "Fuerzas Internas en Barras";
    public const string nodalConstraints = "Restricciones Cinemáticas";
    //Table Fields
    public const string Name = "Nombre";
    public const string Type = "Tipo";
    public const string Value = "Valor";
    public const string Coord = "Coordenada";
    public const string coordSys = "Sistema Coordenado";
    public const string label = "Etiqueta";
    public const string startNode = "Nodo Inicial";
    public const string endNode = "Nodo Final";
    public const string Case = "Caso";
    public const string Distance = "Distancia";
    public const string MaterialID = "ID de Material";
    public const string SectionID = "ID de Sección";
    public const string MaterialName = "Nombre de Material";
    public const string SectionName = "Nombre de Sección";
    public const string unused = "Omitido";
    public const string Rotation = "Giro";
    public const string releases = "Liberación de GDLs";
    public const string offsetY = "Punto de Inserción Y";
    public const string offsetZ = "Punto de Inserción Z";
    public const string masterNodeID = "Nodo Maestro";
    public const string nodesIDList = "Lista de Nodos";

    //Mensajes:

    public const string NewNode = "Nuevo nudo";
    public const string ImputInstr1 = $"Ingrese coordenadas ejemplo: 1.2{SplitChar}3.4{SplitChar}4.5";
    public const string OutPutInstr1 = "Debe haber exactamente dos puntos seleccionados";
    
    public const string ImputOffests = "Ingrese los offsets para x,y,z";
    public const string offsetX2 = "offset X";
    public const string offsetY2 = "offset Y";
    public const string offsetZ2 = "offset Z";

    public const string ImputRotation = "Ingrese los angulos de rotación en ° ej:45:";
    public const string RotationX = "Rotación X (r1)";
    public const string RotationY = "Rotación Y (r2)";
    public const string RotationZ = "Rotación Z (r3)";

    public const string ImputDireVect = "Ingrese la dirección de replicado:";
    public const string DireVectX = "Dirección X";
    public const string DireVectY = "Dirección Y";
    public const string DireVectZ = "Dirección Z";

    public const string ImputRotationCent = "Ingrese el centro de rotación:";
    public const string RotCenX = "Centro X";
    public const string RotCenY = "Centro Y";
    public const string RotCenZ = "Centro Z";

    public const string ImputSpacing = $"Ingrese los espaciamientos separados por {SplitChar}.";
    public const string ImputSpacingExample = $"Ejemplo: 2{GroupingKey}3.5 o 1{SplitChar}5.3{SplitChar}3";
    public const string Spacing = "Espaciamientos: ";

    public const string DivideByElemsCuant = "Dividir usando L/l elementos: ";
    public const string DivideByEleemLen = "Dividir en elementos de máx x ";
    public const string ImputMsgDivideByElemsCuant = "Ingrese el numero de divisiones: ";
    public const string ImputMsgDivideByEleemLen = "Dividir en elementos de máx x ";

    public const string NoOverlappedBarsFound = "No se encontraron barras superpuestas";

    public const string DlgOKText = "Aceptar";
    public const string DlgCancelText = "Cancelar";

    //Dialogs


    public const string LoadDB = "Cargar BD";
    public const string EditCreate = "Editar/Crear";
    public const string leftArrow = "<";
    public const string rightArrow = ">";
    public const string CannotLoadDB = "No se pudo cargar la base de datos.";

    public const string DiaphragmZ = "Diafragma (Z)";
    public const string DiaphragmXX = "Diafragma (XX)";
    public const string DiaphragmYY = "Diafragma (YY)";
    public const string PlateZ = "Placa (Z)";
    public const string PlateXX = "Placa (XX)";
    public const string PlateYY = "Placa (YY)";
    public const string Body = "Cuerpo";
    public const string Weld = "Soldadura";
    public const string BeamX = "Viga (X)";
    public const string BeamY = "Viga (Y)";
    public const string BeamZ = "Viga (Z)";
    public const string RodX = "Varilla (X)";
    public const string RodY = "Varilla (Y)";
    public const string RodZ = "Varilla (Z)";
    public const string Equal = "Igual";
    public const string CheckInput = "Revise los datos ingresados";
    public const string Replace = "Remplazar";
    public const string SetNodalLoads = "Asignar Carga  Nudos";
    public const string SetPunctBarLoads = "Asignar Carga Puntual Barras";
    public const string Fraction = "Fracción";
    public const string Global = "Global";
    public const string Local = "Local";
    public const string CalProps = "Calcular Propiedades";
    public const string Add = "Agregar";
    public const string AddNewLoadCase = "Nuevo Caso de Carga";
    public const string AssignMaterialToBars = "Asignar Material a Barras";
    public const string AssignSectionToBars = "Asignar Sección";
    public const string Restrain = "Restringir";
    public const string Fixed = "Empotrado";
    public const string Free = "Libre";
    public const string Hinge = "Bisagra";
    public const string Roller = "Patín";
    public const string RotationAngle = "Angulo de Giro (°)";

    public const string ip_center = "Centro";
    public const string ip_down = "Arriba (+zz)";
    public const string ip_up = "Abajo  (-zz)";
    public const string ip_left = "Izquierda(-yy)";
    public const string ip_right = "Derecha  (+yy)";

    public const string min = "mín";
    public const string max = "máx";
    public const string ValueAtStartNode = "Valor en nodo inicial";
    public const string ValueAtEndNode = "Valor en nodo final";
    public const string BatchImput = "Linea de comandos";
    
    //Visual Styles texts
    public const string CategoryScope = "Alcance";
    public const string CategoryCanvas = "Lienzo";
    public const string CategoryModelElements = "Elementos del Modelo";
    public const string CategorySelect = "Seleccionar";
    public const string CategoryAppearance = "Apariencia";
    public const string CategoryLoads = "Cargas";
    public const string CategoryResults = "Resultados";
    public const string CategoryFontsAndText = "Fuentes y texto";
    public const string CategoryAuxiliaryElements = "Elementos auxiliares";
    public const string CategoryStyles = "Estilos";

    public const string DisplayApplyToAllWindows = "Aplicar a todas las ventanas";
    public const string DisplayApplyToNewWindows = "Aplicar a las ventanas nuevas";
    public const string DisplayCanvasBackgroundColor = "Color de fondo del Lienzo";
    public const string DisplayNodeColor = "Color de los Nodos";
    public const string DisplayBarShadowColor = "Color de sombra barras";
    public const string DisplayNodeThickness = "Espesor Nudos";
    public const string DisplayBarLineColor = "Color de lineas de Barras";
    public const string DisplayBarLineWidth = "Espesor de lineas de Barras";
    public const string DisplaySelectedColor = "Color de Seleccionados";
    public const string DisplayLoadsColor = "Color de las Cargas";
    public const string DisplayPointLoadsColor = "Color de las Cargas Puntuales";
    public const string DisplayDeformedShapeColor = "Color de la deformada";
    public const string DisplaySupportBackgroundColor = "Color de fondo soportes";
    public const string DisplayFont = "Fuente";
    public const string DisplayGridLineColor = "Color linea malla";
    public const string DisplayGridLineWidth = "Espesor linea malla";
    public const string DisplayRenderDivisions = "Divisiones Renderizado";
    public const string DisplayShowNodeId = "Mostar id Nudos";
    public const string DisplayShowGrid = "Mostrar la regilla";
    public const string DisplayShowBarLoads = "Mostar cargas en barras";
    public const string DisplayShowDeformedShape = "Mostar deformada";
    public const string DisplayShowNodeLoads = "Mostar cargas en nudos";
    public const string DisplayShowNodes = "Mostar nudos";
    public const string DisplayShowSelectedNodes = "Mostar nudos seleccionados";
    public const string DisplayRenderStyle = "Estilo de renderizado";
    public const string DisplayGridLineStyle = "Estilo de lines malla";
    public const string DisplayVisualTheme = "Tema visual";
    public const string DisplayDeformScaleLinear = "Escala deformada dx, dy, dz";
    public const string DisplayDeformScaleRotational = "Escala deformada rx, ry, rz";

    public const string DescriptionRenderSegments = "Numero de segmentos en Render, rec. 6";
    public const string DescriptionRenderOptions = "0/Otro- Lineas, 1- Extruido. 2- Alambre";
    public const string DescriptionCanvasStyle = "Estilo general del lienzo";

#endif


#if EN    // English (en)

    public const string About = "About";

    public const string File = "File";
    public const string New = "New";
    public const string Open = "Open";
    public const string Save = "Save";
    public const string SaveAs = "Save As";
    public const string Export = "Export...";
    public const string ExportToDxf = "Export to DXF";
    public const string Import = "Import...";
    public const string ImportSap2000S2k = "Import SAP2000 S2K";
    public const string Exit = "Exit";
    public const string Edit = "Edit";
    public const string Copy = "Copy";
    public const string Replicate = "Replicate";
    public const string Move = "Move";
    public const string Rotate = "Rotate";
    public const string Delete = "Delete";
    public const string Split = "Split";
    public const string SplitByLengthOrElements = "Split by length or elements";
    public const string SplitAtIntersectionWithSelectedBars = "Split at intersection with selected bars";
    public const string SplitAtIntersectionWithSelectedNodes = "Split at intersection with selected nodes";
    public const string MoveNodeGraphically = "Move Node graphically";
    public const string RemoveOrphanNodes = "Remove orphan nodes";
    public const string RemoveOverlappingNodes = "Remove overlapping nodes";
    public const string MergeSelectedBars = "Merge selected bars";
    public const string Undo = "Undo";
    public const string Draw = "Draw";
    public const string Node = "Node";
    public const string NodeGraphically = "Node (graphically)";
    public const string Bar = "Bar";
    public const string Select = "Select";
    public const string SelectOverlappingNodes = "Select overlapping nodes";
    public const string SelectOverlappingBars = "Select overlapping bars";
    public const string SelectAll = "Select all";
    public const string DeselectAll = "Deselect all";
    public const string Model = "Model";
    public const string Materials = "Materials";
    public const string Sections = "Sections";
    public const string Nodes = "Nodes";
    public const string BoundaryConditions = "Boundary Conditions";
    public const string Loads = "Loads";
    public const string Displacements = "Displacements";
    public const string KinematicConstraints = "Kinematic Constraints";
    public const string Bars = "Bars";
    public const string LoadsEllipsis = "Loads...";
    public const string DistributedLoads = "Distributed Loads";
    public const string PointLoads = "Point Loads";
    public const string ReleaseDegreeOfFreedom = "Release Degree of Freedom";
    public const string InsertionPoints = "Insertion Points";
    public const string CoordinateSystems = "Coordinate Systems";
    public const string NewEdit = "New/Edit...";
    public const string LoadCases = "Load Cases";
    public const string EditCreateDelete = "Edit/Create/Delete";
    public const string SelectActiveCase = "Select Active Case";
    public const string LoadCombinations = "Load Combinations";
    public const string Databases = "Databases";
    public const string TreeView = "Tree View";
    public const string TableView = "Table View";
    public const string Units = "Units";
    public const string Analysis = "Analysis";
    public const string LinearElasticAnalysis = "Linear Elastic Analysis";
    public const string LinearAnalysisWithPd = "Linear Analysis with P-Δ";
    public const string Options = "Options";
    public const string Results = "Results";
    public const string ViewTables = "View Tables";
    public const string NodeDisplacements = "Node Displacements";
    public const string NodeReactions = "Node Reactions";
    public const string BarInternalForces = "Bar Internal Forces";
    public const string Diagrams = "Diagrams";
    public const string ClearResults = "Clear Results";
    public const string View = "View";
    public const string DisplayOptions = "Display Options";
    public const string RestoreView = "Restore View";
    public const string Viewframes = "Viewframes";
    public const string View1 = "1";
    public const string View2 = "2";
    public const string View3 = "3";
    public const string View4 = "4";
    public const string Views = "Views";
    public const string XY = "XY";
    public const string XZ = "XZ";
    public const string YZ = "YZ";
    public const string View3D = "3D";
    public const string Zoom = "Zoom";
    public const string ZoomWindow = "Zoom Window";
    public const string ZoomExtents = "Zoom Extents";
    public const string ZoomToSelection = "Zoom to Selection";
    public const string Info = "Info";
    public const string MeasureDistanceBetweenTwoNodes = "Measure distance between two nodes";
    public const string SelectedBarsInfo = "Selected bars information";
    public const string Windows = "Windows";
    public const string NewWindow = "New window";
    public const string Help = "Help";
    public const string ReferenceManual = "Reference Manual";
    public const string License = "License";
    public const string Test = "Test";
    public const string SnapPoint = "Pnt";
    public const string SnapEnd = "End";
    public const string SnapCenter = "Cen";
    public const string SnapNear = "Nea";
    public const string SnapPerpendicular = "Per";

    public const string gridCoordsX = "X Coordinates";
    public const string gridCoordsY = "Y Coordinates";
    public const string gridCoordsZ = "Z Coordinates";
    public const string barThermalLoads = "Bar Thermal Loads";
    public const string programParams = "Program Parameters";
    public const string loadCombs = "Load Combinations";
    public const string loadCases = "Load Cases";
    public const string nodalDisplacements = "Nodal Displacements";
    public const string nodalReactions = "Nodal Reactions";
    public const string nodalRestraints = "Nodal Restraints";
    public const string nodes = "Nodes";
    public const string nodalLoads = "Nodal Loads";

    public const string bars = "Bars";
    public const string barDistrLoads = "Distributed Loads on Bars";
    public const string barPunctLoads = "Point Loads on Bars";
    public const string barInternForces = "Internal Forces in Bars";
    public const string nodalConstraints = "Kinematic Constraints";

    // Table Fields
    public const string Name = "Name";
    public const string Type = "Type";
    public const string Value = "Value";
    public const string Coord = "Coordinate";
    public const string coordSys = "Coordinate System";
    public const string label = "Label";
    public const string startNode = "Start Node";
    public const string endNode = "End Node";
    public const string Case = "Case";
    public const string Distance = "Distance";
    public const string MaterialID = "Material ID";
    public const string SectionID = "Section ID";
    public const string MaterialName = "Material Name";
    public const string SectionName = "Section Name";
    public const string unused = "Unused";
    public const string Rotation = "Rotation";
    public const string releases = "DOF Releases";
    public const string offsetY = "Insertion Point Y";
    public const string offsetZ = "Insertion Point Z";
    public const string masterNodeID = "Master Node";
    public const string nodesIDList = "Node List";

    public const string NewNode = "New Node";
    public const string ImputInstr1 = $"Enter coordinates, example: 1.2{SplitChar}3.4{SplitChar}4.5";
    public const string OutPutInstr1 = "Exactly two points must be selected";

    public const string ImputOffests = "Enter offsets for x, y, z";
    public const string offsetX2 = "Offset X";
    public const string offsetY2 = "Offset Y";
    public const string offsetZ2 = "Offset Z";

    public const string ImputRotation = "Enter rotation angles in ° e.g.: 45:";
    public const string RotationX = "Rotation X (r1)";
    public const string RotationY = "Rotation Y (r2)";
    public const string RotationZ = "Rotation Z (r3)";

    public const string ImputDireVect = "Enter replication direction:";
    public const string DireVectX = "Direction X";
    public const string DireVectY = "Direction Y";
    public const string DireVectZ = "Direction Z";

    public const string ImputRotationCent = "Enter rotation center:";
    public const string RotCenX = "Center X";
    public const string RotCenY = "Center Y";
    public const string RotCenZ = "Center Z";

    public const string ImputSpacing = $"Enter spacings separated by {SplitChar}.";
    public const string ImputSpacingExample = $"Example: 2{GroupingKey}3.5 or 1{SplitChar}5.3{SplitChar}3";
    public const string Spacing = "Spacings: ";

    public const string DivideByElemsCuant = "Divide using L/l elements: ";
    public const string DivideByEleemLen = "Divide into elements of max x ";
    public const string ImputMsgDivideByElemsCuant = "Enter number of divisions: ";
    public const string ImputMsgDivideByEleemLen = "Divide into elements of max x ";

    public const string NoOverlappedBarsFound = "No overlapping bars found";

    public const string DlgOKText = "OK";
    public const string DlgCancelText = "Cancel";

    public const string LoadDB = "Load DB";
    public const string EditCreate = "Edit/Create";
    public const string leftArrow = "<";
    public const string rightArrow = ">";
    public const string CannotLoadDB = "Could not load database.";

    public const string DiaphragmZ = "Diaphragm (Z)";
    public const string DiaphragmXX = "Diaphragm (XX)";
    public const string DiaphragmYY = "Diaphragm (YY)";
    public const string PlateZ = "Plate (Z)";
    public const string PlateXX = "Plate (XX)";
    public const string PlateYY = "Plate (YY)";
    public const string Body = "Body";
    public const string Weld = "Weld";
    public const string BeamX = "Beam (X)";
    public const string BeamY = "Beam (Y)";
    public const string BeamZ = "Beam (Z)";
    public const string RodX = "Rod (X)";
    public const string RodY = "Rod (Y)";
    public const string RodZ = "Rod (Z)";
    public const string Equal = "Equal";
    public const string CheckInput = "Check input data";
    public const string Replace = "Replace";
    public const string SetNodalLoads = "Assign Nodal Loads";
    public const string SetPunctBarLoads = "Assign Point Bar Loads";
    public const string Fraction = "Fraction";
    public const string Global = "Global";
    public const string Local = "Local";
    public const string CalProps = "Calculate Properties";
    public const string Add = "Add";
    public const string AddNewLoadCase = "New Load Case";
    public const string AssignMaterialToBars = "Assign Material to Bars";
    public const string AssignSectionToBars = "Assign Section";
    public const string Restrain = "Restrain";
    public const string Fixed = "Fixed";
    public const string Free = "Free";
    public const string Hinge = "Hinge";
    public const string Roller = "Roller";
    public const string RotationAngle = "Rotation Angle (°)";

    public const string ip_center = "Center";
    public const string ip_down = "Up (+zz)";
    public const string ip_up = "Down (-zz)";
    public const string ip_left = "Left (-yy)";
    public const string ip_right = "Right (+yy)";

    public const string min = "min";
    public const string max = "max";
    public const string ValueAtStartNode = "Value at start Node";
    public const string ValueAtEndNode = "Value at end Node";
    public const string BatchImput = "Use text commands";
    //Visual Styles texts
    public const string CategoryScope = "Scope";
    public const string CategoryCanvas = "Canvas";
    public const string CategoryModelElements = "Model Elements";
    public const string CategorySelect = "Select";
    public const string CategoryAppearance = "Appearance";
    public const string CategoryLoads = "Loads";
    public const string CategoryResults = "Results";
    public const string CategoryFontsAndText = "Fonts and Text";
    public const string CategoryAuxiliaryElements = "Auxiliary Elements";
    public const string CategoryStyles = "Styles";

    public const string DisplayApplyToAllWindows = "Apply to all windows";
    public const string DisplayApplyToNewWindows = "Apply to new windows";
    public const string DisplayCanvasBackgroundColor = "Canvas background color";
    public const string DisplayNodeColor = "Nodes color";
    public const string DisplayBarShadowColor = "Bars shadow color";
    public const string DisplayNodeThickness = "Nodes thickness";
    public const string DisplayBarLineColor = "Bar lines color";
    public const string DisplayBarLineWidth = "Bar lines thickness";
    public const string DisplaySelectedColor = "Selection color";
    public const string DisplayLoadsColor = "Loads color";
    public const string DisplayPointLoadsColor = "Point loads color";
    public const string DisplayDeformedShapeColor = "Deformed shape color";
    public const string DisplaySupportBackgroundColor = "Supports background color";
    public const string DisplayFont = "Font";
    public const string DisplayGridLineColor = "Grid line color";
    public const string DisplayGridLineWidth = "Grid line thickness";
    public const string DisplayRenderDivisions = "Rendering divisions";
    public const string DisplayShowNodeId = "Show nodes ID";
    public const string DisplayShowGrid = "Show grid";
    public const string DisplayShowBarLoads = "Show loads on bars";
    public const string DisplayShowDeformedShape = "Show deformed shape";
    public const string DisplayShowNodeLoads = "Show loads on nodes";
    public const string DisplayShowNodes = "Show nodes";
    public const string DisplayShowSelectedNodes = "Show selected nodes";
    public const string DisplayRenderStyle = "Rendering style";
    public const string DisplayGridLineStyle = "Grid lines style";
    public const string DisplayVisualTheme = "Visual theme";
    public const string DisplayDeformScaleLinear = "Deformation scale dx, dy, dz";
    public const string DisplayDeformScaleRotational = "Deformation scale rx, ry, rz";

    public const string DescriptionRenderSegments = "Number of segments in Render, rec. 6";
    public const string DescriptionRenderOptions = "0/Other- Lines, 1- Extruded, 2- Wireframe";
    public const string DescriptionCanvasStyle = "General canvas style";

#endif


}
