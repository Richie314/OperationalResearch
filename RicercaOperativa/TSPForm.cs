﻿using OperationalResearch.Models;
using OperationalResearch.Models.Problems;
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

namespace OperationalResearch
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

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                findHamiltonCycleBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            Problem p = new(
                solver: new TravellingSalesmanProblemSolver(considerSymmetric.Checked),
                sMatrix: mainGridStr,
                sVecB: [],
                sVecC: []);
            if (await p.SolveMin(loggers: new StreamWriter?[] { Form.Writer }))
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

        private async void showKTreeBtn_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "K = ", "Chose the node to exclude", string.Empty, 0, 0);
            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int value))
            {
                return;
            }
            value -= 1;
            if (value < 0 || value >= N)
            {
                return;
            }
            findHamiltonCycleBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                findHamiltonCycleBtn.Enabled = true;
                return;
            }
            var solver = new TravellingSalesmanProblemSolver(considerSymmetric.Checked);
            var problem = new Problem(mainGridStr, [], [], solver);
            solver.SetMainMatrix(problem.getMainMatrix());

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                findHamiltonCycleBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            var g = await solver.GetGraph.FindKTree(value, considerSymmetric.Checked, Form.Writer);
            if (g is null)
            {
                findHamiltonCycleBtn.Enabled = true;
                return;
            }

            new GraphForm(new Graph(g)).Show(Form);
        }

        private async void nearestNodeBtn_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Starting node = ", "Chose the node to start from", string.Empty, 0, 0);
            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int value))
            {
                return;
            }
            value -= 1;
            if (value < 0 || value >= N)
            {
                return;
            }
            findHamiltonCycleBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                findHamiltonCycleBtn.Enabled = true;
                return;
            }
            var solver = new TravellingSalesmanProblemSolver(considerSymmetric.Checked);
            var problem = new Problem(mainGridStr, [], [], solver);
            solver.SetMainMatrix(problem.getMainMatrix());

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                findHamiltonCycleBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            var G = solver.GetGraph;
            var g = await G.NearestNodeUpperEstimate(Form.Writer, value);
            if (g is null)
            {
                findHamiltonCycleBtn.Enabled = true;
                return;
            }
            var edges = G.GetEdges(g, considerSymmetric.Checked);
            if (edges is null)
            {
                findHamiltonCycleBtn.Enabled = true;
                return;
            }

            new GraphForm(new Graph(N, edges)).Show(Form);
        }
    }

}
