using Netbird.browser.handlers;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Netbird.controls
{
    /// <summary>
    /// Логика взаимодействия для MainTab.xaml
    /// </summary>
    public partial class MainTab : UserControl
    {
        private TabController tabController;
        private NetbirdTab tab;
        public MainTab(TabController tabController, NetbirdTab netbirdTab)
        {
            InitializeComponent();
            this.tabController = tabController;
            this.tab = netbirdTab;
            searchBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = tabController.getIndex(tab);
            tabController.CloseTab(tab);
            if (index == 0) index = 1;
            tabController.addTab("https://yandex.com", index - 1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int index = tabController.getIndex(tab);
            tabController.CloseTab(tab);
            if (index == 0) index = 1;
            tabController.addTab("https://vk.com", index - 1);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int index = tabController.getIndex(tab);
            tabController.CloseTab(tab);
            if (index == 0) index = 1;
            tabController.addTab("https://youtube.com", index - 1);
        }

        private void searchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
          
            if (e.Key != Key.Enter) return;
            

            int index = tabController.getIndex(tab);
            tabController.CloseTab(tab);
            String query = "https://yandex.ru/search/?text=" + searchBox.Text + "&lr=2";
            tabController.addTab(query, index - 1);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchBox.Focus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            searchBox.Focus();
        }

        private void searchBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
          
        }

        private void searchHint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchBox.Focus();
        }

        private void searchHint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            searchBox.Focus();
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {


          
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text.Length > 0)
            {
                searchHint.Visibility = Visibility.Collapsed;
            }
            else
            {
                searchHint.Visibility = Visibility.Visible;
            }
        }
    }
}
