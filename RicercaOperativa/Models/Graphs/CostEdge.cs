using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;

namespace OperationalResearch.Models.Graphs
{
    public class CostEdge : Edge
    {
        public Fraction Cost { get; set; }

        public CostEdge(Fraction cost, int from, int to) : base(from, to)
        {
            Cost = cost;
        }
        public CostEdge(string s, string cost = "1") : base(s)
        {
            Cost = Fraction.FromString(cost);
        }
        public CostEdge(Edge e) : this(Fraction.One, e.From, e.To)
        { }
    }
}
