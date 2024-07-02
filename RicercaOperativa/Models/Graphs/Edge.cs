using Fractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OperationalResearch.Models.Graphs
{
    [Serializable]
    public class Edge : IComparable<Edge>, ICloneable
    {
        [Range(0, int.MaxValue)]
        public int From, To;
        
        public Edge(int from, int to)
        {
            From = from;
            To = to;
            if (from == to)
            {
                throw new ArgumentException($"Invalid edge, takes from {from + 1} to {to + 1}");
            }
        }
        public Edge(string s)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(s, nameof(s));
            string[] pair = s.Split('-', StringSplitOptions.TrimEntries);
            if (pair.Length < 2)
            {
                throw new ArgumentException("Invalid syntax for edge. Use From-To");
            }
            From = int.Parse(pair[0]) - 1;
            To = int.Parse(pair[1]) - 1;
        }
        
        public override string ToString() => 
            $"({From + 1}, {To + 1})";
        
        public static bool operator <(Edge a, Edge b) =>
            a.From < b.From || (a.To < b.To && a.From == b.From);
        public static bool operator >(Edge a, Edge b) =>
            a.From > b.From || (a.To > b.To && a.From == b.From);

        public static bool operator <=(Edge a, Edge b) =>
            a < b || a == b;
        public static bool operator >=(Edge a, Edge b) => 
            a > b || a == b;

        public static bool operator ==(Edge a, Edge b) =>
            a.From == b.From && a.To == b.To;
        public static bool operator !=(Edge a, Edge b) =>
            a.From != b.From || a.To != b.To;

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

        public override int GetHashCode() => base.GetHashCode();

        public object Clone() => MemberwiseClone();
    }
}
