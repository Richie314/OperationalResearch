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
        IEnumerable<BoundedCostEdge> startBase, 
        IEnumerable<BoundedCostEdge> startFilled, 
        bool UseBounds = true) : 
        GraphSolver<MinimumCostFlow<BoundedCostEdge>, BoundedCostEdge>
    {
        private bool useBounds = UseBounds;
        private IEnumerable<BoundedCostEdge> T = startBase;
        private IEnumerable<BoundedCostEdge> U = startFilled;
        public override Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only minimum problem can be solved");
        }
        public override Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (graph is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return useBounds ? 
                graph.FlowBounded(T, U, Writer: loggers.FirstOrDefault()) : 
                graph.FlowUnbounded(T, Writer: loggers.FirstOrDefault());
        }
    }
}
