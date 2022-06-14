using CefSharp;
using CefSharp.Wpf;
using Netbird.browser;
using System;
using System.IO;
using System.Windows;

namespace Netbird
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    /// 
   
    public partial class App : Application
    {
        public static String downloadsFolder;
        public App() : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            CefSettings settings = new CefSettings();
            // settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36 - Netbird/1.0";
            settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\cache"; ;
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream", "use-fake-ui-for-media-stream");
           settings.CefCommandLineArgs.Add("enable-gpu", "1");
            settings.CefCommandLineArgs.Add("enable-webgl", "1");
            settings.CefCommandLineArgs.Add("persist_session_cookies", "1");
            settings.CefCommandLineArgs.Add("enable-automatic-password-saving", "enable-automatic-password-saving");
            settings.CefCommandLineArgs.Add("enable-password-save-in-page-navigation", "enable-password-save-in-page-navigation");
            settings.CefCommandLineArgs.Add("enable-widevine-cdm", "1");
            settings.CefCommandLineArgs.Add("ppapi-flash-path", Path.Combine(Environment.CurrentDirectory, @"pepflashplayer64_21_0_0_242.dll"));
              settings.CefCommandLineArgs.Add("enable-smooth-scrolling", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");




          


            Cef.Initialize(settings);

          //  NetbirdChromium netbirdChromium = new NetbirdChromium(null);

          //  Window window = new Window();
         //   window.Content = netbirdChromium;

        //    netbirdChromium.Load("https://vk.com");

          //  window.Show();

            loadSavedInstance();





        }



        private void loadSavedInstance()
        {

            // TODO: Implement hitory restore tabs
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.tabController.loadSavedTabs();
        }




        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Netbird Unhandled Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
