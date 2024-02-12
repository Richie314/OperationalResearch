using Accord.Math;
using Fractions;
using OperationalResearch.Models.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    internal class MinCostFlow
    {
        private readonly Matrix E;
        private readonly Vector b;

        private readonly Matrix lb;
        private readonly Matrix? ub;
        public readonly BoundedGraphEdge[] Edges;

        public MinCostFlow(Matrix c, Vector b, Matrix lb, Matrix? ub)
        {
            this.b = b;
            if (!this.b.SumOfComponents().IsZero)
            {
                throw new ArgumentException("Sum of elements of b was not zero");
            }
            this.b = this.b.Get.Skip(1).ToArray();
            var ind = c.NonZeroIndices;
            List<BoundedGraphEdge> edges = new List<BoundedGraphEdge>();
            for (int row = 0; row < ind.Length; row++)
            {
                foreach (int col in ind[row])
                {
                    edges.Add(
                        new BoundedGraphEdge(
                            c[row, col], row, col, 
                            lb[row, col], ub is null ? null : ub[row, col]));
                }
            }
            Edges = edges.ToArray();
            E = new Matrix(b.Size - 1, Edges.Length);
            for (int edgeIndex = 0; edgeIndex < Edges.Length; edgeIndex++)
            {
                if (Edges[edgeIndex].From > 0)
                {
                    // skip first row
                    E[Edges[edgeIndex].From - 1, edgeIndex] = Fraction.MinusOne;
                }
                if (Edges[edgeIndex].To > 0)
                {
                    // skip first row
                    E[Edges[edgeIndex].To - 1, edgeIndex] = Fraction.One;
                }
            }

            this.lb = lb;
            this.ub = ub;
        }
        public Vector c { get => Edges.Select(e => e.Cost).ToArray(); }
        private int getEdgeIndex(BoundedGraphEdge edge)
        {
            return getEdgeIndex(edge.From, edge.To);
        }
        private int getEdgeIndex(int from, int to)
        {
            for (int edgeIndex = 0; edgeIndex < Edges.Length; edgeIndex++)
            {
                if (from == Edges[edgeIndex].From && to == Edges[edgeIndex].To)
                {
                    return edgeIndex;
                }
            }
            throw new ArgumentOutOfRangeException($"Could not find index of edge ({from}, {to})");
        }
        private IEnumerable<int> getEdgeIndices(IEnumerable<BoundedGraphEdge> edges)
        {
            return edges.Select(getEdgeIndex);
        }
        private BoundedGraphEdge? getEdge(int from, int to)
        {
            foreach (BoundedGraphEdge edge in Edges)
            {
                if (edge.From == from && edge.To == to)
                {
                    return edge;
                }
            }
            return null;
        }
        private IEnumerable<BoundedGraphEdge> getEdges(IEnumerable<int> indices)
        {
            if (indices is null || !indices.Any())
            {
                return Enumerable.Empty<BoundedGraphEdge>();
            }
            List<BoundedGraphEdge> edges = new();
            foreach (int i in indices)
            {
                edges.Add(Edges[i]);
            }
            return edges;
        }

        private Vector BasisFlow(IEnumerable<int> T)
        {
            var eT = E.GetCols(T).Inv;
            return eT * b;
        }
        private Vector BasisFlow(IEnumerable<BoundedGraphEdge> T)
        {
            return BasisFlow(T.Select(getEdgeIndex));
        }
        private Vector BasisPotential(IEnumerable<int> T)
        {
            var eT = E.GetCols(T).Inv;
            var cT = c[T].Row;
            var pad = new Vector(Fraction.Zero);
            return pad.Concat((cT * eT)[0]);
        }
        private Vector BasisPotential(IEnumerable<BoundedGraphEdge> T)
        {
            return BasisPotential(T.Select(getEdgeIndex));
        }
        public async Task<IEnumerable<int>?> SolveUnbounded(
            IEnumerable<BoundedGraphEdge>? startBase, StreamWriter? Writer = null)
        {
            if (startBase is null)
            {
                return null;
            }
            Writer ??= StreamWriter.Null;
            var T = getEdgeIndices(startBase).ToArray();
            while (true)
            {
                var L = Enumerable.Range(0, Edges.Length).Where(i => !T.Contains(i)).ToArray();

                var tEdges = getEdges(T);
                await Writer.WriteLineAsync($"T = {{ {string.Join(", ", tEdges.Select(e => e.ToString()))} }}");

                var lEdges = getEdges(L);
                await Writer.WriteLineAsync($"L = {{ {string.Join(", ", lEdges.Select(e => e.ToString()))} }}");

                var xt = BasisFlow(T);
                await Writer.WriteLineAsync($"x = {xt}");

                var π = BasisPotential(T);
                await Writer.WriteLineAsync($"π = {π}");

                Vector cReduced = c.Indices.Select(
                    idx => c[idx] + π[Edges[idx].From] - π[Edges[idx].To]).ToArray();
                await Writer.WriteLineAsync($"Reduced c = {cReduced}");

                if (L.All(i => !cReduced[i].IsNegative))
                {
                    await Writer.WriteLineAsync($"Flow is optimal");
                    return T;
                }

                var pq = L.Where(i => cReduced[i].IsNegative).First();
                await Writer.WriteLineAsync($"Entering edge (p, q) = {Edges[pq]}");

                var Cycles = new Graph(tEdges.Append(Edges[pq])).AllBidirectionalCicles().Where(c => c.Length > 0);
                if (!Cycles.Any())
                {
                    throw new DataMisalignedException("No cycle found but there should be at least one");
                }
                foreach (var cycle in Cycles)
                {
                    await Writer.WriteLineAsync(
                        $"T U {{ {Edges[pq]} }} has cycle: {string.Join("-", cycle.Select(i => i + 1))}");
                }
                var C = Cycles.First();
                bool forward = false;
                for (int i = 0; i < C.Length; i++)
                {
                    int from = C[i];
                    int to = C[(i + 1) % C.Length];
                    if (Edges[pq].From == from && Edges[pq].To == to)
                    { 
                        forward = true; 
                        break;
                    }
                }
                await Writer.WriteLineAsync(
                    $"{Edges[pq]} is used {(forward ? "forward" : "reversed")} in cycle {string.Join("-", C.Select(i => i + 1))}");
                
                List<BoundedGraphEdge> oppositeDirection = new();
                for (int i = 0; i < C.Length; i++)
                {
                    int from = C[i];
                    int to = C[(i + 1) % C.Length];
                    var ForwardEdge = getEdge(from, to);
                    if (ForwardEdge is not null)
                    {
                        if (!forward)
                        {
                            oppositeDirection.Add(ForwardEdge);
                        }
                        continue;
                    }

                    var ReversedEdge = getEdge(to, from);
                    if (ReversedEdge is not null)
                    {
                        if (forward)
                        {
                            oppositeDirection.Add(ReversedEdge);
                        }
                        continue;
                    }
                    throw new DataMisalignedException(
                        $"It was impossible to find edge ({from}, {to}) or ({to}, {from})");
                }

                if (oppositeDirection.Count == 0)
                {
                    await Writer.WriteLineAsync(
                        $"C^- has no edges with direction opposite to {Edges[pq]}");
                    await Writer.WriteLineAsync("Problem is unbounded");
                    return null;
                }

                oppositeDirection = oppositeDirection.Order().ToList();
                await Writer.WriteLineAsync(
                    $"C^- = {{ {string.Join(", ", oppositeDirection.Select(e =>e.ToString()))} }}");

                Fraction Theta = oppositeDirection.Select(getEdgeIndex).Select(i => xt[i]).Min();
                await Writer.WriteLineAsync($"Theta = {Function.Print(Theta)}");

                var ExitingEdge = oppositeDirection.Where(e => xt[getEdgeIndex(e)] == Theta).First();
                await Writer.WriteLineAsync($"Exiting edge (r, s) = {ExitingEdge}");

                // Update T
                int exitingIndex = getEdgeIndex(ExitingEdge);
                T = T.Where(i => i != exitingIndex).Append(pq).ToArray();
                T.Sort();
            }

        }
        public async Task<bool> FlowUnbounded(
            IEnumerable<BoundedGraphEdge>? startBase, StreamWriter? Writer = null)
        {
            Writer ??= StreamWriter.Null;
            if (startBase is null)
            {
                await Writer.WriteLineAsync("Using 1-tree as start base");
                var OneTree = await new Graph(Edges).FindKTree(0, true, null);
                if (OneTree is not null)
                {
                    startBase = OneTree.Select(e => new BoundedGraphEdge(e));
                }
            }
            try
            {
                var sol = await SolveUnbounded(startBase, Writer);
                if (sol is null)
                {
                    await Writer.WriteLineAsync("Solution is null");
                    return false;
                }
                await Writer.WriteLineAsync("Solution is " + Function.Print(sol));
                return true;
            } catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened '{ex.Message}'");
                return false;
            }
        }
    }
}
