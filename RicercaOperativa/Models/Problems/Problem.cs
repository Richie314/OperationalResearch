using OperationalResearch.Extensions;
using OperationalResearch.Models.Problems.Solvers;

namespace OperationalResearch.Models.Problems
{
    public class Problem<Domain, CoDomain, SolverClass> where SolverClass : ISolving<Domain, CoDomain>
    {
        public readonly SolverClass Solver;
        public Problem(Domain domain, CoDomain coDomain, SolverClass solver)
        {
            Solver = solver;
            Solver.SetData(domain, coDomain);
        }
        
        public async Task<bool> SolveMin(IEnumerable<IndentWriter?> loggers) =>
            await Task.Run(() => Solver.SolveMinAsync(loggers));
        public async Task<bool> SolveMax(IEnumerable<IndentWriter?> loggers) => 
            await Task.Run(() => Solver.SolveMaxAsync(loggers));

        public async Task<bool> SolveIntegerMin(IEnumerable<IndentWriter?> loggers) =>
            await Task.Run(() => Solver.SolveIntegerMinAsync(loggers));
        public async Task<bool> SolveIntegerMax(IEnumerable<IndentWriter?> loggers) =>
            await Task.Run(() => Solver.SolveIntegerMaxAsync(loggers));
    }
}
