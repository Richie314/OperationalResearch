using Fractions;
using OperationalResearch.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.NonLinearOptimization.QuadProg
{
    public class QuadProgFunctionAnalyzer : NonLinearOptimizer
    {
        private readonly Matrix H;
        private readonly Vector lin;
        public override Vector Gradient(Vector x)
        {
            return H * x + lin;
        }
        public override Fraction Function(Vector x)
        {
            var x1 = x[0];
            var x2 = x[1];
            var a = H[0, 0] / 2;
            var b = H[0, 1];
            var c = H[1, 1] / 2;
            return a * x1 * x1 + c * x2 * x2 + b * x1 * x2 + x * lin;
        }

        public QuadProgFunctionAnalyzer(Polyhedron P, Matrix H, Vector l) : base(P)
        {
            ArgumentNullException.ThrowIfNull(H, nameof(H));
            ArgumentNullException.ThrowIfNull(l, nameof(l));
            if (!H.IsSquare)
            {
                throw new ArgumentException($"Hessian matrix was not square! ({H.Rows}x{H.Cols})");
            }
            if (l.Size != H.Cols)
            {
                throw new ArgumentException(
                    $"Linear coefficients vector was of invalid size! (got {l.Size} but {H.Cols} was expected)");
            }
            this.H = H;
            lin = l;
        }
    }
}
