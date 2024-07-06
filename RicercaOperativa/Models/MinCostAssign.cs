using Accord.Math;
using Fractions;
using Microsoft.Scripting.Utils;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using Google.OrTools.Graph;
using Matrix = OperationalResearch.Models.Elements.Matrix;
using Vector = OperationalResearch.Models.Elements.Vector;
using Google.OrTools.Sat;

namespace OperationalResearch.Models
{
    public class MinimumCostAssign
    {
        /// <summary>
        /// Cost matrix
        /// </summary>
        private readonly Matrix c;

        /// <summary>
        /// Matrix of jobs durations
        /// </summary>
        private readonly Matrix w;

        /// <summary>
        /// Capacity of each worker: how much time can work
        /// </summary>
        private readonly Vector b;

        public IEnumerable<int> Workers
        {
            get => Enumerable.Range(0, c.Cols);
        }

        public IEnumerable<int> Jobs
        {
            get => Enumerable.Range(0, c.Rows);
        }


        public MinimumCostAssign(Matrix costs, Matrix weights, Vector b)
        {
            if (costs.Rows != weights.Rows)
            {
                throw new ArgumentException(
                    $"Matrix c and w must have same number of rows ({costs.Rows} != {weights.Rows})");
            }
            if (costs.Cols != weights.Cols)
            {
                throw new ArgumentException(
                    $"Matrix c and w must have same number of cols ({costs.Cols} != {weights.Cols})");
            }
            if (costs.Cols != b.Size)
            {
                throw new ArgumentException(
                    $"Cols of c and size of b must must be equal ({costs.Cols} != {b.Size})");
            }
            c = costs;
            w = weights;
            this.b = b;
        }
        public MinimumCostAssign(Matrix c) :
            this(c, new Matrix(c.M.Apply(cij => Fraction.One)), Vector.Ones(c.Cols)) { }

        private int N { get => c.Cols; }
        private int M { get => c.Rows; }

        private Simplex ConvertToLinearProgramming(bool FillWorkers = false)
        {
            Vector costs = c.M.Flatten();
            Polyhedron polyhedron = new(
                A: GetLinearProgrammingMatrix(FillWorkers),
                b: GetLinearProgrammingVector(FillWorkers),
                forcePositive: true);
            return new Simplex(polyhedron, costs).NegateTarget;
        }
        private Matrix GetLinearProgrammingMatrix(bool FillWorkers = false) 
        {
            // N: number of workers
            // M: number of jobs

            // Each job can be done once only
            // xi1 + xi2 + xi3 + ... + xiN <= 1 where i : job
            Matrix JobsOncePos = new(Jobs.Select(i =>
                Vector.Zeros(N * i)
                .Concat(Enumerable.Repeat(Fraction.One, N))
                .Concat(Vector.Zeros(N * M - N - N * i)).Get).ToArray());
            Matrix FullMatrix = JobsOncePos;
            // xi1 + xi2 + xi3 + ... + xiN >= 1
            Matrix JobsOnceNeg = Fraction.MinusOne * JobsOncePos;
            FullMatrix = FullMatrix.AddRows(JobsOnceNeg);

            // Workers have limit of time that can't exceed
            // w1i * x1i + w2i * x2i + w3i * x31 + ... + wMi * xMi <= bi where i : worker
            Matrix WorkersLimitPos = new(Workers.Select(
                i => w.Col(i).Get
                    .Select(wij => 
                        Vector.Zeros(i).Concat([wij]).Concat(Vector.Zeros(N - 1 - i)).Get
                    ).ToArray().Flatten()
                ).ToArray());
            FullMatrix.AddRows(WorkersLimitPos);
            if (FillWorkers)
            { 
                FullMatrix.AddRows(Fraction.MinusOne * WorkersLimitPos);
            }

            // x <= 1
            Matrix xBelowOne = Matrix.Identity(N * M);

            return FullMatrix.AddRows(xBelowOne);
        }
        private Vector GetLinearProgrammingVector(bool FillWorkers = false)
        {
            // xi1 + xi2 + xi3 + ... + xiN <= 1
            var JobsOncePos = new Vector(Enumerable.Repeat(Fraction.One, M));
            // xi1 + xi2 + xi3 + ... + xiN >= 1
            var JobsOnceNeg = Fraction.MinusOne * JobsOncePos;

            // w1i * x1i + w2i * x2i + w3i * x31 + ... + wMi * xMi <= bi
            var WorkersLimitPos = b;
            // w1i * x1i + w2i * x2i + w3i * x31 + ... + wMi * xMi >= bi
            var WorkersLimitNeg = FillWorkers ? (Fraction.MinusOne * b) : Vector.Empty;

            // x <= 1
            var xBelowOne = Enumerable.Repeat(Fraction.One, N * M);
            
            return 
                JobsOncePos
                .Concat(JobsOnceNeg)
                .Concat(WorkersLimitPos)
                .Concat(WorkersLimitNeg)
                .Concat(xBelowOne);
        }

