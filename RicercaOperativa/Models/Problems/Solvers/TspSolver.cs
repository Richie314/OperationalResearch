using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
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
            if (Domain is null)
            {
                throw new InvalidOperationException();
            }
            return await Domain.HamiltonCycleFlow(loggers.FirstOrDefault(), startNode, K, BnBInstantiate);
        }
        public override Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only minimum problems can be solved");
        }
    }
}
