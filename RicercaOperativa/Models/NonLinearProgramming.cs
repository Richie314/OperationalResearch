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
        public async Task<bool> SolveAsync(IEnumerable<StreamWriter?> streams)
        {
            ProjectedGradient s1 = new(A, b, python);
            FrankWolfe s2 = new(A, b, python);
            bool res1 = await s1.SolveFlow(streams.FirstOrDefault(), startingPoint);
            bool res2 = await s2.SolveFlow(streams.ElementAtOrDefault(1), startingPoint);
            return res1 || res2;
        }
    }
}
