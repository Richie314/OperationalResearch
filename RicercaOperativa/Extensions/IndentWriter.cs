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

        private Color Color { get; set; } = Color.Black;
        private FontStyle FontStyle { get; set; } = FontStyle.Regular;

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

        private IndentWriter manageFont(FontStyle style)
        {
            var clone = (IndentWriter)Clone();
            clone.FontStyle = style;
            return clone;
        }
        public IndentWriter Bold { get => manageFont(FontStyle.Bold); }
        public IndentWriter Italic { get => manageFont(FontStyle.Italic); }
        public IndentWriter Strikeout { get => manageFont(FontStyle.Strikeout); }
        public IndentWriter Underline { get => manageFont(FontStyle.Underline); }
        public IndentWriter Regular { get => manageFont(FontStyle.Regular); }

        private IndentWriter manageColor(Color color)
        {
            var clone = (IndentWriter)Clone();
            clone.Color = color;
            return clone;
        }
        public IndentWriter Red { get => manageColor(Color.Red); }
        public IndentWriter Green { get => manageColor(Color.Green); }
        public IndentWriter Blue { get => manageColor(Color.Blue); }
        public IndentWriter Black { get => manageColor(Color.Black); }
        public IndentWriter White { get => manageColor(Color.White); }
        public IndentWriter Orange { get => manageColor(Color.Orange); }
        public IndentWriter Purple { get => manageColor(Color.Purple); }
        public IndentWriter Crimson { get => manageColor(Color.Crimson); }
        public IndentWriter Brown { get => manageColor(Color.Brown); }

        protected Tuple<FontStyle, Color, string> Format(string? s)
        {
            return new Tuple<FontStyle, Color, string>(FontStyle, Color, s ?? string.Empty);
        }

        public static new IndentWriter Null { get => new IndentWriter(StreamWriter.Null.BaseStream); }
    }
}
