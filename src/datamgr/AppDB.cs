using System.Runtime.CompilerServices;

namespace CEF;
public class AppDB 
{
    public DtaTable<Material> libMaterials;
    public DtaTable<Section> libSections;
    public DtaTable<Node> Puntos;
    public AppDB()
    {
        InitThisClassProperies();
    }
    public  void InitThisClassProperies()
    {
        libMaterials = [];
        libSections = [];            
        Puntos = [];
    }

    public void LoadModelFile(string ruta, bool reload = true)
    {
        DataMagrIO.mdb = new ModelDB();
        DataMagrIO.ruta = ruta;
        DataMagrIO.LoadModelFile();
        libMaterials = DataMagrIO.mdb.Materials;
        libSections = DataMagrIO.mdb.Sections;
    }

    public void LoadModelFileReflection(string ruta, bool reload = true)
    {
        LoadModelFile(ruta);
    }

}

