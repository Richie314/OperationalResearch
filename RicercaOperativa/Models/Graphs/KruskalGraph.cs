using OperationalResearch.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    partial class Graph
    {
        /// <summary>
        /// Static method
        /// Finds, if exists, the minimum spanning tree of the graph
        /// The number of nodes (N) is deduced by the edges
        /// </summary>
        /// <param name="edges">The edges of the graph</param>
        /// <param name="Writer">A StreamWriter to record all passages</param>
        /// <returns>The Spanning tree with lower cost, if exists</returns>
        public static async Task<IEnumerable<Edge>?> KruskalMinimumSpanningTree(
            IEnumerable<Edge> edges,
            StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            int N = new Graph(edges).N;
            edges = edges.OrderByCost();

            await Writer.WriteLineAsync($"Calculating Minimum Spanning Kruskal Tree...");
            List<Edge> T = [];
            int h = 1;

            foreach (Edge e in edges)
            {
                await Writer.WriteLineAsync($"Iteration #{h++}");
                await Writer.WriteLineAsync($"T = {Function.Print(T)}");
                await Writer.WriteLineAsync($"Edge {e}:");
                if (e.Type == Edge.EdgeType.Disabled)
                {
                    await Writer.WriteLineAsync($"Edge is disabled: exit");
                    break;
                }
                List<Edge> T2 = T; T2.Add(e);
                if (T2.HasCycle())
                {
                    await Writer.WriteLineAsync($"T U {{ {e} }} has a cycle -> edge is DISCARDED");
                    if (e.Type == Edge.EdgeType.Required)
                    {
                        // Problem unsolvable
                        await Writer.WriteLineAsync(
                            $"Edge was not chosen despite being required. Problem has no solution");
                        return null;
                    }
                }
                else
                {
                    T = T2;
                    await Writer.WriteLineAsync($"T U {{ {e} }} has no cycle -> edge is CHOSEN");
                }

                await Writer.WriteLineAsync();
                if (T.Count() == N - 1)
                {
                    await Writer.WriteLineAsync($"|T| = {T.Count()} = N - 1");
                    break;
                }
            }

            return T;
        }

        /// <summary>
        /// Calculates the k-tree of the graph, uses the kruskal method to find most of the tree
        /// </summary>
        /// <param name="k">The node to isolate. Must be >= 0 and < N</param>
        /// <param name="Writer">The writer to recoed the progress of the calculation</param>
        /// <returns>The tree in the form of edges, if one is found, null otherwise</returns>
        /// <exception cref="ArgumentOutOfRangeException">if k is not in the range [0, N)</exception>
        public async Task<IEnumerable<Edge>?> FindKTree(int k, StreamWriter? Writer = null)
        {
            if (k < 0 || k >= N)
            {
                throw new ArgumentOutOfRangeException(nameof(k));
            }
            Writer ??= StreamWriter.Null;

            await Writer.WriteLineAsync($"Calculating {k + 1}-tree");

            IEnumerable<Edge> KEdges = Edges.Where(edge => edge.From == k || edge.To == k);
            await Writer.WriteLineAsync($"{k + 1}_edges: {Function.Print(KEdges)}");

            IEnumerable<Edge> NonKEdges = Edges.Where(edge => edge.From != k && edge.To != k);
            await Writer.WriteLineAsync($"non_{k + 1}_edges: {Function.Print(NonKEdges)}");

            await Writer.WriteLineAsync($"Finding minimum spanning tree through Kruskal...");
            IEnumerable<Edge>? Tree = await KruskalMinimumSpanningTree(NonKEdges, StreamWriter.Null);

            if (Tree is null)
            {
                await Writer.WriteLineAsync($"Could not find the tree!");
                return null;
            }

            // Concat k-edges to the found tree
            return Tree.Concat(KEdges);
        }
    }
}
