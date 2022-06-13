using CefSharp;
using CefSharp.Enums;
using CefSharp.Structs;
using System;
using System.Collections.Generic;

namespace Netbird.browser.handlers
{
    class DisplayHandler : IDisplayHandler
    {

        private TabController tabController;


        public DisplayHandler(TabController tabController)
        {
            this.tabController = tabController;

        }


        public void OnAddressChanged(IWebBrowser chromiumWebBrowser, AddressChangedEventArgs addressChangedArgs)
        {
            // throw new NotImplementedException();
        }

        public bool OnAutoResize(IWebBrowser chromiumWebBrowser, IBrowser browser, Size newSize)
        {
            //   throw new NotImplementedException();
            return true;
        }

        public bool OnConsoleMessage(IWebBrowser chromiumWebBrowser, ConsoleMessageEventArgs consoleMessageArgs)
        {
            // throw new NotImplementedException();
            return true;
        }

        public bool OnCursorChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IntPtr cursor, CursorType type, CursorInfo customCursorInfo)
        {
            //  throw new NotImplementedException();
            return false;
        }

        public void OnFaviconUrlChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<string> urls)
        {
            System.Diagnostics.Debug.WriteLine(urls[0]);
            tabController.updateFavicon(urls[0], (NetbirdChromium)chromiumWebBrowser);
            //   throw new NotImplementedException();
        }

        public void OnFullscreenModeChange(IWebBrowser chromiumWebBrowser, IBrowser browser, bool fullscreen)
        {



            tabController.RequestFullscreen(fullscreen);
            // throw new NotImplementedException();
        }

        public void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
        {
            // throw new NotImplementedException();
        }

        public void OnStatusMessage(IWebBrowser chromiumWebBrowser, StatusMessageEventArgs statusMessageArgs)
        {
            //  throw new NotImplementedException();
        }

        public void OnTitleChanged(IWebBrowser chromiumWebBrowser, TitleChangedEventArgs titleChangedArgs)
        {
            // throw new NotImplementedException();
            if (titleChangedArgs.Title.Length > 0)
            {
                tabController.updateTitle(titleChangedArgs.Title, (NetbirdChromium)chromiumWebBrowser);
            }

        }

        public bool OnTooltipChanged(IWebBrowser chromiumWebBrowser, ref string text)
        {
            // throw new NotImplementedException();
            return true;
        }
    }
}
