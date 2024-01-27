using OperationalResearch.Extensions;
using OperationalResearch.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RicercaOperativa
{
    public partial class ProblemForm : Form
    {
        private readonly string methodName;
        public ProblemForm(string? methodName = null)
        {
            InitializeComponent();
            MemoryStream stream = new();
            Writer = new ConCurrentStreamWriter(stream, textBox);
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
