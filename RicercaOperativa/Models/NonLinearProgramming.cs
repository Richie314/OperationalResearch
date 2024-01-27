using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython;

namespace OperationalResearch.Models
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
        public async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> streams)
        {
            ProjectedGradient s1 = new(A, b, python);
            FrankWolfe s2 = new(A, b, python);
            var results = await Task.WhenAll(
                new[] {
                    s1.SolveMinFlow(streams.FirstOrDefault(), startingPoint),
                    s2.SolveMinFlow(streams.ElementAtOrDefault(1), startingPoint)
                });
            return results.Any(x => x); // At least one succeded
        }
        public async Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> streams)
        {
            ProjectedGradient s1 = new(A, b, python);
            FrankWolfe s2 = new(A, b, python);
            var results = await Task.WhenAll(
                new[] { 
                    s1.SolveMaxFlow(streams.FirstOrDefault(), startingPoint), 
                    s2.SolveMaxFlow(streams.ElementAtOrDefault(1), startingPoint)
                });
            return results.Any(x => x); // At least one succeded
        }
    }
}
