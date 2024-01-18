using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RicercaOperativa.Models
{
    public class ConcurrentStreamWriter : StreamWriter
    {
        private ConcurrentQueue<string> _stringQueue = new ConcurrentQueue<string>();
        private bool _disposing;
        private RichTextBox _textBox;


        public ConcurrentStreamWriter(Stream stream, RichTextBox textBox)
            : base(stream)
        {
            _textBox = textBox;
            CreateQueueListener();
        }
        public override async Task WriteLineAsync()
        {
            await base.WriteLineAsync();
            _stringQueue.Enqueue(Environment.NewLine);
        }
        public override void WriteLine()
        {
            base.WriteLine();
            _stringQueue.Enqueue(Environment.NewLine);
        }

        public override void WriteLine(string? value)
        {
            base.WriteLine(value);
            _stringQueue.Enqueue(string.Format("{0}" + Environment.NewLine, value));
        }

        public override void Write(string? value)
        {
            base.Write(value);
            _stringQueue.Enqueue(value ?? string.Empty);
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
                    if (_stringQueue.Count == 0 || _textBox is null)
                    {
                        continue;
                    }
                    string value = string.Empty;
                    if (!_stringQueue.TryDequeue(out value))
                    {
                        continue;
                    }
                    if (_textBox.InvokeRequired)
                    {
                        _textBox.Invoke(new Action(() =>
                        {
                            _textBox.AppendText(value);
                            _textBox.ScrollToCaret();
                        }));
                    }
                    else
                    {
                        _textBox.AppendText(value);
                        _textBox.ScrollToCaret();
                    }
                }
            };

            bw.RunWorkerAsync();

        }

    }
}
