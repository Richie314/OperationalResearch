using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.NonLinearOptimization.Python;
using OperationalResearch.Models.NonLinearOptimization.QuadProg;

namespace OperationalResearch.Models.NonLinearOptimization
{
    public class FrankWolfe<Optimizer>
        where Optimizer : NonLinearOptimizer
    {
        private readonly Optimizer optimizer;
        private Polyhedron P { get => optimizer.P; }
        public FrankWolfe(Optimizer optimizer)
        {
            this.optimizer = optimizer;   
        }
        public async Task<Vector?> SolveMin(
            Vector? startX = null,
            IndentWriter? Writer = null,
            int? maxK = null) => await Solve(true, startX, Writer, maxK);

        public async Task<Vector?> SolveMax(
            Vector? startX = null,
            IndentWriter? Writer = null,
            int? maxK = null) => await Solve(false, startX, Writer, maxK);

        private async Task<Vector?> Solve(
            bool IsMin,
            Vector? startX = null,
            IndentWriter? Writer = null,
            int? maxK = null)
        {
            Writer ??= IndentWriter.Null;
            int k = 0;
            await Writer.WriteLineAsync($"A = {P.A}");
            await Writer.WriteLineAsync($"b = {P.b}");
            if (P.ForcePositive)
            {
                await Writer.Indent.WriteLineAsync("Implicitly added x ≥ 0 disequation");
            }

            if (startX is not null && P.IsOutside(startX))
            {
                await Writer.Orange.WriteLineAsync($"Starting x = {startX} is not in the polyhedron!");
                await Writer.Orange.WriteLineAsync($"A new starting point will be generated randomly");
                startX = null;
            }

            Vector xk = startX ??
                P.RandomInternalPoint() ??
                throw new Exception("Could not find a random starting point!");

            while (true)
            {
                if (maxK.HasValue && k >= maxK.Value)
                {
                    await Writer.Red.WriteLineAsync($"Optimal value not reached in {k} iterations.");
                    await Writer.Red.WriteLineAsync($"Exiting with failure.");
                    break;
                }
                await Writer.Bold.WriteLineAsync($"Iteration #{k}:");

                await Writer.WriteLineAsync($"x_{k} = {xk}");
                Writer.LogObject($"FW_{k}", xk);

                if (P.IsOutside(xk)) // Check if A * xk <= b. In that case stop (an error has appened)
                {
                    await Writer.Orange.WriteLineAsync($"Vector x_{k} is out of bound!");
                    break;
                }

                Vector gradF = optimizer.Gradient(xk);
                await Writer.WriteLineAsync($"∇f(x_{k}) = {gradF}");

                Simplex s = new(
                    P,
                    gradF * (IsMin ? Fraction.MinusOne : Fraction.One)); // If we want a max of the simplex we have to multiply its c coefficients by -1 

                Vector? yk = await s.SolvePrimalMax(IndentWriter.Null, null, maxIterations: 25);
                if (yk is null)
                {
                    await Writer.Indent.Red.WriteLineAsync($"It was impossible for the simplex to find y_{k}.");
                    await Writer.Indent.Red.WriteLineAsync($"Exiting with failure.");
                    break;
                }
                await Writer.WriteLineAsync($"y_{k} = {yk}");

                Vector dk = yk - xk;
                await Writer.WriteLineAsync($"d_{k} = y_{k} - x_{k} = {dk}");
                Writer.LogObject(
                    $"FW_d{k}", 
                    new Tuple<Vector, Vector>(xk, dk));

                if (dk.IsZero)
                {
                    await Writer.Indent.Green.WriteLineAsync($"Found best value.");
                    await Writer.Indent.Green.WriteLineAsync($"Exiting with success.");
                    return xk;
                }

                Fraction tk = optimizer.FindArgOfFunction(IsMin,
                    Fraction.Zero, Fraction.One, 5000, xk, dk);
                await Writer.WriteLineAsync($"t_{k} = {Models.Function.Print(tk)}");
                if (tk.IsZero)
                {
                    await Writer.Red.WriteLineAsync($"Internal error (t_{k} = 0).");
                    break;
                }

                xk += tk * dk;
                k++;

                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
            }
            return null;
        }
    }

    public class PythonFrankWolfe: FrankWolfe<PythonFunctionAnalyzer>
    {
        public PythonFrankWolfe(Polyhedron polyhedron, string s) : 
            base(new PythonFunctionAnalyzer(polyhedron, s)) {  }

        public async Task<bool> SolveFlow(bool min, IndentWriter? Writer, Vector? startX)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                await Writer.Bold.WriteLineAsync(
                    $"{(min ? "Minimizing" : "Maximizing")} via Frank-Wolfe method");
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
            catch (Exception ex) {
                await Writer.Red.WriteLineAsync($"Exception happened: '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
                return false;
            }
        }
    }

    public class QuadProgFrankeWolfe : FrankWolfe<QuadProgFunctionAnalyzer>
    {
        public QuadProgFrankeWolfe(Polyhedron polyhedron, Matrix H, Vector l) :
            base(new QuadProgFunctionAnalyzer(polyhedron, H, l))
        { }
    }
}