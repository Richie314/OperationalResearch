using Accord.Math;
using Fractions;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    internal class MinCostAssign(Fraction[,] c)
    {
        readonly Matrix c = new(c);

        public MinCostAssign(string[,] c) : this(c.Apply(Fraction.FromString))
        {
        }
        private int N { get => c.Rows; }
        private IEnumerable<int> Indeces { get => c.RowsIndeces; }
        private Vector GetC()
        {
            return c.M.Flatten();
        }
        private Matrix GetPLMatrix()
        {
            // xi1 + xi2 + xi3 + ... + xiN <= 1
            var FirstPartPos = Indeces.Select(i =>
            {
                var ZeroPartsStarts = Enumerable.Repeat(Fraction.Zero, N * i).ToArray();
                var OneParts = Enumerable.Repeat(Fraction.One, N * 1).ToArray();
                var ZeroPartsEnd = Enumerable.Repeat(Fraction.Zero, N * (N - i - 1)).ToArray();
                return ZeroPartsStarts.Concatenate(OneParts).Concatenate(ZeroPartsEnd);
            });
            // xi1 + xi2 + xi3 + ... + xiN >= 1
            var FirstPartNeg = Indeces.Select(i =>
            {
                var ZeroPartsStarts = Enumerable.Repeat(Fraction.Zero, N * i).ToArray();
                var OneParts = Enumerable.Repeat(Fraction.MinusOne, N * 1).ToArray();
                var ZeroPartsEnd = Enumerable.Repeat(Fraction.Zero, N * (N - i - 1)).ToArray();
                return ZeroPartsStarts.Concatenate(OneParts).Concatenate(ZeroPartsEnd);
            });
            // x1i + x2i + x31 + ... + xNi <= 1
            var SecondPartPos = Indeces.Select(i =>
            {
                var Block =
                    Enumerable.Repeat(Fraction.Zero, i).ToList().Concat(
                    new List<Fraction>() { Fraction.One }).Concat(
                    Enumerable.Repeat(Fraction.Zero, N - 1 - i));
                return Enumerable.Repeat(Block.ToArray(), N).ToArray().Flatten();
            });
            // x1i + x2i + x31 + ... + xNi >= 1
            var SecondPartNeg = Indeces.Select(i =>
            {
                var Block =
                    Enumerable.Repeat(Fraction.Zero, i).ToList().Concat(
                    new List<Fraction>() { Fraction.MinusOne }).Concat(
                    Enumerable.Repeat(Fraction.Zero, N - 1 - i));
                return Enumerable.Repeat(Block.ToArray(), N).ToArray().Flatten();
            });
            var FullMatrix =
                // xij <= 1
                Matrix.Identity(N * N).M.ToJagged().Concat(
                    FirstPartPos).Concat(
                    FirstPartNeg).Concat(
                    SecondPartPos).Concat(
                    SecondPartNeg).ToArray();
            return new Matrix(FullMatrix);
        }
        private Vector GetPLVector()
        {
            // xi1 + xi2 + xi3 + ... + xiN <= 1
            var FirstPartPos = Enumerable.Repeat(Fraction.One, N);
            // xi1 + xi2 + xi3 + ... + xiN >= 1
            var FirstPartNeg = Enumerable.Repeat(Fraction.MinusOne, N);
            // x1i + x2i + x31 + ... + xNi <= 1
            var SecondPartPos = Enumerable.Repeat(Fraction.One, N);
            // x1i + x2i + x31 + ... + xNi >= 1
            var SecondPartNeg = Enumerable.Repeat(Fraction.MinusOne, N);
            var ThirdPart = Enumerable.Repeat(Fraction.One, N * N);
            
            var FullVector =
                // xij <= 1
                ThirdPart.Concat(
                    FirstPartPos).Concat(
                    FirstPartNeg).Concat(
                    SecondPartPos).Concat(
                    SecondPartNeg).ToArray();
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

            var A = GetPLMatrix();
            await Writer.WriteLineAsync($"Matrix A = {A}");

            var B = GetPLVector();
            await Writer.WriteLineAsync($"A * x <= B = {B}");

            Simplex s = new(
                A.M, 
                B, 
                GetC() * Fraction.MinusOne); // We want to minimize c, not maximize

            await Writer.WriteLineAsync("Solving with simplex...");
            var x = await s.SolvePrimalMax(
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
            throw new NotImplementedException();
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
