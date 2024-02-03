using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    internal class KnapsnackProblemSolver(bool isBoolean = false) : IntegerLinearProgramming
    {
        private readonly bool IsBoolean = isBoolean;
        public override async Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> loggers)
        {
            Knapsnack k = new(
                volume: b[0], 
                weight: b[1], 
                values: w.GetRow(0), 
                volumes: w.GetRow(1),
                weights: w.GetRow(2));
            return await k.SolveFlow(loggers.FirstOrDefault(), IsBoolean);
        }
        public override Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> loggers)
        {
            throw new NotImplementedException("Only max problems can be solved!");
        }
    }
}
