using System.Windows;

namespace inventory_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            pnlMainGrid.MouseUp += new System.Windows.Input.MouseButtonEventHandler(pnlMainGrid_MouseUp);
            
        }

        private void pnlMainGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Main Grid clicked! " + e.GetPosition(this).ToString());
            
        }

        private void pnlMainGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            
        }
    }
}