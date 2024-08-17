using System.Data;
using OperationalResearch.Models;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.ViewForms
{
    internal class CartesianForm : WebViewForm
    {
        private readonly Polyhedron? p;
        private readonly IEnumerable<Point2> Points;
        private readonly IEnumerable<Tuple<Point2, Point2>?> Vectors;
        public CartesianForm(
            IEnumerable<Point2> points,
            Polyhedron? polyhedron = null,
            IEnumerable<Tuple<Point2, Point2>?>? vectors = null) : base(WebPath)
        {
            ArgumentNullException.ThrowIfNull(points, nameof(points));
            Text = "Cartesian plane view of Polyhedron";
            webView.NavigationCompleted += WebView_NavigationCompleted;
            p = polyhedron;
            if (p is not null && p.Cols != 2)
            {
                // Too many variables
                throw new ArgumentException("Space has too many dimensions to be drawn");
            }
            Points = points;
            Vectors = vectors ?? new List<Tuple<Point2, Point2>?>();
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
            foreach (var point in Points)
            {
                await AddPoint(point);
            }
            foreach (var vector in Vectors)
            {
                if (vector is null) continue;
                var From = vector.Item1;
                var To = From + vector.Item2;
                string vLabel = vector.Item2.Label.ToLower().Replace(" ", "");

                string fromLabel = await AddPoint(From);
                string toLabel = await AddPoint(To);


                await webView.ExecuteScriptAsync(
                    $"GeoGebraEval('{vLabel} = Vector({fromLabel}, {toLabel})')");

                await webView.ExecuteScriptAsync($"HidePoint('{fromLabel}')");
                await webView.ExecuteScriptAsync($"HidePoint('{toLabel}')");
            }
        }

        private async Task<string> AddPoint(Point2 point)
        {
            string s = point.Label.ToUpper().Replace(" ", "");
            await webView.ExecuteScriptAsync(
                    $"GeoGebraEval('{s} = ({Function.Print(point.x)}, {Function.Print(point.y)})')");
            return s;
        }
    }
}
