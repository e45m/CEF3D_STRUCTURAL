using CEF.matrixImpl;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using static CEF.modelView;
using static System.Math;
namespace CEF;
public partial class MainWindow
{
    


    internal void addNewFile(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(App.activeFileName))
        {
            var res = MessageBox.Show(
                "¿Esta seguro de continuar, perderá la información que no haya guardado?", "", MessageBoxButtons.YesNo);
            if (!(res == DialogResult.Yes || res == DialogResult.OK)) return;
        }
        deleteUndoFiles();
        SaveFileDialog SaveFileDialog1 = new();
        SaveFileDialog1.Title = "CEF3D - Guardar..."; 
        SaveFileDialog1.Filter = " cef | *.cef"; 
        SaveFileDialog1.AddExtension = true; 
        SaveFileDialog1.DefaultExt = "cef"; 
        SaveFileDialog1.ShowDialog(); 
        App.activeFileName = SaveFileDialog1.FileName;
        App.mainWindow.Text = App.activeFileName;

        App.setDefaultsToDb();
        space3d.createGrid(this);

        App.setParamProgramDefaults();
        Uc.setUc(App.currUnits);
        App.mainWindow.tsUnitsSel.Text = App.currUnits;       
        
        var vas = new modelView(); 
        vas.Text = vas.Text + "[0]";
        vas.Show(this); 
       
    }
    internal void openFile(object sender, EventArgs e)
    {
        DialogResult r;
        if (!string.IsNullOrEmpty(App.activeFileName))        {
            var res = MessageBox.Show(
                "¿Esta seguro de continuar, perderá la información que no haya guardado?", "", MessageBoxButtons.YesNo);
            if (!(res == DialogResult.Yes || res == DialogResult.OK)) return;             
        }
        OpenFileDialog OpenFileDialog1 = new();
        OpenFileDialog1.DefaultExt = "Cef"; 
        OpenFileDialog1.Filter = "cef | *.cef"; 
        OpenFileDialog1.Title = "CEF3D - Abrir..."; 
        r = OpenFileDialog1.ShowDialog(); 
        if (r == DialogResult.OK | r == DialogResult.Yes)
        {
            App.model = new ModelDB(); 
            App.appData = new AppDB();
            deleteResultsFromDB();
            deleteUndoFiles();
            CloseAllWindows();
            App.activeFileName = OpenFileDialog1.FileName; 
            App.mainWindow.Text = App.activeFileName;
            App.currCoordSist = "Default";
            App. materialDefaultID = -1;
            App.sectionDefaultID = -1;
            App.currColorScheme = 2;
            App.stopUndoRecording();
            App.model.LoadModelFile(App.activeFileName);
            App.startUndoRecording();
            App.initialized = true;
            space3d.createGrid(this);
            App.currUnits = App.model.getParamText(Mdta.units);
            Uc.setUc(App.currUnits);
            var vas = new modelView(); 
            vas.Text = vas.Text + " [" + App.modelViewInstances + "]"+App.activeFileName; 
            vas.Show(App.mainWindow); 
        }
    }
    internal void closeApp(object sender, EventArgs e)
    {
        Close();
    }
    internal void saveFile(object sender, EventArgs e)
    {
        App.model.SaveModelFile(App.activeFileName);
    }
    internal void saveFileAs(object sender, EventArgs e)
    {
        DialogResult r;
        SaveFileDialog SaveFileDialog1 = new();
        SaveFileDialog1.DefaultExt = "CEF- Guardar..."; 
        SaveFileDialog1.Filter = "cef | *.cef"; 
        SaveFileDialog1.Title = "CEF3D"; 
        r = SaveFileDialog1.ShowDialog(); 
        if (r == DialogResult.OK | r == DialogResult.Yes)
        {
            App.activeFileName = SaveFileDialog1.FileName;
            App.mainWindow.Text = App.activeFileName;
            App.model.SaveModelFile(App.activeFileName);
        }
    }
    internal void importFileFromS2K(object sender, EventArgs e)
    {
        if (App.initialized) 
        {
            if (!string.IsNullOrEmpty(App.activeFileName))
            {
                var res = MessageBox.Show(
                    "¿Esta seguro de continuar, perderá la información que no haya guardado?", "", MessageBoxButtons.YesNo);
                if (!(res == DialogResult.Yes || res == DialogResult.OK)) return;
            }
            CloseAllWindows();
        }
        DialogResult ofd;

        OpenFileDialog OpenFileDialog1 = new();
        OpenFileDialog1.DefaultExt = "SAP2000"; 
        OpenFileDialog1.Filter = "SAP2000 $2k|*.$2k"; 
        OpenFileDialog1.Title = "CEF3D - Importar..."; 
        App.initialized = false; 
        ofd = OpenFileDialog1.ShowDialog(); 
        if (ofd == DialogResult.OK | ofd == DialogResult.Yes)
        {
            App.model = new ModelDB(); 
            App.appData = new AppDB();
            var Sap2KFileName = OpenFileDialog1.FileName;
            App.mainWindow.Text = "**" + Sap2KFileName;
            App.currCoordSist = "Default";
            App. materialDefaultID = -1;
            App.sectionDefaultID = -1;
            App.currColorScheme = 2;
            deleteUndoFiles();
            deleteResultsFromDB();
            App.stopUndoRecording();
            var s2kp = new CEF.datamgr.s2kparser(App.model, Sap2KFileName);
            s2kp.ImprtS2kFile_ordeningTables();

            var sapCurrUnits = App.model.getParamText("CurrUnits").ToLower().Replace(" ", "").Split(",").ToArray();
            if (sapCurrUnits[0] == "lbf" || sapCurrUnits[0] == "kpi")
            {
                App.currUnits = string.Join(",", new[] { sapCurrUnits[0], "lb", sapCurrUnits[1], sapCurrUnits[2] });
            }
            else
            {
                App.currUnits = string.Join(",", new[] { sapCurrUnits[0], "kg", sapCurrUnits[1], sapCurrUnits[2] });
            }

            Uc.setUc(App.currUnits);
            App.mainWindow.tsUnitsSel.Text = App.currUnits;
            App.setParamProgramDefaults();
            App.startUndoRecording();
            App.initialized = true;
            space3d.createGrid(this);
            Uc.setUc(App.currUnits);
            var vas = new modelView(); 
            vas.Text = vas.Text + " [" + App.modelViewInstances + "]"+App.activeFileName; 
                vas.Show(this); 
        }
    }
    static void deleteUndoFiles()
    {
        App.restartUndo();
    }
    internal void exportFileToDxf(object sender, EventArgs e)
    {
        App.model.ExportarDXF(App.activeFileName.Replace(".cef", ".dxf"));
    }
    internal void undoLastAction(object sender, EventArgs e)
    {
        var file = App.popUndo();
        if (string.IsNullOrEmpty(file)) return;
        App.stopUndoRecording();
        App.model.LoadDumpFile(file);
        App.startUndoRecording();

    }





    
}