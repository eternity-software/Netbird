using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            int counter = 1;
            tabController.controllerCallback.OnFileDownloadBegins(downloadItem);

            String path = App.downloadsFolder + "/" + downloadItem.SuggestedFileName;
            while(File.Exists(path))
            {
                path = App.downloadsFolder + "/" + Path.GetFileNameWithoutExtension(downloadItem.SuggestedFileName) + " (" + counter + ")" + Path.GetExtension(downloadItem.SuggestedFileName);
                counter++;
            }
            callback.Continue(path, false);
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
