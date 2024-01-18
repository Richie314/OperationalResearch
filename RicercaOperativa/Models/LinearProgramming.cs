using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RicercaOperativa.Models
{
    internal class LinearProgramming : IProgrammingInterface
    {
        private Fraction[,] A;
        private Fraction[] b;
        private Fraction[] c;
        private readonly int[]? startBase;
        public async Task<bool> SolveAsync(StreamWriter? outStream = null)
        {
            Simplex s = new Simplex(A, b, c);
            return await s.SolvePrimalFlow(startBase, outStream);
        }
        public LinearProgramming(int[]? startBase = null)
        {
            this.startBase = startBase;
            A = new Fraction[0, 0];
            b = new Fraction[0];
            c = new Fraction[0];
        }
        public void SetMainMatrix(Fraction[,] matrix)
        {
            A = matrix;
        }
        public void SetFirstVector(Fraction[] v)
        {
            b = v;
        }
        public void SetSecondVector(Fraction[] v)
        {
            c = v;
        }
    }
}
