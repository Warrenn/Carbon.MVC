using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System;

namespace CarbonKnown.Print
{
    /// <summary>
    /// Browser derived from ApplicationContext supports message loop
    /// </summary>
    public class Browser : ApplicationContext
    {
        private readonly AutoResetEvent resetEvent;
        private WebBrowser ieBrowser;
        private Thread thrd;
        public Bitmap BitmapResult { get; private set; }
        public bool BusyProcessing { get; private set; }

        public Browser(Uri url, AutoResetEvent resetEvent)
        {
            this.resetEvent = resetEvent;
            Init(browser => browser.Navigate(url));
        }

        public Browser(string htmlContents, AutoResetEvent resetEvent)
        {
            this.resetEvent = resetEvent;
            Init(browser =>
                {
                    browser.DocumentText = htmlContents;
                });
        }

        private void Init(Action<WebBrowser> loadContents)
        {
            BusyProcessing = false;
            BitmapResult = null;
            thrd = new Thread(() =>
                {
                    // initialize the WebBrowser and the form
                    ieBrowser = new WebBrowser
                        {
                            ScriptErrorsSuppressed = true,
                            ScrollBarsEnabled = false
                        };
                    ieBrowser.DocumentCompleted += IEBrowser_DocumentCompleted;
                    loadContents(ieBrowser);
                    BusyProcessing = true;
                    Application.Run(this);
                });
            // set thread to STA state before starting
            thrd.SetApartmentState(ApartmentState.STA);
            thrd.Start();
        }

        // dipose the WebBrowser control and the form and its controls
        protected override void Dispose(bool disposing)
        {
            if (thrd != null)
            {
                thrd.Abort();
                thrd = null;
                return;
            }

            Marshal.Release(ieBrowser.Handle);
            ieBrowser.Dispose();
            base.Dispose(disposing);
        }


        // DocumentCompleted event handle
        private void IEBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var doc = ((WebBrowser) sender).Document;
            var body = doc.Body;

            ieBrowser.ScrollBarsEnabled = false;
            ieBrowser.ClientSize = new Size(body.ScrollRectangle.Width, body.ScrollRectangle.Bottom);
            BitmapResult = new Bitmap(body.ScrollRectangle.Width, body.ScrollRectangle.Bottom);
            ieBrowser.BringToFront();
            ieBrowser.DrawToBitmap(BitmapResult, ieBrowser.Bounds);
            BusyProcessing = false;
            resetEvent.Set();
        }
    }
}
