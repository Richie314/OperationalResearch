using Accord.Math;
using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    partial class Graph
    {
        public class DijkstraResult
        {
            public int[] P { get; }
            public Vector PI { get; }
            public DijkstraResult(IEnumerable<int> p, Vector pi)
            {
                ArgumentNullException.ThrowIfNull(p, nameof(p));
                ArgumentNullException.ThrowIfNull(pi, nameof(pi));
                if (p.Count() != pi.Size)
                {
                    throw new ArgumentException(
                        $"Cannot return from dijkstra because p and pi have different sizes ({p.Count()} != {pi.Size})");
                }
                P = p.ToArray();
                PI = pi;
            }
        }
        public async Task<DijkstraResult?> Dijkstra(
            StreamWriter? Writer = null, int startNode = 0, int? maxIterations = 20)
        {
            if (startNode < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startNode));
            }
            if (NonNegativeEdges.Count() < Edges.Count())
            {
                throw new InvalidDataException("There was an edge with cost below zero");
            }
            Writer ??= StreamWriter.Null;

            int[] p = Enumerable.Repeat(-2, N).ToArray();
            Vector π = Enumerable.Repeat(Fraction.FromDouble(int.MaxValue), N).ToArray();
            int[] Q = [startNode];

            p[startNode] = startNode;
            π[startNode] = -1;

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
                await Writer.WriteLineAsync($"u = {u + 1}, removed from Q");

                await Writer.WriteLineAsync($"prev[{u + 1}] = {p[u] + 1}");
                await Writer.WriteLineAsync($"pi[{u + 1}] = {Function.Print(π[u])}");

                // Remove u from Q
                Q = Q.Where(i => i != u).ToArray();

                bool[] updated = Enumerable.Repeat(false, N).ToArray();

                foreach (Edge edge in Edges)
                {
                    if (edge.From == u)
                    {
                        int v = edge.To;
                        if (π[v] > π[u] + edge.Cost)
                        {
                            // Violates Bellman's condition
                            await Writer.WriteLineAsync($"Edge {edge} violates Bellman's condition:");
                            await Writer.WriteLineAsync(
                                $"{Function.Print(π[v])} > {Function.Print(π[u])} + {Function.Print(edge.Cost)} = {Function.Print(π[u] + edge.Cost)}");

                            π[v] = π[u] + edge.Cost;
                            p[v] = u;
                            updated[v] = true;
                            Q = Q.Concatenate(v); Q.Sort();
                            await Writer.WriteLineAsync($"Q = Q U {{ {v + 1} }} = {Function.Print(Q)}");
                        }
                    }
                }

                await Writer.WriteLineAsync("Distances: [updated]");
                for (int i = 0; i < N; i++)
                {
                    await Writer.WriteAsync((updated[i] ? $"[{Function.Print(π[i])}]" : $"{Function.Print(π[i])}") + "  ");
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

            return new DijkstraResult(p, π);
        }

    }
}
