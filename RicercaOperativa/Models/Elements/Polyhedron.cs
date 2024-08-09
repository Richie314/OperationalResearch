using System;
using Accord.Math;
using Fractions;
using OperationalResearch.Extensions;

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
        public bool IsInside(Vector x) => A * x <= b && (!ForcePositive || x.IsPositiveOrZero);

        public bool IsOutside(Vector x) => !IsInside(x);

        public bool IsOnBorder(Vector x) => 
            IsInside(x) && 
            (((A * x) - b).ZeroIndexes.Any() || (ForcePositive && x.ZeroIndexes.Any()));

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
        public int[] AllRows { get => Enumerable.Range(0, Rows).ToArray(); }

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

        public int[]? RandomBasis()
        {
            //Random rnd = Random.Shared;
            var allBasis = getAllBasis();
            if (allBasis.Any())
            {
                return allBasis.First();//.ElementAt(rnd.Next(0, allBasis.Count()));
            }
            return null;
        }

        private static Vector? BasisVertex(Matrix A, Vector b, IEnumerable<int> B)
        {
            try
            {
                Matrix A_B = A[B];
                if (A_B.Det.IsZero)
                {
                    return null;
                }
                Matrix A_B_inv = A_B.Inv;

                Vector b_B = b[B];

                return A_B_inv * b_B;
            }
            catch
            {
                return null;
            }
        }

        private static bool OkBasis(Matrix A, Vector b, IEnumerable<int> B)
        {
            int[] N = A.RowsIndeces.Where(i => !B.Contains(i)).ToArray();
            try
            {
                Vector? x = BasisVertex(A, b, B);
                if (x is null)
                {
                    return false;
                }

                // Solution is acceptable => Basis is acceptable
                return (A[N] * x) <= b[N];
            } catch
            {
                return false;
            }
        }

        private IEnumerable<int[]> getAllBasis()
        {
            var allCombinations = AllRows.OrderedPermutations(this.A.Cols);
            var A = GetMatrix();
            var b = GetVector();
            return allCombinations
                .Where(B => OkBasis(A, b, B))
                .Select(B => B.ToArray());
        }

        public IEnumerable<int[]> AllBasis { get => getAllBasis(); }

        private IEnumerable<Vector> getVertices()
        {
            try
            {
                var A = GetMatrix();
                var b = GetVector();
                return getAllBasis().Select(B => BasisVertex(A, b, B) ?? Vector.Empty);
            }
            catch
            {
                return Enumerable.Empty<Vector>();
            }
        }
        public IEnumerable<Vector> Vertices
        {
            get => getVertices();
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
            int[]? B = RandomBasis();
            if (B is null || B.Length == 0)
            {
                Random rnd = Random.Shared;
                for (int guesses = 0; guesses < maxGuesses; guesses++)
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

        public static Polyhedron FromStringMatrixAndVector(string[][] mat, string[] vector, bool ForcePositive = false)
        {
            string[] Operators = ["<=", ">=", "=", "=="];
            if (!Operators.Contains(mat[0].Last().Trim()))
            {
                // The matrix is in the form Ax <= b already
                return new Polyhedron(
                        A: new Matrix(mat.Apply(row => row.Apply(Fraction.FromString))),
                        b: Vector.FromString(vector) ?? Vector.Empty,
                        forcePositive: ForcePositive);
            }

            if (mat.Length != vector.Length)
            {
                throw new ArgumentException($"Matrix row count must equal vector's size ({mat.Length} != {vector.Length})");
            }

            List<Vector> A = [];
            List<Fraction> b = [];

            Vector b_strings = Vector.FromString(vector) ?? Vector.Empty;

            for (int i = 0; i < mat.Length; i++)
            {
                Vector? r = Vector.FromString(mat[i].SkipLast(1));
                if (r is null)
                    continue;
                switch (mat[i].Last().Trim())
                {
                    case "<=":
                        // Std form
                        A.Add(r);
                        b.Add(b_strings[i]);
                        break;
                    case ">=":
                        // Inverted form
                        A.Add(r * Fraction.MinusOne);
                        b.Add(b_strings[i] * Fraction.MinusOne);
                        break;
                    case "==":
                    case "=":
                        A.Add(r);
                        b.Add(b_strings[i]);
                        A.Add(r * Fraction.MinusOne);
                        b.Add(b_strings[i] * Fraction.MinusOne);
                        break;
                    default:
                        continue;
                }
            }


            return new Polyhedron(
                A: new Matrix(A.Select(r => r.Get).ToArray()),
                b: b,
                forcePositive: ForcePositive);
        }


        public static Polyhedron FromStringMatrix(string[][] mat, bool ForcePositive = false) =>
            FromStringMatrixAndVector(
                mat: mat.Select(row => row.SkipLast(1).ToArray()).ToArray(),
                vector: mat.Select(row => row.Last()).ToArray(),
                ForcePositive: ForcePositive);

        public static Polyhedron FromRow(Vector matrixRow, Fraction limit, bool xPos = false) =>
            new Polyhedron(matrixRow.Row, new Vector(limit), xPos);

        public Polyhedron Copy() => new Polyhedron(A.Copy(), b.Copy(), ForcePositive);
        public Polyhedron RemoveEquation(int index)
        {
            if (index < 0)
            {
                throw new ArgumentException("Invalid index: can't be below 0");
            }
            if (index >= A.Rows)
            {
                throw new ArgumentException($"Invalid index: can't be over {A.Rows}");
            }
            if (index < A.Rows)
            {
                return new Polyhedron(A[A.RowsIndeces.Where(j => j != index)], b.RemoveAt(index));
            }
            var newA = GetMatrix();
            var newB = GetVector();
            return new Polyhedron(
                newA[newA.RowsIndeces.Where(j => j != index)], 
                newB.RemoveAt(index));
        }

        public static Polyhedron operator &(Polyhedron? l, Polyhedron? r)
        {
            if (l is null)
            {
                if (r is null)
                {
                    throw new ArgumentNullException(nameof(l));
                }
                return r;
            }
            if (r is null)
            {
                return l;
            }

            if (l.Cols != r.Cols)
            {
                throw new ArgumentException("Polyhedrons are in different spaces");
            }

            var leftMatrix = (l.A | l.b).M.ToJagged();
            var rightMatrix = (r.A | r.b).M.ToJagged();
            foreach (var equation in rightMatrix)
            {
                if (leftMatrix.Contains(equation)) // Do not duplicate rows
                    continue;
                leftMatrix = leftMatrix.Append(equation).ToArray();
            }
            var fullMatrix = new Matrix(leftMatrix);
            var matrix = fullMatrix.GetCols(l.A.ColsIndeces);
            var vector = fullMatrix.Col(fullMatrix.Cols - 1);
            return new Polyhedron(matrix, vector, l.ForcePositive || r.ForcePositive);
        }

        public Vector? this[IEnumerable<int> B]
        {
            get => BasisVertex(GetMatrix(), GetVector(), B);
        }

        public static Tuple<Fraction, Fraction, Fraction> GetLineFrom2Points(Vector x1, Vector x2)
        {
            if (x1 == x2)
            {
                throw new ArgumentException("Points can't must be different to find a line");
            }

            if (x1.Size != 2 || x2.Size != 2)
            {
                throw new ArgumentException(
                    $"Points must be in ℝ^2 (given sizes {x1.Size} and {x2.Size})");
            }

            var s = new Matrix(new Fraction[,]
            {
                { x1[0], Fraction.One },
                { x2[0], Fraction.One }
            });
            if (!s.Det.IsZero)
            {
                var mq = s.Inv * new Vector(x1[1], x2[1]);
                mq = new Vector(mq[0], Fraction.MinusOne, mq[1]).Simplify();
                return new Tuple<Fraction, Fraction, Fraction>(mq[0], mq[1], mq[2]);
            }


            // x1[0] == x2[0] here. Equation is x = x1[0] => 1*x + 0*y -x1[0] = 0
            return new Tuple<Fraction, Fraction, Fraction>(
                x1[0].Denominator, 
                Fraction.Zero, 
                Fraction.MinusOne * x1[0].Numerator);
        }
        public static Polyhedron FromBidimensionalPoints(IEnumerable<Vector> points)
        {
            if (points.Count() < 3)
            {
                throw new ArgumentException(
                    $"Not enough points were given ({points.Count()} < 3)");
            }

            List<Tuple<Fraction, Fraction, Fraction>> Lines = [];
            Vector predecessor = points.First(), point = points.First();

            while (true)
            {
                // Find the nearest point but exclude the predecessor
                var target = points
                    .Where(p => p != predecessor && p != point)
                    .OrderBy(p => (p - point).Norm2)
                    .First();
                Lines.Add(GetLineFrom2Points(point, target));

                if (target == points.First())
                {
                    break;
                }
                predecessor = point;
                point = target;
            }

            var vectors = Lines.Select(eq =>
            {
                var d = points
                    .Select(p => eq.Item1 * p[0] + eq.Item2 * p[1] + eq.Item3)
                    .Where(di => !di.IsZero);
                if (!d.Any())
                {
                    throw new DataMisalignedException(
                        $"It was impossible to biuld a disequation from the line {Function.Print(eq.Item1)}*x + {Function.Print(eq.Item2)}*y + {Function.Print(eq.Item3)} = 0");
                }
                return d.First().IsPositive ?
                    new Tuple<Vector, Fraction>(
                        new Vector(eq.Item1, eq.Item2) * Fraction.MinusOne,
                        eq.Item3) :
                     new Tuple<Vector, Fraction>(
                        new Vector(eq.Item1, eq.Item2),
                        eq.Item3 * Fraction.MinusOne);

            });

            var A = new Matrix(vectors.Select(r => r.Item1.Get).ToArray());
            var b = new Vector(vectors.Select(r => r.Item2).ToArray());

            return new Polyhedron(A, b);
        }
    }
}
