using Accord.Math;
using Accord.Math.Optimization;
using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrix = OperationalResearch.Models.Elements.Matrix;
using Vector = OperationalResearch.Models.Elements.Vector;

namespace OperationalResearch.Models
{
    internal class QuadProg
    {
        private readonly QuadraticObjectiveFunction f;
        private readonly Polyhedron P;
        public QuadProg(Matrix H, Vector linearPart, Polyhedron p)
        {
            ArgumentNullException.ThrowIfNull(H, nameof(H));
            ArgumentNullException.ThrowIfNull(linearPart, nameof(linearPart));
            ArgumentNullException.ThrowIfNull(p, nameof(p));

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
            P = p;
            if (P.Cols != linearPart.Size)
            {
                throw new ArgumentException(
                    $"Matrix A has {P.Cols} columns but {linearPart.Size} were expected.");
            }
        }
        private GoldfarbIdnani Solver
        {
            get => new(
            function: f,
            constraintMatrix: P.A.M.Apply(a => a.ToDouble()),
            constraintValues: P.b.ToDouble().ToArray());
        }
        private static Vector? GetSolution(GoldfarbIdnani solver)
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

        public async Task<bool> MinimizeFlow(IndentWriter? Writer)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                await Writer.WriteLineAsync("Finding min through Accord.Math.QuadProg");
                Vector? x = Minimize();
                if (x is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved");
                    return false;
                }

                await Writer.WriteLineAsync($"X = {x}");
                return true;
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }

        }
        public async Task<bool> MaximizeFlow(IndentWriter? Writer)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                await Writer.WriteLineAsync("Finding max through Accord.Math.QuadProg");
                Vector? x = Maximize();
                if (x is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved");
                    return false;
                }

                await Writer.WriteLineAsync($"X = {x}");
                return true;
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }
    }
}
