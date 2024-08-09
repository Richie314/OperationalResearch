namespace OperationalResearch.PageForms
{
    partial class QuadraticProgrammingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuadraticProgrammingForm));
            hessianMatrix = new DataGridView();
            label3 = new Label();
            maximizeBtn = new Button();
            minimizeBtn = new Button();
            label4 = new Label();
            polyhedronControl1 = new PolyhedronControl();
            linearFunctionControl1 = new LinearFunctionControl();
            linearFunctionControl2 = new LinearFunctionControl();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)hessianMatrix).BeginInit();
            SuspendLayout();
            // 
            // hessianMatrix
            // 
            hessianMatrix.AllowUserToAddRows = false;
            hessianMatrix.AllowUserToDeleteRows = false;
            hessianMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            hessianMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            hessianMatrix.Location = new Point(9, 247);
            hessianMatrix.Margin = new Padding(0);
            hessianMatrix.Name = "hessianMatrix";
            hessianMatrix.RowHeadersWidth = 50;
            hessianMatrix.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            hessianMatrix.Size = new Size(343, 325);
            hessianMatrix.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 12);
            label3.Name = "label3";
            label3.Size = new Size(129, 20);
            label3.TabIndex = 10;
            label3.Text = "Linear coefficients";
            // 
            // maximizeBtn
            // 
            maximizeBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            maximizeBtn.Cursor = Cursors.Hand;
            maximizeBtn.Location = new Point(237, 173);
            maximizeBtn.Margin = new Padding(3, 4, 3, 4);
            maximizeBtn.Name = "maximizeBtn";
            maximizeBtn.Size = new Size(115, 62);
            maximizeBtn.TabIndex = 11;
            maximizeBtn.Text = "Maximize";
            maximizeBtn.UseVisualStyleBackColor = true;
            maximizeBtn.Click += maximizeBtn_Click;
            // 
            // minimizeBtn
            // 
            minimizeBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            minimizeBtn.Cursor = Cursors.Hand;
            minimizeBtn.Location = new Point(121, 173);
            minimizeBtn.Margin = new Padding(3, 4, 3, 4);
            minimizeBtn.Name = "minimizeBtn";
            minimizeBtn.Size = new Size(100, 62);
            minimizeBtn.TabIndex = 12;
            minimizeBtn.Text = "Minimize";
            minimizeBtn.UseVisualStyleBackColor = true;
            minimizeBtn.Click += minimizeBtn_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 215);
            label4.Name = "label4";
            label4.Size = new Size(106, 20);
            label4.TabIndex = 14;
            label4.Text = "Hessian matrix";
            // 
            // polyhedronControl1
            // 
            polyhedronControl1.Location = new Point(358, 156);
            polyhedronControl1.Margin = new Padding(3, 4, 3, 4);
            polyhedronControl1.MinimumSize = new Size(589, 416);
            polyhedronControl1.Name = "polyhedronControl1";
            polyhedronControl1.Size = new Size(736, 427);
            polyhedronControl1.TabIndex = 15;
            // 
            // linearFunctionControl1
            // 
            linearFunctionControl1.Location = new Point(9, 36);
            linearFunctionControl1.Margin = new Padding(3, 4, 3, 4);
            linearFunctionControl1.MinimumSize = new Size(585, 90);
            linearFunctionControl1.Name = "linearFunctionControl1";
            linearFunctionControl1.Size = new Size(1085, 112);
            linearFunctionControl1.TabIndex = 16;
            linearFunctionControl1.Vector = null;
            // 
            // linearFunctionControl2
            // 
            linearFunctionControl2.Location = new Point(9, 611);
            linearFunctionControl2.Margin = new Padding(3, 4, 3, 4);
            linearFunctionControl2.MinimumSize = new Size(585, 90);
            linearFunctionControl2.Name = "linearFunctionControl2";
            linearFunctionControl2.Size = new Size(1085, 112);
            linearFunctionControl2.TabIndex = 17;
            linearFunctionControl2.Vector = null;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 587);
            label1.Name = "label1";
            label1.Size = new Size(587, 20);
            label1.TabIndex = 18;
            label1.Text = "Point of Interest (starting point for iterative methods and needs multipliers to be found)";
            // 
            // QuadraticProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1108, 736);
            Controls.Add(label1);
            Controls.Add(linearFunctionControl2);
            Controls.Add(linearFunctionControl1);
            Controls.Add(polyhedronControl1);
            Controls.Add(label4);
            Controls.Add(minimizeBtn);
            Controls.Add(maximizeBtn);
            Controls.Add(label3);
            Controls.Add(hessianMatrix);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1093, 783);
            Name = "QuadraticProgrammingForm";
            Text = "Quadratic Programming";
            Load += LinearProgrammingForm_Load;
            ((System.ComponentModel.ISupportInitialize)hessianMatrix).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView hessianMatrix;
        private Label label3;
        private Button maximizeBtn;
        private Button minimizeBtn;
        private Label label4;
        private PolyhedronControl polyhedronControl1;
        private LinearFunctionControl linearFunctionControl1;
        private LinearFunctionControl linearFunctionControl2;
        private Label label1;
    }
}