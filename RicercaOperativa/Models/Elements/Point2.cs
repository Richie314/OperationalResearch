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
        public static Point2 FromVector(Vector? x, string? label = null)
        {
            if (x is null || x.Size != 2)
            {
                throw new ArgumentException("Invalid size");
            }
            return new Point2() { 
                x = x[0], 
                y = x[1], 
                Label = string.IsNullOrWhiteSpace(label) ? $"P_{UnNamedPointCounter++}" : label
            };
        }

        public static Tuple<Point2, Point2>? FromLogs(Tuple<Vector, Vector>? fromAndV, string label)
        {
            if (fromAndV is null)
            {
                return null;
            }
            try
            {
                return new Tuple<Point2, Point2>(
                    FromVector(fromAndV.Item1), 
                    FromVector(fromAndV.Item2, label));
            } catch (ArgumentException) {
                return null;
            }
        }


        public static Point2 operator +(Point2 point1, Point2 point2) =>
            new Point2()
            {
                x = point1.x + point2.x,
                y = point1.y + point2.y,
                Label = $"P_{UnNamedPointCounter++}"
            };
        public static Point2 operator -(Point2 point1, Point2 point2) =>
            new Point2()
            {
                x = point1.x - point2.x,
                y = point1.y - point2.y,
                Label = $"P_{UnNamedPointCounter++}"
            };
    }
}
