using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Fractions;
using OperationalResearch.Models.Graphs;

namespace OperationalResearch.Extensions
{
    public static class EnumerablesExtensions
    {
        public static IEnumerable<int> VisitedNodes<EdgeType>(
            this IEnumerable<EdgeType> edges, 
            int? startNode = null, bool bidirectionalEdges = false) where EdgeType : Edge
        {
            if (!edges.Any())
            {
                if (startNode.HasValue)
                {
                    return [startNode.Value];
                }
                return [];
            }
            if (!startNode.HasValue)
            {
                return edges.VisitedNodes(edges.First().From, bidirectionalEdges);
            }

            // Find the first edge we can move from
            EdgeType? fromEdge = edges.FirstOrDefault(e => e?.From == startNode.Value, null);
            if (fromEdge is not null)
            {
                return new int[] { fromEdge.From }.Concat(
                    edges.Where(e => e != fromEdge).VisitedNodes(fromEdge.To, bidirectionalEdges));
            }
            if (!bidirectionalEdges)
            {
                // Exit -> we have ended the path
                return [startNode.Value];
            }

            // We couldn't move forward from an edge, perhaps we can move towards this one
            EdgeType? toEdge = edges.FirstOrDefault(e => e?.To == startNode.Value, null);
            if (toEdge is null)
            {
                return [startNode.Value];
            }

            return new int[] { toEdge.To }.Concat(
                edges.Where(e => e != toEdge).VisitedNodes(toEdge.From, bidirectionalEdges));
        }
        public static Dictionary<int, int> MentionedNodes<EdgeType>(this IEnumerable<EdgeType> edges)
            where EdgeType : Edge
        {
            Dictionary<int, int> seen = new();
            foreach (var edge in edges)
            {
                if (!seen.ContainsKey(edge.From))
                {
                    seen.Add(edge.From, 0);
                }
                if (!seen.ContainsKey(edge.To))
                {
                    seen.Add(edge.To, 0);
                }
                seen[edge.From]++;
                seen[edge.To]++;
            }
            return seen;
        }
        
        public static IEnumerable<EdgeType> OrderByCost<EdgeType>(
            this IEnumerable<EdgeType> edges) where EdgeType : CostEdge =>
            edges.OrderBy(e => e.Cost);

        public static Fraction TotalCost<EdgeType>(
            this IEnumerable<EdgeType> edges) where EdgeType : CostEdge =>
            edges.Any() ? edges.First().Cost + edges.Skip(1).TotalCost() : Fraction.Zero;

        public static IEnumerable<EdgeType> From<EdgeType>(
            this IEnumerable<EdgeType> edges, int from) where EdgeType : Edge =>
            edges.Where(e => e.From == from);

        public static IEnumerable<int> NodesReached<EdgeType>(
            this IEnumerable<EdgeType> edges) where EdgeType : Edge =>
            edges.Select(e => e.To).Distinct();

        public static IEnumerable<EdgeType> TouchNode<EdgeType>(this IEnumerable<EdgeType> edges, int node) 
            where EdgeType : Edge =>
            edges.Where(e => e.From == node || e.To == node);
        public static IEnumerable<EdgeType> DoNotTouchNode<EdgeType>(this IEnumerable<EdgeType> edges, int node)
            where EdgeType : Edge =>
            edges.Where(e => e.From != node && e.To != node);

        public static IEnumerable<IEnumerable<T>> AllPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new T[] { t });
            return list.AllPermutations(length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)), (t1, t2) => t1.Concat([t2]));
        }
        public static IEnumerable<IEnumerable<T>> OrderedPermutations<T>(this IEnumerable<T> list, int length) =>
            list.AllPermutations<T>(length).Select(row => row.Order()).Distinct();

        public static IEnumerable<int> ReorderPath(this IEnumerable<int> path, 
            int start, int recursionStart = 0)
        {
            if (path.First() == start) 
                return path;
            if (recursionStart >= path.Count())
            {
                throw new DataMisalignedException($"Node {start + 1} not present in {string.Join('-', path.Select(i => i + 1))}");
            }
            return path.Skip(1).Append(path.First()).ReorderPath(start, recursionStart + 1);
        }

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
