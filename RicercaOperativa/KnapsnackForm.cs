using OperationalResearch.Models.Problems;
using OperationalResearch.Models;
using RicercaOperativa;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OperationalResearch
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
            mainGrid.Rows.Add(new string[] { "Values" });//.Concat(Enumerable.Repeat(string.Empty, N))
            mainGrid.Rows.Add(new string[] { "Volumes" });//.Concat(Enumerable.Repeat(string.Empty, N))
            mainGrid.Rows.Add(new string[] { "Weights" });//.Concat(Enumerable.Repeat(string.Empty, N))
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
                    MessageBoxIcon.Warning);
            }
        }

        private async void solveBtn_Click(object sender, EventArgs e)
        {
            solveBtn.Enabled = false;

            var mainGridStr = GetMatrix();
            var mainVectorStr = GetVec();

            if (mainGridStr is null)
            {
                solveBtn.Enabled = true;
                return;
            }

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                solveBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            Problem p = new(
                solver: new KnapsnackProblemSolver(boolean.Checked),
                sMatrix: mainGridStr,
                sVecB: new string[0],
                sVecC: mainVectorStr);
            if (await p.SolveMax(loggers: new StreamWriter?[] { Form.Writer }))
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
        private string[][]? GetMatrix()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < mainGrid.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 1; col < mainGrid.ColumnCount - 1; col++)
                {
                    containsBlank |= string.IsNullOrWhiteSpace((string)mainGrid[col, row].Value);
                    currRow.Add((string)mainGrid[col, row].Value);
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
        private string[] GetVec()
        {
            return new string[] {
                (string)mainGrid[N + 1, 1].Value,
                (string)mainGrid[N + 1, 2].Value
            }.Select(x => string.IsNullOrWhiteSpace(x) ? "1" : x).ToArray();
        }

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
