namespace OperationalResearch
{
    partial class MinCostFlowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MinCostFlowForm));
            label1 = new Label();
            matrix = new DataGridView();
            n = new NumericUpDown();
            setNumBtn = new Button();
            boundsGrid = new DataGridView();
            label2 = new Label();
            button1 = new Button();
            button2 = new Button();
            label3 = new Label();
            startBase = new TextBox();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)n).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boundsGrid).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 29);
            label1.Name = "label1";
            label1.Size = new Size(125, 20);
            label1.TabIndex = 0;
            label1.Text = "Number of nodes";
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Location = new Point(11, 69);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(489, 333);
            matrix.TabIndex = 1;
            // 
            // n
            // 
            n.Location = new Point(168, 27);
            n.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            n.Name = "n";
            n.Size = new Size(150, 27);
            n.TabIndex = 2;
            n.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // setNumBtn
            // 
            setNumBtn.Cursor = Cursors.Hand;
            setNumBtn.Location = new Point(339, 16);
            setNumBtn.Name = "setNumBtn";
            setNumBtn.Size = new Size(111, 47);
            setNumBtn.TabIndex = 3;
            setNumBtn.Text = "Apply";
            setNumBtn.UseVisualStyleBackColor = true;
            setNumBtn.Click += setNumBtn_Click;
            // 
            // boundsGrid
            // 
            boundsGrid.AllowUserToAddRows = false;
            boundsGrid.AllowUserToDeleteRows = false;
            boundsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            boundsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            boundsGrid.Location = new Point(517, 69);
            boundsGrid.Name = "boundsGrid";
            boundsGrid.RowHeadersWidth = 51;
            boundsGrid.Size = new Size(486, 333);
            boundsGrid.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(532, 43);
            label2.Name = "label2";
            label2.Size = new Size(196, 20);
            label2.TabIndex = 5;
            label2.Text = "Lower bounds|Upperbounds";
            // 
            // button1
            // 
            button1.Cursor = Cursors.Hand;
            button1.Location = new Point(734, 34);
            button1.Name = "button1";
            button1.Size = new Size(269, 29);
            button1.TabIndex = 6;
            button1.Text = "Solve Unbounded";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Cursor = Cursors.Hand;
            button2.Location = new Point(734, 4);
            button2.Name = "button2";
            button2.Size = new Size(269, 29);
            button2.TabIndex = 7;
            button2.Text = "Solve Bounded";
            button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(34, 425);
            label3.Name = "label3";
            label3.Size = new Size(96, 20);
            label3.TabIndex = 8;
            label3.Text = "Starting Base";
            // 
            // startBase
            // 
            startBase.Location = new Point(168, 421);
            startBase.Name = "startBase";
            startBase.Size = new Size(761, 27);
            startBase.TabIndex = 9;
            // 
            // MinCostFlowForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1015, 458);
            Controls.Add(startBase);
            Controls.Add(label3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(boundsGrid);
            Controls.Add(setNumBtn);
            Controls.Add(n);
            Controls.Add(matrix);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MinCostFlowForm";
            Text = "Travelling Salesman's Problem";
            Load += TravellingSalesmanProblemForm_Load;
            ((System.ComponentModel.ISupportInitialize)matrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)n).EndInit();
            ((System.ComponentModel.ISupportInitialize)boundsGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView matrix;
        private NumericUpDown n;
        private Button setNumBtn;
        private DataGridView boundsGrid;
        private Label label2;
        private Button button1;
        private Button button2;
        private Label label3;
        private TextBox startBase;
    }
}