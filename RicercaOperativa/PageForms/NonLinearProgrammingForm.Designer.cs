namespace OperationalResearch.PageForms
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
            solveMinButton = new Button();
            label3 = new Label();
            startPointInput = new DataGridView();
            solveMaxButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dimensionOfSpaceInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)matrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numberOfEquationsInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)startPointInput).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button1.Cursor = Cursors.Hand;
            button1.Location = new Point(282, 20);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(129, 74);
            button1.TabIndex = 0;
            button1.Text = "Apply";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 28);
            label1.Name = "label1";
            label1.Size = new Size(140, 20);
            label1.TabIndex = 1;
            label1.Text = "Dimension of space";
            // 
            // dimensionOfSpaceInput
            // 
            dimensionOfSpaceInput.Location = new Point(186, 24);
            dimensionOfSpaceInput.Margin = new Padding(3, 4, 3, 4);
            dimensionOfSpaceInput.Name = "dimensionOfSpaceInput";
            dimensionOfSpaceInput.Size = new Size(81, 27);
            dimensionOfSpaceInput.TabIndex = 2;
            dimensionOfSpaceInput.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // pythonInput
            // 
            pythonInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pythonInput.Font = new Font("CaskaydiaCove NF", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pythonInput.Location = new Point(14, 319);
            pythonInput.Margin = new Padding(3, 4, 3, 4);
            pythonInput.Multiline = true;
            pythonInput.Name = "pythonInput";
            pythonInput.PlaceholderText = "Insert the python code for the functions here";
            pythonInput.ScrollBars = ScrollBars.Both;
            pythonInput.Size = new Size(886, 259);
            pythonInput.TabIndex = 3;
            // 
            // matrix
            // 
            matrix.AllowUserToAddRows = false;
            matrix.AllowUserToDeleteRows = false;
            matrix.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            matrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            matrix.Location = new Point(14, 99);
            matrix.Margin = new Padding(3, 4, 3, 4);
            matrix.Name = "matrix";
            matrix.RowHeadersWidth = 51;
            matrix.Size = new Size(887, 215);
            matrix.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 61);
            label2.Name = "label2";
            label2.Size = new Size(150, 20);
            label2.TabIndex = 5;
            label2.Text = "Number of equations";
            // 
            // numberOfEquationsInput
            // 
            numberOfEquationsInput.Location = new Point(186, 60);
            numberOfEquationsInput.Margin = new Padding(3, 4, 3, 4);
            numberOfEquationsInput.Name = "numberOfEquationsInput";
            numberOfEquationsInput.Size = new Size(81, 27);
            numberOfEquationsInput.TabIndex = 6;
            numberOfEquationsInput.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // solveMinButton
            // 
            solveMinButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            solveMinButton.Cursor = Cursors.Hand;
            solveMinButton.Location = new Point(457, 586);
            solveMinButton.Margin = new Padding(3, 4, 3, 4);
            solveMinButton.Name = "solveMinButton";
            solveMinButton.Size = new Size(443, 45);
            solveMinButton.TabIndex = 7;
            solveMinButton.Text = "Minimize with all methods";
            solveMinButton.UseVisualStyleBackColor = true;
            solveMinButton.Click += solveMinButton_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(417, 7);
            label3.Name = "label3";
            label3.Size = new Size(363, 20);
            label3.TabIndex = 8;
            label3.Text = "Starting Point, if empty a random one will be guessed";
            // 
            // startPointInput
            // 
            startPointInput.AllowUserToAddRows = false;
            startPointInput.AllowUserToDeleteRows = false;
            startPointInput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            startPointInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            startPointInput.Location = new Point(417, 31);
            startPointInput.Margin = new Padding(3, 4, 3, 4);
            startPointInput.Name = "startPointInput";
            startPointInput.RowHeadersWidth = 51;
            startPointInput.Size = new Size(483, 63);
            startPointInput.TabIndex = 9;
            // 
            // solveMaxButton
            // 
            solveMaxButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            solveMaxButton.Cursor = Cursors.Hand;
            solveMaxButton.Location = new Point(12, 586);
            solveMaxButton.Margin = new Padding(3, 4, 3, 4);
            solveMaxButton.Name = "solveMaxButton";
            solveMaxButton.Size = new Size(439, 45);
            solveMaxButton.TabIndex = 10;
            solveMaxButton.Text = "Maximize with all methods";
            solveMaxButton.UseVisualStyleBackColor = true;
            solveMaxButton.Click += solveMaxButton_Click;
            // 
            // NonLinearProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 644);
            Controls.Add(solveMaxButton);
            Controls.Add(startPointInput);
            Controls.Add(label3);
            Controls.Add(solveMinButton);
            Controls.Add(numberOfEquationsInput);
            Controls.Add(label2);
            Controls.Add(matrix);
            Controls.Add(pythonInput);
            Controls.Add(dimensionOfSpaceInput);
            Controls.Add(label1);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(932, 691);
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
        private Button solveMinButton;
        private Label label3;
        private DataGridView startPointInput;
        private Button solveMaxButton;
    }
}