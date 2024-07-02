using System;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.Problems.Solvers;

namespace OperationalResearch.Models.Problems
{
    /// <summary>
    /// Generic linear programming problem
    /// </summary>
    public class LinearProgrammingProblem : Problem<Polyhedron, Vector, LinearSolver>
    {
        public LinearProgrammingProblem(Polyhedron p, Vector c, int[]? startBasis = null) : 
            base(p,
                 c,
                 new LinearSolver(startBasis)) { }
    }

    /// <summary>
    /// Travelling salesman problem (TSP) -> find hamiltonian cycle
    /// https://en.wikipedia.org/wiki/Travelling_salesman_problem
    /// </summary>
    public class TravellingSalesManProblem : Problem<Polyhedron, Vector, TspSolver>
    {
        public TravellingSalesManProblem(Matrix m, bool Symmetric = false) : 
            base(new Polyhedron(m, Vector.Zeros(m.Rows)), Vector.Empty, new TspSolver(Symmetric)) { }
    }

    /// <summary>
    /// Knapsnack problem
    /// https://en.wikipedia.org/wiki/Knapsack_problem
    /// </summary>
    public class KnapsnakProblem : Problem<Polyhedron, Vector, KnapsnackProblemSolver>
    {
        public KnapsnakProblem(Polyhedron p, Vector c, bool isBoolean = false) :
            base(p,
                 c,
                 new KnapsnackProblemSolver(isBoolean))
        { }
    }

    /// <summary>
    /// Analize a function inside a polyhedron. The function, as well as its gradient, must be written in python
    /// </summary>
    public class NonLinearProblem : Problem<Polyhedron, string, NonLinearSolver>
    { 
        public NonLinearProblem(Polyhedron p, string s, Vector? startingPoint = null) :
            base(p,
                 s,
                 new NonLinearSolver(startingPoint))
        { }
    }

    public class QuadraticProblem : Problem<Polyhedron, Tuple<Matrix, Vector>, QuadraticSolver>
    {
        public QuadraticProblem(Polyhedron p, Matrix Hessian, Vector Linear) : 
            base(p,
                 new Tuple<Matrix, Vector>(Hessian, Linear),
                 new QuadraticSolver())
        { }
    }
}
