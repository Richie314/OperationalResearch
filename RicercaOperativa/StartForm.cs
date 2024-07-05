using OperationalResearch.ViewForms;
using OperationalResearch.PageForms;

namespace OperationalResearch
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
        private void productionRevenueMaximazationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new LinearProgrammingForm_Primal();
            NewForm.Show();
        }

        private void assignmentOfMinCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new MinCostAssignForm();
            NewForm.Show();
        }
        private void genericAssignmentOfMinCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new MinCostGenericAssignForm();
            NewForm.Show();
        }

        private void knapsnacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new KnapsnackForm();
            NewForm.Show();
        }

        private void travellingSalesmansProblemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new TSPForm();
            NewForm.Show();
        }

        private void genericProblemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new NonLinearProgrammingForm();
            NewForm.Show();
        }

        private void quadraticProgrammingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new QuadraticProgrammingForm();
            NewForm.Show();
        }

        private void flowOfMinCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new MinCostFlowForm();
            NewForm.Show();
        }

        private void webviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var NewForm = new CartesianForm([]);
            NewForm.Show();
        }
    }
}
