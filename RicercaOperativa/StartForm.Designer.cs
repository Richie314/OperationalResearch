namespace RicercaOperativa
{
    partial class StartForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartForm));
            menuStrip1 = new MenuStrip();
            problemsStripMenuItem = new ToolStripMenuItem();
            LinearProgrammingMenuItem = new ToolStripMenuItem();
            primalFormToolStripMenuItem = new ToolStripMenuItem();
            dualFormToolStripMenuItem = new ToolStripMenuItem();
            IntegerLinearProgrammingMenuItem = new ToolStripMenuItem();
            productionRevenueMaximazationToolStripMenuItem = new ToolStripMenuItem();
            assignmentOfMinCostToolStripMenuItem = new ToolStripMenuItem();
            genericAssignmentOfMinCostToolStripMenuItem = new ToolStripMenuItem();
            knapsnacToolStripMenuItem = new ToolStripMenuItem();
            travellingSalesmansProblemToolStripMenuItem = new ToolStripMenuItem();
            NetworksProgrammingMenuItem = new ToolStripMenuItem();
            flowOfMinCostToolStripMenuItem = new ToolStripMenuItem();
            NonlinearProgrammingMenuItem = new ToolStripMenuItem();
            genericProblemToolStripMenuItem = new ToolStripMenuItem();
            quadraticProgrammingToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem = new ToolStripMenuItem();
            CreditsToolStripMenuItem = new ToolStripMenuItem();
            testToolStripMenuItem = new ToolStripMenuItem();
            webviewToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { problemsStripMenuItem, ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(6, 3, 0, 3);
            menuStrip1.Size = new Size(914, 30);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // problemsStripMenuItem
            // 
            problemsStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { LinearProgrammingMenuItem, IntegerLinearProgrammingMenuItem, NetworksProgrammingMenuItem, NonlinearProgrammingMenuItem });
            problemsStripMenuItem.Name = "problemsStripMenuItem";
            problemsStripMenuItem.Size = new Size(85, 24);
            problemsStripMenuItem.Text = "&Problems";
            // 
            // LinearProgrammingMenuItem
            // 
            LinearProgrammingMenuItem.DropDownItems.AddRange(new ToolStripItem[] { primalFormToolStripMenuItem, dualFormToolStripMenuItem });
            LinearProgrammingMenuItem.Name = "LinearProgrammingMenuItem";
            LinearProgrammingMenuItem.Size = new Size(278, 26);
            LinearProgrammingMenuItem.Text = "Linear Programming";
            // 
            // primalFormToolStripMenuItem
            // 
            primalFormToolStripMenuItem.Name = "primalFormToolStripMenuItem";
            primalFormToolStripMenuItem.Size = new Size(172, 26);
            primalFormToolStripMenuItem.Text = "Primal Form";
            primalFormToolStripMenuItem.Click += primalFormToolStripMenuItem_Click;
            // 
            // dualFormToolStripMenuItem
            // 
            dualFormToolStripMenuItem.Name = "dualFormToolStripMenuItem";
            dualFormToolStripMenuItem.Size = new Size(172, 26);
            dualFormToolStripMenuItem.Text = "Dual Form";
            dualFormToolStripMenuItem.Click += dualFormToolStripMenuItem_Click;
            // 
            // IntegerLinearProgrammingMenuItem
            // 
            IntegerLinearProgrammingMenuItem.DropDownItems.AddRange(new ToolStripItem[] { productionRevenueMaximazationToolStripMenuItem, assignmentOfMinCostToolStripMenuItem, genericAssignmentOfMinCostToolStripMenuItem, knapsnacToolStripMenuItem, travellingSalesmansProblemToolStripMenuItem });
            IntegerLinearProgrammingMenuItem.Name = "IntegerLinearProgrammingMenuItem";
            IntegerLinearProgrammingMenuItem.Size = new Size(278, 26);
            IntegerLinearProgrammingMenuItem.Text = "Integer Linear Programming";
            // 
            // productionRevenueMaximazationToolStripMenuItem
            // 
            productionRevenueMaximazationToolStripMenuItem.Name = "productionRevenueMaximazationToolStripMenuItem";
            productionRevenueMaximazationToolStripMenuItem.Size = new Size(318, 26);
            productionRevenueMaximazationToolStripMenuItem.Text = "Production revenue maximazation";
            productionRevenueMaximazationToolStripMenuItem.Click += productionRevenueMaximazationToolStripMenuItem_Click;
            // 
            // assignmentOfMinCostToolStripMenuItem
            // 
            assignmentOfMinCostToolStripMenuItem.Name = "assignmentOfMinCostToolStripMenuItem";
            assignmentOfMinCostToolStripMenuItem.Size = new Size(318, 26);
            assignmentOfMinCostToolStripMenuItem.Text = "Assignment of min cost";
            assignmentOfMinCostToolStripMenuItem.Click += assignmentOfMinCostToolStripMenuItem_Click;
            // 
            // genericAssignmentOfMinCostToolStripMenuItem
            // 
            genericAssignmentOfMinCostToolStripMenuItem.Name = "genericAssignmentOfMinCostToolStripMenuItem";
            genericAssignmentOfMinCostToolStripMenuItem.Size = new Size(318, 26);
            genericAssignmentOfMinCostToolStripMenuItem.Text = "Generic assignment of min cost";
            genericAssignmentOfMinCostToolStripMenuItem.Click += genericAssignmentOfMinCostToolStripMenuItem_Click;
            // 
            // knapsnacToolStripMenuItem
            // 
            knapsnacToolStripMenuItem.Name = "knapsnacToolStripMenuItem";
            knapsnacToolStripMenuItem.Size = new Size(318, 26);
            knapsnacToolStripMenuItem.Text = "Knapsnack's Problem";
            knapsnacToolStripMenuItem.Click += knapsnacToolStripMenuItem_Click;
            // 
            // travellingSalesmansProblemToolStripMenuItem
            // 
            travellingSalesmansProblemToolStripMenuItem.Name = "travellingSalesmansProblemToolStripMenuItem";
            travellingSalesmansProblemToolStripMenuItem.Size = new Size(318, 26);
            travellingSalesmansProblemToolStripMenuItem.Text = "Travelling Salesman's Problem";
            travellingSalesmansProblemToolStripMenuItem.Click += travellingSalesmansProblemToolStripMenuItem_Click;
            // 
            // NetworksProgrammingMenuItem
            // 
            NetworksProgrammingMenuItem.DropDownItems.AddRange(new ToolStripItem[] { flowOfMinCostToolStripMenuItem });
            NetworksProgrammingMenuItem.Name = "NetworksProgrammingMenuItem";
            NetworksProgrammingMenuItem.Size = new Size(278, 26);
            NetworksProgrammingMenuItem.Text = "Network Programming";
            // 
            // flowOfMinCostToolStripMenuItem
            // 
            flowOfMinCostToolStripMenuItem.Name = "flowOfMinCostToolStripMenuItem";
            flowOfMinCostToolStripMenuItem.Size = new Size(201, 26);
            flowOfMinCostToolStripMenuItem.Text = "Flow of min cost";
            flowOfMinCostToolStripMenuItem.Click += flowOfMinCostToolStripMenuItem_Click;
            // 
            // NonlinearProgrammingMenuItem
            // 
            NonlinearProgrammingMenuItem.DropDownItems.AddRange(new ToolStripItem[] { genericProblemToolStripMenuItem, quadraticProgrammingToolStripMenuItem });
            NonlinearProgrammingMenuItem.Name = "NonlinearProgrammingMenuItem";
            NonlinearProgrammingMenuItem.Size = new Size(278, 26);
            NonlinearProgrammingMenuItem.Text = "Non Linear Programming";
            // 
            // genericProblemToolStripMenuItem
            // 
            genericProblemToolStripMenuItem.Name = "genericProblemToolStripMenuItem";
            genericProblemToolStripMenuItem.Size = new Size(252, 26);
            genericProblemToolStripMenuItem.Text = "Generic problem";
            genericProblemToolStripMenuItem.Click += genericProblemToolStripMenuItem_Click;
            // 
            // quadraticProgrammingToolStripMenuItem
            // 
            quadraticProgrammingToolStripMenuItem.Name = "quadraticProgrammingToolStripMenuItem";
            quadraticProgrammingToolStripMenuItem.Size = new Size(252, 26);
            quadraticProgrammingToolStripMenuItem.Text = "Quadratic Programming";
            quadraticProgrammingToolStripMenuItem.Click += quadraticProgrammingToolStripMenuItem_Click;
            // 
            // ToolStripMenuItem
            // 
            ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { CreditsToolStripMenuItem, testToolStripMenuItem });
            ToolStripMenuItem.Name = "ToolStripMenuItem";
            ToolStripMenuItem.Size = new Size(30, 24);
            ToolStripMenuItem.Text = "&?";
            // 
            // CreditsToolStripMenuItem
            // 
            CreditsToolStripMenuItem.Name = "CreditsToolStripMenuItem";
            CreditsToolStripMenuItem.Size = new Size(224, 26);
            CreditsToolStripMenuItem.Text = "&Credits";
            CreditsToolStripMenuItem.Click += CreditsToolStripMenuItem_Click;
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { webviewToolStripMenuItem });
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(224, 26);
            testToolStripMenuItem.Text = "Test";
            // 
            // webviewToolStripMenuItem
            // 
            webviewToolStripMenuItem.Name = "webviewToolStripMenuItem";
            webviewToolStripMenuItem.Size = new Size(224, 26);
            webviewToolStripMenuItem.Text = "Webview";
            webviewToolStripMenuItem.Click += webviewToolStripMenuItem_Click;
            // 
            // StartForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 4, 3, 4);
            Name = "StartForm";
            Text = "Operational Research";
            Load += StartForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem problemsStripMenuItem;
        private ToolStripMenuItem LinearProgrammingMenuItem;
        private ToolStripMenuItem IntegerLinearProgrammingMenuItem;
        private ToolStripMenuItem NetworksProgrammingMenuItem;
        private ToolStripMenuItem NonlinearProgrammingMenuItem;
        private ToolStripMenuItem ToolStripMenuItem;
        private ToolStripMenuItem CreditsToolStripMenuItem;
        private ToolStripMenuItem primalFormToolStripMenuItem;
        private ToolStripMenuItem dualFormToolStripMenuItem;
        private ToolStripMenuItem assignmentOfMinCostToolStripMenuItem;
        private ToolStripMenuItem travellingSalesmansProblemToolStripMenuItem;
        private ToolStripMenuItem productionRevenueMaximazationToolStripMenuItem;
        private ToolStripMenuItem genericAssignmentOfMinCostToolStripMenuItem;
        private ToolStripMenuItem knapsnacToolStripMenuItem;
        private ToolStripMenuItem genericProblemToolStripMenuItem;
        private ToolStripMenuItem quadraticProgrammingToolStripMenuItem;
        private ToolStripMenuItem flowOfMinCostToolStripMenuItem;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem webviewToolStripMenuItem;
    }
}
