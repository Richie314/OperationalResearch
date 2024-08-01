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
        [TestMethod]
        public async Task Test(
            string m, 
            string startNode, string k, 
            string bnb,

            string lb, string ub,
            string nearestNode,
            
            string bestPath,
            string finalCost)
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
            Assert.IsNotNull(tree);
            
            var treeCost = tree.TotalCost();
            Assert.AreEqual(lb, treeCost.ToString());


            // nearest node testing

            var nn = await p.Solver.Domain.NearestNodeUpperEstimate(
                IndentWriter.Null, int.Parse(startNode) - 1);
            Assert.IsNotNull(nn);

            string nnString = string.Join('-', nn.SkipLast(1).Select(i => i + 1));
            Assert.AreEqual(nearestNode, nnString);

            var nnCost = p.Solver.Domain.Cost(nn, true);
            Assert.AreEqual(ub, nnCost.ToString());


            // resolution testing

            var bnbEdges = p.Solver.Domain.BnBToEdges(bnb);
            Assert.IsNotNull(bnbEdges);
            Assert.AreNotEqual(0, bnbEdges.Count());

            var path = await TSP<CostEdge>.BranchAndBound(
                IndentWriter.Null,
                costs.Length,
                p.Solver.Domain.Edges,
                int.Parse(k) - 1,
                nnCost,
                bnbEdges);
            Assert.IsNotNull(path);
            Assert.IsNotNull(path.Item2);

            var pathStr = string.Join('-', path.Item2.VisitedNodes(int.Parse(startNode) - 1, true).Select(i => i + 1));
            Assert.IsTrue(bestPath == pathStr || bestPath == pathStr.Reverse());

            var cost = path.Item1.ToString();
            Assert.AreEqual(finalCost, cost);
        }
    }
}
