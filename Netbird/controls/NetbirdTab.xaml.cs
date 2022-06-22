using Netbird.browser.handlers;
using Svg;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Netbird.controls
{


    /// <summary>
    /// Логика взаимодействия для NetbirdTab.xaml
    /// </summary>
    public partial class NetbirdTab : TabItem
    {
        public event Action OnActionButtonClicked = delegate { };

        public static int DEFAULT_MARGIN = 10;
        public static int DEFAULT_PADDING_HORIZONTAL = 15;
        public static int DEFAULT_PADDING_VERTICAL = 10;

        public bool ActionButtonHovering = false;
        private bool updateLock = true;
        private String header = "";

        public TabController tabController;
        public String loadedFavicon = "";
        public bool isMouseDown { get; set; }

        public NetbirdTab()
        {
            InitializeComponent();
            BitmapImage actionButtonImage = new BitmapImage();
            actionButtonImage.BeginInit();
            actionButtonImage.UriSource = new Uri(@"/Netbird;component/Resources/close.png", UriKind.Relative);
            actionButtonImage.EndInit();
            SetActionButton(this, actionButtonImage);
            SetPlaceholder();
            this.MouseEnter += NetbirdTab_MouseEnter;
        }

        private void NetbirdTab_MouseEnter(object sender, MouseEventArgs e)
        {
         
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            if (tabController.selectedTab != this)
            {
                tabController.selectedTab = this;
            }
         
            UpdateTab();
            base.OnSelected(e);
        
        }



        public void Button_Click(object sender, MouseButtonEventArgs e)
        {
            if (updateLock) return;
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


        public void SetPlaceholder()
        {
            BitmapImage placeholder = new BitmapImage();
            placeholder.BeginInit();
            placeholder.UriSource = new Uri(@"/Netbird;component/Resources/favicon_placeholder.png", UriKind.Relative);
            placeholder.EndInit();
            SetFavicon(this, placeholder);
        }
        public void SetFavicon(String link)
        {
            if (link == loadedFavicon) return;
            BitmapImage placeholder = new BitmapImage();
            placeholder.BeginInit();
            placeholder.UriSource = new Uri(@"/Netbird;component/Resources/favicon_placeholder.png", UriKind.Relative);
            placeholder.EndInit();
            SetFavicon(this, placeholder);
            Debug.WriteLine("Получена ссылка: " + link);
            try
            {
                if (!link.EndsWith(".svg"))
                {
                
                    if (link.EndsWith(".ico"))
                    {
                        WebRequest request = (HttpWebRequest)WebRequest.Create(link);

                        System.Drawing.Bitmap bm = new System.Drawing.Bitmap(32, 32);
                        MemoryStream memStream;

                        using (Stream response = request.GetResponse().GetResponseStream())
                        {
                            memStream = new MemoryStream();
                            byte[] buffer = new byte[1024];
                            int byteCount;

                            do
                            {
                                byteCount = response.Read(buffer, 0, buffer.Length);
                                memStream.Write(buffer, 0, byteCount);
                            } while (byteCount > 0);
                        }

                        bm = new System.Drawing.Bitmap(System.Drawing.Image.FromStream(memStream));
                     
                        SetFavicon(this, Bitmap2BitmapImage(bm));
                    }
                    else
                    {
                         BitmapImage bitmap = new BitmapImage();
                          bitmap.BeginInit();
                         bitmap.UriSource = new Uri(@link, UriKind.Absolute);
                          bitmap.EndInit();

                        loadedFavicon = link;
                        SetFavicon(this, bitmap);
                    }
                }
                else
                {
                    
                    var webRequest = WebRequest.CreateDefault(new Uri(link));

                    webRequest.ContentType = "image/ico";
                    webRequest.BeginGetResponse((ar) =>

                {

                    var response = webRequest.EndGetResponse(ar);

                    var stream = response.GetResponseStream();

                    if (stream.CanRead)

                    {

                        Byte[] buffer = new Byte[response.ContentLength];

                        stream.BeginRead(buffer, 0, buffer.Length, (aResult) =>

                        {


                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate

                            {
                                loadedFavicon = link;
                                SetFavicon(this, TextToBitmap(new MemoryStream(buffer)));
                            }));



                        }, null);

                    }

                }, null);
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }

        Image btnClose;
        Image favicon;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        private BitmapImage TextToBitmap(MemoryStream mStream)
        {
           

                SvgDocument svgDoc = SvgDocument.Open<SvgDocument>(mStream, null);

                return Bitmap2BitmapImage(svgDoc.Draw());
            
           
        }
        public void UpdateTab()
        {
            if (tabController.selectedTab != this)
            {
                if ((String)this.Header == "") this.Header = header;
            }
                if (updateLock) return;
            if (btnClose == null) return;
            if (favicon == null) return;
          
            if (Width < 100)
            {
                btnClose.Visibility = Visibility.Collapsed;
                btnClose.IsEnabled = false;
                favicon.Margin = new Thickness(0, 0, 1, 0);
                btnClose.Margin = new Thickness(0, 0, 3, 0);
                Padding = new Thickness(5, DEFAULT_PADDING_VERTICAL, 5, DEFAULT_PADDING_VERTICAL);
            }
            else
            {
                btnClose.Visibility = Visibility.Visible;
                btnClose.IsEnabled = true;
                favicon.Margin = new Thickness(0, 0, DEFAULT_MARGIN, 0);
                btnClose.Margin = new Thickness(0, 0, DEFAULT_MARGIN, 0);
                Padding = new Thickness(DEFAULT_PADDING_HORIZONTAL, DEFAULT_PADDING_VERTICAL,
                    DEFAULT_PADDING_HORIZONTAL, DEFAULT_PADDING_VERTICAL);
            }

            if (tabController.selectedTab == this)
            {
                btnClose.Visibility = Visibility.Visible;
                btnClose.IsEnabled = true;
             
                if (Width < 50)
                {
                    if ((String)this.Header != "")
                    {
                        header = (String)this.Header;
                        this.Header = "";
                    }
                    Width = 50;
                }
            }
            else
            {
                if((String) this.Header == "") this.Header = header;

            }
        }
        private void netbirdItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTab();
        }

        

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            btnClose = (Image)sender;
            UpdateTab();
        }

        private void btnClose_Loaded(object sender, RoutedEventArgs e)
        {
            favicon = (Image)sender;
            UpdateTab();
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            updateLock = false;
            UpdateTab();
        }

        public static readonly RoutedEvent ConditionalMouseEnterEvent = EventManager.RegisterRoutedEvent(
      name: "ConditionalMouseEnter",
      routingStrategy: RoutingStrategy.Bubble,
      handlerType: typeof(RoutedEventHandler),
      ownerType: typeof(NetbirdTab));

        // Provide CLR accessors for assigning an event handler.
        public event RoutedEventHandler ConditionalMouseEnter
        {
            add { AddHandler(ConditionalMouseEnterEvent, value); }
            remove { RemoveHandler(ConditionalMouseEnterEvent, value); }
        }

        void RaiseConditionalMouseEnterEvent()
        {
            // Create a RoutedEventArgs instance.
            RoutedEventArgs routedEventArgs = new(routedEvent: ConditionalMouseEnterEvent);

            // Raise the event, which will bubble up through the element tree.
            RaiseEvent(routedEventArgs);
        }


        public static readonly RoutedEvent ConditionalMouseLeaveEvent = EventManager.RegisterRoutedEvent(
      name: "ConditionalMouseLeave",
      routingStrategy: RoutingStrategy.Bubble,
      handlerType: typeof(RoutedEventHandler),
      ownerType: typeof(NetbirdTab));

        // Provide CLR accessors for assigning an event handler.
        public event RoutedEventHandler ConditionalMouseLeave
        {
            add { AddHandler(ConditionalMouseLeaveEvent, value); }
            remove { RemoveHandler(ConditionalMouseLeaveEvent, value); }
        }

        void RaiseConditionalMouseLeaveEvent()
        {
            // Create a RoutedEventArgs instance.
            RoutedEventArgs routedEventArgs = new(routedEvent: ConditionalMouseLeaveEvent);

            // Raise the event, which will bubble up through the element tree.
            RaiseEvent(routedEventArgs);
        }

        private void netbirdItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (tabController.selectedTab != this) RaiseConditionalMouseEnterEvent();
        }

        private void netbirdItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (tabController.selectedTab != this) RaiseConditionalMouseLeaveEvent();
        }

       
    }
}
