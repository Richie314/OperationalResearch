using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.Models.Python
{
    internal sealed class FrankWolfe : PythonFunctionAnalyzer
    {
        public FrankWolfe(Polyhedron P, string python) : base(P, python) { }
        public override async Task<Vector?> SolveMin(
            Vector? startX = null,
            IndentWriter? Writer = null,
            int? maxK = null) => await Solve(true, startX, Writer, maxK);

        public override async Task<Vector?> SolveMax(
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

            if (startX is not null && !P.IsInside(startX))
            {
                await Writer.WriteLineAsync($"Starting x = {startX} is not in the polyhedron!.");
                await Writer.WriteLineAsync($"A new starting point will be generated randomly");
                startX = null;
            }

            Vector xk = startX ?? P.RandomInternalPoint() ?? throw new Exception("Could not find a random starting point!");
            while (true)
            {
                if (maxK.HasValue && k >= maxK.Value)
                {
                    await Writer.WriteLineAsync($"Optimal value not reached in {k} iterations.");
                    await Writer.WriteLineAsync($"Exiting with failure.");
                    break;
                }
                await Writer.WriteLineAsync($"Iteration #{k}:");

                await Writer.WriteLineAsync($"x_{k} = {xk}");

                if (!P.IsInside(xk)) // Check if A * xk <= b. In that case stop (an error has appened)
                {
                    await Writer.WriteLineAsync();
                    await Writer.WriteLineAsync($"Vector x_{k} is out of bound!");
                    await Writer.WriteLineAsync();
                    break;
                }

                Vector gradF = Grad(xk);
                await Writer.WriteLineAsync($"∇f(x_{k}) = {gradF}");

                Simplex s = new(
                    P,
                    gradF * (IsMin ? Fraction.MinusOne : Fraction.One)); // If we want a max of the simplex we have to multiply its c coefficients by -1 

                Vector? yk = await s.SolvePrimalMax(IndentWriter.Null, null, null);
                if (yk is null)
                {
                    await Writer.WriteLineAsync($"It was impossible for the simplex to find y_{k}.");
                    await Writer.WriteLineAsync($"Exiting with failure.");
                    break;
                }
                await Writer.WriteLineAsync($"y_{k} = {yk}");

                Vector dk = yk - xk;
                await Writer.WriteLineAsync($"d_{k} = y_{k} - x_{k} = {dk}");

                if (dk.IsZero)
                {
                    await Writer.WriteLineAsync($"Found best value.");
                    await Writer.WriteLineAsync($"Exiting with success.");
                    return xk;
                }

                Fraction tk = FindArgOfFunction(IsMin,
                    Fraction.Zero, Fraction.One, 5000, xk, dk);
                await Writer.WriteLineAsync($"t_{k} = {Models.Function.Print(tk)}");
                if (tk.IsZero)
                {
                    await Writer.WriteLineAsync($"Internal error.");
                    break;
                }

                xk += tk * dk;
                k++;

                await Writer.WriteLineAsync("Going to new iteraion");
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
            }
            return null;
        }
    }
}