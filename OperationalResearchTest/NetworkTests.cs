using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;
using OperationalResearch.Models;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.Graphs;

namespace OperationalResearchTest
{
    [TestClass]
    public class NetworkTests
    {
        /// <summary>
        /// Parses a string into an edge and tells if it is part of T or U
        /// </summary>
        /// <param name="edge">From-To-cost-capacity|T/U|X/</param>
        /// <returns></returns>
        private static Tuple<BoundedCostEdge, bool, bool> ParseTestString(string edge)
        {
            string[] parts = edge.Split('|');

            var edgeParts = parts[0].Split('-', StringSplitOptions.TrimEntries);
            Assert.AreEqual(4, edgeParts.Length);

            BoundedCostEdge e = new(
                from: edgeParts[0], 
                to: edgeParts[1],
                cost: edgeParts[2],
                lb: null,
                ub: edgeParts[3]);
            return new Tuple<BoundedCostEdge, bool, bool>(
                e, 
                parts.Length > 1 && parts[1] == "T",
                parts.Length > 1 && parts[1] == "U");
        }


        [TestMethod]
        [DataRow(
            new int[] {-5, -3, -6, 3, 7, 4},
            new string[]
            {
                "1-2- 9-11|T",
                "1-4- 8- 5| ",
                "1-5-10- 7|T",
                "2-3- 8- 7| ",
                "2-4- 9- 7|U",
                "3-4- 8-10|T",
                "4-5- 8-11|T",
                "4-6-10- 5| ",
                "5-6- 5- 6|T"
            }
        )]
        [DataRow(
            new int[] { -7, -5, -7, 2, 6, 4, 7 },
            new string[]
            {
                "1-2- 6- 6| ",
                "1-3- 8- 7| ",
                "1-4- 8- 9|T",
                "2-3- 5- 5|T",
                "2-5- 6- 7| ",
                "3-5- 4- 7|U",
                "3-7- 6-11|T",
                "4-3- 6- 9|T",
                "4-6- 3- 4|T",
                "5-7- 4-11|T",
                "6-7- 7- 7| "
            }
        )]
        [DataRow(
            new int[] { -5, -5, -5, 3, 5, 7 },
            new string[]
            {
                "1-2- 4-10| ",
                "1-3- 7- 5|T",
                "2-4- 8-12|U",
                "2-5- 9-10|T",
                "3-2- 7-10|T",
                "4-6- 6- 9|T",
                "5-3- 8- 6| ",
                "5-4- 9- 4| ",
                "6-5- 5-10|T"
            },
            null //"5 0 12 3 5 9 0 0 2"
        )]
        [DataRow(
            new int[] { -3, -5, -6, 6, 6, 3, -1 },
            new string[]
            {
                "1-2- 7- 8| ",
                "1-3- 9- 5|T",
                "2-4- 3- 5|U",
                "2-5- 9-11|T",
                "3-2-10- 9|T",
                "3-5- 4- 6| ",
                "4-6- 3-11|T",
                "5-4- 6- 4|T",
                "5-7- 9- 8| ",
                "6-5- 6-10| ",
                "7-6- 6- 7|T"
            },
            null //"3 0 5 9 6 0 2 3 0 0 1"
        )]
        public async Task MinimumCostFlow(
            int[] b,
            string[] edgeStrings,
            string? finalX = null
        ) {
            // Input checks
            Assert.IsTrue(b.Length > 1);

            var edges = edgeStrings.Select(ParseTestString);

            var g = new MinimumCostFlow<BoundedCostEdge>(
                edges.Select(e => e.Item1),
                b.Select(i => new Fraction(i)).ToArray());

            var result = await g.SolveBounded(
                startBasis: edges.Where(e => e.Item2).Select(e => e.Item1),
                startU: edges.Where(e => e.Item3).Select(e => e.Item1));
            Assert.IsNotNull(result);

            if (!string.IsNullOrWhiteSpace(finalX))
            {
                Vector solution = Vector.FromString(finalX.Split(' ')) ?? Vector.Empty;
                CollectionAssert.AreEqual(solution.Get, result.Get, $"{solution} != {result}");
            }
        }

