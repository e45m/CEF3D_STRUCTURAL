using System;
namespace CEF;
public class LicenceNotice:Form
{
const string stdText = @"Se concede el uso de este programa bajo los términos de la licencia MIT.
Efren Sandoval Mora (2026). Todos los derechos reservados.
Este programa se entrega 'tal cual', sin ninguna garantía de resultados o utilidad. 
Los resultados y su uso son responsabilidad exclusiva del usuario.
Contacto: ing.easm@gmail.com

---

Permission is hereby granted for the use of this program under the terms of the MIT License.
Efren Sandoval Mora (2026). All rights reserved.
This program is provided 'as is', without any warranty of results or usefulness. 
The results and its use are the sole responsibility of the user.
Contact: ing.easm@gmail.com";
	Label licenceLabel;

    private void InitializeComponent()
    {
       

        licenceLabel = new Label();

        licenceLabel.BackColor = SystemColors.Info;
        licenceLabel.Dock = DockStyle.Fill;
        licenceLabel.Enabled = true;
        
        licenceLabel.Location = new Point(0, 0);
        licenceLabel.Margin = new Padding(4, 3, 4, 3);
        //TextBox1.Multiline = true;
        licenceLabel.Size = new Size(684, 250);
        licenceLabel.TabIndex = 0;
        licenceLabel.Text = stdText;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackgroundImageLayout = ImageLayout.Stretch;
        ClientSize = new Size(684, 250);
        ControlBox = true;
        Controls.Add(licenceLabel);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = I18n.About;
        TransparencyKey = Color.Silver;
        Activated += reponerTexto; 
        
        Load += reponerTexto;
        
    }

    public LicenceNotice()
    {
        InitializeComponent();
    }


    private void reponerTexto(object s,EventArgs e)
    {
        licenceLabel.Text = stdText;



    }


}