using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    internal class IntegerLinearProgrammingPrimal(bool xNonNegative = true) : IProgrammingInterface
    {
        private Fraction[,] A = new Fraction[0, 0];
        private Vector b = Array.Empty<Fraction>();
        private Vector c = Array.Empty<Fraction>();
        private readonly bool addXNonNegativeConstraint = xNonNegative;
        public async Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            GoogleIntegerLinearProgramming s = new(A, b, c, addXNonNegativeConstraint);
            return await s.SolveMaxFlow(loggers.FirstOrDefault());
        }
        public async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            GoogleIntegerLinearProgramming s = new(A, b, c, addXNonNegativeConstraint);
            return await s.SolveMinFlow(loggers.FirstOrDefault());
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
