using Fractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using System.Drawing.Drawing2D;
using System.Windows.Input.Manipulations;
using Microsoft.Scripting.Utils;
using static IronPython.SQLite.PythonSQLite;

namespace OperationalResearch.Models.Elements
{
    public sealed class Matrix
    {
        private readonly Fraction[,] m;
        public Matrix()
        {
            m = new Fraction[0, 0];
        }
        public Matrix(Fraction[,] matrix)
        {
            m = matrix;
        }
        public Matrix(Fraction[][] matrix)
        {
            if (matrix is null)
            {
                m = new Fraction[0, 0];
                return;
            }
            int rows = matrix.Length;
            if (rows == 0)
            {
                m = new Fraction[0, 0];
                return;
            }
            int cols = matrix[0].Length;
            m = new Fraction[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                if (matrix[i].Length != cols)
                {
                    throw new ArgumentException(
                        $"Number of columns cannot change in matrix, row: {i}/{rows}, col: {matrix[i].Length} != {cols}");
                }
                for (int j = 0; j < cols; j++)
                {
                    m[i, j] = matrix[i][j];
                }
            }
        }
        public Matrix(int rows, int cols) : this(
            Enumerable.Repeat(Enumerable.Repeat(Fraction.Zero, cols).ToArray(), rows).ToArray())
        {
        }

