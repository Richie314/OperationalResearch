using Accord.Math;
using Fractions;
using OperationalResearch.Models;
using OperationalResearch.Models.Graphs;
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
using Matrix = OperationalResearch.Models.Elements.Matrix;
using Vector = OperationalResearch.Models.Elements.Vector;

namespace OperationalResearch
{
    public partial class MinCostFlowForm : Form
    {
        private int N;
        public MinCostFlowForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            matrix.Rows.Clear();
            boundsGrid.Rows.Clear();

            matrix.ColumnCount = N + 2;
            boundsGrid.ColumnCount = N + 1;

            matrix.RowHeadersVisible = false;
            boundsGrid.RowHeadersVisible = false;

            for (int i = 0; i < matrix.ColumnCount - 1; i++)
            {
                matrix.Columns[i].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                boundsGrid.Columns[i].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                if (i == 0)
                {
                    matrix.Columns[i].Name = "From\\To";
                    boundsGrid.Columns[i].Name = "From\\To";
                    matrix.Columns[i].Width = 50;
                    boundsGrid.Columns[i].Width = 50;
                    continue;
                }
                matrix.Columns[i].Name = i.ToString();
                boundsGrid.Columns[i].Name = i.ToString();
                matrix.Columns[i].Width = 50;
                boundsGrid.Columns[i].Width = 50;
            }
            matrix.Columns[N + 1].Name = "b";
            matrix.Columns[N + 1].Width = 70;
            matrix.Columns[N + 1].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            for (int i = 0; i < N; i++)
            {
                string[] row = new string[N + 1];
                row[0] = (i + 1).ToString();
                row[i + 1] = "0";
                matrix.Rows.Add(row);
                boundsGrid.Rows.Add(row);
                matrix.Rows[i].Height = 20;
                boundsGrid.Rows[i].Height = 20;
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
                for (int col = 1; col < matrix.ColumnCount - 1; col++)
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
        private static Matrix? StrToFraction(string[][]? s)
        {
            if (s is null)
                return null;
            return new Matrix(s.Apply(row => row.Apply(Fraction.FromString)));
        }
        private Matrix LowerBound()
        {
            return new Matrix(
                Enumerable.Repeat(
                Enumerable.Repeat(Fraction.Zero, N).ToArray(), N).ToArray());
        }
        private Matrix? UpperBound()
        {
            var list = new List<string[]>();
            for (int row = 0; row < boundsGrid.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 1; col < boundsGrid.ColumnCount; col++)
                {
                    currRow.Add((string)boundsGrid[col, row].Value);
                }
                list.Add([.. currRow]);
            }
            list = list.Select(
                row => row.Select(x => string.IsNullOrWhiteSpace(x) ? "0" : x).ToArray()).ToList();
            string[][] m = list.ToArray();
            return new Matrix(m.Apply(row => row.Apply(Fraction.FromString)));
        }
        private BoundedGraphEdge[]? getStartBase()
        {
            if (string.IsNullOrWhiteSpace(startBase.Text))
                return null;
            return startBase.Text
                .Split(',', StringSplitOptions.TrimEntries)
                .Select(pair => new BoundedGraphEdge(new Graph.Edge(pair)))
                .ToArray();
        }
        private BoundedGraphEdge[]? getStartU()
        {
            if (string.IsNullOrWhiteSpace(startU.Text))
                return null;
            return startU.Text
                .Split(',', StringSplitOptions.TrimEntries)
                .Select(pair => new BoundedGraphEdge(new Graph.Edge(pair)))
                .ToArray();
        }
        private Vector getB()
        {
            List<Fraction> list = new List<Fraction>();
            for (int i = 0; i < N; i++)
            {
                string s = (string)matrix[N + 1, i].Value;
                s = string.IsNullOrWhiteSpace(s) ? "0" : s;
                list.Add(Fraction.FromString(s));
            }
            return list.ToArray();
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            var c = StrToFraction(MainGridStr());
            if (c is null)
                return;
            Vector b = getB();
            MinCostFlow m = new MinCostFlow(
                c, b, LowerBound(), null);

            var Form = new ProblemForm();
            Form.Show();
            if (await m.FlowUnbounded(getStartBase(), Form.Writer))
            {
                MessageBox.Show(
                    "Network Programming problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Network Programming problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var c = StrToFraction(MainGridStr());
            if (c is null)
                return;
            Vector b = getB();
            var u = UpperBound();
            MinCostFlow m = new MinCostFlow(
                c, b, LowerBound(), u);

            var Form = new ProblemForm();
            Form.Show();
            if (await m.FlowBounded(getStartBase(), getStartU(), Form.Writer))
            {
                MessageBox.Show(
                    "Network Programming problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Network Programming problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "K = ", "Node to start from", string.Empty, 0, 0);
            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int value))
            {
                return;
            }
            value -= 1;
            if (value < 0 || value >= N)
            {
                return;
            }
            var m = StrToFraction(MainGridStr());
            if (m is null)
                return;
            Graph g = Graph.FromMatrix(m);

            var Form = new ProblemForm();
            Form.Show();
            try
            {
                var res = await g.Dijkstra(Form.Writer, startNode: value);
                if (res is null)
                {
                    await Form.Writer.WriteLineAsync("Probem not solved");
                } else
                {
                    await Form.Writer.WriteLineAsync($"Solution:");
                    await Form.Writer.WriteLineAsync($"p = {Function.Print(res.P)}");
                    await Form.Writer.WriteLineAsync($"π = {res.PI}");
                }
            }
            catch (Exception ex)
            {
                await Form.Writer.WriteLineAsync(ex.ToString());
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "s-t", "Node to start from and arrive to", "1-" + N, 0, 0);
            if (string.IsNullOrWhiteSpace(input) || !input.Contains('-'))
            {
                return;
            }
            string[] parts = input.Split('-');
            if (parts.Length != 2)
            {
                return;
            }
            if (!int.TryParse(parts[0], out int s) || !int.TryParse(parts[1], out int t))
            {
                return;
            }
            s--; t--;
            if (s < 0 || s >= N || t < 0 || t >= N)
            {
                return;
            }

            var c = StrToFraction(MainGridStr());
            if (c is null)
                return;
            Vector b = getB();
            var u = UpperBound();
            MinCostFlow m = new MinCostFlow(
                c, b, LowerBound(), u);

            var Form = new ProblemForm();
            Form.Show();
            try
            {
                await m.MinFlowMaxCut(s, t, Form.Writer);
            }
            catch (Exception ex) { 
                await Form.Writer.WriteLineAsync(ex.ToString());
            }
        }
    }

}
