using Accord.Math;
using Fractions;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace RicercaOperativa.Models
{
    public abstract class PythonFunctionAnalyzer
    {
        protected readonly Matrix A;
        protected readonly Vector B;
        private readonly dynamic function;
        private readonly dynamic gradFunction;
        protected Vector Grad(Vector x)
        {
            var outPut = (IronPython.Runtime.PythonList)gradFunction(x.ToDouble().ToList());
            if (outPut.Count != x.Size)
            {
                throw new Exception(
                    $"Gradient function returned {outPut.Count} values but {x.Size} were expected");
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
        protected Fraction Function(Vector x)
        {
            var outPut = (double)function(x.ToDouble().ToList());
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
        protected Fraction FindArgMinOfFunction(
            Fraction t_start, Fraction t_end, int steps,
            Vector x, Vector d)
        {
            return Models.Function.FindArgMin(
                t => Function(x + (d * t)), // Phi(t)
                t_start, t_end, steps);
        }
        protected Fraction FindArgMaxOfFunction(
            Fraction t_start, Fraction t_end, int steps,
            Vector x, Vector d)
        {
            return Models.Function.FindArgMax(
                t => Function(x + (d * t)), // Phi(t)
                t_start, t_end, steps);
        }
        protected Fraction FindArgOfFunction(
            bool FindMin,
            Fraction t_start, Fraction t_end, int steps,
            Vector x, Vector d)
        {
            if (FindMin)
            {
                return FindArgMinOfFunction(t_start, t_end, steps, x, d);
            }
            return FindArgMaxOfFunction(t_start, t_end, steps, x, d);
        }
        public PythonFunctionAnalyzer(Fraction[,] A, Vector b, string python)
        {
            ArgumentNullException.ThrowIfNull(A);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(python);
            if (A.Rows() != b.Size)
            {
                throw new ArgumentException("A must have row number equal to the length of b");
            }
            this.A = new Matrix(A);
            B = b;
            var list = GetFunctions(python);
            function = list[0];
            gradFunction = list[1];
        }
        private static List<dynamic> GetFunctions(string functionString)
        {
            var eng = IronPython.Hosting.Python.CreateEngine();
            var scope = eng.CreateScope();
            string functionCaller =
                "def gradFunctionWrapper(x_array):" + Environment.NewLine + // Do not add type list[float] to it
                "    result = list(gradF(*x_array))" + Environment.NewLine +
                "    return [float(r) for r in result]" + Environment.NewLine +
                                                            Environment.NewLine +
                "def FunctionWrapper(x_array):" + Environment.NewLine + // Do not add type list[float] to it
                "    return float(f(*x_array))" + Environment.NewLine;
            try
            {
                string wholeScript =
                    functionString +
                    Environment.NewLine +
                    functionCaller +
                    Environment.NewLine;
                wholeScript = wholeScript.Trim().Replace("\t", "    ");
                eng.Execute(wholeScript, scope);

                dynamic testFunction = scope.GetVariable("f") ??
                    throw new MissingMemberException("Invalid f definition"); // Test if f exists
                dynamic testGrad = scope.GetVariable("gradF") ??
                    throw new MissingMemberException("Invalid gradF definition"); // Test if gradF exists

                dynamic f = scope.GetVariable("FunctionWrapper") ??
                    throw new MissingMemberException("Could not retrieve FunctionWrapper body");
                dynamic g = scope.GetVariable("gradFunctionWrapper") ??
                    throw new MissingMemberException("Could not retrieve gradFunctionWrapper body");
                return [f, g];
            }
            catch (MissingMemberException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Something has gone wrong: " + ex.Message);
            }
        }

        public abstract Task<Vector?> SolveMin(
            Vector? startX = null,
            StreamWriter? Writer = null,
            int? maxK = null);
        public abstract Task<Vector?> SolveMax(
            Vector? startX = null,
            StreamWriter? Writer = null,
            int? maxK = null);

        public async Task<bool> SolveMinFlow(StreamWriter? Writer = null, Vector? startingPoint = null)
        {
            try
            {
                var result = await SolveMin(Writer: Writer, maxK: 100, startX: startingPoint);
                if (result is not null)
                {
                    if (Writer != null)
                    {
                        await Writer.WriteLineAsync();
                        await Writer.WriteLineAsync();
                        await Writer.WriteLineAsync($"Best (lesser) value = {result}");
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
        public async Task<bool> SolveMaxFlow(StreamWriter? Writer = null, Vector? startingPoint = null)
        {
            try
            {
                var result = await SolveMax(Writer: Writer, maxK: 100, startX: startingPoint);
                if (result is not null)
                {
                    if (Writer != null)
                    {
                        await Writer.WriteLineAsync();
                        await Writer.WriteLineAsync();
                        await Writer.WriteLineAsync($"Best (greater) value = {result}");
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

        public static Vector GetRandomStartPoint(Matrix A, Vector b)
        {
            if (A.Rows != b.Size)
            {
                throw new ArgumentException($"Columns of a must be equal to size of b ({A.Rows} != {b.Size})");
            }

            // if point 0 is acceptable we return it
            if (b.IsPositiveOrZero) // b >= 0
            {
                return Enumerable.Repeat(Fraction.Zero, A.Cols).ToArray();
            }

            Random rnd = new();
            int guesses = 0;
            while (guesses < 3000)
            {
                guesses++;
                Fraction[] x = new Fraction[A.Cols];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = new Fraction(rnd.Next(), rnd.Next() + 1);
                }
                if (A * x <= b)
                {
                    return x;
                }
            }
            throw new Exception($"It was impossible to find a starting x, {guesses} possible vectors considered");
        }
    }
}