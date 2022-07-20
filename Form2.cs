using IWshRuntimeLibrary;
using System;
using System.ComponentModel;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
namespace ARCHBLOXBootstrapper
{   
    public partial class ARCHBLOX : Form
    {
        public bool IsCompleted = false;
        public bool DontEvenBother = false;
        private static WebClient wc = new WebClient();
        private static ManualResetEvent handle = new ManualResetEvent(true);

        private void CreateShortcut()
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\ARCHBLOX Studio.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "ARCHBLOX Studio";
            shortcut.TargetPath = Extensions.GetExecutablePath();
            shortcut.Save();
        }

        public ARCHBLOX()
        {
            InitializeComponent();
            byte[] raw = wc.DownloadData("https://archblox.com/studio/version.txt");
            string webData = Encoding.UTF8.GetString(raw);
            string version_string = webData;
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Archblx\", @"Studio\", @"Versions\");
            string clientPath = Path.Combine(folderPath, version_string + @"\");
            string filePath = Path.Combine(clientPath, Path.GetFileName(@"https://archblox.com/client/" + version_string + ".zip"));
            string studioPath = Path.Combine(clientPath, "ArchbloxStudio.exe");
            if (Directory.Exists(clientPath) & System.IO.File.Exists(studioPath))
            {
                CreateShortcut();
                var pProcess = new Process();
                pProcess.StartInfo.FileName = studioPath;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                pProcess.StartInfo.CreateNoWindow = false;
                pProcess.Start();
                Environment.Exit(0);
            }
            if (Directory.Exists(folderPath))
            {
                DialogResult res = MessageBox.Show("Do you want to delete previous installs of ARCHBLOX Studio?", "ARCHBLOX Studio", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    label1.Text = "Removing previous installs...";
                    Directory.Delete(folderPath, true);
                }
            }
            wc.DownloadProgressChanged += Client_DownloadProgressChanged;
            wc.DownloadFileCompleted += Client_DownloadFileCompleted;
            progressBar2.Style = ProgressBarStyle.Marquee;
            label2.Text = "Configuring ARCHBLOX...";
            wc.DownloadProgressChanged += Client_DownloadProgressChanged;
            wc.DownloadFileCompleted += Client_DownloadFileCompleted;
            if (DontEvenBother == false)
            {
                Directory.CreateDirectory(clientPath);
                wc.DownloadFileAsync(new Uri(@"https://archblox.com/studio/" + version_string + ".zip"), filePath);
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
                byte[] raw = wc.DownloadData("https://archblox.com/studio/version.txt");
                string webData = Encoding.UTF8.GetString(raw);
                string version_string = webData;
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Archblx\", @"Studio\", @"Versions\");
                string clientPath = Path.Combine(folderPath, version_string + @"\");
                string filePath = Path.Combine(clientPath, Path.GetFileName(@"https://archblox.com/studio/" + version_string + ".zip"));
                string studioPath = Path.Combine(clientPath, "ArchbloxStudio.exe");
                ZipFile.ExtractToDirectory(filePath, clientPath);
                System.IO.File.Delete(filePath);
                label2.Text = "ARCHBLOX Studio has been installed!";
                CreateShortcut();
                var pProcess = new Process();
                pProcess.StartInfo.FileName = studioPath;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                pProcess.StartInfo.CreateNoWindow = false;
                pProcess.Start();
                Environment.Exit(0);

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