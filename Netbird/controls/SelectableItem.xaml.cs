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
    /// Логика взаимодействия для SelectableItem.xaml
    /// </summary>
    public partial class SelectableItem : UserControl
    {
        public SelectableItemCallback selectableItemCallback;
        public interface SelectableItemCallback
        {
            void OnMouseDown();
        }
        public SelectableItem()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(selectableItemCallback != null)
            {
                selectableItemCallback.OnMouseDown();
            }
        }
    }
}
