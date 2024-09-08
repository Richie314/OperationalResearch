using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;
using OperationalResearch.Extensions;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.Models.NonLinearOptimization
{
    public abstract class NonLinearOptimizer
    {
        public readonly Polyhedron P;
        public NonLinearOptimizer(Polyhedron p)
        {
            ArgumentNullException.ThrowIfNull(p, nameof(p));
            P = p;
        }

        #region MethodsToImplement
        public abstract Fraction Function(Vector x);
        public abstract Vector Gradient(Vector x);
        public abstract string? FindPhiRepresentation(Vector x, Vector d);

        #endregion

        #region AlreadyProvidedMethods

        /// <summary>
        /// Finds the t between t_start and t_end that has the min value of
        /// f(x + dt)
        /// </summary>
        /// <param name="t_start">The left bound of the time</param>
        /// <param name="t_end">The right bound of the time</param>
        /// <param name="steps">How many steps to do</param>
        /// <param name="x">The vector x</param>
        /// <param name="d">The vector d</param>
        /// <returns>The first t that takes the function to its min value</returns>
        private Fraction FindArgMinOfFunction(
            Fraction t_start,
            Fraction t_end,
            int steps,
            Vector x,
            Vector d) => Models.Function.FindArgMin(
                t => Function(x + d * t), // Phi(t)
                t_start, t_end, steps);

        /// <summary>
        /// Finds the t between t_start and t_end that has the max value of
        /// f(x + dt)
        /// </summary>
        /// <param name="t_start">The left bound of the time</param>
        /// <param name="t_end">The right bound of the time</param>
        /// <param name="steps">How many steps to do</param>
        /// <param name="x">The vector x</param>
        /// <param name="d">The vector d</param>
        /// <returns>The first t that takes the function to its max value</returns>
        private Fraction FindArgMaxOfFunction(
            Fraction t_start,
            Fraction t_end,
            int steps,
            Vector x,
            Vector d) =>
            Models.Function.FindArgMax(
                t => Function(x + d * t), // Phi(t)
                t_start, t_end, steps);

        /// <summary>
        /// Finds min or max of phi(t) = f(x + t * d)
        /// </summary>
        /// <param name="FindMin">Find min or max</param>
        /// <param name="t_start">The starting instant to check</param>
        /// <param name="t_end">The end instant to check</param>
        /// <param name="steps">How many steps to do between t_end and t_start</param>
        /// <param name="x">The x vector</param>
        /// <param name="d">The direction vector</param>
        /// <returns>The t where phi(t) is min or max</returns>
        public Fraction FindArgOfFunction(
            bool FindMin,
            Fraction t_start,
            Fraction t_end,
            int steps,
            Vector x,
            Vector d) => FindMin ?
                FindArgMinOfFunction(t_start, t_end, steps, x, d) :
                FindArgMaxOfFunction(t_start, t_end, steps, x, d);

        #endregion
    }
}
