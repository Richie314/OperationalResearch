using Fractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    partial class Graph
    {
        public class Edge : IComparable<Edge>
        {
            [Range(0, int.MaxValue)]
            public int From, To;
            public Fraction Cost;
            public enum EdgeType
            {
                Required,
                Standard,
                Disabled
            }
            public EdgeType Type;
            public Edge(Fraction cost, int from, int to)
            {
                Cost = cost;
                From = from;
                if (from == to)
                {
                    throw new ArgumentException($"Invalid edge, takes from {from + 1} to {to + 1}");
                }
                To = to;
                Type = EdgeType.Standard;
            }
            public Edge(string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    throw new ArgumentNullException("s");
                Type = EdgeType.Standard;
                string[] pair = s.Split('-', StringSplitOptions.TrimEntries);
                if (pair.Length < 2)
                {
                    throw new ArgumentException("Invalid syntax for edge. Use From-To");
                }
                From = int.Parse(pair[0]) - 1;
                To = int.Parse(pair[1]) - 1;
                Cost = Fraction.One;
            }
            public override string ToString()
            {
                return $"({From + 1}, {To + 1})";
            }
            public static bool operator <(Edge a, Edge b)
            {
                return a.From < b.From || a.To < b.To;
            }
            public static bool operator >(Edge a, Edge b)
            {
                return a.From > b.From || a.To > b.To;
            }
            public static bool operator <=(Edge a, Edge b)
            {
                return a.From <= b.From || a.To <= b.To;
            }
            public static bool operator >=(Edge a, Edge b)
            {
                return a.From >= b.From || a.To >= b.To;
            }
            public static bool operator ==(Edge a, Edge b)
            {
                return a.From == b.From && a.To == b.To;
            }
            public static bool operator !=(Edge a, Edge b)
            {
                return a.From != b.From || a.To != b.To;
            }
            public int CompareTo(Edge? other)
            {
                if (other is null)
                {
                    if (this is null)
                    {
                        return 0;
                    }
                    return -1;
                }
                return this < other ? (-1) : (this == other ? 0 : 1);
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj is null)
                {
                    return false;
                }

                throw new NotImplementedException();
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
