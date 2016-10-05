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

namespace horloge
{
    /// <summary>
    /// config.xaml の相互作用ロジック
    /// </summary>
    public partial class config : Window
    {
        MainWindow window;

        bool movelook;      //trueだとウィンドウ移動不可

        double opt; //透明度
        string fontname;    //フォント

        public config(MainWindow mainWindow)
        {
            InitializeComponent();

            window = mainWindow;

            movelook = window.movelook;
            opt = window.get_opt();
            fontname = window.clockLabel.FontFamily.ToString();

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
            fontMenu.Text = fontname;
            System.Console.WriteLine("font:{0}", fontname);

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
            window.ch_opt(opt);
            window.clockLabel.FontFamily = new FontFamily(fontname);

            window.drawLabel();

            this.Visibility = Visibility.Hidden;
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)    //適用ボタン
        {
            window.movelook = movelook;
            window.ch_opt(opt);
            window.clockLabel.FontFamily = new FontFamily(fontname);

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
