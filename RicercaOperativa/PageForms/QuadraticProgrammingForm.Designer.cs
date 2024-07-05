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
            label1 = new Label();
            label2 = new Label();
            variablesCountInput = new NumericUpDown();
            equationsCountInput = new NumericUpDown();
            resetTableBtn = new Button();
            linearCoeff = new DataGridView();
            label3 = new Label();
            maximizeBtn = new Button();
            minimizeBtn = new Button();
            constraintMatrix = new DataGridView();
            label4 = new Label();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)hessianMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)variablesCountInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)equationsCountInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)linearCoeff).BeginInit();
            ((System.ComponentModel.ISupportInitialize)constraintMatrix).BeginInit();
            SuspendLayout();
            // 
            // hessianMatrix
            // 
            hessianMatrix.AllowUserToAddRows = false;
            hessianMatrix.AllowUserToDeleteRows = false;
            hessianMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            hessianMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            hessianMatrix.Location = new Point(29, 164);
            hessianMatrix.Margin = new Padding(0);
            hessianMatrix.Name = "hessianMatrix";
            hessianMatrix.RowHeadersWidth = 50;
            hessianMatrix.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            hessianMatrix.Size = new Size(473, 467);
            hessianMatrix.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(48, 25);
            label1.Name = "label1";
            label1.Size = new Size(112, 20);
            label1.TabIndex = 4;
            label1.Text = "Variables Count";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            variablesCountInput.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // equationsCountInput
            // 
            equationsCountInput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            equationsCountInput.Location = new Point(456, 22);
            equationsCountInput.Margin = new Padding(3, 4, 3, 4);
            equationsCountInput.Name = "equationsCountInput";
            equationsCountInput.Size = new Size(137, 27);
            equationsCountInput.TabIndex = 7;
            equationsCountInput.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // resetTableBtn
            // 
            resetTableBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            // linearCoeff
            // 
            linearCoeff.AllowUserToAddRows = false;
            linearCoeff.AllowUserToDeleteRows = false;
            linearCoeff.AllowUserToResizeColumns = false;
            linearCoeff.AllowUserToResizeRows = false;
            linearCoeff.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linearCoeff.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            linearCoeff.Location = new Point(176, 65);
            linearCoeff.Margin = new Padding(0);
            linearCoeff.Name = "linearCoeff";
            linearCoeff.RowHeadersWidth = 50;
            linearCoeff.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            linearCoeff.Size = new Size(881, 65);
            linearCoeff.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(29, 87);
            label3.Name = "label3";
            label3.Size = new Size(129, 20);
            label3.TabIndex = 10;
            label3.Text = "Linear coefficients";
            // 
            // maximizeBtn
            // 
            maximizeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            maximizeBtn.Cursor = Cursors.Hand;
            maximizeBtn.Location = new Point(29, 649);
            maximizeBtn.Margin = new Padding(3, 4, 3, 4);
            maximizeBtn.Name = "maximizeBtn";
            maximizeBtn.Size = new Size(473, 71);
            maximizeBtn.TabIndex = 11;
            maximizeBtn.Text = "Maximize";
            maximizeBtn.UseVisualStyleBackColor = true;
            maximizeBtn.Click += maximizeBtn_Click;
            // 
            // minimizeBtn
            // 
            minimizeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            minimizeBtn.Cursor = Cursors.Hand;
            minimizeBtn.Location = new Point(519, 649);
            minimizeBtn.Margin = new Padding(3, 4, 3, 4);
            minimizeBtn.Name = "minimizeBtn";
            minimizeBtn.Size = new Size(538, 71);
            minimizeBtn.TabIndex = 12;
            minimizeBtn.Text = "Minimize";
            minimizeBtn.UseVisualStyleBackColor = true;
            minimizeBtn.Click += minimizeBtn_Click;
            // 
            // constraintMatrix
            // 
            constraintMatrix.AllowUserToAddRows = false;
            constraintMatrix.AllowUserToDeleteRows = false;
            constraintMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            constraintMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            constraintMatrix.Location = new Point(519, 164);
            constraintMatrix.Margin = new Padding(0);
            constraintMatrix.Name = "constraintMatrix";
            constraintMatrix.RowHeadersWidth = 50;
            constraintMatrix.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            constraintMatrix.Size = new Size(538, 467);
            constraintMatrix.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(29, 137);
            label4.Name = "label4";
            label4.Size = new Size(106, 20);
            label4.TabIndex = 14;
            label4.Text = "Hessian matrix";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(519, 137);
            label5.Name = "label5";
            label5.Size = new Size(124, 20);
            label5.TabIndex = 15;
            label5.Text = "Costraints Ax<=b";
            // 
            // QuadraticProgrammingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1075, 736);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(constraintMatrix);
            Controls.Add(minimizeBtn);
            Controls.Add(maximizeBtn);
            Controls.Add(label3);
            Controls.Add(linearCoeff);
            Controls.Add(resetTableBtn);
            Controls.Add(equationsCountInput);
            Controls.Add(variablesCountInput);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(hessianMatrix);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "QuadraticProgrammingForm";
            Text = "Quadratic Programming";
            Load += LinearProgrammingForm_Load;
            ((System.ComponentModel.ISupportInitialize)hessianMatrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)variablesCountInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)equationsCountInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)linearCoeff).EndInit();
            ((System.ComponentModel.ISupportInitialize)constraintMatrix).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView hessianMatrix;
        private Label label1;
        private Label label2;
        private NumericUpDown variablesCountInput;
        private NumericUpDown equationsCountInput;
        private Button resetTableBtn;
        private DataGridView linearCoeff;
        private Label label3;
        private Button maximizeBtn;
        private Button minimizeBtn;
        private DataGridView constraintMatrix;
        private Label label4;
        private Label label5;
    }
}