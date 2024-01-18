using System.Windows.Controls;

namespace RicercaOperativa
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }


        private void LinearProgrammingMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new LinearProgrammingForm();
            NewForm.Show();
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
    }
}
