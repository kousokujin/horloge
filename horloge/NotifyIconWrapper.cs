using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace horloge
{

    public partial class NotifyIconWrapper : Component
    {
        MainWindow window;
        config confWin;

        public NotifyIconWrapper(MainWindow mainWindow)
        {
            InitializeComponent();

            this.toolStrinMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;
            this.toolStripConfig.Click += this.startConfig;

            window = mainWindow;
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }


        //以下イベント


        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            // 現在のアプリケーションを終了
            Application.Current.Shutdown();
        }

        private void startConfig(object sender, EventArgs e)
        {
            if (confWin != null)
            {
                if(confWin.Visibility == Visibility.Hidden)
                {
                    confWin.Visibility = Visibility.Visible;
                }

                confWin.Focus();
            }
            else
            {
                confWin = new config(window);
                confWin.Show();
            }
        }
    }
}
