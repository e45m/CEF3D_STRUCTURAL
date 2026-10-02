using System.ComponentModel;

namespace CEF;
public partial class SetPropWindow 
{
    private System.Windows.Forms.PropertyGrid pGrid;
    private System.Windows.Forms.Form m_Form;

    public SetPropWindow()
    {
        SetUp();
    }


    object _p;
    public void setProperty( object Property)
    {
        _p = Property;
        pGrid.SelectedObject = null;
        pGrid.SelectedObject = _p;
        pGrid.Refresh();
    }

    public object getProperty()
    {
        return _p;
    }
    
    private void load(object sender, EventArgs e)
    {
        pGrid.SelectedObject = _p;
        pGrid.Refresh();
    }
    internal void RefreshPGrid()
    {
        pGrid.Refresh();
    }

    private void SetUp()
    {
        m_Form = new Form();
        pGrid = new PropertyGrid();
       
        // 
        // pGrid
        // 
        pGrid.BackColor = SystemColors.Control;
        pGrid.Dock = DockStyle.Fill;
        pGrid.Location = new Point(0, 0);
        pGrid.Margin = new Padding(4, 3, 4, 3);
        pGrid.Name = "pGrid";
        pGrid.Size = new Size(357, 721);
        pGrid.TabIndex = 0;
        //  
        // SetPropWindow
        // 
        m_Form.AutoScaleDimensions = new SizeF(7F, 15F);
        m_Form.AutoScaleMode = AutoScaleMode.Font;
        m_Form.ClientSize = new Size(357, 721);
        m_Form.Controls.Add(pGrid);
        m_Form.Margin = new Padding(4, 3, 4, 3);
        m_Form.MaximizeBox = false;
        m_Form.MinimizeBox = false;
        m_Form.Name = "SetPropWindow";
        m_Form.ShowInTaskbar = false;
        m_Form.Text = "SetPropWindow";
        m_Form.Load += load;
        
    }
    internal void Show(IWin32Window own=null)
    {
        m_Form.Show(own);
    }
    internal void ShowDialog(IWin32Window own = null)
    {
        m_Form.ShowDialog(own);
    }
    internal bool Created=>    m_Form.Created;
    
    internal void Refresh()
    {
        m_Form.Refresh();

    }
}