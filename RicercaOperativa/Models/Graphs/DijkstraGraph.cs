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

            int[] prev = Enumerable.Repeat(-1, N).ToArray();
            Vector pi = Enumerable.Repeat(Fraction.FromDouble(Double.PositiveInfinity), N).ToArray();
            int[] Q = [startNode];

            prev[startNode] = startNode;
            pi[startNode] = 0;

            await Writer.WriteLineAsync("Dijkstra starting...");
            await Writer.WriteLineAsync("Initial condition:");
            await Writer.WriteLineAsync($"starting node = {startNode + 1}");

            await Writer.WriteLineAsync($"prev = {Function.Print(prev)}");
            await Writer.WriteLineAsync($"pi = {pi}");
            await Writer.WriteLineAsync($"Q = {Function.Print(Q)}");
            await Writer.WriteLineAsync();

            int k = 1;
            while (Q.Any())
            {
                await Writer.WriteLineAsync($"Iteration {k}:");

                int u = Q[Q.Select(i => pi[i]).ToArray().ArgMin()];//ArgMin == 0 -> min is with first element of Q
                await Writer.WriteLineAsync($"u = {u + 1}, removed from Q");

                await Writer.WriteLineAsync($"prev[{u + 1}] = {prev[u] + 1}");
                await Writer.WriteLineAsync($"pi[{u + 1}] = {Function.Print(pi[u])}");

                // Remove u from Q
                Q = Q.Where(i => i != u).ToArray();

                bool[] updated = Enumerable.Repeat(false, N).ToArray();

                foreach (Edge edge in Edges)
                {
                    if (edge.From == u)
                    {
                        int v = edge.To;
                        if (pi[v] > pi[u] + edge.Cost)
                        {
                            // Violates Bellman's condition
                            await Writer.WriteLineAsync($"Edge {edge} violates Bellman's condition:");
                            await Writer.WriteLineAsync(
                                $"{Function.Print(pi[v])} > {Function.Print(pi[u])} + {Function.Print(edge.Cost)} = {Function.Print(pi[u] + edge.Cost)}");

                            pi[v] = pi[u] + edge.Cost;
                            pi[v] = u;
                            updated[v] = true;
                            Q = Q.Concatenate(v); Q.Sort();
                            await Writer.WriteLineAsync($"Q = Q U {{ {v + 1} }} = {Function.Print(Q)}");
                        }
                    }
                }

                await Writer.WriteLineAsync("Distances: [updated]");
                for (int i = 0; i < N; i++)
                {
                    await Writer.WriteAsync(updated[i] ? $"[{Function.Print(pi[i])}]" : $"{Function.Print(pi[i])}");
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

            return new DijkstraResult(prev, pi);
        }

    }
}
