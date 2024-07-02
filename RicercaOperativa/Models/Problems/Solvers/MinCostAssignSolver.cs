using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class MinCostAssignSolver(bool isCooperative = false) : LinearSolverInteger
    {
        private readonly bool isCooperative = isCooperative;
        MinCostAssign? solver = null;

        public override Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Only min problems can be solved!");
        }
        public override async Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            //MinCostAssign Solver = new(c);
            if (solver is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return isCooperative ?
                await solver.SolveCooperativeFlow(loggers.FirstOrDefault()) :
                await solver.SolveNonCooperativeFlow(loggers.FirstOrDefault());
        }
        public override void SetData(Polyhedron domain, Vector codomain)
        {
            throw new NotImplementedException();    
        }
    }
    public class MinCostGenericAssignSolver : LinearSolverInteger
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
