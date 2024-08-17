using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.NonLinearOptimization.Python;
using OperationalResearch.Models.NonLinearOptimization.QuadProg;

namespace OperationalResearch.Models.NonLinearOptimization
{
    public class ProjectedGradientDescent<Optimizer>
        where Optimizer : NonLinearOptimizer
    {
        private readonly Optimizer optimizer;
        private Polyhedron P { get => optimizer.P; }
        public ProjectedGradientDescent(Optimizer optimizer)
        {
            this.optimizer = optimizer;
        }
        public async Task<Vector?> SolveMin(
            Vector? startX = null, 
            IndentWriter? Writer = null, 
            int? maxK = null) =>
            await Solve(true, startX, Writer, maxK);
        public async Task<Vector?> SolveMax(
            Vector? startX = null, 
            IndentWriter? Writer = null, 
            int? maxK = null) =>
            await Solve(false, startX, Writer, maxK);

        public async Task<Vector?> Solve(
            bool IsMin,
            Vector? startX = null,
            IndentWriter? Writer = null,
            int? maxK = null)
        {
            Writer ??= IndentWriter.Null;
            int k = 0;
            Polyhedron p = P.Copy();

            if (startX is not null && p.IsOutside(startX))
            {
                await Writer.Orange.WriteLineAsync($"Starting x = {startX} is not in the polyhedron.");
                await Writer.Indent.Orange.WriteLineAsync($"A new starting point will be generated randomly");
                startX = null;
            }

            Vector xk = startX ??
                p.RandomInternalPoint() ??
                throw new Exception("Could not find a random starting point!");
            bool pChanged = true;
            while (!maxK.HasValue || k < maxK.Value)
            {
                await Writer.Bold.WriteLineAsync($"Iteration #{k}:");
                var A = p.GetMatrix();
                var b = p.GetVector();
                if (pChanged)
                {
                    await Writer.WriteLineAsync($"A|b = {A | b}");
                    pChanged = false;
                }

                await Writer.WriteLineAsync($"x_{k} = {xk}");
                Writer.LogObject($"PGD_{k}", xk);
                if (p.IsOutside(xk)) // Check if A * xk <= b. In that case stop (an error has appened)
                {
                    await Writer.Indent.Orange.WriteLineAsync($"Vector x_{k} is out of bound!");
                    break;
                }

                IEnumerable<int> J = A.RowsIndeces.Where(i => A[i] * xk == b[i]);
                await Writer.WriteLineAsync($"J = {Models.Function.Print(J)}");

                Matrix M = A[J];
                await Writer.WriteLineAsync($"M = {M}");

                Matrix H = await GetHMatrix(M, A.Cols, null);
                await Writer.WriteLineAsync($"H = {H}");

                Vector gradXk = optimizer.Gradient(xk);
                await Writer.WriteLineAsync($"∇f(x_{k}) = {gradXk}");

                Vector dk = H * gradXk * (IsMin ? Fraction.MinusOne : Fraction.One);
                await Writer.WriteLineAsync($"d_{k} = {dk}");
                Writer.LogObject(
                    $"PGD_d{k}",
                    new Tuple<Vector, Vector>(xk, dk));


                if (dk.IsZero) // dk == 0
                {
                    await Writer.Indent.Blue.WriteLineAsync($"d_{k} is zero!");
                    
                    if (M.Rows == 0)
                    {
                        // matrix is empty: we are not at an edge => gradient is null on its own
                        await Writer.Bold.WriteLineAsync($"Gradient is zero not on an edge.");
                        await Writer.Indent.WriteLineAsync($"Exit with unknown (unexpected) result.");
                        return xk;
                    }
                    
                    Vector λ = (IsMin ? Fraction.MinusOne : Fraction.One) * ((M * M.T).Inv * M * gradXk);
                    await Writer.WriteLineAsync($"λ = {λ}");
                    if (λ.IsPositiveOrZero) // lambda >= 0
                    {
                        await Writer.Indent.Green.WriteLineAsync($"λ >= 0.");
                        await Writer.Indent.WriteLineAsync($"Exit with success.");
                        return xk;
                    }
                    await Writer.Indent.WriteLineAsync($"λ has at least one component below zero.");
                    
                    // J has at least 1 element here
                    // J == {} => dk == -gradF(xk)
                    // dk == 0 => gradF(xk) == 0 => λ == 0 => λ >= 0 => It has already exited

                    // argmin == 0 -> min is the first element of J
                    // argmin == 1 -> min is the second element of J
                    // and so on
                    int argmin = λ.ArgMin;
                    await Writer.Indent.WriteLineAsync($"Argmin{{ λ }} = {argmin + 1}");
                    
                    int jToRemove = J.ElementAt(argmin);
                    await Writer.WriteLineAsync($"Removing equation {jToRemove + 1} from A|b.");

                    p = p.RemoveEquation(jToRemove);
                    pChanged = true;
                    continue;
                }

                Fraction tMax = FindTMaxMin(A, xk, dk, b, true);
                await Writer.WriteLineAsync($"t_{k}^ = {Function.Print(tMax)}");

                if (tMax.IsNegative)
                {
                    // We won't move
                    await Writer.Red.WriteLineAsync($"t_{k}^ is negative!");
                    await Writer.Indent.Red.WriteLineAsync(
                        $"Unexpected value. Procedure may be compromised.");
                    break;
                }


                const int PointsToPlot = 5000;
                await Writer.Indent.WriteLineAsync(
                    $"Finding {(IsMin ? "min" : "max")} value of phi(t) = f(x_{k} + t d_{k}) " +
                    $"inside [0, {Function.Print(tMax)}]. {PointsToPlot} points considered");
                
                Fraction tk = optimizer.FindArgOfFunction(IsMin, 0, tMax, PointsToPlot, xk, dk);
                await Writer.WriteLineAsync($"t_{k} = {Models.Function.Print(tk)}");
                if (tk.IsZero)
                {
                    // We won't move
                    await Writer.Orange.WriteLineAsync($"t_{k} is zero!");
                    await Writer.Indent.Orange.WriteLineAsync($"It is impossible to move forward.");
                    break;
                }

                xk += tk * dk;
                k++;
                await Writer.WriteLineAsync();
            }
            if (maxK.HasValue && maxK.Value < k)
            {
                await Writer.Red.WriteLineAsync($"Optimal value not reached in {k} iterations.");
            }
            await Writer.Red.WriteLineAsync($"Exiting with failure.");
            return null;
        }

        private static async Task<Matrix> GetHMatrix(Matrix M, int xLength, IndentWriter? Writer = null)
        {
            if (M.Rows == 0)
            {
                return Matrix.Identity(xLength);
            }
            Writer ??= IndentWriter.Null;

            // M * M.T
            var mmt = M * M.T;
            await Writer.WriteLineAsync($"M * M.T: {mmt.Rows}x{mmt.Rows} = {mmt}");

            // (M * M.T).Inv
            var mmt_inv = mmt.Inv;
            await Writer.WriteLineAsync($"(M * M.T)^-1: {mmt_inv.Rows}x{mmt_inv.Rows} = {mmt_inv}");

            // M.T * (M * M.T).Inv
            var mt_mmt_inv = M.T * mmt_inv;
            await Writer.WriteLineAsync($"M.T * (M * M.T)^-1 = {mt_mmt_inv}");

            // M.T * (M * M.T).Inv * M
            var mt_mmt_inv_m = mt_mmt_inv * M;
            await Writer.WriteLineAsync($"M.T * (M * M.T)^-1 * M = {mt_mmt_inv_m}");

            if (mt_mmt_inv_m.Rows != xLength || mt_mmt_inv_m.Cols != xLength)
            {
                throw new Exception(
                    $"Dimensions of matrix operation are not what was expected" +
                    $"({mt_mmt_inv_m.Rows}, {mt_mmt_inv_m.Cols} != {xLength}, {xLength})");
            }
            return Matrix.Identity(xLength) - mt_mmt_inv_m;
        }


        /// <summary>
        /// Finds max(t) where
        /// A (x + td) < b
        /// A * d != 0
        /// </summary>
        /// <param name="a">The matrix A</param>
        /// <param name="x">The vector x</param>
        /// <param name="d">The vector d</param>
        /// <param name="b">The vector b</param>
        /// <returns>the highest possible value of t according to all equations</returns>
        private static Fraction FindTMaxMin(Matrix a, Vector x, Vector d, Vector b, bool WantRight)
        {
            Vector ax = a * x, ad = a * d;
            Vector b_ax = b - ax;

            Fraction? tRight = null, tLeft = null;
            int[] PositiveIndexes = ad.PositiveIndexes;
            if (PositiveIndexes.Length > 0)
            {
                Vector b_ax_over_ad = Vector.Div(
                    b_ax[PositiveIndexes],
                    ad[PositiveIndexes]);
                // Return the max{t} : t < (b - ax) / ad => return min { (b - ax) / ad }
                tRight = b_ax_over_ad.Min;
            }
            int[] NegativeIndexes = ad.NegativeIndexes;
            if (NegativeIndexes.Length > 0)
            {
                Vector b_ax_over_ad = Vector.Div(
                    b_ax[NegativeIndexes],
                    ad[NegativeIndexes]);
                // Return the min{t} : t > (b - ax) / ad => return max { (b - ax) / ad } ad < 0
                tLeft = b_ax_over_ad.Max;
            }

            // This should be always valid: tLeft < t < tRight

            if (tLeft.HasValue && tRight.HasValue && tLeft.Value > tRight.Value)
            {
                // tLeft < t < tRight but tLeft > tRight!
                throw new Exception(
                    $"No acceptable value of t found ({Models.Function.Print(tLeft)} < t < {Models.Function.Print(tRight)})");
            }

            if (!tLeft.HasValue && !tRight.HasValue)
            {
                // -Inf < t < +Inf
                // No bound found
                throw new ArgumentException("a * d must at least have one non-zero component");
            }
            // At least one has value from now on

            if (WantRight)
            {
                // We want tRight
                if (!tRight.HasValue)
                {
                    // Impossible to get the right bound
                    // tLeft < t < +Inf
                    throw new Exception($"t -> +Inf, unbounded ({Models.Function.Print(tLeft)} < t)");
                }
                return tRight.Value;
            }

            // We want tLeft

            if (!tLeft.HasValue)
            {
                // Impossible to get the left bound
                // -Inf < t < tRight
                throw new Exception($"t -> -Inf, unbounded (t < {Models.Function.Print(tRight)})");
            }
            return tLeft.Value;
        }
    }

