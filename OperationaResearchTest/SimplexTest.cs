using System;
using System.Collections.Generic;
using Accord.Math;
using Fractions;
using OperationalResearch.Models;
using OperationalResearch.Models.Elements;
using Vector = OperationalResearch.Models.Elements.Vector;
using Matrix = OperationalResearch.Models.Elements.Matrix;
using OperationalResearch.Extensions;

namespace OperationaResearchTest
{
    [TestClass]
    public class SimplexTest
    {

        [TestMethod]
        [DataRow(
            "8 12",
            "7/10 1 |" + 
            "5/2 4 |" +
            "4 5/2 |" +
            "3/2 5/4 |" +
            "-1 0 |" +
            "0 -1 |",
            "15 48 50 25 -6 -6",
            true,
            new int[] {4, 5},
            "320/39 268/39"  
            )]
        [DataRow(
            "4 5 2",
            "0 3/5 4/5 |" +
            "-1 2 0 |" +
            "1 0 -1 |",
            "500 0 0",
            true,
            new int[] { 0, 3, 4 },
            "5000/11 2500/11 5000/11"
            )]
        [DataRow(
            "6 20 15",
            "-1 0 0 |" +
            "0 1 0 |" +
            "1/5 1/2 2/5 |" +
            "3/10 13/20 3/5 |" +
            "3/25 9/20 1/5 |",
            "-7 3 6 7 7",
            true,
            new int[] { 3, 6, 7 },
            "7 3 59/12"
            )]
        [DataRow(
            "250 300",
            "19/10 3/2 |" +
            "1/2 1 |" +
            "1 -1/2 |",
            "100 55 0",
            true,
            new int[] { 1, 3 },
            "350/23 1090/23"
            )]
        [DataRow(
            "100 80 60",
            "6 5 4 |" +
            "4 2 10 |" +
            "4 5 3 |" +
            "2 -1 0 |" +
            "-2/5 3/5 -2/5 |",
            "1000 800 500 0 0",
            true,
            new int[] { 1, 5, 6 },
            "650/29 1300/29 1800/29"
            )]
        [DataRow(
            "14 20 16",
            "1 3 2 |" +
            "1 2 4 |" +
            "3 1 1 |",
            "510 400 180",
            true,
            new int[] { 3, 4, 5 },
            "20/21 3250/21 470/21"
            )]
        [DataRow(
            "1200 1500",
            "3/2 3/2 |" +
            "4/5 1 |" +
            "1 11/5 |",
            "1500 2000 1800",
            true,
            new int[] { 0, 4 },
            "1000/3 2000/3"
            )]
        [DataRow(
            "-1 -1",
            "-100 -200 |" +
            "-500 -300 |",
            "-5000 -12000",
            true,
            new int[] { 0, 3 },
            "90/7 130/7"
            )]
        [DataRow(
            "-1 -3",
            "-1 -3 |" +
            "-2 -1 |",
            "-11 -9",
            true,
            new int[] { 1, 2 },
            "16/5 13/5"
            )]
        public async Task TestPrimal(
            string cStr,
            string AStr,
            string bStr,
            bool forcePos,
            
            int[] B,

            string sol)
        {

            Vector x = sol
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(Fraction.FromString)
                .ToArray();

            Vector b = bStr
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(Fraction.FromString)
                .ToArray();

            Vector c = cStr
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(Fraction.FromString)
                .ToArray();

            Matrix A = new(AStr
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Fraction.FromString)
                    .ToArray())
                .ToArray());

            var P = new Polyhedron(A, b, forcePos);
            var s = new Simplex(P, c);

            var result = await s.SolvePrimalMax(IndentWriter.Null, B, 30);
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(x.Get, result?.Get, $"{result} != {x}");
        }
    }
}
