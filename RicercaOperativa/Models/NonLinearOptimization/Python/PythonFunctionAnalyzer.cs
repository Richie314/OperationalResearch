using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.NonLinearOptimization;

namespace OperationalResearch.Models.NonLinearOptimization.Python
{
    public class PythonFunctionAnalyzer : NonLinearOptimizer
    {
        private readonly dynamic function;
        private readonly dynamic gradFunction;
        public override Vector Gradient(Vector x)
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
        public override Fraction Function(Vector x)
        {
            var outPut = (double)function(x.ToDouble().ToList());
            return Fraction.FromDouble(outPut);
        }

        public PythonFunctionAnalyzer(Polyhedron P, string python) : base(P)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(python);
            var list = GetFunctions(python);
            function = list[0];
            gradFunction = list[1];
        }
        private static List<dynamic> GetFunctions(string functionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(functionString);
            var eng = IronPython.Hosting.Python.CreateEngine();
            var scope = eng.CreateScope();
            string functionCaller =
                "def gradFunctionWrapper(x_array):" + Environment.NewLine + // Do not add type list[float] to it
                "    result = list(gradF(*x_array))" + Environment.NewLine +
                "    return [float(r) for r in result]" + Environment.NewLine +
                                                            Environment.NewLine +
                "def FunctionWrapper(x_array):" + Environment.NewLine + // Do not add type list[float] to it
                "    return float(f(*x_array))" + Environment.NewLine;

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
    }
}