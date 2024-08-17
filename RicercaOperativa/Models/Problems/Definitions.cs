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
    public class LinearProgrammingProblem : Problem<Polyhedron, Vector, LinearSolver>
    {
        public LinearProgrammingProblem(Polyhedron p, Vector c, int[]? startBasis = null) :
            base(p,
             c,
             new LinearSolver(startBasis),
             "Linear Programming")
        { }

        public LinearProgrammingProblem(string[][] polyhedron, string[] revenues, bool xPos = false, string[]? startBasis = null) :
            this(
                Polyhedron.FromStringMatrix(polyhedron, xPos),
                revenues,
                startBasis) { }

        public LinearProgrammingProblem(Polyhedron p, Vector revenues, string[]? startBasis = null) :
            this(
                p,
                revenues,
                startBasis is not null ?
                    startBasis.Select(x => Convert.ToInt32(x.Trim()) - 1).ToArray() :
                    null
                )
        { }
        public LinearProgrammingProblem(Polyhedron p, string[] revenues, string[]? startBasis = null) :
            this(
                p,
                Vector.FromString(revenues) ?? Vector.Empty,
                startBasis is not null ?
                    startBasis.Select(x => Convert.ToInt32(x.Trim()) - 1).ToArray() :
                    null
                )
        { }
    }


    /// <summary>
    /// Generic linear programming problem
    /// </summary>
    public class LinearProgrammingDualProblem : Problem<Polyhedron, Vector, LinearDualSolver>
    {
        public LinearProgrammingDualProblem(
            Polyhedron p, 
            Vector c, 
            int[]? startBasis = null) :
            base(p,
             c,
             new LinearDualSolver(startBasis),
             "Linear Programming in dual form")
        { }
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
    public class TravellingSalesManProblem : Problem<TSP<CostEdge>, Tuple<int?, int?, string?>, TspSolver>
    { 
        public TravellingSalesManProblem(
            TSP<CostEdge> graph, 
            Tuple<int?, int?, string?> startNodeKAndBnB) :
            base(graph, startNodeKAndBnB, new TspSolver(), "TSP")
        { }
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
    public class KnapsnakProblem : Problem<Polyhedron, Vector, KnapsnackProblemSolver>
    {
        public KnapsnakProblem(Polyhedron p, Vector c, bool isBoolean = false) : 
            base(
                p,
                c,
                new KnapsnackProblemSolver(isBoolean),
                "Knapsnack") { }
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
    public class SimpleMinimumCostAssignmentProblem :
        Problem<Matrix, bool, SimpleMinimumCostAssignSolver>
    { 
        public SimpleMinimumCostAssignmentProblem(Matrix costs, bool fillWorkers = true) : 
            base(
                costs, 
                fillWorkers, 
                new SimpleMinimumCostAssignSolver(), 
                "Minimum cost assignemnt") { }
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
            base(domain, fillWorkers, new GeneralizedMinimumCostAssignSolver(), "Generalized Minimum Cost Assignment")
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

    public class MinimumCostFlowProblem: 
        Problem<MinimumCostFlow<BoundedCostEdge>, Tuple<int?, int?, string?>, McfpSolver>
    {
        public MinimumCostFlowProblem(
            MinimumCostFlow<BoundedCostEdge> g,
            Tuple<int?, int?, string?> startNodeEndNodeAndUnused,
            IEnumerable<BoundedCostEdge>? T,
            IEnumerable<BoundedCostEdge>? U,
            bool UseBounds = true) :
            base(g, startNodeEndNodeAndUnused, new McfpSolver(T, U, UseBounds), "MCFP")
        { }

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
    public class NonLinearProblem : Problem<Polyhedron, string, NonLinearSolver>
    {
        public NonLinearProblem(
            Polyhedron p, 
            string s, 
            Vector? startingPoint = null) : 
            base(p, s, new NonLinearSolver(startingPoint), "Non linear Programming")
        { }
        public NonLinearProblem(string[][] polyhedron, string s, string[]? startingPoint = null) :
            this(Polyhedron.FromStringMatrix(polyhedron), s, Vector.FromString(startingPoint)) { }
    }

    /// <summary>
    /// Analyze a poliynomial function of multiple variables inside a polyhedron
    /// </summary>
    public class QuadraticProblem : Problem<Polyhedron, Tuple<Matrix, Vector, Vector?>, QuadraticSolver>
    {
        public QuadraticProblem(Polyhedron p, Matrix Hessian, Vector Linear, Vector? PointToStudy = null) :
            base(p,
             new Tuple<Matrix, Vector, Vector?>(Hessian, Linear, PointToStudy),
             new QuadraticSolver(),
             "Quadratic Programming")
        { }

        public QuadraticProblem(string[][] polyhedron, string[][] hessian, string[]? linear, string[]? pointToStudy = null) :
            this(Polyhedron.FromStringMatrix(polyhedron), 
                new Matrix(hessian), 
                Vector.FromString(linear) ?? Vector.Empty,
                Vector.FromString(pointToStudy)) { }
    }

    #endregion
}
