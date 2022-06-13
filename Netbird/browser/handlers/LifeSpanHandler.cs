using CefSharp;

namespace Netbird.browser.handlers
{
    class LifeSpanHandler : ILifeSpanHandler
    {

        private TabController tabController;

        public LifeSpanHandler(TabController tabController)
        {
            this.tabController = tabController;
        }

        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            //throw new NotImplementedException();
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            // throw new NotImplementedException();
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {

            tabController.addTab(targetUrl, tabController.getIndex(tabController.selectedTab));



            newBrowser = null;
            return true;
        }
    }
}