        private string GetXRepresentation() =>
            "( " + string.Join(", ", c.M.Apply((x, i, j) => $"x{i + 1}{j + 1}").Flatten()) + " )";

        private string GetTargetFunctionRepresentation() =>
            string.Join(" + ",
                c.M.Apply((cij, i, j) => $"{Function.Print(cij)} * x{i + 1}{j + 1}").Flatten());

        /// <summary>
        /// x can be fractionary
        /// 0 <= x <= 1
        /// </summary>
        /// <param name="Writer"></param>
        /// <returns>The matrix of x, if it can be found</returns>
        public async Task<Matrix?> SolveCooperative(bool FillWorkers = false, IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync("Calculating representation of X...");
            await Writer.WriteLineAsync($"x = {GetXRepresentation()}");

            await Writer.WriteLineAsync($"Finding min of {GetTargetFunctionRepresentation()}");

            await Writer.WriteLineAsync($"A * x <= b");
            Simplex s = ConvertToLinearProgramming(FillWorkers);
            //await Writer.WriteLineAsync($"A|b = {s.A | B}");

            await Writer.WriteLineAsync("Solving with simplex...");
            var x = await s.SolvePrimalMax(// target function is already inverted in order to find min instead of max
                Writer: IndentWriter.Null,
                maxIterations: 50);

            if (x is null)
            {
                await Writer.WriteLineAsync("Problem could not be solved");
                return null;
            }
            return new Matrix(x.Get.Split(N));
        }
        
        public async Task<bool> SolveCooperativeFlow(bool FillWorkers = false, IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                Matrix? x = await SolveCooperative(FillWorkers, Writer);
                if (x is null)
                {
                    return false;
                }
                await Writer.WriteLineAsync($"Solution X = {x}");
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
        
        
        public async Task<Matrix?> SolveNonCooperative(IndentWriter? Writer = null)
        {
            // https://developers.google.com/optimization/assignment/assignment_cp?hl=it#c
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync("Solving via Google.OrTools library");

            CpModel model = new CpModel();
            BoolVar[,] x = new BoolVar[Jobs.Count(), Workers.Count()];
            await Writer.WriteLineAsync("Initializing variables...");
            foreach (int worker in Workers)
            {
                foreach (int task in Jobs)
                {
                    x[task, worker] = model.NewBoolVar($"x[{task},{worker}]");
                }
            }
            await Writer.Indent.WriteLineAsync("Calculating representation of X...");
            await Writer.Indent.WriteLineAsync($"x = {GetXRepresentation()}");

            await Writer.WriteLineAsync("Setting constarints...");
            // Each worker is assigned to at most max task size.
            foreach (int worker in Workers)
            {
                BoolVar[] vars = new BoolVar[Jobs.Count()];
                foreach (int task in Jobs)
                {
                    vars[task] = x[task, worker];
                }
                // Each worker has limited time
                model.Add(LinearExpr.WeightedSum(vars, w[worker].ToInt()) <= b[worker].ToInt32());
            }

            // Each task is assigned to exactly one worker.
            foreach (int task in Jobs)
            {
                List<ILiteral> workers = new List<ILiteral>();
                foreach (int worker in Workers)
                {
                    workers.Add(x[task, worker]);
                }
                model.AddExactlyOne(workers); // A single task cannot be done more than once
            }

            await Writer.WriteLineAsync("Setting costs...");
            LinearExprBuilder obj = LinearExpr.NewBuilder();
            foreach (int worker in Workers)
            {
                foreach (int task in Jobs)
                {
                    obj.AddTerm(x[task, worker], c[task, worker].ToInt32()); // Add costs of each task
                }
            }
            model.Minimize(obj);


            // Solve the problem
            await Writer.WriteLineAsync("Solving now...");
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);
            if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible)
            {
                await Writer.WriteLineAsync("CpSolver could not solve the problem.");
                return null;
            }


            Matrix ret = new Matrix();
            await Writer.WriteLineAsync("Building solution...");
            foreach (int worker in Workers)
            {
                foreach (int task in Jobs)
                {
                    if (solver.Value(x[task, worker]) > 0.5)
                    {
                        ret[task, worker] = Fraction.One;
                    }
                }
            }
            return ret;
        }
        public async Task<bool> SolveNonCooperativeFlow(IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            try
            {
                Matrix? x = await SolveNonCooperative(Writer: Writer);
                if (x is null)
                {
                    return false;
                }
                await Writer.WriteLineAsync($"Solution X = {x}");
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
