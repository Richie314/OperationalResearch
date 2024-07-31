using Fractions;
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
            new int[] { 3, 1, 0, 0, 0, 0 }, 
            183,
            false)]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            new int[] { 1, 1, 1, 0, 0, 0 },
            129,
            true)]
        public async Task LowerBoundTest(
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
                values: revenues.Select(i => new Fraction(i)).ToArray(),
                weight: 0,
                weights: Vector.Zeros(revenues.Length));

            var result = await k.LowerBound(Boolean: boolean);
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(solution, result);

            var gain = k.Gain(result);
            Assert.AreEqual(value, gain);
        }

        [TestMethod]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            new int[] {1, 1, 1, 8, 0, 0},
            new int[] {1, 1, 1, 22, 1, 1},
            150,
            true)]
        [DataRow(
            new int[] { 52, 27, 50, 60, 31, 11 },
            new int[] { 10, 6, 15, 22, 17, 14 },
            39,
            new int[] { 39, 0, 0, 0, 0, 0 },
            new int[] { 10, 1, 1, 1, 1, 1 },
            202,
            false)]
        public async Task UpperBoundTest(
            int[] revenues,
            int[] volumes,
            int maxVolume,
            int[] numerators,
            int[] denominators,
            int value,
            bool boolean
        ) {
            Assert.AreEqual(revenues.Length, volumes.Length);
            Assert.AreEqual(numerators.Length, denominators.Length);

            Vector solution = Enumerable.Range(0, numerators.Length)
                .Select(i => new Fraction(numerators[i], denominators[i])).ToArray();

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
                values: revenues.Select(i => new Fraction(i)).ToArray(),
                weight: 0,
                weights: Vector.Zeros(revenues.Length));

            var result = await k.Solve(null, Boolean: boolean);
            Assert.IsNotNull(result);
            Assert.AreEqual(solution, result);

            var gain = k.Gain(result);
            Assert.AreEqual(value, gain);
        }

    }
}