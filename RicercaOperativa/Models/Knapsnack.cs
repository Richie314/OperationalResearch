using Accord.Math;
using Fractions;
using Microsoft.Scripting.Utils;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using Vector = OperationalResearch.Models.Elements.Vector;
using Matrix = OperationalResearch.Models.Elements.Matrix;

namespace OperationalResearch.Models
{
    internal class Knapsnack
    {
        public class Item
        {
            public string Label { get; set; }
            public Fraction Value { get; set; }
            public Fraction Weight { get; set; }
            public Fraction Volume { get; set; }
            public Item(Fraction value, Fraction weight, Fraction volume, string label)
            {
                ArgumentNullException.ThrowIfNull(value, nameof(value));
                ArgumentNullException.ThrowIfNull(weight, nameof(weight));
                ArgumentNullException.ThrowIfNull(volume, nameof(volume));
                ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

                if (value.IsNegative)
                {
                    throw new ArgumentException("value can't be negative");
                }
                if (weight.IsNegative)
                {
                    throw new ArgumentException("weight can't be negative");
                }
                if (volume.IsNegative)
                {
                    throw new ArgumentException("volume can't be negative");
                }

                Value = value;
                Weight = weight;
                Volume = volume;
                Label = label;
            }
        }
        private readonly Fraction TotalVolume, TotalWeight;
        private readonly Item[] Items;
        public int N { get => Items.Length; }
        public Vector Values { get => Items.Select(x => x.Value).ToArray(); }
        public Vector Weights { get => Items.Select(x => x.Weight).ToArray(); }
        public Vector Volumes { get => Items.Select(x => x.Volume).ToArray(); }
        public IEnumerable<string> Labels { get => Items.Select(x => x.Label); }
        public bool UsesWeights { get => !Weights.IsZero && TotalWeight.IsPositive; }
        public Knapsnack(
            Fraction volume, Fraction weight,
            Vector weights, Vector values, Vector volumes, string[]? labels = null)
        {
            ArgumentNullException.ThrowIfNull(volume, nameof(volume));
            ArgumentNullException.ThrowIfNull(weight, nameof(weight));
            ArgumentNullException.ThrowIfNull(weights, nameof(weights));
            ArgumentNullException.ThrowIfNull(values, nameof(values));
            ArgumentOutOfRangeException.ThrowIfZero(weights.Size, nameof(weights));
            ArgumentOutOfRangeException.ThrowIfZero(values.Size, nameof(values));
            TotalVolume = volume;
            TotalWeight = weight;

            if (TotalVolume.IsNegative)
            {
                throw new ArgumentException("Max volume can't be negative");
            }
            if (TotalWeight.IsNegative)
            {
                throw new ArgumentException("Max weight can't be negative");
            }

            if (weights.Size != values.Size)
            {
                throw new ArgumentException(
                    $"Dimension of the weights vector must be equal to the one of the values vector ({weights.Size} != {values.Size})");
            }
            if (weights.Size != volumes.Size)
            {
                throw new ArgumentException(
                    $"Dimension of the weights vector must be equal to the one of the volumes vector ({weights.Size} != {volumes.Size})");
            }

            labels ??= weights.Indices.Select(i => (i + 1).ToString()).ToArray();
            if (weights.Size != labels.Length)
            {
                throw new ArgumentException(
                    $"Dimension of the weights vector must be equal to the one of the labels vector ({weights.Size} != {labels.Length})");
            }

            Items = weights.Indices.Select(i => new Item(values[i], weights[i], volumes[i], labels[i])).ToArray();
        }
        
        
        private Knapsnack GetSubProblem(int[] chosenValues)
        {
            if (chosenValues.Length == 0)
            {
                return this;
            }
            if (chosenValues.Length >= N)
            {
                throw new InvalidOperationException("No sub problem can be extracted");
            }
            Vector x = chosenValues.Select(xi => new Fraction(xi)).ToArray();
            IEnumerable<int> chosenRange = Enumerable.Range(0, chosenValues.Length);
            int unChosenLength = N - chosenValues.Length;
            var RemainingItems = Items.TakeLast(unChosenLength).ToArray();
            return new Knapsnack(
                volume: TotalVolume - x * Volumes[chosenRange],
                weight: TotalWeight - x * Weights[chosenRange],
                volumes: RemainingItems.Select(i => i.Volume).ToArray(),
                weights: RemainingItems.Select(i => i.Weight).ToArray(),
                values: RemainingItems.Select(i => i.Value).ToArray(),
                labels: RemainingItems.Select(i => i.Label).ToArray());
        }

