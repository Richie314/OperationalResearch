using OperationalResearch.ViewForms;
using OperationalResearch.Models;
using System.Data;
using OperationalResearch.Models.Problems;

namespace OperationalResearch.PageForms
{
    public partial class KnapsnackForm : Form
    {
        private int N;
        public KnapsnackForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            mainGrid.Rows.Clear();
            mainGrid.ColumnCount = N + 2;
            mainGrid.RowHeadersVisible = false;

            for (int i = 0; i < mainGrid.ColumnCount; i++)
            {
                mainGrid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                {
                    mainGrid.Columns[i].Name = "Parameters\\Items";
                    mainGrid.Columns[i].Width = 150;
                    continue;
                }
                if (i == N + 1)
                {
                    mainGrid.Columns[i].Name = "Max";
                    mainGrid.Columns[i].Width = 100;
                    continue;
                }
                mainGrid.Columns[i].Name = "#" + i;
                mainGrid.Columns[i].Width = 50;
            }
            mainGrid.Rows.Add(["Values"]);
            mainGrid.Rows.Add(["Volumes"]);
            mainGrid.Rows.Add(["Weights"]);
        }
        private void applyItemsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                N = (int)numberOfItems.Value;
                GenerateGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    err.Message, "An error happened",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void solveBtn_Click(object sender, EventArgs e)
        {
            solveBtn.Enabled = false;

            var revenues = Revenues();
            var volumes = Volumes();
            var weights = Weights();

            if (revenues is null || volumes is null)
            {
                MessageBox.Show(
                    "There are blank cells in the input," + Environment.NewLine +
                    "The algorithms cannot run with unknown values." + Environment.NewLine,
                    "Fill blank cells first",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                solveBtn.Enabled = true;
                return;
            }

            KnapsnakProblem problem = new(
                revenues, 
                volumes.Item1, 
                volumes.Item2, 
                weights?.Item1, 
                weights?.Item2, 
                boolean.Checked);

            var Form = new ProblemForm<KnapsnakProblem>(problem, "Knapsnack");
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                solveBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            if (await problem.SolveMax([ Form.Writer ]))
            {
                MessageBox.Show(
                    "Knapsnack problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Knapsnack problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
        }

        private IEnumerable<string>? Revenues()
        {
            List<string> currRow = [];
            for (int col = 1; col < mainGrid.ColumnCount - 1; col++)
            {
                if (string.IsNullOrWhiteSpace((string)mainGrid[col, 0].Value))
                {
                    return null;
                }
                currRow.Add((string)mainGrid[col, 0].Value);
            }
            return currRow;
        }

        private Tuple<IEnumerable<string>, string>? GetRow(int index)
        {
            if (string.IsNullOrWhiteSpace((string)mainGrid[mainGrid.ColumnCount - 1, index].Value))
            {
                return null;
            }

            List<string> currRow = [];
            for (int col = 1; col < mainGrid.ColumnCount - 1; col++)
            {
                if (string.IsNullOrWhiteSpace((string)mainGrid[col, index].Value))
                {
                    return null;
                }
                currRow.Add((string)mainGrid[col, index].Value);
            }
            return new Tuple<IEnumerable<string>, string>(currRow, (string)mainGrid[mainGrid.ColumnCount - 1, index].Value);
        }

        private Tuple<IEnumerable<string>, string>? Volumes() => GetRow(1);
        private Tuple<IEnumerable<string>, string>? Weights() => GetRow(2);

        private void KnapsnackForm_Load(object sender, EventArgs e)
        {
            try
            {
                N = (int)numberOfItems.Value;
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
    }
}
