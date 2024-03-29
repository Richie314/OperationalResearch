﻿using Accord.Math;
using Fractions;
using Microsoft.Scripting.Utils;
using OperationalResearch.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    internal class Simplex
    {
        private readonly Matrix A;
        private readonly Vector b;
        private readonly Vector c;
        private readonly bool addPositive;

        public Simplex(Fraction[,] A, Vector b, Vector c, bool AddXPositiveOrZeroCostraint = true)
        {
            ArgumentNullException.ThrowIfNull(A);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNull(c);

            this.A = new Matrix(A);
            this.b = b;
            this.c = c;
            if (this.A.Rows != b.Size)
            {
                throw new ArgumentException(
                    $"A must have row number equal to the size of b ({this.A.Rows} != {b.Size}");
            }
            if (this.A.Cols != c.Size)
            {
                throw new ArgumentException(
                    $"A must have col number equal to the size of c ({this.A.Cols} != {c.Size})");
            }
            this.addPositive = AddXPositiveOrZeroCostraint;
        }
        private Matrix GetA()
        {
            if (!addPositive)
            {
                return A;
            }
            var copy = A.M.Copy();
            for (int i = 0; i < copy.Columns(); i++)
            {
                Fraction[] rowToAdd =
                    Enumerable.Repeat(Fraction.Zero, i)
                    .Concat(new Fraction[1] { new(-1) })
                    .Concat(Enumerable.Repeat(Fraction.Zero, copy.Columns() - i - 1))
                    .ToArray();
                copy = copy.InsertRow(rowToAdd);
            }
            return new Matrix(copy);
        }
        private Vector GetB()
        {
            if (!addPositive)
            {
                return b;
            }
            return b.Get.Concat(Enumerable.Repeat(Fraction.Zero, A.Cols)).ToArray();
        }
        private Vector GetC()
        {
            if (!addPositive)
            {
                return c;
            }
            return c.Get.Concat(Enumerable.Repeat(Fraction.Zero, A.Cols)).ToArray();
        }
        private static int[] FindStartBase(Matrix A, Vector b, Vector c, bool forX = true)
        {
            int guesses = 0;
            do
            {
                if (guesses > 1000)
                {
                    throw new InvalidNumberException("Could not guess a valid base in 1000 guesses!");
                }

                int[] B = [.. new Random().GetItems(  A.RowsIndeces.ToArray(),  A.Cols)];
                int[] N = A.RowsIndeces.Where(i => !B.Contains(i)).ToArray();

                Matrix A_B = A[B];
                if (A_B.Det.IsZero)
                {
                    continue;
                }
                Matrix A_B_inv = A_B.Inv;

                Vector b_B = b[B];
                Vector b_N = b[N];

                if (forX)
                {
                    Vector x = A_B_inv * b_B;
                    if ((A[N] * x) <= b_N)
                    {
                        // Solution is acceptable => Base is acceptable
                        return B;
                    }
                } else
                {
                    Vector Y_B = (c.Row * A_B_inv)[0];
                    if (Y_B.IsPositiveOrZero)
                    {
                        return B;
                    }
                }
                guesses++;
            } while (true);
        }
        public async Task<Vector?> SolvePrimalMax(
            StreamWriter Writer, 
            int[]? startBase = null,
            int? maxIterations = null)
        {
            ArgumentNullException.ThrowIfNull(Writer);
            var a = GetA();
            var VectorB = GetB();

            int[] B = startBase ?? FindStartBase(a, VectorB, c, true); B.Sort();
            int[] N = a.RowsIndeces.Where(i => !B.Contains(i)).ToArray();

            await Writer.WriteLineAsync("Start:");
            await Writer.WriteLineAsync($"c = {c}");
            await Writer.WriteLineAsync($"A = {a}");
            await Writer.WriteLineAsync($"b = {VectorB}");
            await Writer.WriteLineAsync();

            await Writer.WriteLineAsync($"B = {Function.Print(B)}");
            await Writer.WriteLineAsync($"N = {Function.Print(N)}");
            Debug.Assert(B.Length + N.Length == a.Rows);
            Trace.Assert(B.Length + N.Length == a.Rows);


            int step = 1;

            while (true)
            {
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
                Writer.WriteLine($"Step #{step}:");

                Matrix A_B = a[B];
                await Writer.WriteLineAsync($"A_B = {A_B}");
                Matrix A_N = a[N];
                await Writer.WriteLineAsync($"A_N = {A_N}");
                if (A_B.Det.IsZero)
                {
                    throw new ArgumentException($"Basis {Function.Print(B)} is invalid: Det(A_B) = 0");
                }
                Matrix A_B_inv = A_B.Inv;
                await Writer.WriteLineAsync($"A_B^-1 = {A_B_inv}");
                await Writer.WriteLineAsync();

                Vector b_B = VectorB[B];
                Vector b_N = VectorB[N];
                await Writer.WriteLineAsync($"b_B = {b_B}");
                await Writer.WriteLineAsync($"b_N = {b_N}");
                await Writer.WriteLineAsync();

                Vector x = A_B_inv * b_B;
                await Writer.WriteLineAsync($"x = {x}");
                var A_N_X = A_N * x;
                if (A_N_X <= b_N)
                {
                    await Writer.WriteLineAsync($"x is acceptable");

                } else
                {
                    // Non acceptable solution
                    throw new DataMisalignedException(
                        $"Solution x = {x}  of base B = {Function.Print(B)} is not acceptable");
                }
                await Writer.WriteLineAsync();

                var zeroIndeces = A_N_X.ZeroIndexes;
                if (zeroIndeces.Length != 0)
                {
                    await Writer.WriteLineAsync($"x is degenearate: solution also of other basis");
                    foreach (var i in zeroIndeces)
                    {
                        if (!N.Contains(i))
                        {
                            await Writer.WriteLineAsync($"\tThe {i}-th element of N also verifies the current vertex. It was impossible, however, to find that element in N");
                            continue;
                        }
                        await Writer.WriteLineAsync($"\tIndex {N[i]} in N is also verified");
                    }
                }


                Vector Y_B = (c.Row * A_B_inv)[0]; // Get first row of one row matrix
                await Writer.WriteLineAsync($"Y_B = {Y_B}");
                await Writer.WriteLineAsync();
                if (Y_B.IsPositiveOrZero) // y_b >= 0
                {
                    // Optimal value
                    await Writer.WriteLineAsync($"x is optimal.");

                    try
                    {
                        await Gomory(Writer, a, VectorB, x, B, addPositive);
                    } catch (Exception ex)
                    {
                        await Writer.WriteLineAsync(
                            "Error in calculating Gomory plane: " + ex.Message);
                    }

                    return x;
                } else {
                    await Writer.WriteLineAsync($"x is not yet optimal.");
                }

                int h = FindExitIndex(Y_B, B, out Fraction ExitingElement);
                await Writer.WriteLineAsync($"h = {h + 1} of element {Function.Print(ExitingElement)}");

                Vector Wh = A_B_inv.Col( B.Find(i => i == h).First() ) * (-1); // Wh is the h-th column of -A_b_inv
                await Writer.WriteLineAsync($"Wh = {Wh}");
                await Writer.WriteLineAsync();

                if (N.All(i => !(a[i] * Wh).IsPositive))
                {
                    // optimal value is +inf
                    await Writer.WriteLineAsync($"A[i] * Wh <= 0 for each i inside N");
                    return null;
                }


                int k = FindEnteringIndex(a, Wh, VectorB, x, N, out Fraction Theta);
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
                    throw new ApplicationException(
                        $"Could not complete the calculation: max iteration limit ({maxIterations.Value}) exceeded");
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
        public string CalculatePrimalConstraints(Vector? primalSolution)
        {
            if (primalSolution is null || primalSolution.Size == 0)
            {
                return "";
            }
            return $"A * x = {GetA() * primalSolution}";
        }
        private static int FindExitIndex(Vector Y_B, int[] B, out Fraction NegElement)
        {
            int vecIndex = Y_B.NegativeIndexes.First();
            NegElement = Y_B[vecIndex];
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
            if (PositiveRowsInN.Length == 0)
            {
                throw new Exception(
                    $"Impossible to find i in N : A[i] * {Wh} > 0");
            }
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
                await Writer.WriteLineAsync(CalculatePrimalConstraints(x));
            } catch (Exception ex)
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
        
        public async Task<Vector?> SolveDualMin(
            int[]? startBase = null, 
            StreamWriter? Writer = null,
            int? maxIterations = null)
        {
            ArgumentNullException.ThrowIfNull(Writer);

            int[] B = startBase ?? FindStartBase(A, b, c, false); B.Sort();
            int[] N = A.RowsIndeces.Where(i => !B.Contains(i)).ToArray();
            await Writer.WriteLineAsync("Start:");
            await Writer.WriteLineAsync($"b = {b}");
            await Writer.WriteLineAsync($"A = {A}");
            await Writer.WriteLineAsync($"c = {c}");
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

                Vector Y_B = (c.Row * A_B_inv)[0]; // Get first row of one row matrix
                await Writer.WriteLineAsync($"Y_B = {Y_B}");
                await Writer.WriteLineAsync();
                Vector Y = Enumerable.Range(0, A.Rows).Select(
                    i =>
                    {
                        if (B.Contains(i))
                        {
                            return Y_B[B.IndexOf(i)];
                        }
                        return Fraction.Zero;
                    }).ToArray();
                await Writer.WriteLineAsync($"Y = {Y}");

                if (addPositive)
                {
                    if (!Y.IsPositiveOrZero)
                    {
                        throw new ArgumentException("Y is not acceptable!. Y not >= 0");
                    }
                }
                
                Vector x = A_B_inv * b_B;
                await Writer.WriteLineAsync($"x = {x}");
                if (A * x <= b)
                {
                    await Writer.WriteLineAsync("x is acceptable");
                } else
                {
                    await Writer.WriteLineAsync("x is NOT acceptable");
                }

                if (b_N >= A_N * x)
                {
                    // Optimal value
                    await Writer.WriteLineAsync($"Y is optimal.");
                    await Writer.WriteLineAsync();

                    return Y;
                }
                // Find enetring index
                int k = N.Where(i => b[i] < A[i] * x).Min();
                await Writer.WriteLineAsync($"k = {k + 1}");

                Matrix W = (-1) * A_B_inv;
                Vector AkW = (A[k].Row * W)[0];
                await Writer.WriteLineAsync($"AkW = {AkW}");
                if (AkW.IsPositiveOrZero)
                {
                    // optimal value is -inf
                    await Writer.WriteLineAsync($"AkW >= 0");
                    await Writer.WriteLineAsync($"Problem cannot be solved");
                    return null;
                }

                Fraction Theta = AkW.NegativeIndexes.Select(i => B[i]).Select(i => Y[i] / AkW[B.IndexOf(i)] * (-1)).Min();
                await Writer.WriteLineAsync($"Theta = {Function.Print(Theta)}");

                int h = AkW.NegativeIndexes.Select(i => B[i]).Where(i => Y[i] / AkW[B.IndexOf(i)] * (-1) == Theta).Min();
                await Writer.WriteLineAsync($"h = {h + 1}");

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
        public async Task<bool> SolveDualMinFlow(int[]? startBase = null, StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            bool exitValue = true;
            try
            {
                Vector? y = await SolveDualMin(
                    startBase: startBase,
                    Writer: Writer,
                    maxIterations: 100);
                await Writer.WriteLineAsync(CalculateDual(y));
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
        public string CalculateDual(Vector? dualSolution)
        {
            if (dualSolution is null || dualSolution.Size == 0)
            {
                return "Infinity: problem unbounded";
            }
            return $"y^T * b = {Function.Print(dualSolution * b)}";
        }
        private static async Task Gomory(
            StreamWriter Writer,
            Matrix a, Vector b, 
            Vector Xrc, int[] BestBase,
            bool addedPositivesRestriction)
        {
            await Writer.WriteLineAsync(
                $"Calculating Gomory plane with primary B = {Function.Print(BestBase)}");

            if (!addedPositivesRestriction)
            {
                throw new NotImplementedException();
            }

            a = a[Enumerable.Range(0, a.Rows - a.Cols)];
            b = b[Enumerable.Range(0, a.Rows)];
            await Writer.WriteLineAsync($"Adding {a.Rows} variables");

            Matrix dualM = a | Matrix.Identity(a.Rows);
            await Writer.WriteLineAsync($"Matrix A of dual problem: {dualM}");
            await Writer.WriteLineAsync($"Vector b = A * x: {b}");


            var xRemain = Xrc.Concat(b - (a * Xrc)); // Added x_N+1, x_N+2, ...
            await Writer.WriteLineAsync($"x of dual problem: {xRemain}");
            var dualBase = xRemain.NonZeroIndeces;
            var dualN = dualM.ColsIndeces.Where(i => !dualBase.Contains(i));
            await Writer.WriteLineAsync($"Base of dual problem's solution: {Function.Print(dualBase)}");

            Matrix aB_Inv = 
                dualM.GetCols(dualBase).Inv * 
                dualM.GetCols(dualN);//*a[N] but a[N] is the identity matrix we added before!
            await Writer.WriteLineAsync($"A^~ = {aB_Inv}");

            Matrix aB_Inv_Frac = new(aB_Inv.M.Apply(a => a.FractionPart()));
            await Writer.WriteLineAsync($"{{ A^~ }} = {aB_Inv_Frac}");

            Vector XrcFrac = Xrc[
                aB_Inv_Frac.RowsIndeces.Where(i => i < Xrc.Size)
                ].Get.Select(x => x.FractionPart()).ToArray();
            await Writer.WriteLineAsync($"{{ X_rc }} = {XrcFrac}");

            await Writer.WriteLineAsync();

            for (int r = 0; r < XrcFrac.Size; r++)
            {
                await Writer.WriteAsync($"Plane from row {r + 1} of {{ A~ }}:");
                var arr = aB_Inv_Frac[r].Get.Apply((arj, j) =>
                {
                    if (arj.IsZero)
                    {
                        return string.Empty;
                    }
                    if (arj == Fraction.One)
                    {
                        return $"+ x{dualN.ElementAt(j) + 1}";
                    }
                    if (arj == Fraction.MinusOne)
                    {
                        return $"- x{dualN.ElementAt(j) + 1}";
                    }
                    if (arj.IsPositive)
                    {
                        return $"+{Function.Print(arj)} * x{dualN.ElementAt(j) + 1}";
                    }
                    return $"{Function.Print(arj)} * x{dualN.ElementAt(j) + 1}";
                }).Where(s => !string.IsNullOrEmpty(s));

                if (!arr.Any())
                {
                    await Writer.WriteLineAsync($"0 >= {Function.Print(XrcFrac[r])}");
                    continue;
                }
                await Writer.WriteLineAsync(
                    string.Join(" ", arr) + $" >= {Function.Print(XrcFrac[r])}");

                Vector components = Enumerable.Repeat(Fraction.Zero, a.Cols).ToArray();
                Fraction bComp = XrcFrac[r];
                for (int j = 0; j < aB_Inv_Frac.Cols; j++)
                {
                    int k = dualN.ElementAt(j);
                    if (k < a.Cols)
                    {
                        components[k] += aB_Inv_Frac[r, j];
                        continue;
                    }
                    int OriginalEquation = k - a.Cols;
                    bComp -= b[OriginalEquation] * aB_Inv_Frac[r, j];
                    components -= a[OriginalEquation] * aB_Inv_Frac[r, j];
                }
                components *= Fraction.MinusOne; bComp *= Fraction.MinusOne;
                await Writer.WriteLineAsync(
                    $"\tEquation to add: " +
                        string.Join("  ",
                            components.NonZeroIndeces.Select(
                                j => { 
                                    if (components[j] == Fraction.One)
                                    {
                                        return $"+ x{j + 1}";
                                    }
                                    if (components[j] == Fraction.MinusOne)
                                    {
                                        return $"- x{j + 1}";
                                    }
                                    return (components[j].IsPositive ? "+" : "") + $"{Function.Print(components[j])}*x{j + 1}";
                                }
                        )
                    ) + $" <= {Function.Print(bComp)}");
                
            }
            
        }
    }
}
