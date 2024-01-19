using Accord.Math;
using Fractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RicercaOperativa.Models
{
    internal class Simplex
    {
        private readonly Matrix A;
        private readonly Vector b;
        private readonly Vector c;

        public Simplex(Fraction[,] A, Vector b, Vector c, bool AddXPositiveOrZeroCostraint = true)
        {
            ArgumentNullException.ThrowIfNull(A);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNull(c);
            if (A.Rows() != b.Size)
            {
                throw new ArgumentException("A must have row number equal to the size of b");
            }
            if (A.Columns() != c.Size)
            {
                throw new ArgumentException("A must have col number equal to the size of c");
            }

            if (AddXPositiveOrZeroCostraint)
            {
                // x >= 0 constarint (-x <= 0)
                for (int i = 0; i < A.Columns(); i++)
                {
                    Fraction[] rowToAdd =
                        Enumerable.Repeat(Fraction.Zero, i)
                        .Concat(new Fraction[1] { new(-1) })
                        .Concat(Enumerable.Repeat(Fraction.Zero, A.Columns() - i - 1))
                        .ToArray();
                    A = A.InsertRow(rowToAdd);
                    b = b.Get.Concat(new Fraction[1] { Fraction.Zero }).ToArray();
                }
            }
            this.A = new Matrix(A);
            this.b = b;
            this.c = c;
        }
        private int[] FindStartBase()
        {
            int guesses = 0;
            do
            {
                int[] B = [.. new Random().GetItems(  Enumerable.Range(0, A.Rows).ToArray(),  A.Cols)];
                int[] N = Enumerable.Range(0, A.Rows).Where(i => !B.Contains(i)).ToArray();

                Matrix A_B = A[B];
                if (A_B.Det.IsZero)
                {
                    continue;
                }
                Matrix A_B_inv = A_B.Inv;

                Vector b_B = b[B];
                Vector b_N = b[N];

                Vector x = A_B_inv * b_B;
                if ((A[N] * x) <= b_N)
                {
                    // Solution is acceptable => Base is acceptable
                    return B;
                }
                guesses++;
                if (guesses > 1000)
                {
                    throw new InvalidNumberException("Could not guess a valid base in 1000 guesses!");
                }
            } while (true);
        }
        public async Task<Vector?> SolvePrimalMax(
            StreamWriter Writer, 
            int[]? startBase = null,
            int? maxIterations = null)
        {
            ArgumentNullException.ThrowIfNull(Writer);

            int[] B = startBase ?? FindStartBase(); B.Sort();
            int[] N = Enumerable.Range(0, A.Rows).Where(i => !B.Contains(i)).ToArray();
            await Writer.WriteLineAsync("Start:");
            await Writer.WriteLineAsync($"c = {c}");
            await Writer.WriteLineAsync($"A = {A}");
            await Writer.WriteLineAsync($"b = {b}");
            await Writer.WriteLineAsync();

            await Writer.WriteLineAsync($"B = {Function.Print(B)}");
            await Writer.WriteLineAsync($"N = {Function.Print(N)}");
            Debug.Assert(B.Length + N.Length == A.Rows);
            Trace.Assert(B.Length + N.Length == A.Rows);


            int step = 1;

            while (true)
            {
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
                Writer.WriteLine($"Step #{step}:");

                Matrix A_B = A[B];
                await Writer.WriteLineAsync($"A_B = {A_B}");
                Matrix A_N = A[N];
                await Writer.WriteLineAsync($"A_N = {A_N}");
                if (A_B.Det.IsZero)
                {
                    throw new ArgumentException($"Base {Function.Print(B)} is invalid: Det(A_B) = 0");
                }
                Matrix A_B_inv = A_B.Inv;
                await Writer.WriteLineAsync($"A_B^-1 = {A_B_inv}");
                await Writer.WriteLineAsync();

                Vector b_B = b[B];
                Vector b_N = b[N];
                await Writer.WriteLineAsync($"b_B = {b_B}");
                await Writer.WriteLineAsync($"b_N = {b_N}");
                await Writer.WriteLineAsync();

                Vector x = A_B_inv * b_B;
                await Writer.WriteLineAsync($"x = {x}");
                if (!((A[N] * x) <= b_N))
                {
                    // Non acceptable solution
                    throw new DataMisalignedException(
                        $"Solution x = {x}  of base B = {Function.Print(B)} is not acceptable");
                }
                await Writer.WriteLineAsync();


                Vector Y_B = (c.Row * A_B_inv)[0]; // Get first row of one row matrix
                await Writer.WriteLineAsync($"Y_B = {Y_B}");
                await Writer.WriteLineAsync();
                if (Y_B.IsPositiveOrZero) // y_b >= 0
                {
                    // Optimal value
                    return x;
                } else {
                    await Writer.WriteLineAsync($"x is not yet optimal");
                }
                //Fraction[] Y = [.. Y_B, .. Enumerable.Repeat(Fraction.Zero, N.Length)];

                int h = FindExitIndex(Y_B, B);
                await Writer.WriteLineAsync($"h = {h + 1}");

                Vector Wh = A_B_inv.Col( B.Find(i => i == h).First() ) * (-1); // Wh is the h-th column of -A_b_inv
                await Writer.WriteLineAsync($"Wh = {Wh}");
                await Writer.WriteLineAsync();

                if (N.All(i => !(A[i] * Wh).IsPositive))
                {
                    // optimal value is +inf
                    await Writer.WriteLineAsync($"A[i] * Wh <= 0 for each i inside N");
                    return null;
                }


                int k = FindEnteringIndex(A, Wh, b, x, N, out Fraction Theta);
                await Writer.WriteLineAsync($"Theta = {Theta}");
                await Writer.WriteLineAsync($"k = {k + 1}");
                await Writer.WriteLineAsync();

                B = B.Where(i => i != h).Append(k).ToArray(); B.Sort();
                N = N.Where(i => i != k).Append(h).ToArray(); N.Sort();
                await Writer.WriteLineAsync($"B = B - {{ {h + 1} }} U {{ {k + 1} }} = {Function.Print(B)}");
                await Writer.WriteLineAsync();
                step++;

                if (maxIterations.HasValue && step > maxIterations.Value)
                {
                    throw new ApplicationException("Could not complete the calculation: max iteration limit exceeded");
                }
            }

        }
        
        public string CalculatePrimal(Vector? primalSolution)
        {
            if (primalSolution is null || primalSolution.Size == 0)
            {
                return "Infinity: problem unbounded";
            }
            return $"c * x = {Function.Print(c * primalSolution)}";
        }
        private static int FindExitIndex(Vector Y_B, int[] B)
        {
            int vecIndex = Y_B.NegativeIndexes.First();
            return B[vecIndex];
        }
        private static int FindEnteringIndex(
            Matrix A, 
            Vector Wh, 
            Vector b, 
            Vector x, 
            int[] N, 
            out Fraction Theta)
        {
            int[] PositiveRowsInN = N.Where(i => (A[i] * Wh).IsPositive).ToArray();
            Fraction ThetaCopy = Theta = PositiveRowsInN.Min(
                    i => (b[i] - (A[i] * x)) / (A[i] * Wh));

            int k = PositiveRowsInN.Where(i =>
                    (b[i] - (A[i] * x)) / (A[i] * Wh) == ThetaCopy).Min();
            return k;
        }

        public async Task<bool> SolvePrimalMaxFlow(int[]? startBase = null, StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            bool exitValue = true;
            try
            {
                Vector? x = await SolvePrimalMax(
                    startBase: startBase, 
                    Writer: Writer, 
                    maxIterations: 100);
                await Writer.WriteLineAsync(CalculatePrimal(x));
            } catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
                exitValue = false;
            }
            return exitValue;
        }
        
    }
}
