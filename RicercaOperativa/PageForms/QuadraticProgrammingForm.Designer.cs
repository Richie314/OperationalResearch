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
            hessianMatrix.Location = new Point(8, 185);
            hessianMatrix.Margin = new Padding(0);
            hessianMatrix.Name = "hessianMatrix";
            hessianMatrix.RowHeadersWidth = 50;
            hessianMatrix.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            hessianMatrix.Size = new Size(229, 244);
            hessianMatrix.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 9);
            label3.Name = "label3";
            label3.Size = new Size(103, 15);
            label3.TabIndex = 10;
            label3.Text = "Linear coefficients";
            // 
            // maximizeBtn
            // 
            maximizeBtn.Cursor = Cursors.Hand;
            maximizeBtn.Location = new Point(116, 117);
            maximizeBtn.Name = "maximizeBtn";
            maximizeBtn.Size = new Size(101, 59);
            maximizeBtn.TabIndex = 11;
            maximizeBtn.Text = "Maximize";
            maximizeBtn.UseVisualStyleBackColor = true;
            maximizeBtn.Click += maximizeBtn_Click;
            // 
            // minimizeBtn
            // 
            minimizeBtn.Cursor = Cursors.Hand;
            minimizeBtn.Location = new Point(8, 117);
            minimizeBtn.Name = "minimizeBtn";
            minimizeBtn.Size = new Size(88, 41);
            minimizeBtn.TabIndex = 12;
            minimizeBtn.Text = "Minimize";
            minimizeBtn.UseVisualStyleBackColor = true;
            minimizeBtn.Click += minimizeBtn_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 161);
            label4.Name = "label4";
            label4.Size = new Size(85, 15);
            label4.TabIndex = 14;
            label4.Text = "Hessian matrix";
            // 
            // polyhedronControl1
            // 
            polyhedronControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            polyhedronControl1.Location = new Point(240, 117);
            polyhedronControl1.MinimumSize = new Size(717, 320);
            polyhedronControl1.Name = "polyhedronControl1";
            polyhedronControl1.Size = new Size(717, 320);
            polyhedronControl1.TabIndex = 15;
            // 
            // linearFunctionControl1
            // 
            linearFunctionControl1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linearFunctionControl1.Location = new Point(8, 27);
            linearFunctionControl1.MinimumSize = new Size(512, 68);
            linearFunctionControl1.Name = "linearFunctionControl1";
            linearFunctionControl1.Size = new Size(949, 84);
            linearFunctionControl1.TabIndex = 16;
            linearFunctionControl1.Vector = null;
            // 
            // linearFunctionControl2
            // 
            linearFunctionControl2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            linearFunctionControl2.Location = new Point(8, 458);
            linearFunctionControl2.MinimumSize = new Size(512, 68);
            linearFunctionControl2.Name = "linearFunctionControl2";
            linearFunctionControl2.Size = new Size(949, 84);
            linearFunctionControl2.TabIndex = 17;
            linearFunctionControl2.Vector = null;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(13, 440);
            label1.Name = "label1";
            label1.Size = new Size(467, 15);
            label1.TabIndex = 18;
            label1.Text = "Point of Interest (starting point for iterative methods and needs multipliers to be found)";
            // 
            // QuadraticProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(970, 558);
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
            MinimumSize = new Size(958, 597);
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