using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Fractions;

namespace OperationalResearch.Models.Elements
{
    /// <summary>
    /// Column vector
    /// </summary>
    public sealed class Vector
    {
        /// <summary>
        /// The actual array inside the vector class
        /// </summary>
        private readonly Fraction[] v;

        #region Builders

        public Vector(params Fraction[] v)
        {
            ArgumentNullException.ThrowIfNull(v);
            this.v = v;
        }
        
        public static implicit operator Vector(Fraction[] v) => new Vector(v);
        public static implicit operator Vector(List<Fraction> v) => new Vector(v);

        public Vector(IEnumerable<Fraction> v)
        {
            ArgumentNullException.ThrowIfNull(v);
            this.v = v.ToArray();
        }

        public static Vector? FromString(IEnumerable<string>? v)
        {
            if (v is null || v.Any(string.IsNullOrWhiteSpace))
                return null;
            return v.Select(Fraction.FromString).ToArray();
        }

        public static Vector FromDouble(IEnumerable<double> v) =>
            v.Select(Fraction.FromDouble).ToArray();
        public static Vector FromDecimal(IEnumerable<decimal> v) =>
            v.Select(Fraction.FromDecimal).ToArray();

        #endregion

        #region Converters

        public IEnumerable<double> ToDouble() => v.Select(v => v.ToDouble());
        public IEnumerable<decimal> ToDecimal() => v.Select(v => v.ToDecimal());
        public IEnumerable<int> ToInt() => v.Select(v => v.ToInt32());

        public Fraction[] Get
        {
            get => v;
        }

        /// <summary>
        /// Transforms the column vector to a matrix with one row
        /// </summary>
        public Matrix Row
        {
            get => new([v]);
        }

        #endregion

        #region Accessing

        public IEnumerable<int> Indices { get => Enumerable.Range(0, Size); }
        private Vector ExtractIndices(IEnumerable<int> IndicesToExtract)
        {
            ArgumentNullException.ThrowIfNull(IndicesToExtract);
            return IndicesToExtract.Select(i => this[i]).ToArray();
        }
        public Fraction this[int Index]
        {
            get => v[Index];
            set => v[Index] = value;
        }
        public Vector this[IEnumerable<int> IndicesToExtract]
        {
            get => ExtractIndices(IndicesToExtract);
        }

        #endregion

        #region StaticBuilders

        public static Vector Empty = new();
        public static Vector Zeros(int n) => Repeat(Fraction.Zero, n);
        public static Vector Ones(int n) => Repeat(Fraction.One, n);
        public static Vector Repeat(Fraction f, int n) => new Vector(Enumerable.Repeat(f, n));
        public static Vector Rand(int size)
        {
            var x = Zeros(size);
            var rnd = Random.Shared;
            for (int i = 0; i < size; i++)
            {
                x[i] = new Fraction(
                    rnd.Next(int.MinValue, int.MaxValue),
                    rnd.Next(1, int.MaxValue));
            }
            return x;
        }

        #endregion

        #region Operators

        public static Vector operator +(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.Select(i => a[i] + b[i]).ToArray();
        }
        public static Vector operator -(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.Select(i => a[i] - b[i]).ToArray();
        }

        /// <summary>
        /// Scalar product
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Fraction operator *(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            Fraction sum = Fraction.Zero;
            for (int i = 0; i < a.Size; i++)
            {
                sum += a[i] * b[i];
            }
            return sum;
        }

        /// <summary>
        /// Multiplies component by component
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector Mult(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.Select(i => a[i] * b[i]).ToArray();
        }

        /// <summary>
        /// Divides component by component
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector Div(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.Select(i => a[i] / b[i]).ToArray();
        }
        
        /// <summary>
        /// Multply all acomponents by scalar
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector operator *(Fraction a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            Vector c = b.Get.Copy(); // Not copying the array causes the original to be overwritten
            for (int i = 0; i < c.Size; i++)
            {
                c[i] *= a;
            }
            return c;
        }
        
        /// <summary>
        /// Multply all acomponents by scalar
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector operator *(Vector a, Fraction b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            Vector c = a.Get.Copy(); // Not copying the array causes the original to be overwritten
            for (int i = 0; i < c.Size; i++)
            {
                c[i] *= b;
            }
            return c;
        }

