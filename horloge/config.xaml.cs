using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace horloge
{
    /// <summary>
    /// config.xaml の相互作用ロジック
    /// </summary>
    public partial class config : Window
    {
        MainWindow window;

        bool movelook;      //trueだとウィンドウ移動不可
        bool backgroundEnable;
        bool topEnable;   //trueだと常に最前面

        double opt; //透明度
        Brush backColor;    //背景色
        string fontname;    //フォント
        Brush fontColor; //フォントカラー

        public config(MainWindow mainWindow)
        {
            InitializeComponent();

            window = mainWindow;

            movelook = window.movelook;
            backgroundEnable = window.backgroundEnable;
            topEnable = window.Topmost;
            opt = window.get_opt();
            fontname = window.clockLabel.FontFamily.ToString();
            backColor = window.backColor;
            fontColor = window.fontColor;

            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            this.DataContext = new MainWindowViewModel();

        }

        //以下イベント

        private void opLabel_Loaded(object sender, RoutedEventArgs e)
        {

            //透明度
            opt = window.get_opt();
            slider.Value = opt * 100;
            opLabel.Content = string.Format("{0}", (int)slider.Value);

            //フォント
            fontMenu.Text = fontname;

            //ドラッグ
            enableDragBox.IsChecked = movelook;

            //最前面
            enableTopBox.IsChecked = topEnable;

            //背景
            disableBackgroundBox.IsChecked = !backgroundEnable;

        }


        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)   //透明度スライダー
        {
            opt = slider.Value/100;

            if (opLabel != null)
            {
                opLabel.Content = string.Format("{0}", (int)slider.Value);
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)   //OKボタン
        {

            window.movelook = movelook;
            window.backgroundEnable = backgroundEnable;
            window.Topmost = topEnable;
            window.ch_opt(opt);
            window.clockLabel.FontFamily = new FontFamily(fontname);

            window.fontColor = fontColor;
            window.backColor = backColor;

            window.drawLabel();

            System.Console.WriteLine("backEnable:{0}",window.backgroundEnable);

            this.Visibility = Visibility.Hidden;
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)    //適用ボタン
        {
            window.movelook = movelook;
            window.backgroundEnable = backgroundEnable;
            window.Topmost = topEnable;
            window.ch_opt(opt);
            window.clockLabel.FontFamily = new FontFamily(fontname);

            window.fontColor = fontColor;
            window.backColor = backColor;

            window.drawLabel();

            System.Console.WriteLine("backEnable:{0}", window.backgroundEnable);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void fontMenu_DropDownClosed(object sender, EventArgs e)    //フォントメニューを閉じた時
        {
            fontname = fontMenu.Text;
        }

        private void enableDragBox_Click(object sender, RoutedEventArgs e)  //移動可能チェックボックス
        {
            if(enableDragBox.IsChecked != null )
            {
                if((bool)enableDragBox.IsChecked == true)
                {
                    movelook = true;
                }
                else
                {
                    movelook = false;
                }
            }
            else
            {
                movelook = false;
            }
        }

        private void disableBackgroundBox_Click(object sender, RoutedEventArgs e)   //背景透過チェックボックス
        {
            if (disableBackgroundBox.IsChecked != null)
            {
                if ((bool)disableBackgroundBox.IsChecked == true)
                {
                    backgroundEnable = false;
                }
                else
                {
                    backgroundEnable = true;
                }
            }
            else
            {
                backgroundEnable = true;
            }
        }

        private void backgroundColorButton_Click(object sender, RoutedEventArgs e)  //背景変更ボタン
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color color = Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
                backColor= new SolidColorBrush(color);
            }
        }

        private void fontColorButton_Click(object sender, RoutedEventArgs e)    //フォントカラー変更ボタン
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color color = Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
                fontColor = new SolidColorBrush(color);
            }
        }

        private void enableTopBox_Click(object sender, RoutedEventArgs e)
        {
            if (enableTopBox.IsChecked != null)
            {
                if ((bool)enableTopBox.IsChecked == true)
                {
                    topEnable = true;
                }
                else
                {
                    topEnable = false;
                }
            }
            else
            {
                topEnable = false;
            }
        }
    }

    class MainWindowViewModel
    {
        public IEnumerable<FontFamily> FontList { get; set; }

        public MainWindowViewModel()
        {
            this.FontList = Fonts.SystemFontFamilies;
        }
    }
}
