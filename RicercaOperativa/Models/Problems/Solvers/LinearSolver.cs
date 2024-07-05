using Google.OrTools.ConstraintSolver;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class LinearSolver(int[]? startBasis = null) : ISolving<Polyhedron, Vector>
    {
        private Simplex? simplex = null;
        private GoogleIntegerOperationWrapper? googleSolver = null;

        public int[]? startBasis = startBasis;

        public Polyhedron? Domain { get; set; } = null;
        public Vector? CoDomain { get; set; } = null;
        
        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (simplex is null)
            {
                throw new InvalidOperationException("Informations about the problem not yet loaded!");
            }
            return await simplex.SolvePrimalMaxFlow(
                startBasis: startBasis, Writer: loggers.FirstOrDefault());
        }
        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (simplex is null)
            {
                throw new InvalidOperationException("Informations about the problem not yet loaded!");
            }
            return await simplex.NegateTarget.SolvePrimalMaxFlow(
                startBasis: startBasis, Writer: loggers.FirstOrDefault());
        }

        public async Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (googleSolver is null)
            {
                throw new InvalidOperationException("Informations about the problem not yet loaded!");
            }
            return await googleSolver.SolveMaxFlow(loggers.FirstOrDefault());
        }
        public async Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (googleSolver is null)
            {
                throw new InvalidOperationException("Informations about the problem not yet loaded!");
            }
            return await googleSolver.SolveMinFlow(loggers.FirstOrDefault());
        }

        public void SetData(Polyhedron domain, Vector codomain)
        {
            Domain = domain;
            CoDomain = codomain;
            simplex = new Simplex(domain, codomain);
            googleSolver = new GoogleIntegerOperationWrapper(domain, codomain);
        }
    }
}
