namespace OperationalResearch.PageForms
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
            From = new DataGridViewTextBoxColumn();
            To = new DataGridViewTextBoxColumn();
            Cost = new DataGridViewTextBoxColumn();
            Minimum = new DataGridViewTextBoxColumn();
            Capacity = new DataGridViewTextBoxColumn();
            Tree = new DataGridViewCheckBoxColumn();
            Saturated = new DataGridViewCheckBoxColumn();
            edges = new NumericUpDown();
            setNumBtn = new Button();
            button2 = new Button();
            label2 = new Label();
            nodes = new NumericUpDown();
            label3 = new Label();
            balances = new DataGridView();
            b = new DataGridViewTextBoxColumn();
            label4 = new Label();
            label5 = new Label();
            startNode = new NumericUpDown();
            endNode = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)edges).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nodes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)balances).BeginInit();
            ((System.ComponentModel.ISupportInitialize)startNode).BeginInit();
            ((System.ComponentModel.ISupportInitialize)endNode).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(24, 10);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 0;
            label1.Text = "Number of arcs";
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Columns.AddRange(new DataGridViewColumn[] { From, To, Cost, Minimum, Capacity, Tree, Saturated });
            matrix.Location = new Point(10, 61);
            matrix.Margin = new Padding(3, 2, 3, 2);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(837, 434);
            matrix.TabIndex = 1;
            // 
            // From
            // 
            From.Frozen = true;
            From.HeaderText = "From";
            From.Name = "From";
            // 
            // To
            // 
            To.Frozen = true;
            To.HeaderText = "To";
            To.Name = "To";
            // 
            // Cost
            // 
            Cost.Frozen = true;
            Cost.HeaderText = "Cost";
            Cost.Name = "Cost";
            // 
            // Minimum
            // 
            Minimum.Frozen = true;
            Minimum.HeaderText = "Minimum";
            Minimum.Name = "Minimum";
            // 
            // Capacity
            // 
            Capacity.Frozen = true;
            Capacity.HeaderText = "Capacity";
            Capacity.Name = "Capacity";
            // 
            // Tree
            // 
            Tree.Frozen = true;
            Tree.HeaderText = "Tree";
            Tree.Name = "Tree";
            Tree.Resizable = DataGridViewTriState.True;
            Tree.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // Saturated
            // 
            Saturated.Frozen = true;
            Saturated.HeaderText = "Satureted";
            Saturated.Name = "Saturated";
            Saturated.Resizable = DataGridViewTriState.True;
            Saturated.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // edges
            // 
            edges.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            edges.Location = new Point(143, 7);
            edges.Margin = new Padding(3, 2, 3, 2);
            edges.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            edges.Name = "edges";
            edges.Size = new Size(134, 23);
            edges.TabIndex = 2;
            edges.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // setNumBtn
            // 
            setNumBtn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            setNumBtn.Cursor = Cursors.Hand;
            setNumBtn.Location = new Point(297, 12);
            setNumBtn.Margin = new Padding(3, 2, 3, 2);
            setNumBtn.Name = "setNumBtn";
            setNumBtn.Size = new Size(100, 35);
            setNumBtn.TabIndex = 3;
            setNumBtn.Text = "Apply";
            setNumBtn.UseVisualStyleBackColor = true;
            setNumBtn.Click += setNumBtn_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button2.Cursor = Cursors.Hand;
            button2.Location = new Point(401, 12);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(209, 35);
            button2.TabIndex = 7;
            button2.Text = "Solve";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(24, 38);
            label2.Name = "label2";
            label2.Size = new Size(100, 15);
            label2.TabIndex = 14;
            label2.Text = "Number of nodes";
            // 
            // nodes
            // 
            nodes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nodes.Location = new Point(143, 34);
            nodes.Margin = new Padding(3, 2, 3, 2);
            nodes.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            nodes.Name = "nodes";
            nodes.Size = new Size(134, 23);
            nodes.TabIndex = 15;
            nodes.Value = new decimal(new int[] { 7, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(853, 42);
            label3.Name = "label3";
            label3.Size = new Size(85, 15);
            label3.TabIndex = 16;
            label3.Text = "Node balances";
            // 
            // balances
            // 
            balances.AllowUserToAddRows = false;
            balances.AllowUserToDeleteRows = false;
            balances.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            balances.Columns.AddRange(new DataGridViewColumn[] { b });
            balances.Location = new Point(853, 61);
            balances.Name = "balances";
            balances.Size = new Size(88, 434);
            balances.TabIndex = 17;
            // 
            // b
            // 
            b.Frozen = true;
            b.HeaderText = "b";
            b.Name = "b";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(630, 9);
            label4.Name = "label4";
            label4.Size = new Size(74, 15);
            label4.TabIndex = 18;
            label4.Text = "Start Node =";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(631, 39);
            label5.Name = "label5";
            label5.Size = new Size(73, 15);
            label5.TabIndex = 19;
            label5.Text = "End Node = ";
            // 
            // startNode
            // 
            startNode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            startNode.Location = new Point(710, 7);
            startNode.Margin = new Padding(3, 2, 3, 2);
            startNode.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            startNode.Name = "startNode";
            startNode.Size = new Size(57, 23);
            startNode.TabIndex = 20;
            startNode.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // endNode
            // 
            endNode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            endNode.Location = new Point(710, 34);
            endNode.Margin = new Padding(3, 2, 3, 2);
            endNode.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            endNode.Name = "endNode";
            endNode.Size = new Size(57, 23);
            endNode.TabIndex = 21;
            endNode.Value = new decimal(new int[] { 7, 0, 0, 0 });
            // 
            // MinCostFlowForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(953, 506);
            Controls.Add(endNode);
            Controls.Add(startNode);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(balances);
            Controls.Add(label3);
            Controls.Add(nodes);
            Controls.Add(label2);
            Controls.Add(button2);
            Controls.Add(setNumBtn);
            Controls.Add(edges);
            Controls.Add(matrix);
            Controls.Add(label1);
            Cursor = Cursors.Hand;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "MinCostFlowForm";
            Text = "Flow of min cost";
            Load += MinCostFlowForm_Load;
            ((System.ComponentModel.ISupportInitialize)matrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)edges).EndInit();
            ((System.ComponentModel.ISupportInitialize)nodes).EndInit();
            ((System.ComponentModel.ISupportInitialize)balances).EndInit();
            ((System.ComponentModel.ISupportInitialize)startNode).EndInit();
            ((System.ComponentModel.ISupportInitialize)endNode).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView matrix;
        private NumericUpDown edges;
        private Button setNumBtn;
        private Button button2;
        private Label label2;
        private NumericUpDown nodes;
        private Label label3;
        private DataGridView balances;
        private Label label4;
        private Label label5;
        private NumericUpDown startNode;
        private NumericUpDown endNode;
        private DataGridViewTextBoxColumn From;
        private DataGridViewTextBoxColumn To;
        private DataGridViewTextBoxColumn Cost;
        private DataGridViewTextBoxColumn Minimum;
        private DataGridViewTextBoxColumn Capacity;
        private DataGridViewCheckBoxColumn Tree;
        private DataGridViewCheckBoxColumn Saturated;
        private DataGridViewTextBoxColumn b;
    }
}