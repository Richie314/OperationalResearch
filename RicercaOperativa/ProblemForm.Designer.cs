namespace RicercaOperativa
{
    partial class ProblemForm
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
            textBox.Font = new Font("Lucida Console", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox.Location = new Point(0, 0);
            textBox.Margin = new Padding(11, 13, 11, 13);
            textBox.Name = "textBox";
            textBox.ReadOnly = true;
            textBox.ShortcutsEnabled = false;
            textBox.Size = new Size(914, 600);
            textBox.TabIndex = 0;
            textBox.Text = "";
            // 
            // ProblemForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(textBox);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ProblemForm";
            ShowIcon = false;
            Text = "Solving problem";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox textBox;
    }
}