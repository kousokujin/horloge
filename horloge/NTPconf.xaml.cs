using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace horloge
{
    /// <summary>
    /// NTPconf.xaml の相互作用ロジック
    /// </summary>
    public partial class NTPconf : Window
    {
        public NTPconf()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void enableNTPCheck_Click(object sender, RoutedEventArgs e)
        {
            if(enableNTPCheck.IsChecked != null)
            {
                if(enableNTPCheck.IsChecked == true)
                {
                    serverCombo.IsEnabled = true;
                    getTimeButton.IsEnabled = true;
                }else
                {
                    serverCombo.IsEnabled = false;
                    getTimeButton.IsEnabled = false;
                }
            }
        }
    }
}
