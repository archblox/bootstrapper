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
using System.Security.Permissions;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.DirectoryServices;
using System.Windows.Forms;
namespace ARCHBLOXBootstrapper
{

    public partial class ARCHBLOX : Form
    {
        public bool IsCompleted = false;
        public bool DontEvenBother = false;
        private static WebClient wc = new WebClient();
        private static ManualResetEvent handle = new ManualResetEvent(true);

        public ARCHBLOX()
        {
            InitializeComponent();
            byte[] raw = wc.DownloadData("https://archblox.com/client/version.txt");
            string webData = Encoding.UTF8.GetString(raw);
            string version_string = webData;
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Archblx\", @"Versions\");
            string clientPath = Path.Combine(folderPath, version_string + @"\");
            string filePath = Path.Combine(clientPath, Path.GetFileName(@"https://archblox.com/client/" + version_string + ".zip"));
            {
                if (Directory.Exists(folderPath))
                {
                    DialogResult res = MessageBox.Show("Do you want to delete previous installs of ARCHBLOX?", "ARCHBLOX", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.Yes)
                    {
                        label2.Text = "Removing previous installs...";
                        Directory.Delete(folderPath, true);

                    }
                }
            }
            wc.DownloadProgressChanged += Client_DownloadProgressChanged;
            wc.DownloadFileCompleted += Client_DownloadFileCompleted;
            progressBar2.Style = ProgressBarStyle.Marquee;
            label2.Text = "Configuring ARCHBLOX...";
            wc.DownloadProgressChanged += Client_DownloadProgressChanged;
            wc.DownloadFileCompleted += Client_DownloadFileCompleted;
            if (Directory.Exists(clientPath))
            {
                DialogResult res = MessageBox.Show("The latest version of ARCHBLOX is already installed. Do you want to re-install it?", "ARCHBLOX", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    label2.Text = "Removing previous install...";
                    Directory.Delete(clientPath, true);
                    
                }
                if (res == DialogResult.No)
                {
                    label2.Text = "Cancelled install.";
                    DontEvenBother = true;
                    Application.Exit();
                }
            }
            if (DontEvenBother == false)
            {
                Directory.CreateDirectory(clientPath);
                wc.DownloadFileAsync(new Uri(@"https://archblox.com/client/" + version_string + ".zip"), filePath);
                progressBar2.Style = ProgressBarStyle.Blocks;
                handle.WaitOne();
            }
        }

        private void ARCHBLOX_Load(object sender, EventArgs e)
        {
           // nothing
        }
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (IsCompleted == false)
            {
                IsCompleted = true;
                byte[] raw = wc.DownloadData("https://archblox.com/client/version.txt");
                string webData = Encoding.UTF8.GetString(raw);
                string version_string = webData;
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Archblx\", @"Versions\");
                string clientPath = Path.Combine(folderPath, version_string + @"\");
                string filePath = Path.Combine(clientPath, Path.GetFileName(@"https://archblox.com/client/" + version_string + ".zip"));
                ZipFile.ExtractToDirectory(filePath, clientPath);
                File.Delete(filePath);
                label2.Text = "ARCHBLOX has been installed!";

            }
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