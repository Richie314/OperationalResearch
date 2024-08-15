using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using OperationalResearch.ViewForms;

namespace OperationalResearch.PageForms
{
    public class AboutForm : WebViewForm
    {
        private static string WebPath = Path.Combine(
            "file:///", 
            AppDomain.CurrentDomain.BaseDirectory, 
            "Assets/Credits.html");
        public AboutForm() :base(WebPath) 
        {
            Text = "Credits";
            webView.NavigationStarting += OpenBrowser;
        }
        public static void OpenBrowser(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("file:///"))
            {
                return;
            }
            e.Cancel = true;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri,
                    UseShellExecute = false
                });
            }
            catch
            {
                MessageBox.Show(
                    "Error",
                    "It was impossible to open the default browser",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}
