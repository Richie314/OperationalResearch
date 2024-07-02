using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Accord.Math;
using Fractions;

namespace OperationalResearch.Models.Elements
{
    /// <summary>
    /// A x <= b
    /// </summary>
    public class Polyhedron
    {
        public Matrix A;
        public Vector b;
        public bool ForcePositive = false;

        public Polyhedron(Matrix A, Vector b, bool forcePositive = false)
        {
            this.A = A;
            this.b = b;
            ForcePositive = forcePositive;

            if (this.A.Rows != this.b.Size)
            {
                throw new ArgumentException(
                    $"A must have row number equal to the size of b ({this.A.Rows} != {this.b.Size}");
            }
        }
        public Polyhedron(Fraction[,] A, Vector b, bool forcePositive) : this(new Matrix(A), b, forcePositive) { }
        
        public Polyhedron NegateKnownVector
        {
            get => new Polyhedron(A, Fraction.MinusOne * b, ForcePositive);
        }
        public bool IsInside(Vector x) => A * x <= b && (ForcePositive || x.IsPositiveOrZero);

        public bool IsOutside(Vector x) => !IsInside(x);

        public bool IsOnBorder(Vector x) => A * x == b;

        public Polyhedron AddRow(Vector aRow, Fraction bVal)
        {
            if (aRow.Size != Cols)
            {
                throw new ArgumentException(
                    $"row added must have col number equal to the cols of A ({aRow.Size} != {Cols}");
            }
            A = A.AddRow(aRow);
            b = b.Concat(new Vector(bVal));
            return this;
        }

        public int Cols { get => A.Cols; }
        public int Rows { get => A.Rows + (ForcePositive ? A.Cols : 0); }

        public Matrix GetMatrix()
        {
            if (!ForcePositive)
            {
                return A;
            }
            return A.AddRows(Fraction.MinusOne * Matrix.Identity(Cols));
        }

        public Vector GetVector()
        {
            if (!ForcePositive)
            {
                return b;
            }
            return b.Concat(Vector.Zeros(Cols));
        }

        public int[]? RandomBasis(int maxGuesses = 1000)
        {
            Random rnd = Random.Shared;
            for (int guesses = 0; guesses < maxGuesses; guesses++) {

                int[] B = [.. rnd.GetItems(A.RowsIndeces.ToArray(), A.Cols)];
                int[] N = A.RowsIndeces.Where(i => !B.Contains(i)).ToArray();

                Matrix A_B = A[B];
                if (A_B.Det.IsZero)
                {
                    continue;
                }
                Matrix A_B_inv = A_B.Inv;

                Vector b_B = b[B];
                Vector b_N = b[N];

                Vector x = A_B_inv * b_B;
                if ((A[N] * x) <= b_N)
                {
                    // Solution is acceptable => Basis is acceptable
                    return B.Sorted();
                }
            }
            return null;
        }

        public int[]? RandomDualBasis(Vector c, int maxGuesses = 1000)
        {
            Random rnd = Random.Shared;
            for (int guesses = 0; guesses < maxGuesses; guesses++)
            {

                int[] B = [.. rnd.GetItems(A.RowsIndeces.ToArray(), A.Cols)];
                int[] N = A.RowsIndeces.Where(i => !B.Contains(i)).ToArray();

                Matrix A_B = A[B];
                if (A_B.Det.IsZero)
                {
                    continue;
                }
                Matrix A_B_inv = A_B.Inv;

                Vector b_B = b[B];
                Vector b_N = b[N];

                Vector Y_B = (c.Row * A_B_inv)[0];
                if (Y_B.IsPositiveOrZero)
                {
                    return B;
                }
            }
            return null;

        }

        public Vector? RandomInternalPoint(int maxGuesses = 2000)
        {
            int[]? B = RandomBasis(maxGuesses / 2);
            if (B is null || B.Length == 0)
            {
                Random rnd = Random.Shared;
                for (int guesses = 0; guesses < maxGuesses / 2; guesses++)
                {
                    var x = Vector.Rand(Cols);
                    if (IsInside(x))
                    {
                        return x;
                    }
                }
                return null;
            }
            return A[B].Inv * b[B];
        }

        public static Polyhedron FromStringMatrixAndVector(string[][] mat, string[] vector, bool ForcePositive = false) =>
            new Polyhedron(
                A: new Matrix(mat.Apply(row => row.Apply(Fraction.FromString))), 
                b: vector.Apply(Fraction.FromString),
                forcePositive: ForcePositive);

        public static Polyhedron FromStringMatrix(string[][] mat, bool ForcePositive = false) =>
            FromStringMatrixAndVector(
                mat: mat.Select(row => row.SkipLast(1).ToArray()).ToArray(), 
                vector: mat.Select(row => row.Last()).ToArray(),
                ForcePositive: ForcePositive);

        public Polyhedron Copy() => new Polyhedron(A.Copy(), b.Copy(), ForcePositive);
        public Polyhedron RemoveEquation(int index) =>
            new Polyhedron(A[A.RowsIndeces.Where(j => j != index)], b.RemoveAt(index));
    }
}
