using Accord.Math;
using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using Vector = OperationalResearch.Models.Elements.Vector;

namespace OperationalResearch.Models.Graphs
{
    partial class CostGraph<EdgeType>
    {
        public class DijkstraResult
        {
            public int[] p { get; }
            public Vector π { get; }
            public DijkstraResult(IEnumerable<int> p, Vector π)
            {
                ArgumentNullException.ThrowIfNull(p, nameof(p));
                ArgumentNullException.ThrowIfNull(π, nameof(π));
                if (p.Count() != π.Size)
                {
                    throw new ArgumentException(
                        $"Cannot return from dijkstra because p and π have different sizes ({p.Count()} != {π.Size})");
                }
                this.p = p.ToArray();
                this.π = π;
            }
            public Graph<Edge> Graph()
            {
                List<Edge> edges = [];
                for (int dest = p.Length - 1; dest >= 0; dest--)
                {
                    if (p[dest] < 0) 
                        continue;
                    edges.Add(new Edge(from: p[dest], dest));
                }
                return new Graph<Edge>(edges);
            }
        }
        public async Task<DijkstraResult?> Dijkstra(
            IndentWriter? Writer = null, int startNode = 0, int? maxIterations = 20)
        {
            if (startNode < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startNode));
            }
            if (NonNegativeEdges.Count() < Edges.Count())
            {
                throw new InvalidDataException("There was an edge with cost below zero");
            }
            Writer ??= IndentWriter.Null;

            int[] p = Enumerable.Repeat(-2, N).ToArray();
            Vector π = Enumerable.Repeat(Fraction.PositiveInfinity, N).ToArray();
            int[] Q = [startNode];

            p[startNode] = startNode;
            π[startNode] = Fraction.MinusOne;

            await Writer.WriteLineAsync("Dijkstra starting...");
            await Writer.WriteLineAsync("Initial condition:");
            await Writer.WriteLineAsync($"starting node = {startNode + 1}");

            await Writer.WriteLineAsync($"p = {Function.Print(p)}");
            await Writer.WriteLineAsync($"π = {π}");
            await Writer.WriteLineAsync($"Q = {Function.Print(Q)}");
            await Writer.WriteLineAsync();

            int k = 1;
            while (Q.Any())
            {
                await Writer.WriteLineAsync($"Iteration {k}:");

                int u = Q[Q.Select(i => π[i]).ToArray().ArgMin()];//ArgMin == 0 -> min is with first element of Q
                await Writer.Indent.WriteLineAsync($"u = {u + 1}, removed from Q");

                await Writer.Indent.WriteLineAsync($"p[{u + 1}] = {p[u] + 1}");
                await Writer.Indent.WriteLineAsync($"π[{u + 1}] = {Function.Print(π[u])}");

                // Remove u from Q
                Q = Q.Where(i => i != u).ToArray();

                bool[] updated = Enumerable.Repeat(false, N).ToArray();

                foreach (var edge in Edges)
                {
                    if (edge.From == u)
                    {
                        int v = edge.To;
                        if (π[v] > π[u] + edge.Cost)
                        {
                            // Violates Bellman's condition
                            await Writer.Indent.WriteLineAsync($"Edge {edge} violates Bellman's condition:");
                            await Writer.Indent.Indent.WriteLineAsync(
                                $"{Function.Print(π[v])} > {Function.Print(π[u])} + {Function.Print(edge.Cost)} = {Function.Print(π[u] + edge.Cost)}");

                            π[v] = π[u] + edge.Cost;
                            p[v] = u;
                            updated[v] = true;
                            Q = Q.Concatenate(v); Q.Sort();
                            await Writer.Indent.WriteLineAsync($"Q = Q U {{ {v + 1} }} = {Function.Print(Q)}");
                        }
                    }
                }

                await Writer.Indent.WriteLineAsync("Distances: [updated]");
                for (int i = 0; i < N; i++)
                {
                    await Writer.Indent.WriteAsync((updated[i] ? $"[{Function.Print(π[i])}]" : $"{Function.Print(π[i])}") + "  ");
                }
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();


                k++;
                if (maxIterations.HasValue && k >= maxIterations.Value)
                {
                    await Writer.WriteLineAsync(
                        $"Maxinum number of iterations ({maxIterations.Value}) exceeded. Could not solve the problem");
                    return null;
                }
            }

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == i)
                {
                    await Writer.Indent.WriteLineAsync(
                        $"It appears that predecessor  that the predecessor of node {i + 1} is the node {i + 1} itself!");
                }
            }

            return new DijkstraResult(p, π);
        }

    }
}
