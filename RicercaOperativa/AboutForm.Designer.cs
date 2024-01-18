namespace RicercaOperativa
{
    partial class AboutForm
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
            mainTextLabel = new Label();
            mainButton = new Button();
            SuspendLayout();
            // 
            // mainTextLabel
            // 
            mainTextLabel.AutoSize = true;
            mainTextLabel.Location = new Point(12, 46);
            mainTextLabel.Name = "mainTextLabel";
            mainTextLabel.Size = new Size(0, 20);
            mainTextLabel.TabIndex = 0;
            // 
            // mainButton
            // 
            mainButton.Cursor = Cursors.Hand;
            mainButton.Location = new Point(11, 177);
            mainButton.Name = "mainButton";
            mainButton.Size = new Size(348, 64);
            mainButton.TabIndex = 1;
            mainButton.Text = "GitHub";
            mainButton.UseVisualStyleBackColor = true;
            mainButton.Click += mainButton_Click;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(371, 253);
            Controls.Add(mainButton);
            Controls.Add(mainTextLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "AboutForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "About this software";
            Load += AboutForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label mainTextLabel;
        private Button mainButton;
    }
}