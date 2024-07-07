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
                linearPart.ToDouble().ToArray());
            P = p;
            if (P.Cols != linearPart.Size)
            {
                throw new ArgumentException(
                    $"Matrix A has {P.Cols} columns but {linearPart.Size} were expected.");
            }
        }
        private IEnumerable<LinearConstraint> GetConstraints()
        {
            List<LinearConstraint> constraints = [];
            var A = P.GetMatrix();
            var b = P.GetVector();
            foreach (var i in P.AllRows)
            {
                constraints.Add(new LinearConstraint(numberOfVariables: A.Cols) { 
                    CombinedAs = A[i].ToDouble().ToArray(),
                    ShouldBe = ConstraintType.LesserThanOrEqualTo,
                    Value = b[i].ToDouble()
                });
            }
            return constraints;
        }

        private GoldfarbIdnani Solver
        {
            get => new(function: f, constraints: GetConstraints());
        }
        private static Tuple<Vector, Fraction, Vector>? GetSolution(GoldfarbIdnani solver)
        {
            if (solver.Status == GoldfarbIdnaniStatus.NoPossibleSolution)
            {
                return null;
            }
            return new(
                Vector.FromDouble(solver.Solution),
                Fraction.FromDouble(solver.Value),
                Vector.FromDouble(solver.Lagrangian));
        }
        public Tuple<Vector, Fraction, Vector>? Minimize()
        {
            var solver = Solver;
            if (!solver.Minimize())
            {
                return null;
            }
            return GetSolution(solver);
        }
        public Tuple<Vector, Fraction, Vector>? Maximize()
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
                Tuple<Vector, Fraction, Vector>? sol = Minimize();
                if (sol is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved");
                    return false;
                }

                await Writer.WriteLineAsync($"X = {sol.Item1} -> {Function.Print(sol.Item2)}");
                await Writer.WriteLineAsync($"λ = {sol.Item3}");
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
                Tuple<Vector, Fraction, Vector>? sol = Maximize();
                if (sol is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved");
                    return false;
                }

                await Writer.WriteLineAsync($"X = {sol.Item1} -> {Function.Print(sol.Item2)}");
                await Writer.WriteLineAsync($"λ = {sol.Item3}");
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