        private Knapsnack GetSubProblem(Dictionary<int, int> setVariables)
        {
            if (setVariables.Count == 0)
            {
                return this;
            }
            if (setVariables.Count >= N)
            {
                throw new InvalidOperationException("No sub problem can be extracted");
            }

            Vector x = Vector.Zeros(N);
            foreach (KeyValuePair<int, int> kvp in setVariables)
            {
                x[kvp.Key] = new Fraction(kvp.Value);
            }
            Fraction usedVolume = x * Volumes, usedWeight = x * Weights;


            List<Item> RemainingItems = [];
            foreach (var i in Enumerable.Range(0, N))
            {
                if (setVariables.ContainsKey(i)) 
                    continue;
                RemainingItems.Add(Items[i]);
            }
            
            return new Knapsnack(
                volume: TotalVolume - usedVolume,
                weight: TotalWeight - usedWeight,
                volumes: RemainingItems.Select(i => i.Volume).ToArray(),
                weights: RemainingItems.Select(i => i.Weight).ToArray(),
                values: RemainingItems.Select(i => i.Value).ToArray(),
                labels: RemainingItems.Select(i => i.Label).ToArray());
        }

        private Knapsnack GetSubProblem(Dictionary<int, bool> setVariables) =>
            GetSubProblem(
                new Dictionary<int, int>(
                    setVariables.Select((v, u) => new KeyValuePair<int, int>(v.Key, v.Value ? 1 : 0))
                ));

        private static bool[] BuildSolution(
            Dictionary<int, bool> setVariables, IEnumerable<bool> remaining)
        {
            var v = Enumerable.Repeat(-1, setVariables.Count + remaining.Count()).ToArray();
            foreach (var kvp in setVariables)
            {
                v[kvp.Key] = kvp.Value ? 1 : 0;
            }
            foreach (var r in remaining)
            {
                var index = v.IndexOf(-1);
                v[index] = r ? 1 : 0;
            }
            if (v.IndexOf(-1) >= 0)
            {
                throw new DataMisalignedException("Not all variables were reconstructed");
            }
            return v.Select(i => i > 0).ToArray();
        }

        private static Vector BuildSolution(
            Dictionary<int, bool> setVariables, Vector remaining)
        {
            if (remaining.NegativeIndexes.Length != 0)
            {
                throw new ArgumentException("Cannot give negative varibles in input");
            }
            Vector v = Vector.Repeat(Fraction.MinusOne, setVariables.Count + remaining.Size);
            foreach (var kvp in setVariables)
            {
                v[kvp.Key] = kvp.Value ? Fraction.One : Fraction.Zero;
            }
            for (int i = 0; i < remaining.Size; i++)
            {
                var index = v.NegativeIndexes[0];
                v[index] = remaining[i];
            }
            if (v.NegativeIndexes.Length != 0)
            {
                throw new DataMisalignedException("Not all variables were reconstructed");
            }
            return v;
        }

        private string SolutionGain(Vector? x)
        {
            if (x is null || x.Size == 0)
            {
                return "?";
            }
            return Function.Print(x * Values);
        }
        private Fraction? Gain(Vector? x)
        {
            if (x is null || x.Size != N)
            {
                return null;
            }
            return Values * x;
        }
        private Fraction? Gain(int[]? x) => 
            Gain(x is null ? null : new Vector(x.Select(i => new Fraction(i)).ToArray()));
        private Fraction? Gain(bool[]? x) =>
            Gain(x?.Select(i => i ? 1 : 0).ToArray());
        public async Task<Vector?> UpperBound(
            IndentWriter? Writer = null, bool Boolean = false)
        {
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync("Finding upper bound of the problem through simplex");

            Matrix A = Volumes.Row;
            Vector b = new Fraction[] { TotalVolume };
            if (UsesWeights)
            {
                A = A.AddRow(Weights);
                b = b.Concat([TotalWeight]);
            }
            if (Boolean)
            {
                // Add x <= 1
                A = A.AddRows(Matrix.Identity(N));
                b = b.Concat(Vector.Ones(N));
            }
            await Writer.WriteLineAsync($"A|b = {A | b}");
            await Writer.WriteLineAsync($"c = {Values}");
            var p = new Polyhedron(A, b, forcePositive: true);
            var s = new Simplex(p, Values);
            return await s.SolvePrimalMax(IndentWriter.Null, null, 50);
        }


