using CEF.matrixImpl;
using static CEF.modelView;
using static System.Math;
namespace CEF;
public partial class MainWindow: Cef3DWindowBase
{


    public MainWindow()
    {
        App.mainWindow = this;

        IniInitializeComponent();
        App.activeFileName = "";
        App.visualModelStyles = new();
        App.visualModelStylesInitialized = false;
        
        App.currUnits = "kN,kg,m,C";
        uc.setUc(App.currUnits);     
        
    }

    internal void setMSglob(double Escala)
    {
        App.currColorScheme = 1;
        if (!App.visualModelStylesInitialized)
            App.visualModelStyles = new ModelStyles((float)(10d * Escala), App.currColorScheme);
        App.visualModelStylesInitialized = true;
    }
}