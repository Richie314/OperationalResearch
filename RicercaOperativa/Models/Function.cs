using Fractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationalResearch.Models.Graphs;

namespace OperationalResearch.Models
{
    internal static class Function
    {
        public static Fraction FindArgMin(
            Func<Fraction, Fraction> func, 
            Fraction t_start, Fraction t_end, int steps)
        {
            if (t_start == t_end)
            {
                return t_start;
            }

            Fraction MinY = func(t_start);
            Fraction MinT = t_start;

            Fraction dt = (t_end - t_start) / steps;
            Fraction currT = t_start + dt;

            while (currT <= t_end)
            {
                Fraction CurrY = func(currT);
                if (CurrY < MinY)
                {
                    MinY = CurrY;
                    MinT = currT;
                }

                currT += dt;
            }
            return MinT;
        }

        public static Fraction FindArgMax(
            Func<Fraction, Fraction> func,
            Fraction t_start, Fraction t_end, int steps)
        {
            if (t_start == t_end)
            {
                return t_start;
            }

            Fraction MaxY = func(t_start);
            Fraction MaxT = t_start;

            Fraction dt = (t_end - t_start) / steps;
            Fraction currT = t_start + dt;

            while (currT <= t_end)
            {
                Fraction CurrY = func(currT);
                if (CurrY > MaxY)
                {
                    MaxY = CurrY;
                    MaxT = currT;
                }

                currT += dt;
            }
            return MaxT;
        }

        public static string Print(Fraction? x)
        {
            if (!x.HasValue)
            {
                return "null";
            }
            string s = x.Value.ToString();
            if (s.Length < 12)
                return s;
            return x.Value.ToDouble().ToString("N5", CultureInfo.InvariantCulture);
        }
        public static string Print<EdgeType>(IEnumerable<EdgeType>? edges) where EdgeType : Edge
        {
            if (edges is null)
            {
                return "null";
            }
            return "{ " + string.Join(", ", edges.Select(e => e.ToString())) + " }";
        }
        public static string Print(IEnumerable<int> x, bool addOne = true)
        {
            if (x is null)
            {
                return "null";
            }
            if (!x.Any())
            {
                return "{ }";
            }
            int y = addOne ? 1 : 0;
            return "{ " + string.Join(", ", x.Select(x => (x + y).ToString())) + " }";
        }

        public static int Factorial(int n) =>
            n <= 1 ? 1 : n * Factorial(n - 1);
    }
}
