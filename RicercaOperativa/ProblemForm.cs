using RicercaOperativa.Models;
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
        public ProblemForm()
        {
            InitializeComponent();
            MemoryStream stream = new MemoryStream();
            Writer = new ConcurrentStreamWriter(stream, textBox);
        }
        public ConcurrentStreamWriter Writer { get; private set; }

    }
}
