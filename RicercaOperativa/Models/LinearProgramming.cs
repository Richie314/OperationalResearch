using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RicercaOperativa.Models
{
    internal class LinearProgramming(int[]? startBase = null) : IProgrammingInterface
    {
        private Fraction[,] A = new Fraction[0, 0];
        private Vector b = Array.Empty<Fraction>();
        private Vector c = Array.Empty<Fraction>();
        private readonly int[]? startBase = startBase;
        public async Task<bool> SolveAsync(StreamWriter? outStream = null)
        {
            Simplex s = new(A, b, c);
            return await s.SolvePrimalFlow(startBase, outStream);
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
