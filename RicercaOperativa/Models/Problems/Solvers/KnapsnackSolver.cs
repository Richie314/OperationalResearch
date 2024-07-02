using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class KnapsnackProblemSolver(bool isBoolean = false) : LinearSolverInteger
    {
        private readonly bool IsBoolean = isBoolean;
        private Knapsnack? Knapsnack = null;
        public override async Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (Knapsnack is null)
            {
                throw new InvalidOperationException("Parameters not yet initialized");
            }
            return await Knapsnack.SolveFlow(loggers.FirstOrDefault(), IsBoolean);
        }
        public override Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only max problems can be solved!");
        }
        public override void SetData(Polyhedron domain, Vector codomain)
        {
            Knapsnack = new(
                volume: domain.b[0],
                weight: domain.b[1],
                values: codomain,
                volumes: domain.A[0],
                weights: domain.A[0]);
        }
    }
}
