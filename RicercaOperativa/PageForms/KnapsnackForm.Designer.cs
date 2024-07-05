namespace OperationalResearch.PageForms
{
    partial class KnapsnackForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KnapsnackForm));
            mainGrid = new DataGridView();
            applyItemsBtn = new Button();
            boolean = new CheckBox();
            label1 = new Label();
            numberOfItems = new NumericUpDown();
            solveBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)mainGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numberOfItems).BeginInit();
            SuspendLayout();
            // 
            // mainGrid
            // 
            mainGrid.AllowUserToAddRows = false;
            mainGrid.AllowUserToDeleteRows = false;
            mainGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            mainGrid.Location = new Point(12, 51);
            mainGrid.Name = "mainGrid";
            mainGrid.Size = new Size(676, 275);
            mainGrid.TabIndex = 0;
            // 
            // applyItemsBtn
            // 
            applyItemsBtn.Cursor = Cursors.Hand;
            applyItemsBtn.Location = new Point(256, 7);
            applyItemsBtn.Name = "applyItemsBtn";
            applyItemsBtn.Size = new Size(75, 34);
            applyItemsBtn.TabIndex = 1;
            applyItemsBtn.Text = "Apply";
            applyItemsBtn.UseVisualStyleBackColor = true;
            applyItemsBtn.Click += applyItemsBtn_Click;
            // 
            // boolean
            // 
            boolean.AutoSize = true;
            boolean.Location = new Point(350, 14);
            boolean.Name = "boolean";
            boolean.Size = new Size(69, 19);
            boolean.TabIndex = 2;
            boolean.Text = "Boolean";
            boolean.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 15);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 3;
            label1.Text = "Number of items";
            // 
            // numberOfItems
            // 
            numberOfItems.Location = new Point(130, 12);
            numberOfItems.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numberOfItems.Name = "numberOfItems";
            numberOfItems.Size = new Size(120, 23);
            numberOfItems.TabIndex = 4;
            numberOfItems.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // solveBtn
            // 
            solveBtn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            solveBtn.Cursor = Cursors.Hand;
            solveBtn.Location = new Point(445, 4);
            solveBtn.Name = "solveBtn";
            solveBtn.Size = new Size(241, 43);
            solveBtn.TabIndex = 5;
            solveBtn.Text = "Solve";
            solveBtn.UseVisualStyleBackColor = true;
            solveBtn.Click += solveBtn_Click;
            // 
            // KnapsnackForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(solveBtn);
            Controls.Add(numberOfItems);
            Controls.Add(label1);
            Controls.Add(boolean);
            Controls.Add(applyItemsBtn);
            Controls.Add(mainGrid);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "KnapsnackForm";
            Text = "Knapsnack's Problemm";
            Load += KnapsnackForm_Load;
            ((System.ComponentModel.ISupportInitialize)mainGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)numberOfItems).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView mainGrid;
        private Button applyItemsBtn;
        private CheckBox boolean;
        private Label label1;
        private NumericUpDown numberOfItems;
        private Button solveBtn;
    }
}