        public static bool operator <(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] < b[i]);
        }
        public static bool operator <=(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] <= b[i]);
        }

        public static bool operator >(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] > b[i]);
        }
        public static bool operator >=(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] >= b[i]);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] == b[i]);
        }
        public static bool operator !=(Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.Any(i => a[i] != b[i]);
        }

        #endregion

        #region Shorthands

        public int Size { get { return v.Length; } }
        public bool IsEmpty { get => Size == 0; }

        /// <summary>
        /// v == 0
        /// </summary>
        public bool IsZero
        {
            get => v.All(x => x.IsZero);
        }
        /// <summary>
        /// v > 0
        /// </summary>
        public bool IsPositive
        {
            get => v.All(x => x.IsPositive);
        }
        /// <summary>
        /// v < 0
        /// </summary>
        public bool IsNegative
        {
            get => v.All(x => x.IsNegative);
        }
        /// <summary>
        /// v >= 0
        /// </summary>
        public bool IsPositiveOrZero
        {
            get => v.All(x => !x.IsNegative);
        }
        /// <summary>
        /// v <= 0
        /// </summary>
        public bool IsNegativeOrZero
        {
            get => v.All(x => !x.IsPositive);
        }
        /// <summary>
        /// Index of the min element of the vector
        /// </summary>
        public int ArgMin
        {
            get => v.ArgMin();
        }
        /// <summary>
        /// The min element of the vector
        /// </summary>
        public Fraction Min
        {
            get => v.Min();
        }
        /// <summary>
        /// The max element of the vector
        /// </summary>
        public Fraction Max
        {
            get => v.Max();
        }
        public int[] PositiveIndexes
        {
            get => v.Find(x => x.IsPositive);
        }
        public int[] ZeroIndexes
        {
            get => v.Find(x => x.IsZero);
        }
        public int[] NonZeroIndeces
        {
            get => v.Find(x => !x.IsZero);
        }
        public int[] NegativeIndexes
        {
            get => v.Find(x => x.IsNegative);
        }
        public int[] FractionaryIndeces
        {
            get => v.Find(x => x.Denominator != 1);
        }
        public int[] IntegerIndeces
        {
            get => v.Find(x => x.Denominator == 1);
        }

        #endregion

        public Vector RemoveAt(int jToRemove) => v.RemoveAt(jToRemove);

        #region Overrides

        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object? obj) => base.Equals(obj);
        public override string ToString() => 
            $"( {string.Join(", ", v.Select(x => Function.Print(x)))} )";
        public Vector Copy() => v.ToList();

        #endregion

        public Vector Concat(Vector? x2)
        {
            if (x2 is null || x2.Size == 0)
                return this;
            return Get.Concat(x2.Get).ToArray();
        }
        public Vector Concat(IEnumerable<Fraction>? x2) =>
            x2 is null ? this : Concat((Vector?)x2.ToArray());

        public static Vector Sum(IEnumerable<Vector> vectors)
        {
            ArgumentNullException.ThrowIfNull(vectors);
            if (!vectors.Any())
            {
                throw new ArgumentException("Requested sum of empty collection");
            }
            if (vectors.Count() == 1)
            {
                return vectors.First();
            }
            return vectors.First() + Sum(vectors.Skip(1));
        }
        public Fraction SumOfComponents()
        {
            if (Size == 0)
            {
                return Fraction.Zero;
            }
            Fraction fraction = Fraction.Zero;
            for (int i = 0; i < Size; i++)
            {
                fraction += this[i];
            }
            return fraction;
        }
        private static BigInteger GCD(BigInteger a, BigInteger b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
        private static BigInteger LCM(BigInteger a, BigInteger b)
        {
            BigInteger num1, num2;
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (BigInteger i = 1; i < num2; i++)
            {
                BigInteger mult = num1 * i;
                if (mult % num2 == 0)
                {
                    return mult;
                }
            }
            return num1 * num2;
        }
        public Vector Simplify()
        {
            BigInteger numGcd = v.Select(i => i.Numerator).Aggregate(GCD);
            BigInteger denGcd = v.Select(i => i.Denominator).Aggregate(LCM);
            return this * new Fraction(denGcd, numGcd);
        }

        public Fraction Norm2 { get => (this * this).Sqrt(); }
    }
}
