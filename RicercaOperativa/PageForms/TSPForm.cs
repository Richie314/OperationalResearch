using System;
using System.Data;
using OperationalResearch.Models.Problems;
using OperationalResearch.ViewForms;

namespace OperationalResearch.PageForms
{
    public partial class TSPForm : Form
    {
        private int N;
        public TSPForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            matrix.Rows.Clear();
            matrix.ColumnCount = N + 1;
            matrix.RowHeadersVisible = false;

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                matrix.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                {
                    matrix.Columns[i].Name = "From\\To";
                    matrix.Columns[i].Width = 180;
                    continue;
                }
                matrix.Columns[i].Name = i.ToString();
                matrix.Columns[i].Width = 50;
            }
            for (int i = 0; i < N; i++)
            {
                string[] row = new string[N + 1];
                row[0] = (i + 1).ToString();
                row[i + 1] = "0";
                matrix.Rows.Add(row);
                matrix.Rows[i].Height = 20;
            }
        }

        private void setNumBtn_Click(object sender, EventArgs e)
        {
            try
            {
                N = (int)n.Value;
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

        private async void findHamiltonCycleBtn_Click(object sender, EventArgs e)
        {
            findHamiltonCycleBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                findHamiltonCycleBtn.Enabled = true;
                return;
            }

            string BnB = branchAndBound.Text;
            string k = this.k.Text;
            string startNode = this.startNode.Text;

            TravellingSalesManProblem problem = new(mainGridStr, startNode, k, BnB, bidirectional: considerSymmetric.Checked);

            var Form = new ProblemForm<TravellingSalesManProblem>(problem);
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                findHamiltonCycleBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            if (await problem.SolveMin([Form.Writer]))
            {
                MessageBox.Show(
                    "TSP solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "TSP could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
        }

        private void TravellingSalesmanProblemForm_Load(object sender, EventArgs e)
        {
            try
            {
                N = (int)n.Value;
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
                    containsBlank |= string.IsNullOrWhiteSpace((string)matrix[col, row].Value);
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
