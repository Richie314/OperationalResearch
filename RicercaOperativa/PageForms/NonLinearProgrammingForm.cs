using OperationalResearch.Models;
using OperationalResearch.Models.Problems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using OperationalResearch.ViewForms;
using OperationalResearch.Models.Elements;

namespace OperationalResearch.PageForms
{
    public partial class NonLinearProgrammingForm : Form
    {

        public NonLinearProgrammingForm()
        {
            InitializeComponent();
        }

        private async void SpaceDimensionChange(object? sender, PolyhedronControl.PolyhedronChangeEventArgs e)
        {
            linearFunctionControl1.setSize(e.SpaceDimension, sender, e);
            await pythonFunctionControl1.setSize(e.SpaceDimension);
        }


        private async Task Solve(bool max)
        {
            //
            // Retrieve the parameters
            //
            if (polyhedronControl1.Polyhedron is null)
            {
                MessageBox.Show(
                    "Invalid polyhedron", 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
                return;
            }
            var code = await pythonFunctionControl1.getCode();
            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show(
                    "Empty code." + Environment.NewLine + "Write your function to continue", 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
                return;
            }

            //
            // Create the problem
            // 
            NonLinearProblem problem = new(
                polyhedronControl1.Polyhedron, 
                code, 
                startingPoint: linearFunctionControl1.Vector);

            //
            // Create the windows
            //
            var dialogForm1 = new ProblemForm<NonLinearProblem>(problem, "Projected Gradient Descent");
            var dialogForm2 = new ProblemForm<NonLinearProblem>(problem, "Franke-Wolfe");
            bool OneFormDisposed = false;
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                if (OneFormDisposed)
                {
                    solveMaxButton.Enabled = solveMinButton.Enabled = true;
                }
                OneFormDisposed = true;
            };
            solveMaxButton.Enabled = solveMinButton.Enabled = false;
            dialogForm1.FormClosed += new FormClosedEventHandler(closeFormCallback);
            dialogForm1.Show();
            dialogForm2.FormClosed += new FormClosedEventHandler(closeFormCallback);
            dialogForm2.Show();

            if (max ? 
                await problem.SolveMax([dialogForm1.Writer, dialogForm2.Writer]) :
                await problem.SolveMin([dialogForm1.Writer, dialogForm2.Writer]))
            {
                MessageBox.Show(
                    "Non Linear Programming problem solved",
                    "Problem solved", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Non Linear Programming problem could not be solved",
                    "Error", 
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }

            //
            // Eventually display the domain
            //
            if (problem.Solver.Domain?.Cols == 2)
            {
                var graphForm = new CartesianForm(
                    problem.LogsOfType<Vector>()
                        .Select(t => Point2.FromVector(t.Value, t.Label)),
                    problem.Solver.Domain,
                    problem.LogsOfType<Tuple<Vector, Vector>>()
                        .Select(t => Point2.FromLogs(t.Value, t.Label))
                );
                graphForm.Show();
            }
        }


        private async void solveMinButton_Click(object sender, EventArgs e) => await Solve(max: false);
        private async void solveMaxButton_Click(object sender, EventArgs e) => await Solve(max: true);

        private async void NonLinearProgrammingForm_Load(object sender, EventArgs e)
        {
            polyhedronControl1.OnPolyhedronChange += new PolyhedronControl.PolyhedronChangeHandler(SpaceDimensionChange);
            linearFunctionControl1.setSize(polyhedronControl1.SpaceDimension, sender, e);
            await pythonFunctionControl1.setSize(polyhedronControl1.SpaceDimension);
        }
    }
}
