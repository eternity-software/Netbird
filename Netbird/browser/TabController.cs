using Netbird.controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Netbird.browser.handlers
{
    public class TabController
    {

        public static int DEFAULT_CONTENT_MARGIN = 85;
        public static int DEFAULT_MAX_TAB_WIDTH = 150;
        public static int DEFAULT_TAB_HORIZONTAL_MARGIN = 3;


        public bool currentlyTabHovering = false;
        public ControllerCallback controllerCallback;
        public Window window;
        public bool isFullscreen = false;
        public NetbirdTab selectedTab;
        private TabControl tabControl;
        private StackPanel tabsPresenter;
        private List<NetbirdChromium> tabs = new List<NetbirdChromium>();

        public TabController(TabControl tabControl, Window window, StackPanel tabsPresenter)
        {
            this.tabControl = tabControl;
            this.window = window;
            this.tabsPresenter = tabsPresenter;

            this.tabControl.SelectionChanged += TabControl_SelectionChanged;

        }

        public String GetTabTitle(int index)
        {
            Debug.WriteLine(index);
            if (index > tabs.Count - 2 | index < 0)
            {
                return "Blank Page";
            }
            if (tabs[index] != null)
            {
                String result = tabs[index].Title;
                if (result == null)
                {
                    result = "Netbird Browser";
                }
                return result;
            }
            else
            {
                return "Netbird";
            }
          
          
        }


        public int GetTabCount()
        {
            return tabsPresenter.Children.Count;
        }

        public void loadSavedTabs()
        {
            List<TabInfo> tabInfos = (List<TabInfo>)JsonConvert.DeserializeObject<List<TabInfo>>(Properties.Settings.Default.SavedTabs);
            try
            {
                int selectedId = 0;
                int i = 0;
                foreach (TabInfo tabInfo in tabInfos)
                {
                    NetbirdChromium netbirdChromium = new NetbirdChromium(this);
                    netbirdChromium.Address = tabInfo.Adress;
                    if (tabInfo.Adress == "main" || tabInfo.Adress == null) netbirdChromium = null;
                    AddTab(netbirdChromium, tabInfo.Header);
                    if (tabInfo.isSelected)
                    {
                        selectedId = i;
                    }
                    i++;
                }
                tabControl.SelectedIndex = selectedId;
            }
            catch
            {
                MessageBox.Show("Tabs loading error", "Netbird Error");
            }


        }

        public void saveOpenTabs()
        {
            int i = 0;
            List<TabInfo> tabInfos = new List<TabInfo>();
            foreach (Object obj in tabsPresenter.Children)
            {
                if (obj is NetbirdTab)
                {
                    NetbirdTab netbirdTab = (NetbirdTab)obj;
                    NetbirdChromium netbirdChromium = tabs[i];
                    String adress = "main";
                    if (netbirdChromium != null)
                    {
                        adress = netbirdChromium.Address;
                    }
                    bool selected = (tabControl.SelectedIndex == i);

                    TabInfo tabInfo = new TabInfo
                    {
                        Adress = adress,
                        Header = (String)netbirdTab.Header,
                        isSelected = selected
                    };

                    tabInfos.Add(tabInfo);
                }
                i++;
            }

            String json = JsonConvert.SerializeObject(tabInfos);

            Properties.Settings.Default.SavedTabs = json;
            Properties.Settings.Default.Save();




        }

        public class TabInfo
        {
            public String Adress { get; set; }
            public String Header { get; set; }
            public bool isSelected { get; set; }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = 0;
            int selectedId = tabControl.SelectedIndex;
            foreach (Object obj in tabsPresenter.Children)
            {
                if (obj is TabItem)
                {
                    TabItem tabItem = (TabItem)obj;

                    if (i == selectedId)
                    {
                        tabItem.IsSelected = true;
                    }
                    else
                    {
                        tabItem.IsSelected = false;
                    }
                }
                i++;
            }
        }

        public void updateTitle(String newTitle, NetbirdChromium netbird)
        {
            if (!tabs.Contains(netbird)) return;
            window.Dispatcher.Invoke(() =>
            {

                try
                {
                    NetbirdTab tabItem = (NetbirdTab)tabsPresenter.Children[tabs.IndexOf(netbird)];
                    if (controllerCallback != null)
                    {
                        controllerCallback.OnTitleUpdate(tabItem, newTitle);
                    }
                    if (newTitle.Length > 10)
                    {
                        newTitle = newTitle.Substring(0, 10) + "..";
                    }

                    tabItem.Header = newTitle;

                }
                catch { }


            });
        }

        public void updateFavicon(String url, NetbirdChromium netbird)
        {
            //if (!url.StartsWith("http")) return;
            if (!tabs.Contains(netbird)) return;
            window.Dispatcher.Invoke(() =>
            {

                NetbirdTab tabItem = (NetbirdTab)tabsPresenter.Children[tabs.IndexOf(netbird)];
                tabItem.SetFavicon(url);
            });
        }

        public void openNewWindow(NetbirdTab netbirdTab)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Dispatcher.Invoke(() =>
            {
                int index = tabsPresenter.Children.IndexOf(netbirdTab);
                NetbirdChromium netbirdChromium = tabs[tabsPresenter.Children.IndexOf(netbirdTab)];

                if(netbirdChromium != null)
                {
                    netbirdChromium.updateController(mainWindow.tabController);
                }
               
                mainWindow.tabController.addTab(netbirdChromium);
                tabsPresenter.Children.RemoveAt(index);
                tabs.Remove(netbirdChromium);
                tabControl.Items.RemoveAt(index);
                mainWindow.Show();
                mainWindow.Focus();
                mainWindow.moveToCursor(85, 30);
            });


        }


        public NetbirdChromium GetSelectedChromium()
        {
            return tabs[tabsPresenter.Children.IndexOf(selectedTab)];
        }

        public void CloseTab(NetbirdTab netbirdTab)
        {

            int index = tabsPresenter.Children.IndexOf(netbirdTab);
            NetbirdChromium netbirdChromium = tabs[tabsPresenter.Children.IndexOf(netbirdTab)];
            tabsPresenter.Children.RemoveAt(index);
            tabs.Remove(netbirdChromium);
            tabControl.Items.RemoveAt(index);

            if(tabs.Count == 0)
            {
                AddTab(null, "Main");
            }

        }

        private (double height, double width) GetVirtualWindowSize()
        {
            Window virtualWindow = new Window();
            virtualWindow.Show();
            virtualWindow.WindowStyle = WindowStyle.None;
            virtualWindow.Opacity = 0;
            virtualWindow.WindowState = WindowState.Maximized;
            double returnHeight = virtualWindow.Height;
            double returnWidth = virtualWindow.Width;
            virtualWindow.Close();
            return (returnHeight, returnWidth);
        }

        public WindowState state;
        public Thickness tabConrollThickness;
        public void RequestFullscreen(bool isFullscreen)
        {
            this.isFullscreen = isFullscreen;
            window.Dispatcher.Invoke(() =>
            {

                if (isFullscreen)
                {
                    state = window.WindowState;
                    if (!OnScreenNotification.IsShown())
                    {
                        OnScreenNotification onScreenNotification = new OnScreenNotification();
                        onScreenNotification.Show();
                    }


                    var sizingParams = GetVirtualWindowSize();
                    window.WindowState = WindowState.Normal;
                    window.ResizeMode = ResizeMode.NoResize;
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
                    tabConrollThickness = tabControl.Margin;
                    tabControl.Margin = new Thickness(0, 0, 0, 0);


                }
                else
                {
                    tabControl.Margin = new Thickness(0, DEFAULT_CONTENT_MARGIN, 0, 0);
                    window.WindowState = state;
                    window.ResizeMode = ResizeMode.CanResize;
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                    window.WindowState = WindowState.Maximized;
                    window.WindowStyle = WindowStyle.None;
                }
            });
        }


        public NetbirdTab draggingControl;

        public void addTab(NetbirdChromium netbird)
        {
            addTab(netbird, tabsPresenter.Children.Count - 1);
        }
        public void addTab(NetbirdChromium netbird, int index)
        {
            if (netbird == null)
            {
                AddTab(netbird, "Main", index);
            }
            else
            {
                AddTab(netbird, netbird.Title, index);
            }
         
        }


        public void AddTab(NetbirdChromium netbird, String header)
        {
            AddTab(netbird, header, tabsPresenter.Children.Count - 1);
        }


        public void AddTab(NetbirdChromium netbird, String header, int index)
        {
            window.Dispatcher.Invoke(() =>
            {
                NetbirdTab tab = new NetbirdTab();

                
            
             

                tab.tabController = this;


                tab.MouseMove += OnTabMouseMove;

                tab.MouseEnter += delegate { currentlyTabHovering = true; };
                tab.MouseLeave += delegate { currentlyTabHovering = false; };

                tab.MouseUp += delegate
                {
                    draggingControl = null;
                    draggingControl = null;
                    tab.isMouseDown = false;
                };

                // tab.Content = chromium;
                tab.Header = header;
                TabItem virtualTab = new TabItem();
                if(netbird == null)
                {
                    virtualTab.Content = new MainTab(this, tab);
                    tab.Header = "Netbird";
                }
                else
                {
                    virtualTab.Content = netbird;
                    netbird.DisplayHandler = new DisplayHandler(this);
                }
              



                tab.OnActionButtonClicked += delegate
                {

                    CloseTab(tab);


                };





                tab.PreviewMouseUp += (sender, e) =>
                {
                    UpdateTabs();
                };


                tab.PreviewMouseDown += (sender, e) =>
                {
                    if (!tab.ActionButtonHovering)
                    {
                        tab.isMouseDown = true;
                        draggingControl = tab;
                        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                        {
                            selectedTab = tab;
                            tabControl.SelectedIndex = tabControl.Items.IndexOf(virtualTab);


                        }
                        else if (e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
                        {
                            if (tabControl.Items.Count > 0)
                            {
                                CloseTab(tab);
                            }
                            draggingControl = null;



                        }

                    
                        tab.isMouseDown = false;
                    }
                };

                if (index == tabsPresenter.Children.Count - 1)
                {
                   
                        tabs.Add(netbird);
                    
                 
                    tabControl.Items.Add(virtualTab);
                    tabsPresenter.Children.Insert(tabsPresenter.Children.Count - 1, tab);
                    tabControl.SelectedIndex = tabControl.Items.Count - 1;

                }
                else
                {
                  
                        tabs.Insert(index + 1, netbird);
                    
                    tabControl.Items.Insert(index + 1, virtualTab);
                    tabsPresenter.Children.Insert(index + 1, tab);
                    tabControl.SelectedIndex = index + 1;

                }



                selectedTab = tab;
                UpdateTabs();

            });
        }

        public int getIndex(NetbirdTab netbirdTab)
        {

            return tabsPresenter.Children.IndexOf(netbirdTab);
        }




        public void UpdateTabs()
        {
            int i = 0;

           
            double presenterWidth = tabsPresenter.ActualWidth - 100;
            double tabWidth = presenterWidth / (tabsPresenter.Children.Count - 1);

            if (tabWidth < 5)
            {
                tabWidth = 5;
            }
            if (tabWidth >= DEFAULT_MAX_TAB_WIDTH)
            {
                tabWidth = DEFAULT_MAX_TAB_WIDTH;

            }
            else
            {
                tabWidth = tabWidth - (DEFAULT_TAB_HORIZONTAL_MARGIN * 2);

            }

            Thickness margin;
            if (tabWidth < 70)
            {
                margin = new Thickness(1, DEFAULT_TAB_HORIZONTAL_MARGIN,
                    1, DEFAULT_TAB_HORIZONTAL_MARGIN);
                tabWidth += (DEFAULT_TAB_HORIZONTAL_MARGIN * 2) - 2;
            }
            else
            {
                margin = new Thickness(DEFAULT_TAB_HORIZONTAL_MARGIN, DEFAULT_TAB_HORIZONTAL_MARGIN,
                   DEFAULT_TAB_HORIZONTAL_MARGIN, DEFAULT_TAB_HORIZONTAL_MARGIN);
            }
            foreach (Object obj in tabsPresenter.Children)
            {
                if (obj is NetbirdTab)
                {
                    NetbirdTab netbirdTab = (NetbirdTab)obj;




                    netbirdTab.Width = tabWidth;
                    netbirdTab.Margin = margin;
                    netbirdTab.UpdateTab();
                }
                i++;
            }
        }

        private void OnTabMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (draggingControl != null && draggingControl != sender)
            {
                Debug.WriteLine("moved X: " + e.GetPosition((sender as NetbirdTab)).X + " " + (sender as NetbirdTab).Width);
                int indexDragging = tabsPresenter.Children.IndexOf(draggingControl);
                int indexHovering = tabsPresenter.Children.IndexOf((sender as NetbirdTab));
                int offset = 0;
                if (indexDragging > indexHovering)
                {
                    offset = -1;
                }
                else
                {
                    offset = 1;
                }
                tabsPresenter.Children.RemoveAt(indexDragging);
                tabsPresenter.Children.Insert(indexDragging + offset, draggingControl);

                TabItem tabItem = (TabItem)tabControl.Items.GetItemAt(indexDragging);
                tabControl.Items.RemoveAt(indexDragging);
                tabControl.Items.Insert(indexDragging + offset, tabItem);

                NetbirdChromium netbirdChromium = tabs[indexDragging];
                tabs.RemoveAt(indexDragging);
                tabs.Insert(indexDragging + offset, netbirdChromium);




                Debug.WriteLine("moved -1");
                UpdateTabs();

            }
            else if (draggingControl == sender)
            {
                int indexDragging = tabsPresenter.Children.IndexOf(draggingControl);
                tabControl.SelectedIndex = indexDragging;
            }
        }

        public void addTab(String link)
        {
            //  if(!link.StartsWith("http")) return;



            NetbirdChromium chromium = new NetbirdChromium(this);
            chromium.Address = link;
            addTab(chromium);



        }

        public void addTab(String link, int index)
        {
            //  if(!link.StartsWith("http")) return;

            

            NetbirdChromium chromium = new NetbirdChromium(this);
            chromium.Address = link;
            if (link == "main") chromium = null;

            addTab(chromium, index);



        }



        public interface ControllerCallback
        {
            void OnTitleUpdate(NetbirdTab tab, String title);

        }
    }
}
