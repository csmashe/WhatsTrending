using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cryptosmasher
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void CopyToClipBoard(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBlock tb)) return;
            Clipboard.SetText(tb.Text);
            MessageBox.Show("Copied to clipboard!");
        }

        private void ClickPaypal(object sender,MouseButtonEventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=LQCGXKN53BHZS");
        }
    }
}
