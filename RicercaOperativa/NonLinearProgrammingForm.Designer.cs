namespace RicercaOperativa
{
    partial class NonLinearProgrammingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NonLinearProgrammingForm));
            button1 = new Button();
            label1 = new Label();
            dimensionOfSpaceInput = new NumericUpDown();
            pythonInput = new TextBox();
            matrix = new DataGridView();
            label2 = new Label();
            numberOfEquationsInput = new NumericUpDown();
            solveButton = new Button();
            label3 = new Label();
            startPointInput = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dimensionOfSpaceInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numberOfEquationsInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)startPointInput).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Cursor = Cursors.Hand;
            button1.Location = new Point(599, 27);
            button1.Name = "button1";
            button1.Size = new Size(87, 34);
            button1.TabIndex = 0;
            button1.Text = "Apply";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(28, 37);
            label1.Name = "label1";
            label1.Size = new Size(111, 15);
            label1.TabIndex = 1;
            label1.Text = "Dimension of space";
            // 
            // dimensionOfSpaceInput
            // 
            dimensionOfSpaceInput.Location = new Point(164, 34);
            dimensionOfSpaceInput.Name = "dimensionOfSpaceInput";
            dimensionOfSpaceInput.Size = new Size(120, 23);
            dimensionOfSpaceInput.TabIndex = 2;
            dimensionOfSpaceInput.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // pythonInput
            // 
            pythonInput.Font = new Font("CaskaydiaCove NF", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pythonInput.Location = new Point(12, 223);
            pythonInput.Multiline = true;
            pythonInput.Name = "pythonInput";
            pythonInput.PlaceholderText = "Insert the python code for the functions here";
            pythonInput.ScrollBars = ScrollBars.Both;
            pythonInput.Size = new Size(776, 159);
            pythonInput.TabIndex = 3;
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Location = new Point(12, 67);
            matrix.Name = "matrix";
            matrix.Size = new Size(776, 150);
            matrix.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(314, 37);
            label2.Name = "label2";
            label2.Size = new Size(120, 15);
            label2.TabIndex = 5;
            label2.Text = "Number of equations";
            // 
            // numberOfEquationsInput
            // 
            numberOfEquationsInput.Location = new Point(440, 35);
            numberOfEquationsInput.Name = "numberOfEquationsInput";
            numberOfEquationsInput.Size = new Size(120, 23);
            numberOfEquationsInput.TabIndex = 6;
            numberOfEquationsInput.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // solveButton
            // 
            solveButton.Cursor = Cursors.Hand;
            solveButton.Location = new Point(12, 400);
            solveButton.Name = "solveButton";
            solveButton.Size = new Size(115, 65);
            solveButton.TabIndex = 7;
            solveButton.Text = "Solve with Projected Gradient";
            solveButton.UseVisualStyleBackColor = true;
            solveButton.Click += solveButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(133, 425);
            label3.Name = "label3";
            label3.Size = new Size(79, 15);
            label3.TabIndex = 8;
            label3.Text = "Starting Point";
            // 
            // startPointInput
            // 
            startPointInput.AllowUserToAddRows = false;
            startPointInput.AllowUserToDeleteRows = false;
            startPointInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            startPointInput.Location = new Point(234, 400);
            startPointInput.Name = "startPointInput";
            startPointInput.Size = new Size(554, 65);
            startPointInput.TabIndex = 9;
            // 
            // NonLinearProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 478);
            Controls.Add(startPointInput);
            Controls.Add(label3);
            Controls.Add(solveButton);
            Controls.Add(numberOfEquationsInput);
            Controls.Add(label2);
            Controls.Add(matrix);
            Controls.Add(pythonInput);
            Controls.Add(dimensionOfSpaceInput);
            Controls.Add(label1);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "NonLinearProgrammingForm";
            Text = "Non Linear Programming";
            Load += NonLinearProgrammingForm_Load;
            ((System.ComponentModel.ISupportInitialize)dimensionOfSpaceInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)matrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)numberOfEquationsInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)startPointInput).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private NumericUpDown dimensionOfSpaceInput;
        private TextBox pythonInput;
        private DataGridView matrix;
        private Label label2;
        private NumericUpDown numberOfEquationsInput;
        private Button solveButton;
        private Label label3;
        private DataGridView startPointInput;
    }
}