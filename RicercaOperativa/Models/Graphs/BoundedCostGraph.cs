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
            E = new Matrix(n - 1, Edges.Count());
            for (int edgeIndex = 0; edgeIndex < Edges.Count(); edgeIndex++)
            {
                if (Edges.ElementAt(edgeIndex).From > 0)
                {
                    // skip first row
                    E[Edges.ElementAt(edgeIndex).From - 1, edgeIndex] = Fraction.MinusOne;
                }
                if (Edges.ElementAt(edgeIndex).To > 0)
                {
                    // skip first row
                    E[Edges.ElementAt(edgeIndex).To - 1, edgeIndex] = Fraction.One;
                }
            }
        }
        public Vector u { get => Edges.Select(e => e.ub).ToArray(); }
        public Vector l { get => Edges.Select(e => e.lb).ToArray(); }
        public Vector c { get => Edges.Select(e => e.Cost).ToArray(); }

        public async Task<bool> MinFlowMaxCut(int s, int t, IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;

            // Set xij = 0
            Vector x = Vector.Zero(Edges.Count());

            int it = 1;
            bool qEmpty = false;
            // E-K algorithm
            while (it < 10)
            {
                await Writer.WriteLineAsync($"Iteration #{it} of E-K starts now...");
                it++;

                // Build G(x)
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
                r = r.ToArray().Order().ToList();
                foreach (var rij in r)
                {
                    await Writer.WriteLineAsync($"\t{rij} with capacity = {rij.ub}");
                }

                // Predecessors
                var p = Enumerable.Repeat(-2, N).ToArray();
                if (t >= p.Length || s >= p.Length || s < 0 || t < 0)
                {
                    throw new ArgumentException("Destination or starting node not found!");
                }
                p[s] = -1;

                IEnumerable<int> Q = [s];

                while (Q.Any())
                {
                    await Writer.WriteLineAsync($"Q = {Function.Print(Q)}");
                    await Writer.WriteLineAsync($"X = {x}");
                    await Writer.WriteLineAsync($"p = {Function.Print(p)}");

                    // extract first element
                    int i = Q.First();
                    Q = Q.Skip(1);

                    await Writer.WriteLineAsync($"Extracting {i + 1} from Q");

                    if (r.Any(rij => rij.From == i && rij.To == t))
                    {
                        p[t] = i;
                        await Writer.WriteLineAsync($"{new Edge(i, t)} found inside A(x)");
                        await Writer.WriteLineAsync($"p_t = p_{t + 1} = {i + 1}");
                        await Writer.WriteLineAsync($"Exiting loop");
                        break;
                    }

                    foreach (var edge in r.Where(e =>
                        e.From == i &&
                        p[e.To] < 0/* &&
                        e.Cost < e.ub*/)) // Not filled edges
                    {
                        if (p[edge.To] == -1) continue;
                        await Writer.WriteLineAsync($"Analizing edge {edge}");

                        p[edge.To] = i;
                        await Writer.WriteLineAsync($"\tp_{edge.To + 1} = {i + 1}");

                        Q = Q.Append(edge.To);
                        await Writer.WriteLineAsync($"\tAdding {edge.To + 1} to Q");
                    }
                }

                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync("EK-iteration ended");
                await Writer.WriteLineAsync();


                await Writer.WriteLineAsync($"\tp = {Function.Print(p)}");
                await Writer.WriteLineAsync();


                // Find min
                int curr = t; // start from end
                Fraction min = int.MaxValue;
                IEnumerable<int> Path = [t];
                while (p[curr] >= 0)
                {
                    var rij = r.First(rij => rij.From == p[curr] && rij.To == curr);
                    if (rij is null)
                    {
                        throw new Exception("This should not happen :/");
                    }
                    if (rij.ub < min)
                    {
                        min = rij.ub;
                    }
                    curr = p[curr]; // Get predecessor
                    Path = Path.Prepend(curr);
                }

                await Writer.WriteLineAsync($"\tPath: {string.Join('-', Path.Select(i => i + 1))}");
                await Writer.WriteLineAsync($"\tMax flow that can be sent: {Function.Print(min)}");

                // Update X
                for (int i = Path.Count() - 1; i > 0; i--)
                {
                    // current edge
                    curr = Path.ElementAt(i);
                    int prev = Path.ElementAt(i - 1);

                    // find edge index to find which x modify
                    int edgeIndex = GetEdgeIndex(prev, curr);

                    x[edgeIndex] = min;
                }
                await Writer.WriteLineAsync($"\tX = {x}");

                if (Q.Any())
                {
                    // We were "interruped"
                    await Writer.WriteLineAsync($"\tQ = {Function.Print(Q)}");
                }

                // End of E-K algorithm

                if (!Q.Any())
                {
                    if (qEmpty)
                    {
                        await Writer.WriteLineAsync("Could not find a new flow, so the previous is optimal");


                        await Writer.WriteLineAsync("Cut:");
                        var Ns = Enumerable.Range(0, p.Length).Where(i => p[i] != -2);
                        await Writer.WriteLineAsync($"N_s = N_{s + 1} = {Function.Print(Ns)}");

                        var Nt = Enumerable.Range(0, p.Length).Where(i => p[i] == -2);
                        await Writer.WriteLineAsync($"N_t = N_{t + 1} = {Function.Print(Nt)}");

                        break;
                    }
                    qEmpty = true;


                }

                await Writer.WriteLineAsync();
                await Writer.WriteLineAsync();
            }
            return true;
        }

    }
}
