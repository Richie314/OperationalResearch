namespace RicercaOperativa
{
    partial class LinearProgrammingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearProgrammingForm));
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
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Location = new Point(29, 183);
            matrix.Margin = new Padding(3, 4, 3, 4);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(1017, 459);
            matrix.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(67, 25);
            label1.Name = "label1";
            label1.Size = new Size(112, 20);
            label1.TabIndex = 4;
            label1.Text = "Variables Count";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(333, 25);
            label2.Name = "label2";
            label2.Size = new Size(117, 20);
            label2.TabIndex = 5;
            label2.Text = "Equations Count";
            // 
            // variablesCountInput
            // 
            variablesCountInput.Location = new Point(176, 23);
            variablesCountInput.Margin = new Padding(3, 4, 3, 4);
            variablesCountInput.Name = "variablesCountInput";
            variablesCountInput.Size = new Size(137, 27);
            variablesCountInput.TabIndex = 6;
            variablesCountInput.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // equationsCountInput
            // 
            equationsCountInput.Location = new Point(448, 23);
            equationsCountInput.Margin = new Padding(3, 4, 3, 4);
            equationsCountInput.Name = "equationsCountInput";
            equationsCountInput.Size = new Size(137, 27);
            equationsCountInput.TabIndex = 7;
            equationsCountInput.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // resetTableBtn
            // 
            resetTableBtn.Cursor = Cursors.Hand;
            resetTableBtn.Location = new Point(621, 16);
            resetTableBtn.Margin = new Padding(3, 4, 3, 4);
            resetTableBtn.Name = "resetTableBtn";
            resetTableBtn.Size = new Size(86, 40);
            resetTableBtn.TabIndex = 8;
            resetTableBtn.Text = "Apply";
            resetTableBtn.UseVisualStyleBackColor = true;
            resetTableBtn.Click += resetTableBtn_Click;
            // 
            // functionGrid
            // 
            functionGrid.AllowUserToAddRows = false;
            functionGrid.AllowUserToDeleteRows = false;
            functionGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            functionGrid.Location = new Point(29, 108);
            functionGrid.Margin = new Padding(3, 4, 3, 4);
            functionGrid.Name = "functionGrid";
            functionGrid.RowHeadersWidth = 51;
            functionGrid.Size = new Size(1017, 60);
            functionGrid.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(29, 72);
            label3.Name = "label3";
            label3.Size = new Size(154, 20);
            label3.TabIndex = 10;
            label3.Text = "Function to maximize:";
            // 
            // startSimplexBtn
            // 
            startSimplexBtn.Cursor = Cursors.Hand;
            startSimplexBtn.Location = new Point(29, 649);
            startSimplexBtn.Margin = new Padding(3, 4, 3, 4);
            startSimplexBtn.Name = "startSimplexBtn";
            startSimplexBtn.Size = new Size(195, 71);
            startSimplexBtn.TabIndex = 11;
            startSimplexBtn.Text = "Start Simplex";
            startSimplexBtn.UseVisualStyleBackColor = true;
            startSimplexBtn.Click += startSimplexBtn_Click;
            // 
            // startBaseInput
            // 
            startBaseInput.Location = new Point(471, 671);
            startBaseInput.Margin = new Padding(3, 4, 3, 4);
            startBaseInput.Name = "startBaseInput";
            startBaseInput.Size = new Size(574, 27);
            startBaseInput.TabIndex = 12;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(297, 677);
            label4.Name = "label4";
            label4.Size = new Size(153, 20);
            label4.TabIndex = 13;
            label4.Text = "Starting Base indexes:";
            // 
            // LinearProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1075, 736);
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
            Margin = new Padding(3, 4, 3, 4);
            Name = "LinearProgrammingForm";
            Text = "LinearProgrammingForm";
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
    }
}