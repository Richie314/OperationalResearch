using Accord.Math;
using Fractions;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace OperationalResearch.Models.Python
{
    internal sealed class FrankWolfe : PythonFunctionAnalyzer
    {
        public FrankWolfe(Fraction[,] A, Vector b, string python) : base(A, b, python) { }
        public override async Task<Vector?> SolveMin(
            Vector? startX = null,
            StreamWriter? Writer = null,
            int? maxK = null)
        {
            return await Solve(true, startX, Writer, maxK);
        }
        public override async Task<Vector?> SolveMax(
            Vector? startX = null,
            StreamWriter? Writer = null,
            int? maxK = null)
        {
            return await Solve(false, startX, Writer, maxK);
        }
        private async Task<Vector?> Solve(
            bool IsMin,
            Vector? startX = null,
            StreamWriter? Writer = null,
            int? maxK = null)
        {
            Writer ??= StreamWriter.Null;
            Matrix a = A;
            Vector b = B;
            int k = 0;

            Vector xk = startX ?? GetRandomStartPoint(A, B);
            while (true)
            {
                if (maxK.HasValue && k >= maxK.Value)
                {
                    await Writer.WriteLineAsync($"Optimal value not reached in {k} iterations.");
                    await Writer.WriteLineAsync($"Exiting with failure.");
                    break;
                }
                await Writer.WriteLineAsync($"Iteration {k}:");

                await Writer.WriteLineAsync($"A = {a}");
                await Writer.WriteLineAsync($"b = {b}");
                await Writer.WriteLineAsync($"x{k} = {xk}");

                if (a * xk > b) // Check if A * xk > b. In that case stop (an error has appened)
                {
                    await Writer.WriteLineAsync();
                    await Writer.WriteLineAsync($"Vector x{k} is out of bound!");
                    await Writer.WriteLineAsync();
                    break;
                }

                Vector gradF = Grad(xk);
                await Writer.WriteLineAsync($"gradF(x{k}) = {gradF}");

                Simplex s = new(
                    a.M, b,
                    gradF * (IsMin ? -1 : Fraction.One), // If we want a max of the simplex we have to multiply its c coefficients by -1 
                    false);
                Vector? yk = await s.SolvePrimalMax(StreamWriter.Null, null, null);
                if (yk is null)
                {
                    await Writer.WriteLineAsync($"It was impossible for the simplex to find y{k}.");
                    await Writer.WriteLineAsync($"Exiting with failure.");
                    break;
                }
                await Writer.WriteLineAsync($"y{k} = {yk}");

                Vector dk = yk - xk;
                await Writer.WriteLineAsync($"d{k} = y{k} - x{k} = {dk}");

                if (dk.IsZero)
                {
                    await Writer.WriteLineAsync($"Found best value.");
                    await Writer.WriteLineAsync($"Exiting with success.");
                    return xk;
                }

                Fraction tk = FindArgOfFunction(IsMin,
                    Fraction.Zero, Fraction.One, 5000, xk, dk);
                await Writer.WriteLineAsync($"t{k} = {Models.Function.Print(tk)}");

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