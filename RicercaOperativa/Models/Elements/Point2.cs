using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Elements
{
    internal struct Point2
    {
        public Fraction x, y;
        public string Label { get; set; }
        private static int UnNamedPointCounter = 0;
        public static Point2 FromVector(Vector x, string? label = null)
        {
            if (x.Size != 2)
            {
                throw new ArgumentException("Invalid size");
            }
            return new Point2() { 
                x = x[0], 
                y = x[1], 
                Label = string.IsNullOrWhiteSpace(label) ? UnNamedPointCounter++.ToString() : label
            };
        }

        public static Tuple<Point2, Point2>? FromLogs(LoggedObject<Tuple<Vector, Vector>> log)
        {
            ArgumentNullException.ThrowIfNull(log, nameof(log));
            if (log.Value is null)
            {
                return null;
            }
            try
            {
                return new Tuple<Point2, Point2>(
                    FromVector(log.Value.Item1), 
                    FromVector(log.Value.Item2, log.Label));
            } catch (ArgumentException) {
                return null;
            }
        } 
    }
}
