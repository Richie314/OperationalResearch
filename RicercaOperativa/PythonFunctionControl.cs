using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationalResearch
{
    public partial class PythonFunctionControl : UserControl
    {
        private int spaceDimension = 0;
        private static string WebPath = 
            Path.Combine("file:///", 
                AppDomain.CurrentDomain.BaseDirectory, 
                "Assets/AceCodeContainer.html");
        private bool initialized = false;
        private string? startText = null;

        public PythonFunctionControl()
        {
            InitializeComponent();
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.Source = new Uri(WebPath);
        }
        private async void WebView_NavigationCompleted(
            object? sender,
            Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            initialized = true;
            await setCode(startText);
        }

        public async Task<string> getCode() =>
            (await webView.ExecuteScriptAsync("getEditorText()"))
            .Replace("\\n", Environment.NewLine)
            .Replace("\\\n", Environment.NewLine)
            .Replace("\\t", "\t")
            .Replace("\\", string.Empty)
            .Replace("\"", string.Empty)
            .Replace("'", string.Empty);

        public async Task<bool> setCode(string? s)
        {
            if (!initialized)
            {
                startText = s;
                return false;
            }
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }
            await webView.ExecuteScriptAsync($"setEditorText('{s.ReplaceLineEndings().Replace(Environment.NewLine, "\\n").Replace("'", "\\'")}')");
            return true;
        }

        private string getTemplateCode(int size)
        {

            string libs = "import math" + Environment.NewLine;
            string comment =
                "# Expect input to be made of floats" + Environment.NewLine +
                "# Return a single float in the main function and a tuple in its gradient" + Environment.NewLine +
                "# Use 4 spaces instead of tabs!" + Environment.NewLine;
            IEnumerable<string> xArray = Enumerable.Range(1, size).Select(i => $"x{i}");
            IEnumerable<string> xFloatArray = xArray.Select(x => $"{x}: float");
            IEnumerable<string> xSquareArray = xArray.Select(x => $"{x}**2");
            string f =
                "def f(" + string.Join(", ", xFloatArray.ToArray()) + "):" + Environment.NewLine +
                "    return (" + string.Join(" + ", xSquareArray.ToArray()) + ") / 2" + Environment.NewLine;
            string grad =
                "def gradF(" + string.Join(", ", xFloatArray.ToArray()) + "):" + Environment.NewLine +
                "    return " + string.Join(", ", xArray.ToArray()) + Environment.NewLine;
            return
                libs +
                comment +
                Environment.NewLine +
                f +
                Environment.NewLine +
                grad;
        }

        private async Task setTemplateCode(int size) => await setCode(getTemplateCode(size));

        public async Task setSize(int newSize)
        {
            if (newSize == spaceDimension)
            {
                return;
            }
            spaceDimension = newSize;
            await setTemplateCode(spaceDimension);
        }
    }
}
