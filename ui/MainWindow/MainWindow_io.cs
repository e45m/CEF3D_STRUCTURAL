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


        SaveFileDialog SaveFileDialog1 = new();
        SaveFileDialog1.Title = "CEF3D - Guardar..."; 
        SaveFileDialog1.Filter = " cef | *.cef"; 
        SaveFileDialog1.AddExtension = true; 
        SaveFileDialog1.DefaultExt = "cef"; 
        var  r = SaveFileDialog1.ShowDialog(); 
        
        if (r == DialogResult.OK || r == DialogResult.Yes)
        {
        
                    
        deleteResultsFromDB();
        deleteUndoFiles();
        CloseAllWindows();
        Engine.clearModelDbs();
        App.activeFileName = SaveFileDialog1.FileName;
        App.mainWindow.Text = App.activeFileName;
        App.currCoordSist = "Default";
        App.areaSectionDefaultID = -1;
        App.barSectionDefaultID = -1;
        App.currColorScheme = 2;

        App.setDefaultsToDb();
        space3d.createGrid(Engine.getModel(),Engine.getAppData(),App.currCoordSist,ref App.g_gridXPointsCount,ref App.g_gridYPointsCount,ref App.g_gridZPointsCount);


        App.setParamProgramDefaults();
        Uc.setUc(App.currUnits);        
        
        var vas = new modelView(); 
        vas.Text = vas.Text + "[0]";
        vas.Show(this); 

        }
       
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
        if (r == DialogResult.OK || r == DialogResult.Yes)
        {
            Engine.clearModelDbs();             
            deleteResultsFromDB();
            deleteUndoFiles();
            CloseAllWindows();

            App.activeFileName = OpenFileDialog1.FileName; 
            App.mainWindow.Text = App.activeFileName;
            App.currCoordSist = "Default";
            App.areaSectionDefaultID = -1;
            App.barSectionDefaultID = -1;
            App.currColorScheme = 2;
            App.stopUndoRecording();
             App.setDefaultsToDb();
            Engine.getModel().LoadModelFile(App.activeFileName);
            App.startUndoRecording();
            App.initialized = true;
            space3d.createGrid(Engine.getModel(),Engine.getAppData(),App.currCoordSist,ref App.g_gridXPointsCount,ref App.g_gridYPointsCount,ref App.g_gridZPointsCount);

            App.currUnits = Engine.getModel().getParamText(paramDefaults.units);
            Uc.setUc(App.currUnits);
            var vas = new modelView(); 
            vas.Text = vas.Text + " [" + App.modelViewInstances + "]"+App.activeFileName; 
            vas.Show(App.mainWindow); 
            var isRan = Engine.getModel().getParamValue(paramDefaults.analysed);
            if(isRan==1)
             {
                enableDisable_calc(false);
                //Calc stations   
                Calculo.CalcBarInternalActions(Engine.getModel(),Engine.getAppData());

             }else
             {
                enableDisable_calc(true);
             }
        }
    }
    internal void closeApp(object sender, EventArgs e)
    {
        Close();
    }
    internal void saveFile(object sender, EventArgs e)
    {
        Engine.getModel().SaveModelFile(App.activeFileName);
    }
    internal void saveFileAs(object sender, EventArgs e)
    {
        DialogResult r;
        SaveFileDialog SaveFileDialog1 = new();
        SaveFileDialog1.DefaultExt = "CEF- Guardar..."; 
        SaveFileDialog1.Filter = "cef | *.cef"; 
        SaveFileDialog1.Title = "CEF3D"; 
        r = SaveFileDialog1.ShowDialog(); 
        if (r == DialogResult.OK || r == DialogResult.Yes)
        {
            App.activeFileName = SaveFileDialog1.FileName;
            App.mainWindow.Text = App.activeFileName;
            Engine.getModel().SaveModelFile(App.activeFileName);
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
        if (ofd == DialogResult.OK || ofd == DialogResult.Yes)
        {
            Engine.clearModelDbs();
            deleteResultsFromDB();
            deleteUndoFiles();
            CloseAllWindows();
            var Sap2KFileName = OpenFileDialog1.FileName;
            App.mainWindow.Text = "**" + Sap2KFileName;
            App.currCoordSist = "Default";
            App. areaSectionDefaultID = -1;
            App.barSectionDefaultID = -1;
            App.currColorScheme = 2;
            deleteUndoFiles();
            deleteResultsFromDB();
            App.stopUndoRecording();
            var s2kp = new CEF.datamgr.s2kparser(Engine.getModel(), Sap2KFileName);
            s2kp.ImprtS2kFile_ordeningTables();

                App.currUnits = string.Join(",", new[] { Uc.activeForceU.ToString(), 
                Uc.activeMassU.ToString(), Uc.activeLenU.ToString(), Uc.activeTempU.ToString() });
            

            Uc.setUc(App.currUnits);            
            App.setParamProgramDefaults();
            App.startUndoRecording();
            App.initialized = true;
            space3d.createGrid(Engine.getModel(),Engine.getAppData(),App.currCoordSist,ref App.g_gridXPointsCount,ref App.g_gridYPointsCount,ref App.g_gridZPointsCount);

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
        Engine.getModel().ExportarDXF(App.activeFileName.Replace(".cef", ".dxf"));
    }
    internal void undoLastAction(object sender, EventArgs e)
    {
        var file = App.popUndo();
        if (string.IsNullOrEmpty(file)) return;
        App.stopUndoRecording();
        Engine.getModel().LoadDumpFile(file);
        App.startUndoRecording();

    }





    
}