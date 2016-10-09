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
        public bool enableNTP; //NTP
        public string ntpServer;   //NTPサーバ

        MainWindow window;

        public NTPconf(MainWindow mw)
        {
            window = mw;

            InitializeComponent();
        }


        private void drawWindow()
        {
            enableNTP = window.time.getEnableNTP();
            ntpServer = window.time.getServerAddress();

            enableNTPCheck.IsChecked = enableNTP;
            serverCombo.Text = ntpServer;

            if (enableNTPCheck.IsChecked != null)
            {
                if (enableNTPCheck.IsChecked == true)
                {
                    serverCombo.IsEnabled = true;
                    getTimeButton.IsEnabled = true;
                }
                else
                {
                    serverCombo.IsEnabled = false;
                    getTimeButton.IsEnabled = false;
                }
            }

            ntpData lastGET = window.time.lastgetTime();

            if(lastGET.error == 0)
            {
                lastGetlabel.Content = lastGET.dt.ToString("yyyy/MM/dd HH:mm:ss");
            }
            if(lastGET.error == -1)
            {
                lastGetlabel.Content = lastGET.dt.ToString("yyyy/MM/dd HH:mm:ss 失敗");
            }
        }

        //イベント

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            drawWindow();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            drawWindow();
        }

        private void enableNTPCheck_Click(object sender, RoutedEventArgs e)
        {
            if(enableNTPCheck.IsChecked != null)
            {
                if(enableNTPCheck.IsChecked == true)
                {
                    serverCombo.IsEnabled = true;
                    getTimeButton.IsEnabled = true;

                    enableNTP = true;

                }else
                {
                    serverCombo.IsEnabled = false;
                    getTimeButton.IsEnabled = false;

                    enableNTP = false;
                }
            }
        }


        private void ntpwindow_Loaded(object sender, RoutedEventArgs e) //ウィンドウ読み込み
        {
            drawWindow();
        }

        private async void OK_button_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("NTPconfEnable:{0}", enableNTP);

            await Task.Run(()=>
            {
                window.time.changeNTP(ntpServer, enableNTP);
            });

            //window.time.changeNTP(ntpServer, enableNTP);

            if(enableNTP == true)
            {
                getTimeButton.IsEnabled = true;
            }

            drawWindow();
            window.drawLabel();

            window.time.saveFile();

            this.Visibility = Visibility.Hidden;
        }

        private async void getTimeButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                window.time.ntpGet();
            });
            //window.time.ntpGet();

            ntpData lastGET = window.time.lastgetTime();

            if (lastGET.error == 0)
            {
                lastGetlabel.Content = lastGET.dt.ToString("yyyy/MM/dd HH:mm:ss");
            }
            if (lastGET.error == -1)
            {
                lastGetlabel.Content = lastGET.dt.ToString("yyyy/MM/dd HH:mm:ss 失敗");
            }
        }

        private void serverCombo_DropDownClosed(object sender, EventArgs e)
        {
            if(window.time.getServerAddress() != serverCombo.Text)
            {
                getTimeButton.IsEnabled = false;
            }else
            {
                getTimeButton.IsEnabled = true;
            }

            ntpServer = serverCombo.Text;
        }
    }
}
