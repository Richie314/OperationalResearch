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
        private readonly Matrix H;
        private readonly Vector lin;
        private readonly Elements.Polyhedron P;
        public QuadProg(Matrix H, Vector linearPart, Elements.Polyhedron p)
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
            this.H = H;
            lin = linearPart;
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
                await GradientFlow(Writer);
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
                await GradientFlow(Writer);
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

        public Vector? WhereGradientIsZero()
        {
            if (H.Det.IsZero)
            {
                // There are no solutions
                return null;
            }
            return H.Inv * (Fraction.MinusOne * lin);
        }

        public async Task<bool> GradientFlow(IndentWriter Writer)
        {
            try
            {
                var x = WhereGradientIsZero();
                if (x is null)
                {
                    await Writer.WriteLineAsync("Could not find points where ∇f(x, y) = (0, 0)");
                    return false;
                }
                await Writer.WriteLineAsync($"∇f({Function.Print(x[0])}, {Function.Print(x[1])}) = (0, 0)");
                await Writer.WriteLineAsync($"f({Function.Print(x[0])}, {Function.Print(x[1])}) = {Function.Print(Evaluate(x))}");
                await Writer.WriteLineAsync($"det(Hf) = {Function.Print(H.Det)}");
                return true;
            } catch
            {
                return false;
            }
        }

        public Fraction Evaluate(Vector x)
        {
            var x1 = x[0];
            var x2 = x[1];
            var a = 2 * H[0, 0];
            var b = H[0, 1];
            var c = 2 * H[1, 1];
            return a * x1 * x1 + c * x2 * x2 + b * x1 * x2 + x1 * lin[0] + x2 * lin[1];
        }
    }
}
