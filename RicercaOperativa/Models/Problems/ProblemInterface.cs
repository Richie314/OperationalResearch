using OperationalResearch.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    public interface IProblem
    {
        public Task<bool> SolveMin(IEnumerable<IndentWriter?> loggers);
        public Task<bool> SolveMax(IEnumerable<IndentWriter?> loggers);

        public Task<bool> SolveIntegerMin(IEnumerable<IndentWriter?> loggers);
        public Task<bool> SolveIntegerMax(IEnumerable<IndentWriter?> loggers);
    }
}
