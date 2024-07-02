using Accord.Math;
using Fractions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Extensions;
using Vector = OperationalResearch.Models.Elements.Vector;

namespace OperationalResearch.Models.Graphs
{
    public class MinimumCostFlow<EdgeType> : BoundedCostGraph<EdgeType>
        where EdgeType : BoundedCostEdge
    {
        /// <summary>
        /// Node balances vector
        /// </summary>
        private readonly Vector b;

        public MinimumCostFlow(IEnumerable<EdgeType> edges, Vector b) : base(b.Size, edges)
        {
            this.b = b;
            if (!this.b.SumOfComponents().IsZero)
            {
                throw new ArgumentException("Sum of elements of b was not zero");
            }
            // Ignore first row
            this.b = this.b.Get.Skip(1).ToArray();
        }
        
        public Vector BasisFlow(IEnumerable<int> T)
        {
            var eT = E.GetCols(T).Inv;
            return eT * b;
        }
        public Vector BasisFlow(IEnumerable<int> T, IEnumerable<int> U)
        {
            var eT = E.GetCols(T);
            var eU = E.GetCols(U);
            return eT.Inv * (b - eU * u[U]);
        }
        public Vector BasisFlow(IEnumerable<EdgeType> T) => BasisFlow(T.Select(GetEdgeIndex));

        public Vector BasisPotential(IEnumerable<int> T)
        {
            var eT = E.GetCols(T).Inv;
            var cT = c[T].Row;
            var pad = new Vector(Fraction.Zero);
            return pad.Concat((cT * eT)[0]);
        }
        public Vector BasisPotential(IEnumerable<EdgeType> T) => BasisPotential(T.Select(GetEdgeIndex));

        public async Task<IEnumerable<int>?> SolveUnbounded(
            IEnumerable<EdgeType>? startBase, IndentWriter? Writer = null)
        {
            if (startBase is null)
            {
                return null;
            }
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync(
                $"Edges = {{ {string.Join(", ", Edges.Select(e => e.ToString()))} }}");
            await Writer.WriteLineAsync($"E = {E}");
            var T = GetEdgeIndices(startBase).ToArray();
            while (true)
            {
                var L = Enumerable.Range(0, Edges.Count()).Where(i => !T.Contains(i)).ToArray();

                var tEdges = GetEdges(T);
                await Writer.WriteLineAsync($"T = {{ {string.Join(", ", tEdges.Select(e => e.ToString()))} }}");

                var lEdges = GetEdges(L);
                await Writer.WriteLineAsync($"L = {{ {string.Join(", ", lEdges.Select(e => e.ToString()))} }}");

                await Writer.WriteLineAsync($"E_T = {E.GetCols(T)}");

                var xt = BasisFlow(T);
                await Writer.WriteLineAsync($"x_t = {xt}");

                var π = BasisPotential(T);
                await Writer.WriteLineAsync($"π = {π}");

                Vector cReduced = c.Indices.Select(
                    idx => c[idx] + π[Edges.ElementAt(idx).From] - π[Edges.ElementAt(idx).To]).ToArray();
                await Writer.WriteLineAsync($"Reduced c = {cReduced}");

                if (L.All(i => !cReduced[i].IsNegative))
                {
                    await Writer.WriteLineAsync($"Flow is optimal");
                    return T;
                }

                var pq = L.Where(i => cReduced[i].IsNegative).First();
                await Writer.WriteLineAsync($"Entering edge (p, q) = {Edges.ElementAt(pq)}");

                var Cycles = new Graph<EdgeType>(tEdges.Append(Edges.ElementAt(pq)))
                    .AllBidirectionalCycles()
                    .Where(c => c.Length > 0);
                if (!Cycles.Any())
                {
                    throw new DataMisalignedException("No cycle found but there should be at least one");
                }
                foreach (var cycle in Cycles)
                {
                    await Writer.WriteLineAsync(
                        $"T U {{ {Edges.ElementAt(pq)} }} has cycle: {string.Join("-", cycle.Select(i => i + 1))}");
                }
                var C = Cycles.First();
                bool forward = false;
                for (int i = 0; i < C.Length; i++)
                {
                    int from = C[i];
                    int to = C[(i + 1) % C.Length];
                    if (Edges.ElementAt(pq).From == from && Edges.ElementAt(pq).To == to)
                    {
                        forward = true;
                        break;
                    }
                }
                await Writer.WriteLineAsync(
                    $"{Edges.ElementAt(pq)} is used {(forward ? "forward" : "reversed")} in cycle {string.Join("-", C.Select(i => i + 1))}");

                List<EdgeType> oppositeDirection = new();
                for (int i = 0; i < C.Length; i++)
                {
                    int from = C[i];
                    int to = C[(i + 1) % C.Length];
                    var ForwardEdge = FindEdge(from, to);
                    if (ForwardEdge is not null)
                    {
                        if (!forward)
                        {
                            oppositeDirection.Add(ForwardEdge);
                        }
                        continue;
                    }

                    var ReversedEdge = FindEdge(to, from);
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
                        $"C^- has no edges with direction opposite to {Edges.ElementAt(pq)}");
                    await Writer.WriteLineAsync("Problem is unbounded");
                    return null;
                }

                oppositeDirection = oppositeDirection.Order().ToList();
                await Writer.WriteLineAsync(
                    $"C^- = {{ {string.Join(", ", oppositeDirection.Select(e => e.ToString()))} }}");

                Fraction Theta = oppositeDirection
                    .Select(e => tEdges.IndexOf(e))
                    .Select(i => xt[i]).Min();
                await Writer.WriteLineAsync($"Theta = {Function.Print(Theta)}");

                var ExitingEdge = oppositeDirection.Where(e => xt[tEdges.IndexOf(e)] == Theta).First();
                await Writer.WriteLineAsync($"Exiting edge (r, s) = {ExitingEdge}");

                // Update T
                int exitingIndex = GetEdgeIndex(ExitingEdge);
                T = T.Where(i => i != exitingIndex).Append(pq).ToArray();
                T.Sort();
            }

        }
        public async Task<bool> FlowUnbounded(
            IEnumerable<EdgeType>? startBase, IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            if (startBase is null)
            {
                await Writer.WriteLineAsync("Using 1-tree as start base");
                var OneTree = await FindKTree(0, true, null);
                if (OneTree is not null)
                {
                    startBase = OneTree;
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
            }
            catch (Exception ex)
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
            for (int i = 0; i < Edges.Count(); i++)
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
            IEnumerable<EdgeType>? startBase,
            IEnumerable<EdgeType>? startU, IndentWriter? Writer = null)
        {
            if (startBase is null)
            {
                return null;
            }
            startU ??= Enumerable.Empty<EdgeType>();
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync(
                $"Edges = {{ {string.Join(", ", Edges.Select(e => e.ToString()))} }}");
            await Writer.WriteLineAsync($"u = {u}");

            await Writer.WriteLineAsync($"E = {E}");
            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();

            var T = GetEdgeIndices(startBase).ToArray();
            var U = GetEdgeIndices(startU).ToArray();
            while (true)
            {
                var L = Enumerable.Range(0, Edges.Count())
                    .Where(i => !T.Contains(i) && !U.Contains(i)).ToArray();

                var tEdges = GetEdges(T);
                await Writer.WriteLineAsync($"T = {{ {string.Join(", ", tEdges.Select(e => e.ToString()))} }}");

                var uEdges = GetEdges(U);
                await Writer.WriteLineAsync($"U = {{ {string.Join(", ", uEdges.Select(e => e.ToString()))} }}");

                var lEdges = GetEdges(L);
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
                    idx => c[idx] + π[Edges.ElementAt(idx).From] - π[Edges.ElementAt(idx).To]).ToArray();
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
                    .OrderBy(i => Edges.ElementAt(i)).First();
                await Writer.WriteLineAsync($"Entering edge (p, q) = {Edges.ElementAt(pq)}");

                var Cycles = new Graph<EdgeType>(tEdges.Append(Edges.ElementAt(pq)))
                    .AllBidirectionalCycles()
                    .Where(c => c.Length > 0);
                if (!Cycles.Any())
                {
                    throw new DataMisalignedException("No cycle found but there should be at least one");
                }
                foreach (var cycle in Cycles)
                {
                    await Writer.WriteLineAsync(
                        $"T U {{ {Edges.ElementAt(pq)} }} has cycle: {string.Join("-", cycle.Select(i => i + 1))}");
                }
                var C = Cycles.First();
                bool forward = false;
                for (int i = 0; i < C.Length; i++)
                {
                    int from = C[i];
                    int to = C[(i + 1) % C.Length];
                    if (Edges.ElementAt(pq).From == from && Edges.ElementAt(pq).To == to)
                    {
                        forward = true;
                        break;
                    }
                }

                await Writer.WriteLineAsync(
                    $"{Edges.ElementAt(pq)} is used {(forward ? "forward" : "reversed")} in cycle {string.Join("-", C.Select(i => i + 1))}");

                if (U.Contains(pq))
                {
                    forward = !forward;
                    await Writer.WriteLineAsync(
                        $"Considering Cycle in opposite direction since {Edges.ElementAt(pq)} is in U");
                }


                List<EdgeType> oppositeDirection = [];
                List<EdgeType> currentDirection = [];
                for (int i = 0; i < C.Length; i++)
                {
                    int from = C[i];
                    int to = C[(i + 1) % C.Length];
                    var ForwardEdge = FindEdge(from, to);
                    if (ForwardEdge is not null)
                    {
                        if (forward)
                        {
                            currentDirection.Add(ForwardEdge);
                        }
                        else
                        {
                            oppositeDirection.Add(ForwardEdge);
                        }
                        continue;
                    }

                    var ReversedEdge = FindEdge(to, from);
                    if (ReversedEdge is not null)
                    {
                        if (forward)
                        {
                            oppositeDirection.Add(ReversedEdge);
                        }
                        else
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
                        $"C^- has no edges with direction opposite to {Edges.ElementAt(pq)}");
                    await Writer.WriteLineAsync("Problem is unbounded");
                    return null;
                }
                if (currentDirection.Count == 0)
                {
                    await Writer.WriteLineAsync(
                        $"C^+ has no edges with direction opposite to {Edges.ElementAt(pq)}");
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
                    .Select(GetEdgeIndex)
                    .Select(i => x[i]).Min();
                await Writer.WriteLineAsync($"Theta^- = {Function.Print(ThetaMinus)}");

                Fraction ThetaPlus = currentDirection
                    .Select(GetEdgeIndex)
                    .Select(i => u[i] - x[i]).Min();
                await Writer.WriteLineAsync($"Theta^+ = {Function.Print(ThetaPlus)}");

                Fraction Theta = ThetaMinus > ThetaPlus ? ThetaPlus : ThetaMinus;
                await Writer.WriteLineAsync($"Theta = {Function.Print(Theta)}");

                var rs =
                    oppositeDirection.Where(e => x[GetEdgeIndex(e)] == Theta).Concat(
                    currentDirection.Where(e =>
                        u[GetEdgeIndex(e)] - xt[GetEdgeIndex(e)] == Theta))
                    .First();
                await Writer.WriteLineAsync($"Exiting edge (r, s) = {rs}");

                // Update T, U, L
                if (L.Contains(pq))
                {
                    if (currentDirection.Any(e => e == rs))
                    {
                        if (Edges.ElementAt(pq) != rs)
                        {
                            T = T.Where(i => i != GetEdgeIndex(rs)).Append(pq).ToArray();
                        }
                        U = U.Append(GetEdgeIndex(rs)).ToArray();
                    }
                    else
                    {
                        T = T.Where(i => i != GetEdgeIndex(rs)).Append(pq).ToArray();
                    }
                }
                else
                {
                    if (oppositeDirection.Any(e => e == rs))
                    {
                        if (Edges.ElementAt(pq) != rs)
                        {
                            T = T.Where(i => i != GetEdgeIndex(rs)).Append(pq).ToArray();
                        }
                        U = U.Where(i => i != pq).ToArray();
                    }
                    else
                    {
                        T = T.Where(i => i != GetEdgeIndex(rs)).Append(pq).ToArray();
                        U = U.Where(i => i != pq).Append(GetEdgeIndex(rs)).ToArray();
                    }
                }
                T.Sort();
                U.Sort();

                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
            }
        }
        
        public async Task<bool> FlowBounded(
            IEnumerable<EdgeType>? startBase,
            IEnumerable<EdgeType>? startU, 
            IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            if (startBase is null)
            {
                await Writer.WriteLineAsync("Using 1-tree as start base");
                var OneTree = await FindKTree(0, true, null);
                if (OneTree is not null)
                {
                    startBase = OneTree;
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
