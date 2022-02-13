using System;
using System.Collections.Generic;
using System.Linq;
using CefSharp;
using System.Text;
using System.Threading.Tasks;

namespace Netbird.browser.handlers
{
    class LoadErrorHandler : ILoadHandler
    {
        public void OnFrameLoadEnd(IWebBrowser chromiumWebBrowser, FrameLoadEndEventArgs frameLoadEndArgs)
        {
          //  throw new NotImplementedException();
        }

        public void OnFrameLoadStart(IWebBrowser chromiumWebBrowser, FrameLoadStartEventArgs frameLoadStartArgs)
        {
           // throw new NotImplementedException();
        }

        public void OnLoadError(IWebBrowser chromiumWebBrowser, LoadErrorEventArgs loadErrorArgs)
        {
            chromiumWebBrowser.GetMainFrame().LoadHtml("Netbird Error: " + loadErrorArgs.ErrorText + " #" + loadErrorArgs.ErrorCode);
           // throw new NotImplementedException();
        }

        public void OnLoadingStateChange(IWebBrowser chromiumWebBrowser, LoadingStateChangedEventArgs loadingStateChangedArgs)
        {
          //  throw new NotImplementedException();
        }
    }
}
