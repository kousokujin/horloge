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
using System.Xml.Serialization;

namespace horloge
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIconWrapper notifyIcon;

        public bool movelook = false;      //trueだとウィンドウ移動不可
        public bool backgroundEnable = true;      //falseだと背景透過

        public Brush backColor; //背景色
        public Brush fontColor; //フォントカラー

        public int fontSizeMode = 1;    //フォントサイズ(大中小)

        public MainWindow()
        {
            InitializeComponent();
            this.notifyIcon = new NotifyIconWrapper(this);

            loadsetting();
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

            switch(fontSizeMode)
            {
                case 0:
                    dataLabel.FontSize = 18;
                    clockLabel.FontSize = 30;
                    secLabel.FontSize = 20;
                    break;

                case 1:
                    dataLabel.FontSize = 25;
                    clockLabel.FontSize = 45;
                    secLabel.FontSize = 30;
                    break;

                case 2:
                    dataLabel.FontSize = 32;
                    clockLabel.FontSize = 54;
                    secLabel.FontSize = 38;
                    break;

                default:
                    dataLabel.FontSize = 25;
                    clockLabel.FontSize = 45;
                    secLabel.FontSize = 30;
                    break;
            }

            secLabel.FontFamily = new FontFamily(clockLabel.FontFamily.ToString());
            dataLabel.FontFamily = new FontFamily(clockLabel.FontFamily.ToString());

            Size textSize = MeasureString(clockLabel.Content.ToString(), clockLabel.FontSize, clockLabel.FontFamily.ToString());
            Size dataSize = MeasureString(dataLabel.Content.ToString(), dataLabel.FontSize, dataLabel.FontFamily.ToString());

            clockLabel.Width = textSize.Width+5;
            clockLabel.Height = textSize.Height+5;

            dataLabel.Width = clockLabel.Width + secLabel.Width + 5;
            dataLabel.Height = dataSize.Height;

            clockLabel.Margin = new Thickness(0, dataSize.Height,0,0);
            secLabel.Margin = new Thickness(clockLabel.Width, clockLabel.Margin.Top+5, 0, 0);

            /*
            if (fontSizeMode == 2)
            {
                this.Width = clockLabel.Width + secLabel.Width+5;
            }else
            {
                this.Width = clockLabel.Width + secLabel.Width;
            }
            */

            if(dataLabel.Width < clockLabel.Width + secLabel.Width)
            {
                this.Width = clockLabel.Width + secLabel.Width;
            }else
            {
                this.Width = dataLabel.Width;
            }

            this.Height = clockLabel.Height + dataLabel.Height;

            clockLabel.Opacity = this.Opacity;
            secLabel.Opacity = this.Opacity;
            dataLabel.Opacity = this.Opacity;

            if(backgroundEnable == true)
            {
                this.Background = backColor;
            }
            else
            {
                this.Background = Brushes.Transparent;
            }

            clockLabel.Foreground = fontColor;
            secLabel.Foreground = fontColor;
            dataLabel.Foreground = fontColor;

            saveClock();
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

        private void saveClock()    //設定保存
        {
            save saveData = new save();
            saveData.writeSave(this);
            string filename = @"config/horlogeconfig.conf";

            if (System.IO.File.Exists("config") == false)
            {
                System.IO.Directory.CreateDirectory(@"config");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(save));
            //ファイルを開く（UTF-8 BOM無し）
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
            filename, false, new System.Text.UTF8Encoding(false));

            serializer.Serialize(sw, saveData);
            //閉じる
            sw.Close();
        }

        private void loadsetting()  //設定読み込み
        {
            if(System.IO.File.Exists(@"config/horlogeconfig.conf") == true)
            {
                save clockData = loadfile();

                movelook = clockData.movelook;
                backgroundEnable = clockData.backgroundEnable;
                this.Topmost = clockData.topEnable;

                backColor = Convertbrash(clockData.backColor); //背景色
                fontColor = Convertbrash(clockData.fontColor); //フォントカラー
                clockLabel.FontFamily = new FontFamily(clockData.fontname);
                this.Opacity = clockData.opt;
                fontSizeMode = clockData.fontSizeMode;

                this.Top = clockData.p.Y;
                this.Left = clockData.p.X;
            }
            else
            {
                backColor = Brushes.White;
                fontColor = Brushes.Black;
            }
        }

        private Brush Convertbrash(my_color c)
        {
            Color memofontcolor = Color.FromArgb(c.A, c.R, c.G, c.B);
            Brush output = new SolidColorBrush(memofontcolor);

            return output;
        }

        private save loadfile()
        {
            string filename = @"config/horlogeconfig.conf";
            //＜XMLファイルから読み込む＞
            //XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer2 = new System.Xml.Serialization.XmlSerializer(typeof(save));
            //ファイルを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(filename, new System.Text.UTF8Encoding(false));
            //XMLファイルから読み込み、逆シリアル化する
            save savefile = (save)serializer2.Deserialize(sr);
            //閉じる
            sr.Close();

            return savefile;
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
            //backColor = Brushes.White;
            //fontColor = clockLabel.Foreground;
            drawLabel();
            start_tick();
        }

        private void clockWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveClock();
        }
    }

    public class save
    {
        public bool movelook;      //trueだとウィンドウ移動不可
        public bool backgroundEnable;
        public bool topEnable;   //trueだと常に最前面

        public double opt; //透明度
        public my_color backColor;    //背景色
        public string fontname;    //フォント
        public my_color fontColor; //フォントカラー
        public int fontSizeMode;   //フォントサイズ

        public Point p; //座標

        public void writeSave(MainWindow mw)
        {
            movelook = mw.movelook;
            backgroundEnable = mw.backgroundEnable;
            topEnable = mw.Topmost;
            opt = mw.Opacity;
            backColor = new my_color(mw.backColor);
            fontColor = new my_color(mw.fontColor);
            fontSizeMode = mw.fontSizeMode;
            fontname = mw.clockLabel.FontFamily.ToString();

            p = mw.PointToScreen(new Point(0.0d, 0.0d));
        }
    }

    public struct my_color
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public my_color(Brush c)
        {
            SolidColorBrush scb = c as SolidColorBrush;

            A = scb.Color.A;
            R = scb.Color.R;
            G = scb.Color.G;
            B = scb.Color.B;
        }
    }
}
