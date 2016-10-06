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
        int fontSizeMode;   //フォントサイズ

        about aboutThis; //このアプリケーションについてのウィンドウ

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
            fontSizeMode = window.fontSizeMode;

            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            this.DataContext = new MainWindowViewModel();

        }

        private void preViewDraw()
        {
            previewLabel.Content = window.clockLabel.Content;
            if (backgroundEnable == true)
            {
                previewLabel.Background = backColor;
            }
            else
            {
                previewLabel.Background = Brushes.Transparent;
            }

            previewLabel.Foreground = fontColor;
            previewLabel.Opacity = opt;

            previewLabel.FontFamily = new FontFamily(fontname);
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

            //フォントサイズ
            switch (fontSizeMode)
            {
                case 0:
                    radioSmall.IsChecked = true;
                    break;
                case 1:
                    radioNomal.IsChecked = true;
                    break;
                case 2:
                    radioLarge.IsChecked = true;
                    break;
                default:
                    break;
            }

            //プレビューラベル
            preViewDraw();

        }


        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)   //透明度スライダー
        {
            opt = slider.Value / 100;

            if (opLabel != null)
            {
                opLabel.Content = string.Format("{0}", (int)slider.Value);
            }

            if (previewLabel != null)
            {
                previewLabel.Opacity = opt;
                preViewDraw();
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
            window.fontSizeMode = fontSizeMode;

            window.drawLabel();


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
            window.fontSizeMode = fontSizeMode;

            window.drawLabel();

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
            preViewDraw();
        }

        private void enableDragBox_Click(object sender, RoutedEventArgs e)  //移動可能チェックボックス
        {
            if (enableDragBox.IsChecked != null)
            {
                if ((bool)enableDragBox.IsChecked == true)
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

            preViewDraw();
        }

        private void backgroundColorButton_Click(object sender, RoutedEventArgs e)  //背景変更ボタン
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color color = Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
                backColor = new SolidColorBrush(color);
            }

            preViewDraw();
        }

        private void fontColorButton_Click(object sender, RoutedEventArgs e)    //フォントカラー変更ボタン
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color color = Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
                fontColor = new SolidColorBrush(color);
            }

            preViewDraw();
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

        private void radioSmall_Checked(object sender, RoutedEventArgs e)   //フォント小
        {
            fontSizeMode = 0;
        }

        private void radioNomal_Checked(object sender, RoutedEventArgs e)   //フォント中
        {
            fontSizeMode = 1;
        }

        private void radioLarge_Checked(object sender, RoutedEventArgs e)   //フォント大
        {
            fontSizeMode = 2;
        }

        private void versionButton_Click(object sender, RoutedEventArgs e)  //このアプリケーションについてボタン
        {
            if (aboutThis != null)
            {
                if (aboutThis.Visibility == Visibility.Hidden)
                {
                    aboutThis.Visibility = Visibility.Visible;
                }

                aboutThis.Focus();
            }
            else
            {
                aboutThis = new about();
                aboutThis.Show();
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
