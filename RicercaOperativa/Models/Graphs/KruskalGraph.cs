﻿using OperationalResearch.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Graphs
{
    partial class CostGraph<EdgeType>
    {
        /// <summary>
        /// Static method
        /// Finds, if exists, the minimum spanning tree of the graph
        /// The number of nodes (N) is deduced by the edges
        /// </summary>
        /// <param name="edges">The edges of the graph</param>
        /// <param name="Writer">A StreamWriter to record all passages</param>
        /// <returns>The Spanning tree with lower cost, if exists</returns>
        public static async Task<IEnumerable<EdgeType>?> KruskalMinimumSpanningTree(
            IEnumerable<EdgeType> edges,
            bool symmetric,
            IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            Graph<EdgeType> graph = new Graph<EdgeType>(edges);
            graph.Edges = graph.Edges.OrderByCost();

            await Writer.WriteLineAsync($"Calculating Minimum Spanning Kruskal Tree...");
            await Writer.WriteLineAsync($"Edges = {Function.Print(graph.Edges)}");
            
            List<EdgeType> T = [];
            int h = 1;

            foreach (var e in edges)
            {
                await Writer.WriteLineAsync($"Iteration #{h++}");
                await Writer.WriteLineAsync($"T = {Function.Print(T)}");
                await Writer.WriteLineAsync($"Edge {e}:");
                List<EdgeType> T2 = new(T)
                {
                    e
                };
                var g2 = new Graph<EdgeType>(T2);
                if (g2.HasCycle(symmetric))
                {
                    await Writer.Indent().WriteLineAsync($"T U {{ {e} }} has a cycle -> edge is DISCARDED");
                }
                else
                {
                    T = T2;
                    await Writer.Indent().WriteLineAsync($"T U {{ {e} }} has no cycle -> edge is CHOSEN");
                }

                await Writer.WriteLineAsync();
                if (T.Count() == graph.N - 1)
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
        public async Task<IEnumerable<EdgeType>?> FindKTree(
            int k, bool symmetric, IndentWriter? Writer = null)
        {
            if (k < 0 || k >= N)
            {
                throw new ArgumentOutOfRangeException(nameof(k));
            }
            Writer ??= IndentWriter.Null;

            await Writer.WriteLineAsync($"Calculating {(symmetric ? "bidirectional" : "unidirectional")} {k + 1}-tree");

            IEnumerable<EdgeType> KEdges = Edges.Where(edge => edge.From == k || edge.To == k).ToList();
            await Writer.WriteLineAsync($"{k + 1}_edges: {Function.Print(KEdges)}");

            IEnumerable<EdgeType> NonKEdges = Edges.Where(edge => edge.From != k && edge.To != k).ToList();
            await Writer.WriteLineAsync($"non_{k + 1}_edges: {Function.Print(NonKEdges)}");

            await Writer.WriteLineAsync($"Finding minimum spanning tree through Kruskal...");
            IEnumerable<EdgeType>? Tree = await KruskalMinimumSpanningTree(NonKEdges, symmetric, Writer);

            if (Tree is null)
            {
                await Writer.WriteLineAsync($"Could not find the tree!");
                return null;
            }
            await Writer.WriteLineAsync($"Kruskal MSP = {Function.Print(Tree)}");

            // Concat k-edges to the found tree
            return Tree.Concat(KEdges.OrderByCost().Take(2));
        }
    }
}
