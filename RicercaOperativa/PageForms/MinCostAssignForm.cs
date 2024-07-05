using System.Data;
using OperationalResearch.ViewForms;
using OperationalResearch.Models.Problems;

namespace OperationalResearch.PageForms
{
    public partial class MinCostAssignForm : Form
    {
        private int Number;
        public MinCostAssignForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            matrix.Rows.Clear();
            matrix.ColumnCount = Number + 1;
            matrix.RowHeadersVisible = false;

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                matrix.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                {
                    matrix.Columns[i].Name = "Jobs\\Workers";
                    matrix.Columns[i].Width = 200;
                    continue;
                }
                matrix.Columns[i].Name = "w" + i;
                matrix.Columns[i].Width = 50;
            }
            for (int i = 0; i < Number; i++)
            {
                string[] row = new string[Number + 1];
                row[0] = "j" + (i + 1);
                matrix.Rows.Add(row);
                matrix.Rows[i].Height = 20;
            }
        }

        private void setNumBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Number = (int)n.Value;
                GenerateGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    err.Message, "An error happened",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async void solveBtn_Click(object sender, EventArgs e)
        {
            solveBtn.Enabled  = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                solveBtn.Enabled = true;
                return;
            }

            SimpleMinimumCostAssignmentProblem problem = new(mainGridStr, true);

            bool closedOneWindow = false;

            var RealForm = new ProblemForm<SimpleMinimumCostAssignmentProblem>(problem, "Simplex (real results)");
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                if (!closedOneWindow)
                {
                    closedOneWindow = true;
                    return;
                }
                solveBtn.Enabled = true;
            };
            RealForm.FormClosed += new FormClosedEventHandler(closeFormCallback);
            RealForm.Show();

            var IntegerForm = new ProblemForm<SimpleMinimumCostAssignmentProblem>(problem, "Libray (integer results)");
            IntegerForm.FormClosed += new FormClosedEventHandler(closeFormCallback);
            IntegerForm.Show();

            bool realResult = await problem.SolveMin(loggers: [RealForm.Writer]);
            bool intsResult = await problem.SolveIntegerMin(loggers: [RealForm.Writer]);

            if (realResult || intsResult)
            {
                MessageBox.Show(
                    "Linear Programming problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Linear Programming problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
        }

        private void MinCostAssignForm_Load(object sender, EventArgs e)
        {
            try
            {
                Number = (int)n.Value;
                GenerateGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    err.Message, "An error happened",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        private string[][]? MainGridStr()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < matrix.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 1; col < matrix.ColumnCount; col++)
                {
                    containsBlank = containsBlank || string.IsNullOrWhiteSpace((string)matrix[col, row].Value);
                    currRow.Add((string)matrix[col, row].Value);
                }
                list.Add([.. currRow]);
            }
            if (containsBlank)
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "There are blank cells in the input," + Environment.NewLine +
                    "The algorithms cannot run with unknown values." + Environment.NewLine +
                    "Do you want to fill in with zeros?",
                    "Blank cells",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning))
                {
                    return null;
                }
                list = list.Select(
                    row => row.Select(x => string.IsNullOrWhiteSpace(x) ? "0" : x).ToArray()).ToList();
            }
            return [.. list];
        }

    }

}
