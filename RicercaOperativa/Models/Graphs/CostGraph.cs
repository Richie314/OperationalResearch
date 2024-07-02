using Fractions;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.Models.Graphs
{
    public partial class CostGraph<EdgeType> : 
        Graph<EdgeType> where EdgeType : CostEdge
    {
        public CostGraph(int n, IEnumerable<EdgeType>? edges) : base(n, edges) { }
        public CostGraph(IEnumerable<EdgeType>? edges) : base(edges) { }

        public Matrix BuildMatrix(bool ConsiderSymmetric = false)
        {
            Matrix m = new Matrix(N, N);

            foreach (EdgeType edge in Edges)
            {
                m[edge.From, edge.To] = edge.Cost;
                if (ConsiderSymmetric)
                {
                    m[edge.To, edge.From] = edge.Cost;
                }
            }
            return m;
        }

        public static new CostGraph<CostEdge> FromMatrix(Matrix m, bool ForceSymmetric = false)
        {
            ArgumentNullException.ThrowIfNull(m, nameof(m));
            if (m.Rows == 0 || m.Cols == 0 || m.Rows != m.Cols)
            {
                throw new ArgumentException($"Invalid ({m.Rows}x{m.Cols}) matrix of edges!");
            }

            List<CostEdge> edges = [];
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Cols; j++)
                {
                    if (!m[i, j].IsZero)
                    {
                        edges.Add(new CostEdge(m[i, j], i, j));
                        if (ForceSymmetric)
                        {
                            edges.Add(new CostEdge(m[i, j], j, i));
                        }
                    }
                }
            }
            return new CostGraph<CostEdge>(m.Rows, edges);
        }

        public Fraction Cost(IEnumerable<int> nodes, bool bidirectional = false)
        {
            ArgumentNullException.ThrowIfNull(nodes, nameof(nodes));
            if (nodes.Count() <= 1)
            {
                return Fraction.Zero;
            }
            var edge = FindEdge(from: nodes.First(), to: nodes.ElementAt(1));
            edge ??= bidirectional ? FindEdge(to: nodes.First(), from: nodes.ElementAt(1)) : edge;
            if (edge is null)
            {
                throw new Exception(
                    $"Path invalid, impossible to go from {nodes.First() + 1} to {nodes.ElementAt(1) + 1}!");
            }
            return edge.Cost + Cost(nodes.Skip(1), bidirectional);
        }

        public static Fraction Cost(IEnumerable<EdgeType>? edges)
        {
            if (edges is null || !edges.Any())
            {
                return Fraction.Zero;
            }

            return edges.First().Cost + Cost(edges.Skip(1));
        }



        /// <summary>
        /// Edges with cost > 0
        /// </summary>
        public IEnumerable<EdgeType> PositiveEdges
        {
            get => Edges.Where(edge => edge.Cost.IsPositive);
        }

        /// <summary>
        /// Edges with cost >= 0
        /// </summary>
        public IEnumerable<EdgeType> NonNegativeEdges
        {
            get => Edges.Where(edge => !edge.Cost.IsNegative);
        }

        /// <summary>
        /// Edges with cost == 0
        /// </summary>
        public IEnumerable<EdgeType> ZeroEdges
        {
            get => Edges.Where(edge => edge.Cost.IsZero);
        }

        /// <summary>
        /// Edges with cost <= 0
        /// </summary>
        public IEnumerable<EdgeType> NonPositiveEdges
        {
            get => Edges.Where(edge => !edge.Cost.IsPositive);
        }

        /// <summary>
        /// Edges with cost < 0
        /// </summary>
        public IEnumerable<EdgeType> NegativeEdges
        {
            get => Edges.Where(edge => edge.Cost.IsNegative);
        }
    }
}
