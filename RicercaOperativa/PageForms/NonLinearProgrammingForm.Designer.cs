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
            solveMinButton = new Button();
            label3 = new Label();
            solveMaxButton = new Button();
            pythonFunctionControl1 = new PythonFunctionControl();
            polyhedronControl1 = new PolyhedronControl();
            linearFunctionControl1 = new LinearFunctionControl();
            SuspendLayout();
            // 
            // solveMinButton
            // 
            solveMinButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            solveMinButton.Cursor = Cursors.Hand;
            solveMinButton.Location = new Point(318, 586);
            solveMinButton.Margin = new Padding(3, 4, 3, 4);
            solveMinButton.Name = "solveMinButton";
            solveMinButton.Size = new Size(283, 45);
            solveMinButton.TabIndex = 7;
            solveMinButton.Text = "Minimize with all methods";
            solveMinButton.UseVisualStyleBackColor = true;
            solveMinButton.Click += solveMinButton_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(607, 9);
            label3.Name = "label3";
            label3.Size = new Size(363, 20);
            label3.TabIndex = 8;
            label3.Text = "Starting Point, if empty a random one will be guessed";
            // 
            // solveMaxButton
            // 
            solveMaxButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            solveMaxButton.Cursor = Cursors.Hand;
            solveMaxButton.Location = new Point(12, 586);
            solveMaxButton.Margin = new Padding(3, 4, 3, 4);
            solveMaxButton.Name = "solveMaxButton";
            solveMaxButton.Size = new Size(253, 45);
            solveMaxButton.TabIndex = 10;
            solveMaxButton.Text = "Maximize with all methods";
            solveMaxButton.UseVisualStyleBackColor = true;
            solveMaxButton.Click += solveMaxButton_Click;
            // 
            // pythonFunctionControl1
            // 
            pythonFunctionControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pythonFunctionControl1.Location = new Point(607, 134);
            pythonFunctionControl1.Name = "pythonFunctionControl1";
            pythonFunctionControl1.Size = new Size(775, 498);
            pythonFunctionControl1.TabIndex = 11;
            // 
            // polyhedronControl1
            // 
            polyhedronControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            polyhedronControl1.Location = new Point(12, 13);
            polyhedronControl1.Margin = new Padding(3, 4, 3, 4);
            polyhedronControl1.MinimumSize = new Size(589, 416);
            polyhedronControl1.Name = "polyhedronControl1";
            polyhedronControl1.Size = new Size(589, 565);
            polyhedronControl1.TabIndex = 12;
            // 
            // linearFunctionControl1
            // 
            linearFunctionControl1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            linearFunctionControl1.Location = new Point(607, 37);
            linearFunctionControl1.Margin = new Padding(3, 4, 3, 4);
            linearFunctionControl1.MinimumSize = new Size(585, 90);
            linearFunctionControl1.Name = "linearFunctionControl1";
            linearFunctionControl1.Size = new Size(770, 90);
            linearFunctionControl1.TabIndex = 13;
            linearFunctionControl1.Vector = null;
            // 
            // NonLinearProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1389, 644);
            Controls.Add(linearFunctionControl1);
            Controls.Add(polyhedronControl1);
            Controls.Add(pythonFunctionControl1);
            Controls.Add(solveMaxButton);
            Controls.Add(label3);
            Controls.Add(solveMinButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(932, 691);
            Name = "NonLinearProgrammingForm";
            Text = "Non Linear Programming";
            Load += NonLinearProgrammingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button solveMinButton;
        private Label label3;
        private Button solveMaxButton;
        private PythonFunctionControl pythonFunctionControl1;
        private PolyhedronControl polyhedronControl1;
        private LinearFunctionControl linearFunctionControl1;
    }
}