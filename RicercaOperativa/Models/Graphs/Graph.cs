using Accord.Math;
using Fractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationalResearch.Extensions;
using System.Xaml;

namespace OperationalResearch.Models
{
    public partial class Graph
    {        
        [Range(1, int.MaxValue)]
        public int N;
        [MinLength(1)]
        public IEnumerable<Edge> Edges;

        public Graph(int n, IEnumerable<Edge>? edges)
        {
            N = n;
            Edges = edges is null ? new List<Edge>() : edges.Order();
        }
        public Graph(IEnumerable<Edge>? edges) : 
            this(
                (edges != null && edges.Any()) ? edges.Select(edge => Math.Max(edge.From, edge.To)).Max() : 0, 
                edges)
        {
        }
        public Matrix BuildMatrix(bool Symmetric = false)
        {
            Fraction[,] m = new Fraction[N, N];
            for (int i = 0; i < N; i++)
            {
                m.SetRow(i, Enumerable.Repeat(Fraction.Zero, N).ToArray());
            }
            foreach (Edge edge in Edges)
            {
                m[edge.From, edge.To] = edge.Cost;
                if (Symmetric)
                {
                    m[edge.To, edge.From] = edge.Cost;
                }
            }
            foreach (Edge edge in Edges)
            {
                if (m[edge.To, edge.From].IsZero && Symmetric)
                {
                    m[edge.To, edge.From] = edge.Cost;
                }
            }
            return new Matrix(m);
        }
        public static Graph FromMatrix(Matrix m, bool makeSymmetric = false)
        {
            ArgumentNullException.ThrowIfNull(m);
            if (m.Rows == 0 || m.Cols == 0 || m.Rows != m.Cols)
            {
                throw new ArgumentException($"Invalid ({m.Rows}x{m.Cols}) matrix of edges!");
            }
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Cols; j++)
                {
                    if (!m[i, j].IsZero)
                    {
                        edges.Add(new Edge(m[i, j], i, j));
                        if (makeSymmetric)
                        {
                            edges.Add(new Edge(m[i, j], j, i));
                        }
                    }
                }
            }
            return new Graph(m.Rows, edges);
        }
        private Edge? FindEdge(int from , int to)
        {
            int? matchIndex = Edges.ToArray().FirstOrNull(edge => edge.From == from && edge.To == to);
            return matchIndex.HasValue ? Edges.ElementAt(matchIndex.Value) : null;
        }
        public Fraction Cost(IEnumerable<int> nodes, bool missingEdgesValid = false)
        {
            ArgumentNullException.ThrowIfNull(nodes, nameof(nodes));
            if (nodes.Count() <= 1)
            {
                return Fraction.Zero;
            }
            var edge = FindEdge(from: nodes.First(), to: nodes.ElementAt(1));
            if (edge is null)
            {
                if (missingEdgesValid)
                {
                    edge = new Edge(Fraction.Zero, from: nodes.First(), to: nodes.ElementAt(1));
                } else
                {
                    throw new Exception(
                        $"Path invalid, impossible to go from {nodes.First()} to {nodes.ElementAt(1)}!");
                }
            }
            return edge.Cost + Cost(nodes.Skip(1), missingEdgesValid);
        }
        public Fraction Cost(IEnumerable<Edge>? edges)
        {
            if (edges is null || !edges.Any())
            {
                return Fraction.Zero;
            }

            return edges.First().Cost + Cost(edges.Skip(1));
        }
        public IEnumerable<Edge>? GetEdges(IEnumerable<int> nodes)
        {
            ArgumentNullException.ThrowIfNull(nodes, nameof(nodes));
            if (nodes.Count() <= 1)
            {
                return Enumerable.Empty<Edge>();
            }
            var edge = FindEdge(from: nodes.First(), to: nodes.ElementAt(1));
            if (edge is null)
            {
                return null;
            }
            var rest = GetEdges(nodes.Skip(1));
            if (rest is null)
            {
                return null;
            }
            return new List<Edge>() { edge }.Concat(rest);
        }

        /// <summary>
        /// Edges with cost > 0
        /// </summary>
        public IEnumerable<Edge> PositiveEdges
        {
            get => Edges.Where(edge => edge.Cost.IsPositive);
        }
        /// <summary>
        /// Edges with cost >= 0
        /// </summary>
        public IEnumerable<Edge> NonNegativeEdges
        {
            get => Edges.Where(edge => !edge.Cost.IsNegative);
        }
        /// <summary>
        /// Edges with cost == 0
        /// </summary>
        public IEnumerable<Edge> ZeroEdges
        {
            get => Edges.Where(edge => edge.Cost.IsZero);
        }
        /// <summary>
        /// Edges with cost <= 0
        /// </summary>
        public IEnumerable<Edge> NonPositiveEdges
        {
            get => Edges.Where(edge => !edge.Cost.IsPositive);
        }
        /// <summary>
        /// Edges with cost < 0
        /// </summary>
        public IEnumerable<Edge> NegativeEdges
        {
            get => Edges.Where(edge => edge.Cost.IsNegative);
        }
        public IEnumerable<Edge> RequiredEdges
        {
            get => Edges.Where(edge => edge.Type == Edge.EdgeType.Required);
        }

        /// <summary>
        /// Returns an enumerable with values 0, 1, 2, 3, ..., N - 1
        /// </summary>
        public IEnumerable<int> NodeList
        {
            get => Enumerable.Range(0, N);
        }
        /// <summary>
        /// Searches for Hamiltonian cycle by trying every possible combination
        /// This method is sync
        /// </summary>
        /// <returns>The best cycle or null if no cycle can be found</returns>
        /// <exception cref="DataMisalignedException">If permutation of the nodes creates problems</exception>
        public IEnumerable<int>? BruteForceHamiltonCycle()
        {
            IEnumerable<int> BestPerm = [];
            Fraction BestPermCost = Fraction.Zero;
            int requiredCount = RequiredEdges.Count();

            foreach (var perm in NodeList.AllPermutations())
            {
                if (perm.Count() != N)
                {
                    throw new DataMisalignedException(
                        $"Permutation has number of element different from the number of nodes ({perm.Count()} != {N})");
                }
                var cyclePerm = perm.Append(perm.First()); 
                // transforms (A-B-C) in (A-B-C-A)
                try
                {
                    Fraction cost = Cost(cyclePerm);
                    int currentRequiredEdges = 
                        (GetEdges(cyclePerm) ?? Enumerable.Empty<Edge>())
                        .Count(edge => edge.Type == Edge.EdgeType.Required);

                    if (requiredCount != 0 && currentRequiredEdges < requiredCount)
                    {
                        continue;
                    }

                    if (cost < BestPermCost || BestPerm.Count() == 0)
                    {
                        BestPermCost = cost;
                        BestPerm = cyclePerm;
                    }
                } catch
                {
                    continue;
                }
                
            }

            return BestPerm.Any() ? BestPerm : null;
        }
        
        

    }

}
