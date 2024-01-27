using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;

namespace OperationalResearch.Models
{
    public interface IProgrammingInterface
    {
        public Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers);
        public Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers);
        public void SetMainMatrix(Fraction[,] m);
        public void SetFirstVector(Vector v);
        public void SetSecondVector(Vector v);
    }
}
