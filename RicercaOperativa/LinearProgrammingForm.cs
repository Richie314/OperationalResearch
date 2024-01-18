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

namespace RicercaOperativa
{
    public partial class LinearProgrammingForm : Form
    {
        private int EquationsCount = 3;
        private int VariablesCount = 3;
        public LinearProgrammingForm()
        {
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


        private async void startSimplexBtn_Click(object sender, EventArgs e)
        {
            startSimplexBtn.Enabled = false;
            var dialogForm = new ProblemForm();
            FormClosedEventHandler closeFormCallback = (object? sender, FormClosedEventArgs e) =>
                        {
                            startSimplexBtn.Enabled = true;
                        };
            dialogForm.FormClosed += closeFormCallback;
            dialogForm.Show();
            int[]? startBase = GetStartBase();
 
            Problem p = new Problem(
                solver: new LinearProgramming(startBase),
                sMatrixAndB: MainGridStr(),
                sVecC: MainVectorStr());

            if (await p.Solve(dialogForm.Writer))
            {
                MessageBox.Show(
                    "Linear Programming problem solved", 
                    "Problem solved", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            } else
            {
                MessageBox.Show(
                    "Linear Programming problem could not be solved", 
                    "Error", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
            }

        }
        private string[] MainVectorStr()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < functionGrid.ColumnCount; i++)
            {
                list.Add((string)functionGrid[i, 0].Value);
            }
            return list.ToArray();
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
            } catch {
                return null;
            }
        }
    }
}
