using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Extensions
{
    public class IndentWriter : StreamWriter, ICloneable
    {
        public int Indentation { get; set; } = 0;
        public string IndentationString { get; set; } = "\t";
        private string GetIndent() => string.Join(string.Empty, Enumerable.Repeat(IndentationString, Indentation));
        public IndentWriter(Stream stream) : base(stream) { }

        private string? GetContent(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            string i = GetIndent();
            return string.Join(Environment.NewLine, 
                s.Split(Environment.NewLine)
                .Select(line => i + line));
        }

        public override void WriteLine(string? value) => base.WriteLine(GetContent(value));

        public override void Write(string? value) => base.Write(GetContent(value));

        public override async Task WriteLineAsync(string? value) => 
            await base.WriteLineAsync(GetContent(value));

        public override async Task WriteAsync(string? value) => 
            await base.WriteAsync(GetContent(value));
        public object Clone() => MemberwiseClone();
        private IndentWriter getIndented()
        {
            var clone = (IndentWriter)Clone();
            clone.Indentation++;
            return clone;
        }
        public IndentWriter Indent { get => getIndented(); }

        public static new IndentWriter Null { get => new IndentWriter(StreamWriter.Null.BaseStream); }
    }
}
