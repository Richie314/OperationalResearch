using Accord.Math;
using Fractions;
using OperationalResearch.Extensions;
using Google.OrTools.ConstraintSolver;

namespace OperationalResearch.Models.Graphs
{
    public class TSP<EdgeType> : CostGraph<EdgeType> where EdgeType : CostEdge
    {
        private bool Bidirectional = false;
        public TSP(int n, IEnumerable<EdgeType>? edges, bool makeSymmetric = false) : base(n, edges)
        {
            Bidirectional = makeSymmetric;
        }
        public TSP(IEnumerable<EdgeType>? edges, bool makeSymmetric = false) : base(edges)
        {
            Bidirectional = makeSymmetric;
        }
        private IEnumerable<EdgeType> AllEdges()
        {
            if (!Bidirectional)
                return Edges;
            List<EdgeType> edges = [.. Edges];
            foreach (var edge in Edges)
            {
                EdgeType clone = (EdgeType)edge.Clone();
                clone.From = edge.To;
                clone.To = edge.From;
                edges.Add(clone);
            }
            return edges;
        }

        public async Task<bool> HamiltonCycleFlow(
            IndentWriter? Writer = null, 
            int? startNode = null,
            int? k = null,
            string? BnB = null)
        {
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync("Finding best hamiltonian cycle");
            IEnumerable<bool> Results = [
                await SolveWithEuristichsFlow(Writer, startNode, k, BnB),
                await SolveWithOrToolsFlow(Writer, startNode),
            ];
            return Results.Any(x => x);
        }

