
namespace CEF;
public  class viewInternalBarActions:Form
{




    internal Label lblElID,lblElemento, lblElement, lblCase, lblMin, lblMax, lbl_J, lbl_I, lblElType, lblNudos, lblLength, lblAction;
    internal TextBox EleTipo, EleNudos, EleLong,EleId, minVal,maxVal, valJ, valI;
    internal ComboBox cb_selElem, loadCase, cb_internalAction;
    internal Button exit;
    internal PictureBox pbDiagramCanvas;
    internal GroupBox gbElData;

    private Label addLabel(string text, int x, int y) =>
     new Label { Text = text, Location = new Point(x, y), AutoSize = true };

    private TextBox addTextBox(int x, int y, int w = 140, bool enabled = false) =>
        new TextBox { Location = new Point(x, y), Size = new Size(w, 23), Enabled = enabled };

    private ComboBox addComboBox(int x, int y, int w = 143, bool sorted = false, EventHandler ev = null)
    {
        var cb = new ComboBox
        {
            Location = new Point(x, y),
            Size = new Size(w, 23),
            Sorted = sorted,
            FormattingEnabled = true
        };
        if (ev != null) cb.SelectedIndexChanged += ev;
        return cb;
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // === Combos principales ===
        cb_selElem = addComboBox(477, 30, ev: Selccion_SelectedIndexChanged);
        loadCase = addComboBox(477, 61, sorted: true, ev: Caso_SelectedIndexChanged);
        cb_internalAction = addComboBox(477, 90, sorted: true);

        lblElement = addLabel("Elemento:", 356, 33);
        lblCase = addLabel("Caso/Combinacion", 356, 64);
        lblAction = addLabel("Acción", 356, 94);

        // === GroupBox ===
        gbElData = new GroupBox
        {
            Text = "Resumen del elemento",
            Location = new Point(26, 14),
            Size = new Size(292, 201)
        };

        gbElData.Controls.AddRange(new Control[]
        {
        addLabel("Elemento:", 20, 24),
        addLabel("Id", 24, 46),
        addLabel("Tipo", 24, 76),
        addLabel("Longitud", 24, 106),
        addLabel("Nudos:", 24, 136),

        EleId    = addTextBox(86, 42),
        EleTipo  = addTextBox(86, 72),
        EleLong  = addTextBox(86, 102),
        EleNudos = addTextBox(86, 132)
        });

        // === Canvas ===
        pbDiagramCanvas = new PictureBox
        {
            BackColor = Color.LightYellow,
            Location = new Point(13, 248),
            Size = new Size(627, 256)
        };

        // === Valores derecha ===
        lbl_I = addLabel(I18n.ValueAtStartNode, 684, 255);
        lbl_J = addLabel(I18n.ValueAtEndNode, 684, 284);
        lblMax = addLabel(I18n.max, 677, 323);
        lblMin = addLabel(I18n.min, 677, 352);

        valI = addTextBox(710, 248, 104);
        valJ = addTextBox(710, 281, 104);
        maxVal = addTextBox(710, 316, 104);
        minVal = addTextBox(710, 349, 104);

        // === Botón ===
        exit = new Button
        {
            Text = I18n.Exit,
            Location = new Point(725, 481),
            Size = new Size(114, 23),
            DialogResult = DialogResult.Cancel
        };
        exit.Click += Salir_Click;

        // === Form ===
        ClientSize = new Size(869, 547);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Text = I18n.BarInternalForces;
        CancelButton = exit;

        Controls.AddRange(new Control[]
        {
        cb_selElem, lblElement,
        loadCase, lblCase,
        cb_internalAction, lblAction,
        gbElData,
        pbDiagramCanvas,
        lbl_I, lbl_J, lblMax, lblMin,
        valI, valJ, maxVal, minVal,
        exit
        });

        Load += EsfuerzosEnSeccion_Load;

        ResumeLayout(false);
        PerformLayout();
    }




    public static int CifDec = 9;
    private int[] s;
    public viewInternalBarActions()
    {
        InitializeComponent();
;

    }
    private void Salir_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void EsfuerzosEnSeccion_Load(object sender, EventArgs e)
    {
    }
    private void Selccion_SelectedIndexChanged(object sender, EventArgs e)
    {
      
        CalculaDiagramas();
    }
    private void CalculaDiagramas()
    {
    }
    private void DibujaDiagrama(double[] Datos, double L, ref PictureBox PB)
    {
       
    }
    private void TBC_contienDiagramas_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selccion_SelectedIndexChanged(sender, e);
    }
    private void Caso_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selccion_SelectedIndexChanged(sender, e);
    }
}