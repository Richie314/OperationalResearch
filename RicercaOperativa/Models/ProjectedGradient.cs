using Accord.Math;
using Fractions;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace RicercaOperativa.Models
{
    internal sealed class ProjectedGradient : PythonFunctionAnalyzer
    {
        public ProjectedGradient(Fraction[,] A, Vector b, string python) : base(A, b, python) { }

        public override async Task<Vector?> Solve(Vector? startX = null, StreamWriter? Writer = null, int? maxK = null)
        {
            Writer ??= StreamWriter.Null;
            Matrix a = A;
            Vector b = B;
            int k = 0;
        //step1:

            Vector xk = startX ?? GetRandomStartPoint(A, B);
        step2:
            await Writer.WriteLineAsync($"Iteration {k}:");
            if (maxK.HasValue && maxK.Value < k)
            {
                await Writer.WriteLineAsync($"Optimal value not reached in {k} iterations.");
                await Writer.WriteLineAsync($"Exiting with failure.");
                return null;
            }
            await Writer.WriteLineAsync($"A = {a}");
            await Writer.WriteLineAsync($"b = {b}");
            await Writer.WriteLineAsync($"x{k} = {xk}");

            if ((a * xk) > b) // Check if A * xk > b. In that case stop (an error has appened)
            {
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync($"Vector x{k} is out of bound!");
                await Writer.WriteLineAsync();
                return null;
            }

        step3:
            IEnumerable<int> J = a.RowsIndeces.Where(i => a[i] * xk == b[i]);
            await Writer.WriteLineAsync($"J = {Models.Function.Print(J)}");
            Matrix M = a[J];
            await Writer.WriteLineAsync($"M = {M}");

            //step4:
            Matrix H = await GetHMatrix(M, A.Cols, null);
            await Writer.WriteLineAsync($"H = {H}");

            Vector gradXk = Grad(xk);
            await Writer.WriteLineAsync($"gradF(x{k}) = {gradXk}");

            Vector dk = ((-1) * H) * gradXk;
            await Writer.WriteLineAsync($"d{k} = {dk}");
            if (dk.IsZero) // dk == 0
            {
                await Writer.WriteLineAsync($"d{k} is zero!");
                goto step5;
            }

            Fraction tMax = FindTMax(a, xk, dk, b);
            await Writer.WriteLineAsync($"t{k}^ = {Models.Function.Print(tMax)}");

            const int PointsToPlot = 5000;
            await Writer.WriteLineAsync(
                $"Finding min value of f(x{k} + t d{k}) inside [0, {Models.Function.Print(tMax)}]. {PointsToPlot} points considered");
            
            Fraction tk = FindArgMinOfFunction(0, tMax, PointsToPlot, xk, dk);
            await Writer.WriteLineAsync($"t{k} = {Models.Function.Print(tk)}");

            xk += tk * dk; // implict operator overloading
            k++;
            await Writer.WriteLineAsync($"Going to iteration {k}...");
            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();
            goto step2;

        step5:
            if (M.Rows == 0)
            {
                // matrix is empty: we are not at an edge => gradient is null on its own
                await Writer.WriteLineAsync($"Gradient is zero not on an edge.");
                await Writer.WriteLineAsync($"Exit with unknown (unexpected) result.");
                return xk;
            }
            Vector lambda = (-1) * (((M * M.T).Inv * M) * gradXk);
            await Writer.WriteLineAsync($"lambda = {lambda}");
            if (lambda.IsPositiveOrZero) // lambda >= 0
            {
                await Writer.WriteLineAsync($"lambda >= 0.");
                await Writer.WriteLineAsync($"Exit with success.");
                return xk;
            } else
            {
                await Writer.WriteLineAsync($"lambda has at least one component below zero.");
            }
            // J has at least 1 element here
            // J == {} => dk == -gradF(xk)
            // dk == 0 => gradF(xk) == 0 => lambda == 0 => lambda >= 0 => It has already exited

            // argmin == 0 -> min is the first element of J
            // argmin == 1 -> min is the second element of J
            // and so on
            int argmin = lambda.ArgMin; 
            await Writer.WriteLineAsync($"Argmin{{lambda}} = {argmin + 1}");
            int jToRemove = J.ElementAt(argmin);
            await Writer.WriteLineAsync($"Removing equation {jToRemove + 1} from A|b.");
            // Remove row from a
            a = a[a.RowsIndeces.Where(j => j != jToRemove)];
            // Remove row from b
            b = b.RemoveAt(jToRemove);
            await Writer.WriteLineAsync($"(A|b)' = {a | b}");
            goto step3;
        }
        
        private static async Task<Matrix> GetHMatrix(Matrix M, int xLength, StreamWriter? Writer = null)
        {
            if (M.Rows == 0)
            {
                return Matrix.Identity(xLength);
            }
            Writer ??= StreamWriter.Null;

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
        private static Fraction FindTMax(Matrix a, Vector x, Vector d, Vector b)
        {
            Vector ax = a * x, ad = a * d;
            Vector b_ax = b - ax;

            Fraction? tMin = null, tMax = null;
            int[] PositiveIndexes = ad.PositiveIndexes;
            if (PositiveIndexes.Length > 0)
            {
                Vector b_ax_over_ad = Vector.Div(
                    b_ax[PositiveIndexes],
                    ad[PositiveIndexes]);
                // Return the max{t} : t < (b - ax) / ad => return min { (b - ax) / ad }
                tMin = b_ax_over_ad.Min;
            }
            int[] NegativeIndexes = ad.NegativeIndexes;
            if (NegativeIndexes.Length > 0)
            {
                Vector b_ax_over_ad = Vector.Div(
                    b_ax[NegativeIndexes],
                    ad[NegativeIndexes]);
                // Return the max{t} : t > (b - ax) / ad => return max { (b - ax) / ad } ad < 0
                tMax = b_ax_over_ad.Max;
            }
            // tMax < t < tMin
            if (!tMin.HasValue)
            {
                if (!tMax.HasValue)
                {
                    // -Inf < t < +Inf
                    throw new ArgumentException("a * d must at least have one non-zero component");
                }

                // tMax < t < +Inf
                throw new Exception($"t -> +Inf, unbounded ({tMax} < t)");
            }
            if (tMax.HasValue && tMax > tMin)
            {
                // tMax < t < tMin but tMax > tMin!
                throw new Exception($"No acceptable value of t found ({tMax} < t < {tMin})");
            }
            return tMin.Value;
        }
    }
}