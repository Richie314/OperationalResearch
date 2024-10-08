﻿using OperationalResearch.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems.Solvers
{
    public interface ISolving<TDomain, TCoDomain>
    {
        public TDomain? Domain { get; set; }
        public TCoDomain? CoDomain { get; set; }
        public Task<bool> SolveMaxAsync(IEnumerable<IndentWriter?> loggers);
        public Task<bool> SolveMinAsync(IEnumerable<IndentWriter?> loggers);

        public Task<bool> SolveIntegerMaxAsync(IEnumerable<IndentWriter?> loggers);
        public Task<bool> SolveIntegerMinAsync(IEnumerable<IndentWriter?> loggers);

        public void SetData(TDomain domain, TCoDomain codomain);

    }
}
