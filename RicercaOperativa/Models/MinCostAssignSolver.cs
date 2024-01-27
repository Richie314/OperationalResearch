using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{

    internal class MinCostAssignSolver(bool isCooperative = false) : IProgrammingInterface
    {
        private Fraction[,] c = new Fraction[0, 0];
        private readonly bool isCooperative = isCooperative;

        public async Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }
        public async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            MinCostAssign Solver = new(c);
            return isCooperative ?
                await Solver.SolveCooperativeFlow(loggers.FirstOrDefault()) :
                await Solver.SolveNonCooperativeFlow(loggers.FirstOrDefault());
        }

        public void SetMainMatrix(Fraction[,] matrix)
        {
            c = matrix;
        }
        public void SetFirstVector(Vector v)
        {
        }
        public void SetSecondVector(Vector v)
        {
        }
    }
}
