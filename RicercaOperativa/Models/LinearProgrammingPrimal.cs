using Fractions;
using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    internal class LinearProgrammingPrimal(int[]? startBase = null, bool xNonNegative = true) : IProgrammingInterface
    {
        private Fraction[,] A = new Fraction[0, 0];
        private Vector b = Array.Empty<Fraction>();
        private Vector c = Array.Empty<Fraction>();
        private readonly int[]? startBase = startBase;
        private readonly bool addXNonNegativeConstraint = xNonNegative;
        public async Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            Simplex s = new(A, b, c, addXNonNegativeConstraint);
            return await s.SolvePrimalMaxFlow(startBase, loggers.FirstOrDefault());
        }
        public async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            Simplex s = new(A, b, c * (-1), addXNonNegativeConstraint);
            return await s.SolvePrimalMaxFlow(startBase, loggers.FirstOrDefault());
        }

        public void SetMainMatrix(Fraction[,] matrix)
        {
            A = matrix;
        }
        public void SetFirstVector(Vector v)
        {
            b = v;
        }
        public void SetSecondVector(Vector v)
        {
            c = v;
        }
    }
}
