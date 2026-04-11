using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using System.Xml.Linq;
namespace CEF;
public  class defineNewCoordSys:Form
{

     Button btOK, btCancel, btRemove;
     PictureBox Imagen;
     DataGridView GrillaX;
     DataGridView GrillaY;
     DataGridView GrillaZ;
    
     Label Label2, Label3, Label4;
     internal ComboBox cbSCName;
    Button spaceX, spaceY, spaceZ;
    private void InitializeComponent()
    {
        btOK = new Button();
        btCancel = new Button();
        btRemove = new Button();
        Imagen = new PictureBox();
        GrillaX = new DataGridView();
        GrillaY = new DataGridView();
        GrillaZ = new DataGridView();
        cbSCName = new ComboBox();
        Label2 = new Label();
        Label3 = new Label();
        Label4 = new Label();
        spaceX = new Button();
        spaceY = new Button();
        spaceZ = new Button();
        ((System.ComponentModel.ISupportInitialize)Imagen).BeginInit();
        ((System.ComponentModel.ISupportInitialize)GrillaX).BeginInit();
        ((System.ComponentModel.ISupportInitialize)GrillaY).BeginInit();
        ((System.ComponentModel.ISupportInitialize)GrillaZ).BeginInit();
        SuspendLayout();
        // 
        // btOK
        // 
        btOK.Anchor = AnchorStyles.None;
        btOK.Location = new Point(367, 324);
        btOK.Margin = new Padding(4, 3, 4, 3);
        btOK.Size = new Size(77, 27);
        btOK.TabIndex = 0;
        btOK.Text = I18n.DlgOKText;
        btOK.Click += OK_Button_Click;
        // 
        // btCancel
        // 
        btCancel.Anchor = AnchorStyles.None;
        btCancel.DialogResult = DialogResult.Cancel;
        btCancel.Location = new Point(286, 324);
        btCancel.Margin = new Padding(4, 3, 4, 3);
        btCancel.Size = new Size(77, 27);
        btCancel.TabIndex = 1;
        btCancel.Text = I18n.DlgCancelText;
        btCancel.Click += Cancel_Button_Click;
        // 
        // btRemove
        // 
        btRemove.Anchor = AnchorStyles.None;
        btRemove.Location = new Point(18, 324);
        btRemove.Margin = new Padding(4, 3, 4, 3);
        btRemove.Size = new Size(77, 27);
        btRemove.TabIndex = 0;
        btRemove.Text =I18n.Delete;
        btRemove.Click += BtRemove;
        // 
        // Imagen
        // 
        Imagen.Image = Properties.Resources.Ejes;
        Imagen.Location = new Point(18, 12);
        Imagen.Margin = new Padding(4, 3, 4, 3);
        Imagen.Padding = new Padding(4, 3, 4, 3);
        Imagen.Size = new Size(88, 67);
        Imagen.SizeMode = PictureBoxSizeMode.StretchImage;
        Imagen.TabIndex = 0;
        Imagen.TabStop = false;
        // 
        // GrillaX
        // 
        GrillaX.Location = new Point(10, 115);
        GrillaX.Margin = new Padding(4, 3, 4, 3);
        GrillaX.Name = "X_Grid";
        GrillaX.Size = new Size(137, 150);
        GrillaX.TabIndex = 2;
        GrillaX.CellEndEdit += Grilla_CurrentCellChanged;
        // 
        // GrillaY
        // 
        GrillaY.Location = new Point(157, 115);
        GrillaY.Margin = new Padding(4, 3, 4, 3);
        GrillaY.Name = "Y_Grid";
        GrillaY.Size = new Size(137, 150);
        GrillaY.TabIndex = 2;
        GrillaY.CellEndEdit += Grilla_CurrentCellChanged;
        // 
        // GrillaZ
        // 
        GrillaZ.Location = new Point(304, 115);
        GrillaZ.Margin = new Padding(4, 3, 4, 3);
        GrillaZ.Name = "Z_Grid";
        GrillaZ.Size = new Size(137, 150);
        GrillaZ.TabIndex = 2;
        GrillaZ.CellEndEdit += Grilla_CurrentCellChanged;
        // 
        // cbSCName
        // 
        cbSCName.Location = new Point(178, 56);
        cbSCName.Margin = new Padding(4, 3, 4, 3);
        cbSCName.Name = "cbSCName";
        cbSCName.Size = new Size(251, 23);
        cbSCName.TabIndex = 7;
        cbSCName.SelectedIndexChanged += CbSCNameChanged;
  
        // 
        // Label2
        // 
        Label2.AutoSize = true;
        Label2.Location = new Point(55, 90);
        Label2.Margin = new Padding(4, 0, 4, 0);
        Label2.Name = "Label2";
        Label2.Size = new Size(14, 15);
        Label2.TabIndex = 6;
        Label2.Text = "X";
        // 
        // Label3
        // 
        Label3.AutoSize = true;
        Label3.Location = new Point(202, 90);
        Label3.Margin = new Padding(4, 0, 4, 0);
        Label3.Name = "Label3";
        Label3.Size = new Size(14, 15);
        Label3.TabIndex = 6;
        Label3.Text = "Y";
        // 
        // Label4
        // 
        Label4.AutoSize = true;
        Label4.Location = new Point(349, 90);
        Label4.Margin = new Padding(4, 0, 4, 0);
        Label4.Name = "Label4";
        Label4.Size = new Size(14, 15);
        Label4.TabIndex = 6;
        Label4.Text = "Z";
        // 
        // spaceX
        // 
        spaceX.Location = new Point(10, 270);
        spaceX.Margin = new Padding(4, 3, 4, 3);
        spaceX.Name = "spaceX";
        spaceX.Size = new Size(137, 25);
        spaceX.TabIndex = 7;
        spaceX.Text = I18n.BatchImput;
        spaceX.Click += Space_Click;
        // 
        // spaceY
        // 
        spaceY.Location = new Point(157, 270);
        spaceY.Margin = new Padding(4, 3, 4, 3);
        spaceY.Name = "spaceY";
        spaceY.Size = new Size(137, 25);
        spaceY.TabIndex = 7;
        spaceY.Text = I18n.BatchImput; 
        spaceY.Click += Space_Click;
        // 
        // spaceZ
        // 
        spaceZ.Location = new Point(304, 270);
        spaceZ.Margin = new Padding(4, 3, 4, 3);
        spaceZ.Name = "spaceZ";
        spaceZ.Size = new Size(137, 25);
        spaceZ.TabIndex = 7;
        spaceZ.Text = I18n.BatchImput;
        spaceZ.Click += Space_Click;
        // 
        // defineNewCoordSys
        // 
        AcceptButton = btOK;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btCancel;
        ClientSize = new Size(450, 360);
        Controls.Add(spaceX);
        Controls.Add(spaceY);
        Controls.Add(spaceZ);
        Controls.Add(GrillaX);
        Controls.Add(GrillaY);
        Controls.Add(GrillaZ);
        Controls.Add(Imagen);
        Controls.Add(btCancel);
        Controls.Add(btOK);
        Controls.Add(btRemove);
        Controls.Add(cbSCName);
        Controls.Add(Label2);
        Controls.Add(Label3);
        Controls.Add(Label4);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "defineNewCoordSys";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = I18n.coordSys;
        TopMost = true;
        Load += CrearNuevo_Load;
        ((System.ComponentModel.ISupportInitialize)Imagen).EndInit();
        ((System.ComponentModel.ISupportInitialize)GrillaX).EndInit();
        ((System.ComponentModel.ISupportInitialize)GrillaY).EndInit();
        ((System.ComponentModel.ISupportInitialize)GrillaZ).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private string nuevosist;
    private CEF.MainWindow _parent = null;
    DataSet CoordSisDataset;

    private void BtRemove(object? sender, EventArgs e)
    {
        
        if (nuevosist == "Default") return;
        if (App.model.gridCoordsX.Count == 0) return;
        if (App.model.gridCoordsX.Select(x => x.coordSys).Distinct().Count()<2) return;
        
        removeSC(nuevosist);
        space3d.createGrid(_parent);
        DialogResult = DialogResult.OK;
        Close();
    }
      
    private void OK_Button_Click(object sender, EventArgs e)
    {
        guardar_datos_grilla();
        space3d.createGrid(_parent);
        DialogResult = DialogResult.OK;
        Close();
    }
    private void Cancel_Button_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }


    public void removeSC(string name)
    {
        var cX = App.model.gridCoordsX.Where(c => c.coordSys != name).ToList();
        var cY = App.model.gridCoordsY.Where(c => c.coordSys != name).ToList();
        var cZ = App.model.gridCoordsZ.Where(c => c.coordSys != name).ToList();

        App.model.gridCoordsX.Clear();
        App.model.gridCoordsY.Clear();
        App.model.gridCoordsZ.Clear();

        foreach (var c in cX)
            App.model.gridCoordsX.Add(c);
        foreach (var c in cY)
            App.model.gridCoordsY.Add(c);
        foreach (var c in cZ)
            App.model.gridCoordsZ.Add(c);

    }
    public void guardar_datos_grilla()
    {
        int i,cgx,cgy,cgz;
        nuevosist = cbSCName.Text;
        removeSC(nuevosist);
        cgx = CoordSisDataset.Tables["X_Grid"].Rows.Count;     
        cgy = CoordSisDataset.Tables["Y_Grid"].Rows.Count;
        cgz = CoordSisDataset.Tables["Z_Grid"].Rows.Count;

        for (i = 0; i < cgx; i++)        
            App.model.gridCoordsX.Add(new coordinate() { ID = App.model.gridCoordsX.Count - 1 ,
                Coord = (double)(CoordSisDataset.Tables["X_Grid"].Rows[i]["Coord"])
            ,coordSys = nuevosist
            });
        
        for (i = 0; i < cgy; i++)        
            App.model.gridCoordsY.Add(new coordinate() { ID = App.model.gridCoordsY.Count - 1,
                Coord = (double)(CoordSisDataset.Tables["Y_Grid"].Rows[i]["Coord"]),
                coordSys = nuevosist
            });
                
        for (i = 0; i < cgz; i++)
             App.model.gridCoordsZ.Add(new coordinate() {  ID = App.model.gridCoordsX.Count - 1,
                Coord = (double)(CoordSisDataset.Tables["Z_Grid"].Rows[i]["Coord"]) ,
                coordSys = nuevosist
            });        
    }

