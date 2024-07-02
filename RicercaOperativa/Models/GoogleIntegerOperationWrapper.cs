using Accord.Math;
using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.LinearSolver;
using OperationalResearch.Models.Elements;
using Vector = OperationalResearch.Models.Elements.Vector;
using OperationalResearch.Extensions;

namespace OperationalResearch.Models
{
    internal class GoogleIntegerOperationWrapper
    {
        private readonly Polyhedron P;
        private readonly Vector c;
        private const string SOLVER_ID = "SCIP";
        public GoogleIntegerOperationWrapper(
            Polyhedron p, Vector c)
        {
            ArgumentNullException.ThrowIfNull(p, nameof(p));
            ArgumentNullException.ThrowIfNull(c, nameof(c));

            if (p.Cols != c.Size)
            {
                throw new ArgumentException(
                    $"A must have col number equal to the size of c ({p.Cols} != {c.Size})");
            }

            P = p;
            this.c = c;
        }
        private int[]? Solve(bool max)
        {
            Solver solver = Solver.CreateSolver(SOLVER_ID);
            if (solver is null)
            {
                return null;
            }
            double lb = P.ForcePositive ? 0.00 : double.NegativeInfinity;
            Variable[] x = c.Indices.Select(i =>
                solver.MakeIntVar(lb, double.PositiveInfinity, $"x_{i + 1}")).ToArray();
            for (int i = 0; i < P.A.Rows; ++i)
            {
                Constraint constraint = solver.MakeConstraint(lb, P.b[i].ToDouble(), $"A[{i + 1}] * x <= b_{i + 1}");
                for (int j = 0; j < c.Size; ++j)
                {
                    constraint.SetCoefficient(x[j], P.A[i, j].ToDouble());
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
            }
            else
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
        public int[]? FindMax() => Solve(true);
        public int[]? FindMin() => Solve(false);
        public async Task<bool> SolveMaxFlow(IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                await Writer.WriteLineAsync($"A|b = {P.A | P.b}");
                if (P.ForcePositive)
                {
                    await Writer.WriteLineAsync($"x >= 0 added at the end of A added.");
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
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent().WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }
        public async Task<bool> SolveMinFlow(IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                await Writer.WriteLineAsync($"A|b = {P.A | P.b}");
                if (P.ForcePositive)
                {
                    await Writer.WriteLineAsync($"x >= 0 added at the end of A added.");
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
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent().WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }
    }
}
