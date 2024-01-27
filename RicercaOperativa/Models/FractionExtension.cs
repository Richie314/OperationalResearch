using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;

namespace OperationalResearch.Models
{
    public static class FractionExtension
    {
        public static Fraction Floor(this Fraction x)
        {
            return Fraction.FromDouble(Math.Floor(x.ToDouble()));
        }
        public static Fraction FractionPart(this Fraction x)
        {
            return x - Fraction.FromDouble(Math.Floor(x.ToDouble()));
        }
    }
}
