﻿using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.Models.Problems.Solvers
{
    public class QuadraticSolver : ISolving<Elements.Polyhedron, Tuple<Matrix, Vector>>
    {
        private QuadProg? quadProg = null;

        public Elements.Polyhedron? Domain { get; set; } = null;
        public Tuple<Matrix, Vector>? CoDomain { get; set; } = null;

        public async Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (quadProg is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await quadProg.SolveFlow(loggers.FirstOrDefault(), max: true);
        }
        public async Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            if (quadProg is null)
            {
                throw new InvalidOperationException("Problem not yet initialized");
            }
            return await quadProg.SolveFlow(loggers.FirstOrDefault(), max: false);
        }

        public Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Can't solve for integers");
        }
        public Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers)
        {
            throw new NotImplementedException("Can't solve for integers");
        }

        public void SetData(Elements.Polyhedron p, Tuple<Matrix, Vector> codomain)
        {
            Domain = p;
            CoDomain = codomain;
            quadProg = new QuadProg(codomain.Item1, codomain.Item2, p);
        }
    }
}
