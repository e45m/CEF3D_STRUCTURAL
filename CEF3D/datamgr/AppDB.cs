using System.Runtime.CompilerServices;
using Windows.UI.Composition;

namespace CEF;
public class AppDB 
{
    public DtaTable<material> libMaterials;
    public DtaTable<section> libSections;
    public DtaTable<node> Puntos;
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
        libMaterials = DataMagrIO.mdb.materials;
        libSections = DataMagrIO.mdb.sections;
    }

    public void LoadModelFileReflection(string ruta, bool reload = true)
    {
        LoadModelFile(ruta);
    }

}

