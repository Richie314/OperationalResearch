namespace OperationalResearch.PageForms
{
    partial class LinearProgrammingForm_Dual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearProgrammingForm_Dual));
            matrix = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            variablesCountInput = new NumericUpDown();
            equationsCountInput = new NumericUpDown();
            resetTableBtn = new Button();
            functionGrid = new DataGridView();
            maximizeBtn = new Button();
            startBaseInput = new TextBox();
            label4 = new Label();
            xNonNegativeCheckbox = new CheckBox();
            minimizeBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)variablesCountInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)equationsCountInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)functionGrid).BeginInit();
            SuspendLayout();
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Location = new Point(29, 183);
            matrix.Margin = new Padding(3, 4, 3, 4);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(1017, 479);
            matrix.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 26);
            label1.Name = "label1";
            label1.Size = new Size(112, 20);
            label1.TabIndex = 4;
            label1.Text = "Variables Count";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(29, 59);
            label2.Name = "label2";
            label2.Size = new Size(117, 20);
            label2.TabIndex = 5;
            label2.Text = "Equations Count";
            // 
            // variablesCountInput
            // 
            variablesCountInput.Location = new Point(170, 20);
            variablesCountInput.Margin = new Padding(3, 4, 3, 4);
            variablesCountInput.Name = "variablesCountInput";
            variablesCountInput.Size = new Size(137, 27);
            variablesCountInput.TabIndex = 6;
            variablesCountInput.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // equationsCountInput
            // 
            equationsCountInput.Location = new Point(170, 55);
            equationsCountInput.Margin = new Padding(3, 4, 3, 4);
            equationsCountInput.Name = "equationsCountInput";
            equationsCountInput.Size = new Size(137, 27);
            equationsCountInput.TabIndex = 7;
            equationsCountInput.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // resetTableBtn
            // 
            resetTableBtn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            resetTableBtn.Cursor = Cursors.Hand;
            resetTableBtn.Location = new Point(335, 12);
            resetTableBtn.Margin = new Padding(3, 4, 3, 4);
            resetTableBtn.Name = "resetTableBtn";
            resetTableBtn.Size = new Size(269, 40);
            resetTableBtn.TabIndex = 8;
            resetTableBtn.Text = "Apply";
            resetTableBtn.UseVisualStyleBackColor = true;
            resetTableBtn.Click += resetTableBtn_Click;
            // 
            // functionGrid
            // 
            functionGrid.AllowUserToAddRows = false;
            functionGrid.AllowUserToDeleteRows = false;
            functionGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            functionGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            functionGrid.Location = new Point(29, 108);
            functionGrid.Margin = new Padding(3, 4, 3, 4);
            functionGrid.Name = "functionGrid";
            functionGrid.RowHeadersWidth = 51;
            functionGrid.Size = new Size(1017, 60);
            functionGrid.TabIndex = 9;
            // 
            // maximizeBtn
            // 
            maximizeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            maximizeBtn.Cursor = Cursors.Hand;
            maximizeBtn.Location = new Point(27, 678);
            maximizeBtn.Margin = new Padding(3, 4, 3, 4);
            maximizeBtn.Name = "maximizeBtn";
            maximizeBtn.Size = new Size(502, 42);
            maximizeBtn.TabIndex = 11;
            maximizeBtn.Text = "Maximize";
            maximizeBtn.UseVisualStyleBackColor = true;
            maximizeBtn.Click += maximizeBtn_Click;
            // 
            // startBaseInput
            // 
            startBaseInput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            startBaseInput.Location = new Point(610, 55);
            startBaseInput.Margin = new Padding(3, 4, 3, 4);
            startBaseInput.Name = "startBaseInput";
            startBaseInput.Size = new Size(435, 27);
            startBaseInput.TabIndex = 12;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(610, 22);
            label4.Name = "label4";
            label4.Size = new Size(155, 20);
            label4.TabIndex = 13;
            label4.Text = "Starting Basis indexes:";
            // 
            // xNonNegativeCheckbox
            // 
            xNonNegativeCheckbox.AutoSize = true;
            xNonNegativeCheckbox.Checked = true;
            xNonNegativeCheckbox.CheckState = CheckState.Checked;
            xNonNegativeCheckbox.Location = new Point(335, 55);
            xNonNegativeCheckbox.Margin = new Padding(3, 4, 3, 4);
            xNonNegativeCheckbox.Name = "xNonNegativeCheckbox";
            xNonNegativeCheckbox.Size = new Size(269, 24);
            xNonNegativeCheckbox.TabIndex = 15;
            xNonNegativeCheckbox.Text = "Automatically add y >= 0 constraint";
            xNonNegativeCheckbox.UseVisualStyleBackColor = true;
            // 
            // minimizeBtn
            // 
            minimizeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            minimizeBtn.Cursor = Cursors.Hand;
            minimizeBtn.Location = new Point(543, 678);
            minimizeBtn.Margin = new Padding(3, 4, 3, 4);
            minimizeBtn.Name = "minimizeBtn";
            minimizeBtn.Size = new Size(502, 42);
            minimizeBtn.TabIndex = 16;
            minimizeBtn.Text = "Minimize";
            minimizeBtn.UseVisualStyleBackColor = true;
            minimizeBtn.Click += minimizeBtn_Click;
            // 
            // LinearProgrammingForm_Dual
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1075, 736);
            Controls.Add(minimizeBtn);
            Controls.Add(xNonNegativeCheckbox);
            Controls.Add(label4);
            Controls.Add(startBaseInput);
            Controls.Add(maximizeBtn);
            Controls.Add(functionGrid);
            Controls.Add(resetTableBtn);
            Controls.Add(equationsCountInput);
            Controls.Add(variablesCountInput);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(matrix);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1093, 783);
            Name = "LinearProgrammingForm_Dual";
            Text = "Dual Form Simplex";
            Load += LinearProgrammingForm_Load;
            ((System.ComponentModel.ISupportInitialize)matrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)variablesCountInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)equationsCountInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)functionGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView matrix;
        private Label label1;
        private Label label2;
        private NumericUpDown variablesCountInput;
        private NumericUpDown equationsCountInput;
        private Button resetTableBtn;
        private DataGridView functionGrid;
        private Button maximizeBtn;
        private TextBox startBaseInput;
        private Label label4;
        private CheckBox xNonNegativeCheckbox;
        private Button minimizeBtn;
    }
}