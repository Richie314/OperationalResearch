using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Extensions
{
    public class IndentWriter : StreamWriter
    {
        public int Indentation { get; set; } = 0;
        public string IndentationString { get; set; } = "\t";
        private string GetIndent() => string.Join(string.Empty, Enumerable.Repeat(IndentationString, Indentation));
        public IndentWriter(Stream stream) : base(stream)
        {
        }
        public override void WriteLine(string? value) => base.WriteLine(GetIndent() + value);

        public override void Write(string? value) => base.Write(GetIndent() + value);
        public IndentWriter Indent() => new IndentWriter(BaseStream) { Indentation = Indentation + 1 };

        public static new IndentWriter Null { get => new IndentWriter(StreamWriter.Null.BaseStream); }
    }
}