    public class PythonProjectedGradientDescent : ProjectedGradientDescent<PythonFunctionAnalyzer>
    {
        public PythonProjectedGradientDescent(Polyhedron polyhedron, string s) :
            base(new PythonFunctionAnalyzer(polyhedron, s))
        { }

        public async Task<bool> SolveFlow(bool min, IndentWriter? Writer, Vector? startX)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                await Writer.Bold.WriteLineAsync(
                    $"{(min ? "Minimizing" : "Maximizing")} via Projected Gradient Descent method");
                var x = min ?
                    await SolveMin(startX: startX, Writer: Writer) :
                    await SolveMax(startX: startX, Writer: Writer);
                if (x is null || x.IsEmpty)
                {
                    await Writer.Red.WriteLineAsync($"{(min ? "Minimization" : "Maximization")} failed!");
                    return false;
                }
                await Writer.Bold.Green.WriteLineAsync($"x = {x}");
                return true;
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"Exception happened: '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
                return false;
            }
        }
    }

    public class QuadProgProjectedGradientDescent : ProjectedGradientDescent<QuadProgFunctionAnalyzer>
    {
        public QuadProgProjectedGradientDescent(Polyhedron polyhedron, Matrix H, Vector l) :
            base(new QuadProgFunctionAnalyzer(polyhedron, H, l))
        { }
    }
}
