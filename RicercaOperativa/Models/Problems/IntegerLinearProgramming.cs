using Accord.Math;
using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    abstract class IntegerLinearProgramming : IProgrammingInterface
    {
        protected Fraction[,] c = new Fraction[0, 0];
        protected Fraction[,] w = new Fraction[0, 0];
        protected Vector b = new();
        public abstract Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers);
        public abstract Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers);

        public void SetMainMatrix(Fraction[,] matrix)
        {
            c = matrix;
        }
        
        public void SetFirstVector(Vector v)
        {
            w = new Matrix(v.Get.Split(c.Columns())).T.M;
        }
        public void SetSecondVector(Vector v)
        {
            b = v;
        }
    }
}
