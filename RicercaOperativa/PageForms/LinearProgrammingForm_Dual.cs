using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OperationalResearch.Models;
using OperationalResearch.Models.Problems;
using OperationalResearch.ViewForms;

namespace OperationalResearch.PageForms
{
    public partial class LinearProgrammingForm_Dual : Form
    {
        private int EquationsCount = 3;
        private int VariablesCount = 3;
        public LinearProgrammingForm_Dual()
        {
            InitializeComponent();
        }

        private void GenerateGrid()
        {
            matrix.Rows.Clear();
            matrix.ColumnCount = VariablesCount + 1;
            matrix.RowHeadersVisible = false;

            functionGrid.Rows.Clear();
            functionGrid.ColumnCount = VariablesCount;
            functionGrid.RowHeadersVisible = false;
            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                matrix.Columns[i].Width = 50;
                matrix.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i < VariablesCount)
                {
                    matrix.Columns[i].Name = "y" + (i + 1).ToString();

                    functionGrid.Columns[i].Name = "b" + (i + 1).ToString();
                    functionGrid.Columns[i].Width = 50;
                    functionGrid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    matrix.Columns[i].Name = "c";
                }
            }
            for (int i = 0; i < EquationsCount; i++)
            {
                string[] row = new string[VariablesCount + 1];
                matrix.Rows.Add(row);
                matrix.Rows[i].Height = 20;
            }
            functionGrid.Rows.Add(new string[VariablesCount]);
            startBaseInput.Clear();
        }

        private void LinearProgrammingForm_Load(object sender, EventArgs e)
        {
            try
            {
                EquationsCount = (int)equationsCountInput.Value;
                VariablesCount = (int)variablesCountInput.Value;
                GenerateGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void resetTableBtn_Click(object sender, EventArgs e)
        {
            try
            {
                EquationsCount = (int)equationsCountInput.Value;
                VariablesCount = (int)variablesCountInput.Value;
                GenerateGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private async Task Solve(bool min = true)
        {
            maximizeBtn.Enabled = minimizeBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                maximizeBtn.Enabled = minimizeBtn.Enabled = true;
                return;
            }

            LinearProgrammingDualProblem problem = new(mainGridStr, MainVectorStr(), xNonNegativeCheckbox.Checked, GetStartBasis());

            var dualForm = new ProblemForm<LinearProgrammingDualProblem>(problem, "Simplex");
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                maximizeBtn.Enabled = minimizeBtn.Enabled = true;
            };
            dualForm.FormClosed += new FormClosedEventHandler(closeFormCallback);
            dualForm.Show();

            if (min ? await problem.SolveMin([dualForm.Writer]) : await problem.SolveMax([dualForm.Writer]))
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
        private async void maximizeBtn_Click(object sender, EventArgs e) => await Solve(false);
        private async void minimizeBtn_Click(object sender, EventArgs e) => await Solve(true);

        private string[] MainVectorStr()
        {
            List<string> list = [];
            for (int i = 0; i < functionGrid.ColumnCount; i++)
            {
                list.Add((string)functionGrid[i, 0].Value);
            }
            return [.. list];
        }
        private string[][]? MainGridStr()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < matrix.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 0; col < matrix.ColumnCount; col++)
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
        private string[]? GetStartBasis()
        {
            string value = startBaseInput.Text;
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return value.Split(",", StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
