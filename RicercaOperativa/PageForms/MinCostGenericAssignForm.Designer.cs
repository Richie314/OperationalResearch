namespace OperationalResearch.PageForms
{
    partial class MinCostGenericAssignForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MinCostGenericAssignForm));
            label1 = new Label();
            costsMatrix = new DataGridView();
            n = new NumericUpDown();
            setNumBtn = new Button();
            label2 = new Label();
            m = new NumericUpDown();
            label3 = new Label();
            timeMatrix = new DataGridView();
            label4 = new Label();
            button = new Button();
            ((System.ComponentModel.ISupportInitialize)costsMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)n).BeginInit();
            ((System.ComponentModel.ISupportInitialize)m).BeginInit();
            ((System.ComponentModel.ISupportInitialize)timeMatrix).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 29);
            label1.Name = "label1";
            label1.Size = new Size(138, 20);
            label1.TabIndex = 0;
            label1.Text = "Number of Workers";
            // 
            // costsMatrix
            // 
            costsMatrix.AllowUserToAddRows = false;
            costsMatrix.AllowUserToDeleteRows = false;
            costsMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            costsMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            costsMatrix.Location = new Point(12, 137);
            costsMatrix.Name = "costsMatrix";
            costsMatrix.RowHeadersWidth = 51;
            costsMatrix.Size = new Size(635, 455);
            costsMatrix.TabIndex = 1;
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
            setNumBtn.Location = new Point(416, 27);
            setNumBtn.Name = "setNumBtn";
            setNumBtn.Size = new Size(125, 61);
            setNumBtn.TabIndex = 3;
            setNumBtn.Text = "Apply";
            setNumBtn.UseVisualStyleBackColor = true;
            setNumBtn.Click += setNumBtn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 61);
            label2.Name = "label2";
            label2.Size = new Size(114, 20);
            label2.TabIndex = 5;
            label2.Text = "Number of Jobs";
            // 
            // m
            // 
            m.Location = new Point(238, 61);
            m.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            m.Name = "m";
            m.Size = new Size(150, 27);
            m.TabIndex = 6;
            m.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(12, 99);
            label3.Name = "label3";
            label3.Size = new Size(106, 20);
            label3.TabIndex = 7;
            label3.Text = "Matrix of costs";
            // 
            // timeMatrix
            // 
            timeMatrix.AllowUserToAddRows = false;
            timeMatrix.AllowUserToDeleteRows = false;
            timeMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            timeMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            timeMatrix.Location = new Point(676, 137);
            timeMatrix.Name = "timeMatrix";
            timeMatrix.RowHeadersWidth = 51;
            timeMatrix.Size = new Size(609, 455);
            timeMatrix.TabIndex = 8;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(676, 102);
            label4.Name = "label4";
            label4.Size = new Size(409, 20);
            label4.TabIndex = 9;
            label4.Text = "Matrix of time weights, last row is max value for each worker";
            // 
            // button
            // 
            button.Cursor = Cursors.Hand;
            button.Location = new Point(676, 27);
            button.Name = "button";
            button.Size = new Size(546, 61);
            button.TabIndex = 11;
            button.Text = "Solve";
            button.UseVisualStyleBackColor = true;
            button.Click += button_Click;
            // 
            // MinCostGenericAssignForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1297, 604);
            Controls.Add(button);
            Controls.Add(label4);
            Controls.Add(timeMatrix);
            Controls.Add(label3);
            Controls.Add(m);
            Controls.Add(label2);
            Controls.Add(setNumBtn);
            Controls.Add(n);
            Controls.Add(costsMatrix);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MinCostGenericAssignForm";
            Text = "MinCostAssignForm";
            Load += MinCostAssignForm_Load;
            ((System.ComponentModel.ISupportInitialize)costsMatrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)n).EndInit();
            ((System.ComponentModel.ISupportInitialize)m).EndInit();
            ((System.ComponentModel.ISupportInitialize)timeMatrix).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView costsMatrix;
        private NumericUpDown n;
        private Button setNumBtn;
        private Label label2;
        private NumericUpDown m;
        private Label label3;
        private DataGridView timeMatrix;
        private Label label4;
        private Button button;
    }
}