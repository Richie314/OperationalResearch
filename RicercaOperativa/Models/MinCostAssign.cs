using Accord.Math;
using Fractions;
using Microsoft.Scripting.Utils;
using OperationalResearch.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    internal class MinCostAssign(Fraction[,] c, Fraction[,] w, Fraction[] b, bool wxEqualsB = false)
    {
        private readonly Matrix w = new(w);
        private readonly Vector b = b;
        private readonly Matrix c = new(c);
        private readonly bool bEquals = wxEqualsB;

        public MinCostAssign(string[,] c, string[,] w, string[] b, bool bEq = false) : 
            this(c.Apply(Fraction.FromString), 
                w.Apply(Fraction.FromString), 
                b.Apply(Fraction.FromString),
                bEq)
        {
            if (c.Rows() != w.Rows())
            {
                throw new ArgumentException(
                    $"Matrix c and w must have same number of rows ({c.Rows()} != {w.Rows()})");
            }
            if (c.Columns() != b.Length)
            {
                throw new ArgumentException(
                    $"Cols of c and size of b must must be equal ({c.Columns()} != {b.Length})");
            }
            if (c.Columns() != w.Columns())
            {
                throw new ArgumentException(
                    $"Matrix c and w must have same number of cols ({c.Columns()} != {w.Columns()})");
            }
        }
        public MinCostAssign(string[,] c) : this(c.Apply(Fraction.FromString))
        {
        }
        public MinCostAssign(Fraction[,] c) :
            this(c,
                c.Apply(cij => Fraction.One),
                Enumerable.Repeat(Fraction.One, c.Rows()).ToArray(), true)
        {
        }
        private int N { get => c.Cols; }
        private int M { get => c.Rows; }
        private IEnumerable<int> RowIndeces { get => c.RowsIndeces; }
        private IEnumerable<int> ColIndeces { get => c.ColsIndeces; }
        private Vector GetC()
        {
            return c.M.Flatten();
        }
        private Matrix GetPLMatrix()
        {
            // xi1 + xi2 + xi3 + ... + xiN <= 1 for i < M
            var FirstPartPos = RowIndeces.Select(i =>
            {
                var ZeroPartsStarts = Enumerable.Repeat(Fraction.Zero, N * i).ToArray();
                var OneParts = Enumerable.Repeat(Fraction.One, N).ToArray();
                var ZeroPartsEnd = Enumerable.Repeat(Fraction.Zero, N * M - N - N * i).ToArray();
                return 
                    ZeroPartsStarts
                    .Concatenate(OneParts)
                    .Concatenate(ZeroPartsEnd);
            });
            // xi1 + xi2 + xi3 + ... + xiN >= 1 for i < M
            var FirstPartNeg = RowIndeces.Select(i =>
            {
                var ZeroPartsStarts = Enumerable.Repeat(Fraction.Zero, N * i).ToArray();
                var OneParts = Enumerable.Repeat(Fraction.MinusOne, N).ToArray();
                var ZeroPartsEnd = Enumerable.Repeat(Fraction.Zero, N * M - N - N * i).ToArray();
                return 
                ZeroPartsStarts
                .Concatenate(OneParts)
                .Concatenate(ZeroPartsEnd);
            });

            // w1i * x1i + w2i * x2i + w3i * x31 + ... + wMi * xMi <= bi for i < N
            var SecondPartPos = ColIndeces.Select(i =>
            {
                var wCol = w.Col(i);
                var LeadZeros = Enumerable.Repeat(Fraction.Zero, i).ToArray();
                var TrailZeros = Enumerable.Repeat(Fraction.Zero, N - 1 - i).ToArray();

                return wCol.Get.Select(wji =>
                    LeadZeros
                    .Concatenate([ wji ])
                    .Concatenate(TrailZeros))
                .ToArray().Flatten();
            });
            // w1i * x1i + w2i * x2i + w3i * x31 + ... + wMi * xMi >= bi for i < N
            
            var SecondPartNeg = !bEquals ? 
                Enumerable.Empty<Fraction[]>() : 
                ColIndeces.Select(i =>
            {
                var wColNeg = w.Col(i) * Fraction.MinusOne;
                var LeadZeros = Enumerable.Repeat(Fraction.Zero, i).ToArray();
                var TrailZeros = Enumerable.Repeat(Fraction.Zero, N - 1 - i).ToArray();

                return wColNeg.Get.Select(wji =>
                    LeadZeros
                    .Concatenate([wji])
                    .Concatenate(TrailZeros))
                .ToArray().Flatten();
            });
            
            var FullMatrix =
                // xij <= 1
                Matrix.Identity(N * M).M.ToJagged() // Xij <= 1
                    .Concat((Matrix.Identity(N * M) * Fraction.MinusOne).M.ToJagged()) // Xij >= 0
                    .Concat(FirstPartPos)
                    .Concat(FirstPartNeg)
                    .Concat(SecondPartPos)
                    .Concat(SecondPartNeg)
                    .ToArray();
            return new Matrix(FullMatrix);
        }
        private Vector GetPLVector()
        {
            // xi1 + xi2 + xi3 + ... + xiN <= 1
            var FirstPartPos = Enumerable.Repeat(Fraction.One, M);
            // xi1 + xi2 + xi3 + ... + xiN >= 1
            var FirstPartNeg = Enumerable.Repeat(Fraction.MinusOne, M);
            // w1i * x1i + w2i * x2i + w3i * x31 + ... + wMi * xMi <= bi
            var SecondPartPos = b.Get;
            // w1i * x1i + w2i * x2i + w3i * x31 + ... + wMi * xMi >= bi
            var SecondPartNeg = !bEquals ? 
                Enumerable.Empty<Fraction>() : (b * Fraction.MinusOne).Get;

            var LeadPart = Enumerable.Repeat(Fraction.One, N * M) // Xij <= 1
                .Concat(Enumerable.Repeat(Fraction.Zero, N * M)); // Xij >= 0
            
            var FullVector = LeadPart // 2 * N * M
                .Concat(FirstPartPos) // M
                .Concat(FirstPartNeg) // M
                .Concat(SecondPartPos) // N
                .Concat(SecondPartNeg) // N
                .ToArray();
            return FullVector;
        }

        private string GetXRepresentation()
        {
            return "( " + string.Join(", ",
                c.M.Apply((x, i, j) => $"x{i + 1}{j + 1}")
                .Flatten() ) + " )";
        }

        private string GetTargetFunction()
        {
            return string.Join(" + ",
                c.M.Apply((cij, i, j) => $"{Function.Print(cij)} * x{i + 1}{j + 1}")
                .Flatten());
        }
        /// <summary>
        /// x can be fractionary
        /// 0 <= x <= 1
        /// </summary>
        /// <param name="Writer"></param>
        /// <returns>The matrix of x, if it can be found</returns>
        public async Task<Matrix?> SolveCooperative(StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            await Writer.WriteLineAsync("Calculating representation of X...");
            await Writer.WriteLineAsync($"x = {GetXRepresentation()}");

            await Writer.WriteLineAsync($"Finding min of {GetTargetFunction()}");

            await Writer.WriteLineAsync($"A * x <= b");
            var A = GetPLMatrix();
            await Writer.WriteLineAsync($"A generated");
            var B = GetPLVector();
            await Writer.WriteLineAsync($"A|b = {A|B}");


            Simplex s = new(
                A.M, 
                B, 
                GetC() * Fraction.MinusOne, 
                false); // We want to minimize c, not maximize
            
            await Writer.WriteLineAsync("Solving with simplex...");
            var x = await s.SolvePrimalMax(// target function is already inverted in order to find min instead of max
                Writer: StreamWriter.Null,
                startBase: null,
                maxIterations: 50);

            if (x is null)
            {
                await Writer.WriteLineAsync("Problem could not be solved");
                return null;
            }
            return new Matrix(x.Get.Split(N));
        }
        
        public async Task<bool> SolveCooperativeFlow(StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            bool exitValue = true;
            try
            {
                Matrix? x = await SolveCooperative(Writer: Writer);
                if (x is null)
                {
                    return false;
                }
                await Writer.WriteLineAsync($"Solution X = {x}");
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                exitValue = false;
            }
            return exitValue;
        }
        public async Task<Matrix?> SolveNonCooperative(StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            IEnumerable<Fraction> bestX = Enumerable.Empty<Fraction>();
            Fraction bestXCost = Fraction.Zero;

            var StartVector = Enumerable.Repeat(Fraction.One, N)
                .Concat(Enumerable.Repeat(Fraction.Zero, N * M - N));

            await Writer.WriteLineAsync("Brute forcing the problem of non cooperative assignment of minimum cost.");
            await Writer.WriteLineAsync($"{N * M}! = {Function.Factorial(N * M)} guesses will be done...");

            await Writer.WriteLineAsync($"Time weights w = {w}");


            var A = GetPLMatrix();
            var b = GetPLVector();

            await Writer.WriteLineAsync($"A|b = {A | b}");

            foreach (IEnumerable<Fraction> x in StartVector.AllPermutations())
            {
                Vector X = x.ToArray();
                // await Writer.WriteLineAsync($"Case X = {X}");
                if (!(A * X <= b))
                {
                    // Solution not acceptable
                    continue;
                }
                Fraction currCost = GetC() * X;
                if (currCost < bestXCost || bestX.Count() == 0)
                {
                    bestXCost = currCost;
                    bestX = x;
                }
            }
            if (bestX is null || !bestX.Any())
            {
                await Writer.WriteLineAsync($"No acceptable solution was found. Exit with failure");
                return null;
            }
            await Writer.WriteLineAsync($"All guesses done: best X = {new Vector(bestX.ToArray())}");
            await Writer.WriteLineAsync($"Cost of solution = {Function.Print(bestXCost)}");
#if DEBUG
            await Writer.WriteLineAsync($"Ax = {A * bestX.ToArray()}");
            await Writer.WriteLineAsync($"b = {b}");
#endif
            return new Matrix(bestX.ToArray().Split(N)).T;
        }
        public async Task<bool> SolveNonCooperativeFlow(StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            bool exitValue = true;
            try
            {
                Matrix? x = await SolveNonCooperative(Writer: Writer);
                if (x is null)
                {
                    return false;
                }
                await Writer.WriteLineAsync($"Solution X = {x}");
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                exitValue = false;
            }
            return exitValue;
        }
    }
}