    private void CrearNuevo_Load(object sender, EventArgs e)
    {
        CoordSisDataset = new DataSet();
        CoordSisDataset.DataSetName = "CoordSisDataset";
        DataTable scTable;
        scTable = new DataTable("X_Grid");
        scTable.Columns.Add("Coord", typeof(double));    
        scTable.Columns["Coord"].Unique = true;

        CoordSisDataset.Tables.Add(scTable);

        scTable = new DataTable("Y_Grid");
        scTable.Columns.Add("Coord", typeof(double));        
        scTable.Columns["Coord"].Unique = true;

        CoordSisDataset.Tables.Add(scTable);
        scTable = new DataTable("Z_Grid");
        scTable.Columns.Add("Coord", typeof(double));
        scTable.Columns["Coord"].Unique = true;
        
        CoordSisDataset.Tables.Add(scTable);

        GrillaX.DataSource = CoordSisDataset;        
        GrillaX.DataMember = "X_Grid"; 
        GrillaY.DataSource = CoordSisDataset;
        GrillaY.DataMember = "Y_Grid"; 
        GrillaZ.DataSource = CoordSisDataset;
        GrillaZ.DataMember = "Z_Grid";


        int i;
        string dumm = cbSCName.Text;
        if(string.IsNullOrEmpty(dumm))
        dumm = App.model.gridCoordsX[0].coordSys;

        cbSCName.Items.Clear();
        cbSCName.Items.Add(dumm);


        var scs = App.model.gridCoordsX.
            Select(c => c.coordSys).Distinct().ToArray();

        cbSCName.Items.Clear();
        cbSCName.Items.AddRange(scs);
        cbSCName.SelectedItem = 0;
        nuevosist = cbSCName.Text;
        CbSCNameChanged(sender, e);
       
    }
    private void Grilla_CurrentCellChanged(object sender, EventArgs e)
    {
  
       switch (((DataGridView)sender).Name)
        {
            case "X_Grid":
                sortColumn("X_Grid");
                break;
            case "Y_Grid":
                sortColumn("Y_Grid");
                break;
            case "Z_Grid":
                sortColumn("Z_Grid");
                break;
        }

       
    }

    private void CbSCNameChanged(object? sender, EventArgs e)
    {
       
        nuevosist = cbSCName.Text;

        var filteredCX = App.model.gridCoordsX.Where(c => c.coordSys == nuevosist).ToArray();
        var filteredCY = App.model.gridCoordsY.Where(c => c.coordSys == nuevosist).ToArray();
        var filteredCZ = App.model.gridCoordsZ.Where(c => c.coordSys == nuevosist).ToArray();
       
        CoordSisDataset.Tables["X_Grid"].Clear();
        CoordSisDataset.Tables["Y_Grid"].Clear();
        CoordSisDataset.Tables["Z_Grid"].Clear();
        foreach (var i in filteredCX)
            CoordSisDataset.Tables["X_Grid"].Rows.Add(i.Coord);

        foreach (var i in filteredCY)
            CoordSisDataset.Tables["Y_Grid"].Rows.Add(i.Coord);

        foreach (var i in filteredCZ)
            CoordSisDataset.Tables["Z_Grid"].Rows.Add(i.Coord);
        

    }

