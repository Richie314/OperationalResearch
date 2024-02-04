namespace RicercaOperativa
{
    partial class LinearProgrammingForm_Primal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearProgrammingForm_Primal));
            matrix = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            variablesCountInput = new NumericUpDown();
            equationsCountInput = new NumericUpDown();
            resetTableBtn = new Button();
            functionGrid = new DataGridView();
            label3 = new Label();
            startSimplexBtn = new Button();
            startBaseInput = new TextBox();
            label4 = new Label();
            xNonNegativeCheckbox = new CheckBox();
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
            matrix.Location = new Point(25, 132);
            matrix.Margin = new Padding(0);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 50;
            matrix.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            matrix.Size = new Size(900, 350);
            matrix.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(59, 19);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 4;
            label1.Text = "Variables Count";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(291, 19);
            label2.Name = "label2";
            label2.Size = new Size(95, 15);
            label2.TabIndex = 5;
            label2.Text = "Equations Count";
            // 
            // variablesCountInput
            // 
            variablesCountInput.Location = new Point(154, 17);
            variablesCountInput.Name = "variablesCountInput";
            variablesCountInput.Size = new Size(120, 23);
            variablesCountInput.TabIndex = 6;
            variablesCountInput.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // equationsCountInput
            // 
            equationsCountInput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            equationsCountInput.Location = new Point(392, 17);
            equationsCountInput.Name = "equationsCountInput";
            equationsCountInput.Size = new Size(120, 23);
            equationsCountInput.TabIndex = 7;
            equationsCountInput.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // resetTableBtn
            // 
            resetTableBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            resetTableBtn.Cursor = Cursors.Hand;
            resetTableBtn.Location = new Point(543, 12);
            resetTableBtn.Name = "resetTableBtn";
            resetTableBtn.Size = new Size(75, 30);
            resetTableBtn.TabIndex = 8;
            resetTableBtn.Text = "Apply";
            resetTableBtn.UseVisualStyleBackColor = true;
            resetTableBtn.Click += resetTableBtn_Click;
            // 
            // functionGrid
            // 
            functionGrid.AllowUserToAddRows = false;
            functionGrid.AllowUserToDeleteRows = false;
            functionGrid.AllowUserToResizeColumns = false;
            functionGrid.AllowUserToResizeRows = false;
            functionGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            functionGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            functionGrid.Location = new Point(25, 71);
            functionGrid.Margin = new Padding(0);
            functionGrid.Name = "functionGrid";
            functionGrid.RowHeadersWidth = 50;
            functionGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            functionGrid.Size = new Size(900, 52);
            functionGrid.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(25, 54);
            label3.Name = "label3";
            label3.Size = new Size(125, 15);
            label3.TabIndex = 10;
            label3.Text = "Function to maximize:";
            // 
            // startSimplexBtn
            // 
            startSimplexBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            startSimplexBtn.Cursor = Cursors.Hand;
            startSimplexBtn.Location = new Point(25, 487);
            startSimplexBtn.Name = "startSimplexBtn";
            startSimplexBtn.Size = new Size(171, 53);
            startSimplexBtn.TabIndex = 11;
            startSimplexBtn.Text = "Start Simplex";
            startSimplexBtn.UseVisualStyleBackColor = true;
            startSimplexBtn.Click += startSimplexBtn_Click;
            // 
            // startBaseInput
            // 
            startBaseInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            startBaseInput.Location = new Point(412, 503);
            startBaseInput.Name = "startBaseInput";
            startBaseInput.Size = new Size(503, 23);
            startBaseInput.TabIndex = 12;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(260, 508);
            label4.Name = "label4";
            label4.Size = new Size(121, 15);
            label4.TabIndex = 13;
            label4.Text = "Starting Base indexes:";
            // 
            // xNonNegativeCheckbox
            // 
            xNonNegativeCheckbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            xNonNegativeCheckbox.AutoSize = true;
            xNonNegativeCheckbox.Checked = true;
            xNonNegativeCheckbox.CheckState = CheckState.Checked;
            xNonNegativeCheckbox.Location = new Point(657, 21);
            xNonNegativeCheckbox.Name = "xNonNegativeCheckbox";
            xNonNegativeCheckbox.Size = new Size(216, 19);
            xNonNegativeCheckbox.TabIndex = 14;
            xNonNegativeCheckbox.Text = "Automatically add x >= 0 constraint";
            xNonNegativeCheckbox.UseVisualStyleBackColor = true;
            // 
            // LinearProgrammingForm_Primal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(941, 552);
            Controls.Add(xNonNegativeCheckbox);
            Controls.Add(label4);
            Controls.Add(startBaseInput);
            Controls.Add(startSimplexBtn);
            Controls.Add(label3);
            Controls.Add(functionGrid);
            Controls.Add(resetTableBtn);
            Controls.Add(equationsCountInput);
            Controls.Add(variablesCountInput);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(matrix);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LinearProgrammingForm_Primal";
            Text = "Primal Form Simplex";
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
        private Label label3;
        private Button startSimplexBtn;
        private TextBox startBaseInput;
        private Label label4;
        private CheckBox xNonNegativeCheckbox;
    }
}