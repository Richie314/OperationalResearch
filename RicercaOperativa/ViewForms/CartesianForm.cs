using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OperationalResearch.Models;

namespace OperationalResearch.ViewForms
{
    internal class CartesianForm : WebViewForm
    {
        private readonly Matrix? Ab;
        private readonly Point2[]? Points;
        public CartesianForm(
            Matrix? Ab = null,
            Point2[]? Points = null) : base(
            Path.Combine(
                "file:///",
                AppDomain.CurrentDomain.BaseDirectory, 
                "Assets/GeoGebraLoader.html"))
        {
            webView.NavigationCompleted += WebView_NavigationCompleted;
            this.Ab = Ab;
            this.Points = Points;
        }


        private async void WebView_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (Ab is not null && Ab.Cols >= 3)
            {
                const string joinChar = " ∧ ";
                string eq = string.Join(joinChar,
                    Ab.RowsIndeces.Select(row => $"({Function.Print(Ab[row, 0])} * x + {Function.Print(Ab[row, 1])} * y <= {Function.Print(Ab[row, 2])})")
                );
                await webView.ExecuteScriptAsync($"GeoGebraEval('{eq}')");
            }
            if (Points is not null)
            {
                foreach (var point in Points)
                {
                    await webView.ExecuteScriptAsync(
                        $"GeoGebraEval('{Function.Print(point.x)}, {Function.Print(point.y)} ')");
                }
            }
        }
    }
}
