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
    public partial class MinCostAssignForm : Form
    {
        private int Number;
        public MinCostAssignForm()
        {
            InitializeComponent();
        }
        private void GenerateGrid()
        {
            matrix.Rows.Clear();
            matrix.ColumnCount = Number + 1;
            matrix.RowHeadersVisible = false;

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                matrix.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                {
                    matrix.Columns[i].Name = "Jobs\\Workers";
                    matrix.Columns[i].Width = 200;
                    continue;
                }
                matrix.Columns[i].Name = "w" + i;
                matrix.Columns[i].Width = 50;
            }
            for (int i = 0; i < Number; i++)
            {
                string[] row = new string[Number + 1];
                row[0] = "j" + (i + 1);
                matrix.Rows.Add(row);
                matrix.Rows[i].Height = 20;
            }
        }

        private void setNumBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Number = (int)n.Value;
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

        private async void solveCooperativeBtn_Click(object sender, EventArgs e)
        {
            solveCooperativeBtn.Enabled = solveNonCooperativeBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                solveCooperativeBtn.Enabled = solveNonCooperativeBtn.Enabled = true;
                return;
            }

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                solveCooperativeBtn.Enabled = solveNonCooperativeBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            Problem p = new(
                solver: new MinCostAssignSolver(isCooperative: true),
                sMatrix: mainGridStr,
                sVecB: new string[0],
                sVecC: new string[0]);
            if (await p.SolveMin(loggers: new StreamWriter?[] { Form.Writer }))
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
        }

        private void MinCostAssignForm_Load(object sender, EventArgs e)
        {
            try
            {
                Number = (int)n.Value;
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
                    containsBlank = containsBlank || string.IsNullOrWhiteSpace((string)matrix[col, row].Value);
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

        private async void solveNonCooperativeBtn_Click(object sender, EventArgs e)
        {
            solveCooperativeBtn.Enabled = solveNonCooperativeBtn.Enabled = false;
            string[][]? mainGridStr = MainGridStr();
            if (mainGridStr is null || mainGridStr.Length == 0)
            {
                solveCooperativeBtn.Enabled = solveNonCooperativeBtn.Enabled = true;
                return;
            }

            var Form = new ProblemForm();
            void closeFormCallback(object? sender, FormClosedEventArgs e)
            {
                solveCooperativeBtn.Enabled = solveNonCooperativeBtn.Enabled = true;
            };
            Form.FormClosed += new FormClosedEventHandler(closeFormCallback);
            Form.Show();

            Problem p = new(
                solver: new MinCostAssignSolver(isCooperative: false),
                sMatrix: mainGridStr,
                sVecB: new string[0],
                sVecC: new string[0]);
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
            solveCooperativeBtn.Enabled = solveNonCooperativeBtn.Enabled = true;
        }
    }

}