    private void sortColumn(string column)
    {

        int i;
        int N = CoordSisDataset.Tables[column].Rows.Count;
        var D = new double[N];
        CoordSisDataset.Tables[column].Columns["Coord"].Unique = false;
        for (i = 0; i < N; i++)
            D[i] = double.TryParse(CoordSisDataset.Tables[column].Rows[i][0].ToString(),out double r)?r:double.NegativeInfinity;

        CoordSisDataset.Tables[column].Rows.Clear();
        Array.Sort(D);
        for (i = 0; i < N; i++)
        {   if (D[i] == double.NegativeInfinity) continue;
            CoordSisDataset.Tables[column].Rows.Add(D[i]);
        }
        CoordSisDataset.Tables[column].Columns["Coord"].Unique = true;

    }

    private void setColumn(string column, double[] D)
    {
        int i;
        int N = D.Length;
        int seek = CoordSisDataset.Tables[column].Rows.Count;
        if(seek>0)
        {
           double sumStart = (double)(CoordSisDataset.Tables[column].Rows[seek-1][0]);
            D = D.Select(d => d + sumStart).Distinct().ToArray();
        }
        if (seek == 0d) D = new double[] { 0d }.Concat(D).Distinct().ToArray();

        CoordSisDataset.Tables[column].Columns["Coord"].Unique = false;
        Array.Sort(D);
        for (i = seek; i < (N+seek); i++)
            CoordSisDataset.Tables[column].Rows.Add(D[i-seek]);

        CoordSisDataset.Tables[column].Columns["Coord"].Unique = true;
    }

    public defineNewCoordSys(CEF.MainWindow _Parent)
    {
        InitializeComponent();
        _parent = _Parent;
        if (!App.initialized)
        {
            nuevosist = "Default";
            cbSCName.Text = nuevosist;
            App.initialized = true;
        }
        else if (string.IsNullOrEmpty(cbSCName.Text))
        {
            nuevosist = "NCS";
        }
        cbSCName.Text = nuevosist;
    }

    private void Space_Click(object? sender, EventArgs e)
    {
        var s = (Button)sender;
        var name = s.Name;

        double[] spaces;

        int fW = 300, fH = 150, lTop = 5, lLfet = 30;
        using (var imputDlg = new Form() { Width = fW, Height = fH, FormBorderStyle = FormBorderStyle.FixedToolWindow })
        {
            var lblCenter = new Label() { Location = new Point(lLfet, lTop), Width = 200, Text = I18n.ImputSpacing };
            var lblCenter2 = new Label() { Location = new Point(lLfet, lTop + 25), Width = 200, Text = I18n.ImputSpacingExample };
            var lblX = new Label() { Location = new Point(lLfet, lTop + 50), Width = 100, Text = I18n.Spacing };
            var tbspacing = new TextBox() { Location = new Point(lLfet + 110, lTop + 50), Width = 140,Height = 30, Text = "0" };
            var bttOk = new Button() { Location = new Point(lLfet+180, lTop + 75), Width = 60, Text = I18n.DlgOKText, DialogResult = DialogResult.OK };
            var bttCancel = new Button() { Location = new Point(lLfet + 110, lTop + 75), Width = 60, Text = I18n.DlgCancelText, DialogResult = DialogResult.Cancel };
         
            imputDlg.Controls.AddRange(new Control[] { lblCenter, lblCenter2, lblX, tbspacing, bttOk, bttCancel });

            imputDlg.Load += ((object s, EventArgs e) =>
            {
                int _Left = ((Form)s).Owner.Left;
                int _with = ((Form)s).Owner.Width;
                int _top  = ((Form)s).Owner.Top ;
                ((Form)s).Owner.OwnedForms[0].Location = new Point(_Left + _with, _top);
            });
            imputDlg.ShowDialog(this);
            
            if (imputDlg.DialogResult == DialogResult.Cancel) return;
            spaces = tbspacing.Text.Split(I18n.SplitChar).SelectMany(i =>
            {   
                double spaceVal = 0.0;
                double[] spacesArray;
                int times = 1;

                if (i.Contains(I18n.GroupingKey))
                {
                    var s = i.Split(I18n.GroupingKey);
                    times = int.Parse(s[0]);
                    i = s[1].ToString();
                }

                if (double.TryParse(i, out spaceVal))
                {
                    spacesArray = new double[times];
                    Array.Fill(spacesArray, spaceVal);
                    return spacesArray;
                }
                return new double[] { 0.0 };
            }).ToArray();

            var suma = 0d;
            spaces = spaces.Select(s => suma += s).ToArray();
            if (spaces.Any(x => x < 0)) return;// spacing cannot be negative  
        }

        if (spaces.Length < 2) return;

        switch (name)
        {
            case "spaceX":
                setColumn("X_Grid", spaces);
                break;
            case "spaceY":
                setColumn("Y_Grid", spaces);
                break;
            case "spaceZ":
                setColumn("Z_Grid", spaces);
                break;


        }

    }


}