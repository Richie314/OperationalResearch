using System;
using System.Collections.Generic;
using Accord.Math;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Graphs;
using OperationalResearch.Models.Problems;

namespace OperationaResearchTest
{
    [TestClass]
    public class TspTest
    {
        [DataRow(
            "-16-17-24-23|" +
            "-  -21-13-32|" +
            "-  -  -20-30|" +
            "-  -  -  -11|",
            "3", "4",
            "4-5, 2-4, 1-2",

            "80", "87",
            "3-1-2-4-5",

            "3-2-4-5-1-3",
            "85"
        )]
        [DataRow(
            "-40-18-30-35|" +
            "-  -27-36-32|" +
            "-  -  -17-28|" +
            "-  -  -  -38|",
            "4", "3",
            "3-4, 1-3, 1-4",

            "132", "138",
            "4-3-1-5-2"
        )]
        [DataRow(
            "-16-15-27-19|" +
            "-  -25-20-17|" +
            "-  -  -14-12|" +
            "-  -  -  -13|",
            "1", "4",
            "3-4, 3-5, 4-5",

            "70", "76",
            "1-3-5-4-2",

            "1-2-5-4-3-1",
            "75"
        )]
        [DataRow(
            "-14-16-34-18|" +
            "-  -20-35-21|" +
            "-  -  -22-19|" +
            "-  -  -  -17|",
            "3", "5",
            "1-2, 1-4, 2-4",

            "87", "90",
            "3-1-2-5-4"
        )]
        [DataRow(
            "-23-18-17-21|" +
            "-  -22-16-16|" +
            "-  -  -20-19|" +
            "-  -  -  -14|",
            "2", "2",
            "2-4, 2-5, 4-5",

            "81", "90",
            "2-4-5-3-1",

            "2-4-1-3-5-2",
            "86"
        )]
        [DataRow(
            "-29-24-67-47|" +
            "-  -18-94-61|" +
            "-  -  -23-26|" +
            "-  -  -  -20|",
            "2", "2",
            "3-4, 2-4, 4-5",

            "114", "137",
            "2-3-4-5-1"
        )]
        [DataRow(
            "-30-35-32-25|" +
            "-  -28-33-26|" +
            "-  -  -24-16|" +
            "-  -  -  -12|",
            "4", "5",
            "3-4, 3-5, 4-5",

            "110", "118",
            "4-5-3-2-1"
        )]
        [DataRow(
            "-20-24-21-32|" +
            "-  -17-30-19|" +
            "-  -  -22-18|" +
            "-  -  -  -25|",
            "1", "5",
            "2-3, 2-4, 2-5",

            "95", "101",
            "1-2-3-5-4",

            "1-2-5-3-4-1",
            "100"
        )]
        [DataRow(
            "-14-16-34-18|" +
            "-  -20-35-21|" +
            "-  -  -22-19|" +
            "-  -  -  -17|",
            "3", "5",
            "1-2, 1-4, 4-5",

            "87", "90",
            "3-1-2-5-4"
        )]
        [DataRow(
            "-26-20-24-19|" +
            "-  -34-23-22|" +
            "-  -  -27-21|" +
            "-  -  -  -32|",
            "3", "4",
            "3-4, 4-5",

            "108", "111",
            "3-1-5-2-4"

            // "3-5-2-4-1-3", // Solution is known but can't be found with BnB after two levels
            // "110"
        )]
        [TestMethod]
        public async Task Test(
            string m, 
            string startNode, string k, 
            string bnb,

            string lb, string ub,
            string nearestNode,
            
            string? bestPath = null,
            string? finalCost = null)
        {
            string[][] costs = m
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Split('-', StringSplitOptions.None).Select(i =>
                {
                    if (string.IsNullOrWhiteSpace(i))
                    { 
                        return "0";
                    }
                    return i;
                }).ToArray())
                .ToArray();
            costs = costs.Append(Enumerable.Repeat("0", costs[0].Length).ToArray()).ToArray();

            TravellingSalesManProblem p = new(
                costs,
                startNode, k, 
                bnb, true);

            Assert.IsNotNull(p.Solver.Domain);


            // k-tree testing

            var tree = await p.Solver.Domain.FindKTree(int.Parse(k) - 1, true);
            Assert.IsNotNull(tree, $"Could not find {k}-tree");
            
            var treeCost = tree.TotalCost();
            Assert.AreEqual(lb, treeCost.ToString());


            // nearest node testing

            var nn = await p.Solver.Domain.NearestNodeUpperEstimate(
                IndentWriter.Null, int.Parse(startNode) - 1);
            Assert.IsNotNull(nn, $"Could not solve for nearest from {startNode}");

            string nnString = string.Join('-', nn.SkipLast(1).Select(i => i + 1));
            Assert.AreEqual(nearestNode, nnString);

            var nnCost = p.Solver.Domain.Cost(nn, true);
            Assert.AreEqual(ub, nnCost.ToString());


            // resolution testing

            var bnbEdges = p.Solver.Domain.BnBToEdges(bnb);
            Assert.IsNotNull(bnbEdges, "Error in parsing Branch & Bound edges");
            Assert.AreNotEqual(0, bnbEdges.Count());

            var path = await TSP<CostEdge>.BranchAndBound(
                IndentWriter.Null,
                costs.Length,
                p.Solver.Domain.Edges,
                int.Parse(k) - 1,
                nnCost,
                bnbEdges);
            Assert.IsNotNull(path);
            if (string.IsNullOrWhiteSpace(bestPath))
            {
                Assert.IsNull(path.Item2, "No solution was expected but one was, apparently, found");
            } else
            {
                Assert.IsNotNull(path.Item2, "No path was found");
                var pathStr = string.Join('-', path.Item2.VisitedNodes(int.Parse(startNode) - 1, true).Select(i => i + 1));
                Assert.IsTrue(
                    bestPath == pathStr || bestPath == pathStr.Reverse(), 
                    $"Path {pathStr} does not match the desired {bestPath}");
            }

            if (!string.IsNullOrWhiteSpace(finalCost))
            {
                var cost = path.Item1.ToString();
                Assert.AreEqual(finalCost, cost);
            }
        }
    }
}
