using OperationalResearch.Extensions;
using OperationalResearch.Models.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class TspSolver : GraphSolver<TSP<CostEdge>, CostEdge>
    {
        public override async Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (graph is null)
            {
                throw new InvalidOperationException();
            }
            return await graph.HamiltonCycleFlow(loggers.FirstOrDefault(), startingPoint);
        }
        public override Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only minimum problems can be solved");
        }
    }
}
