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
            startU = new TextBox();
            label4 = new Label();
            button3 = new Button();
            button4 = new Button();
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
            matrix.Size = new Size(489, 347);
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
            boundsGrid.Size = new Size(486, 347);
            boundsGrid.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(517, 44);
            label2.Name = "label2";
            label2.Size = new Size(103, 20);
            label2.TabIndex = 5;
            label2.Text = "Upper bounds";
            // 
            // button1
            // 
            button1.Cursor = Cursors.Hand;
            button1.Location = new Point(459, 4);
            button1.Name = "button1";
            button1.Size = new Size(269, 29);
            button1.TabIndex = 6;
            button1.Text = "Solve as unbounded";
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
            button2.Click += button2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 427);
            label3.Name = "label3";
            label3.Size = new Size(98, 20);
            label3.TabIndex = 8;
            label3.Text = "Starting Basis";
            // 
            // startBase
            // 
            startBase.Location = new Point(187, 421);
            startBase.Name = "startBase";
            startBase.Size = new Size(814, 27);
            startBase.TabIndex = 9;
            // 
            // startU
            // 
            startU.Location = new Point(187, 453);
            startU.Name = "startU";
            startU.Size = new Size(815, 27);
            startU.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(14, 459);
            label4.Name = "label4";
            label4.Size = new Size(157, 20);
            label4.TabIndex = 11;
            label4.Text = "Starting satureted arcs";
            // 
            // button3
            // 
            button3.Location = new Point(734, 39);
            button3.Name = "button3";
            button3.Size = new Size(125, 29);
            button3.TabIndex = 12;
            button3.Text = "Dijkstra";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Cursor = Cursors.Hand;
            button4.Location = new Point(865, 39);
            button4.Margin = new Padding(3, 4, 3, 4);
            button4.Name = "button4";
            button4.Size = new Size(136, 31);
            button4.TabIndex = 13;
            button4.Text = "FFEK";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // MinCostFlowForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1015, 488);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label4);
            Controls.Add(startU);
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
            Cursor = Cursors.Hand;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MinCostFlowForm";
            Text = "Flow of min cost";
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
        private TextBox startU;
        private Label label4;
        private Button button3;
        private Button button4;
    }
}