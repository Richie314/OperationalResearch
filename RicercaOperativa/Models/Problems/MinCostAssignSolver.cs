using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{

    internal class MinCostAssignSolver(bool isCooperative = false) : IntegerLinearProgramming
    {
        private readonly bool isCooperative = isCooperative;

        public override Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }
        public override async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            MinCostAssign Solver = new(c);
            return isCooperative ?
                await Solver.SolveCooperativeFlow(loggers.FirstOrDefault()) :
                await Solver.SolveNonCooperativeFlow(loggers.FirstOrDefault());
        }

    }
    internal class MinCostGenericAssignSolver : IntegerLinearProgramming
    {
        public override Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }
        public override async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            MinCostAssign Solver = new(c: c, w: w, b: b.Get, wxEqualsB: false);
            return await Solver.SolveNonCooperativeFlow(loggers.FirstOrDefault());
        }

    }
}
