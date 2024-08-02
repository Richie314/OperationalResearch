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
            spaceDimension = new NumericUpDown();
            equationsCount = new NumericUpDown();
            x1 = new DataGridViewTextBoxColumn();
            Operation = new DataGridViewComboBoxColumn();
            b = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spaceDimension).BeginInit();
            ((System.ComponentModel.ISupportInitialize)equationsCount).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 13);
            label1.Name = "label1";
            label1.Size = new Size(187, 15);
            label1.TabIndex = 0;
            label1.Text = "Variables count";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 44);
            label2.Name = "label2";
            label2.Size = new Size(93, 15);
            label2.TabIndex = 1;
            label2.Text = "Equations count";
            // 
            // xPositiveCheckbox
            // 
            xPositiveCheckbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            xPositiveCheckbox.AutoSize = true;
            xPositiveCheckbox.Location = new Point(358, 12);
            xPositiveCheckbox.Name = "xPositiveCheckbox";
            xPositiveCheckbox.Size = new Size(141, 19);
            xPositiveCheckbox.TabIndex = 2;
            xPositiveCheckbox.Text = "Add x >= 0 constraint";
            xPositiveCheckbox.UseVisualStyleBackColor = true;
            xPositiveCheckbox.Checked = true;
            // 
            // blankCellsAsZeroCheckbox
            // 
            blankCellsAsZeroCheckbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            blankCellsAsZeroCheckbox.AutoSize = true;
            blankCellsAsZeroCheckbox.Location = new Point(358, 38);
            blankCellsAsZeroCheckbox.Name = "blankCellsAsZeroCheckbox";
            blankCellsAsZeroCheckbox.Size = new Size(154, 19);
            blankCellsAsZeroCheckbox.TabIndex = 3;
            blankCellsAsZeroCheckbox.Text = "Consider blank cells as 0";
            blankCellsAsZeroCheckbox.UseVisualStyleBackColor = true;
            blankCellsAsZeroCheckbox.Checked = true;
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Columns.AddRange(new DataGridViewColumn[] { x1, Operation, b });
            matrix.Location = new Point(3, 67);
            matrix.Name = "matrix";
            matrix.Size = new Size(509, 239);
            matrix.TabIndex = 4;
            // 
            // spaceDimension
            // 
            spaceDimension.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            spaceDimension.Location = new Point(213, 11);
            spaceDimension.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            spaceDimension.Name = "spaceDimension";
            spaceDimension.Size = new Size(120, 23);
            spaceDimension.TabIndex = 5;
            spaceDimension.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // equationsCount
            // 
            equationsCount.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            equationsCount.Location = new Point(213, 38);
            equationsCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            equationsCount.Name = "equationsCount";
            equationsCount.Size = new Size(120, 23);
            equationsCount.TabIndex = 6;
            equationsCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // x1
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "0";
            x1.DefaultCellStyle = dataGridViewCellStyle1;
            x1.HeaderText = "x1";
            x1.Name = "x1";
            x1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Operation
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "<=";
            Operation.DefaultCellStyle = dataGridViewCellStyle2;
            Operation.HeaderText = " ";
            Operation.Items.AddRange(new object[] { "<=", ">=", "=" });
            Operation.Name = "Operation";
            // 
            // b
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = "0";
            b.DefaultCellStyle = dataGridViewCellStyle3;
            b.HeaderText = "b";
            b.Name = "b";
            b.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // PolyhedronControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(equationsCount);
            Controls.Add(spaceDimension);
            Controls.Add(matrix);
            Controls.Add(blankCellsAsZeroCheckbox);
            Controls.Add(xPositiveCheckbox);
            Controls.Add(label2);
            Controls.Add(label1);
            MinimumSize = new Size(515, 312);
            Name = "PolyhedronControl";
            Size = new Size(515, 312);
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
        private DataGridViewTextBoxColumn x1;
        private DataGridViewComboBoxColumn Operation;
        private DataGridViewTextBoxColumn b;
    }
}
