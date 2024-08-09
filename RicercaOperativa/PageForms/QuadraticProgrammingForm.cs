using System;
using System.Data;
using Microsoft.Scripting.Utils;
using OperationalResearch.ViewForms;
using OperationalResearch.Models.Problems;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.PageForms
{
    public partial class QuadraticProgrammingForm : Form
    {
        public QuadraticProgrammingForm()
        {
            InitializeComponent();
        }

        private void GenerateGrid()
        {
            hessianMatrix.RowHeadersVisible = false;
            while (hessianMatrix.RowCount < polyhedronControl1.SpaceDimension)
            {
                hessianMatrix.ColumnCount++;
                int i = hessianMatrix.ColumnCount - 1;
                hessianMatrix.Columns[i]
                    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                hessianMatrix.Columns[i].Width = 50;
                if (i == 0)
                {
                    hessianMatrix.Columns[i].Name = "\\";
                    continue;
                }
                hessianMatrix.Columns[i].Name = "x" + i;

                string[] row = ["x" + i];
                hessianMatrix.Rows.Add(row);
            }
            while (hessianMatrix.RowCount > polyhedronControl1.SpaceDimension)
            {
                hessianMatrix.ColumnCount--;
                hessianMatrix.RowCount--;
            }
        }

        private void SpaceDimensionChange(object? sender, PolyhedronControl.PolyhedronChangeEventArgs e)
        {
            linearFunctionControl1.setSize(e.SpaceDimension, sender, e);
            linearFunctionControl2.setSize(e.SpaceDimension, sender, e);
            GenerateGrid();
        }
        private void LinearProgrammingForm_Load(object sender, EventArgs e)
        {
            polyhedronControl1.OnPolyhedronChange += 
                new PolyhedronControl.PolyhedronChangeHandler(SpaceDimensionChange);
            linearFunctionControl1.setSize(polyhedronControl1.SpaceDimension, sender, e);
            linearFunctionControl2.setSize(polyhedronControl1.SpaceDimension, sender, e);
            GenerateGrid();
        }
        private async Task solve(bool max)
        {
            var Hessian = HessianGrid();
            if (Hessian is null)
            {
                return;
            }

            if (polyhedronControl1.Polyhedron is null)
            {
                MessageBox.Show(
                    "Could not load the polyhedron!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            QuadraticProblem problem = new(
                polyhedronControl1.Polyhedron,
                Hessian,
                linearFunctionControl1.Vector ?? Vector.Empty,
                linearFunctionControl2.Vector);

            maximizeBtn.Enabled = minimizeBtn.Enabled = false;
            var Form = new ProblemForm<QuadraticProblem>(problem, "QuadProg");
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                maximizeBtn.Enabled = minimizeBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            bool solved = max ?
                await problem.SolveMax([Form.Writer]) :
                await problem.SolveMin([Form.Writer]);
            if (solved)
            {
                MessageBox.Show(
                    "Quadratic Programming problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Quadratic Programming problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
            maximizeBtn.Enabled = minimizeBtn.Enabled = true;

            if (problem.Solver.Domain?.Cols == 2)
            {
                var graphForm = new CartesianForm([], problem.Solver.Domain);
                graphForm.Show();
            }
        }
        private async void maximizeBtn_Click(object sender, EventArgs e) => await solve(true);
        private async void minimizeBtn_Click(object sender, EventArgs e) => await solve(false);

        private Matrix? HessianGrid()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < hessianMatrix.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 1; col < hessianMatrix.ColumnCount; col++)
                {
                    containsBlank |= string.IsNullOrWhiteSpace((string)hessianMatrix[col, row].Value);
                    currRow.Add((string)hessianMatrix[col, row].Value);
                }
                list.Add([.. currRow]);
            }
            if (containsBlank)
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "There are blank cells in the hessian input," + Environment.NewLine +
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
            return new Matrix([.. list]);
        }

    }
}
