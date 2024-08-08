using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationalResearch.Models;
using OperationalResearch.Models.NonLinearOptimization.QuadProg;
using OperationalResearch.Models.Elements;

namespace OperationalResearchTest
{
    [TestClass]
    public class QuadProgTest
    {
        [DataRow(
            new string[]
            {
                "0 2",
                "2 -8"
            },
            "-8 4",
            new string[]
            {
                "-3 2 <= 6",
                "-3 -2 <= 6",
                "3 4 <= 12",
                "3 -4 <= 12",
            },
            
            new string[]
            {
                "4 0 | 0 0 -1/6 17/6",
            }
        )]
        [DataRow(
            new string[]
            {
                "4 -4",
                "-4 8"
            },
            "-10 -8",
            new string[]
            {
                "0 -1 <= -1",
                "-4 -1 <= -1",
                "1 0 <= 4",
                "2 5 <= 23",
            },

            new string[]
            {
                "-1 5 | 0 -121/9 0 -89/9",
                "4 3 | 0 0 6 0",
            }
        )]
        [DataRow(
            new string[]
            {
                "2 0",
                "0 2"
            },
            "-8 -6",
            new string[]
            {
                "-1 -1 <= -1",
                "0 1 >= 0",
                "1 -1 <= 2",
                "1 1 <= 4",
                "0 1 <= 2",
                "-1 1 <= 1"
            },

            new string[]
            {
                "0 1 | -6 0 0 0 0 -2",
                "5/2 3/2 | 0 0 0 3 0 0",
            }
        )]
        [TestMethod]
        public void LKKT(
            string[] H, 
            string lin, 
            string[] p,

            string[] points
        ) {
            var P = Polyhedron.FromStringMatrix(
                p.Select(r => r.Split(' ')).ToArray());
            var Q = new QuadProg(
                H: new Matrix(H.Select(r => r.Split(' ')).ToArray()),
                linearPart: Vector.FromString(lin.Split(' ')) ?? Vector.Empty,
                p: P
            );
            Assert.IsTrue(Q.IsValid);

            var solutions = Q.SolveLKKT().ToArray();
            Assert.IsNotNull(solutions);
            CollectionAssert.AllItemsAreNotNull(solutions);
            Assert.IsTrue(solutions.Any());

            foreach (var point in points)
            {
                var x = Vector.FromString(
                    point.Split("|", StringSplitOptions.TrimEntries)[0].Split(' '));
                Assert.IsNotNull(x);

                var λ = Vector.FromString(
                    point.Split("|", StringSplitOptions.TrimEntries)[1].Split(' '));
                Assert.IsNotNull(λ);

                Assert.IsTrue(
                    solutions.Any(s => s.Item1 == x && s.Item2 == λ),
                    $"Point x = {x}; λ = {λ} not found!");
            }
        }
    }
}
