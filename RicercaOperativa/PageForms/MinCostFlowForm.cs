using Accord.Math;
using OperationalResearch.Models.Problems;
using OperationalResearch.ViewForms;
using System.Data;
using Matrix = OperationalResearch.Models.Elements.Matrix;
using Vector = OperationalResearch.Models.Elements.Vector;

namespace OperationalResearch.PageForms
{
    public partial class MinCostFlowForm : Form
    {
        private int EdgeCount { get => (int) edges.Value; }
        private int NodeCount { get => (int)nodes.Value; }
        public MinCostFlowForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            // Edges grid

            while (matrix.Rows.Count > EdgeCount)
            {
                matrix.Rows.RemoveAt(matrix.Rows.Count - 1);
            }
            while (matrix.Rows.Count < EdgeCount)
            {
                string[] row = new string[matrix.ColumnCount];
                matrix.Rows.Add(row);
                matrix.Rows[matrix.Rows.Count - 1].Height = 20;
            }

            // Node balances grid

            while (balances.Rows.Count > NodeCount)
            {
                balances.Rows.RemoveAt(balances.Rows.Count - 1);
            }
            while (balances.Rows.Count < NodeCount)
            {
                balances.Rows.Add([""]);
                balances.Rows[balances.Rows.Count - 1].Height = 20;
            }
        }

        private void setNumBtn_Click(object sender, EventArgs e)
        {
            try
            {
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

        private void MinCostFlowForm_Load(object sender, EventArgs e)
        {
            try
            {
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
                for (int col = 0; col < matrix.ColumnCount - 2; col++)
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
                    row => row.Select(x => string.IsNullOrWhiteSpace(x) ? "0" : x).ToArray())
                    .Where(row => row.Any(cell => cell != "0") // Ignore blank rows
                    ).ToList();
            }
            return [.. list];
        }
        private string[][]? getStartTreeOrSaturated(int colToCheck)
        {
            var list = new List<string[]>();
            for (int row = 0; row < matrix.RowCount; row++)
            {
                string boolParse = 
                    string.IsNullOrWhiteSpace((string?)matrix[colToCheck, row].Value) ?
                        "False" : (string)matrix[colToCheck, row].Value;
                if (bool.Parse(boolParse))
                {
                    continue;
                }
                List<string> currRow = [];
                for (int col = 0; col < matrix.ColumnCount - 2; col++)
                {
                    currRow.Add((string)matrix[col, row].Value);
                }
                list.Add([.. currRow]);
            }
            return [.. list];
        }
        private string[][]? getStartBasis() => getStartTreeOrSaturated(5);
        private string[][]? getSaturatedArcs() => getStartTreeOrSaturated(6);
        private string[] NodeBalances()
        {
            var list = new List<string>();
            for (int row = 0; row < balances.RowCount; row++)
            {
                string? s = (string)balances[0, row].Value;
                list.Add(string.IsNullOrWhiteSpace(s) ? "0" : s);
            }
            return list.ToArray();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            var mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                button2.Enabled = true;
                return;
            }
            var startTree = getStartBasis();
            var saturated = getSaturatedArcs();
            var balances = NodeBalances();
            var startNodeStr = startNode.Text;
            var endNodeStr = endNode.Text;

            MinimumCostFlowProblem problem = new(
                mainGridStr,
                balances,
                startNodeStr,
                endNodeStr,
                startTree,
                saturated,
                true);

            var Form = new ProblemForm<MinimumCostFlowProblem>(problem, "Simplex for networks");
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                button2.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            if (await problem.SolveMin([Form.Writer]))
            {
                MessageBox.Show(
                    "Flow problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Flow problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
        }
    }

}
