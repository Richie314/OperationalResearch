namespace OperationalResearch
{
    partial class PolyhedronControl
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            label1 = new Label();
            label2 = new Label();
            xPositiveCheckbox = new CheckBox();
            blankCellsAsZeroCheckbox = new CheckBox();
            matrix = new DataGridView();
            x1 = new DataGridViewTextBoxColumn();
            Operation = new DataGridViewComboBoxColumn();
            b = new DataGridViewTextBoxColumn();
            spaceDimension = new NumericUpDown();
            equationsCount = new NumericUpDown();
            fromPointsButton = new Button();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spaceDimension).BeginInit();
            ((System.ComponentModel.ISupportInitialize)equationsCount).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 17);
            label1.Name = "label1";
            label1.Size = new Size(110, 20);
            label1.TabIndex = 0;
            label1.Text = "Variables count";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(24, 59);
            label2.Name = "label2";
            label2.Size = new Size(115, 20);
            label2.TabIndex = 1;
            label2.Text = "Equations count";
            // 
            // xPositiveCheckbox
            // 
            xPositiveCheckbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            xPositiveCheckbox.AutoSize = true;
            xPositiveCheckbox.Checked = true;
            xPositiveCheckbox.CheckState = CheckState.Checked;
            xPositiveCheckbox.Location = new Point(293, 16);
            xPositiveCheckbox.Margin = new Padding(3, 4, 3, 4);
            xPositiveCheckbox.Name = "xPositiveCheckbox";
            xPositiveCheckbox.Size = new Size(175, 24);
            xPositiveCheckbox.TabIndex = 2;
            xPositiveCheckbox.Text = "Add x >= 0 constraint";
            xPositiveCheckbox.UseVisualStyleBackColor = true;
            // 
            // blankCellsAsZeroCheckbox
            // 
            blankCellsAsZeroCheckbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            blankCellsAsZeroCheckbox.AutoSize = true;
            blankCellsAsZeroCheckbox.Checked = true;
            blankCellsAsZeroCheckbox.CheckState = CheckState.Checked;
            blankCellsAsZeroCheckbox.Location = new Point(292, 51);
            blankCellsAsZeroCheckbox.Margin = new Padding(3, 4, 3, 4);
            blankCellsAsZeroCheckbox.Name = "blankCellsAsZeroCheckbox";
            blankCellsAsZeroCheckbox.Size = new Size(192, 24);
            blankCellsAsZeroCheckbox.TabIndex = 3;
            blankCellsAsZeroCheckbox.Text = "Consider blank cells as 0";
            blankCellsAsZeroCheckbox.UseVisualStyleBackColor = true;
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Columns.AddRange(new DataGridViewColumn[] { x1, Operation, b });
            matrix.Location = new Point(3, 89);
            matrix.Margin = new Padding(3, 4, 3, 4);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(582, 319);
            matrix.TabIndex = 4;
            // 
            // x1
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "0";
            x1.DefaultCellStyle = dataGridViewCellStyle1;
            x1.HeaderText = "x1";
            x1.MinimumWidth = 6;
            x1.Name = "x1";
            x1.SortMode = DataGridViewColumnSortMode.NotSortable;
            x1.Width = 125;
            // 
            // Operation
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "≤";
            Operation.DefaultCellStyle = dataGridViewCellStyle2;
            Operation.HeaderText = " ";
            Operation.Items.AddRange(new object[] { "≤", "≥", "=" });
            Operation.MinimumWidth = 6;
            Operation.Name = "Operation";
            Operation.Width = 50;
            // 
            // b
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = "0";
            b.DefaultCellStyle = dataGridViewCellStyle3;
            b.HeaderText = "b";
            b.MinimumWidth = 6;
            b.Name = "b";
            b.SortMode = DataGridViewColumnSortMode.NotSortable;
            b.Width = 125;
            // 
            // spaceDimension
            // 
            spaceDimension.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            spaceDimension.Location = new Point(144, 15);
            spaceDimension.Margin = new Padding(3, 4, 3, 4);
            spaceDimension.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            spaceDimension.Name = "spaceDimension";
            spaceDimension.Size = new Size(137, 27);
            spaceDimension.TabIndex = 5;
            spaceDimension.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // equationsCount
            // 
            equationsCount.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            equationsCount.Location = new Point(142, 51);
            equationsCount.Margin = new Padding(3, 4, 3, 4);
            equationsCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            equationsCount.Name = "equationsCount";
            equationsCount.Size = new Size(137, 27);
            equationsCount.TabIndex = 6;
            equationsCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // fromPointsButton
            // 
            fromPointsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            fromPointsButton.Cursor = Cursors.Hand;
            fromPointsButton.Location = new Point(491, 17);
            fromPointsButton.Name = "fromPointsButton";
            fromPointsButton.Size = new Size(94, 58);
            fromPointsButton.TabIndex = 7;
            fromPointsButton.Text = "From points";
            fromPointsButton.UseVisualStyleBackColor = true;
            fromPointsButton.Click += fromPointsButton_Click;
            // 
            // PolyhedronControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(fromPointsButton);
            Controls.Add(equationsCount);
            Controls.Add(spaceDimension);
            Controls.Add(matrix);
            Controls.Add(blankCellsAsZeroCheckbox);
            Controls.Add(xPositiveCheckbox);
            Controls.Add(label2);
            Controls.Add(label1);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(589, 416);
            Name = "PolyhedronControl";
            Size = new Size(589, 416);
            ((System.ComponentModel.ISupportInitialize)matrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)spaceDimension).EndInit();
            ((System.ComponentModel.ISupportInitialize)equationsCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private CheckBox xPositiveCheckbox;
        private CheckBox blankCellsAsZeroCheckbox;
        private DataGridView matrix;
        private NumericUpDown spaceDimension;
        private NumericUpDown equationsCount;
        private Button fromPointsButton;
        private DataGridViewTextBoxColumn x1;
        private DataGridViewComboBoxColumn Operation;
        private DataGridViewTextBoxColumn b;
    }
}
