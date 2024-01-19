﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;

namespace RicercaOperativa.Models
{
    public interface IProgrammingInterface
    {
        public Task<bool> SolveAsync(IEnumerable<StreamWriter?> loggers);
        public void SetMainMatrix(Fraction[,] m);
        public void SetFirstVector(Vector v);
        public void SetSecondVector(Vector v);
    }
}
