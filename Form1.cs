using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ARCHBLOXBootstrapper
{
    public partial class ARCHBLOX : Form
    {
        WebClient client;
        public ARCHBLOX()
        {
            InitializeComponent();
        }

        private void ARCHBLOX_Load(object sender, EventArgs e)
        {
            client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            label2.Text = "Configuring ARCHBLOX...";
            Uri uri = new Uri("https://archblox.com/img/ARCHBLOXarched.png");
            string fileName = System.IO.Path.GetFileName(uri.AbsolutePath);
            client.DownloadFile(uri, Application.StartupPath + "/" + fileName);
        }
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("ARCHBLOX has been successfully installed!", "yay!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Minimum = 0;
            double receive = double.Parse(e.BytesReceived.ToString());
            double total = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = receive / total * 100;
            label2.Text = "Installing ARCHBLOX...";
            progressBar2.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
    }
}
