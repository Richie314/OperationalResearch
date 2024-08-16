using Accord.Math;
using Fractions;
using OperationalResearch.Models.Elements;
using OperationalResearch.Extensions;
using Vector = OperationalResearch.Models.Elements.Vector;
using System.Xml;

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
                    .Where(c => c.Length > 1);
                if (!Cycles.Any())
                {
                    throw new DataMisalignedException("No cycle found but there should be at least one");
                }
                foreach (var cycle in Cycles)
                {
                    await Writer.WriteLineAsync(
                        $"T U {{ {Edges.ElementAt(pq)} }} has cycle: {string.Join("→", cycle.Select(i => i + 1))}");
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

                List<EdgeType> oppositeDirection = [];
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

                oppositeDirection = [.. oppositeDirection.Order()];
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
            IEnumerable<EdgeType>? startBase, 
            int? startNode, IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            startNode ??= 0;
            if (startBase is null)
            {
                await Writer.WriteLineAsync($"Using {startNode.Value + 1}-tree as start base");
                var sTree = await FindKTree(startNode.Value, true, null);
                if (sTree is not null)
                {
                    startBase = sTree;
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
            List<Fraction> x = [];
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
        
        public async Task<Vector?> SolveBounded(
            IEnumerable<EdgeType>? startBasis,
            IEnumerable<EdgeType>? startU, IndentWriter? Writer = null)
        {
            if (startBasis is null)
            {
                return null;
            }
            startU ??= [];
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync(
                $"Edges = {{ {string.Join(", ", Edges.Select(e => e.ToString()))} }}");
            await Writer.WriteLineAsync($"u = {u}");

            await Writer.WriteLineAsync($"E = {E}");
            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();

            var T = GetEdgeIndices(startBasis).ToArray();
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
                await Writer.WriteLineAsync($"x_T = {xt}");

                var x = BuildX(T, U);
                await Writer.WriteLineAsync($"Full X = {x}");
                if (T.Any(i => x[i].IsZero || x[i] == u[i]))
                {
                    var degIndexes = T
                        .Where(i => x[i].IsZero || x[i] == u[i]).
                        Select(i =>
                        {
                            var e = Edges.ElementAt(i);
                            return $"x[{e.From + 1},{e.To + 1}] = {Function.Print(x[i])}";
                        });
                    await Writer.Indent.Brown.WriteLineAsync(
                        $"x is degenerate: {string.Join("; ", degIndexes)}");
                } else
                {
                    await Writer.Indent.Purple.WriteLineAsync(
                        $"x is NOT degenerate: x_i ≠ 0 ∧ x_i ≠ u_i ∀ i ∈ T");
                }

                var π = BasisPotential(T);
                await Writer.WriteLineAsync($"π = {π}");

                Vector cReduced = c.Indices.Select(
                    idx => c[idx] + π[Edges.ElementAt(idx).From] - π[Edges.ElementAt(idx).To]).ToArray();
                await Writer.WriteLineAsync($"c^π = {cReduced}");
                await Writer.WriteLineAsync($"c^π_L = {cReduced[L]}");
                await Writer.WriteLineAsync($"c^π_U = {cReduced[U]}");

                if (cReduced.ZeroIndexes.Any(i => U.Contains(i) || L.Contains(i)))
                {
                    var degIndexes = cReduced.ZeroIndexes
                        .Where(i => U.Contains(i) || L.Contains(i)).
                        Select(i =>
                        {
                            var e = Edges.ElementAt(i);
                            return $"c^π[{e.From + 1},{e.To + 1}]";
                        });
                    await Writer.Indent.Brown.WriteLineAsync(
                        $"π is degenerate: {string.Join(" = ", degIndexes)} = 0");
                }
                else
                {
                    await Writer.Indent.Purple.WriteLineAsync(
                        $"π is NOT degenerate: c^π_i ≠ 0 ∀ (i,j) : (i,j) ∈ L V (i,j) ∈ U");
                }

                if (Enumerable.Range(0, Edges.Count()).All(i =>
                    {
                        if (x[i] == Fraction.Zero)
                        {
                            return cReduced[i] >= Fraction.Zero;
                        }
                        if (x[i] == u[i])
                        {
                            return cReduced[i] <= Fraction.Zero;
                        }
                        return cReduced[i].IsZero;
                    }))
                {
                    await Writer.Green.WriteLineAsync($"Flow is optimal");
                    return x;
                }

                var pq =
                    L.Where(i => cReduced[i].IsNegative).Concat(
                        U.Where(i => cReduced[i].IsPositive))
                    .OrderBy(i => Edges.ElementAt(i)).First();
                await Writer.Blue.WriteLineAsync($"Entering edge (p, q) = {Edges.ElementAt(pq)}");

                var Cycles = new Graph<EdgeType>(tEdges.Append(Edges.ElementAt(pq)))
                    .AllBidirectionalCycles()
                    .Where(c => c.Length > 1)
                    .Where(c => c.First() == c.Min());
                if (!Cycles.Any())
                {
                    throw new DataMisalignedException("No cycle found but there should be at least one");
                }
                foreach (var cycle in Cycles)
                {
                    await Writer.Indent.Purple.WriteLineAsync(
                        $"T U {{ {Edges.ElementAt(pq)} }} has cycle: {string.Join("→", cycle.Select(i => i + 1))}");
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

                await Writer.Blue.WriteLineAsync(
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

                oppositeDirection = [.. oppositeDirection.Order()];
                await Writer.WriteLineAsync(
                    $"C^- = {{ {string.Join(", ", oppositeDirection.Select(e => e.ToString()))} }}");
                currentDirection = [.. currentDirection.Order()];
                await Writer.WriteLineAsync(
                    $"C^+ = {{ {string.Join(", ", currentDirection.Select(e => e.ToString()))} }}");


                Fraction ThetaMinus = oppositeDirection
                    .Select(GetEdgeIndex)
                    .Select(i => x[i]).Min();
                await Writer.WriteLineAsync($"θ^- = {Function.Print(ThetaMinus)}");

                Fraction ThetaPlus = currentDirection
                    .Select(GetEdgeIndex)
                    .Select(i => u[i] - x[i]).Min();
                await Writer.WriteLineAsync($"θ^+ = {Function.Print(ThetaPlus)}");

                Fraction θ = ThetaMinus > ThetaPlus ? ThetaPlus : ThetaMinus;
                await Writer.WriteLineAsync($"θ = {Function.Print(θ)}");

                var rs =
                    oppositeDirection.Where(e => x[GetEdgeIndex(e)] == θ).Concat(
                    currentDirection.Where(e =>
                        u[GetEdgeIndex(e)] - x[GetEdgeIndex(e)] == θ))
                    .First();
                await Writer.Blue.WriteLineAsync($"Exiting edge (r, s) = {rs}");

                // Update T, U, L
                if (L.Contains(pq))
                {
                    if (currentDirection.Any(e => e == rs))
                    {
                        if (Edges.ElementAt(pq) != rs)
                        {
                            T = T.Where(i => i != GetEdgeIndex(rs)).Append(pq).ToArray();
                        }
                        U = [.. U, GetEdgeIndex(rs)];
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
            IEnumerable<EdgeType>? startBasis,
            IEnumerable<EdgeType>? startU, 
            int? startNode = null,
            int? endNode = null,
            IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            startNode = startNode ?? 0;
            endNode = endNode ?? N - 1;
            if (startBasis is null)
            {
                await Writer.WriteLineAsync($"Using {startNode.Value + 1}-tree as start base");
                var sTree = await FindKTree(startNode.Value, true, null);
                if (sTree is not null)
                {
                    startBasis = sTree;
                }
            }
            bool solved = false;
            // Flow of min cost
            try
            {
                var sol = await SolveBounded(startBasis, startU, Writer);
                if (sol is null)
                {
                    await Writer.Red.WriteLineAsync("Solution is null");
                    solved = false;
                } else
                {
                    await Writer.Green.Bold.WriteLineAsync($"Solution is {sol}");
                    solved = true;
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"Exception happened '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
            }

            // Min-cut and max-flow
            try
            {
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();

                var minflowmaxcut = await FordFulkerson(
                    startNode.Value, endNode.Value, Writer.Indent);

                if (minflowmaxcut is null)
                {
                    await Writer.WriteLineAsync(
                        $"Could not calculate min-cut from {startNode.Value} to {endNode.Value}");
                } else
                {
                    IEnumerable<int> Ns = minflowmaxcut.Item1, Nt = minflowmaxcut.Item2;
                    Fraction capacity = minflowmaxcut.Item3;
                    Vector x = minflowmaxcut.Item4;

                    await Writer.Bold.WriteLineAsync(
                        $"N_s = N_{startNode.Value + 1} = {Function.Print(Ns)}");
                    await Writer.Indent.WriteLineAsync(
                        $"N_t = N_{endNode.Value + 1} = {Function.Print(Nt)}");
                    await Writer.Indent.WriteLineAsync(
                        $"u(N_{startNode.Value + 1}, N_{endNode.Value + 1}) = {Function.Print(capacity)}");

                    await Writer.Bold.Green.WriteLineAsync($"Max flow x = {x}");
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"Exception happened '{ex.Message}'");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    await Writer.Indent.Orange.WriteLineAsync(ex.StackTrace);
                }
            }


            // Minimum paths tree
            try
            {
                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
                var dijkstra = await Dijkstra(Writer.Indent, startNode: startNode.Value);
                if (dijkstra is null)
                {
                    await Writer.Red.WriteLineAsync(
                        $"Could not calculate Minimum paths tree from {startNode.Value + 1}");
                } else
                {
                    await Writer.WriteLineAsync($"Final p = {Function.Print(dijkstra.p)}");
                    await Writer.WriteLineAsync($"Final π = {dijkstra.π}");
                    try
                    {
                        var g = dijkstra.Get;
                        await Writer.Green.WriteLineAsync(
                            $"Minimum paths {startNode.Value + 1}-tree = {g.Item1}");
                        await Writer.Green.WriteLineAsync(
                            $"Flow in {startNode.Value + 1}-tree = {g.Item2}");
                    }
                    catch (Exception ex) {
                        await Writer.Orange.WriteLineAsync(
                            $"Exception during reconstruction of graph from vector of predecessors:");
                        await Writer.Indent.Orange.WriteLineAsync(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                await Writer.Red.WriteLineAsync($"Exception happened '{ex.Message}'");
            }
            return solved;
        }

    }
}
