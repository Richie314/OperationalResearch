using Accord.Math;
using Fractions;
using OperationalResearch.Extensions;
using Google.OrTools.ConstraintSolver;
using Matrix = OperationalResearch.Models.Elements.Matrix;
using Vector = OperationalResearch.Models.Elements.Vector;

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
        public async Task<IEnumerable<EdgeType>?> BestHamiltonCycle(
            IndentWriter Writer, 
            int? startNode, 
            int? k, 
            IEnumerable<EdgeType> bnb)
        {
            startNode ??= 0;
            k ??= startNode.Value;
            var kTree = await FindKTree(
                k.Value, Bidirectional, Writer.Indent);
    
            var nearestNode = await NearestNodeUpperEstimate(Writer.Indent, startNode);

            if (kTree is null || nearestNode is null)
            {
                await Writer.WriteLineAsync("Could not calculate the bounds. Failure");
                return null;
            }

            var lb = Cost(kTree);
            await Writer.WriteLineAsync($"{k.Value + 1}-tree ({Function.Print(lb)}): {Function.Print(kTree)}");
            var ub = Cost(nearestNode, Bidirectional);
            await Writer.WriteLineAsync($"Nearest node from {startNode.Value + 1} ({Function.Print(ub)}): {Function.Print(nearestNode)}");


            var bnbresult = await BranchAndBound(Writer, N, Edges, k.Value, ub, bnb, null, Bidirectional);
            return bnbresult.Item2;
        }

        public static async Task<Tuple<Fraction, IEnumerable<EdgeType>?>> BranchAndBound(
            IndentWriter Writer,
            int n, 
            IEnumerable<EdgeType> edges, 
            int k, 
            Fraction ub, 
            IEnumerable<EdgeType> bnb,
            IEnumerable<EdgeType>? forcedEdges = null,
            bool Bidirectional = true,
            int pRow = 1, int pSubRow = 0)
        {
            CostGraph<EdgeType> CurrentGraph = new(n, edges);

            IEnumerable<EdgeType>? kTree = await CurrentGraph.FindKTree(
                k, Bidirectional, 
                Writer: null, 
                requiredEdges: forcedEdges);
            if (kTree is null)
            {
                throw new DataMisalignedException();
            }
            var lb = Cost(kTree);

            await Writer.WriteLineAsync($"P({pSubRow},{pRow}) -> ({Function.Print(lb)}, {Function.Print(ub)})");
            await Writer.WriteLineAsync($"{k + 1}-tree: {Function.Print(kTree)}");

            if (lb > ub)
            {
                await Writer.WriteLineAsync($"Closed for Lower Bound > Upper Bound");
                return new Tuple<Fraction, IEnumerable<EdgeType>?>(lb, null);
            }

            // Check if k-tree is solution
            var e = Vector.Zeros(n);
            foreach (var edge in kTree)
            {
                e[edge.From] += Fraction.One;
                e[edge.To] += Fraction.One;
            }
            if (e == Vector.Repeat(2, n))
            {
                // k-tree is solution
                await Writer.WriteLineAsync($"{k + 1}-tree is admissible! => P({pSubRow},{pRow}): ({Function.Print(lb)}, {Function.Print(lb)})");
                return new Tuple<Fraction, IEnumerable<EdgeType>?>(lb, kTree);
            }

            if (lb == ub)
            {
                await Writer.WriteLineAsync($"Closed for Lower Bound == Upper Bound");
                return new Tuple<Fraction, IEnumerable<EdgeType>?>(lb, kTree);
            }

            if (!bnb.Any())
            {
                return new Tuple<Fraction, IEnumerable<EdgeType>?>(ub, null);
            }

            EdgeType currEdge = bnb.First();
            var w = Writer.Indent;

            Tuple<Fraction, IEnumerable<EdgeType>?> sol = new(ub, null);

            await w.WriteLineAsync();
            await w.WriteLineAsync($"x_{currEdge.From + 1},{currEdge.To + 1} = 0");
            // Do without the edge
            var removedUB = await BranchAndBound(
                w, 
                n, 
                edges.Where(e => e != currEdge), 
                k, 
                ub, 
                bnb.Skip(1),
                forcedEdges,
                Bidirectional,
                2 * pRow - 1, 
                pSubRow + 1);
            if (removedUB.Item1 < ub && removedUB.Item2 is not null)
            {
                sol = removedUB;
            }

            await w.WriteLineAsync();
            await w.WriteLineAsync($"x_{currEdge.From + 1},{currEdge.To + 1} = 1");
            // Force the edge
            var forcedUB = await BranchAndBound(
                w,
                n,
                edges,
                k, 
                sol.Item1,
                bnb.Skip(1),
                (forcedEdges ?? Enumerable.Empty<EdgeType>()).Append(currEdge), 
                Bidirectional,
                2 * pRow, 
                pSubRow + 1);
            if (forcedUB.Item1 < ub && forcedUB.Item2 is not null)
            {
                sol = forcedUB;
            }

            return sol;
        }

        public IEnumerable<EdgeType> BnBToEdges(string? BnB)
        {
            if (string.IsNullOrWhiteSpace(BnB))
            {
                return [];
            }
            return BnB.Split(',')
                    .Select(e => new Edge(e.Trim()))
                    .Select(e => FindEdge(e.From, e.To) ??
                        throw new DataMisalignedException($"Edge {e} was not found"));
        }

        public async Task<bool> SolveWithEuristichsFlow(
            IndentWriter Writer, 
            int? startNode = null,
            int? k = null,
            string? BnB = null)
        {
            try
            {
                var bnb = BnBToEdges(BnB);
                  

                var result = await BestHamiltonCycle(Writer, startNode, k, bnb);
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

        public MinimumCostAssign MinimumCostAssign
        { 
            get => new MinimumCostAssign(
                BuildMatrix(Bidirectional) + Matrix.Identity(N) * Fraction.PositiveInfinity // set cii 0 +inf to avoid self loops
            );
        }


        /// <summary>
        /// Solves as an assignment of minimum cost and applies the "patches" algortihm
        /// </summary>
        /// <returns>Best cycle, lb, ub (cycle cost)</returns>
        public async Task<
            Tuple<IEnumerable<EdgeType>, Fraction, Fraction>?> MinCostAssignUpperEstimate(
            IndentWriter? Writer)
        {
            Writer ??= IndentWriter.Null;

            await Writer.Bold.WriteLineAsync("Solving assignment of min cost");
            var mca = MinimumCostAssign;
            var x = await mca.SolveNonCooperative(Writer);

            if (x is null)
            {
                await Writer.Indent.Red.WriteLineAsync("Could not solve");
                return null;
            }

            var xVector = new Vector(x.M.Flatten()); 
            var lb = new Vector(x.M.Apply((x, i, j) => x * mca.C[i, j]).Flatten()).SumOfComponents();
            await Writer.WriteLineAsync($"x = {xVector} with cost {Function.Print(lb)}");


            await Writer.Bold.WriteLineAsync("Applying \"Patches\" Algorithm");

            // Detect cycles as bidirectional because we are searching in the graph of the solution
            var cycles = FromMatrix(x).FindAllCycles(false);
            if (cycles is null || !cycles.Any())
            {
                await Writer.Indent.Red.WriteLineAsync("No cycle was found");
                return null;
            }

            foreach (var C in cycles)
            {
                await Writer.Indent.WriteLineAsync($"Found cycle: {string.Join('-', C.Select(i => i + 1))}");
            }

            while (cycles.Count() > 1)
            {
                var C1 = cycles.First();
                var C2 = cycles.ElementAt(1);

                List<Tuple<Fraction, IEnumerable<int>>> WaysToMerge = [];

                foreach (int i in C1)
                {
                    // Add edges (i, j); (k, l)
                    // Remove edges (k, j); (i, l)
                    int iPos = C1.IndexOf(i);
                    var C1EndsWithI = C1
                        .Skip(iPos + 1).Concat(
                        C1.Take(iPos + 1));

                    int l = C1EndsWithI.First();
                    foreach (int j in C2)
                    {
                        int jPos = C2.IndexOf(j);
                        var C2StartsWithJ = C2.Skip(jPos).Concat(C2.Take(jPos));

                        int k = C2StartsWithJ.Last(); // Get predecessor

                        var newCycle = C1EndsWithI.Concat(C2StartsWithJ);

                        // Edges to add:

                        // (i, j)
                        var ij = FindEdge(i, j);
                        if (ij is null && Bidirectional)
                        {
                            ij = FindEdge(j, i);
                        }
                        if (ij is null)
                            continue;

                        // (k,l)
                        var kl = FindEdge(k, l);
                        if (kl is null && Bidirectional)
                        {
                            kl = FindEdge(l, k);
                        }
                        if (kl is null)
                            continue;


                        // Edges to remove:

                        // (k, j)
                        var kj = FindEdge(k, j);
                        if (kj is null && Bidirectional)
                        {
                            kj = FindEdge(j, k);
                        }
                        if (kj is null)
                            continue;

                        // (i,l)
                        var il = FindEdge(i, l);
                        if (il is null && Bidirectional)
                        {
                            il = FindEdge(l, i);
                        }
                        if (il is null)
                            continue;

                        var costVariation = ij.Cost + kl.Cost - kj.Cost - il.Cost;
                        WaysToMerge.Add(new Tuple<Fraction, IEnumerable<int>>(costVariation, newCycle));
                    }
                }

                if (!WaysToMerge.Any())
                {
                    return null;
                }

                var bestMerge = WaysToMerge.MinBy(t => t.Item1);
                if (bestMerge is null)
                {
                    // Should not happen
                    return null;
                }

                // Remove
                cycles = cycles.Skip(1).ToList(); // Remove the first element
                cycles[0] = bestMerge.Item2.ToList(); // Transform the second element in the new cycle
            }

            var cycleEdges = GetEdges(cycles.First(), true);
            if (cycleEdges is null || !cycleEdges.Any())
            {
                return null;
            }

            var ub = Cost(cycleEdges);

            return new Tuple<IEnumerable<EdgeType>, Fraction, Fraction>(cycleEdges, lb, ub);
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
                RoutingModel routing = new(manager);

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
                path.Add(startNode ?? 0);
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
