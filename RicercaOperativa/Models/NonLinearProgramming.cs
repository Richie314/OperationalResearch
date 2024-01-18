using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython;

namespace RicercaOperativa.Models
{
    internal class NonLinearProgramming : IProgrammingInterface
    {
        private Fraction[,] A;
        private Fraction[] b;
        private string python;
        private Fraction[]? startingPoint;
        public NonLinearProgramming(string pythonString, string[]? startingPointStrings)
        {
            A = new Fraction[0, 0];
            b = new Fraction[0];
            python = pythonString;
            ArgumentNullException.ThrowIfNullOrWhiteSpace(pythonString);
            if (startingPointStrings is null)
            {
                startingPoint = null;
            } else
            {
                try
                {
                    startingPoint = startingPointStrings.Select(x => Fraction.FromString(x)).ToArray();
                }
                catch
                {
                    startingPoint = null;
                }
            }
            
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
            // Do nothing
        }
        public async Task<bool> SolveAsync(StreamWriter? outStream = null)
        {
            ProjectedGradient s = new ProjectedGradient(A, b, python);
            return await s.SolveFlow(outStream, startingPoint);
        }
    }
}
