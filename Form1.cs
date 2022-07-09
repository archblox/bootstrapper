using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.IO.Compression;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.DirectoryServices;
using System.Windows.Forms;
namespace ARCHBLOXBootstrapper
{
    public partial class ARCHBLOX : Form
    {
        private static WebClient wc = new WebClient();
        private static ManualResetEvent handle = new ManualResetEvent(true);

        public ARCHBLOX()
        {
            InitializeComponent();
            wc.DownloadProgressChanged += Client_DownloadProgressChanged;
            wc.DownloadFileCompleted += Client_DownloadFileCompleted;
            progressBar2.Style = ProgressBarStyle.Marquee;
            label2.Text = "Configuring ARCHBLOX...";
            wc.DownloadProgressChanged += Client_DownloadProgressChanged;
            wc.DownloadFileCompleted += Client_DownloadFileCompleted;
            string fileName = System.IO.Path.GetFileName(@"https://archblox.com/img/ARCHBLOXarched.png");
            string folderPath = Path.Combine(@"%LocalAppData%", @"\ARCHBLOX\");
            string filePath = Path.Combine(folderPath, "test.png");
            Directory.CreateDirectory(folderPath);
            wc.DownloadFileAsync(new Uri(@"https://archblox.com/img/ARCHBLOXarched.png"), filePath);
            progressBar2.Style = ProgressBarStyle.Blocks;
            handle.WaitOne();
        }

        private void ARCHBLOX_Load(object sender, EventArgs e)
        {
           // nothing
        }
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label2.Text = "ARCHBLOX has been installed!";
            handle.WaitOne();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Minimum = 0;
            double receive = double.Parse(e.BytesReceived.ToString());
            double total = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = receive / total * 100;
            label2.Text = "Installing ARCHBLOX... (" + Math.Truncate(percentage).ToString() + "% Completed)";
            progressBar2.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
    }
}