using Accord.Math;
using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.LinearSolver;

namespace OperationalResearch.Models
{
    internal class GoogleIntegerLinearProgramming
    {
        private readonly Matrix A;
        private readonly Vector b;
        private readonly Vector c;
        private const string SOLVER_ID = "SCIP";
        bool xPos = true;
        public GoogleIntegerLinearProgramming(
            Fraction[,] A, Vector b, Vector c, bool AddXPositiveOrZeroCostraint = true)
        {
            ArgumentNullException.ThrowIfNull(A);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNull(c);

            if (A.Rows() != b.Size)
            {
                throw new ArgumentException(
                    $"A must have row number equal to the size of b ({A.Rows()} != {b.Size}");
            }
            if (A.Columns() != c.Size)
            {
                throw new ArgumentException(
                    $"A must have col number equal to the size of c ({A.Columns()} != {c.Size})");
            }
            xPos = AddXPositiveOrZeroCostraint;

            this.A = new Matrix(A);
            this.b = b;
            this.c = c;
        }
        private int[]? Solve(bool max)
        {
            Solver solver = Solver.CreateSolver(SOLVER_ID);
            if (solver is null)
            {
                return null;
            }
            double lb = xPos ? 0.00 : double.NegativeInfinity;
            Variable[] x = c.Indices.Select(i =>
                solver.MakeIntVar(lb, double.PositiveInfinity, $"x_{i + 1}")).ToArray();
            for (int i = 0; i < A.Rows; ++i)
            {
                Constraint constraint = solver.MakeConstraint(lb, b[i].ToDouble(), $"A[{i + 1}] * x <= b_{i + 1}");
                for (int j = 0; j < c.Size; ++j)
                {
                    constraint.SetCoefficient(x[j], A[i, j].ToDouble());
                }
            }

            Objective objective = solver.Objective();
            for (int j = 0; j < c.Size; ++j)
            {
                objective.SetCoefficient(x[j], c[j].ToDouble());
            }
            if (max)
            {
                objective.SetMaximization();
            } else
            {
                objective.SetMinimization();
            }

            Solver.ResultStatus resultStatus = solver.Solve();

            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                return null;
            }

            return x.Select(xi => (int)xi.SolutionValue()).ToArray();
        }
        public int[]? FindMax()
        {
            return Solve(true);
        }
        public int[]? FindMin()
        {
            return Solve(false);
        }
        public async Task<bool> SolveMaxFlow(StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            try
            {
                await Writer.WriteLineAsync($"A|b = {A | b}");
                if (xPos)
                {
                    await Writer.WriteLineAsync($"x >= 0 implicitly added.");
                }
                await Writer.WriteLineAsync($"Maximize c = {c}");
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync($"Building Google solver {SOLVER_ID}...");
                int[]? sol = FindMax();
                if (sol is null)
                {
                    await Writer.WriteLineAsync("No solution was found");
                    return false;
                }
                await Writer.WriteLineAsync($"Solution X = {Function.Print(sol, false)}");
                await Writer.WriteLineAsync($"c * X = {Function.Print(c * sol.Select(x => new Fraction(x)).ToArray())}");
                return true;
            } catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }
        public async Task<bool> SolveMinFlow(StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            try
            {
                await Writer.WriteLineAsync($"A|b = {A | b}");
                if (xPos)
                {
                    await Writer.WriteLineAsync($"x >= 0 implicitly added.");
                }
                await Writer.WriteLineAsync($"Minimize c = {c}");
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync($"Building Google solver {SOLVER_ID}...");
                int[]? sol = FindMax();
                if (sol is null)
                {
                    await Writer.WriteLineAsync("No solution was found");
                    return false;
                }
                await Writer.WriteLineAsync($"Solution X = {Function.Print(sol, false)}");
                await Writer.WriteLineAsync($"c * X = {Function.Print(c * sol.Select(x => new Fraction(x)).ToArray())}");
                return true;
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
                return false;
            }
        }
    }
}
