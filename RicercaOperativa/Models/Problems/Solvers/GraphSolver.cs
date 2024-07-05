using OperationalResearch.Extensions;
using OperationalResearch.Models.Graphs;

namespace OperationalResearch.Models.Problems.Solvers
{
    public abstract class GraphSolver<TGraph, TArc> : ISolving<TGraph, Tuple<int?, int?, string?>> 
        where TGraph : Graph<TArc>
        where TArc : Edge
    {
        public TGraph? Domain { get; set; } = null;
        public Tuple<int?, int?, string?>? CoDomain { get; set; } = null;

        // Short way to access
        public int? startNode { get => CoDomain?.Item1; }
        public int? K { get => CoDomain?.Item2; }
        public string? BnBInstantiate { get => CoDomain?.Item3; }


        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers) =>
            await SolveIntegerMaxAsync(loggers);

        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers) =>
            await SolveIntegerMinAsync(loggers);

        public abstract Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers);
        public abstract Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers);

        public void SetData(TGraph domain, Tuple<int?, int?, string?> startNodeKAndBnB)
        {
            Domain = domain;
            CoDomain = startNodeKAndBnB;
        }
    }
}
