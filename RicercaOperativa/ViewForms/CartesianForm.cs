using System.Data;
using OperationalResearch.Models;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.ViewForms
{
    internal class CartesianForm : WebViewForm
    {
        private readonly Polyhedron? p;
        private readonly IEnumerable<Point2> Points;
        public CartesianForm(
            IEnumerable<Point2> points,
            Polyhedron? polyhedron = null) : base(WebPath)
        {
            Text = "Cartesian plane view of Polyhedron";
            webView.NavigationCompleted += WebView_NavigationCompleted;
            p = polyhedron;
            if (p is not null && p.Cols != 2)
            {
                // Too many variables
                throw new ArgumentException("Space has too many dimensions to be drawn");
            }
            Points = points;
        }

        private static string WebPath = Path.Combine("file:///", AppDomain.CurrentDomain.BaseDirectory, "Assets/GeoGebraLoader.html");

        private async void WebView_NavigationCompleted(
            object? sender, 
            Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (p is not null)
            {
                const string joinChar = " ∧ ";
                var constraints = p.A.RowsIndeces.Select(
                    row => $"{Function.Print(p.A[row, 0])} * x + {Function.Print(p.A[row, 1])} * y <= {Function.Print(p.b[row])}");
                if (p.ForcePositive)
                {
                    constraints = constraints.Append("x >= 0").Append("y >= 0");
                }
                string eq = string.Join(joinChar, constraints.Select(c => $"({c})"));
                await webView.ExecuteScriptAsync($"GeoGebraEval('{eq}')");
            }
            if (Points is not null)
            {
                foreach (var point in Points)
                {
                    await webView.ExecuteScriptAsync(
                        $"GeoGebraEval('{point.Label.ToUpper()} = ({Function.Print(point.x)}, {Function.Print(point.y)})')");
                }
            }
        }
    }
}
