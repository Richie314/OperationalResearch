using Accord.Math;
using Accord.Math.Optimization;
using Fractions;
using Microsoft.Msagl.Core.ProjectionSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OperationalResearch.Models
{
    internal class QuadProg
    {
        private readonly QuadraticObjectiveFunction f;
        private readonly double[,] A;
        private readonly double[] b;
        public QuadProg(Matrix H, Vector linearPart, Matrix Alin, Vector blin)
        { 
            ArgumentNullException.ThrowIfNull(H, nameof(H));
            ArgumentNullException.ThrowIfNull(linearPart, nameof(linearPart));
            ArgumentNullException.ThrowIfNull(Alin, nameof(Alin));
            ArgumentNullException.ThrowIfNull(blin, nameof(blin));

            if (!H.IsSquare)
            {
                throw new ArgumentException($"Hessian matrix was not square! ({H.Rows}x{H.Cols})");
            }
            if (linearPart.Size != H.Cols)
            {
                throw new ArgumentException($"Linear coefficients vector was of invalid size! (got {linearPart.Size} but {H.Cols} was expected)");
            }

            f = new QuadraticObjectiveFunction(
                H.M.Apply(h => h.ToDouble()), 
                linearPart.Get.Select(x => x.ToDouble()).ToArray());
            if (Alin.Cols != linearPart.Size)
            {
                throw new ArgumentException(
                    $"Matrix A has {Alin.Cols} columns but {linearPart.Size} were expected.");
            }
            if (Alin.Rows != blin.Size)
            {
                throw new ArgumentException(
                    $"Vector b has {blin.Size} elements but {Alin.Rows} were expected.");
            }
            A = Alin.M.Apply(a => a.ToDouble());
            b = blin.Get.Apply(bi => bi.ToDouble());
        }
        private GoldfarbIdnani Solver { get => new GoldfarbIdnani(f, A, b); }
        private Vector? GetSolution(GoldfarbIdnani solver)
        {
            if (solver.Status == GoldfarbIdnaniStatus.NoPossibleSolution)
            {
                return null;
            }
            return solver.Solution.Select(Fraction.FromDouble).ToArray();
        }
        public Vector? Minimize()
        {
            var solver = Solver;
            if (!solver.Minimize())
            {
                return null;
            }
            return GetSolution(solver);
        }
        public Vector? Maximize()
        {
            var solver = Solver;
            if (!solver.Maximize())
            {
                return null;
            }
            return GetSolution(solver);
        }
    }
}
