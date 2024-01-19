using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RicercaOperativa.Models
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
        public static string Print(Fraction? x)
        {
            if (!x.HasValue)
            {
                return "null";
            }
            string s = x.Value.ToString();
            if (s.Length < 15)
                return s;
            return x.Value.ToDecimal().ToString("N5");
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
    }
}
