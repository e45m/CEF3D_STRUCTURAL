using System;
namespace CEF;
public class setCurrentLoadCase : Form
{
    #region UI
    internal ComboBox ComboBox1;
    internal Button Button2;
    internal Button Button1;
    private void InitializeComponent()
    {
        ComboBox1 = new ComboBox();
        Button2 = new Button();
        Button1 = new Button();
        SuspendLayout();
        // 
        // ComboBox1
        // 
        ComboBox1.FormattingEnabled = true;
        ComboBox1.Location = new Point(44, 24);
        ComboBox1.Margin = new Padding(4, 3, 4, 3);
        ComboBox1.Size = new Size(294, 23);
        ComboBox1.TabIndex = 0;
        // 
        // Button2
        // 
        Button2.Location = new Point(158, 67);
        Button2.Margin = new Padding(4, 3, 4, 3);
        Button2.Size = new Size(88, 25);
        Button2.TabIndex = 4;
        Button2.Text = I18n.DlgOKText;
        Button2.UseVisualStyleBackColor = true;
        Button2.Click += Button2_Click;
        // 
        // Button1
        // 
        Button1.Location = new Point(252, 67);
        Button1.Margin = new Padding(4, 3, 4, 3);
        Button1.Size = new Size(88, 25);
        Button1.TabIndex = 3;
        Button1.Text = I18n.DlgCancelText;
        Button1.UseVisualStyleBackColor = true;
        Button1.Click += Button1_Click;
        // 
        // AsignaCasoCargaActivo
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(371, 114);
        Controls.Add(Button2);
        Controls.Add(Button1);
        Controls.Add(ComboBox1);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Text = I18n.SelectActiveCase;
        Load += AsignaCasoCargaActivo_Load;
        ResumeLayout(false);
    }


    #endregion
    public setCurrentLoadCase()
    {
        InitializeComponent();
    }
    private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    private void Button2_Click(object sender, EventArgs e)
    {
        App.model.setParamValue(Mdta.ActiveCase, ComboBox1.SelectedIndex);
        Close();
    }
    private void Button1_Click(object sender, EventArgs e)
    {
        Close();
    }
    private void AsignaCasoCargaActivo_Load(object sender, EventArgs e)
    {
    }
}