using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class SimpleMinimumCostAssignSolver : ISolving<Matrix, bool>
    {
        private MinimumCostAssign? solver = null;
        private bool fillWorkers = false;

        public Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }
        public Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }


        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (solver is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await solver.SolveCooperativeFlow(fillWorkers, loggers.FirstOrDefault());
        }
        public async Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (solver is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await solver.SolveNonCooperativeFlow(loggers.FirstOrDefault());  
        }
        public void SetData(Matrix costs, bool fillWorkers)
        {
            solver = new MinimumCostAssign(costs);
            this.fillWorkers = fillWorkers;
        }
    }
    public class GeneralizedMinimumCostAssignSolver : ISolving<Tuple<Matrix, Matrix, Vector>, bool>
    {
        private MinimumCostAssign? solver = null;
        private bool fillWorkers = false;

        public Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }
        public Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }


        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (solver is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await solver.SolveCooperativeFlow(fillWorkers, loggers.FirstOrDefault());
        }
        public async Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (solver is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await solver.SolveNonCooperativeFlow(loggers.FirstOrDefault());
        }
        public void SetData(Tuple<Matrix, Matrix, Vector> domain, bool fillWorkers)
        {
            solver = new MinimumCostAssign(domain.Item1, domain.Item2, domain.Item3);
            this.fillWorkers = fillWorkers;
        }
    }
}
