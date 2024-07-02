using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.Models.Python
{
    public abstract class PythonFunctionAnalyzer
    {
        protected readonly Polyhedron P;
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
            Vector x, Vector d) => Models.Function.FindArgMin(
                t => Function(x + d * t), // Phi(t)
                t_start, t_end, steps);

        protected Fraction FindArgMaxOfFunction(
            Fraction t_start, Fraction t_end, int steps,
            Vector x, Vector d) => Models.Function.FindArgMax(
                t => Function(x + d * t), // Phi(t)
                t_start, t_end, steps);

        /// <summary>
        /// Finds min or max of phi(t) = f(x + t * d)
        /// </summary>
        /// <param name="FindMin">Find min or max</param>
        /// <param name="t_start">The starting instant to check</param>
        /// <param name="t_end">The end instant to check</param>
        /// <param name="steps">How many steps to do between t_end and t_start</param>
        /// <param name="x">The x vector</param>
        /// <param name="d">The direction vector</param>
        /// <returns>The t where phi(t) is min or max</returns>
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
        public PythonFunctionAnalyzer(Polyhedron P, string python)
        {
            ArgumentNullException.ThrowIfNull(P);
            ArgumentException.ThrowIfNullOrWhiteSpace(python);
            this.P = P;
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
            IndentWriter? Writer = null,
            int? maxK = null);
        public abstract Task<Vector?> SolveMax(
            Vector? startX = null,
            IndentWriter? Writer = null,
            int? maxK = null);

        public async Task<bool> SolveMinFlow(IndentWriter? Writer = null, Vector? startingPoint = null)
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
                        await Writer.WriteLineAsync($" x = {result}");
                        await Writer.WriteLineAsync($"Best (lesser) f(x) = {Function(result)}");
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
        public async Task<bool> SolveMaxFlow(IndentWriter? Writer = null, Vector? startingPoint = null)
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
                        await Writer.WriteLineAsync($" x = {result}");
                        await Writer.WriteLineAsync($"Best (greater) f(x) = {Function(result)}");
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
    }
}