using Fractions;
using IronPython.Runtime;
using OperationalResearch.Extensions;
using OperationalResearch.Models;
using OperationalResearch.Models.Elements;

namespace OperationaResearchTest
{
    [TestClass]
    public class KnapsnackTest
    {
        [TestMethod]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            null,
            new int[] { 3, 1, 0, 0, 0, 0 }, 
            183,
            false)]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            Knapsnack.OrderCriteria.ByValueVolumeRatio,
            new int[] { 1, 1, 1, 0, 0, 0 },
            129,
            true)]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            null,
            new int[] { 1, 1, 0, 1, 0, 0 },
            139,
            true)]
        public async Task LowerBoundTest(
            int[] revenues, 
            int[] volumes, 
            int maxVolume, 
            Knapsnack.OrderCriteria? orderCriteria,
            int[] solution, 
            int value, 
            bool boolean
        ) {
            Assert.AreEqual(revenues.Length, volumes.Length);
            
            Knapsnack k = new(
                volume: maxVolume,
                volumes: volumes.Select(i => new Fraction(i)).ToArray(),
                values: revenues.Select(i => new Fraction(i)).ToArray());

            var result = 
                orderCriteria is null ? 
                await k.LowerBound(Boolean: boolean) :
                (await k.LowerBoundBy(orderCriteria.Value, Boolean: boolean))?.ToInt().ToArray();
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(
                solution, result, 
                $"{Function.Print(solution, false)} != {Function.Print(result, false)}");

            var gain = k.Gain(result);
            Assert.AreEqual(value, gain);
        }

        [TestMethod]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            "1 1 1 8/22 0 0",
            150,
            true)]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            "39/10 0 0 0 0 0",
            202,
            false)]
        public async Task UpperBoundTest(
            int[] revenues,
            int[] volumes,
            int maxVolume,
            string x,
            int value,
            bool boolean
        ) {
            Assert.AreEqual(revenues.Length, volumes.Length);

            Vector solution = x
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(Fraction.FromString)
                .ToArray();

            Knapsnack k = new(
                volume: maxVolume,
                volumes: volumes.Select(i => new Fraction(i)).ToArray(),
                values: revenues.Select(i => new Fraction(i)).ToArray(),
                weight: 0,
                weights: Vector.Zeros(revenues.Length));

            var result = await k.UpperBound(Boolean: boolean);
            CollectionAssert.AreEqual(solution.Get, result?.Get);

            var gain = k.Gain(result);
            Assert.IsNotNull(gain);
            Assert.AreEqual(new Fraction(value), gain.Value.Floor());
        }

        [TestMethod]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            new int[] { 1, 1, 0, 1, 0, 0 },
            139,
            true)]
        public async Task SolveTest(
            int[] revenues,
            int[] volumes,
            int maxVolume,
            int[] solution,
            int value,
            bool boolean
        ) {
            Assert.AreEqual(revenues.Length, volumes.Length);

            Knapsnack k = new(
                volume: maxVolume,
                volumes: volumes.Select(i => new Fraction(i)).ToArray(),
                values: revenues.Select(i => new Fraction(i)).ToArray());

            var result = await k.Solve(null, Boolean: boolean);
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(solution, result);

            var gain = k.Gain(result);
            Assert.AreEqual(value, gain);
        }
    }
}