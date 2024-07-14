using System;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.Graphs;
using OperationalResearch.Models.Problems.Solvers;
using Fractions;

namespace OperationalResearch.Models.Problems
{
    #region LinearProgramming
    
    /// <summary>
    /// Generic linear programming problem
    /// </summary>
    public class LinearProgrammingProblem (Polyhedron p, Vector c, int[]? startBasis = null) : 
        Problem<Polyhedron, Vector, LinearSolver>
            (p,
             c,
             new LinearSolver(startBasis))
    {
        public LinearProgrammingProblem(string[][] polyhedron, string[] revenues, bool xPos = false, string[]? startBasis = null) :
            this(
                Polyhedron.FromStringMatrix(polyhedron, xPos),
                Vector.FromString(revenues) ?? Vector.Empty,
                startBasis is not null ? 
                    startBasis.Select(x => Convert.ToInt32(x.Trim()) - 1).ToArray() : 
                    null
                ) { }
    }


    /// <summary>
    /// Generic linear programming problem
    /// </summary>
    public class LinearProgrammingDualProblem(Polyhedron p, Vector c, int[]? startBasis = null) :
        Problem<Polyhedron, Vector, LinearDualSolver>
            (p,
             c,
             new LinearDualSolver(startBasis))
    {
        public LinearProgrammingDualProblem(string[][] polyhedron, string[] revenues, bool xPos = false, string[]? startBasis = null) :
            this(
                Polyhedron.FromStringMatrix(polyhedron, xPos),
                Vector.FromString(revenues) ?? Vector.Empty,
                startBasis is not null ?
                    startBasis.Select(x => Convert.ToInt32(x.Trim()) - 1).ToArray() :
                    null
                ) { }
    }

    #endregion

    #region IntegerLinearProgramming

    /// <summary>
    /// Travelling salesman problem (TSP) -> find hamiltonian cycle
    /// https://en.wikipedia.org/wiki/Travelling_salesman_problem
    /// </summary>
    public class TravellingSalesManProblem (TSP<CostEdge> graph, Tuple<int?, int?, string?> startNodeKAndBnB) : 
        Problem<TSP<CostEdge>, Tuple<int?, int?, string?>, TspSolver> (graph, startNodeKAndBnB, new TspSolver())
    { 
        public TravellingSalesManProblem(Matrix m, int? startNode, int? k, string? BnB, bool bidirectional = false) :
            this(
                new TSP<CostEdge>(CostGraph<CostEdge>.FromMatrix(m).Edges, bidirectional), 
                new Tuple<int?, int?, string?> (startNode, k, BnB)) { }
        public TravellingSalesManProblem(string[][] m, string? startNode, string? k, string? BnB, bool bidirectional = false) :
            this(
                new Matrix(m), 
                string.IsNullOrWhiteSpace(startNode) ? null : int.Parse(startNode) - 1,
                string.IsNullOrWhiteSpace(k) ? null : int.Parse(k) - 1,
                BnB,
                bidirectional)
        { }
    }

    /// <summary>
    /// Knapsnack problem
    /// https://en.wikipedia.org/wiki/Knapsack_problem
    /// </summary>
    public class KnapsnakProblem(Polyhedron p, Vector c, bool isBoolean = false) : 
        Problem<Polyhedron, Vector, KnapsnackProblemSolver>
            (p,
             c,
             new KnapsnackProblemSolver(isBoolean))
    {
        public KnapsnakProblem(
            Vector revenues,
            Vector volumes, 
            Fraction totalVolume, 
            Vector? weights, 
            Fraction? totalWeight, 
            bool isBoolean = false) : this(
                Polyhedron.FromRow(volumes, totalVolume) & 
                    (weights is not null && totalWeight.HasValue ? 
                Polyhedron.FromRow(weights, totalWeight.Value) : null), 
                revenues, 
                isBoolean) { }

        public KnapsnakProblem(
            IEnumerable<string> revenues,
            IEnumerable<string> volumes,
            string totalVolume,
            IEnumerable<string>? weights,
            string? totalWeight,
            bool isBoolean = false) : this(
                Vector.FromString(revenues) ?? Vector.Empty,
                Vector.FromString(volumes) ?? Vector.Empty,
                Fraction.FromString(totalVolume),
                Vector.FromString(weights),
                string.IsNullOrWhiteSpace(totalWeight) ? null : Fraction.FromString(totalWeight),
                isBoolean)
        { }
    }

