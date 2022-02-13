using CefSharp;
using Netbird.controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static Netbird.controls.NetbirdTab;

namespace Netbird.browser.handlers
{
    public class TabController
    {
        public bool currentlyTabHovering = false;
        public ControllerCallback controllerCallback;

        private TabControl tabControl;
        private StackPanel tabsPresenter;
        public Window window;
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
            String result = tabs[index].Title;
            if(result == null)
            {
                result = "Netbird Browser";
            }
            return result;
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
                    addTab(netbirdChromium, tabInfo.Header);
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
                    NetbirdTab netbirdTab = (NetbirdTab) obj;
                    NetbirdChromium netbirdChromium = tabs[i];

                    String adress = netbirdChromium.Address;
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
            if (!url.StartsWith("http")) return;
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
               
              
                netbirdChromium.updateController(mainWindow.tabController);
                mainWindow.tabController.addTab(netbirdChromium);
                tabsPresenter.Children.RemoveAt(index);
                tabs.Remove(netbirdChromium);
                tabControl.Items.RemoveAt(index);
                mainWindow.Show();
                mainWindow.Focus();
                mainWindow.moveToCursor(20, 20);
            });
          

        }


        public WindowState state;
        public void requestFullscreen(bool isFullscreen)
        {
            window.Dispatcher.Invoke(() =>
            {

                if (isFullscreen)
                {
                    state = window.WindowState;
                    window.WindowState = WindowState.Maximized;
                    window.WindowStyle = WindowStyle.None;
                  
                }
                else
                {
                    window.WindowState = state;
             
                }
            });
        }


        public NetbirdTab draggingControl;


        public void addTab(NetbirdChromium netbird)
        {
            addTab(netbird, netbird.Title);
        }


        public void addTab(NetbirdChromium netbird, String header)
        {
            window.Dispatcher.Invoke(() =>
            {
                NetbirdTab tab = new NetbirdTab();




                tab.MouseMove += OnTabMouseMove;

                tab.MouseEnter += delegate { currentlyTabHovering = true; };
                tab.MouseLeave += delegate { currentlyTabHovering = false; };

                tab.MouseUp += delegate
                {
                    draggingControl = null;
                    draggingControl = null;
                    tab.isMouseDown = false;
                };
                tabs.Add(netbird);
                // tab.Content = chromium;
                tab.Header = header;
                TabItem virtualTab = new TabItem();
                virtualTab.Content = netbird;

                tab.OnActionButtonClicked += delegate
                {
                   
                    tabControl.Items.Remove(virtualTab);
                    tabs.Remove(netbird);
                    tabsPresenter.Children.Remove(tab);
                    netbird.Dispose();
                   
                };
               

               

               

                tab.PreviewMouseDown += (sender, e) =>
                {
                    if (!tab.ActionButtonHovering)
                    {
                        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                        {
                            tabControl.SelectedIndex = tabControl.Items.IndexOf(virtualTab);
                        }


                        tab.isMouseDown = true;
                        draggingControl = tab;
                    }
                };

                tabControl.Items.Add(virtualTab);
                tabsPresenter.Children.Insert(tabsPresenter.Children.Count - 1, tab);
                tabControl.SelectedIndex = tabControl.Items.Count - 1;

            });
        }

        public int getIndex(NetbirdTab netbirdTab)
        {
            return tabsPresenter.Children.IndexOf(netbirdTab);
        }

        public int getIndex(NetbirdChromium netbirdChromium)
        {
            return tabsPresenter.Children.IndexOf(netbirdChromium);
        }


        private void OnTabMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
           if(draggingControl != null && draggingControl != sender)
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

                TabItem tabItem = (TabItem) tabControl.Items.GetItemAt(indexDragging);
                tabControl.Items.RemoveAt(indexDragging);
                tabControl.Items.Insert(indexDragging + offset, tabItem);

                NetbirdChromium netbirdChromium = tabs[indexDragging];
                tabs.RemoveAt(indexDragging);
                tabs.Insert(indexDragging + offset, netbirdChromium);


                

                Debug.WriteLine("moved -1");
               
            }
           else if(draggingControl == sender)
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

      

        public interface ControllerCallback
        {
            void OnTitleUpdate(NetbirdTab tab, String title);
           
        }
    }
}
