using System;
using System.Windows.Forms;
using OperationalResearch.Models.Elements;

namespace OperationalResearch
{
    public partial class LinearFunctionControl : UserControl
    {
        #region Events

        public delegate void LinearFunctionHandler(object? source, LinearFunctionEventArgs e);
        public class LinearFunctionEventArgs : EventArgs
        {
            public Vector? CurrentVector { get; set; }
            public LinearFunctionEventArgs(Vector? data)
            {
                CurrentVector = data;
            }
        }

        public event LinearFunctionHandler? OnLinearFunctionChange;

        private void regenerateVector(object? sender, EventArgs e)
        {
            Vector = getNewVector();
            if (OnLinearFunctionChange != null)
            {
                OnLinearFunctionChange(sender, new LinearFunctionEventArgs(Vector));
            }
        }

        #endregion
        public Vector? Vector { get; set; }
        public LinearFunctionControl()
        {
            InitializeComponent();
            vectorGrid.Rows.Add();
            vectorGrid.CellValueChanged += regenerateVector;
        }

        public void setSize(int newSize)
        {
            if (newSize <= 0)
            {
                throw new ArgumentException($"Invalid size ({newSize} <= 0");
            }
            if (newSize == vectorGrid.Columns.Count)
            {
                return;
            }
            while (newSize > vectorGrid.ColumnCount)
            {
                vectorGrid.Columns.Add(new DataGridViewColumn(vectorGrid.Columns[0].CellTemplate));
                vectorGrid.Columns[vectorGrid.ColumnCount - 1].Name = $"c{vectorGrid.ColumnCount}";
            }
            while (newSize < vectorGrid.ColumnCount)
            {
                vectorGrid.Columns.RemoveAt(vectorGrid.ColumnCount - 1);
            }
        }
        public void setSize(int newSize, object? sender, EventArgs e)
        {
            setSize(newSize);
            regenerateVector(sender, e);
        }
        private Vector? getNewVector() => Vector.FromString(MainVectorStr());

        private string[] MainVectorStr()
        {
            List<string> list = [];
            for (int i = 0; i < vectorGrid.ColumnCount; i++)
            {
                string s = (string)vectorGrid[i, 0].Value;
                list.Add(string.IsNullOrWhiteSpace(s) ? "0" : s.Trim());
            }
            return [.. list];
        }
    }
}
