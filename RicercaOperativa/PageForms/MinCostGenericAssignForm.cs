using Accord.Math;
using OperationalResearch.Models;
using OperationalResearch.Models.Problems;
using RicercaOperativa;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationalResearch
{
    public partial class MinCostGenericAssignForm : Form
    {
        private int N;
        private int M;
        public MinCostGenericAssignForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            costsMatrix.Rows.Clear();
            timeMatrix.Rows.Clear();

            costsMatrix.ColumnCount = N + 1;
            timeMatrix.ColumnCount = N + 1;

            costsMatrix.RowHeadersVisible = false;
            timeMatrix.RowHeadersVisible = false;

            for (int i = 0; i < costsMatrix.ColumnCount; i++)
            {
                costsMatrix.Columns[i].DefaultCellStyle.Alignment =
                timeMatrix.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                {
                    costsMatrix.Columns[i].Name = timeMatrix.Columns[i].Name = "Jobs\\Workers";
                    costsMatrix.Columns[i].Width = timeMatrix.Columns[i].Width = 120;
                    continue;
                }
                costsMatrix.Columns[i].Name = timeMatrix.Columns[i].Name = "w" + i;
                costsMatrix.Columns[i].Width = timeMatrix.Columns[i].Width = 50;
            }
            for (int i = 0; i < M + 1; i++)
            {
                if (i == M)
                {
                    string[] bRow = new string[N + 1];
                    bRow[0] = "b";
                    timeMatrix.Rows.Add(bRow);
                    timeMatrix.Rows[i].Height = 25;
                    continue;
                }
                string[] row = new string[N + 1];
                row[0] = "j" + (i + 1);
                costsMatrix.Rows.Add(row); timeMatrix.Rows.Add(row);
                costsMatrix.Rows[i].Height = timeMatrix.Rows[i].Height = 20;
            }
        }

        private void setNumBtn_Click(object sender, EventArgs e)
        {
            try
            {
                N = (int)n.Value;
                M = (int)m.Value;
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

        private void MinCostAssignForm_Load(object sender, EventArgs e)
        {
            try
            {
                N = (int)n.Value;
                M = (int)m.Value;
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
        private string[][]? CostMatrixStr()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < costsMatrix.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 1; col < costsMatrix.ColumnCount; col++)
                {
                    containsBlank |= string.IsNullOrWhiteSpace((string)costsMatrix[col, row].Value);
                    currRow.Add((string)costsMatrix[col, row].Value);
                }
                list.Add([.. currRow]);
            }
            if (containsBlank)
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "There are blank cells in the cost matrix," + Environment.NewLine +
                    "The algorithms cannot run with unknown values." + Environment.NewLine,
                    "Blank cells",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning))
                {
                    return null;
                }
            }
            return [.. list];
        }
        private string[][]? TimeMatrixStr()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < timeMatrix.RowCount - 1; row++)
            {
                List<string> currRow = [];
                for (int col = 1; col < timeMatrix.ColumnCount; col++)
                {
                    containsBlank |= string.IsNullOrWhiteSpace((string)timeMatrix[col, row].Value);
                    currRow.Add((string)timeMatrix[col, row].Value);
                }
                list.Add([.. currRow]);
            }
            if (containsBlank)
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "There are blank cells in the time matrix," + Environment.NewLine +
                    "The algorithms cannot run with unknown values." + Environment.NewLine,
                    "Blank cells",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning))
                {
                    return null;
                }
            }
            return [.. list];
        }
        private string[]? MaxTimeVector()
        {
            bool containsBlank = false;
            List<string> currRow = [];
            for (int col = 1; col < timeMatrix.ColumnCount; col++)
            {
                containsBlank |= string.IsNullOrWhiteSpace(
                    (string)timeMatrix[col, timeMatrix.RowCount - 1].Value);
                currRow.Add((string)timeMatrix[col, timeMatrix.RowCount - 1].Value);
            }
            if (containsBlank)
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "There are blank cells in the max time vector," + Environment.NewLine +
                    "The algorithms cannot run with unknown values." + Environment.NewLine,
                    "Blank cells",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning))
                {
                    return null;
                }
            }
            return [.. currRow];
        }
        private async void button_Click(object sender, EventArgs e)
        {
            button.Enabled = false;
            string[][]? costMatrix = CostMatrixStr();
            if (costMatrix is null)
            {
                button.Enabled = true;
                return;
            }
            string[][]? timeMatrix = TimeMatrixStr();
            if (timeMatrix is null)
            {
                button.Enabled = true;
                return;
            }
            string[]? maxTimeVector = MaxTimeVector();
            if (maxTimeVector is null)
            {
                button.Enabled = true;
                return;
            }

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                button.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            Problem p = new(
                solver: new MinCostGenericAssignSolver(),
                sMatrix: costMatrix,
                sVecB: timeMatrix.Flatten(),
                sVecC: maxTimeVector);
            if (await p.SolveMin(loggers: new StreamWriter?[] { Form.Writer }))
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
            button.Enabled = true;
        }
    }

}
