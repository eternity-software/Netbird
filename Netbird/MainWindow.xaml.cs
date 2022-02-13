using CefSharp;
using CefSharp.Wpf;
using Netbird.browser;
using Netbird.browser.handlers;
using Netbird.controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Netbird
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, TabController.ControllerCallback
    {

        public TabController tabController;
        private NetbirdChromium selectedNetbirdChromium;
        private bool isEditingUrl = false;

        public MainWindow()
        {
          
           

            InitializeComponent();
            this.DpiChanged += MainWindow_DpiChanged;
            tabController = new TabController(tabControl, this, tabsPresenter);
            tabController.controllerCallback = this;
            tabControl.SelectionChanged += tabControl_SelectionChanged;
           // tabController.addTab("https://yandex.ru/");
        }

        
        private void MainWindow_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            Debug.WriteLine("DPI changed");
            try
            {
               if(e.OldDpi.PixelsPerDip != e.NewDpi.PixelsPerDip)
                {


                    MessageBox.Show("It seems that DPI has been changed. Netbird currently not supporting dynamic DPI. ", "Netbird Warning");
                }
               
            }
            catch
            {
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tabController.addTab("https://yandex.ru/");
      
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        
            Title = tabController.GetTabTitle(tabControl.SelectedIndex);
            updateChromiumInfo();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tabController.saveOpenTabs();
        }

        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            if(tabController.draggingControl != null && tabController.GetTabCount() > 2)
            {
                tabController.openNewWindow(tabController.draggingControl);
                tabController.draggingControl = null;
            }
        }

        private bool hookAllowed = false;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((tabController.currentlyTabHovering && tabsPresenter.Children.Count > 1) | currentlyAddHovering) return;
           
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                hookAllowed = true;
                this.DragMove();
            }
           
           
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            HookWindowMinimize(sender, e);
        }

        

        public void HookWindowMinimize(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && WindowState == WindowState.Maximized && hookAllowed)
            {


                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);




                double percentOfWindow = pointToWindow.X / System.Windows.SystemParameters.WorkArea.Width;
                Debug.WriteLine("percent " + percentOfWindow);
                WindowState = WindowState.Normal;

                this.Left = pointToScreen.X - this.Width * percentOfWindow;
                this.Top = pointToScreen.Y;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }

            }
        }

        public void moveToCursor(double x, double y)
        {
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);





            this.Top = pointToScreen.Y - y;
            this.Left = pointToScreen.X - x;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

        }

      

        private void Window_StateChanged(object sender, EventArgs e)
        {
           if(WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                hookAllowed = false;
            }
          
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.None;
           
        }

        private void tabsPresenter_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            hookAllowed = false;
        }

        public void OnTitleUpdate(NetbirdTab tab, string title)
        {
            if(tabController.getIndex(tab) == tabControl.SelectedIndex)
            {
                Title = title;
            }
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            tabController.addTab("https://yandex.ru/");
        }

        private bool currentlyAddHovering = false;

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            currentlyAddHovering = true;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/add_hovered.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            currentlyAddHovering = false;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/add.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Minimized;
            WindowStyle = WindowStyle.None;
        }

        private void Image_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }

        }

        private void Image_MouseEnter_1(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/mini_close_hovered.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseLeave_1(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/mini_close.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseEnter_2(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/windowstate_hovered.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseLeave_2(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/windowstate.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseEnter_3(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/minimize_hovered.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseLeave_3(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/minimize.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
        }

        private void Image_MouseEnter_4(object sender, MouseEventArgs e)
        {
            if((sender as Image).IsEnabled)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"/Netbird;component/Resources/back_hovered.png", UriKind.Relative);
                image.EndInit();
                (sender as Image).Source = image;
            }
            else
            {
               
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(@"/Netbird;component/Resources/back_disabled.png", UriKind.Relative);
                    image.EndInit();
                    (sender as Image).Source = image;
                
            }
        }

        private void Image_MouseLeave_4(object sender, MouseEventArgs e)
        {
            if ((sender as Image).IsEnabled)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"/Netbird;component/Resources/back.png", UriKind.Relative);
                image.EndInit();
                (sender as Image).Source = image;
            }
            else
            {
               
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(@"/Netbird;component/Resources/back_disabled.png", UriKind.Relative);
                    image.EndInit();
                    (sender as Image).Source = image;
                
            }
        }

        private void ForwardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((sender as Image).IsEnabled)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"/Netbird;component/Resources/forward_hovered.png", UriKind.Relative);
                image.EndInit();
                (sender as Image).Source = image;
            }
            else
            {
              
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(@"/Netbird;component/Resources/forward_disabled.png", UriKind.Relative);
                    image.EndInit();
                    (sender as Image).Source = image;
                
            }
        }

        private void ForwardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((sender as Image).IsEnabled)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"/Netbird;component/Resources/forward.png", UriKind.Relative);
                image.EndInit();
                (sender as Image).Source = image;
            }
            else
            {
              
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(@"/Netbird;component/Resources/forward_disabled.png", UriKind.Relative);
                    image.EndInit();
                    (sender as Image).Source = image;
                
            }
        }

      



        /** 
         * WARNING!
         * This method invokes on every layout update!
         * Be careful of unsafe UI change
         */
        private void updateChromiumInfo()
        {
            if(tabControl.SelectedItem == null)
            {
                return;
            }
            if ((tabControl.SelectedItem as TabItem).Content is NetbirdChromium)
            {
                NetbirdChromium netbirdChromium = (NetbirdChromium)(tabControl.SelectedItem as TabItem).Content;
                if(selectedNetbirdChromium != netbirdChromium)
                {
                    if(selectedNetbirdChromium != null)
                    {

                        selectedNetbirdChromium.LayoutUpdated -= NetbirdChromium_LayoutUpdated;

                    }

                    netbirdChromium.LayoutUpdated += NetbirdChromium_LayoutUpdated;
                    selectedNetbirdChromium = netbirdChromium;
                   
                }

                String adress = netbirdChromium.Address;
                if (adress != null)
                {
                    Uri myUri = new Uri(adress);
                    String domain = myUri.Host;

                    if (!isEditingUrl)
                    {
                        if (UrlBox.Text.ToString() != adress)
                        {
                            UrlBox.Text = adress;
                        }
                    }
                    if (Domain.Content.ToString() != domain)
                    {
                        Domain.Content = domain;
                    }
                }
                else if (Domain.Content.ToString() != "Netbird")
                {
                    Domain.Content = "Netbird";

                }


                    if (HeaderBar.Content == null)
                {
                    HeaderBar.Content = netbirdChromium.Title;
                }
                else
                {
                    if (HeaderBar.Content.ToString() != netbirdChromium.Title)
                    {
                        HeaderBar.Content = netbirdChromium.Title;
                    }
                }
               

                if (netbirdChromium.CanGoBack)
                {
                    if (!BackButton.IsEnabled)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(@"/Netbird;component/Resources/back.png", UriKind.Relative);
                        image.EndInit();
                        BackButton.Source = image;
                        BackButton.IsEnabled = true;
                    }
                }
                else
                {
                    if (BackButton.IsEnabled)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(@"/Netbird;component/Resources/back_disabled.png", UriKind.Relative);
                        image.EndInit();
                        BackButton.Source = image;
                        BackButton.IsEnabled = false;
                    }
                }

                if (netbirdChromium.CanGoForward)
                {
                    if (!ForwardButton.IsEnabled)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(@"/Netbird;component/Resources/forward.png", UriKind.Relative);
                        image.EndInit();
                        ForwardButton.Source = image;
                        ForwardButton.IsEnabled = true;
                    }
                }
                else
                {
                    if (ForwardButton.IsEnabled)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(@"/Netbird;component/Resources/forward_disabled.png", UriKind.Relative);
                        image.EndInit();
                        ForwardButton.Source = image;
                        ForwardButton.IsEnabled = false;
                    }
                }
            }
        }

        private void NetbirdChromium_LayoutUpdated(object sender, EventArgs e)
        {
            updateChromiumInfo();
        }

        private void Image_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            if((tabControl.SelectedItem as TabItem).Content is NetbirdChromium)
            {
                NetbirdChromium netbirdChromium = (NetbirdChromium)(tabControl.SelectedItem as TabItem).Content;
                if(netbirdChromium.CanGoBack)
                {
                    netbirdChromium.Back();
                  
                }
            }
        }

        private void ForwardButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((tabControl.SelectedItem as TabItem).Content is NetbirdChromium)
            {
                NetbirdChromium netbirdChromium = (NetbirdChromium)(tabControl.SelectedItem as TabItem).Content;
                if (netbirdChromium.CanGoForward)
                {
                    netbirdChromium.Forward();
                  
                }
            }
        }

        private void BackButton1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(selectedNetbirdChromium != null)
            {
                selectedNetbirdChromium.Reload();
            }
        }

        private void BackButton1_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/refresh_hovered.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
            
        }

        private void BackButton1_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/refresh.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;
          
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
           
            (sender as Border).Background = (Brush)new BrushConverter().ConvertFromString("#DCDCDC");
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = (Brush)new BrushConverter().ConvertFromString("#E5E5E5");
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(selectedNetbirdChromium != null)
            {
                selectedNetbirdChromium.Address = "https://" + Domain.Content.ToString();
            }
        }

        private void HeaderBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
          
            UrlBar.Visibility = Visibility.Visible;
            InfoBar.Visibility = Visibility.Collapsed;
            InfoBar.IsEnabled = false;
            
         
            isEditingUrl = true;
         
            UrlBox.SelectAll();

        }

        private void AddHandler()
        {
            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            UrlBar.Visibility = Visibility.Collapsed;
            InfoBar.Visibility = Visibility.Visible;
            ReleaseMouseCapture();
            isEditingUrl = false;
            InfoBar.IsEnabled = true;
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            UrlBar.Visibility = Visibility.Collapsed;
            InfoBar.Visibility = Visibility.Visible;
            isEditingUrl = false;
            InfoBar.IsEnabled = true;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            UrlBar.Visibility = Visibility.Collapsed;
            InfoBar.Visibility = Visibility.Visible;
            isEditingUrl = false;
            InfoBar.IsEnabled = true;
        }

        private void UrlBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;

            // your event handler here
            e.Handled = true;
            if(selectedNetbirdChromium != null)
            {
                selectedNetbirdChromium.Address = UrlBox.Text;
            }
        }
    }
}
