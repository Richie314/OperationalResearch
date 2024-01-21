using System.Windows.Controls;

namespace RicercaOperativa
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }


        private void CreditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new AboutForm();
            NewForm.Show();
        }

        private void NonlinearProgrammingMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new NonLinearProgrammingForm();
            NewForm.Show();
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }

        private void primalFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new LinearProgrammingForm_Primal();
            NewForm.Show();
        }

        private void dualFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new LinearProgrammingForm_Dual();
            NewForm.Show();
        }
    }
}
