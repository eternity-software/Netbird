using System;
using System.Collections.Generic;
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
        bool IDownloadHandler.CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return true;
        }

        void IDownloadHandler.OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            callback.Continue(App.downloadsFolder + "/" + downloadItem.SuggestedFileName, false);
        }

        void IDownloadHandler.OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
           if(downloadItem.IsComplete)
            {
                MessageBox.Show("File " + downloadItem.SuggestedFileName + " downloaded.", "Netbird Downloader");
            }
        }
    }
}
