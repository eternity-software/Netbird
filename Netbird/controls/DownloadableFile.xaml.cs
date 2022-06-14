using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для DownloadableFile.xaml
    /// </summary>
    /// 

    public partial class DownloadableFile : UserControl
    {

        public String filePath;
       
        public DownloadableFile()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (filePath == null) return;
            System.Diagnostics.Process.Start(filePath);

        }

        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Process.Start(App.downloadsFolder);
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
             
            if ( e.LeftButton == MouseButtonState.Pressed)
            {
              
              
                    //TODO : If you want to delete you need to use FileWatchers to catch the drop events and delete from temp.
                    //But as long as it is temp the users/admins can delete it when required.
                    DataObject dragObj = new DataObject();
                    dragObj.SetFileDropList(new System.Collections.Specialized.StringCollection() { filePath });
                    DragDrop.DoDragDrop(this, dragObj, DragDropEffects.Copy);
                

            }
        }
    }
}
