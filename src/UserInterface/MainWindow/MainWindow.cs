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
        App.UserSettings = new();        
        UISettings.initialiced  = false;
        
        App.currUnits = "kN,kg,m,C";
        Uc.setUc(App.currUnits);  

       App.UserSettings = UISettings.Load(10,0);   
       UISettings.initialiced  = true;
       I18n.Init( App.UserSettings.lang);
        
    }

    internal void setMSglob(double Escala)
    {
        App.currColorScheme = 1;
        if (!UISettings.initialiced )
            App.UserSettings = new UISettings((float)(10d * Escala), App.currColorScheme);
        UISettings.initialiced = true;
    }
}