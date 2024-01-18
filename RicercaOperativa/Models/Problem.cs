﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;

namespace RicercaOperativa.Models
{
    class Problem
    {
        private Fraction[,] matrix;
        private Fraction[] vecB;
        private Fraction[] vecC;
        public IProgrammingInterface Solver;
        public Problem(Fraction[,] matrix, Fraction[] vecB, Fraction[] vecC, IProgrammingInterface solver)
        {
            this.matrix = matrix;
            this.vecB = vecB;
            this.vecC = vecC;
            this.Solver = solver;
        }
        public Problem(string[][] sMatrix, string[] sVecB, string[] sVecC, IProgrammingInterface solver)
        {
            ArgumentNullException.ThrowIfNull(sMatrix);
            ArgumentNullException.ThrowIfNull(sVecB);
            ArgumentNullException.ThrowIfNull(sVecC);
            matrix = new Fraction[sMatrix.Length, sMatrix[0].Length];
            for (int i = 0; i < sMatrix.Length; i++)
            {
                for (int j = 0; j < sMatrix[i].Length; j++)
                {
                    matrix[i, j] = Fraction.FromString(sMatrix[i][j]);
                }
            }
            vecB = sVecB.Select(Fraction.FromString).ToArray();
            vecC = sVecC.Select(Fraction.FromString).ToArray();
            Solver = solver;
        }
        public Problem(string[][] sMatrixAndB, string[] sVecC, IProgrammingInterface solver) : this(
            sMatrixAndB.Select(row => row.SkipLast(1).ToArray()).ToArray(),
            sMatrixAndB.Select(row => row.Last()).ToArray(),
            sVecC,
            solver)
        {            
        }
        public async Task<bool> Solve(StreamWriter? logger)
        {
            Solver.SetMainMatrix(matrix);
            Solver.SetFirstVector(vecB);
            Solver.SetSecondVector(vecC);
            Task<bool> Solving = Solver.SolveAsync(logger);
            return await Solving.WaitAsync(CancellationToken.None);
        }
    }
}
