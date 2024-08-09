using Accord.Math;
using Accord.Math.Optimization;
using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using Matrix = OperationalResearch.Models.Elements.Matrix;
using Vector = OperationalResearch.Models.Elements.Vector;

namespace OperationalResearch.Models.NonLinearOptimization.QuadProg
{
    public class QuadProg
    {
        private readonly QuadraticObjectiveFunction f;
        private readonly Matrix H;
        private readonly Vector lin;
        private readonly Polyhedron P;
        public bool IsValid { get => H.Rows == 2; }
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
                constraints.Add(new LinearConstraint(numberOfVariables: A.Cols)
                {
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
        public Tuple<Vector, Fraction, Vector>? AccordMinimize()
        {
            var solver = Solver;
            if (!solver.Minimize())
            {
                return null;
            }
            return GetSolution(solver);
        }
        public Tuple<Vector, Fraction, Vector>? AccordMaximize()
        {
            var solver = Solver;
            if (!solver.Maximize())
            {
                return null;
            }
            return GetSolution(solver);
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

        public Fraction Evaluate(Vector x)
        {
            var x1 = x[0];
            var x2 = x[1];
            var a = H[0, 0] / 2;
            var b = H[0, 1];
            var c = H[1, 1] / 2;
            return a * x1 * x1 + c * x2 * x2 + b * x1 * x2 + x * lin;
        }
        public enum LKKTPointType
        {
            Max,
            Min,
            Saddle,
            Unknown
        }
        public static LKKTPointType ClassifyLKKTPoint(Vector λ)
        {
            if (λ.IsNegativeOrZero)
            {
                return LKKTPointType.Max;
            }
            if (λ.IsPositiveOrZero)
            {
                return LKKTPointType.Min;
            }
            if (!λ.IsZero)
            {
                return LKKTPointType.Saddle;
            }
            return LKKTPointType.Unknown;
        }
        public IEnumerable<Tuple<Vector, Vector, LKKTPointType>> SolveLKKT()
        {
            var A = P.GetMatrix();
            var b = P.GetVector();
            List<Tuple<Vector, Vector, LKKTPointType>> solutions = [];

            // If solution is vertex
            foreach (var B in P.AllBasis)
            {
                var x = P[B];
                if (x is null) continue; // Should not happen
                if (solutions.Any(s => s.Item1 == x))
                {
                    continue; // Point already found
                }

                // A[B] * λ[B] = -c - Hx
                Matrix s = A[B].T;
                if (s.Det.IsZero)
                {
                    continue;
                }
                Vector v = Fraction.MinusOne * (lin + H * x);
                var partialλ = s.Inv * v;

                Vector λ = Vector.Zeros(A.Rows);
                for (int i = 0; i < B.Length; i++)
                {
                    λ[B[i]] = partialλ[i];
                }

                solutions.Add(new Tuple<Vector, Vector, LKKTPointType>(x, λ, ClassifyLKKTPoint(λ)));
            }


            // Solution is on a line
            foreach (var rowIndex in A.RowsIndeces)
            {
                var row = A[rowIndex];

                var s = (H | row).AddRow(row.Concat([Fraction.Zero]));
                var v = (Fraction.MinusOne * lin).Concat([b[rowIndex]]);

                if (s.Det.IsZero)
                {
                    continue;
                }
                var x_λ = s.Inv * v;

                var x = x_λ[x_λ.Indices.SkipLast(1)];
                if (P.IsOutside(x))
                {
                    continue;
                }

                Vector λ = Vector.Zeros(A.Rows);
                λ[rowIndex] = x_λ[x_λ.Size - 1];

                solutions.Add(new Tuple<Vector, Vector, LKKTPointType>(x, λ, ClassifyLKKTPoint(λ)));
            }

            return solutions;
        }

        public async Task PrintLKKT(IndentWriter? Writer)
        {
            if (Writer is null) return;
            var A = P.GetMatrix();
            var b = P.GetVector();
            await Writer.WriteLineAsync("Equations:");
            for (int i = 0; i < H.Rows; i++)
            {
                await Writer.Indent.WriteLineAsync(
                    string.Join(" + ", lin.Indices.Select(j => $"{Function.Print(H[i, j])} * x{j + 1}")) +
                    " + " +
                    string.Join(" + ", A.RowsIndeces.Select(j => $"{Function.Print(A[j, i])} * λ{j + 1}")) +
                    " = " +
                    Function.Print(lin[i] * Fraction.MinusOne)
                );
            }
            foreach (int i in A.RowsIndeces)
            {
                await Writer.Indent.WriteLineAsync(
                    $"λ{i + 1} * (" +
                    string.Join(" + ",
                        A.ColsIndeces.Select(j => $"{Function.Print(A[i, j])} * x{j + 1}")) +
                    $" - {Function.Print(b[i])}) = 0"
                );
            }

            await Writer.WriteLineAsync("Disequations:");
            foreach (int i in A.RowsIndeces)
            {
                await Writer.Indent.WriteLineAsync(
                    string.Join(" + ",
                        A.ColsIndeces.Select(j => $"{Function.Print(A[i, j])} * x{j + 1}")) +
                    $" - {Function.Print(b[i])} <= 0"
                );
            }
        }
        public async Task<LKKTPointType> ClassifyGivenPoint(IndentWriter Writer, Vector x)
        {
            var gradF = H * x + lin;
            await Writer.WriteLineAsync($"∇f(x) = {gradF}");
            if (!P.IsOnBorder(x))
            {
                if (gradF.IsZero)
                {
                    await Writer.Indent.Purple.WriteLineAsync(
                        "x is not on the border of the polyhedron but ∇f(x) = 0, so x is a stationary point (λ = 0)"); 
                } else
                {
                    await Writer.Indent.Orange.Italic.WriteLineAsync(
                        "x is not on the border of the polyhedron, so can't be an LKKT solution");
                }
                return LKKTPointType.Unknown;
            }
            var g = P.GetMatrix() * x - P.GetVector();
            var B = g.ZeroIndexes;
            var A = P.GetMatrix();
            await Writer.Blue.WriteLineAsync("Solving λ[B] = - (A[B].T)^-1 ∇f(x); λ[N] = 0");
            if (B.Length != x.Size)
            {
                await Writer.Orange.WriteLineAsync(
                    $"It appears that ({string.Join(", ", B.Select(i => $"λ{i + 1}"))}) != 0 but we don't have enough equations (we have {x.Size}) to solve for the coefficients");
                if (B.Length > x.Size)
                {
                    // We have more coeffs than equations
                    return LKKTPointType.Unknown;
                }
                // We have more equations than coeffs

                await Writer.Indent.WriteLineAsync(
                    $"Ignoring {x.Size - B.Length} equations");
                A = A.T[A.ColsIndeces.SkipLast(x.Size - B.Length)].T;
                gradF = gradF[gradF.Indices.SkipLast(x.Size - B.Length)];
            }
            var s = A[B].T;
            if (s.Det.IsZero)
            {
                await Writer.Orange.WriteLineAsync("Cannot solve: matrix can't be inverted");
                return LKKTPointType.Unknown;
            }

            Vector λ = Vector.Zeros(g.Size);
            var λ_B = Fraction.MinusOne * s.Inv * gradF;
            for (int i = 0; i < λ_B.Size; i++)
            {
                λ[B[i]] = λ_B[i];
            }

            await Writer.WriteLineAsync($"λ = {λ}");
            return ClassifyLKKTPoint(λ);
        }
        public async Task<bool> SolveFlow(
            Vector? pointOfInterest = null,
            IndentWriter? Writer = null,
            bool max = true)
        {
            Writer ??= IndentWriter.Null;
            bool solved = false;
            await Writer.WriteLineAsync($"det(Hf) = {Function.Print(H.Det)}");

            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();
            //
            // Solve in R^2
            //
            try
            {
                await Writer.Bold.WriteLineAsync("Searching points in ℝ^2");
                var x = WhereGradientIsZero();
                if (x is null)
                {
                    await Writer.Red.WriteLineAsync("Could not find points where ∇f(x, y) = (0, 0)");
                } else
                {
                    await Writer.Green.WriteLineAsync($"∇f({Function.Print(x[0])}, {Function.Print(x[1])}) = (0, 0)");
                    await Writer.WriteLineAsync($"f({Function.Print(x[0])}, {Function.Print(x[1])}) = {Function.Print(Evaluate(x))}");
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"An exception happened: '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
            }

            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();
            //
            // LKKT
            //
            try
            {
                await Writer.Bold.WriteLineAsync("Solving LKKT system:");
                await PrintLKKT(Writer);
                await Writer.WriteLineAsync();
                var lkkt = SolveLKKT();
                if (lkkt is null || !lkkt.Any())
                {
                    await Writer.Red.WriteLineAsync("LKKT problem was not solved :/");
                }
                else
                {
                    solved = true;
                    foreach (Tuple<Vector, Vector, LKKTPointType> tuple in lkkt)
                    {
                        await Writer.WriteLineAsync(
                            $"x = {tuple.Item1}; f(x) = {Function.Print(Evaluate(tuple.Item1))}");
                        await Writer.Indent.WriteLineAsync($"λ = {tuple.Item2}");
                        switch (tuple.Item3)
                        {
                            case LKKTPointType.Max:
                                await Writer.Indent.Italic.WriteLineAsync("x is a local maximum point");
                                break;
                            case LKKTPointType.Min:
                                await Writer.Indent.Italic.WriteLineAsync("x is a local minimum point");
                                break;
                            case LKKTPointType.Saddle:
                                await Writer.Indent.Purple.WriteLineAsync("x is a saddle point");
                                break;
                            case LKKTPointType.Unknown:
                            default:
                                await Writer.Indent.Orange.WriteLineAsync("x cannot be classified");
                                break;
                        }
                    }

                    if (max)
                    {
                        var maxValue = lkkt
                            .Where(t => t.Item3 == LKKTPointType.Max)
                            .Select(t => Evaluate(t.Item1)).Max();
                        foreach (var x in lkkt
                            .Where(t => t.Item3 == LKKTPointType.Max &&
                                Evaluate(t.Item1) == maxValue)
                            .Select(t => t.Item1))
                        {
                            await Writer.Green.WriteLineAsync($"Global maximum: x = {x}");
                        }
                    }
                    else
                    {
                        var minValue = lkkt
                            .Where(t => t.Item3 == LKKTPointType.Min)
                            .Select(t => Evaluate(t.Item1)).Min();
                        foreach (var x in lkkt
                            .Where(t => t.Item3 == LKKTPointType.Min &&
                                Evaluate(t.Item1) == minValue)
                            .Select(t => t.Item1))
                        {
                            await Writer.Green.WriteLineAsync($"Global minimum: x = {x}");
                        }
                    }
                }
                
                //
                // Analyze the given point
                //
                if (pointOfInterest is not null && pointOfInterest.Size == H.Cols)
                {
                    await Writer.WriteLineAsync();
                    await Writer.Blue.Bold.WriteLineAsync($"Studying x = {pointOfInterest}");
                    switch (await ClassifyGivenPoint(Writer.Indent, pointOfInterest))
                    {
                        case LKKTPointType.Max:
                            await Writer.Indent.WriteLineAsync("x is a local maximum point");
                            break;
                        case LKKTPointType.Min:
                            await Writer.Indent.WriteLineAsync("x is a local minimum point");
                            break;
                        case LKKTPointType.Saddle:
                            await Writer.Indent.WriteLineAsync("x is a saddle point");
                            break;
                        case LKKTPointType.Unknown:
                        default:
                            await Writer.Indent.Red.WriteLineAsync("x cannot be classified");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"An exception happened: '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
            }

            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();
            //
            // LKKT via Accord.Math.QuadProg
            //
            try
            {
                await Writer.Bold.WriteLineAsync($"Searching {(max ? "max" : "min")} via Accord.Math.QuadProg");
                var sol = max ? AccordMaximize() : AccordMinimize();

                if (sol is null)
                {
                    await Writer.Red.WriteLineAsync("Cound not find a solution");
                }
                else
                {
                    await Writer.Green.WriteLineAsync($"x = {sol.Item1}; f(x) = {Function.Print(sol.Item2)}");
                    await Writer.Indent.WriteLineAsync($"λ = {sol.Item3}");
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"An exception happened: '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
            }

            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();
            //
            // min/max via Franke-Wolfe method
            //
            try
            {
                await Writer.Bold.WriteLineAsync(
                    $"Searching {(max ? "max" : "min")} via Franke-Wolfe method");

                var fw = new QuadProgFrankeWolfe(P, H, lin);

                var sol = max ? 
                    await fw.SolveMax(startX: pointOfInterest, Writer.Indent) :
                    await fw.SolveMin(startX: pointOfInterest, Writer.Indent);

                if (sol is null)
                {
                    await Writer.Red.WriteLineAsync("Cound not find a solution");
                }
                else
                {
                    await Writer.Green.WriteLineAsync($"x = {sol}; f(x) = {Evaluate(sol)}");
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"An exception happened: '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
            }

            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();
            //
            // min/max via Projected Gradient Descent
            //
            try
            {
                await Writer.Bold.WriteLineAsync($"Searching {(max ? "max" : "min")} via Projected Gradient Descent");

                var fw = new QuadProgProjectedGradientDescent(P, H, lin);

                var sol = max ?
                    await fw.SolveMax(startX: pointOfInterest, Writer.Indent) :
                    await fw.SolveMin(startX: pointOfInterest, Writer.Indent);

                if (sol is null)
                {
                    await Writer.Red.WriteLineAsync("Cound not find a solution");
                }
                else
                {
                    await Writer.Green.WriteLineAsync($"x = {sol}; f(x) = {Evaluate(sol)}");
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"An exception happened: '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
            }

            return solved;
        }
    }
}