        [DataRow(
            7,
            new string[]
            {
                "1-2- 7- 8",
                "1-3- 9- 5",
                "2-4- 3- 5",
                "2-5- 9-11",
                "3-2-10- 9",
                "3-5- 4- 6",
                "4-6- 3-11",
                "5-4- 6- 4",
                "5-7- 9- 8",
                "6-5- 6-10",
                "7-6- 6- 7"
            },
            1, 7,

            new int[] { 1, 2, 3, 4, 5, 6 },
            "8",
            "8 0 0 8 0 0 0 0 8 0 0"
        )]
        [DataRow(
            7,
            new string[]
            {
                "1-2- 9- 5",
                "1-3- 5-12",
                "1-4-10- 9",
                "2-4- 4-10",
                "3-5- 7-10",
                "4-3- 3- 8",
                "4-6- 7- 4",
                "5-4- 8- 6",
                "5-7- 4- 4",
                "6-5- 7- 9",
                "6-7- 7- 6",
            },
            1, 7,

            new int[] { 1, 2, 3, 4, 5 },
            null,
            "0 4 4 0 4 0 4 0 4 0 4"
        )]
        [DataRow(
            7,
            new string[]
            {
                "1-2-10- 5",
                "1-3-10-10",
                "1-4- 8- 6",
                "2-3- 8- 5",
                "2-5- 9- 7",
                "3-5- 6- 4",
                "3-7- 5- 6",
                "4-3- 6- 4",
                "4-6- 9- 4",
                "5-7- 9- 6",
                "6-7-10-11",
            },
            1, 7,

            new int[] { 1, 2, 3, 4, 5 },
            "16",
            "5 7 4 0 5 1 6 0 4 6 4"
        )]
        [TestMethod]
        public async Task MaxFlowMinCut(
            int N, 
            string[] edgeStrings,
            int s,
            int t,

            int[] Ns,
            string? capacity = null,
            string? x = null
        ) {
            s--;
            t--;
            Ns = Ns.Select(i => i - 1).ToArray();
            Assert.IsTrue(s >= 0  && s < N);
            Assert.IsTrue(t >= 0 && t < N);


            var edges = edgeStrings.Select(s => ParseTestString(s).Item1);
            Assert.IsTrue( edges.Any() );
            
            var g = new BoundedCostGraph<BoundedCostEdge>(N, edges);

            var result = await g.FordFulkerson(s, t);
            Assert.IsNotNull(result);

            CollectionAssert.Contains(result.Item1.ToArray(), s);
            CollectionAssert.Contains(result.Item2.ToArray(), t);

            CollectionAssert.AreEqual(Ns, result.Item1.ToArray(), 
                $"{Function.Print(Ns)} != {Function.Print(result.Item1)}");

            if (!string.IsNullOrWhiteSpace(capacity))
            {
                Assert.AreEqual(
                    Fraction.FromString(capacity), result.Item3,
                    $"{capacity} != {Function.Print(result.Item3)}");
            }

            if (!string.IsNullOrWhiteSpace(x))
            {
                Vector? flow = Vector.FromString(x.Split(' '));
                Assert.IsNotNull(flow);
                CollectionAssert.AreEqual(
                    flow.Get,
                    result.Item4.Get,
                    $"{flow} != {result.Item4}");
            }
        }

        [DataRow(
            new string[]
            {
                "1-2- 7- 8",
                "1-3- 9- 5",
                "2-4- 3- 5",
                "2-5- 9-11",
                "3-2-10- 9",
                "3-5- 4- 6",
                "4-6- 3-11",
                "5-4- 6- 4",
                "5-7- 9- 8",
                "6-5- 6-10",
                "7-6- 6- 7"
            },
            1,
            "3 3 2 0 0 2 1 0 1 0 0"
        )]
        [DataRow(
            new string[]
            {
                "1-2- 9- 5",
                "1-3- 5-12",
                "1-4-10- 9",
                "2-4- 4-10",
                "3-5- 7-10",
                "4-3- 3- 8",
                "4-6- 7- 4",
                "5-4- 8- 6",
                "5-7- 4- 4",
                "6-5- 7- 9",
                "6-7- 7- 6",
            },
            1,
            "1 3 2 0 2 0 1 0 1 0 0"
        )]
        [TestMethod]
        public async Task Dijkstra(
            string[] edgeStrings,
            int r,
            string x
        ) {
            r--;
            Assert.IsTrue(r >= 0);

            var edges = edgeStrings.Select(s => ParseTestString(s).Item1);
            Assert.IsTrue(edges.Any());

            var g = new CostGraph<BoundedCostEdge>(edges);
            Assert.IsTrue(g.N > r);

            var result = await g.Dijkstra(Writer: null, startNode: r, maxIterations: 15);
            Assert.IsNotNull(result);

            Vector? solution = Vector.FromString(x.Split(' '));
            Assert.IsNotNull(solution);

            CollectionAssert.AreEqual(
                solution.Get, 
                result.Get.Item2.Get,
                $"{solution} != {result.Get.Item2} ({result.Get.Item1})");
        }
    }
}
