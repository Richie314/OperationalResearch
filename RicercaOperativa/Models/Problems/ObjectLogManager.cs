using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models.Problems
{
    public static class ObjectLogManager
    {
        private static Dictionary<string, List<LoggedObject<object>>> Logs = [];

        public static void Log<T>(
            string listId,
            string label,
            object? value)
        {
            Log(listId, new LoggedObject<object>() { Label = label, Value = value });
        }
        public static void Log(string listId, LoggedObject<object> loggedObject)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(listId, nameof(listId));
            ArgumentNullException.ThrowIfNull(loggedObject, nameof(loggedObject));

            if (!Logs.Keys.Contains(listId))
            {
                Logs[listId] = [];
            }
            Logs[listId].Add(loggedObject);
        }
        public static IEnumerable<LoggedObject<object>> Get(string listId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(listId, nameof(listId));

            if (!Logs.Keys.Contains(listId))
            {
                throw new DataMisalignedException($"List '{listId}' not found");
            }

            return Logs[listId];
        }

        public static IEnumerable<LoggedObject<T>> GetOfType<T>(string listId) =>
            Get(listId)
                .Where(x => x.Value is T)
                .Select(x => new LoggedObject<T>()
                {
                    Label = x.Label,
                    Value = (T?)x.Value
                });

    }
}
