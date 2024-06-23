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
        }
        private Vector u { get => Edges.Select(e => e.ub ?? Fraction.Zero).ToArray(); }
        public Vector c { get => Edges.Select(e => e.Cost).ToArray(); }
        private int getEdgeIndex(BoundedGraphEdge edge) => 
            getEdgeIndex(edge.From, edge.To);

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
        private IEnumerable<int> getEdgeIndices(IEnumerable<BoundedGraphEdge> edges) =>
            edges.Select(getEdgeIndex);

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
        private Vector BasisFlow(IEnumerable<int> T, IEnumerable<int> U)
        {
            var eT = E.GetCols(T);
            var eU = E.GetCols(U);
            return eT.Inv * (b - (eU * u[U]));
        }
        private Vector BasisFlow(IEnumerable<BoundedGraphEdge> T) =>
            BasisFlow(T.Select(getEdgeIndex));

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
            await Writer.WriteLineAsync(
                $"Edges = {{ {string.Join(", ", Edges.Select(e => e.ToString()))} }}");
            await Writer.WriteLineAsync($"E = {E}");
            var T = getEdgeIndices(startBase).ToArray();
            while (true)
            {
                var L = Enumerable.Range(0, Edges.Length).Where(i => !T.Contains(i)).ToArray();

                var tEdges = getEdges(T).ToArray();
                await Writer.WriteLineAsync($"T = {{ {string.Join(", ", tEdges.Select(e => e.ToString()))} }}");

                var lEdges = getEdges(L).ToArray();
                await Writer.WriteLineAsync($"L = {{ {string.Join(", ", lEdges.Select(e => e.ToString()))} }}");

                await Writer.WriteLineAsync($"E_T = {E.GetCols(T)}");

                var xt = BasisFlow(T);
                await Writer.WriteLineAsync($"x_t = {xt}");

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

                Fraction Theta = oppositeDirection
                    .Select(e => indexInArr(tEdges, e))
                    .Select(i => xt[i]).Min();
                await Writer.WriteLineAsync($"Theta = {Function.Print(Theta)}");

                var ExitingEdge = oppositeDirection.Where(e => xt[indexInArr(tEdges, e)] == Theta).First();
                await Writer.WriteLineAsync($"Exiting edge (r, s) = {ExitingEdge}");

                // Update T
                int exitingIndex = getEdgeIndex(ExitingEdge);
                T = T.Where(i => i != exitingIndex).Append(pq).ToArray();
                T.Sort();
            }
            
        }
        private static int indexInArr(
            BoundedGraphEdge[] haystack, BoundedGraphEdge needle)
        {
            for (int i = 0; i < haystack.Length; i++)
            {
                if (haystack[i] == needle) return i;
            }
            return -1;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="T">Indexes of the EDGES in T</param>
        /// <param name="U">Indexes if the EDGES in U</param>
        /// <returns></returns>
        private Vector BuildX(IEnumerable<int> T, IEnumerable<int> U)
        {
            var xt = BasisFlow(T, U);
            var xu = u[U];
            List<Fraction> x = new();
            for (int i = 0; i < Edges.Length; i++)
            {
                //i: index of edges in order
                if (T.Contains(i))
                {
                    // edge of index i is in T
                    x.Add(xt[T.ToArray().IndexOf(i)]);
                    continue;
                }
                if (U.Contains(i))
                {
                    // egde of index i is in U
                    x.Add(xu[U.ToArray().IndexOf(i)]);
                    continue;
                }
                x.Add(Fraction.Zero);
            }
            return x.ToArray();
        }
        public async Task<IEnumerable<int>?> SolveBounded(
            IEnumerable<BoundedGraphEdge>? startBase, 
            IEnumerable<BoundedGraphEdge>? startU, StreamWriter? Writer = null)
        {
            if (startBase is null)
            {
                return null;
            }
            startU ??= Enumerable.Empty<BoundedGraphEdge>();   
            Writer ??= StreamWriter.Null;
            await Writer.WriteLineAsync(
                $"Edges = {{ {string.Join(", ", Edges.Select(e => e.ToString()))} }}");
            await Writer.WriteLineAsync($"u = {u}");

            await Writer.WriteLineAsync($"E = {E}");
            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();

            var T = getEdgeIndices(startBase).ToArray();
            var U = getEdgeIndices(startU).ToArray();
            while (true)
            {
                var L = Enumerable.Range(0, Edges.Length)
                    .Where(i => !T.Contains(i) && !U.Contains(i)).ToArray();

                var tEdges = getEdges(T).ToArray();
                await Writer.WriteLineAsync($"T = {{ {string.Join(", ", tEdges.Select(e => e.ToString()))} }}");

                var uEdges = getEdges(U).ToArray();
                await Writer.WriteLineAsync($"U = {{ {string.Join(", ", uEdges.Select(e => e.ToString()))} }}");

                var lEdges = getEdges(L).ToArray();
                await Writer.WriteLineAsync($"L = {{ {string.Join(", ", lEdges.Select(e => e.ToString()))} }}");

                await Writer.WriteLineAsync($"E_T = {E.GetCols(T)}");

                await Writer.WriteLineAsync($"x_U = u_U = {u[U]}");
                await Writer.WriteLineAsync($"x_L = {(Vector)Enumerable.Repeat(Fraction.Zero, L.Length).ToArray()}");

                var xt = BasisFlow(T, U);
                await Writer.WriteLineAsync($"x_t = {xt}");

                var x = BuildX(T, U);
                await Writer.WriteLineAsync($"Full X = {x}");

                var π = BasisPotential(T);
                await Writer.WriteLineAsync($"π = {π}");

                Vector cReduced = c.Indices.Select(
                    idx => c[idx] + π[Edges[idx].From] - π[Edges[idx].To]).ToArray();
                await Writer.WriteLineAsync($"Reduced c = {cReduced}");
                await Writer.WriteLineAsync($"Reduced cL = {cReduced[L]}");
                await Writer.WriteLineAsync($"Reduced cU = {cReduced[U]}");

                if (
                    L.All(i => !cReduced[i].IsNegative) &&
                    U.All(i => !cReduced[i].IsPositive))
                {
                    await Writer.WriteLineAsync($"Flow is optimal");
                    return T;
                }
                //var pql = L.Where(i => cReduced[i].IsNegative);
                //var pqu = U.Where(i => cReduced[i].IsPositive);
                //await Writer.WriteLineAsync($"{Function.Print(pql)} | {Function.Print(pqu)}");
                
                var pq = 
                    L.Where(i => cReduced[i].IsNegative).Concat(
                        U.Where(i => cReduced[i].IsPositive))
                    .OrderBy(i => Edges[i]).First();
                await Writer.WriteLineAsync($"Entering edge (p, q) = {Edges[pq]}");

                var Cycles = new Graph(tEdges.Append(Edges[pq]))
                    .AllBidirectionalCicles().Where(c => c.Length > 0);
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

                if (U.Contains(pq))
                {
                    forward = !forward; 
                    await Writer.WriteLineAsync(
                        $"Considering Cycle in opposite direction since {Edges[pq]} is in U");
                }


                List<BoundedGraphEdge> oppositeDirection = new();
                List<BoundedGraphEdge> currentDirection = new();
                for (int i = 0; i < C.Length; i++)
                {
                    int from = C[i];
                    int to = C[(i + 1) % C.Length];
                    var ForwardEdge = getEdge(from, to);
                    if (ForwardEdge is not null)
                    {
                        if (forward)
                        {
                            currentDirection.Add(ForwardEdge);
                        } else {
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
                        } else
                        {
                            currentDirection.Add(ReversedEdge);
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
                if (currentDirection.Count == 0)
                {
                    await Writer.WriteLineAsync(
                        $"C^+ has no edges with direction opposite to {Edges[pq]}");
                    await Writer.WriteLineAsync("Problem is unbounded");
                    return null;
                }

                oppositeDirection = oppositeDirection.Order().ToList();
                await Writer.WriteLineAsync(
                    $"C^- = {{ {string.Join(", ", oppositeDirection.Select(e => e.ToString()))} }}");
                currentDirection = currentDirection.Order().ToList();
                await Writer.WriteLineAsync(
                    $"C^+ = {{ {string.Join(", ", currentDirection.Select(e => e.ToString()))} }}");


                Fraction ThetaMinus = oppositeDirection
                    .Select(getEdgeIndex)
                    .Select(i => x[i]).Min();
                await Writer.WriteLineAsync($"Theta^- = {Function.Print(ThetaMinus)}");

                Fraction ThetaPlus = currentDirection
                    .Select(getEdgeIndex)
                    .Select(i => u[i] - x[i]).Min();
                await Writer.WriteLineAsync($"Theta^+ = {Function.Print(ThetaPlus)}");

                Fraction Theta = ThetaMinus > ThetaPlus ? ThetaPlus : ThetaMinus;
                await Writer.WriteLineAsync($"Theta = {Function.Print(Theta)}");

                var rs = 
                    oppositeDirection.Where(e => x[getEdgeIndex(e)] == Theta).Concat(
                    currentDirection.Where(e => 
                        u[getEdgeIndex(e)] - xt[getEdgeIndex(e)] == Theta))
                    .First();
                await Writer.WriteLineAsync($"Exiting edge (r, s) = {rs}");

                // Update T, U, L
                if (L.Contains(pq))
                {
                    if (currentDirection.Any(e => e == rs))
                    {
                        if (Edges[pq] != rs)
                        {
                            T = T.Where(i => i != getEdgeIndex(rs)).Append(pq).ToArray();
                        }
                        U = U.Append(getEdgeIndex(rs)).ToArray();
                    } else
                    {
                        T = T.Where(i => i != getEdgeIndex(rs)).Append(pq).ToArray();
                    }
                } else
                {
                    if (oppositeDirection.Any(e => e == rs))
                    {
                        if (Edges[pq] != rs)
                        {
                            T = T.Where(i => i != getEdgeIndex(rs)).Append(pq).ToArray();
                        }
                        U = U.Where(i => i != pq).ToArray();
                    }
                    else
                    {
                        T = T.Where(i => i != getEdgeIndex(rs)).Append(pq).ToArray();
                        U = U.Where(i => i != pq).Append(getEdgeIndex(rs)).ToArray();
                    }
                }
                T.Sort();
                U.Sort();

                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
            }
        }
        public async Task<bool> FlowBounded(
            IEnumerable<BoundedGraphEdge>? startBase,
            IEnumerable<BoundedGraphEdge>? startU, StreamWriter? Writer = null)
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
                var sol = await SolveBounded(startBase, startU, Writer);
                if (sol is null)
                {
                    await Writer.WriteLineAsync("Solution is null");
                    return false;
                }
                await Writer.WriteLineAsync("Solution is " + Function.Print(sol));
                return true;
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened '{ex.Message}'");
                return false;
            }
        }
    }
}
