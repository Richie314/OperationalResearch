namespace OperationalResearch.PageForms
{
    partial class TSPForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TSPForm));
            label1 = new Label();
            matrix = new DataGridView();
            n = new NumericUpDown();
            setNumBtn = new Button();
            findHamiltonCycleBtn = new Button();
            considerSymmetric = new CheckBox();
            k = new NumericUpDown();
            label2 = new Label();
            label3 = new Label();
            startNode = new NumericUpDown();
            branchAndBound = new TextBox();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)n).BeginInit();
            ((System.ComponentModel.ISupportInitialize)k).BeginInit();
            ((System.ComponentModel.ISupportInitialize)startNode).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 22);
            label1.Name = "label1";
            label1.Size = new Size(100, 15);
            label1.TabIndex = 0;
            label1.Text = "Number of nodes";
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Location = new Point(10, 52);
            matrix.Margin = new Padding(3, 2, 3, 2);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(679, 240);
            matrix.TabIndex = 1;
            // 
            // n
            // 
            n.Location = new Point(147, 20);
            n.Margin = new Padding(3, 2, 3, 2);
            n.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            n.Name = "n";
            n.Size = new Size(131, 23);
            n.TabIndex = 2;
            n.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // setNumBtn
            // 
            setNumBtn.Cursor = Cursors.Hand;
            setNumBtn.Location = new Point(297, 12);
            setNumBtn.Margin = new Padding(3, 2, 3, 2);
            setNumBtn.Name = "setNumBtn";
            setNumBtn.Size = new Size(97, 35);
            setNumBtn.TabIndex = 3;
            setNumBtn.Text = "Apply";
            setNumBtn.UseVisualStyleBackColor = true;
            setNumBtn.Click += setNumBtn_Click;
            // 
            // findHamiltonCycleBtn
            // 
            findHamiltonCycleBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            findHamiltonCycleBtn.Cursor = Cursors.Hand;
            findHamiltonCycleBtn.Location = new Point(10, 296);
            findHamiltonCycleBtn.Margin = new Padding(3, 2, 3, 2);
            findHamiltonCycleBtn.Name = "findHamiltonCycleBtn";
            findHamiltonCycleBtn.Size = new Size(157, 51);
            findHamiltonCycleBtn.TabIndex = 4;
            findHamiltonCycleBtn.Text = "Find Hamiltonian Cycle";
            findHamiltonCycleBtn.UseVisualStyleBackColor = true;
            findHamiltonCycleBtn.Click += findHamiltonCycleBtn_Click;
            // 
            // considerSymmetric
            // 
            considerSymmetric.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            considerSymmetric.AutoSize = true;
            considerSymmetric.Checked = true;
            considerSymmetric.CheckState = CheckState.Checked;
            considerSymmetric.Location = new Point(494, 313);
            considerSymmetric.Name = "considerSymmetric";
            considerSymmetric.Size = new Size(194, 19);
            considerSymmetric.TabIndex = 6;
            considerSymmetric.Text = "Consider Problem as symmetric";
            considerSymmetric.UseVisualStyleBackColor = true;
            // 
            // k
            // 
            k.Location = new Point(446, 19);
            k.Margin = new Padding(3, 2, 3, 2);
            k.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            k.Name = "k";
            k.Size = new Size(38, 23);
            k.TabIndex = 9;
            k.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(411, 22);
            label2.Name = "label2";
            label2.Size = new Size(28, 15);
            label2.TabIndex = 10;
            label2.Text = "K = ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(501, 24);
            label3.Name = "label3";
            label3.Size = new Size(75, 15);
            label3.TabIndex = 11;
            label3.Text = "Start node = ";
            // 
            // startNode
            // 
            startNode.Location = new Point(582, 20);
            startNode.Margin = new Padding(3, 2, 3, 2);
            startNode.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            startNode.Name = "startNode";
            startNode.Size = new Size(39, 23);
            startNode.TabIndex = 12;
            startNode.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // branchAndBound
            // 
            branchAndBound.Location = new Point(178, 324);
            branchAndBound.Name = "branchAndBound";
            branchAndBound.PlaceholderText = "1-2, 2-3";
            branchAndBound.Size = new Size(306, 23);
            branchAndBound.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(178, 304);
            label4.Name = "label4";
            label4.Size = new Size(136, 15);
            label4.TabIndex = 14;
            label4.Text = "Branch and Bound order";
            // 
            // TSPForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 355);
            Controls.Add(label4);
            Controls.Add(branchAndBound);
            Controls.Add(startNode);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(k);
            Controls.Add(considerSymmetric);
            Controls.Add(findHamiltonCycleBtn);
            Controls.Add(setNumBtn);
            Controls.Add(n);
            Controls.Add(matrix);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "TSPForm";
            Text = "Travelling Salesman's Problem";
            Load += TravellingSalesmanProblemForm_Load;
            ((System.ComponentModel.ISupportInitialize)matrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)n).EndInit();
            ((System.ComponentModel.ISupportInitialize)k).EndInit();
            ((System.ComponentModel.ISupportInitialize)startNode).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView matrix;
        private NumericUpDown n;
        private Button setNumBtn;
        private Button findHamiltonCycleBtn;
        private CheckBox considerSymmetric;
        private NumericUpDown k;
        private Label label2;
        private Label label3;
        private NumericUpDown startNode;
        private TextBox branchAndBound;
        private Label label4;
    }
}