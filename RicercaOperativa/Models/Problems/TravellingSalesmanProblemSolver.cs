using OperationalResearch.Models.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    internal class TravellingSalesmanProblemSolver(bool symmetric = false) : IntegerLinearProgramming
    {
        private bool Symmetric = symmetric;
        public override Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            throw new NotImplementedException("Only lowest cost cycle can be found!");
        }
        public override async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            TSP tsp = (TSP)Graph.FromMatrix(new Matrix(c), makeSymmetric: Symmetric);
            return await tsp.HamiltonCycleFlow(loggers.FirstOrDefault());
        }
    }
}
