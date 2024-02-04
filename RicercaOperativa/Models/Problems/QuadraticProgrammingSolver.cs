using Accord.Math;
using Fractions;
using IronPython.Hosting;
using OperationalResearch.Models.Python;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    internal class QuadraticProgrammingSolver : IProgrammingInterface
    {
        private readonly Matrix H;
        private readonly Vector lin;
        private Fraction[,] A;
        private Vector b;
        public QuadraticProgrammingSolver(Matrix H, Vector lin)
        {
            ArgumentNullException.ThrowIfNull(H, nameof(H));
            ArgumentNullException.ThrowIfNull(lin, nameof(lin));
            this.H = H;
            this.lin = lin;
        }
        public QuadraticProgrammingSolver(string[][] H, string[] lin) : 
            this(new Matrix(  H.Select(  r => r.Select(Fraction.FromString).ToArray()  ).ToArray()   ), 
                lin.Select(Fraction.FromString).ToArray())
        {
        }

        public void SetMainMatrix(Fraction[,] matrix)
        {
            A = matrix;
        }
        public void SetFirstVector(Vector v)
        {
            b = v;
        }
        public void SetSecondVector(Vector v)
        {
            // Do nothing
        }
        public async Task<bool> SolveMinAsync(IEnumerable<StreamWriter?> streams)
        {
            StreamWriter Writer = streams.FirstOrDefault() ?? StreamWriter.Null;
            try
            {
                await Writer.WriteLineAsync("Finding min through Accord.Math.QuadProg");
                QuadProg q = new(H, lin, new Matrix(A), b);
                Vector? x = q.Minimize();
                if (x is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved");
                    return false;
                }

                await Writer.WriteLineAsync($"X = {x}");
                return true;
            } catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }
        public async Task<bool> SolveMaxAsync(IEnumerable<StreamWriter?> streams)
        {
            StreamWriter Writer = streams.FirstOrDefault() ?? StreamWriter.Null;
            try
            {
                await Writer.WriteLineAsync("Finding max through Accord.Math.QuadProg");
                QuadProg q = new(H, lin, new Matrix(A), b);
                Vector? x = q.Maximize();
                if (x is null)
                {
                    await Writer.WriteLineAsync("Problem was not solved");
                    return false;
                }

                await Writer.WriteLineAsync($"X = {x}");
                return true;
            }
            catch (Exception ex)
            {
                await Writer.WriteLineAsync($"Exception happened: '{ex.Message}'");
#if DEBUG
                if (ex.StackTrace is not null)
                {
                    await Writer.WriteLineAsync($"Stack Trace: {ex.StackTrace}");
                }
#endif
                return false;
            }
        }
    }
}
