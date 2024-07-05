using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public abstract class LinearSolverInteger : ISolving<Polyhedron, Vector>
    {
        public Polyhedron? Domain { get; set; } = null;
        public Vector? CoDomain { get; set; } = null;
        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers) =>
            await SolveIntegerMaxAsync(loggers);
        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers) =>
            await SolveIntegerMinAsync(loggers);

        public abstract Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers);
        public abstract Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers);

        public abstract void SetData(Polyhedron domain, Vector codomain);
    }
}
