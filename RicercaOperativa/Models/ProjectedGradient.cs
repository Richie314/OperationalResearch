using Accord.Math;
using Fractions;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace RicercaOperativa.Models
{
    internal class ProjectedGradient
    {
        private Matrix A;
        private Fraction[] b;
        private dynamic function;
        private dynamic gradFunction;
        private Fraction[] Grad(Fraction[] x)
        {
            var outPut = (IronPython.Runtime.PythonList) gradFunction(
                x.Select(xi => xi.ToDouble()).ToList()
                );
            if (outPut.Count != x.Length)
            {
                throw new Exception(
                    $"Gradient function returned {outPut.Count} values but {x.Length} were expected");
            }
            foreach (var valueToCheck in outPut)
            {
                if (valueToCheck is null)
                {
                    throw new Exception(
                        $"Gradient function returned null values");
                }
            }
            return outPut.Select(r => Fraction.FromDouble(value: (double)(r ?? 0))).ToArray();
        }
        private Fraction Function(Fraction[] x)
        {
            var outPut = (double)function(  x.Select(xi => xi.ToDouble()).ToList() );
            return Fraction.FromDouble(outPut);
        }
        /// <summary>
        /// Finds the t between t_start and t_end that has the min value of
        /// f(x + dt)
        /// </summary>
        /// <param name="t_start">The left bound of the time</param>
        /// <param name="t_end">The right bound of the time</param>
        /// <param name="steps">How many steps to do</param>
        /// <param name="x">The vector x</param>
        /// <param name="d">The vector d</param>
        /// <returns>The first t that takes the function to its min value</returns>
        private Fraction FindArgMinTofFunction(
            Fraction t_start, Fraction t_end, int steps, 
            Fraction[] x, Fraction[] d)
        {
            if (t_start == t_end)
            {
                return t_start;
            }
            Fraction MinY = Function(
                Simplex.Sum(x, Simplex.Mult(t_start, d)));
            Fraction MinT = t_start;

            Fraction dt = (t_end - t_start) / steps;
            Fraction currT = t_start + dt;

            while (currT <= t_end)
            {
                Fraction CurrY = Function(
                    Simplex.Sum(x, Simplex.Mult(currT, d)));
                if (CurrY < MinY)
                {
                    MinY = CurrY;
                    MinT = currT;
                }

                currT += dt;
            }
            return MinT;
        }
        public ProjectedGradient(Fraction[,] A, Fraction[] b, string python)
        {
            ArgumentNullException.ThrowIfNull(A);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(python);
            if (A.Rows() != b.Length)
            {
                throw new ArgumentException("A must have row number equal to the length of b");
            }
            this.A = new Matrix(A);
            this.b = b;
            var list = getFunctions(python);
            this.function = list[0];
            this.gradFunction = list[1];
        }
        private static List<dynamic> getFunctions(string functionString)
        {
            var eng = IronPython.Hosting.Python.CreateEngine();
            var scope = eng.CreateScope();
            string functionCaller = 
                "def gradFunctionWrapper(x_array):" +       Environment.NewLine + // Do not add type list[float] to it
                "    result = list(gradF(*x_array))" +      Environment.NewLine +
                "    return [float(r) for r in result]" +   Environment.NewLine +
                                                            Environment.NewLine +
                "def FunctionWrapper(x_array):" +           Environment.NewLine + // Do not add type list[float] to it
                "    return float(f(*x_array))" +           Environment.NewLine;
            try
            {
                string wholeScript =
                    functionString +
                    Environment.NewLine +
                    functionCaller +
                    Environment.NewLine;
                wholeScript = wholeScript.Trim().Replace("\t", "    ");
                eng.Execute(wholeScript, scope);

                dynamic testFunction = scope.GetVariable("f"); // Test if f exists
                if (testFunction is null)
                {
                    throw new MissingMemberException("Invalid f definition");
                }
                dynamic testGrad = scope.GetVariable("gradF"); // Test if gradF exists
                if (testGrad is null)
                {
                    throw new MissingMemberException("Invalid gradF definition");
                }

                dynamic f = scope.GetVariable("FunctionWrapper");
                if (f is null)
                {
                    throw new MissingMemberException("Could not retrieve FunctionWrapper body");
                }
                dynamic g = scope.GetVariable("gradFunctionWrapper");
                if (g is null)
                {
                    throw new MissingMemberException("Could not retrieve gradFunctionWrapper body");
                }
                return new List<dynamic>(){ f, g };
            } catch (MissingMemberException)
            {
                throw;
            } catch (Exception ex)
            {
                throw new Exception("Something has gone wrong: " + ex.Message);
            }
        }
        
        public async Task<Fraction[]?> Solve(Fraction[]? startX = null, StreamWriter? Writer = null, int? maxK = null)
        {
            Writer = Writer ?? StreamWriter.Null;
            Matrix a = A;
            int k = 0;
        //step1:

            Fraction[] xk = startX ?? getRandomStartPoint(A, b);
        step2:
            await Writer.WriteLineAsync($"Iteration {k}:");
            if (maxK.HasValue && maxK.Value < k)
            {
                await Writer.WriteLineAsync($"Optimal value not reached in {k} iterations.");
                await Writer.WriteLineAsync($"Exiting with failure.");
                return null;
            }
            await Writer.WriteLineAsync($"A = {a}");
            await Writer.WriteLineAsync($"b = {Simplex.Print(b)}");
            await Writer.WriteLineAsync($"x{k} = {Simplex.Print(xk)}");

            if (!Simplex.LessOrEqual(a * xk, b)) // Check if A * xk > b. In that case stop (an error has appened)
            {
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync($"Vector x{k} is out of bound!");
                await Writer.WriteLineAsync();
                return null;
            }

        step3:
            IEnumerable<int> J = a.RowsIndeces.Where(i => Matrix.Scalar(a[i], xk) == b[i]);
            await Writer.WriteLineAsync($"J = {Simplex.Print(J)}");
            Matrix M = a[J];
            await Writer.WriteLineAsync($"M = {M}");

            //step4:
            Matrix H = await getHMatrix(M, A.Cols, null);
            await Writer.WriteLineAsync($"H = {H}");

            Fraction[] gradXk = Grad(xk);
            await Writer.WriteLineAsync($"gradF(x{k}) = {Simplex.Print(gradXk)}");

            Fraction[] dk = ((-1) * H) * gradXk;
            await Writer.WriteLineAsync($"d{k} = {Simplex.Print(dk)}");
            if (dk.All(dki => dki.IsZero)) // dk == 0
            {
                await Writer.WriteLineAsync($"d{k} is zero!");
                goto step5;
            }

            Fraction tMax = findTMax(a, xk, dk, b);
            await Writer.WriteLineAsync($"t{k}^ = {tMax}");

            const int PointsToPlot = 5000;
            await Writer.WriteLineAsync($"Finding min value of f(x{k} + t d{k}) inside [0, {tMax}]. {PointsToPlot} points considered");
            
            Fraction tk = FindArgMinTofFunction(0, tMax, PointsToPlot, xk, dk);
            await Writer.WriteLineAsync($"t{k} = {tk}");

            xk = Simplex.Sum(xk, Simplex.Mult(tk, dk));
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
            Fraction[] lambda = (-1) * (M * M.T).Inv * M * gradXk;
            await Writer.WriteLineAsync($"lambda = {Simplex.Print(lambda)}");
            if (lambda.All(l => !l.IsNegative)) // lambda >= 0
            {
                await Writer.WriteLineAsync($"lambda >= 0.");
                await Writer.WriteLineAsync($"Exit with success.");
                return xk;
            }
            // J has at least 1 element here
            // J == {} => dk == -gradF(xk)
            // dk == 0 => gradF(xk) == 0 => lambda == 0 => lambda >= 0 => It has already exited

            // argmin == 0 -> min is the first element of J
            // argmin == 1 -> min is the second element of J
            // and so on
            int argmin = lambda.ArgMin(); 
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
        
        public async Task<bool> SolveFlow(StreamWriter? Writer = null, Fraction[]? startingPoint = null)
        {
            try
            {
                var result = await Solve(Writer: Writer, maxK: 100, startX: startingPoint);
                if (result != null)
                {
                    if (Writer != null)
                    {
                        await Writer.WriteLineAsync();
                        await Writer.WriteLineAsync();
                        await Writer.WriteLineAsync($"Best value = {Simplex.Print(result)}");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                if (Writer != null)
                {
                    await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
                    if (ex.StackTrace is not null)
                    {
                        await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                    }
                }
                return false;
            }
        }
        public static async Task<Matrix> getHMatrix(Matrix M, int xLength, StreamWriter? Writer = null)
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
        public static Fraction findTMax(Matrix a, Fraction[] x, Fraction[] d, Fraction[] b)
        {
            Fraction[] ax = a * x, ad = a * d;
            Fraction[] b_ax = Simplex.Sub(b, ax);

            Fraction? tMin = null, tMax = null;
            int[] PositiveIndexes = ad.Find(adi => adi.IsPositive);
            if (PositiveIndexes.Length > 0)
            {
                Fraction[] b_ax_over_ad = Simplex.Div(
                    Simplex.ExtractRows(b_ax, PositiveIndexes), 
                    Simplex.ExtractRows(ad, PositiveIndexes));
                // Return the max{t} : t < (b - ax) / ad => return min { (b - ax) / ad }
                tMin = b_ax_over_ad.Min();
            }
            int[] NegativeIndexes = ad.Find(adi => adi.IsNegative);
            if (NegativeIndexes.Length > 0)
            {
                Fraction[] b_ax_over_ad = Simplex.Div(
                    Simplex.ExtractRows(b_ax, NegativeIndexes),
                    Simplex.ExtractRows(ad, NegativeIndexes));
                // Return the max{t} : t > (b - ax) / ad => return max { (b - ax) / ad } ad < 0
                tMax = b_ax_over_ad.Max();
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
        public static Fraction[] getRandomStartPoint(Matrix A, Fraction[] b)
        {
            if (A.Rows != b.Length)
            {
                throw new ArgumentException($"Columns of a must be equal to length of b ({A.Rows} != {b.Length})");
            }

            // if point 0 is acceptable we return it
            if (b.All(bi => !bi.IsNegative)) // b >= 0
            {
                return Enumerable.Repeat(Fraction.Zero, A.Cols).ToArray();
            }

            Random rnd = new Random();
            int guesses = 0;
            while (guesses < 3000)
            {
                guesses++;
                Fraction[] x = new Fraction[A.Cols];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = new Fraction(rnd.Next(), rnd.Next() + 1);
                }
                if (Simplex.LessOrEqual(A * x, b))
                {
                    return x;
                }
            }
            throw new Exception($"It was impossible to find a starting x, {guesses} possible vectors considered");
        }
    }
}