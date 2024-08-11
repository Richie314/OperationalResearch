# Operational Research problem solver
[![Build, test and release](https://github.com/Richie314/OperationalResearch/actions/workflows/dotnet-desktop.yml/badge.svg?branch=master)](https://github.com/Richie314/OperationalResearch/actions/workflows/dotnet-desktop.yml)

[![Winget distribution](https://github.com/Richie314/OperationalResearch/actions/workflows/winget.yml/badge.svg)](https://github.com/Richie314/OperationalResearch/actions/workflows/winget.yml)

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
    - Solution via [Branch and Bound](https://en.wikipedia.org/wiki/Branch_and_bound)
  - Recursive solution
- [Travelling Salesman Problem (TSP)](https://en.wikipedia.org/wiki/Travelling_salesman_problem)
  - Lower and upper bounds
  - Step by step
  - Solution via [Branch and Bound](https://en.wikipedia.org/wiki/Branch_and_bound)
- [Minimum Cost Flow Problem (MCFP)](https://en.wikipedia.org/wiki/Minimum-cost_flow_problem)
  - Step by step solution (simplex for networks)
  - [Max flow and cut of minimum capacity](https://en.wikipedia.org/wiki/Max-flow_min-cut_theorem)
  - [Shortest paths tree](https://en.wikipedia.org/wiki/Shortest-path_tree) via [Dijkstra](https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm)
- Non linear programming problems
  - [Project gradient descent](https://en.wikipedia.org/wiki/Gradient_descent) and [Franke Wolfe](https://en.wikipedia.org/wiki/Frank%E2%80%93Wolfe_algorithm) algorithms for a [IronPython](https://ironpython.net) function
  - LKKT solution for a QuadProg function
# Credits
All the algorithms are implemented following the lectures of the _Ricerca Operativa_ course of professor Massimo Pappalardo of the University of Pisa.
