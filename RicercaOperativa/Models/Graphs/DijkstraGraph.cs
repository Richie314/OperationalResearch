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

            public int radix { get; }
            private Graph<EdgeType> OriginalGraph { get; }

            public DijkstraResult(
                IEnumerable<int> p, 
                Vector π, 
                int radix, 
                CostGraph<EdgeType> OriginalGraph
            ) {
                ArgumentNullException.ThrowIfNull(p, nameof(p));
                ArgumentNullException.ThrowIfNull(π, nameof(π));
                ArgumentNullException.ThrowIfNull(OriginalGraph, nameof(OriginalGraph));
                if (p.Count() != π.Size)
                {
                    throw new ArgumentException(
                        $"Cannot return from dijkstra because p and π have different sizes ({p.Count()} != {π.Size})");
                }
                this.p = p.ToArray();
                this.π = π;
                this.radix = radix;
                this.OriginalGraph = OriginalGraph;
            }
            private Tuple<CostGraph<EdgeType>, Vector> GetGraph()
            {
                List<EdgeType> edges = [];
                for (int dest = p.Length - 1; dest >= 0; dest--)
                {
                    if (p[dest] < 0 || p[dest] == dest) 
                        continue;
                    var edge = OriginalGraph[from: p[dest], to: dest];
                    if (edge is null)
                    {
                        throw new DataMisalignedException(
                            $"Edge {new Edge(p[dest], dest)} not found in the original graph");
                    }
                    edges.Add(edge);
                }

                Vector x = Vector.Zeros(OriginalGraph.Edges.Count());
                for (int node  = 0; node < p.Length; node++)
                {
                    for (int curr = node; curr != radix && p[curr] >= 0; curr = p[curr])
                    {
                        int xIndex = OriginalGraph.GetEdgeIndex(from: p[curr], to: curr);
                        x[xIndex] = x[xIndex] + Fraction.One;
                    }
                }

                return new Tuple<CostGraph<EdgeType>, Vector>(new CostGraph<EdgeType>(edges), x);
            }

            public Tuple<CostGraph<EdgeType>, Vector> Get { get => GetGraph(); }
        }
        public async Task<DijkstraResult?> Dijkstra(
            IndentWriter? Writer = null, int startNode = 0, int? maxIterations = 20)
        {
            const int NO_PREDECESSOR = -2;
            if (startNode < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startNode));
            }
            if (NonNegativeEdges.Count() < Edges.Count())
            {
                throw new InvalidDataException("There was an edge with cost below zero");
            }
            Writer ??= IndentWriter.Null;

            int[] p = Enumerable.Repeat(NO_PREDECESSOR, N).ToArray();
            Vector π = Enumerable.Repeat(Fraction.PositiveInfinity, N).ToArray();
            int[] Q = [startNode];

            p[startNode] = startNode;
            π[startNode] = Fraction.Zero;

            await Writer.WriteLineAsync($"Dijkstra from node = {startNode + 1}");
            await Writer.Indent.WriteLineAsync($"p = {Function.Print(p)}");
            await Writer.Indent.WriteLineAsync($"π = {π}");
            await Writer.Indent.WriteLineAsync($"Q = {Function.Print(Q)}");
            await Writer.WriteLineAsync();

            int k = 1;
            while (Q.Any())
            {
                await Writer.Bold.WriteLineAsync($"Iteration #{k} of Dijkstra:");
                
                int u = Q[Q.Select(i => π[i]).ToArray().ArgMin()];//ArgMin == 0 -> min is with first element of Q
                await Writer.Indent.WriteLineAsync($"u = {u + 1}, removed from Q");

                // Remove u from Q
                Q = Q.Where(i => i != u).ToArray();

                foreach (var edge in Edges)
                {
                    if (edge.From == u)
                    {
                        int v = edge.To;
                        if (π[v] > π[u] + edge.Cost)
                        {
                            // Violates Bellman's condition
                            await Writer.Indent.WriteLineAsync(
                                $"Edge {edge} violates Bellman's condition:");
                            await Writer.Indent.Indent.WriteLineAsync(
                                $"{Function.Print(π[v])} > {Function.Print(π[u])} + {Function.Print(edge.Cost)} = {Function.Print(π[u] + edge.Cost)}");

                            π[v] = π[u] + edge.Cost;
                            p[v] = u;

                            await Writer.Indent.Indent.WriteLineAsync($"Set π[{v + 1}] = {Function.Print(π[v])}");
                            await Writer.Indent.Indent.WriteLineAsync($"Set p[{v + 1}] = {p[v] + 1}");
                            Q = Q.Concatenate(v); Q.Sort();
                            await Writer.Indent.WriteLineAsync($"Q = Q U {{ {v + 1} }} = {Function.Print(Q)}");
                        }
                    }
                }


                k++;
                if (maxIterations.HasValue && k >= maxIterations.Value)
                {
                    await Writer.Red.WriteLineAsync(
                        $"Maxinum number of iterations ({maxIterations.Value}) exceeded. Could not solve the problem");
                    return null;
                }

                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
            }

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == i)
                {
                    await Writer.Indent.Orange.WriteLineAsync(
                        $"It appears that predecessor  that the predecessor of node {i + 1} is the node {i + 1} itself!");
                }
            }

            return new DijkstraResult(p, π, startNode, this);
        }

    }
}