        #region Euristichs
        public Task<IEnumerable<EdgeType>?> BestHamiltonCycle()
        {
            throw new NotImplementedException("Functionality not yet implemented");
        }
        public async Task<bool> SolveWithEuristichsFlow(
            IndentWriter Writer, 
            int? startNode = null,
            int? k = null,
            string? BnB = null)
        {
            try
            {
                var result = await BestHamiltonCycle();
                if (result is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved by euristichs");
                    return false;
                }
                await Writer.WriteLineAsync($"Cycle: {Function.Print(result)}");
                await Writer.WriteLineAsync($"Cost: {Function.Print(Cost(result))}");
                return true;
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }

        public async Task<IEnumerable<EdgeType>?> GreedyUpperEstimate(IndentWriter Writer)
        {
            var ArcsByCost = AllEdges().OrderByCost().ToList();
            await Writer.WriteLineAsync("Finding upper estimate by ordering arcs by cost");

            await Writer.WriteLineAsync(
                string.Join(", ", ArcsByCost.Select(arc => arc.ToString())));

            int startingArcIndex = 0;
            while (true)
            {
                if (startingArcIndex == ArcsByCost.Count)
                {
                    await Writer.WriteLineAsync($"No cycle could be found!");
                    return null;
                }
                var currArc = ArcsByCost.ElementAt(startingArcIndex);
                await Writer.WriteLineAsync($"Finding cycle starting by arc {currArc}");

                List<int> nodes = [currArc.From];
                int nextNode = currArc.To;
                while (!nodes.Contains(nextNode))
                {
                    nodes.Add(nextNode);
                    currArc = ArcsByCost.First(
                        a => a.From == nextNode && !nodes.Contains(a.To));
                    nextNode = currArc.To;
                }
                nodes.Add(currArc.To);

                if (nodes.Count == N + 1)
                {
                    await Writer.WriteLineAsync(
                        $"Cycle: {string.Join('-', nodes.Select(node => (node + 1).ToString()))}");
                    return GetEdges(nodes, bidirectional: Bidirectional);
                }
                await Writer.WriteLineAsync($"Cycle not found with current starting arc");

                startingArcIndex++;
            }
        }

        public async Task<IEnumerable<int>?> NearestNodeUpperEstimate(
            IndentWriter Writer, int? startingNode)
        {
            var NodesToStart = startingNode.HasValue ?
                new List<int>() { startingNode.Value } : Enumerable.Range(0, N);
            List<int>? bestCycle = null;
            Fraction bestCost = Fraction.Zero;
            var EdgeAndReversed = AllEdges();

            foreach (int start in NodesToStart)
            {
                await Writer.WriteLineAsync($"Starting from node {start + 1}:");
                List<int> nodes = [start];
                int currNode = start;
                while (nodes.Count < N)
                {
                    var possibleNodes = EdgeAndReversed
                        .From(currNode)
                        .NodesReached()
                        .Where(node => !nodes.Contains(node));

                    if (!possibleNodes.Any())
                    {
                        await Writer.WriteLineAsync(
                            $"No nodes to reach, cycle ends");
                        break;
                    }

                    await Writer.WriteLineAsync(
                        $"Possible nodes to reach: {Function.Print(possibleNodes)}");

                    var newNode = EdgeAndReversed
                        .From(currNode)
                        .Where(edge => !nodes.Contains(edge.To))
                        .OrderByCost().First().To;
                    await Writer.WriteLineAsync($"Chosen node {newNode + 1}");

                    currNode = newNode;

                    nodes.Add(currNode);
                }
                nodes.Add(start);
                await Writer.WriteLineAsync(
                        $"Cycle: {string.Join('-', nodes.Select(node => (node + 1).ToString()))}");

                Fraction currCost = Cost(nodes, bidirectional: true);
                await Writer.WriteLineAsync($"has cost {Function.Print(currCost)}");
                if (bestCycle is null)
                {
                    bestCycle = nodes;
                    bestCost = currCost;
                }
                if (currCost < bestCost)
                {
                    bestCost = currCost;
                    bestCycle = nodes;
                }
            }

            if (bestCycle is null || !bestCycle.Any())
            {
                await Writer.WriteLineAsync($"No cycle found!");
                return null;
            }

            await Writer.WriteLineAsync(
                $"Best cycle is: {string.Join('-', bestCycle.Select(node => (node + 1).ToString()))}");
            await Writer.WriteLineAsync($"Best cost is: {Function.Print(bestCost)}");

            return bestCycle;
        }
        #endregion

        #region Library
        /// <summary>
        /// Solve using Google OrTools
        /// </summary>
        /// <param name="startNode">The starting node</param>
        /// <returns></returns>
        public IEnumerable<int>? SolveWithOrTools(int? startNode = null)
        {
            try
            {
                // Define the problem
                var Matrix = BuildMatrix(Bidirectional);
                RoutingIndexManager manager = new(N, 1, startNode ?? 0);
                RoutingModel routing = new RoutingModel(manager);

                // Tell how to search distances (costs) in the graph
                int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
                {
                    int fromNode = manager.IndexToNode(fromIndex);
                    int toNode = manager.IndexToNode(toIndex);
                    return Matrix[fromNode, toNode].ToInt64();
                });
                routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);
                
                // Tell which strategy to use
                RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
                searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.Automatic;
                
                // Actually solve the problem
                Assignment solution = routing.SolveWithParameters(searchParameters);

                // Build the path from the solution
                List<int> path = [];
                for (long index = routing.Start(0); !routing.IsEnd(index); index = solution.Value(routing.NextVar(index)))
                {
                    path.Add(manager.IndexToNode((int)index));
                }
                return path;
            }
            catch {
#if DEBUG
                throw;
#else
                return null;
#endif
            }
        }

        public async Task<bool> SolveWithOrToolsFlow(IndentWriter Writer, int? startNode = null)
        {
            try
            {
                await Writer.WriteLineAsync("Solving with Google OrTools...");
                var result = SolveWithOrTools(startNode);
                if (result is null || !result.Any())
                {
                    await Writer.WriteLineAsync("Problem was not solved by the library");
                    return false;
                }
                await Writer.WriteLineAsync($"Found cycle: {Function.Print(result)}");
                await Writer.WriteLineAsync($"Cost: {Function.Print(Cost(result, Bidirectional))}");
                return true;
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }
        #endregion
    }
}
