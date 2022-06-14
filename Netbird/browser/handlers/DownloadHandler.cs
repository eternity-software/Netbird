using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Enums;
using CefSharp.Structs;

namespace Netbird.browser.handlers
{
    public class DownloadHandler : IDownloadHandler
    {

        private TabController tabController;

        public DownloadHandler(TabController tabController)
        {
            this.tabController = tabController;
        }

        bool IDownloadHandler.CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return true;
        }

        void IDownloadHandler.OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            tabController.controllerCallback.OnFileDownloadBegins(downloadItem);
            callback.Continue(App.downloadsFolder + "/" + downloadItem.SuggestedFileName, false);
        }

        void IDownloadHandler.OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            callback.Resume();
           
            try
            {
                tabController.controllerCallback.OnFileDownloadUpdate(downloadItem);
            }
            catch(Exception e)
            {
                MessageBox.Show("errro!");
                Debug.Write(e.ToString());

            }
           
         
        }
    }
}
