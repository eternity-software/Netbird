using System;
using System.Threading;
using System.Windows;

namespace Netbird
{
    /// <summary>
    /// Логика взаимодействия для OnScreenNotification.xaml
    /// </summary>
    public partial class OnScreenNotification : Window
    {

        private static bool isShown = false;
        public OnScreenNotification()
        {
            InitializeComponent();
            Topmost = true;

            isShown = true;

            Thread thread = new Thread(() =>
            {
                Thread.Sleep(5000);
                this.Dispatcher.Invoke(() =>
                {
                    Close();
                    isShown = false;
                });

            });

            thread.Start();
        }

        public static bool IsShown()
        {
            return isShown;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.Top = WpfScreen.GetScreenFrom(this).WorkingArea.Height / 6;
        }
    }
}
