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
        private readonly Fraction[] b;
        private readonly Fraction[] c;

        public Simplex(Fraction[,] A, Fraction[] b, Fraction[] c)
        {
            ArgumentNullException.ThrowIfNull(A);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNull(c);
            if (A.Rows() != b.Length)
            {
                throw new ArgumentException("A must have row number equal to the length of b");
            }
            if (A.Columns() != c.Length)
            {
                throw new ArgumentException("A must have col number equal to the length of c");
            }

            // x >= 0 constarint (-x <= 0)
            for (int i = 0; i < A.Columns(); i++)
            {
                Fraction[] rowToAdd = 
                    Enumerable.Repeat(Fraction.Zero, i)
                    .Concat(new Fraction[1] { new Fraction(-1) })
                    .Concat(Enumerable.Repeat(Fraction.Zero, A.Columns() - i - 1))
                    .ToArray();
                A = A.InsertRow(rowToAdd);
                b = b.Concat(new Fraction[1] { Fraction.Zero }).ToArray();
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

                Fraction[] b_B = ExtractRows(b, B);
                Fraction[] b_N = ExtractRows(b, N);

                Fraction[] x = A_B_inv * b_B;
                if (LessOrEqual(A[N] * x, b_N))
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
        public async Task<Fraction[]?> SolvePrimal(
            StreamWriter? Writer, 
            int[]? startBase = null,
            int? maxIterations = null)
        {
            ArgumentNullException.ThrowIfNull(Writer);

            int[] B = startBase ?? FindStartBase(); B.Sort();
            int[] N = Enumerable.Range(0, A.Rows).Where(i => !B.Contains(i)).ToArray();
            await Writer.WriteLineAsync("Start:");
            await Writer.WriteLineAsync($"c = {Print(c)}");
            await Writer.WriteLineAsync($"A = {A}");
            await Writer.WriteLineAsync($"b = {Print(b)}");
            await Writer.WriteLineAsync();

            await Writer.WriteLineAsync($"B = {Print(B)}");
            await Writer.WriteLineAsync($"N = {Print(N)}");
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
                    throw new ArgumentException($"Base {Print(B)} is invalid");
                }
                Matrix A_B_inv = A_B.Inv;
                await Writer.WriteLineAsync($"A_B^-1 = {A_B_inv}");
                await Writer.WriteLineAsync();

                Fraction[] b_B = ExtractRows(b, B);
                Fraction[] b_N = ExtractRows(b, N);
                await Writer.WriteLineAsync($"b_B = {Print(b_B)}");
                await Writer.WriteLineAsync($"b_N = {Print(b_N)}");
                await Writer.WriteLineAsync();

                Fraction[] x = A_B_inv * b_B;
                await Writer.WriteLineAsync($"x = {Print(x)}");
                if (!LessOrEqual(A[N] * x, b_N))
                {
                    // Non acceptable solution
                    throw new DataMisalignedException($"Solution x = {Print(x)}  of base B = {Print(B)} is not acceptable");
                }
                await Writer.WriteLineAsync();


                Fraction[] Y_B = c * A_B_inv;
                await Writer.WriteLineAsync($"Y_B = {Print(Y_B)}");
                await Writer.WriteLineAsync();
                if (Y_B.All(yi => !yi.IsNegative)) // y_b >= 0
                {
                    // Optimal value
                    return x;
                } else {
                    await Writer.WriteLineAsync($"x is not yet optimal");
                }
                //Fraction[] Y = [.. Y_B, .. Enumerable.Repeat(Fraction.Zero, N.Length)];

                int h = findExitIndex(Y_B, B);
                await Writer.WriteLineAsync($"h = {h + 1}");

                Fraction[] Wh = A_B_inv.Col(
                    B.Find(i => i == h).First())
                    .Select(wh => wh * (-1)).ToArray(); // Wh is the h-th column of -A_b_inv
                await Writer.WriteLineAsync($"Wh = {Print(Wh)}");
                await Writer.WriteLineAsync();

                if (N.All(i => !Matrix.Scalar(A[i], Wh).IsPositive))
                {
                    // optimal value is +inf
                    await Writer.WriteLineAsync($"A[i] * Wh <= 0 for each i inside N");
                    return null;
                }


                int k = findEnteringIndex(A, Wh, b, x, N, out Fraction Theta);
                await Writer.WriteLineAsync($"Theta = {Theta}");
                await Writer.WriteLineAsync($"k = {k + 1}");
                await Writer.WriteLineAsync();

                B = B.Where(i => i != h).Append(k).ToArray(); B.Sort();
                N = N.Where(i => i != k).Append(h).ToArray(); N.Sort();
                await Writer.WriteLineAsync($"B = B - {{ {h + 1} }} U {{ {k + 1} }} = {Print(B)}");
                await Writer.WriteLineAsync();
                step++;

                if (maxIterations.HasValue && step > maxIterations.Value)
                {
                    throw new ApplicationException("Could not complete the calculation: max iteration limit exceeded");
                }
            }

        }
        public string CalculatePrimal(Fraction[]? primalSolution)
        {
            if (primalSolution is null || primalSolution.Length == 0)
            {
                return "Infinity: problem unbounded";
            }
            return $"c * x = {Matrix.Scalar(c, primalSolution)}";
        }
        private static int findExitIndex(Fraction[] Y_B, int[] B)
        {
            int vecIndex = Y_B.Find(y => y.IsNegative).First();
            return B[vecIndex];
        }
        private static int findEnteringIndex(Matrix A, Fraction[] Wh, Fraction[] b, Fraction[] x, int[] N, out Fraction Theta)
        {
            int[] PositiveRowsInN = N.Where(i => Matrix.Scalar(A[i], Wh) > 0).ToArray();
            Fraction ThetaCopy = Theta = PositiveRowsInN.Min(
                    i => (b[i] - Matrix.Scalar(A[i], x)) / Matrix.Scalar(A[i], Wh));

            int k = PositiveRowsInN.Where(i =>
                    (b[i] - Matrix.Scalar(A[i], x)) / Matrix.Scalar(A[i], Wh) == ThetaCopy).Min();
            return k;
        }

        public async Task<bool> SolvePrimalFlow(int[]? startBase = null, StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            bool exitValue = true;
            try
            {
                Fraction[] x = await SolvePrimal(
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
            //await Writer.FlushAsync();
            return exitValue;
        }


        public static bool LessOrEqual(Fraction[] a, Fraction[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException($"a and b have different lengths ({a.Length} != {b.Length})");
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > b[i])
                    return false;
            }
            return true;
        }
        public static Fraction[] ExtractRows(Fraction[] A, int[] Base)
        {
            List<Fraction> rows = [];
            for (int i = 0; i < A.Length; i++)
            {
                if (Base.Contains(i))
                {
                    rows.Add(A[i]);
                }
            }
            return [.. rows];
        }
        public static string Print(Fraction[] A)
        {
            if (A is null || A.Length == 0)
                return "{ }";
            return "{ " + string.Join(", ", A.Select(x => x.ToString())) + " }";
        }
        public static string Print(IEnumerable<int> A)
        {
            if (A is null || !A.Any())
                return "{ }";
            return "{ " + string.Join(", ", A.Select(x => (x + 1).ToString())) + " }";
        }
        public static Fraction[] Sum(Fraction[] a, Fraction[] b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Length != b.Length)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Length} != {b.Length})");
            }
            return Enumerable.Range(0, a.Length).Select(i => a[i] + b[i]).ToArray();
        }
        public static Fraction[] Sub(Fraction[] a, Fraction[] b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Length != b.Length)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Length} != {b.Length})");
            }
            return Enumerable.Range(0, a.Length).Select(i => a[i] - b[i]).ToArray();
        }
        public static Fraction[] Mult(Fraction[] a, Fraction[] b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Length != b.Length)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Length} != {b.Length})");
            }
            return Enumerable.Range(0, a.Length).Select(i => a[i] * b[i]).ToArray();
        }
        public static Fraction[] Mult(Fraction a, Fraction[] b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return Enumerable.Range(0, b.Length).Select(i => a * b[i]).ToArray();
        }
        public static Fraction[] Div(Fraction[] a, Fraction[] b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Length != b.Length)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Length} != {b.Length})");
            }
            return Enumerable.Range(0, a.Length).Select(i => a[i] / b[i]).ToArray();
        }
    }
}
