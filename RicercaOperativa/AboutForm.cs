using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RicercaOperativa
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            mainTextLabel.MaximumSize = new Size(ClientSize.Width - 30, 0);
            mainTextLabel.Location = new Point(15, 15);
            mainTextLabel.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;
            mainTextLabel.AutoSize = true;

            mainTextLabel.Text =
                "This software was created by Riccardo Ciucci and is an open source project." +
                Environment.NewLine + Environment.NewLine +
                "You can contribute to it through the following GitHub:";
        }

        private void mainButton_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/Richie314";
            OpenBrowser(url);
        }
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show(
                    "Error",
                    "It was impossible to open the default browser",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
