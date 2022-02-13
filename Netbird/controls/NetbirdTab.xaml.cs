using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для NetbirdTab.xaml
    /// </summary>
    public partial class NetbirdTab : TabItem
    {
        public event Action OnActionButtonClicked = delegate { };

        public bool ActionButtonHovering = false;
        public bool isMouseDown { get; set; }

        public NetbirdTab()
        {
            InitializeComponent();
            BitmapImage actionButtonImage = new BitmapImage();
            actionButtonImage.BeginInit();
            actionButtonImage.UriSource = new Uri(@"/Netbird;component/Resources/close.png", UriKind.Relative);
            actionButtonImage.EndInit();
            SetActionButton(this, actionButtonImage);
        }
     
        public void Button_Click(object sender, MouseButtonEventArgs e)
        {
            OnActionButtonClicked();
        }

        public void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ActionButtonHovering = true;
            BitmapImage actionButtonImage = new BitmapImage();
            actionButtonImage.BeginInit();
            actionButtonImage.UriSource = new Uri(@"/Netbird;component/Resources/close_hovered.png", UriKind.Relative);
            actionButtonImage.EndInit();
            SetActionButton(this, actionButtonImage);
        }

        public void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ActionButtonHovering = false;
            BitmapImage actionButtonImage = new BitmapImage();
            actionButtonImage.BeginInit();
            actionButtonImage.UriSource = new Uri(@"/Netbird;component/Resources/close.png", UriKind.Relative);
            actionButtonImage.EndInit();
            SetActionButton(this, actionButtonImage);
        }

        public static readonly DependencyProperty FaviconProperty =
    DependencyProperty.Register(
      name: "Favicon",
      propertyType: typeof(BitmapImage),
      ownerType: typeof(NetbirdTab),
      typeMetadata: new FrameworkPropertyMetadata(
          defaultValue: new BitmapImage(),
          flags: FrameworkPropertyMetadataOptions.AffectsRender));


     

        public static void SetFavicon(UIElement element, BitmapImage value)
        {
            element.SetValue(FaviconProperty, value);
        }

        public static void SetActionButton(UIElement element, BitmapImage value)
        {
            element.SetValue(ActionButtonProperty, value);
        }

        public BitmapImage Favicon
        {
            get => (BitmapImage)GetValue(FaviconProperty);
            set => SetValue(FaviconProperty, value);
        }

        public static readonly DependencyProperty ActionButtonProperty =
      DependencyProperty.Register(
      name: "ActionButton",
      propertyType: typeof(BitmapImage),
      ownerType: typeof(NetbirdTab),
      typeMetadata: new FrameworkPropertyMetadata(
       defaultValue: new BitmapImage(),
       flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public BitmapImage ActionButton
        {
            get => (BitmapImage)GetValue(FaviconProperty);
            set => SetValue(FaviconProperty, value);
        }


        public void SetFavicon(String link)
        {
            Debug.WriteLine("Получена ссылка: " + link);

            var fullFilePath = @"" + link;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(fullFilePath, UriKind.Absolute);
          
            bitmapImage.EndInit();

            SetFavicon(this, bitmapImage);
        }

      

       
    }
}
