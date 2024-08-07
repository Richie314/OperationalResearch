# Operational Research problem solver
This is a Windows Forms (GUI) application that solves problems of Operational Research

## Possible problems
These are the problems that can be solved via the application
- Linear programming in primal form (via [simplex](https://en.wikipedia.org/wiki/Simplex_algorithm))
  - Step by step output
  - Gomory planes
- Linear programming in dual form (not tested)
  - Step by step output
- [Knapsnack](https://en.wikipedia.org/wiki/Knapsack_problem)
  - Lower and upper bounds
  - [Boolean version](https://en.wikipedia.org/wiki/Knapsack_problem#0-1_knapsack_problem)
    - Step by step
    - Solution via branch and bound
  - Recursive solution
- [Travelling Salesman Problem (TSP)](https://en.wikipedia.org/wiki/Travelling_salesman_problem)
  - Lower and upper bounds
  - Step by step
  - Solution via branch and bound
- [Minimum Cost Flow Problem (MCFP)](https://en.wikipedia.org/wiki/Minimum-cost_flow_problem)
  - Step by step solution (simplex for networks)
  - Max flow result
  - Minimum paths tree via Dijkstra
  - Cut of minimum capacity
- Non linear programming problems
  - [Project gradient descent](https://en.wikipedia.org/wiki/Gradient_descent) and [Franke Wolfe](https://en.wikipedia.org/wiki/Frank%E2%80%93Wolfe_algorithm) algorithms for a [IronPython](https://ironpython.net) function
  - LKKT solution for a QuadProg function
# Credits
All the algorithms are implemented following the lectures of the _Ricerca Operativa_ course of professor Massimo Pappalardo of the University of Pisa.
