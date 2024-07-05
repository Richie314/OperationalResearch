using OperationalResearch.Extensions;
using OperationalResearch.Models.Problems;
using System;

namespace OperationalResearch.ViewForms
{
    public partial class ProblemForm<ProblemType> : Form
        where ProblemType : IProblem
    {
        private readonly string methodName;
        public ProblemType Problem;
        public readonly MemoryStream Stream;
        public ProblemForm(ProblemType p, string? methodName = null)
        {
            Problem = p;
            InitializeComponent();
            Stream = new();
            Writer = new ConCurrentStreamWriter(Stream, textBox);
            this.methodName = methodName ?? string.Empty;
        }
        public ConCurrentStreamWriter Writer { get; private set; }

        private void ProblemForm_Load(object sender, EventArgs e)
        {
            textBox.SetInnerMargins(15, 0, 5, 10);
            if (!string.IsNullOrWhiteSpace(methodName))
            {
                Text = $"Solving problem with {methodName} method";
            }
        }
    }
}
