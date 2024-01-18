using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;

namespace RicercaOperativa.Models
{
    public interface IProgrammingInterface
    {
        public Task<bool> SolveAsync(StreamWriter? logger);
        public void SetMainMatrix(Fraction[,] m);
        public void SetFirstVector(Fraction[] v);
        public void SetSecondVector(Fraction[] v);
    }
}
