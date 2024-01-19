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
        private Vector b;
        private readonly string python;
        private readonly Vector? startingPoint;
        public NonLinearProgramming(string pythonString, string[]? startingPointStrings)
        {
            A = new Fraction[0, 0];
            b = Array.Empty<Fraction>();
            python = pythonString;
            ArgumentNullException.ThrowIfNullOrWhiteSpace(pythonString);
            if (startingPointStrings is null)
            {
                startingPoint = null;
            } else
            {
                try
                {
                    startingPoint = startingPointStrings.Select(Fraction.FromString).ToArray();
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
        public void SetFirstVector(Vector v)
        {
            b = v;
        }
        public void SetSecondVector(Vector v)
        {
            // Do nothing
        }
        public async Task<bool> SolveAsync(StreamWriter? outStream = null)
        {
            ProjectedGradient s = new(A, b, python);
            return await s.SolveFlow(outStream, startingPoint);
        }
    }
}
