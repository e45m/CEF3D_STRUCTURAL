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

        Aceptar = new Button();
        Cancelar = new Button();
        DataGridView1 = new DataGridView();
        BttAdd = new Button();
        BttDel = new Button();
        ((System.ComponentModel.ISupportInitialize)DataGridView1).BeginInit();
        SuspendLayout();
        // 
        // Aceptar
        // 
        Aceptar.Location = new Point(180, 242);
        Aceptar.Margin = new Padding(4, 3, 4, 3);
        Aceptar.Size = new Size(80, 25);
        Aceptar.TabIndex = 0;
        Aceptar.Text = I18n.DlgOKText;
        Aceptar.UseVisualStyleBackColor = true;
        Aceptar.Click += Aceptar_Click;
        // 
        // Cancelar
        // 
        Cancelar.DialogResult = DialogResult.Cancel;
        Cancelar.Location = new Point(260, 242);
        Cancelar.Margin = new Padding(4, 3, 4, 3);
        Cancelar.Size = new Size(80, 25);
        Cancelar.TabIndex = 1;
        Cancelar.Text = I18n.DlgCancelText;
        Cancelar.UseVisualStyleBackColor = true;
        Cancelar.Click += Cancelar_Click;
 
        // 
        // DataGridView1
        // 
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        DataGridView1.Location = new Point(31, 62);
        DataGridView1.Margin = new Padding(4, 3, 4, 3);
        DataGridView1.MultiSelect = false;
        DataGridView1.Size = new Size(285, 173);
        DataGridView1.Enabled = false;
        DataGridView1.TabIndex = 5;
        // 
        // BttAdd
        // 
        BttAdd.Location = new Point(20, 242);
        BttAdd.Margin = new Padding(4, 3, 4, 3);
        BttAdd.Size = new Size(80, 25);
        BttAdd.TabIndex = 6;
        BttAdd.Text = I18n.Add;
        BttAdd.UseVisualStyleBackColor = true;
        BttAdd.Click += BttAdd_Click;

        BttDel.Location = new Point(100, 242);
        BttDel.Margin = new Padding(4, 3, 4, 3);
        BttDel.Size = new Size(80, 25);
        BttDel.TabIndex = 6;
        BttDel.Text = I18n.Delete;
        BttDel.UseVisualStyleBackColor = true;
        BttDel.Click += BttDel_Click; ;
        // 
        // NCasoC
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AcceptButton = Aceptar;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = Cancelar;
        ClientSize = new Size(348, 309);
        Controls.Add(BttAdd);
        Controls.Add(DataGridView1);
        Controls.Add(Cancelar);
        Controls.Add(Aceptar);
        Controls.Add(BttDel);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Text = I18n.AddNewLoadCase;
        Load += NCasoC_Load;
        ((System.ComponentModel.ISupportInitialize)DataGridView1).EndInit();
        ResumeLayout(false);
        PerformLayout();
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
        int i = App.model.loadCases.Max(c=>c.ID)+1;

        var form = new Form() { StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedToolWindow,Width = 250, Height = 100 };
        var typeCombo = new ComboBox() { Left = 150, Top = 10, Width = 80 };
        typeCombo.Items.AddRange(caseTypeNames);
        typeCombo.SelectedIndex = 0;
        var tbName = new TextBox() { Left = 20, Top = 10, Width = 120 };
        tbName.Text = $"NewCaseName{i}";

        var btNO = new Button() {Text = I18n.DlgOKText, DialogResult= DialogResult.Cancel, Top = 35, Left = 80 };
        var btOK = new Button() {Text = I18n.DlgCancelText, DialogResult= DialogResult.OK, Top = 35, Left = 150 };
        form.Controls.AddRange([typeCombo,tbName,btNO,btOK]);
        btNO.Click += (_, _) => form.Close();
        btOK.Click += (_, _) => {
        
            if (App.model.loadCases.Any(c=> c.Name == tbName.Text )) return;
            var row = new loadCase
            {
                ID = i,
                Name = tbName.Text,
                Type = typeCombo.SelectedIndex,
            };
            App.model.loadCases.Add(row);
            NCasoC_Load();
            this.Refresh();
        };
        form.ShowDialog(this);

    }



    private void BttDel_Click(object? sender, EventArgs e)
    {
        App.takeUndoSnapShot();
        int i = App.model.loadCases.Max(c => c.ID) + 1;

        var form = new Form()
        {
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedToolWindow,
            Width = 190,
            Height = 100
        };
        var caseCombo = new ComboBox() { Left = 30, Top = 10, Width = 120 };
        foreach(DataGridViewRow r in DataGridView1.Rows){
            var val = r.Cells[1].Value?.ToString() ?? null;
            if(val!=null)
            caseCombo.Items.Add(val);
        }
        caseCombo.SelectedIndex = 0;


        var btNO = new Button() { Text = I18n.DlgOKText, DialogResult = DialogResult.Cancel, Top = 35, Left = 10 };
        var btOK = new Button() { Text = I18n.DlgCancelText, DialogResult = DialogResult.OK, Top = 35, Left = 90 };
        form.Controls.AddRange([caseCombo, btNO, btOK]);
        btNO.Click += (_, _) => form.Close();
        btOK.Click += (_, _) => {


            var casoObj = App.model.loadCases.First(c=>c.Name == caseCombo.Text);
            var casoIndx = App.model.loadCases.FindIndexByID(casoObj.ID);
          if (App.model.barDistrLoads.Any(c => c.Case == casoObj.ID)) return;
          if (App.model.barPunctLoads.Any(c => c.Case == casoObj.ID)) return;
          if (App.model.nodalLoads.Any(c => c.Case == casoObj.ID)) return;
            // if (CEFglobal.bd.Cargas_Termicas_Barras.Any(c => c.Caso == casoObj.ID)) return;

            App.model.loadCases.RemoveAt(casoIndx);
            NCasoC_Load();
            this.Refresh();
        };
        form.ShowDialog(this);
    }
}