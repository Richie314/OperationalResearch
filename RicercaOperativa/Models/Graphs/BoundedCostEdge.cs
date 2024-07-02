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
    }
}