        public enum OrderCriteria
        {
            ByWeight,
            ByVolume,
            ByValue,
            ByValueWeightRatio,
            ByValueVolumeRatio,
        }
        private async Task<IEnumerable<int>> OrderByCriteria(IndentWriter Writer, OrderCriteria order)
        {
            switch (order)
            {
                case OrderCriteria.ByWeight:
                    await Writer.WriteLineAsync("Ordering products in ascending order by weight");
                    return Enumerable.Range(0, N).OrderBy(i => Items[i].Weight);
                case OrderCriteria.ByVolume:
                    await Writer.WriteLineAsync("Ordering products in ascending order by volume");
                    return Enumerable.Range(0, N).OrderBy(i => Items[i].Volume);
                case OrderCriteria.ByValue:
                    await Writer.WriteLineAsync("Ordering products in descending order by value");
                    return Enumerable.Range(0, N).OrderBy(i => Items[i].Value).Reverse();

                case OrderCriteria.ByValueWeightRatio:
                    await Writer.WriteLineAsync("Ordering products in descending order by value / weight ratio");
                    return Enumerable.Range(0, N).OrderBy(i => {
                        if (Items[i].Weight.IsZero)
                        {
                            return Fraction.PositiveInfinity;
                        }
                        return Items[i].Value / Items[i].Weight;
                    }).Reverse();
                case OrderCriteria.ByValueVolumeRatio:
                    await Writer.WriteLineAsync("Ordering products in descending order by value / volume ratio");
                    return Enumerable.Range(0, N).OrderBy(i => {
                        if (Items[i].Volume.IsZero)
                        {
                            return Fraction.PositiveInfinity;
                        }
                        return Items[i].Value / Items[i].Volume;
                    }).Reverse();
                default: throw new NotImplementedException();
            }
        }
        private async Task<Vector?> LowerBoundBy(OrderCriteria order,
            IndentWriter? Writer = null, bool Boolean = false)
        {
            Writer ??= IndentWriter.Null;
            var OrderedIndices = await OrderByCriteria(Writer, order);

            var labels = string.Join(", ", OrderedIndices.Select(i => Labels.ElementAt(i)));
            await Writer.WriteLineAsync($"Elements = {labels}");

            var vals = Values[OrderedIndices];
            await Writer.WriteLineAsync($"Values = {vals}");

            var w = Weights[OrderedIndices];
            await Writer.WriteLineAsync($"Weights = {w}");

            var v = Volumes[OrderedIndices];
            await Writer.WriteLineAsync($"Volumes = {v}");

            if (w[0] > TotalWeight || v[0] > TotalVolume)
            {
                // Cannot insert any element
                return null;
            }

            Fraction currW = Fraction.Zero, currV = Fraction.Zero;
            Vector x = Enumerable.Repeat(Fraction.Zero, N).ToArray();
            for (int i = 0; i < N; i++)
            {
                Fraction 
                    DeltaW = TotalWeight - currW, 
                    DeltaV = TotalVolume - currV;
                if (!DeltaW.IsPositive || !DeltaV.IsPositive)
                {
                    break;
                }
                Fraction maxOfThis = Boolean ? Fraction.One : Fraction.PositiveInfinity;
                Fraction maxW = !w[i].IsZero ? (DeltaW / w[i]).Floor() : Fraction.PositiveInfinity;
                Fraction maxV = !v[i].IsZero ? (DeltaV / v[i]).Floor() : Fraction.PositiveInfinity;
                Fraction numOfThis = Function.Min(maxOfThis, maxW, maxV);
                currW += w[i] * numOfThis;
                currV += v[i] * numOfThis;
                x[i] = numOfThis;
            }
            if (x.IsZero || x.Get.Contains(Fraction.PositiveInfinity))
            {
                return null;
            }
            return Enumerable.Range(0, N)
                .Select(i => x[OrderedIndices.ToArray().IndexOf(i)]).ToArray();
        }
        private async Task<int[]?> LowerBound(bool Boolean)
        {
            Vector min = Vector.Empty;
            Fraction minGain = Fraction.Zero;
            foreach (OrderCriteria criteria in Enum.GetValues<OrderCriteria>())
            {
                if (criteria == OrderCriteria.ByWeight || criteria == OrderCriteria.ByValueWeightRatio && !UsesWeights)
                {
                    continue;
                }
                var vec = await LowerBoundBy(criteria, null, Boolean);
                if (vec is null)
                {
                    continue;
                }
                Fraction? gain = Gain(vec);
                if (!gain.HasValue)
                {
                    continue;
                }
                if (min.Size == 0 || gain.Value > minGain)
                {
                    min = vec;
                    minGain = gain.Value;
                }
            }
            if (min.IsEmpty)
            {
                return null;
            }
            return min.ToInt().ToArray();
        }
        public async Task<int[]?> Solve(IndentWriter? Writer, bool Boolean = false)
        {
            Writer ??= IndentWriter.Null;
            await Writer.WriteLineAsync("Lower bounds: ");

            Vector ValueLB = await LowerBoundBy(OrderCriteria.ByValue, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By value: {ValueLB} -> {SolutionGain(ValueLB)}");

            if (UsesWeights)
            {
                Vector WeightLB = await LowerBoundBy(OrderCriteria.ByWeight, null, Boolean) ?? Vector.Empty;
                await Writer.WriteLineAsync($"- By weight: {WeightLB} -> {SolutionGain(WeightLB)}");
            }

            Vector VolumeLB = await LowerBoundBy(OrderCriteria.ByVolume, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By volume: {VolumeLB} -> {SolutionGain(VolumeLB)}");

            if (UsesWeights)
            {
                Vector Ratio1LB = await LowerBoundBy(OrderCriteria.ByValueWeightRatio, null, Boolean) ?? Vector.Empty;
                await Writer.WriteLineAsync($"- By value / weight: {Ratio1LB} -> {SolutionGain(Ratio1LB)}");
            }

            Vector Ratio2LB = await LowerBoundBy(OrderCriteria.ByValueVolumeRatio, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By value / volume: {Ratio2LB} -> {SolutionGain(Ratio2LB)}");

            await Writer.WriteLineAsync("Upper bound (simplex): ");
            Vector UB = await UpperBound(Writer, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"{UB} -> {SolutionGain(UB)}");
            
            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync();


            if (Boolean)
            {
                await Writer.WriteLineAsync("Solving with branch and bound from left to right:");
                var ltrSol = await BooleanBranchAndBoundLeftToRight([], Writer.Indent);
                
                await Writer.WriteLineAsync();

                await Writer.WriteLineAsync("Solving with branch and bound by setting fractionary variables:");
                var fracSol = await BooleanBranchAndBoundFractionary([], Fraction.Zero, Writer.Indent);

                if (ltrSol is null && fracSol is null)
                {
                    await Writer.WriteLineAsync("Branch and bound could not solve the problem");
                    return null;
                }

                if (ltrSol != fracSol)
                {
                    // Probably one method failed
                    await Writer.WriteLineAsync();
                    await Writer.WriteLineAsync("Solutions of BnB are different:");
                    Fraction? ltrGain = Gain(ltrSol), fracGain = Gain(fracSol);
                    Vector? ltrV = ltrSol is null ? null : new Vector(ltrSol.Select(i => i ? Fraction.One : Fraction.Zero).ToArray());
                    Vector? fracV = ltrSol is null ? null : new Vector(ltrSol.Select(i => i ? Fraction.One : Fraction.Zero).ToArray());

                    await Writer.Indent.WriteLineAsync(
                        $"- Left to Right: {(ltrSol is null ? "null" : ltrV)} -> {Function.Print(ltrGain)}");
                    await Writer.Indent.WriteLineAsync(
                        $"- Fractionary: {(fracSol is null ? "null" : fracV)} -> {Function.Print(fracGain)}");
                
                    if (ltrSol is null || !ltrGain.HasValue)
                    {
                        return fracSol?.Select(i => i ? 1 : 0).ToArray();
                    }
                    if (fracSol is null || !fracGain.HasValue)
                    {
                        return ltrSol?.Select(i => i ? 1 : 0).ToArray();
                    }
                    return (ltrGain.Value > fracGain.Value ? ltrSol : fracSol)
                        .Select(i => i ? 1 : 0).ToArray();
                }
                ltrSol ??= fracSol;
                return ltrSol?.Select(q => q ? 1 : 0).ToArray();
            }

            Fraction[] count = Vector.Zeros(N).Get, best = Vector.Zeros(N).Get;
            Fraction bestVal = Fraction.Zero;
            await Writer.WriteLineAsync("Recursive solver starting now...");
            int calls = RecursiveSolver(
                ref count, ref best, ref bestVal,
                0, Fraction.Zero, TotalWeight, TotalVolume);
            await Writer.WriteLineAsync($"{calls} recursions done.");
            Vector v = new(best);
            await Writer.WriteLineAsync($"Best solution = {v}");
            await Writer.WriteLineAsync($"Best solution gain = {Function.Print(bestVal)}");
            return v.ToInt().ToArray();
        }
        private int RecursiveSolver(
            ref Fraction[] count,
            ref Fraction[] best,
            ref Fraction best_val,
            int i,
            Fraction Value, Fraction Weight, Fraction Volume)
        {
            if (i == N)
            {
                if (Value > best_val)
                {
                    best_val = Value;
                    best = count.Copy();
                }
                return 1;
            }
            Fraction countW = Items[i].Weight.IsZero ?
                Fraction.PositiveInfinity : (int)(Weight / Items[i].Weight).Floor();
            Fraction countV = Items[i].Volume.IsZero ?
                Fraction.PositiveInfinity : (int)(Volume / Items[i].Volume).Floor();
            int callsDone = 0;
            for (count[i] = Function.Min(countW, countV); count[i] >= 0; count[i] = count[i] - Fraction.One)
            {
                callsDone += RecursiveSolver(
                ref count, ref best, ref best_val,
                    i + 1,
                    Value + count[i] * Items[i].Value,
                    Weight - count[i] * Items[i].Weight,
                    Volume - count[i] * Items[i].Volume);
            }
            return callsDone + 1;
        }
        private bool IsAcceptable(bool[] x)
        {
            if (x is null || x.Length != N)
                return false;
            Vector X = x.Select(xi => xi ? Fraction.One : Fraction.Zero).ToArray();
            return Weights * X <= TotalWeight && Volumes * X <= TotalVolume;
        }
        private bool IsAcceptable(int[] x)
        {
            if (x is null || x.Length != N)
                return false;
            Vector X = x.Select(xi => new Fraction(xi)).ToArray();
            return Weights * X <= TotalWeight && Volumes * X <= TotalVolume;
        }



        public async Task<bool[]?> BooleanBranchAndBoundLeftToRight(
            bool[] ChosenX,
            IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            if (ChosenX.Length == N)
            {
                if (IsAcceptable(ChosenX))
                {
                    await Writer.WriteLineAsync(
                        $"X = {Function.Print(ChosenX.Select(x => x ? 1 : 0), false)} -> {Function.Print(Gain(ChosenX))} is acceptable");
                    return ChosenX;
                }
                return null;
            }
            var X = "[ " + string.Join(", ", ChosenX.Select(x => x ? "1" : "0").Concat(Enumerable.Repeat("?", N - ChosenX.Length))) + " ]";

            try
            {
                var SubProblem = GetSubProblem(ChosenX.Select(x => x ? 1 : 0).ToArray());

                var lb = await SubProblem.LowerBound(true);
                if (lb is null)
                {
                    await Writer.WriteLineAsync($"Cutting X = {X} for missing lower bound");
                    return null;
                }

                var ub = await SubProblem.UpperBound(null, true);
                if (ub is null)
                {
                    await Writer.WriteLineAsync($"Cutting X = {X} for missing upper bound");
                    return null;
                }
                // now we have bounds
                if (SubProblem.Gain(ub) < SubProblem.Gain(lb))
                {
                    await Writer.WriteLineAsync(
                        $"Cutting X = {X} for lower bound > upper bound. {SubProblem.Gain(lb)} > {SubProblem.Gain(ub)}");
                    return null;
                }

                var Next0Res = await BooleanBranchAndBoundLeftToRight([.. ChosenX, false], Writer.Indent);
                var Next1Res = await BooleanBranchAndBoundLeftToRight([.. ChosenX, true], Writer.Indent);

                if (Next0Res is null)
                {
                    if (Next1Res is null)
                    {
                        // Both results are null -> exit
                        await Writer.WriteLineAsync(
                            $"Cutting X = {X} for being non-solving leaf");
                        return null;
                    }
                    return Next1Res;
                }
                if (Next1Res is null)
                {
                    return Next0Res;
                }
                if (Gain(Next0Res) > Gain(Next1Res))
                {
                    await Writer.WriteLineAsync($"Branch {X} -> {Function.Print(Gain(Next0Res))}");
                    return Next0Res;
                }
                await Writer.WriteLineAsync($"Branch {X} -> {Function.Print(Gain(Next1Res))}");
                return Next1Res;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public async Task<bool[]?> BooleanBranchAndBoundFractionary(
            Dictionary<int, bool> ChosenX,
            Fraction AlreadyGained,
            IndentWriter? Writer = null)
        {
            Writer ??= IndentWriter.Null;
            if (ChosenX.Count == N)
            {
                var xs = BuildSolution(ChosenX, []);
                if (IsAcceptable(xs))
                {
                    var g = Gain(xs);
                    var v = new Vector(xs.Select(x => x ? Fraction.One : Fraction.Zero));
                    await Writer.WriteLineAsync();
                    await Writer.WriteLineAsync(
                        $"X = {v} -> {Function.Print(g)} is acceptable");
                    return xs;
                }
                return null;
            }

            string[] xStr = Enumerable.Repeat("?", N).ToArray();
            foreach (KeyValuePair<int, bool> kvp in ChosenX)
            {
                xStr[kvp.Key] = kvp.Value ? "1" : "0";
            }
            var X = "[ " + string.Join(", ", xStr) + " ]";

            try
            {
                await Writer.WriteLineAsync($"X = {X}");
                Knapsnack SubProblem = GetSubProblem(ChosenX);

                var lb = await SubProblem.LowerBound(true);
                if (lb is null)
                {
                    await Writer.WriteLineAsync($"Cutting X for missing lower bound");
                    return null;
                }

                var ub = await SubProblem.UpperBound(null, true);
                if (ub is null)
                {
                    await Writer.WriteLineAsync($"Cutting X for missing upper bound");
                    return null;
                }

                Fraction 
                    iv = SubProblem.Gain(lb) ?? Fraction.Zero + AlreadyGained, 
                    sv = SubProblem.Gain(ub) ?? Fraction.Zero + AlreadyGained;
                iv = iv.Floor();
                sv = sv.Floor();

                await Writer.WriteLineAsync($"lb = {Function.Print(iv)}, ub = {Function.Print(sv)}");
                
                // now we have bounds
                if (iv >= sv)
                {
                    await Writer.WriteLineAsync(
                        $"Cutting X for lower bound >= upper bound.");
                    return null;
                }

                int[] fracIndex = BuildSolution(ChosenX, ub).FractionaryIndeces;
                if (fracIndex.Length == 0)
                {
                    // Upper bound is optimal
                    await Writer.WriteLineAsync($"Upper bound is optimal solution! (has no fractionary part)");
                    return BuildSolution(ChosenX, ub.ToInt().Select(i => i > 0));
                }

                if (fracIndex.Length != 1)
                {
                    throw new Exception("There are multiple fractionary elements in upper bound");
                }

                int newIndex = fracIndex[0];
                var instantiatedVars = new Dictionary<int, bool>(ChosenX);
                
                instantiatedVars[newIndex] = false;
                var ubZero = await BooleanBranchAndBoundFractionary(
                    instantiatedVars, AlreadyGained, Writer.Indent);
                var ubZeroGain = Gain(ubZero);


                instantiatedVars[newIndex] = true;
                var ubOne = await BooleanBranchAndBoundFractionary(
                    instantiatedVars, AlreadyGained + Items[newIndex].Value, Writer.Indent);
                var ubOneGain = Gain(ubOne);

                return ubZeroGain < ubOneGain ? ubZero : ubOne;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }



        public async Task<bool> SolveFlow(IndentWriter? Writer, bool isBoolean = false)
        {
            Writer ??= IndentWriter.Null;
            bool exitValue = true;
            try
            {
                int[]? x = await Solve(Writer, isBoolean);
                if (x is null)
                {
                    exitValue = false;
                    await Writer.WriteLineAsync("Could not find solution to the problem");
                }
                else
                {
                    await Writer.WriteLineAsync($"Solution X = {Function.Print(x, false)}");
                    await Writer.WriteLineAsync($"Gain = {Function.Print(Gain(x))}");
                }
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                exitValue = false;
            }
            return exitValue;
        }
    }
}