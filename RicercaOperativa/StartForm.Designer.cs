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
            IntegerLinearProgrammingMenuItem = new ToolStripMenuItem();
            NetworksProgrammingMenuItem = new ToolStripMenuItem();
            NonlinearProgrammingMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem = new ToolStripMenuItem();
            CreditsToolStripMenuItem = new ToolStripMenuItem();
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
            LinearProgrammingMenuItem.Name = "LinearProgrammingMenuItem";
            LinearProgrammingMenuItem.Size = new Size(278, 26);
            LinearProgrammingMenuItem.Text = "Linear Programming";
            LinearProgrammingMenuItem.Click += LinearProgrammingMenuItem_Click;
            // 
            // IntegerLinearProgrammingMenuItem
            // 
            IntegerLinearProgrammingMenuItem.Name = "IntegerLinearProgrammingMenuItem";
            IntegerLinearProgrammingMenuItem.Size = new Size(278, 26);
            IntegerLinearProgrammingMenuItem.Text = "Integer Linear Programming";
            // 
            // NetworksProgrammingMenuItem
            // 
            NetworksProgrammingMenuItem.Name = "NetworksProgrammingMenuItem";
            NetworksProgrammingMenuItem.Size = new Size(278, 26);
            NetworksProgrammingMenuItem.Text = "Network Programming";
            // 
            // NonlinearProgrammingMenuItem
            // 
            NonlinearProgrammingMenuItem.Name = "NonlinearProgrammingMenuItem";
            NonlinearProgrammingMenuItem.Size = new Size(278, 26);
            NonlinearProgrammingMenuItem.Text = "Non Linear Programming";
            NonlinearProgrammingMenuItem.Click += NonlinearProgrammingMenuItem_Click;
            // 
            // ToolStripMenuItem
            // 
            ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { CreditsToolStripMenuItem });
            ToolStripMenuItem.Name = "ToolStripMenuItem";
            ToolStripMenuItem.Size = new Size(30, 24);
            ToolStripMenuItem.Text = "&?";
            // 
            // CreditsToolStripMenuItem
            // 
            CreditsToolStripMenuItem.Name = "CreditsToolStripMenuItem";
            CreditsToolStripMenuItem.Size = new Size(138, 26);
            CreditsToolStripMenuItem.Text = "&Credits";
            CreditsToolStripMenuItem.Click += CreditsToolStripMenuItem_Click;
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
    }
}
