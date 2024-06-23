using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Scripting.Utils;
using OperationalResearch.Models;
using OperationalResearch.Models.Problems;

namespace RicercaOperativa
{
    public partial class QuadraticProgrammingForm : Form
    {
        private int EquationsCount = 3;
        private int VariablesCount = 3;
        public QuadraticProgrammingForm()
        {
            InitializeComponent();
        }

        private void GenerateGrid()
        {
            hessianMatrix.Rows.Clear();
            hessianMatrix.ColumnCount = VariablesCount + 1;
            hessianMatrix.RowHeadersVisible = false;

            constraintMatrix.Rows.Clear();
            constraintMatrix.ColumnCount = VariablesCount + 1;
            constraintMatrix.RowHeadersVisible = false;

            linearCoeff.Rows.Clear();
            linearCoeff.ColumnCount = VariablesCount;
            linearCoeff.RowHeadersVisible = false;

            for (int i = 0; i < hessianMatrix.ColumnCount; i++)
            {
                hessianMatrix.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                {
                    hessianMatrix.Columns[i].Width = 80;
                    hessianMatrix.Columns[i].Name = "\\";
                    continue;
                }
                hessianMatrix.Columns[i].Width = 50;
                hessianMatrix.Columns[i].Name = "x" + i;
                string[] row = ["x" + i];
                hessianMatrix.Rows.Add(row);

                linearCoeff.Columns[i - 1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                linearCoeff.Columns[i - 1].Width = 50;
                linearCoeff.Columns[i - 1].Name = "x" + i;

                constraintMatrix.Columns[i - 1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                constraintMatrix.Columns[i - 1].Width = 50;
                constraintMatrix.Columns[i - 1].Name = "x" + i;
            }
            linearCoeff.Rows.Add();
            linearCoeff.Rows[0].Height = 25;
            constraintMatrix.Columns[VariablesCount].Name = "b";
            constraintMatrix.Columns[VariablesCount].Width = 50;

            for (int i = 0; i < EquationsCount; i++)
            {
                constraintMatrix.Rows.Add();
                constraintMatrix.Rows[i].Height = 20;
            }
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


        private async void maximizeBtn_Click(object sender, EventArgs e)
        {
            var LinearPart = LinearPartString();
            var Hessian = HessianGridStr();
            if (Hessian is null)
            {
                return;
            }
            var Constarints = ConstraintGridStr();
            if (Constarints is null)
            {
                return;
            }
            maximizeBtn.Enabled = minimizeBtn.Enabled = false;

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                maximizeBtn.Enabled = minimizeBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            Problem p = new(
                Constarints,
                [],
                new QuadraticProgrammingSolver(Hessian, LinearPart));
            if (await p.SolveMax(loggers: new StreamWriter?[] { Form.Writer }))
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
        }
        private string[][]? ConstraintGridStr()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < constraintMatrix.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 0; col < constraintMatrix.ColumnCount; col++)
                {
                    containsBlank |= string.IsNullOrWhiteSpace((string)constraintMatrix[col, row].Value);
                    currRow.Add((string)constraintMatrix[col, row].Value);
                }
                list.Add([.. currRow]);
            }
            if (containsBlank)
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "There are blank cells in the constraint input," + Environment.NewLine +
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
        private string[][]? HessianGridStr()
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
            return [.. list];
        }
        private string[] LinearPartString()
        {
            var terms = new List<string>();
            for (int i = 0; i < linearCoeff.ColumnCount; i++)
            {
                terms.Add(
                    string.IsNullOrWhiteSpace((string)linearCoeff[i, 0].Value) ? "0" : (string)linearCoeff[i, 0].Value);
            }
            return [.. terms];
        }

        private async void minimizeBtn_Click(object sender, EventArgs e)
        {
            var LinearPart = LinearPartString();
            var Hessian = HessianGridStr();
            if (Hessian is null)
            {
                return;
            }
            var Constarints = ConstraintGridStr();
            if (Constarints is null)
            {
                return;
            }
            maximizeBtn.Enabled = minimizeBtn.Enabled = false;

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                maximizeBtn.Enabled = minimizeBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            Problem p = new(
                Constarints,
                [],
                new QuadraticProgrammingSolver(Hessian, LinearPart));
            if (await p.SolveMin(loggers: new StreamWriter?[] { Form.Writer }))
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
        }
    }
}
