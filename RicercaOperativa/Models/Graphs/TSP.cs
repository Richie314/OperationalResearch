using Accord.Math;
using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Graphs
{
    internal class TSP : Graph
    {
        public TSP(int n, IEnumerable<Edge>? edges, bool makeSymmetric = false) : base(n, edges)
        {
            c = BuildMatrix(makeSymmetric);
        }
        private Matrix c;
        public async Task<bool> HamiltonCycleFlow(StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            await Writer.WriteLineAsync("Finding best hamiltonian cycle");
            bool FoundCycle = false;
            try
            {
                var result = await BestHamiltonCycle();
                if (result is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved by euristichs");
                } else {
                    await Writer.WriteLineAsync($"Cycle: {Function.Print(result)}");
                    await Writer.WriteLineAsync($"Cost: {Function.Print(Cost(result))}");
                    FoundCycle = true;
                }
            } catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
            }

            try
            {
                await Writer.WriteLineAsync("Brute forcing cycle...");
                var result = BruteForceHamiltonCycle();
                if (result is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved by brute force");
                }
                else
                {
                    await Writer.WriteLineAsync($"Brute forced cycle: {Function.Print(result)}");
                    await Writer.WriteLineAsync($"Cost: {Function.Print(Cost(result))}");
                    FoundCycle = true;
                }
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
            }
            return FoundCycle;
        }
        public async Task<IEnumerable<Edge>?> BestHamiltonCycle()
        {
            throw new NotImplementedException();
        }
        private IEnumerable<Edge> AllEdges()
        {
            return c.M
                .Apply((cost, row, col) => new Edge(cost, row, col))
                .Reshape()
                .Where(arc => !arc.Cost.IsZero); // Remove first diagonal
        }

        public async Task<IEnumerable<Edge>?> GreedyUpperEstimate(StreamWriter Writer)
        {
            var ArcsByCost = AllEdges().OrderBy(arc => arc.Cost).ToList();
            await Writer.WriteLineAsync("Finding upper estimate by ordering arcs by cost");

            await Writer.WriteLineAsync(
                string.Join(", ", ArcsByCost.Select(arc => arc.ToString())));

            int startingArcIndex = 0;
            while (true)
            {
                if (startingArcIndex == ArcsByCost.Count())
                {
                    await Writer.WriteLineAsync($"No cycle could be found!");
                    return null;
                }
                Edge currArc = ArcsByCost.ElementAt(startingArcIndex);
                await Writer.WriteLineAsync($"Finding cycle starting by arc {currArc}");

                List<int> nodes = new List<int>() { currArc.From };
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
                    return
                        Enumerable.Range(0, nodes.Count)
                        .Select(i => new Edge(Fraction.One, nodes[i % nodes.Count], nodes[(i + 1) % nodes.Count]));
                }
                await Writer.WriteLineAsync($"Cycle not found with current starting arc");

                startingArcIndex++;
            }
        }

        public async Task<IEnumerable<Edge>?> NearestNodeUpperEstimate(
            StreamWriter Writer, int? startingNode)
        {
            var NodesToStart = startingNode.HasValue ?
                new List<int>() { startingNode.Value } : Enumerable.Range(0, N);
            List<int>? bestCycle = null;
            Fraction bestCost = Fraction.Zero;

            foreach (int start in NodesToStart)
            {
                await Writer.WriteLineAsync($"Starting from node {start + 1}:");
                List<int> nodes = new List<int>() { start };
                int currNode = start;
                while (nodes.Count < N)
                {
                    var possibleNodes = c[currNode]
                        .NonZeroIndeces
                        .Where(i => !nodes.Contains(i));

                    if (!possibleNodes.Any())
                    {
                        await Writer.WriteLineAsync(
                            $"No nodes to reach, cycle ends");
                        break;
                    }

                    await Writer.WriteLineAsync(
                        $"Possible nodes to reach: {Function.Print(possibleNodes)}");
                    var bestIndex = possibleNodes
                        .Select(i => c[currNode, i])
                        .ToArray().ArgMin();
                    currNode = possibleNodes.ElementAt(bestIndex);

                    await Writer.WriteLineAsync($"Chosen node {currNode + 1}");

                    nodes.Add(currNode);
                }
                nodes.Add(start);
                await Writer.WriteLineAsync(
                        $"Cycle: {string.Join('-', nodes.Select(node => (node + 1).ToString()))}");

                Fraction currCost = Cost(nodes);
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

            if (bestCycle is null)
            {
                await Writer.WriteLineAsync($"No cycle found!");
                return null;
            }

            await Writer.WriteLineAsync(
                $"Best cycle is: {string.Join('-', bestCycle.Select(node => (node + 1).ToString()))}");
            await Writer.WriteLineAsync($"Best cost is: {Function.Print(bestCost)}");

            return Enumerable.Range(0, bestCycle.Count)
                .Select(i => new Edge(Fraction.One,
                    bestCycle[i], bestCycle[(i + 1) % bestCycle.Count]));
        }




    }
}
