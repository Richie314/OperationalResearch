using Fractions;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Graphs
{
    public class BoundedCostEdge : CostEdge
    {
        public Fraction lb { get; set; }
        public Fraction ub { get; set; }
        public BoundedCostEdge(
            Fraction cost, int from, int to, 
            Fraction? lb, Fraction? ub = null) : base(cost, from , to) 
        {
            this.lb = lb ?? Fraction.Zero;
            if (this.lb.IsNegative)
            {
                throw new ArgumentException("Cannot have negative lower bound");
            }
            this.ub = ub ?? new Fraction(int.MaxValue);
            if (this.ub.IsNegative)
            {
                throw new ArgumentException("Cannot have negative upper bound!");
            }
        }
        public BoundedCostEdge(CostEdge e):this(e.Cost, e.From, e.To, null, null)
        { }

        public BoundedCostEdge(string from, string to, string cost, string? lb, string? ub) :
            this(from: int.Parse(from), to: int.Parse(to), cost: Fraction.FromString(cost),
                lb: string.IsNullOrWhiteSpace(lb) ? null : Fraction.FromString(lb),
                ub: string.IsNullOrWhiteSpace(ub) ? null : Fraction.FromString(ub))
        { }

        public BoundedCostEdge(string[] v) :
            this(from: v[0], to: v[1], cost: v[2], lb: v.Length > 3 ? v[3] : null, ub: v.Length >= 4 ? v[4] : null)
        { }

        public new string Label { get => $"({base.Label}, {Function.Print(lb)}, {Function.Print(ub)})"; }
    }
}
