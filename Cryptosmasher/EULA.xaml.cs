using System.Windows;

namespace Cryptosmasher
{
    /// <summary>
    /// Interaction logic for EULA.xaml
    /// </summary>
    public partial class EULA : Window
    {
        public EULA()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
