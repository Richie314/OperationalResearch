namespace OperationalResearch
{
    partial class LinearFunctionControl
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
            vectorGrid = new DataGridView();
            c1 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)vectorGrid).BeginInit();
            SuspendLayout();
            // 
            // vectorGrid
            // 
            vectorGrid.AllowUserToAddRows = false;
            vectorGrid.AllowUserToDeleteRows = false;
            vectorGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            vectorGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            vectorGrid.Columns.AddRange(new DataGridViewColumn[] { c1 });
            vectorGrid.Location = new Point(3, 4);
            vectorGrid.Margin = new Padding(3, 4, 3, 4);
            vectorGrid.Name = "vectorGrid";
            vectorGrid.RowHeadersWidth = 51;
            vectorGrid.Size = new Size(578, 82);
            vectorGrid.TabIndex = 0;
            // 
            // c1
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "0";
            c1.DefaultCellStyle = dataGridViewCellStyle1;
            c1.HeaderText = "c1";
            c1.MinimumWidth = 6;
            c1.Name = "c1";
            c1.SortMode = DataGridViewColumnSortMode.NotSortable;
            c1.Width = 125;
            // 
            // LinearFunctionControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(vectorGrid);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(585, 90);
            Name = "LinearFunctionControl";
            Size = new Size(585, 90);
            ((System.ComponentModel.ISupportInitialize)vectorGrid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView vectorGrid;
        private DataGridViewTextBoxColumn c1;
    }
}
