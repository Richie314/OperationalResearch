using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using OperationalResearch.Models;
using OperationalResearch.Models.Graphs;

namespace OperationalResearch.Extensions
{
    public static class EnumerablesExtensions
    {
        public static IEnumerable<int> VisitedNodes<EdgeType>(this IEnumerable<EdgeType> edges) where EdgeType : Edge
        {
            ArgumentNullException.ThrowIfNull(edges);
            if (!edges.Any())
            {
                return Enumerable.Empty<int>();
            }

            int currNode = edges.First().From;
            int nextNode = edges.First().To;

            if (edges.Count() == 1)
            {
                return new List<int>() { currNode, nextNode };
            }

            EdgeType? nextEdge = null;

            foreach (var edge in edges)
            {
                if (edge.From == nextNode)
                {
                    nextEdge = edge;
                    break;
                }
            }

            if (nextEdge is null)
            {
                // There is no next edge, it ends with nextNode
                return new List<int>() { currNode, nextNode };
            }

            var nextArr = new List<EdgeType>() { nextEdge };
            bool ExcludedEdge = false; // exclude nextEdge only once
            foreach (var edge in edges)
            {
                if (edge != nextEdge)
                {
                    nextArr.Add(edge);
                    continue;
                }
                if (ExcludedEdge)
                {
                    nextArr.Add(edge);
                }
                ExcludedEdge = true;
            }
            // nextNode will be the first element of the sub problem
            return new int[] { currNode }.Concat(nextArr.VisitedNodes());
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

        public static IEnumerable<EdgeType> From<EdgeType>(
            this IEnumerable<EdgeType> edges, int from) where EdgeType : Edge =>
            edges.Where(e => e.From == from);

        public static IEnumerable<int> NodesReached<EdgeType>(
            this IEnumerable<EdgeType> edges) where EdgeType : Edge =>
            edges.Select(e => e.To).Distinct();

        public static IEnumerable<IEnumerable<T>> AllPermutations<T>(this IEnumerable<T> vector)
        {
            ArgumentNullException.ThrowIfNull(vector, nameof(vector));
            if (!vector.Any())
            {
                return Enumerable.Empty<IEnumerable<T>>();
            }
            if (vector.Count() == 1)
            {
                return new List<IEnumerable<T>>() { vector };
            }
            if (vector.Count() == 2)
            {
                return new List<IEnumerable<T>>() { vector, vector.Reverse() };
            }

            List<IEnumerable<T>> startList = new ();
            return PermuteEnumerable(vector.ToArray(), 0, vector.Count() - 1, startList);
        }
        private static IList<IEnumerable<T>> PermuteEnumerable<T> (
            T[] vector, 
            int start, int end, 
            IList<IEnumerable<T>> list)
        {
            if (start == end)
            {
                list.Add(vector.Copy());
                return list;
            }
            for (var i = start; i <= end; i++)
            {
                vector.Swap(start, i);
                PermuteEnumerable(vector, start + 1, end, list);
                vector.Swap(start, i);
            }
            return list;
        }
    }
}
