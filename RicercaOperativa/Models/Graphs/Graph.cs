using Accord.Collections;
using Accord.Math;
using OperationalResearch.Extensions;
using System.ComponentModel.DataAnnotations;
using Matrix = OperationalResearch.Models.Elements.Matrix;

namespace OperationalResearch.Models.Graphs
{
    public partial class Graph<EdgeType> where EdgeType : Edge
    {        
        [Range(1, int.MaxValue)]
        public int N;

        [MinLength(1)]
        public IEnumerable<EdgeType> Edges;

        public Graph(int n, IEnumerable<EdgeType>? edges)
        {
            N = n;
            Edges = edges is null ? [] : edges.Order().ToList();
        }
        public Graph(IEnumerable<EdgeType>? edges)
        {
            if (edges is null)
            {
                Edges = [];
                N = 0;
            } else
            {
                Edges = edges.Order().ToList();
                N = Edges.Select(edge => Math.Max(edge.From, edge.To)).Max() + 1;
            }
        }
        
        public Dictionary<int, HashSet<int>> GetConnectionDictionary(bool symmetric)
        {
            Dictionary<int, HashSet<int>> dict = new();
            foreach (var Edge in Edges)
            {
                if (!dict.ContainsKey(Edge.From))
                {
                    dict[Edge.From] = new();
                }
                dict[Edge.From].Add(Edge.To);

                if (symmetric)
                {
                    if (!dict.ContainsKey(Edge.To))
                    {
                        dict[Edge.To] = new();
                    }
                    dict[Edge.To].Add(Edge.From);
                }
            }
            return dict;
        }
        
        public static Graph<Edge> FromMatrix(Matrix m, bool makeSymmetric = false)
        {
            ArgumentNullException.ThrowIfNull(m);
            if (m.Rows == 0 || m.Cols == 0 || m.Rows != m.Cols)
            {
                throw new ArgumentException($"Invalid ({m.Rows}x{m.Cols}) matrix of edges!");
            }
            List<Edge> edges = [];
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Cols; j++)
                {
                    if (!m[i, j].IsZero)
                    {
                        edges.Add(new Edge(i, j));
                        if (makeSymmetric)
                        {
                            edges.Add(new Edge(j, i));
                        }
                    }
                }
            }
            return new Graph<Edge>(m.Rows, edges);
        }

        protected EdgeType? FindEdge(int from , int to)
        {
            int? matchIndex = Edges.ToArray().FirstOrNull(edge => edge.From == from && edge.To == to);
            return matchIndex.HasValue ? Edges.ElementAt(matchIndex.Value) : null;
        }

        public EdgeType? this[int from , int to] => FindEdge(from, to);
        public IEnumerable<EdgeType> this[int from] => Edges.Where(e => e.From == from);

        public IEnumerable<EdgeType>? GetEdges(
            IEnumerable<int> nodes, 
            bool bidirectional = false)
        {
            ArgumentNullException.ThrowIfNull(nodes, nameof(nodes));
            if (nodes.Count() <= 1)
            {
                return Enumerable.Empty<EdgeType>();
            }
            var edge = FindEdge(from: nodes.First(), to: nodes.ElementAt(1));
            edge ??= bidirectional ? FindEdge(to: nodes.First(), from: nodes.ElementAt(1)) : edge;
            if (edge is null)
            {
                return null;
            }
            var rest = GetEdges(nodes.Skip(1), bidirectional);
            if (rest is null)
            {
                return null;
            }
            return new List<EdgeType>() { edge }.Concat(rest);
        }

        public int GetEdgeIndex(EdgeType edge) => GetEdgeIndex(edge.From, edge.To);

        public int GetEdgeIndex(int from, int to)
        {
            for (int edgeIndex = 0; edgeIndex < Edges.Count(); edgeIndex++)
            {
                if (from == Edges.ElementAt(edgeIndex).From && to == Edges.ElementAt(edgeIndex).To)
                {
                    return edgeIndex;
                }
            }
            throw new ArgumentOutOfRangeException($"Could not find index of edge ({from}, {to})");
        }
        public IEnumerable<int> GetEdgeIndices(IEnumerable<EdgeType> edges) => edges.Select(GetEdgeIndex);
        public EdgeType[] GetEdges(IEnumerable<int> basis)
        {
            List<EdgeType> edges = [];
            foreach (int i in basis)
            {
                edges.Add(Edges.ElementAt(i));
            }
            return [.. edges];
        }

        /// <summary>
        /// Returns an enumerable with values 0, 1, 2, 3, ..., N - 1
        /// </summary>
        public IEnumerable<int> NodeList
        {
            get => Enumerable.Range(0, N);
        }
        
        public bool HasCycle(bool symmetric = false)
        {
            if (!Edges.Any())
            {
                return false;
            }
            if (Edges.Count() == 1)
            {
                return Edges.First().From == Edges.First().To;
            }
            return FindAllCycles(symmetric).Any();
        }
        
        public IEnumerable<int[]> AllBidirectionalCycles() =>
            FindAllCycles(true).Select(c => c.ToArray());

        public List<List<int>> FindAllCycles(bool symmetric)
        {
            List<List<int>> cycles = new List<List<int>>();
            var dict = GetConnectionDictionary(symmetric);

            foreach (int node in dict.Keys)
            {
                List<int> visited = [node];

                FindCyclesDFS(node, node, visited, cycles, dict);
            }

            return cycles;
        }

        // <summary>
        // Depth-first search (DFS) algorithm to find cycles in the graph.
        // </summary>
        // <param name="startNode">The starting node of the current DFS traversal.</param>
        // <param name="currentNode">The current node being visited.</param>
        // <param name="visited">A list of visited nodes in the current DFS traversal.</param>
        // <param name="cycles">A list to store the found cycles.</param>
        private void FindCyclesDFS(
            int startNode, int currentNode, 
            List<int> visited, List<List<int>> cycles,
            Dictionary<int, HashSet<int>> dict)
        {
            if (!dict.ContainsKey(currentNode))
            {
                return;
            }
            foreach (int neighbor in dict[currentNode])
            {
                if (visited.Contains(neighbor))
                {
                    if (visited.Count >= 3 && neighbor == startNode)
                    {
                        cycles.Add(new List<int>(visited));
                    }
                }
                else
                {
                    visited.Add(neighbor);
                    FindCyclesDFS(startNode, neighbor, visited, cycles, dict);
                    visited.Remove(neighbor);
                }
            }
        }

        public IEnumerable<int[]> AllOreintedPaths(int s, int t)
        {
            if (s < 0 || t < 0 || s >= N || t >= N)
            {
                throw new ArgumentException($"Invalid path points: {s + 1} -> {t + 1}");
            }

            bool[] visited = new bool[N];
            List<int[]> paths = new();

            allOrientedPathsUtil(
                s, t,
                visited,
                new List<int>(), paths);

            return paths;
        }

        private void allOrientedPathsUtil(
            int s, int t, 
            bool[] visited,
            List<int> local,
            List<int[]> finalPaths)
        {
            if (s == t)
            {
                finalPaths.Add(local.ToArray());
                return;
            }

            visited[s] = true;
            foreach (int neighbor in Edges.Where(e => e.From == s).Select(e => e.To).Distinct())
            {
                if (visited[neighbor])
                {
                    continue;
                }

                local.Add(neighbor);
                allOrientedPathsUtil(neighbor, t, visited, local, finalPaths);
                local.Remove(neighbor);
            }

            visited[s] = false;
        }

        public override string ToString() => string.Join(", ", Edges.Order().Select(e => e.ToString()));
    }

}
