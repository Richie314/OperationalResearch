namespace OperationalResearch.PageForms
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
            maximizeBtn = new Button();
            startBaseInput = new TextBox();
            label4 = new Label();
            minimizeBtn = new Button();
            polyhedronControl1 = new PolyhedronControl();
            linearFunctionControl1 = new LinearFunctionControl();
            SuspendLayout();
            // 
            // maximizeBtn
            // 
            maximizeBtn.Cursor = Cursors.Hand;
            maximizeBtn.Location = new Point(29, 39);
            maximizeBtn.Margin = new Padding(3, 4, 3, 4);
            maximizeBtn.Name = "maximizeBtn";
            maximizeBtn.Size = new Size(289, 43);
            maximizeBtn.TabIndex = 11;
            maximizeBtn.Text = "Maximize";
            maximizeBtn.UseVisualStyleBackColor = true;
            maximizeBtn.Click += maximizeBtn_Click;
            // 
            // startBaseInput
            // 
            startBaseInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            startBaseInput.Location = new Point(187, 91);
            startBaseInput.Margin = new Padding(3, 4, 3, 4);
            startBaseInput.Name = "startBaseInput";
            startBaseInput.Size = new Size(420, 27);
            startBaseInput.TabIndex = 12;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(29, 95);
            label4.Name = "label4";
            label4.Size = new Size(155, 20);
            label4.TabIndex = 13;
            label4.Text = "Starting Basis indexes:";
            // 
            // minimizeBtn
            // 
            minimizeBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            minimizeBtn.Cursor = Cursors.Hand;
            minimizeBtn.Location = new Point(347, 37);
            minimizeBtn.Margin = new Padding(3, 4, 3, 4);
            minimizeBtn.Name = "minimizeBtn";
            minimizeBtn.Size = new Size(261, 43);
            minimizeBtn.TabIndex = 15;
            minimizeBtn.Text = "Minimize";
            minimizeBtn.UseVisualStyleBackColor = true;
            minimizeBtn.Click += minimizeBtn_Click;
            // 
            // polyhedronControl1
            // 
            polyhedronControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            polyhedronControl1.Location = new Point(14, 240);
            polyhedronControl1.Margin = new Padding(3, 5, 3, 5);
            polyhedronControl1.MinimumSize = new Size(589, 416);
            polyhedronControl1.Name = "polyhedronControl1";
            polyhedronControl1.Size = new Size(606, 492);
            polyhedronControl1.TabIndex = 16;
            // 
            // linearFunctionControl1
            // 
            linearFunctionControl1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linearFunctionControl1.Location = new Point(23, 157);
            linearFunctionControl1.Margin = new Padding(3, 5, 3, 5);
            linearFunctionControl1.MinimumSize = new Size(585, 75);
            linearFunctionControl1.Name = "linearFunctionControl1";
            linearFunctionControl1.Size = new Size(585, 75);
            linearFunctionControl1.TabIndex = 17;
            linearFunctionControl1.Vector = null;
            // 
            // LinearProgrammingForm_Primal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(633, 748);
            Controls.Add(linearFunctionControl1);
            Controls.Add(polyhedronControl1);
            Controls.Add(minimizeBtn);
            Controls.Add(label4);
            Controls.Add(startBaseInput);
            Controls.Add(maximizeBtn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(649, 784);
            Name = "LinearProgrammingForm_Primal";
            Text = "Primal Form Simplex";
            Load += LinearProgrammingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button maximizeBtn;
        private TextBox startBaseInput;
        private Label label4;
        private Button minimizeBtn;
        private PolyhedronControl polyhedronControl1;
        private LinearFunctionControl linearFunctionControl1;
    }
}