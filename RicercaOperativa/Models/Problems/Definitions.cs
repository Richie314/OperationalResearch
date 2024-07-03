using System;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.Graphs;
using OperationalResearch.Models.Problems.Solvers;

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
             new LinearSolver(startBasis)) { }


    /// <summary>
    /// Generic linear programming problem
    /// </summary>
    public class LinearProgrammingDualProblem(Polyhedron p, Vector c, int[]? startBasis = null) :
        Problem<Polyhedron, Vector, LinearDualSolver>
            (p,
             c,
             new LinearDualSolver(startBasis))
    { }

    #endregion

    #region IntegerLinearProgramming

    /// <summary>
    /// Travelling salesman problem (TSP) -> find hamiltonian cycle
    /// https://en.wikipedia.org/wiki/Travelling_salesman_problem
    /// </summary>
    public class TravellingSalesManProblem (TSP<CostEdge> graph, int? startNode) : 
        Problem<TSP<CostEdge>, int?, TspSolver> (graph, startNode, new TspSolver()) { }

    /// <summary>
    /// Knapsnack problem
    /// https://en.wikipedia.org/wiki/Knapsack_problem
    /// </summary>
    public class KnapsnakProblem(Polyhedron p, Vector c, bool isBoolean = false) : 
        Problem<Polyhedron, Vector, KnapsnackProblemSolver>
            (p,
             c,
             new KnapsnackProblemSolver(isBoolean)) { }

    /// <summary>
    /// Assign tasks to workers
    /// </summary>
    /// <param name="costs">The cost of the row-th task to be done by the col-th worker</param>
    /// <param name="fillWorkers">If every worker should be filled</param>
    public class SimpleMinimumCostAssignmentProblem(Matrix costs, bool fillWorkers = true) :
        Problem<Matrix, bool, SimpleMinimumCostAssignSolver> (costs, fillWorkers, new SimpleMinimumCostAssignSolver ()) { }

    #endregion

    #region NetworkProgramming

    public class MinimumCostFlowProblem(
        MinimumCostFlow<BoundedCostEdge> g,
        int? startNode,
        IEnumerable<BoundedCostEdge> T,
        IEnumerable<BoundedCostEdge> U,
        bool UseBounds = true) : 
        Problem<MinimumCostFlow<BoundedCostEdge>, int?, McfpSolver>
            (g, startNode, new McfpSolver(T, U, UseBounds)) { }

    #endregion

    #region NonLinearProgramming

    /// <summary>
    /// Analize a function inside a polyhedron. The function, as well as its gradient, must be written in python
    /// </summary>
    public class NonLinearProblem(Polyhedron p, string s, Vector? startingPoint = null) : 
        Problem<Polyhedron, string, NonLinearSolver>(p, s, new NonLinearSolver(startingPoint)) { }

    /// <summary>
    /// Analyze a poliynomial function of multiple variables inside a polyhedron
    /// </summary>
    public class QuadraticProblem(Polyhedron p, Matrix Hessian, Vector Linear) : 
        Problem<Polyhedron, Tuple<Matrix, Vector>, QuadraticSolver>
            (p,
             new Tuple<Matrix, Vector>(Hessian, Linear),
             new QuadraticSolver()) { }

    #endregion
}
