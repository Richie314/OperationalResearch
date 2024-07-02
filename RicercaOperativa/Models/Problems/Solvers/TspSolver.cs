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
    public class TspSolver(bool symmetric = false) : LinearSolverInteger
    {
        private TSP? Tsp = null;
        private readonly bool Symmetric = symmetric;
        public override Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only lowest cost cycle can be found!");
        }
        public override async Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (Tsp is null)
            {
                throw new InvalidOperationException();
            }
            return await Tsp.HamiltonCycleFlow(loggers.FirstOrDefault(), Symmetric);
        }
        public override void SetData(Polyhedron domain, Vector unused)
        {
            Tsp = new TSP(Graph.FromMatrix(domain.A), Symmetric);
        }
    }
}
