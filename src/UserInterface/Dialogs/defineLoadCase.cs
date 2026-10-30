using System;
using System.Data;
namespace CEF;
public  class defineLoadCase:Form
{


    internal Button Aceptar;
    internal Button Cancelar;
    internal DataGridView DataGridView1;
    internal Button BttAdd, BttDel;
    private string[] caseTypeNames;

    private void InitializeComponent()
    {


        DataGridView1 = new DataGridView();

        Aceptar = dlgStyle.button(I18n.DlgOKText,180,242,80,true);
        Aceptar.Click += Aceptar_Click;

        Cancelar = dlgStyle.button(I18n.DlgCancelText,260,242,80);
        CancelButton = Cancelar;
        Cancelar.Click += Cancelar_Click;

          
        BttAdd = dlgStyle.button(I18n.Add,20,242,80);        
        BttAdd.Click += BttAdd_Click;
        
        BttDel =dlgStyle.button(I18n.Delete,100,242,80);
        BttDel.Click += BttDel_Click;  
        
        DataGridView1 = new DataGridView() {
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                Location = new Point(31, 62),
                Margin = new Padding(4, 3, 4, 3),
                MultiSelect = false,
                Size = new Size(285, 173),                  
                
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                ColumnHeadersVisible = true,   // the group title already says which axis it is
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.FixedSingle,    
                ReadOnly = true,
            };
    
        ClientSize = new Size(348, 309);
        Controls.Add(BttAdd);
        Controls.Add(DataGridView1);
        Controls.Add(Cancelar);
        Controls.Add(Aceptar);
        Controls.Add(BttDel);

        Text = I18n.AddNewLoadCase;
        Load += NCasoC_Load;

        dlgStyle.applyForm(this);

    }


    public defineLoadCase()
    {
        InitializeComponent();
        caseTypeNames = Enum.GetNames<LoadCaseType>(); ;
    }
    private void NCasoC_Load(object sender, EventArgs e)
    {
        DataGridView1.Columns.Add("ID", "ID");
        DataGridView1.Columns.Add(I18n.Name, I18n.Name);
        DataGridView1.Columns.Add(I18n.Type, I18n.Type);
        NCasoC_Load(); }
    private void NCasoC_Load()
    {
        int indx = App.model.loadCases.Max( x => x.ID) + 1;

        loadCase cc;

        DataGridView1.Rows.Clear();
        for (int i = 0; i< App.model.loadCases.Count;  i++)
        {
            cc = App.model.loadCases[i];
            DataGridView1.Rows.Add(cc.ID, cc.Name, caseTypeNames[cc.Type]);
        }
    }
    private void Aceptar_Click(object sender, EventArgs e)
    {
        Close();
    }
    private void Cancelar_Click(object sender, EventArgs e)
    {
        Close();
    }
    private void BttAdd_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();
        int lcCount = App.model.loadCases.Count+1;

        var form = new Form() { StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedToolWindow,Width = 250, Height = 100 };
        var typeCombo = new ComboBox() { Left = 150, Top = 10, Width = 80 };
        typeCombo.Items.AddRange(caseTypeNames);
        typeCombo.SelectedIndex = 0;
        var tbName = new TextBox() { Left = 20, Top = 10, Width = 120 };
        tbName.Text = $"NewCaseName{lcCount}";

        var btNO = new Button() {Text = I18n.DlgCancelText, DialogResult= DialogResult.Cancel, Top = 35, Left = 80 };
        var btOK = new Button() {Text = I18n.DlgOKText, DialogResult= DialogResult.OK, Top = 35, Left = 150 };
        form.Controls.AddRange([typeCombo,tbName,btNO,btOK]);
        btNO.Click += (_, _) => form.Close();
        btOK.Click += (_, _) => {
        
            if (App.model.loadCases.Any(c=> c.Name == tbName.Text )) {
                
                MessageBox.Show($"{tbName.Text}");
                return;}
            var row = new loadCase
            {
                ID = App.model.loadCases.NextID(),
                Name = tbName.Text,
                Type = typeCombo.SelectedIndex,
            };
            App.model.loadCases.Add(row);
            NCasoC_Load();
            this.Refresh();
        };
        form.ShowDialog(this);

    }



    private void BttDel_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();
        

        var form = new Form()
        {
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedToolWindow,
            Width = 190,
            Height = 100
        };
        var caseCombo = new ComboBox() { Left = 30, Top = 10, Width = 120 };
        foreach(DataGridViewRow r in DataGridView1.Rows){
            var val = r.Cells[1].Value;//If .Value does not existe trouble
            if(val!=null)
            caseCombo.Items.Add(val.ToString());
        }
        caseCombo.SelectedIndex = 0;


        var btNO = new Button() { Text = I18n.DlgCancelText, DialogResult = DialogResult.Cancel, Top = 35, Left = 10 };
        var btOK = new Button() { Text = I18n.DlgOKText, DialogResult = DialogResult.OK, Top = 35, Left = 90 };
        form.Controls.AddRange([caseCombo, btNO, btOK]);
        btNO.Click += (_, _) => form.Close();
        btOK.Click += (_, _) => {
            var casoObj = App.model.loadCases.First(c=>c.Name == caseCombo.Text);
            var casoIndx = App.model.loadCases.FindIndexByID(casoObj.ID);
          if (App.model.barDistrLoads!=null && App.model.barDistrLoads.Any(c => c.Case == casoObj.ID)) return;
          if (App.model.barPunctLoads !=null && App.model.barPunctLoads.Any(c => c.Case == casoObj.ID)) return;
          if (App.model.nodalLoads !=null && App.model.nodalLoads.Any(c => c.Case == casoObj.ID)) return;
            // if (CEFglobal.bd.Cargas_Termicas_Barras.Any(c => c.Caso == casoObj.ID)) return;

            App.model.loadCases.RemoveAt(casoIndx);
            NCasoC_Load();
            this.Refresh();
        };
        form.ShowDialog(this);
    }
}