    /// <summary>
    /// Assign tasks to workers
    /// </summary>
    /// <param name="costs">The cost of the row-th task to be done by the col-th worker</param>
    /// <param name="fillWorkers">If every worker should be filled</param>
    public class SimpleMinimumCostAssignmentProblem(Matrix costs, bool fillWorkers = true) :
        Problem<Matrix, bool, SimpleMinimumCostAssignSolver> (costs, fillWorkers, new SimpleMinimumCostAssignSolver())
    { 
        public SimpleMinimumCostAssignmentProblem(string[][] costs, bool fillWorkers = true) :
            this(new Matrix(costs), fillWorkers) { }
    }

    /// <summary>
    /// Assign tasks to workers
    /// </summary>
    public class GeneralizedMinimumCostAssignmentProblem :
        Problem<Tuple<Matrix, Matrix, Vector>, bool, GeneralizedMinimumCostAssignSolver>
    {
        public GeneralizedMinimumCostAssignmentProblem(Tuple<Matrix, Matrix, Vector> domain, bool fillWorkers = false) :
            base(domain, fillWorkers, new GeneralizedMinimumCostAssignSolver())
        { }

        public GeneralizedMinimumCostAssignmentProblem(Matrix costs, Matrix times, Vector timeLimits, bool fillWorkers = false) :
            this(new Tuple<Matrix, Matrix, Vector>(costs, times, timeLimits), fillWorkers)
        { }

        public GeneralizedMinimumCostAssignmentProblem(string[][] costs, string[][] times, string[] timeLimits, bool fillWorkers = false) :
            this(new Tuple<Matrix, Matrix, Vector>(
                new Matrix(costs), new Matrix(times), Vector.FromString(timeLimits) ?? Vector.Empty), fillWorkers)
        { }
    }

    #endregion

    #region NetworkProgramming

    public class MinimumCostFlowProblem(
        MinimumCostFlow<BoundedCostEdge> g,
        Tuple<int?, int?, string?> startNodeEndNodeAndUnused,
        IEnumerable<BoundedCostEdge>? T,
        IEnumerable<BoundedCostEdge>? U,
        bool UseBounds = true) : 
        Problem<MinimumCostFlow<BoundedCostEdge>, Tuple<int?, int?, string?>, McfpSolver>
            (g, startNodeEndNodeAndUnused, new McfpSolver(T, U, UseBounds))
    {
        public MinimumCostFlowProblem(
            IEnumerable<BoundedCostEdge> edges, 
            Vector balances,
            int? startNode,
            int? endNode,
            IEnumerable<BoundedCostEdge>? T,
            IEnumerable<BoundedCostEdge>? U,
            bool UseBounds = true) :
            this(
                new MinimumCostFlow<BoundedCostEdge>(edges, balances), 
                new Tuple<int?, int?, string?>(startNode, endNode, null), 
                T, 
                U, 
                UseBounds) { }


        public MinimumCostFlowProblem(
            IEnumerable<string[]> edges,
            string[] balances,
            string? startNode,
            string? endNode,
            IEnumerable<string[]>? T,
            IEnumerable<string[]>? U,
            bool UseBounds = true) :
            this(
                edges.Select(e => new BoundedCostEdge(e)), 
                Vector.FromString(balances) ?? Vector.Empty, 
                string.IsNullOrWhiteSpace(startNode) ? null : int.Parse(startNode) - 1,
                string.IsNullOrWhiteSpace(endNode) ? null : int.Parse(endNode) - 1,
                T?.Select(e => new BoundedCostEdge(e)),
                U?.Select(e => new BoundedCostEdge(e)),
                UseBounds)
        { }
    }

    #endregion

    #region NonLinearProgramming

    /// <summary>
    /// Analize a function inside a polyhedron. The function, as well as its gradient, must be written in python
    /// </summary>
    public class NonLinearProblem(Polyhedron p, string s, Vector? startingPoint = null) : 
        Problem<Polyhedron, string, NonLinearSolver>(p, s, new NonLinearSolver(startingPoint))
    {
        public NonLinearProblem(string[][] polyhedron, string s, string[]? startingPoint = null) :
            this(Polyhedron.FromStringMatrix(polyhedron), s, Vector.FromString(startingPoint)) { }
    }

    /// <summary>
    /// Analyze a poliynomial function of multiple variables inside a polyhedron
    /// </summary>
    public class QuadraticProblem(Polyhedron p, Matrix Hessian, Vector Linear) : 
        Problem<Polyhedron, Tuple<Matrix, Vector>, QuadraticSolver>
            (p,
             new Tuple<Matrix, Vector>(Hessian, Linear),
             new QuadraticSolver())
    {
        public QuadraticProblem(string[][] polyhedron, string[][] hessian, string[]? linear) :
            this(Polyhedron.FromStringMatrix(polyhedron), new Matrix(hessian), Vector.FromString(linear) ?? Vector.Empty) { }
    }

    #endregion
}
