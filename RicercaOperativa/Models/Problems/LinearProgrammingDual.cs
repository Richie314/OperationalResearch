using Fractions;
using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    internal class LinearProgrammingDual(int[]? startBase = null, bool yPos = true) : IProgrammingInterface
    {
        private Fraction[,] A = new Fraction[0, 0];
        private Vector b = Array.Empty<Fraction>();
        private Vector c = Array.Empty<Fraction>();
        private readonly int[]? startBase = startBase;
        private readonly bool yPos = yPos;
        public async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            Simplex s = new(A, b, c, yPos);
            return await s.SolveDualMinFlow(startBase, loggers.FirstOrDefault());
        }
        public async Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            Simplex s = new(A, b * -1, c, yPos);
            return await s.SolveDualMinFlow(startBase, loggers.FirstOrDefault());
        }

        public void SetMainMatrix(Fraction[,] matrix)
        {
            A = new Matrix(matrix).T.M;
        }
        public void SetFirstVector(Vector v)
        {
            c = v;
        }
        public void SetSecondVector(Vector v)
        {
            b = v;
        }
    }
}
