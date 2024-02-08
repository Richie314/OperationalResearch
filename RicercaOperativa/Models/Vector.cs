using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Fractions;

namespace OperationalResearch.Models
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
        public Vector(params Fraction[] v)
        {
            ArgumentNullException.ThrowIfNull(v);
            this.v = v;
        }
        public static implicit operator Vector(Fraction[] v)
        {
            return new Vector(v);
        }
        public static implicit operator Vector(List<Fraction> v)
        {
            return new Vector(v);
        }
        public Vector(IEnumerable<Fraction> v)
        {
            ArgumentNullException.ThrowIfNull(v);
            this.v = v.ToArray();
        }
        public IEnumerable<double> ToDouble()
        {
            return v.Select(v => v.ToDouble());
        }
        public IEnumerable<decimal> ToDecimal()
        {
            return v.Select(v => v.ToDecimal());
        }
        public static Vector Empty = new();
        public int Size { get { return v.Length; } }
        public bool IsEmpty { get => Size == 0; }
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
        public Fraction[] Get
        {
            get => v;
        }
        public Vector Concat(Vector? x2)
        {
            if (x2 is null || x2.Size == 0)
                return this;
            return Get.Concat(x2.Get).ToArray();
        }
        public static Vector operator + (Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.Select(i => a[i] + b[i]).ToArray();
        }
        public static Vector operator - (Vector a, Vector b)
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
        public static Fraction operator * (Vector a, Vector b)
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
        public static Vector operator * (Fraction a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            Vector c = b.Get.Copy(); // Not copying the array causes the original to be overwritten
            for (int i = 0; i <  c.Size; i++)
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
        public static Vector operator * (Vector a, Fraction b)
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
        
        public static bool operator < (Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] < b[i]);
        }
        public static bool operator <= (Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] <= b[i]);
        }

        public static bool operator > (Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] > b[i]);
        }
        public static bool operator >= (Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] >= b[i]);
        }
        
        public static bool operator == (Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.All(i => a[i] == b[i]);
        }
        public static bool operator != (Vector a, Vector b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Size != b.Size)
            {
                throw new ArgumentException($"Length of vectors must be equal ({a.Size} != {b.Size})");
            }
            return a.Indices.Any(i => a[i] != b[i]);
        }
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
        public Vector RemoveAt(int jToRemove)
        {
            return v.RemoveAt(jToRemove);
        }
        /// <summary>
        /// Transforms the column vector to a matrix with one row
        /// </summary>
        public Matrix Row
        {
            get => new([v]);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return "( " + string.Join(", ", v.Select( x => Function.Print(x))) + " )";
        }

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
    }
}
