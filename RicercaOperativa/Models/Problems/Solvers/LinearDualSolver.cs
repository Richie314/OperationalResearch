using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class LinearDualSolver(int[]? startBasis = null) : ISolving<Polyhedron, Vector>
    {
        private Simplex? simplex = null;

        public int[]? startBasis = startBasis;
        public Polyhedron? Domain { get; set; } = null;
        public Vector? CoDomain { get; set; } = null;

        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (simplex is null)
            {
                throw new InvalidOperationException("Informations about the problem not yet loaded!");
            }
            return await simplex.NegateKnownVector.SolveDualMinFlow(
                startBasis: startBasis, Writer: loggers.FirstOrDefault());
        }
        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (simplex is null)
            {
                throw new InvalidOperationException("Informations about the problem not yet loaded!");
            }
            return await simplex.SolveDualMinFlow(
                startBasis: startBasis, Writer: loggers.FirstOrDefault());
        }

        public Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only fractionary problems can be solved");
        }
        public Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only fractionary problems can be solved");
        }

        public void SetData(Polyhedron domain, Vector codomain)
        {
            simplex = new Simplex(domain, codomain);
        }
    }
}
