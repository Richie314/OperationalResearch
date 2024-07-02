using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.Python;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class NonLinearSolver(Vector? startingPoint = null) : ISolving<Polyhedron, string>
    {
        private ProjectedGradient? projectedGradient = null;
        private FrankWolfe? frankWolfe = null;
        private Vector? startingPoint = startingPoint;
        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> writers)
        {
            if (projectedGradient is null || frankWolfe is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            var results = await Task.WhenAll(
                new[] {
                    projectedGradient.SolveMinFlow(writers.FirstOrDefault(), startingPoint),
                    frankWolfe.SolveMinFlow(writers.ElementAtOrDefault(1), startingPoint)
                });
            return results.Any(x => x); // At least one succeded
        }
        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> writers)
        {
            if (projectedGradient is null || frankWolfe is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            var results = await Task.WhenAll(
                new[] {
                    projectedGradient.SolveMaxFlow(writers.FirstOrDefault(), startingPoint),
                    frankWolfe.SolveMaxFlow(writers.ElementAtOrDefault(1), startingPoint)
                });
            return results.Any(x => x); // At least one succeded
        }

        public Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> writers)
        {
            throw new NotImplementedException("Can't solve for integers");
        }
        public Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> writers)
        {
            throw new NotImplementedException("Can't solve for integers");
        }

        public void SetData(Polyhedron p, string pythonString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pythonString);
            projectedGradient = new ProjectedGradient(p, pythonString);
            frankWolfe = new FrankWolfe(p, pythonString);
        }
    }
}
