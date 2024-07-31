using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;

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
        public override void SetData(Elements.Polyhedron domain, Vector codomain)
        {
            Domain = domain;
            CoDomain = codomain;
            Knapsnack = new(
                values: codomain,
                volumes: domain.A[0],
                volume: domain.b[0],
                weight: domain.b.Size > 1 ? domain.b[1] : Fraction.One,
                weights: domain.A.Rows > 1 ? domain.A[1] : Vector.Zeros(domain.A.Cols));
        }
    }
}
