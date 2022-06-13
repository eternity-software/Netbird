using CefSharp;
using CefSharp.Wpf;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Netbird.browser.handlers
{
    /// <summary>
    /// LifeSpanHandler implementation that demos hosting a popup in a new ChromiumWebBrowser instance.
    /// This example code is EXPERIMENTAL
    /// </summary>
    public class ExperimentalLifespanHandler : ILifeSpanHandler
    {

        private TabController tabController;

        public ExperimentalLifespanHandler(TabController tabController)
        {
            this.tabController = tabController;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        private static string GetWindowTitle(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            //Set newBrowser to null unless your attempting to host the popup in a new instance of ChromiumWebBrowser
            //newBrowser = null;

            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            ChromiumWebBrowser popupChromiumWebBrowser = null;

            var windowX = (windowInfo.X == int.MinValue) ? double.NaN : windowInfo.X;
            var windowY = (windowInfo.Y == int.MinValue) ? double.NaN : windowInfo.Y;
            var windowWidth = (windowInfo.Width == int.MinValue) ? double.NaN : windowInfo.Width;
            var windowHeight = (windowInfo.Height == int.MinValue) ? double.NaN : windowInfo.Height;

            tabController.window.Dispatcher.Invoke(() =>
            {
                var owner = Window.GetWindow(chromiumWebBrowser);
                popupChromiumWebBrowser = new NetbirdChromium(tabController);

                popupChromiumWebBrowser.SetAsPopup();
                popupChromiumWebBrowser.LifeSpanHandler = this;

                var popup = new Window
                {
                    Left = windowX,
                    Top = windowY,
                    Width = windowWidth,
                    Height = windowHeight,
                    Content = popupChromiumWebBrowser,
                    Owner = owner,
                    Title = targetFrameName
                };

                var windowInteropHelper = new WindowInteropHelper(popup);
                //Create the handle Window handle (In WPF there's only one handle per window, not per control)
                var handle = windowInteropHelper.EnsureHandle();

                //The parentHandle value will be used to identify monitor info and to act as the parent window for dialogs,
                //context menus, etc. If parentHandle is not provided then the main screen monitor will be used and some
                //functionality that requires a parent window may not function correctly.
                windowInfo.SetAsWindowless(handle);

                tabController.addTab((NetbirdChromium)popupChromiumWebBrowser, tabController.getIndex(tabController.selectedTab));

                popup.Closed += (o, e) =>
                {

                };
            });

            newBrowser = popupChromiumWebBrowser;

            return false;
        }

        void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            if (!browser.IsDisposed && browser.IsPopup)
            {
                var windowTitle = GetWindowTitle(browser.GetHost().GetWindowHandle());

                //CEF doesn't currently provide an option to determine if the new Popup is
                //DevTools so we use a hackyworkaround to check the Window Title.
                //DevTools is hosted in it's own popup, we don't perform any action here
                if (windowTitle != "DevTools")
                {
                    var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

                    chromiumWebBrowser.Dispatcher.Invoke(() =>
                    {
                        var owner = Window.GetWindow(chromiumWebBrowser);

                        if (owner != null && owner.Content == browserControl)
                        {
                            owner.Show();
                        }
                    });
                }
            }
        }

        bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        void ILifeSpanHandler.OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            if (!browser.IsDisposed && browser.IsPopup)
            {
                //DevTools is hosted in it's own popup, we don't perform any action here
                if (!browser.MainFrame.Url.Equals("devtools://devtools/devtools_app.html"))
                {
                    var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

                    chromiumWebBrowser.Dispatcher.Invoke(() =>
                    {
                        var owner = Window.GetWindow(chromiumWebBrowser);

                        if (owner != null && owner.Content == browserControl)
                        {
                            owner.Close();
                        }
                    });
                }
            }
        }
    }
}
