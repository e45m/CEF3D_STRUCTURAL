using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
namespace CEF;
public  class viewModelDBTable: Form
{
    private TabControl tabControl1;
    private int currIndexTab = 0;

    private readonly List<string> tableNames;

    private bool editable;
    public viewModelDBTable(List<string> Tables, bool Editable = true)
    {
        //InitializeComponent();
        tabControl1 = new TabControl();

        tabControl1.Alignment = TabAlignment.Bottom;
        tabControl1.Dock = DockStyle.Fill;
       // tabControl1.ItemSize = new Size(Width, Height-60);
        tabControl1.Location = new Point(0, 30);
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(905, 702);
        tabControl1.TabIndex = 4;
        tabControl1.KeyDown += tabControl1_KeyDown;
        // 

        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        ClientSize = new Size(905, 702);
        Controls.Add(tabControl1);
        Text = I18n.TableView;
        Load += VisorBD_Table_Load;
        tableNames = Tables;
        editable = Editable;

    }
    class TablaInfo
    {
        public List<object> Objetos = [];
        public List<FieldInfo[]> Campos = [];
    }
    private Dictionary<DataGridView, TablaInfo> tablas = [];
    private void VisorBD_Table_Load(object sender, EventArgs e)
    {
        CrearTablas(App.model);
    }


    void CrearTablas(object baseObj)
    {

        foreach (FieldInfo prop in baseObj.GetType().GetFields())
        {
            var propi = prop.GetValue(baseObj);
            if (propi == null) continue;
            var itemsProp = propi.GetType().GetProperty("Items");
            if (itemsProp == null) continue;
            var items = itemsProp.GetValue(propi) as IEnumerable<object>;
            if (items == null) continue;            
            if (!tableNames.Any(t => t == "*" || t == prop.Name)) continue;

            TabPage tab;
            DataGridView grid;





            if (!tabControl1.TabPages.ContainsKey(prop.Name))
            {
            if (I18n.translateTableNames.ContainsKey(prop.Name)) {
                tab = new TabPage(I18n.translateTableNames[prop.Name]) { Name = prop.Name };
            }else
            {
                tab = new TabPage(prop.Name) { Name = prop.Name };
            }
            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.CellEndEdit += Grid_CellEndEdit;
            grid.Name = prop.Name;
            tab.Controls.Add(grid);
            tabControl1.TabPages.Add(tab);
            } else
            {
                tab =(TabPage) tabControl1.Controls.Find(prop.Name, true)[0];
                grid = (DataGridView)tab.Controls.Find(prop.Name, true)[0];
                grid.Rows.Clear();
                grid.Columns.Clear();
            }
            var info = new TablaInfo();
            tablas[grid] = info;


            foreach (var item in items)
            {
                var flds = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                info.Campos.Add(flds);
                info.Objetos.Add(item);
            }
            if (info.Objetos.Count == 0) continue;
            var primera = info.Campos[0];
            foreach (var f in primera){
                
                if(!I18n.translateTableNames.TryGetValue(f.Name,  out string fName))
                     fName = f.Name;

                grid.Columns.Add(f.Name, fName);
            }

            for (int i = 0; i < info.Objetos.Count; i++)
            {
                var obj = info.Objetos[i];
                var flds = info.Campos[i];
                var valores = new object[flds.Length];
                for (int c = 0; c < flds.Length; c++)
                {
                    

                    if (parseMetadata.getEnumFieldDisplayValue(typeof(loadCase), Enum.GetNames<LoadCaseType>(), "Type",
                        flds[c], obj, ref valores[c])) continue;
                    if (parseMetadata.getEnumFieldDisplayValue(typeof(section), Enum.GetNames<sectType>(), "Type",
                        flds[c], obj, ref valores[c])) continue;
                    if (parseMetadata.getEnumFieldDisplayValue(typeof(material), Enum.GetNames<materialType>(), "Type",
                        flds[c], obj, ref valores[c])) continue;
                    if (parseMetadata.getEnumFieldDisplayValue(typeof(nodalConstraint), Enum.GetNames<MFCType>(), "Type",
                        flds[c], obj, ref valores[c])) continue;
                    valores[c] = flds[c].GetValue(obj);
                    parseMetadata.useUserUnits(ref valores[c], flds[c])  ;
                        
                }
                
                grid.Rows.Add(valores);
            }
        }

        tabControl1.SelectedIndex = currIndexTab;

    }
    private void Grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        
        if (!editable) return;
        var grid = sender as DataGridView;
        if (grid == null) return;
        var info = tablas[grid];
        if (e.RowIndex < 0 || e.RowIndex >= info.Objetos.Count) return;
        var obj = info.Objetos[e.RowIndex];
        var flds = info.Campos[e.RowIndex];
        var fld = flds[e.ColumnIndex];
        object txt = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
        if (txt == null) return;
        if ("ID" == fld.Name) return; 
        if (typeof(loadCase) == obj.GetType() && "Type" == fld.Name) return;//TODO: Function to use the string enum to user UI
        if (typeof(section) == obj.GetType() && "Type" == fld.Name) return; 
        if (typeof(material) == obj.GetType() && "Type" == fld.Name) return; 
        if (typeof(nodalConstraint) == obj.GetType() && "Type" == fld.Name) return;

        parseMetadata.useDefautlDBUnits(ref txt, fld);

        object convertido = Convert.ChangeType(txt, fld.FieldType);
        fld.SetValue(obj, convertido);
    }
    private void tabControl1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F5)
        {
            currIndexTab = tabControl1.SelectedIndex;
            CrearTablas(App.model);
            this.Refresh();
        }
    }
}