using OperationalResearch.Extensions;
using OperationalResearch.Models.Problems.Solvers;

namespace OperationalResearch.Models.Problems
{
    public class Problem<Domain, CoDomain, SolverClass> : IProblem 
        where SolverClass : ISolving<Domain, CoDomain>
    {
        public readonly SolverClass Solver;
        private readonly string LogCollectionName;
        public Problem(Domain domain, CoDomain coDomain, SolverClass solver, 
            string problemName = "problem")
        {
            Solver = solver;
            Solver.SetData(domain, coDomain);

            DateTime dateTime = DateTime.Now;
            LogCollectionName = problemName + "-" + dateTime.ToString();
            ObjectLogManager.Log<DateTime>(
                LogCollectionName,
                "Problem class generated",
                dateTime);
        }
        
        public async Task<bool> SolveMin(IEnumerable<IndentWriter?> loggers)
        {
            foreach (var logger in loggers)
            {
                if (logger is null) continue;
                logger.LogCollectionId = LogCollectionName;
            }
            return await Task.Run(() => Solver.SolveMinAsync(loggers));
        }
            
        public async Task<bool> SolveMax(IEnumerable<IndentWriter?> loggers)
        {
            foreach (var logger in loggers)
            {
                if (logger is null) continue;
                logger.LogCollectionId = LogCollectionName;
            }
            return await Task.Run(() => Solver.SolveMaxAsync(loggers));
        }

        public async Task<bool> SolveIntegerMin(IEnumerable<IndentWriter?> loggers)
        {
            foreach (var logger in loggers)
            {
                if (logger is null) continue;
                logger.LogCollectionId = LogCollectionName;
            }
            return await Task.Run(() => Solver.SolveIntegerMinAsync(loggers));
        }
            
        public async Task<bool> SolveIntegerMax(IEnumerable<IndentWriter?> loggers)
        {
            foreach (var logger in loggers)
            {
                if (logger is null) continue;
                logger.LogCollectionId = LogCollectionName;
            }
            return await Task.Run(() => Solver.SolveIntegerMaxAsync(loggers));
        }
        public IEnumerable<LoggedObject<object>> Logs
        {
            get => ObjectLogManager.Get(LogCollectionName);
        }

        public IEnumerable<LoggedObject<T>> LogsOfType<T>() =>
            ObjectLogManager.GetOfType<T>(LogCollectionName);
    }
}
