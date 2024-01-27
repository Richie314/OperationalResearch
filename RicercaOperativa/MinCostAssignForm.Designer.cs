namespace OperationalResearch
{
    partial class MinCostAssignForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MinCostAssignForm));
            label1 = new Label();
            matrix = new DataGridView();
            n = new NumericUpDown();
            setNumBtn = new Button();
            solveCooperativeBtn = new Button();
            solveNonCooperativeBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)n).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 29);
            label1.Name = "label1";
            label1.Size = new Size(188, 20);
            label1.TabIndex = 0;
            label1.Text = "NumberOfWorkersAndJobs";
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Location = new Point(12, 69);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(776, 322);
            matrix.TabIndex = 1;
            // 
            // n
            // 
            n.Location = new Point(238, 27);
            n.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            n.Name = "n";
            n.Size = new Size(150, 27);
            n.TabIndex = 2;
            n.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // setNumBtn
            // 
            setNumBtn.Cursor = Cursors.Hand;
            setNumBtn.Location = new Point(416, 16);
            setNumBtn.Name = "setNumBtn";
            setNumBtn.Size = new Size(111, 47);
            setNumBtn.TabIndex = 3;
            setNumBtn.Text = "Apply";
            setNumBtn.UseVisualStyleBackColor = true;
            setNumBtn.Click += setNumBtn_Click;
            // 
            // solveCooperativeBtn
            // 
            solveCooperativeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            solveCooperativeBtn.Cursor = Cursors.Hand;
            solveCooperativeBtn.Location = new Point(12, 400);
            solveCooperativeBtn.Name = "solveCooperativeBtn";
            solveCooperativeBtn.Size = new Size(376, 50);
            solveCooperativeBtn.TabIndex = 4;
            solveCooperativeBtn.Text = "Solve as Cooperative";
            solveCooperativeBtn.UseVisualStyleBackColor = true;
            solveCooperativeBtn.Click += solveCooperativeBtn_Click;
            // 
            // solveNonCooperativeBtn
            // 
            solveNonCooperativeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            solveNonCooperativeBtn.Cursor = Cursors.Hand;
            solveNonCooperativeBtn.Location = new Point(416, 400);
            solveNonCooperativeBtn.Name = "solveNonCooperativeBtn";
            solveNonCooperativeBtn.Size = new Size(376, 50);
            solveNonCooperativeBtn.TabIndex = 5;
            solveNonCooperativeBtn.Text = "Solve as non Cooperative";
            solveNonCooperativeBtn.UseVisualStyleBackColor = true;
            // 
            // MinCostAssignForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 462);
            Controls.Add(solveNonCooperativeBtn);
            Controls.Add(solveCooperativeBtn);
            Controls.Add(setNumBtn);
            Controls.Add(n);
            Controls.Add(matrix);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MinCostAssignForm";
            Text = "MinCostAssignForm";
            Load += MinCostAssignForm_Load;
            ((System.ComponentModel.ISupportInitialize)matrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)n).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView matrix;
        private NumericUpDown n;
        private Button setNumBtn;
        private Button solveCooperativeBtn;
        private Button solveNonCooperativeBtn;
    }
}