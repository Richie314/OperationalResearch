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
        public class Edge
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

        }
    }
}
