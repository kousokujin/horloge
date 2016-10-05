using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace horloge
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIconWrapper notifyIcon;

        public bool movelook = false;      //trueだとウィンドウ移動不可


        public MainWindow()
        {
            InitializeComponent();
            this.notifyIcon = new NotifyIconWrapper(this);
        }

        public double get_opt() //透明度取得
        {
            return this.Opacity;
        }

        public void ch_opt(double value)
        {
            this.Opacity = value;
        }

        public void drawLabel()
        {
            loadNowtime();

            secLabel.FontFamily = new FontFamily(clockLabel.FontFamily.ToString());
            dataLabel.FontFamily = new FontFamily(clockLabel.FontFamily.ToString());

            Size textSize = MeasureString(clockLabel.Content.ToString(), clockLabel.FontSize, clockLabel.FontFamily.ToString());
            Size dataLabel_height = MeasureString(dataLabel.Content.ToString(), dataLabel.FontSize, dataLabel.FontFamily.ToString());
            clockLabel.Width = textSize.Width+5;
            clockLabel.Height = textSize.Height;
            secLabel.Margin = new Thickness(clockLabel.Width, clockLabel.Margin.Top, 0, 0);

        }

        private void loadNowtime()
        {
            DateTime nowtime = DateTime.Now;

            clockLabel.Content = nowtime.ToString("HH:mm");
            secLabel.Content = nowtime.ToString("ss");
            dataLabel.Content = nowtime.ToString("yyyy/MM/dd");
        }

        private async void start_tick()
        {
            await Task.Run(() => tick());
        }

        private void tick()
        {
            while(true)
            {
                this.Dispatcher.BeginInvoke(
                    new Action(()=>
                    {
                        loadNowtime();
                    })
                    );

                System.Threading.Thread.Sleep(1000);
            }
        }

        private Size MeasureString(string text, double fontSize, string typeFace)
        {
            var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(typeFace), fontSize, Brushes.White);
            return new Size(ft.Width, ft.Height);
        }

        //以下イベント

        private void clockWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) //ウィンドウを右クリックした時
        {
            if(e.ButtonState != MouseButtonState.Pressed)
            {
                return;
            }
            else
            {
                if (movelook == false)
                {
                    this.DragMove();    //ウィンドウの移動
                }
            }

        }

        private void clockWindow_Loaded(object sender, RoutedEventArgs e)   //メインウィンドウの描画イベント
        {
            drawLabel();
            start_tick();
        }
    }
}
