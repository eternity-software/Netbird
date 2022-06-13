using CefSharp;
using Netbird.browser;
using Netbird.browser.handlers;
using Netbird.controls;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Netbird
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, TabController.ControllerCallback
    {

        public TabController tabController;
        private NetbirdChromium selectedNetbirdChromium = null;
        private bool isEditingUrl = false;
        private bool isCtrlPressed = false;

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
                if (e.OldDpi.PixelsPerDip != e.NewDpi.PixelsPerDip)
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
            tabController.AddTab(null, "Main");

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
            if (tabController.draggingControl != null && tabController.GetTabCount() > 2)
            {
                tabController.openNewWindow(tabController.draggingControl);
                tabController.draggingControl = null;
            }
        }

        private bool hookAllowed = false;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((tabController.currentlyTabHovering && tabsPresenter.Children.Count > 1) | currentlyAddHovering) return;

            if (e.LeftButton == MouseButtonState.Pressed)
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
            if (WindowState == WindowState.Maximized)
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
            if (tabController.getIndex(tab) == tabControl.SelectedIndex)
            {
                Title = title;
            }
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            tabController.AddTab(null, "Main");
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
            if ((sender as Image).IsEnabled)
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
        private long milliseconds = 0;
       
        private void updateChromiumInfo()
        {
          
            long curr  = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (curr - 10 < milliseconds) return;
            
            milliseconds = curr;
            if (tabControl.SelectedItem == null)
            {
                return;
            }
            if ((tabControl.SelectedItem as TabItem).Content is NetbirdChromium)
            {
                NetbirdChromium netbirdChromium = (NetbirdChromium)(tabControl.SelectedItem as TabItem).Content;
                if (selectedNetbirdChromium != netbirdChromium)
                {
                    if (selectedNetbirdChromium != null)
                    {

                        selectedNetbirdChromium.LayoutUpdated -= NetbirdChromium_LayoutUpdated;

                    }

                    netbirdChromium.LayoutUpdated += NetbirdChromium_LayoutUpdated;
                    selectedNetbirdChromium = netbirdChromium;

                }

                String adress = netbirdChromium.Address;
                if (adress != null)
                {
                    String domain = "Main";
                    if (adress.StartsWith("http"))
                    {
                        Uri myUri = new Uri(adress);
                         domain = myUri.Host;
                    }
                  

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
               
                if (netbirdChromium.IsLoading)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(@"/Netbird;component/Resources/stop_icon.png", UriKind.Relative);
                    image.EndInit();



                    reloadBtn.Source = image;
                }
                else
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(@"/Netbird;component/Resources/refresh.png", UriKind.Relative);
                    image.EndInit();
                    reloadBtn.Source = image;

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
            if ((tabControl.SelectedItem as TabItem).Content is NetbirdChromium)
            {
                NetbirdChromium netbirdChromium = (NetbirdChromium)(tabControl.SelectedItem as TabItem).Content;
                if (netbirdChromium.CanGoBack)
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
            try
            {
                if (selectedNetbirdChromium != null)
                {
                    selectedNetbirdChromium.Reload();
                }
            }
            catch { }
           
        }

        private void BackButton1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (selectedNetbirdChromium == null) return;
            if (selectedNetbirdChromium.IsLoading) return;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"/Netbird;component/Resources/refresh_hovered.png", UriKind.Relative);
            image.EndInit();
            (sender as Image).Source = image;

        }

        private void BackButton1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (selectedNetbirdChromium == null) return;
            if (selectedNetbirdChromium.IsLoading) return;
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
            if (selectedNetbirdChromium != null)
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
            if (selectedNetbirdChromium != null)
            {
                selectedNetbirdChromium.Address = UrlBox.Text;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {

                if (tabController.isFullscreen)
                {


                    // selectedNetbirdChromium.ExecuteScriptAsync("document.getElementsByTagName('video')[0].webkitExitFullScreen()");
                }
                if (tabController.isFullscreen)
                {
                    try
                    {
                        selectedNetbirdChromium.ExecuteScriptAsync("document.webkitExitFullscreen()");
                    }
                    catch { }

                    tabController.RequestFullscreen(false);
                }

            }
            else if (e.Key == Key.F11)
            {


                tabController.RequestFullscreen(!tabController.isFullscreen);
            }
            else if (e.Key == Key.LeftCtrl ||
                e.Key == Key.RightCtrl)
            {
               
                isCtrlPressed = true;
            }
            else if (e.Key == Key.W)
            {
                if (isCtrlPressed)
                {
                    if (tabControl.Items.Count > 0)
                    {
                        tabController.CloseTab(tabController.selectedTab);
                    }

                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl ||
                e.Key == Key.RightCtrl)
            {

                isCtrlPressed = false;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tabController.UpdateTabs();
        }

        private void ScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            hookAllowed = false;
         
        }
    }
}
