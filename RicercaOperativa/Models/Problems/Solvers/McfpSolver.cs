using OperationalResearch.Extensions;
using OperationalResearch.Models.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class McfpSolver(
        IEnumerable<BoundedCostEdge>? startBase, 
        IEnumerable<BoundedCostEdge>? startFilled, 
        bool UseBounds = true) : 
        GraphSolver<MinimumCostFlow<BoundedCostEdge>, BoundedCostEdge>
    {
        private readonly bool useBounds = UseBounds;
        private readonly IEnumerable<BoundedCostEdge>? T = startBase;
        private readonly IEnumerable<BoundedCostEdge>? U = startFilled;
        public override Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only minimum problem can be solved");
        }
        public override Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (Domain is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return useBounds ?
                Domain.FlowBounded(T, U, startNode: startNode, endNode: K, Writer: loggers.FirstOrDefault()) :
                Domain.FlowUnbounded(T, Writer: loggers.FirstOrDefault());
        }
    }
}
