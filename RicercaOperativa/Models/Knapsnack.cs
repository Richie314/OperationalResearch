using Accord.Math;
using Fractions;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using Microsoft.Scripting.Utils;

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
            public Item(Fraction value,  Fraction weight, Fraction volume, string label)
            {
                ArgumentNullException.ThrowIfNull(value, nameof(value));
                ArgumentNullException.ThrowIfNull(weight, nameof(weight));
                ArgumentNullException.ThrowIfNull(volume, nameof(volume));
                ArgumentNullException.ThrowIfNullOrWhiteSpace(label, nameof(label));

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
        public Vector Values {  get => Items.Select(x => x.Value).ToArray(); }
        public Vector Weights { get => Items.Select(x =>x.Weight).ToArray(); }
        public Vector Volumes { get => Items.Select(x => x.Volume).ToArray(); }
        public IEnumerable<string> Labels { get => Items.Select(x => x.Label); }
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

            if (weights.Size !=  values.Size)
            {
                throw new ArgumentException(
                    $"Dimension of the weights vector must be equal to the one of the values vector ({weights.Size} != {values.Size})");
            }
            if (weights.Size != volumes.Size)
            {
                throw new ArgumentException(
                    $"Dimension of the weights vector must be equal to the one of the volumes vector ({weights.Size} != {volumes.Size})");
            }

            if (labels is null)
            {
                labels = weights.Indices.Select(i => (i + 1).ToString()).ToArray();
            }
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
        private Fraction? Gain(int[]? x)
        {
            if (x is null || x.Length != N)
            {
                return null;
            }
            return Values * x.Select(q => new Fraction(q)).ToArray();
        }
        private Fraction? Gain(bool[]? x)
        {
            if (x is null || x.Length != N)
            {
                return null;
            }
            return Values * x.Select(q => q ? Fraction.One : Fraction.Zero).ToArray();
        }
        public async Task<Vector?> UpperBound(
            StreamWriter? Writer = null, bool Boolean = false)
        {
            Writer ??= StreamWriter.Null;
            await Writer.WriteLineAsync("Finding upper bound of the problem through simplex");

            Fraction[,] A = Weights.Row.M.InsertRow(Volumes.Get);
            if (Boolean)
            {
                var id = Matrix.Identity(N);
                foreach (int i in id.RowsIndeces)
                {
                    A = A.InsertRow(id[i].Get);
                }
            }
            Vector b = new Fraction[] { TotalWeight, TotalVolume };
            if (Boolean)
            {
                b = b.Get.Concat(Enumerable.Repeat(Fraction.One, N)).ToArray();
            }
            await Writer.WriteLineAsync($"A|b = {new Matrix(A) | b}");
            await Writer.WriteLineAsync($"c = {Values}");
            Simplex s = new(A, b, Values, true);
            return await s.SolvePrimalMax(StreamWriter.Null, null, 50);
        }
        public enum OrderCriteria
        {
            ByWeight,
            ByVolume,
            ByValue,
            ByValueWeightRatio,
            ByValueVolumeRatio,
        }
        private async Task<IEnumerable<int>> OrderByCriteria(StreamWriter Writer, OrderCriteria order)
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
                            return new Fraction(int.MaxValue);
                        }
                        return Items[i].Value / Items[i].Weight;
                    }).Reverse();
                case OrderCriteria.ByValueVolumeRatio:
                    await Writer.WriteLineAsync("Ordering products in descending order by value / volume ratio");
                    return Enumerable.Range(0, N).OrderBy(i => {
                        if (Items[i].Volume.IsZero)
                        {
                            return new Fraction(int.MaxValue);
                        }
                        return Items[i].Value / Items[i].Volume;
                    }).Reverse();
                default: throw new NotImplementedException();
            }
        }
        private async Task<Vector?> LowerBoundBy(OrderCriteria order,
            StreamWriter? Writer = null, bool Boolean = false)
        {
            Writer ??= StreamWriter.Null;
            var OrderedIndices = await OrderByCriteria(Writer, order);

            var vals = Values[OrderedIndices];
            await Writer.WriteLineAsync($"Values = {vals}");

            var w = Weights[OrderedIndices];
            await Writer.WriteLineAsync($"Weights = {w}");

            var v = Volumes[OrderedIndices];
            await Writer.WriteLineAsync($"Volumes = {v}");

            if (w[0] > TotalVolume)
            {
                // Cannot insert any element
                return null;
            }

            Fraction currW = Fraction.Zero, currV = Fraction.Zero;
            List<int> x = [];
            for (int i = 0; i < N; i++)
            {
                Fraction DeltaW = TotalWeight - currW, DeltaV = TotalVolume - currV;
                if (!DeltaW.IsPositive || !DeltaV.IsPositive)
                {
                    break;
                }
                int maxOfThis = Boolean ? 1 : int.MaxValue;
                int maxW = w[i].IsZero ? (int)(DeltaW / w[i]) : int.MaxValue;
                int maxV = v[i].IsZero ? (int)(DeltaV / v[i]) : int.MaxValue;
                int numOfThis = Math.Min(maxOfThis, Math.Min(maxW, maxV));
                currW += w[i] * numOfThis;
                currV += v[i] * numOfThis;
                x.Add(numOfThis);
            }
            if (!x.Any() || x.Contains(int.MaxValue))
            {
                return null;
            }
            Vector xToReorder = x.Select(xi => new Fraction(xi)).ToArray();
            return Enumerable.Range(0, N)
                .Select(i => xToReorder[OrderedIndices.ElementAt(i)]).ToArray();
        }
        private async Task<int[]?> LowerBound(bool Boolean)
        {
            Vector min = Vector.Empty;
            Fraction minGain = Fraction.Zero;
            foreach (OrderCriteria criteria in Enum.GetValues<OrderCriteria>())
            {
                var vec = await LowerBoundBy(criteria, null, Boolean);
                if (vec is not null)
                {
                    Fraction? gain = Gain(vec);
                    if (gain.HasValue)
                    {
                        if (min.Size == 0 || gain.Value > minGain)
                        {
                            min = vec;
                            minGain = gain.Value;
                        }
                    }
                }
            }
            return min.Get.Select(i => (int)i).ToArray();
        }
        public async Task<int[]?> Solve(StreamWriter? Writer, bool Boolean = false)
        {
            Writer ??= StreamWriter.Null;
            await Writer.WriteLineAsync("Lower bounds: ");

            Vector ValueLB = await LowerBoundBy(OrderCriteria.ByValue, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By value: {ValueLB} -> {SolutionGain(ValueLB)}");

            Vector WeightLB = await LowerBoundBy(OrderCriteria.ByWeight, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By weight: {WeightLB} -> {SolutionGain(WeightLB)}");

            Vector VolumeLB = await LowerBoundBy(OrderCriteria.ByVolume, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By volume: {VolumeLB} -> {SolutionGain(VolumeLB)}");

            Vector Ratio1LB = await LowerBoundBy(OrderCriteria.ByValueWeightRatio, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By value / weight: {Ratio1LB} -> {SolutionGain(Ratio1LB)}");

            Vector Ratio2LB = await LowerBoundBy(OrderCriteria.ByValueWeightRatio, null, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"- By value / volume: {Ratio2LB} -> {SolutionGain(Ratio2LB)}");

            await Writer.WriteLineAsync("Upper bound (simplex): ");
            Vector UB = await UpperBound(Writer, Boolean) ?? Vector.Empty;
            await Writer.WriteLineAsync($"{UB} -> {SolutionGain(UB)}");

            if (Boolean)
            {
                // Do Branch and bound
                return null;
            }

            int[] count = new int[N], best = new int[N];
            Fraction bestVal = Fraction.Zero;
            await Writer.WriteLineAsync("Recursive solver starting now...");
            RecursiveSolver(ref count, ref best, ref bestVal, 0, Fraction.Zero, TotalWeight, TotalVolume);
            await Writer.WriteLineAsync($"Best solution indices = {Function.Print(best)}");
            await Writer.WriteLineAsync($"Best solution gain = {Function.Print(bestVal)}");
            return best;
        }
        private void RecursiveSolver(
            ref int[] count, 
            ref int[] best, 
            ref Fraction best_val, 
            int i, 
            Fraction Value, Fraction Weight, Fraction Volume)
        {
            if (i == N)
            {
                if (Value > best_val)
                {
                    best_val = Value;
                    best = count;
                }
                return;
            }
            int countW = Items[i].Weight.IsZero ? int.MaxValue : (int)(Weight / Items[i].Weight);
            int countV = Items[i].Volume.IsZero ? int.MaxValue : (int)(Volume / Items[i].Volume);
            count[i] = Math.Min(countW, countV);
            while (count[i] >= 0)
            {
                RecursiveSolver(
                ref count, ref best, ref best_val,
                    i + 1,
                    Value + count[i] * Items[i].Value,
                    Weight - count[i] * Items[i].Weight,
                    Volume - count[i] * Items[i].Volume);
                count[i]--;
            }
        }
        public async Task<bool[]?> BooleanBranchAndBound(
            bool[] ChosenX,
            StreamWriter? Writer = null)
        {
            if (ChosenX.Length == N)
            {
                return ChosenX;
            }
            Writer ??= StreamWriter.Null;
            var X = ChosenX.Select(x => x ? 1 : 0).ToArray();
            var SubProblem = GetSubProblem(X);
            
            var lb = await SubProblem.LowerBound(true);
            if (lb is null)
            {
                await Writer.WriteLineAsync($"Cutting X = {X} ... for missing lower bound");
                return null;
            }

            var ub = await SubProblem.UpperBound(null, true);
            if (ub is null)
            {
                await Writer.WriteLineAsync($"Cutting X = {X} ... for missing upper bound");
                return null;
            }
            // now we have bounds
            if (SubProblem.Gain(ub) <= SubProblem.Gain(lb))
            {
                await Writer.WriteLineAsync($"Cutting X = {X} ... for lower bound > upper bound");
                return null;
            }

            var Next0 = ChosenX.Append(false).ToArray();
            var Next0Res = await BooleanBranchAndBound(Next0, Writer);

            var Next1 = ChosenX.Append(true).ToArray();
            var Next1Res = await BooleanBranchAndBound(Next1, Writer);

            if (Next0 is null)
            {
                if (Next1 is null)
                {
                    // Both results are null -> exit
                    await Writer.WriteLineAsync($"Cutting X = {X} ... for being non solving leaf");
                    return null;
                }
                return Next1;
            }
            if (Next1 is null)
            {
                return Next0;
            }
            return Gain(Next0Res) > Gain(Next1Res) ? Next0 : Next1;
        }

        public async Task<bool> SolveFlow(StreamWriter? Writer, bool isBoolean = false)
        {
            Writer ??= StreamWriter.Null;
            bool exitValue = true;
            try
            {
                int[]? x = await Solve(Writer, isBoolean);
                if (x is null)
                {
                    exitValue = false;
                    await Writer.WriteLineAsync("Could not find solution to the problem");
                } else
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
