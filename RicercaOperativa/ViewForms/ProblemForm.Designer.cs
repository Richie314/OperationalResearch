namespace OperationalResearch.ViewForms
{
    partial class ProblemForm<ProblemType>
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
            textBox = new RichTextBox();
            SuspendLayout();
            // 
            // textBox
            // 
            textBox.Dock = DockStyle.Fill;
            textBox.Font = new Font("Cascadia Mono", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox.Location = new Point(0, 0);
            textBox.Margin = new Padding(10, 10, 10, 10);
            textBox.Name = "textBox";
            textBox.ReadOnly = true;
            textBox.ShortcutsEnabled = false;
            textBox.Size = new Size(800, 450);
            textBox.TabIndex = 0;
            textBox.Text = "";
            // 
            // ProblemForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox);
            Name = "ProblemForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "Solving problem";
            Load += ProblemForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox textBox;
    }
}