using System;
using OperationalResearch.Models.Elements;
using OperationalResearch.Models.Problems;
using OperationalResearch.ViewForms;

namespace OperationalResearch.PageForms
{
    public partial class LinearProgrammingForm_Primal : Form
    {
        public LinearProgrammingForm_Primal()
        {
            InitializeComponent();
        }
        private void SpaceDimensionChange(object? sender, PolyhedronControl.PolyhedronChangeEventArgs e)
        {
            linearFunctionControl1.setSize(e.SpaceDimension, sender, e);
        }
        private void LinearProgrammingForm_Load(object sender, EventArgs e)
        {
            polyhedronControl1.OnPolyhedronChange += new PolyhedronControl.PolyhedronChangeHandler(SpaceDimensionChange);
            linearFunctionControl1.setSize(polyhedronControl1.SpaceDimension, sender, e);
        }

        private async Task SolveProblem(bool max = true)
        {
            if (polyhedronControl1.Polyhedron is null)
            {
                MessageBox.Show(
                    "Could not load the polyhedron!", 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                return;
            }
            maximizeBtn.Enabled = minimizeBtn.Enabled = false;

            LinearProgrammingProblem problem = new(
                polyhedronControl1.Polyhedron,
                linearFunctionControl1.Vector ?? Vector.Empty,
                GetStartBasis());

            var realForm = new ProblemForm<LinearProgrammingProblem>(problem, "Simplex");
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                maximizeBtn.Enabled = minimizeBtn.Enabled = true;
            };
            realForm.FormClosed += new FormClosedEventHandler(closeFormCallback);
            realForm.Show();


            bool realSolved = max ?
                await problem.SolveMax(loggers: [realForm.Writer]) :
                await problem.SolveMin(loggers: [realForm.Writer]);

            var intForm = new ProblemForm<LinearProgrammingProblem>(problem, "Libraries");
            intForm.Show();

            bool intSolved = max ?
                await problem.SolveIntegerMax(loggers: [intForm.Writer]) :
                await problem.SolveIntegerMin(loggers: [intForm.Writer]);
            if (realSolved)
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

            if (problem.Solver.Domain?.Cols == 2)
            {
                var graphForm = new CartesianForm([], problem.Solver.Domain);
                graphForm.Show();
            }
        }

        private async void maximizeBtn_Click(object sender, EventArgs e) => await SolveProblem(true);
        private async void minimizeBtn_Click(object sender, EventArgs e) => await SolveProblem(false);

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
