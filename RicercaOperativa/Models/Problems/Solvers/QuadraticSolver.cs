using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.NonLinearOptimization.QuadProg;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class QuadraticSolver : ISolving<Polyhedron, Tuple<Matrix, Vector, Vector?>>
    {
        private QuadProg? quadProg = null;

        public Polyhedron? Domain { get; set; } = null;
        public Tuple<Matrix, Vector, Vector?>? CoDomain { get; set; } = null;

        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (quadProg is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await quadProg.SolveFlow(
                pointOfInterest: CoDomain?.Item3, 
                Writer: loggers.FirstOrDefault(), 
                max: true);
        }
        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (quadProg is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await quadProg.SolveFlow(
                pointOfInterest: CoDomain?.Item3,
                Writer: loggers.FirstOrDefault(),
                max: false);
        }

        public Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Can't solve for integers");
        }
        public Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Can't solve for integers");
        }

        public void SetData(Polyhedron p, Tuple<Matrix, Vector, Vector?> codomain)
        {
            Domain = p;
            CoDomain = codomain;
            quadProg = new QuadProg(codomain.Item1, codomain.Item2, p);
        }
    }
}
