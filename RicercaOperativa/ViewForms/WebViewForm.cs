using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationalResearch.ViewForms
{
    public partial class WebViewForm : Form
    {
        private string url;
        public WebViewForm(string url)
        {
            InitializeComponent();
            this.url = url;
            this.url = string.IsNullOrWhiteSpace(url) ? 
                "https://www.github.com" : url;
        }


        private void WebViewForm_Load(object sender, EventArgs e)
        {
            webView.Source = new Uri(url);
        }
    }
}
