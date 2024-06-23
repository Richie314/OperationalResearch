using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Math;
using OperationalResearch.Models;
using OperationalResearch.Models.Problems;
using OperationalResearch.ViewForms;
using Matrix = OperationalResearch.Models.Matrix;

namespace RicercaOperativa
{
    public partial class LinearProgrammingForm_Primal : Form
    {
        private int EquationsCount = 3;
        private int VariablesCount = 3;
        private bool IntSolutions = false;
        public LinearProgrammingForm_Primal(bool IntSolutions = false)
        {
            this.IntSolutions = IntSolutions;
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
                    matrix.Columns[i].Name = "x" + (i + 1).ToString();

                    functionGrid.Columns[i].Name = "c" + (i + 1).ToString();
                    functionGrid.Columns[i].Width = 50;
                    functionGrid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    matrix.Columns[i].Name = "b";
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
        private void EditGrid()
        {
            while (matrix.Columns.Count > VariablesCount + 1)
            {
                matrix.Columns.RemoveAt(matrix.Columns.Count - 1);
            }
            while (matrix.Columns.Count < VariablesCount + 1)
            {
                matrix.Columns.Add(new DataGridViewColumn(matrix.Columns[0].CellTemplate));
                matrix.Columns[matrix.Columns.Count - 1].Width = 50;
                matrix.Columns[matrix.Columns.Count - 1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            while (functionGrid.Columns.Count > VariablesCount)
            {
                functionGrid.Columns.RemoveAt(functionGrid.Columns.Count - 1);
            }
            while (functionGrid.Columns.Count < VariablesCount)
            {
                functionGrid.Columns.Add(new DataGridViewColumn(functionGrid.Columns[0].CellTemplate));
                functionGrid.Columns[functionGrid.Columns.Count - 1].Width = 50;
                functionGrid.Columns[functionGrid.Columns.Count - 1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                if (i < VariablesCount)
                {
                    matrix.Columns[i].Name = "x" + (i + 1).ToString();
                    functionGrid.Columns[i].Name = "c" + (i + 1).ToString();
                }
                else
                {
                    matrix.Columns[i].Name = "b";
                }
            }

            while (matrix.Rows.Count > EquationsCount)
            {
                matrix.Rows.RemoveAt(matrix.Rows.Count - 1);
            }
            while (matrix.Rows.Count < EquationsCount)
            {
                string[] row = new string[VariablesCount + 1];
                matrix.Rows.Add(row);
                matrix.Rows[matrix.Rows.Count - 1].Height = 20;
            }

        }

        private void LinearProgrammingForm_Load(object sender, EventArgs e)
        {
            try
            {
                xNonNegativeCheckbox.Checked = true;
                EquationsCount = (int)equationsCountInput.Value;
                VariablesCount = (int)variablesCountInput.Value;
                GenerateGrid();
                if (IntSolutions)
                {
                    Text = "Integer Linear Programming - Primal Form";
                    startSimplexBtn.Text = "Solve for ints";
                }
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
                EditGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(this, err.Message, "An error happened", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private async Task SolveLP()
        {
            startSimplexBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                startSimplexBtn.Enabled = true;
                return;
            }
            var primalForm = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                startSimplexBtn.Enabled = true;
            };
            primalForm.FormClosed += new FormClosedEventHandler(closeFormCallback);
            primalForm.Show();
            int[]? startBase = GetStartBase();

            Problem p = new(
                solver: new LinearProgrammingPrimal(startBase, xNonNegativeCheckbox.Checked),
                sMatrixAndB: mainGridStr,
                sVecC: MainVectorStr());
            if (p.getMainMatrix().Columns() == 2)
            {
                var graphForm = new CartesianForm(
                    new Matrix(p.getMainMatrix()) | p.getMainVetcor());
                graphForm.Show();
            }
            if (await p.SolveMax(loggers: new StreamWriter?[] { primalForm.Writer, null }))
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
            startSimplexBtn.Enabled = true;
        }
        private async Task SolveILP()
        {
            startSimplexBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                startSimplexBtn.Enabled = true;
                return;
            }
            var primalForm = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                startSimplexBtn.Enabled = true;
            };
            primalForm.FormClosed += new FormClosedEventHandler(closeFormCallback);
            primalForm.Show();

            Problem p = new(
                solver: new IntegerLinearProgrammingPrimal(xNonNegativeCheckbox.Checked),
                sMatrixAndB: mainGridStr,
                sVecC: MainVectorStr());
            if (p.getMainMatrix().Columns() == 2)
            {
                var graphForm = new CartesianForm(
                    new Matrix(p.getMainMatrix()) | p.getMainVetcor());
                graphForm.Show();
            }
            if (await p.SolveMax(loggers: new StreamWriter?[] { primalForm.Writer }))
            {
                MessageBox.Show(
                    "Integer Linear Programming problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Integer Linear Programming problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
            startSimplexBtn.Enabled = true;
        }
        private async void startSimplexBtn_Click(object sender, EventArgs e)
        {
            if (IntSolutions)
            {
                await SolveILP();
            } else
            {
                await SolveLP();
            }

        }
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
        private int[]? GetStartBase()
        {
            string value = startBaseInput.Text;
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            try
            {
                return value.Split(
                    ",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()) - 1).ToArray();
            }
            catch
            {
                return null;
            }
        }

    }
}
