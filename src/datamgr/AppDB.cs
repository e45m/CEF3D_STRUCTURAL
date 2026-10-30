using System.Runtime.CompilerServices;

namespace CEF;
public class AppDB 
{
    public DtaTable<Material> libMaterials;
    public DtaTable<Section> libSections;
    public DtaTable<Node> Puntos;

    public DtaTable<barInternForce> barStatiosForces;
    public bool biFSeted;
    public AppDB()
    {
        InitThisClassProperies();
    }
    public  void InitThisClassProperies()
    {
        libMaterials = [];
        libSections = [];            
        Puntos = [];
        barStatiosForces = [];
        biFSeted = false;
    }

    public void LoadModelFile(string ruta)
    {
        DataMagrIO.mdb = new ModelDB();
        DataMagrIO.ruta = ruta;
        DataMagrIO.LoadModelFile();
        libMaterials = DataMagrIO.mdb.materials;
        libSections = DataMagrIO.mdb.sections;
    }

    public void LoadSectionsDataBase(string ruta)
    {
        DataMagrIO.mdb = new ModelDB();
        DataMagrIO.ruta = ruta;
        DataMagrIO.LoadModelFile();      
        libSections = DataMagrIO.mdb.sections;
    }
        public void LoadMaterialsDataBase(string ruta)
    {
        DataMagrIO.mdb = new ModelDB();
        DataMagrIO.ruta = ruta;
        DataMagrIO.LoadModelFile();
       libMaterials = DataMagrIO.mdb.materials;
      
    }

}

