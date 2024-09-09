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
            solveMinButton.Location = new Point(284, 440);
            solveMinButton.Name = "solveMinButton";
            solveMinButton.Size = new Size(344, 34);
            solveMinButton.TabIndex = 7;
            solveMinButton.Text = "Minimize with all methods";
            solveMinButton.UseVisualStyleBackColor = true;
            solveMinButton.Click += solveMinButton_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(634, 7);
            label3.Name = "label3";
            label3.Size = new Size(289, 15);
            label3.TabIndex = 8;
            label3.Text = "Starting Point, if empty a random one will be guessed";
            // 
            // solveMaxButton
            // 
            solveMaxButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            solveMaxButton.Cursor = Cursors.Hand;
            solveMaxButton.Location = new Point(10, 440);
            solveMaxButton.Name = "solveMaxButton";
            solveMaxButton.Size = new Size(268, 34);
            solveMaxButton.TabIndex = 10;
            solveMaxButton.Text = "Maximize with all methods";
            solveMaxButton.UseVisualStyleBackColor = true;
            solveMaxButton.Click += solveMaxButton_Click;
            // 
            // pythonFunctionControl1
            // 
            pythonFunctionControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pythonFunctionControl1.Location = new Point(634, 100);
            pythonFunctionControl1.Margin = new Padding(3, 2, 3, 2);
            pythonFunctionControl1.Name = "pythonFunctionControl1";
            pythonFunctionControl1.Size = new Size(678, 374);
            pythonFunctionControl1.TabIndex = 11;
            // 
            // polyhedronControl1
            // 
            polyhedronControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            polyhedronControl1.Location = new Point(10, 10);
            polyhedronControl1.MinimumSize = new Size(515, 312);
            polyhedronControl1.Name = "polyhedronControl1";
            polyhedronControl1.Size = new Size(618, 424);
            polyhedronControl1.TabIndex = 12;
            // 
            // linearFunctionControl1
            // 
            linearFunctionControl1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            linearFunctionControl1.Location = new Point(634, 28);
            linearFunctionControl1.MinimumSize = new Size(512, 68);
            linearFunctionControl1.Name = "linearFunctionControl1";
            linearFunctionControl1.Size = new Size(674, 68);
            linearFunctionControl1.TabIndex = 13;
            linearFunctionControl1.Vector = null;
            // 
            // NonLinearProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1318, 489);
            Controls.Add(linearFunctionControl1);
            Controls.Add(polyhedronControl1);
            Controls.Add(pythonFunctionControl1);
            Controls.Add(solveMaxButton);
            Controls.Add(label3);
            Controls.Add(solveMinButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1334, 528);
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