        public Matrix(string[][] matrix) :
            this(matrix.Apply((s, i, j) => Fraction.FromString(s.Trim())))
        { }
        public int Rows { get => m.Rows(); }
        public int Cols { get => m.Columns(); }
        public Matrix T { get => new(m.Transpose()); }
        public bool IsSquare { get => Rows > 0 && m.IsSquare(); }
        private Matrix GetSub(int excludeRow, int excludeCol) =>
            new Matrix(m.RemoveRow(excludeRow).RemoveColumn(excludeCol));
        private Fraction Determinant()
        {
            if (Rows == 0 || Cols == 0)
            {
                throw new InvalidOperationException($"Cannot calculate determinant of empty matrix ({Rows}X{Cols})");
            }
            if (!m.IsSquare())
            {
                throw new InvalidOperationException($"Cannot calculate determinant of non square matrix ({Rows}X{Cols})");
            }

            if (Rows == 1)
            {
                return m[0, 0];
            }
            if (Rows == 2)
            {
                return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
            }
            Fraction det = 0;
            for (int i = 0; i < Cols; i++)
            {
                // Expand through the first row
                if (i % 2 == 0)
                    det += m[0, i] * GetSub(0, i).Determinant();
                else
                    det -= m[0, i] * GetSub(0, i).Determinant();
            }
            return det;
        }
        public Fraction Det { get => Determinant(); }
        private Matrix Invert()
        {
            if (!m.IsSquare())
            {
                throw new InvalidOperationException("Cannot invert a non square matrix");
            }
            if (Rows == 1)
            {
                return new Matrix(
                    new Fraction[1, 1] { { Fraction.One / m[0, 0] } });
            }
            Fraction det = Det;
            if (det.IsZero)
            {
                throw new InvalidOperationException("Matrix cannot be inverted");
            }
            Fraction[,] m2 = new Fraction[Rows, Rows];
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Cols; ++j)
                {
                    if ((i + j) % 2 == 0)
                    {
                        m2[i, j] = GetSub(i, j).Det;
                    }
                    else
                    {
                        m2[i, j] = -GetSub(i, j).Det;
                    }
                    m2[i, j] /= det;
                }
            }
            return new Matrix(m2).T;
        }
        public Matrix Inv { get => Invert(); }
        private Matrix ToBaseIndexes(int[] indexes) =>
            new Matrix(T.m.GetColumns(indexes)).T;
        public Vector this[int rowIndex]
        {
            get => m.GetRow(rowIndex);
            set => m.SetRow(rowIndex, value.Get);
        }
        public Vector Col(int colIndex)
        {
            return m.GetColumn(colIndex);
        }
        public Fraction this[int rowIndex, int colIndex]
        {
            get => m[rowIndex, colIndex];
            set => m[rowIndex, colIndex] = value;
        }
        public Matrix this[IEnumerable<int> baseIndexes]
        {
            get => ToBaseIndexes(baseIndexes.ToArray());
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
            {
                throw new ArgumentException($"Columns of a must be equal to rows of b ({a.Cols} != {b.Rows})");
            }

            Fraction[,] m2 = new Fraction[a.Rows, b.Cols];
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Cols; j++)
                {
                    m2[i, j] = a[i] * b.Col(j); // i-th row of a * j-th col of b
                }
            }
            return new Matrix(m2);
        }
        public static Matrix operator *(Fraction a, Matrix b) =>
            new Matrix(b.m.Convert(x => a * x));
        public static Matrix operator *(Matrix a, Fraction b) =>
            new Matrix(a.m.Convert(x => b * x));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">matrix</param>
        /// <param name="b">column vector</param>
        /// <returns></returns>
        public static Vector operator *(Matrix a, Vector b)
        {
            if (a.Cols != b.Size)
            {
                throw new ArgumentException($"Columns of a must be equal to length of b ({a.Cols} != {b.Size})");
            }

            Vector c = new Fraction[a.Rows];
            for (int i = 0; i < a.Rows; i++)
            {
                c[i] = a[i] * b;
            }
            return c;
        }

        public override string ToString()
        {
            if (m.GetNumberOfElements() == 0)
            {
                return "()";
            }
            if (Rows == 1)
            {
                return $"( {string.Join(' ', m.GetRow(0))} )";
            }

            int maxLength = 0;
            string[,] RenderedMatrix = m.Apply(x =>
            {
                string s = Function.Print(x);
                maxLength = int.Max(maxLength, s.Length);
                return s;
            });
            List<string> lines = [];
            for (int i = 0; i < Rows; i++)
            {
                string line = string.Join(' ',
                    RenderedMatrix
                    .GetRow(i)
                    .Select(s => s.PadLeft(maxLength))
                    .ToArray());
                if (i == 0)
                {
                    lines.Add($"\t/ {line} \\");
                }
                else if (i == Rows - 1)
                {
                    lines.Add($"\t\\ {line} /");
                }
                else
                {
                    lines.Add($"\t| {line} |");
                }
            }
            return Environment.NewLine + string.Join(Environment.NewLine, [.. lines]);
        }

        public IEnumerable<int> RowsIndeces { get => Enumerable.Range(0, Rows); }
        public IEnumerable<int> ColsIndeces { get => Enumerable.Range(0, Cols); }
        public static Matrix Identity(int n)
        {
            Matrix I = new(n, n);
            for (int i = 0; i < n; i++)
            {
                I[i, i] = Fraction.One;
            }
            return I;
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            if (a.Rows != b.Rows)
            {
                throw new ArgumentException($"Matrixes have different row count ({a.Rows} != {b.Rows})");
            }
            if (a.Cols != b.Cols)
            {
                throw new ArgumentException($"Matrixes have different column count ({a.Cols} != {b.Cols})");
            }
            Fraction[,] c = a.m.Copy();
            for (int i = 0; i < c.Rows(); i++)
            {
                for (int j = 0; j < c.Columns(); j++)
                {
                    c[i, j] += b[i, j];
                }
            }
            return new Matrix(c);
        }
        public static Matrix operator -(Matrix a, Matrix b) => a + (-1 * b);
        public static Matrix operator |(Matrix a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Rows != b.Size)
            {
                throw new ArgumentException($"Matrixes and vector have incompatible sizes ({a.Rows} != {b.Size})");
            }
            return new(a.m.InsertColumn(b.Get));
        }
        public static Matrix operator |(Matrix a, Matrix b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Rows != b.Rows)
            {
                throw new ArgumentException($"Matrixes have incompatible sizes ({a.Rows} != {b.Rows})");
            }
            var m = a.m.Copy();
            for (int i = 0; i < b.Cols; i++)
            {
                m = m.InsertColumn(b.Col(i).Get);
            }
            return new(m);
        }
        public Matrix GetCols(IEnumerable<int> cols) => T[cols].T;
        public Fraction[,] M { get => m; }
        public int[][] AllIndices
        {
            get => RowsIndeces.Select(r => this[r].Indices.ToArray()).ToArray();
        }
        public int[][] NonZeroIndices
        {
            get => RowsIndeces.Select(r => this[r].NonZeroIndeces).ToArray();
        }
        public Matrix AddRow(Vector x)
        {
            if (x.Size != Cols)
            {
                throw new ArgumentException(
                    $"row to add must have col number equal to the cols of the matrix ({x.Size} != {Cols})");
            }
            return new Matrix(M.InsertRow(x.Get));
        }
        public Matrix AddRows(IEnumerable<Vector> arr)
        {
            if (!arr.Any())
            {
                return this;
            }
            return AddRow(arr.First()).AddRows(arr.Skip(1));
        }
        private IEnumerable<Vector> getRows() => RowsIndeces.Select(i => new Vector(m.GetRow(i)));
        public Matrix AddRows(Matrix m) => AddRows(m.getRows());
        public Matrix Copy() => new Matrix(M.Copy());
    }
}
