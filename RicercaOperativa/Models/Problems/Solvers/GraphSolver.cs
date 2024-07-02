using OperationalResearch.Extensions;
using OperationalResearch.Models.Graphs;

namespace OperationalResearch.Models.Problems.Solvers
{
    public abstract class GraphSolver<TGraph, TArc> : ISolving<TGraph, int?> 
        where TGraph : Graph<TArc>
        where TArc : Edge
    {
        protected TGraph? graph = null;
        protected int? startingPoint = null;
        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers) =>
            await SolveIntegerMaxAsync(loggers);

        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers) =>
            await SolveIntegerMinAsync(loggers);


        public abstract Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers);
        public abstract Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers);

        public void SetData(TGraph domain, int? startPoint)
        {
            graph = domain;
            startingPoint = startPoint;
        }
    }
}
