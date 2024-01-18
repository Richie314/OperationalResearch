using RicercaOperativa.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static IronPython.SQLite.PythonSQLite;

namespace RicercaOperativa
{
    public partial class NonLinearProgrammingForm : Form
    {

        private int EquationsCount = 3;
        private int VariablesCount = 3;
        public NonLinearProgrammingForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            matrix.Rows.Clear();
            matrix.ColumnCount = VariablesCount + 1;
            matrix.RowHeadersVisible = false;

            startPointInput.Rows.Clear();
            startPointInput.ColumnCount = VariablesCount;
            startPointInput.RowHeadersVisible = false;

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                matrix.Columns[i].Width = 50;
                matrix.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i < VariablesCount)
                {
                    matrix.Columns[i].Name = "x" + (i + 1).ToString();

                    startPointInput.Columns[i].Width = 50;
                    startPointInput.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    startPointInput.Columns[i]. Name = "x" + (i + 1).ToString();
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
            startPointInput.Rows.Add(new string[VariablesCount]);
            startPointInput.Rows[0].Height = 25;


            GetPythonSnippet();
        }
        private void GetPythonSnippet()
        {
            pythonInput.Clear();
            string libs = "import math" + Environment.NewLine;
            string comment =
                "# Expect input to be made of floats. Return the same number of floats as a tuple" +    Environment.NewLine +
                "# Return a single float in the main function" +                                        Environment.NewLine +
                "# Return the same number of floats as a tuple in the gradient function" +              Environment.NewLine +
                "# Use 4 spaces instead of tabs!" +                                                     Environment.NewLine +
                "# Do not import strange libraries" +                                                   Environment.NewLine;

            IEnumerable<string> xArray = Enumerable.Range(1, VariablesCount).Select(i => $"x{i}");
            IEnumerable<string> xFloatArray = xArray.Select(x => $"{x}: float");
            IEnumerable<string> xSquareArray = xArray.Select(x => $"{x}**2");
            string f =
                "def f(" + string.Join(", ", xFloatArray.ToArray()) + "):" +                        Environment.NewLine +
                "    # The function to minimize, remember to use 4 spaces instead of tabs" +        Environment.NewLine +
                "    return (" + string.Join(" + ", xSquareArray.ToArray()) + ") / 2" +             Environment.NewLine;
            string grad =
                "def gradF(" + string.Join(", ", xFloatArray.ToArray()) + "):" +                    Environment.NewLine +
                "    # Code here, remember to use 4 spaces instead of tabs" +                       Environment.NewLine +
                "    return " + string.Join(", ", xArray.ToArray()) +                               Environment.NewLine;
            string wholeScript =
                libs +
                comment +
                Environment.NewLine +
                f +
                Environment.NewLine +
                grad;
                
            pythonInput.Text = wholeScript;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                EquationsCount = (int)numberOfEquationsInput.Value;
                VariablesCount = (int)dimensionOfSpaceInput.Value;
                GenerateGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void NonLinearProgrammingForm_Load(object sender, EventArgs e)
        {
            try
            {

                EquationsCount = (int)numberOfEquationsInput.Value;
                VariablesCount = (int)dimensionOfSpaceInput.Value;
                GenerateGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private async void solveButton_Click(object sender, EventArgs e)
        {
            solveButton.Enabled = false;
            var dialogForm = new ProblemForm();
            FormClosedEventHandler closeFormCallback = (object? sender, FormClosedEventArgs e) =>
            {
                solveButton.Enabled = true;
            };
            dialogForm.FormClosed += closeFormCallback;
            dialogForm.Show();

            Problem p = new Problem(
                solver: new NonLinearProgramming(pythonInput.Text, GetStartingPoint()),
                sMatrixAndB: MainGridStr(),
                sVecC: new string[0]);

            if (await p.Solve(dialogForm.Writer))
            {
                MessageBox.Show(
                    "Non Linear Programming problem solved",
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Non Linear Programming problem could not be solved",
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }
        }
        private string[][] MainGridStr()
        {
            var list = new List<string[]>();
            for (int row = 0; row < matrix.RowCount; row++)
            {
                List<string> currRow = new List<string>();
                for (int col = 0; col < matrix.ColumnCount; col++)
                {
                    currRow.Add((string)matrix[col, row].Value);
                }
                list.Add(currRow.ToArray());
            }
            return list.ToArray();
        }
        private string[]? GetStartingPoint()
        {
            var arr = new string[VariablesCount];
            for (int col = 0; col < startPointInput.ColumnCount; col++)
            {
                arr[col] = (string)startPointInput[col, 0].Value;
                if (string.IsNullOrWhiteSpace(arr[col]))
                {
                    return null;
                }
            }
            return arr;
        }
    }
}
