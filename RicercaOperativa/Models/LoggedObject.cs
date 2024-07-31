using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Models
{
    internal class LoggedObject<T>
    {
        public required string Group { get; set; }
        public required string Label { get; set; }
        public T? Value { get; set; } = default;
    }
}
