using OperationalResearch.Models.Elements;
using System.Data;
namespace OperationalResearch
{
    public partial class PolyhedronControl : UserControl
    {

        #region Events

        public delegate void PolyhedronChangeHandler(object? source, PolyhedronChangeEventArgs e);
        public class PolyhedronChangeEventArgs : EventArgs
        {
            public Polyhedron? CurrentPolyhedron { get; set; }
            public int SpaceDimension { get; set; }
            public PolyhedronChangeEventArgs(Polyhedron? data, int spaceDimension)
            {
                CurrentPolyhedron = data;
                SpaceDimension = spaceDimension;
            }
        }

        public event PolyhedronChangeHandler? OnPolyhedronChange;

        private void regeneratePolyhedron(object? sender, EventArgs e)
        {
            Polyhedron = getNewPolyhedron();
            if (OnPolyhedronChange != null)
            {
                OnPolyhedronChange(sender, new PolyhedronChangeEventArgs(Polyhedron, SpaceDimension));
            }
        }

        #endregion

        public Polyhedron? Polyhedron { get; private set; } = null;
        public int SpaceDimension { get => (int)spaceDimension.Value; }
        public PolyhedronControl()
        {
            InitializeComponent();

            // Handle changes in values
            xPositiveCheckbox.CheckedChanged += regeneratePolyhedron;
            matrix.CellValueChanged += regeneratePolyhedron;

            // Handle changes in dimensions
            spaceDimension.ValueChanged += addOrRemoveColumn;
            equationsCount.ValueChanged += addOrRemoveRow;

            setColumns((int)spaceDimension.Value);
            setRows((int)equationsCount.Value);
        }
        private void setRows(int targetRowCount)
        {
            // Remove the last row until we reach the desired target
            while (matrix.RowCount > targetRowCount)
            {
                matrix.Rows.RemoveAt(matrix.RowCount - 1);
            }
            // Add a new row until we reach the desired target
            while (matrix.RowCount < targetRowCount)
            {
                matrix.Rows.Add(); // Add blank row
            }
        }
        private void setColumns(int targetColumnCount)
        {
            while (matrix.ColumnCount - 2 > targetColumnCount)
            {
                matrix.Columns.RemoveAt(matrix.ColumnCount - 3);
            }
            while (matrix.ColumnCount - 2 < targetColumnCount)
            {
                matrix.Columns.Insert(matrix.ColumnCount - 2,
                    new DataGridViewColumn(matrix.Columns[0].CellTemplate));

                // Change the name of the last row added
                matrix.Columns[matrix.ColumnCount - 3].Name = $"x{matrix.ColumnCount - 2}";
            }
        }
        private void addOrRemoveRow(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            int currRowCount = matrix.RowCount;
            int? targetRowCount = (int?)((sender as NumericUpDown)?.Value);
            if (!targetRowCount.HasValue || currRowCount == targetRowCount.Value)
            {
                return;
            }
            setRows(targetRowCount.Value);
            // We have rempoed all the equations that were needed to be removed.
            // Now we can rebuild the polyhedron
            regeneratePolyhedron(sender, e);
        }
        private void addOrRemoveColumn(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            int currColCount = matrix.ColumnCount;
            int? targetColCount = (int?)((sender as NumericUpDown)?.Value);
            if (!targetColCount.HasValue || currColCount - 2 == targetColCount.Value)
            {
                return;
            }
            setColumns(targetColCount.Value);
            regeneratePolyhedron(sender, e);
        }
        private Polyhedron? getNewPolyhedron()
        {
            var grid = MainGridStr();
            if (grid is null || grid.Length == 0)
            {
                return null;
            }
            return Polyhedron.FromStringMatrix(grid, xPositiveCheckbox.Checked);
        }
        private string[][]? MainGridStr()
        {
            var list = new List<string[]>();
            bool containsBlank = false;
            for (int row = 0; row < matrix.RowCount; row++)
            {
                List<string> currRow = [];
                for (int col = 0; col < matrix.ColumnCount; col++)
                {
                    if (col == matrix.ColumnCount - 2)
                    {
                        // Is the operation column
                        var cell = matrix[col, row] as DataGridViewComboBoxCell;
                        string? operation = cell?.FormattedValue.ToString();
                        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
                        currRow.Add(operation);
                    }
                    else
                    {
                        containsBlank = containsBlank || string.IsNullOrWhiteSpace((string)matrix[col, row].Value);
                        currRow.Add((string)matrix[col, row].Value);
                    }
                }
                list.Add([.. currRow]);
            }
            if (containsBlank)
            {
                if (!blankCellsAsZeroCheckbox.Checked)
                {
                    return null;
                }

                list = list.Select(
                    row => row.Select(x => string.IsNullOrWhiteSpace(x) ? "0" : x).ToArray()).ToList();
            }
            return [.. list];
        }
        
        private string oldPoints = "0;0 | 1;1 | 0;1 | 1;0";
        private void fromPointsButton_Click(object sender, EventArgs e)
        {

            var s = Microsoft.VisualBasic.Interaction.InputBox(
                "Vertices",
                "Insert the vertices of the Polyhedron",
                oldPoints);
            if (string.IsNullOrWhiteSpace(s))
            {
                return;
            }
            oldPoints = s;

            var points = s
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(';', StringSplitOptions.TrimEntries))
                .Where(x => x.Length == 2)
                .Select(x => Vector.FromString(x) ?? Vector.Empty)
                .Where(x => !x.IsEmpty);
            try
            {
                var p = Polyhedron.FromBidimensionalPoints(points);
                spaceDimension.Value = 2;
                setColumns(2);
                equationsCount.Value = p.A.Rows;
                xPositiveCheckbox.Checked = p.ForcePositive;

                matrix.Rows.Clear();
                foreach (int i in p.A.RowsIndeces)
                {
                    matrix.Rows.Add(new string[]
                    {
                        p.A[i][0].ToString(),
                        p.A[i][1].ToString(),
                        "<=",
                        p.b[i].ToString()
                    });
                }

                // Polyhedron = p;
                regeneratePolyhedron(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "An error happened",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
