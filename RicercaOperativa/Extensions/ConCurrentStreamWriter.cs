using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalResearch.Extensions
{
    public class ConCurrentStreamWriter : IndentWriter
    {
        private readonly ConcurrentQueue<Tuple<FontStyle, Color, string>> _stringQueue = new();
        private bool _disposing;
        private readonly RichTextBox _textBox;


        public ConCurrentStreamWriter(Stream stream, RichTextBox textBox)
            : base(stream)
        {
            _textBox = textBox;
            CreateQueueListener();
        }
        public override async Task WriteLineAsync()
        {
            await base.WriteLineAsync();
            _stringQueue.Enqueue(Format(Environment.NewLine));
        }
        public override void WriteLine()
        {
            base.WriteLine();
            _stringQueue.Enqueue(Format(Environment.NewLine));
        }

        public override void WriteLine(string? value)
        {
            base.WriteLine(value);
            _stringQueue.Enqueue(Format(string.Format("{0}" + Environment.NewLine, value)));
        }

        public override void Write(string? value)
        {
            base.Write(value);
            _stringQueue.Enqueue(Format(value));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _disposing = disposing;
        }

        private void CreateQueueListener()
        {
            var bw = new BackgroundWorker();

            bw.DoWork += (sender, args) =>
            {
                while (!_disposing)
                {
                    if (_stringQueue.IsEmpty || _textBox is null)
                    {
                        continue;
                    }
                    Tuple<FontStyle, Color, string>? value = null;
                    if (!_stringQueue.TryDequeue(out value))
                    {
                        continue;
                    }
                    if (value is null)
                    {
                        continue;
                    }
                    if (_textBox is null || _textBox.IsDisposed)
                    {
                        break;
                    }
                    if (_textBox.InvokeRequired)
                    {
                        _textBox.Invoke(new Action(() =>
                        {
                            if (_textBox.ForeColor != value.Item2 ||
                                _textBox.Font.Style != value.Item1)
                            {
                                _textBox.SelectionStart = _textBox.TextLength;
                                _textBox.SelectionLength = 0;


                                _textBox.SelectionColor = value.Item2;
                                _textBox.SelectionFont = new Font(_textBox.Font, value.Item1);
                                _textBox.AppendText(value.Item3);
                                _textBox.SelectionColor = _textBox.ForeColor;
                                _textBox.SelectionFont = _textBox.Font;
                            }
                            else
                            {
                                _textBox.AppendText(value.Item3);
                            }
                            _textBox.ScrollToCaret();
                        }));
                    }
                    else
                    {
                        if (_textBox.ForeColor != value.Item2 ||
                            _textBox.Font.Style != value.Item1)
                        {
                            _textBox.SelectionStart = _textBox.TextLength;
                            _textBox.SelectionLength = 0;


                            _textBox.SelectionColor = value.Item2;
                            _textBox.SelectionFont = new Font(_textBox.Font, value.Item1);
                            _textBox.AppendText(value.Item3);
                            _textBox.SelectionColor = _textBox.ForeColor;
                            _textBox.SelectionFont = _textBox.Font;
                        }
                        else
                        {
                            _textBox.AppendText(value.Item3);
                        }
                        _textBox.ScrollToCaret();
                    }
                }
            };

            bw.RunWorkerAsync();

        }

    }
}
