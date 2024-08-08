using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.NonLinearOptimization;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class NonLinearSolver(Vector? startingPoint = null) : ISolving<Polyhedron, string>
    {
        private PythonProjectedGradientDescent? projectedGradient = null;
        private PythonFrankWolfe? frankWolfe = null;
        private Vector? startingPoint = startingPoint;
        public Polyhedron? Domain { get; set; } = null;
        public string? CoDomain { get; set; } = null;
        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> writers)
        {
            if (projectedGradient is null || frankWolfe is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            var results = await Task.WhenAll(
                new[] {
                    projectedGradient.SolveFlow(min: true, Writer: writers.FirstOrDefault(), startX: startingPoint),
                    frankWolfe.SolveFlow(min: true, Writer: writers.ElementAtOrDefault(1), startX: startingPoint)
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
                    projectedGradient.SolveFlow(min: false, Writer: writers.FirstOrDefault(), startX: startingPoint),
                    frankWolfe.SolveFlow(min: false, Writer: writers.ElementAtOrDefault(1), startX: startingPoint)
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
            Domain = p;
            CoDomain = pythonString;
            projectedGradient = new PythonProjectedGradientDescent(p, pythonString);
            frankWolfe = new PythonFrankWolfe(p, pythonString);
        }
    }
}
