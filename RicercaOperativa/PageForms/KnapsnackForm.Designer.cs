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
            mainGrid.Location = new Point(14, 96);
            mainGrid.Margin = new Padding(3, 4, 3, 4);
            mainGrid.Name = "mainGrid";
            mainGrid.RowHeadersWidth = 51;
            mainGrid.Size = new Size(648, 386);
            mainGrid.TabIndex = 0;
            // 
            // applyItemsBtn
            // 
            applyItemsBtn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            applyItemsBtn.Cursor = Cursors.Hand;
            applyItemsBtn.Location = new Point(214, 16);
            applyItemsBtn.Margin = new Padding(3, 4, 3, 4);
            applyItemsBtn.Name = "applyItemsBtn";
            applyItemsBtn.Size = new Size(120, 62);
            applyItemsBtn.TabIndex = 1;
            applyItemsBtn.Text = "Apply";
            applyItemsBtn.UseVisualStyleBackColor = true;
            applyItemsBtn.Click += applyItemsBtn_Click;
            // 
            // boolean
            // 
            boolean.AutoSize = true;
            boolean.Location = new Point(31, 54);
            boolean.Margin = new Padding(3, 4, 3, 4);
            boolean.Name = "boolean";
            boolean.Size = new Size(177, 24);
            boolean.TabIndex = 2;
            boolean.Text = "Use boolean variables";
            boolean.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(28, 20);
            label1.Name = "label1";
            label1.Size = new Size(121, 20);
            label1.TabIndex = 3;
            label1.Text = "Number of items";
            // 
            // numberOfItems
            // 
            numberOfItems.Location = new Point(149, 16);
            numberOfItems.Margin = new Padding(3, 4, 3, 4);
            numberOfItems.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numberOfItems.Name = "numberOfItems";
            numberOfItems.Size = new Size(59, 27);
            numberOfItems.TabIndex = 4;
            numberOfItems.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // solveBtn
            // 
            solveBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            solveBtn.Cursor = Cursors.Hand;
            solveBtn.Location = new Point(354, 16);
            solveBtn.Margin = new Padding(3, 4, 3, 4);
            solveBtn.Name = "solveBtn";
            solveBtn.Size = new Size(308, 62);
            solveBtn.TabIndex = 5;
            solveBtn.Text = "Solve";
            solveBtn.UseVisualStyleBackColor = true;
            solveBtn.Click += solveBtn_Click;
            // 
            // KnapsnackForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(675, 498);
            Controls.Add(solveBtn);
            Controls.Add(numberOfItems);
            Controls.Add(label1);
            Controls.Add(boolean);
            Controls.Add(applyItemsBtn);
            Controls.Add(mainGrid);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(693, 545);
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