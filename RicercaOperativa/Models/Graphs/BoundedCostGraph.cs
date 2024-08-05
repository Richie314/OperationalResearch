using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.Models.Graphs
{
    public class BoundedCostGraph<EdgeType> : CostGraph<EdgeType> where EdgeType : BoundedCostEdge
    {
        protected readonly Matrix E;
        public BoundedCostGraph(int n, IEnumerable<EdgeType>? edges) :
            base(n, edges)
        {
            E = new Matrix(n, Edges.Count());
            for (int edgeIndex = 0; edgeIndex < Edges.Count(); edgeIndex++)
            {
                var edge = Edges.ElementAt(edgeIndex);
                E[edge.From, edgeIndex] = Fraction.MinusOne;
                E[edge.To, edgeIndex] = Fraction.One;
            }
            E = E[E.RowsIndeces.Where(i => i != 0)]; // skip first row
        }
        public Vector u { get => Edges.Select(e => e.ub).ToArray(); }
        public Vector l { get => Edges.Select(e => e.lb).ToArray(); }
        public Vector c { get => Edges.Select(e => e.Cost).ToArray(); }

        

        public IEnumerable<EdgeType> BuildResidueGraph(Vector x)
        {
            List<EdgeType> r = [];
            for (int i = 0; i < Edges.Count(); i++)
            {
                EdgeType edge = Edges.ElementAt(i);
                if (x[i] < edge.ub)
                {
                    EdgeType clone = (EdgeType)edge.Clone();
                    clone.ub = edge.ub - x[i];
                    r.Add(clone);
                }
                if (x[i].IsPositive)
                {
                    EdgeType rev = (EdgeType)edge.Clone();
                    rev.From = edge.To;
                    rev.To = edge.From;
                    rev.ub = x[i];
                    r.Add(rev);
                }
            }
            return r.ToArray().Order();
        }

        public async Task<Tuple<
            IEnumerable<int>, 
            IEnumerable<int>,
            Fraction,
            Vector>?> 
            FordFulkerson(
            int s, 
            int t,
            IndentWriter? Writer = null,
            int? maxIterations = null
        ) {
            Writer ??= IndentWriter.Null;
            Vector x = Vector.Zeros(Edges.Count());
            int iteration = 1;
            while (!maxIterations.HasValue || iteration <= maxIterations.Value)
            {
                await Writer.Bold.WriteLineAsync($"FordFurkelson #{iteration}");
                await Writer.WriteLineAsync($"x = {x}");
                
                var G = new Graph<EdgeType>(N, BuildResidueGraph(x));
                await Writer.WriteLineAsync($"G(x) = {G}");

                var C = await EdmondsKarp(s, t, G, Writer.Indent, 20);
                if (C is null || !C.Any())
                {
                    await Writer.WriteLineAsync("No increasing path was found in G(x)");
                    await Writer.Green.WriteLineAsync("x is a maxinum flow");

                    // Build the cut
                    List<int> Ns = [s], Nt = [t];
                    foreach (int i in Enumerable.Range(0, N))
                    {
                        if (i == s || i == t)
                            continue;
                        var paths = G.AllOreintedPaths(s, i);
                        if (paths.Any())
                        {
                            await Writer.Indent.Blue.WriteLineAsync(
                                $"Path from {s + 1} to {i + 1}: {string.Join('-', paths.First().Select(j => j + 1))}");
                            Ns.Add(i);
                        } else
                        {
                            Nt.Add(i);
                        }
                    }
                    var APlusEdges = Edges.Where(e => Ns.Contains(e.From) && Nt.Contains(e.To)).ToArray();
                    
                    if (APlusEdges.Length == 0)
                    {
                        throw new DataMisalignedException($"No edges were found from N_{s + 1} to N_{t + 1}");
                    }
                    await Writer.WriteLineAsync(
                        $"A^+ = {{ {string.Join(", ", APlusEdges.Select(e => e.ToString()))} }}");

                    
                    Fraction cutCapacity = new Vector(
                        APlusEdges.Select(e => e.ub).ToArray()).SumOfComponents();

                    await Writer.WriteLineAsync($"u(N_{s + 1}, N_{t + 1}) = {Function.Print(cutCapacity)}");

                    return new Tuple<IEnumerable<int>, IEnumerable<int>, Fraction, Vector>(
                        Ns, Nt, cutCapacity, x);
                }

                var δ = C.Select(rij => rij.ub).Min();
                await Writer.WriteLineAsync($"δ = {Function.Print(δ)}");

                // Update x with new flow δ
                for (int i = 0; i < x.Size; i++)
                {
                    if (C.Any(e => e == Edges.ElementAt(i)))
                    {
                        x[i] = x[i] + δ;
                    }
                    if (C.Any(e => e == Edges.ElementAt(i).Reversed))
                    {
                        x[i] = x[i] - δ;
                    }
                }
            }

            await Writer.Orange.WriteLineAsync($"Limit of {iteration} reached");
            return null;

        }

        public async Task<IEnumerable<EdgeType>?> EdmondsKarp(
            int s, int t,
            Graph<EdgeType> G,
            IndentWriter? Writer = null,
            int? maxIterations = null
        ) {
            const int NO_PREDECESSOR = -2;
            Writer ??= IndentWriter.Null;

            var p = Enumerable.Repeat(NO_PREDECESSOR, N).ToArray();
            p[s] = 0;

            Queue<int> Q = new Queue<int>([s]);

            while (Q.TryDequeue(out int i))
            {
                await Writer.Bold.WriteLineAsync($"Analizing node {i + 1}");

                await Writer.Indent.WriteLineAsync($"p = {Function.Print(p)}");

                if (G.Edges.Any(e => e.From == i && e.To == t))
                {
                    await Writer.Indent.Green.WriteLineAsync(
                        $"Found {new Edge(i, t)} inside A(x)");
                    p[t] = i;


                    List<int> path = new();
                    for (int j = t; j != s; j = p[j])
                    {
                        if (p[j] == NO_PREDECESSOR) 
                            continue;
                        path.Add(j);
                    }
                    path.Add(s);
                    path.Reverse();

                    await Writer.Indent.WriteLineAsync($"path: {string.Join('-', path.Select(k => k + 1))}");

                    return G.GetEdges(path, false);
                }

                foreach (var e in G.Edges)
                {
                    if (e.From != i) continue;
                    if (p[e.To] == NO_PREDECESSOR)
                    {
                        p[e.To] = e.From;
                        Q.Enqueue(e.To);
                    }
                }
            }

            await Writer.Orange.WriteLineAsync("Q is empty and an increasing path was not found");
            await Writer.WriteLineAsync("A cut may possibly be found:");

            var Ns = NodeList.Where(i => p[i] != NO_PREDECESSOR);
            var Nt = NodeList.Where(i => p[i] == NO_PREDECESSOR);

            await Writer.Indent.WriteLineAsync($"N_{s + 1} = {Function.Print(Ns)}");
            await Writer.Indent.WriteLineAsync($"N_{t + 1} = {Function.Print(Nt)}");

            return null;
        }
    }
}
