using System.Globalization;

namespace CEF;
internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        CultureInfo culture = CultureInfo.InvariantCulture;

        // Establece la cultura para (., ,, ; etc)
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;      
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        // Iniciar el resto de la app...
        //TODO: Open File with DblClick
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainWindow()); 
    }
}