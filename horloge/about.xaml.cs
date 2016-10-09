using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// about.xaml の相互作用ロジック
    /// </summary>
    public partial class about : Window
    {
        public about()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void MITlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://opensource.org/licenses/mit-license.php");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var assm = Assembly.GetExecutingAssembly();

            var name = assm.GetName();

            version.Content = name.Version;
            //Console.WriteLine("{0} {1}", name.Name, name.Version);
        }
    }
}
