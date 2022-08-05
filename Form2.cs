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
        // set up variables
        public bool IsCompleted = false;
        public bool exitafterarg = false;
        public bool DontEvenBother = false;
        private static WebClient wc = new WebClient();
        private static ManualResetEvent handle = new ManualResetEvent(true);

        private void CreateShortcut()
        {
            // create a shorcut on the user's desktop
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
            // check for an internet connection first
            try
            {
                wc.DownloadData("http://archblox.com/studio/version.txt");
            }
            catch
            {
                MessageBox.Show("An error occoured while starting ARCHBLOX Studio\n\nDetails: HttpOpenRequest failed for GET http://archblox.com/studio/version.txt, Error ID: 6", "ARCHBLOX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            // setup paths
            byte[] raw = wc.DownloadData("http://archblox.com/studio/version.txt");
            string webData = Encoding.UTF8.GetString(raw);
            string version_string = webData;
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Archblx\", @"Studio\", @"Versions\");
            string clientPath = Path.Combine(folderPath, version_string + @"\");
            string filePath = Path.Combine(clientPath, Path.GetFileName(@"http://archblox.com/client/" + version_string + ".zip"));
            string studioPath = Path.Combine(clientPath, "ArchbloxStudio.exe");
            ARCHBLOXProtocol.ARCHBLOXURIProtocol.Register();
            CreateShortcut();
            // TODO: Make studio place launching URI.
            if (Directory.Exists(clientPath) & System.IO.File.Exists(studioPath))
            {
                // studio exists, create shortcut and launch studio
                label1.Text = "Launching Studio...";
                DontEvenBother = true;
                CreateShortcut();
                var pProcess = new Process();
                pProcess.StartInfo.FileName = studioPath;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                pProcess.StartInfo.CreateNoWindow = false;
                pProcess.Start();
            }
            if (Directory.Exists(folderPath) & DontEvenBother == false)
            {
                // ask user if they want to delete previous installs
                DialogResult res = MessageBox.Show("Do you want to delete previous installs of ARCHBLOX Studio?", "ARCHBLOX Studio", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    label1.Text = "Removing previous installs...";
                    Directory.Delete(folderPath, true);
                }
            }
            // setup events
            wc.DownloadProgressChanged += Client_DownloadProgressChanged;
            wc.DownloadFileCompleted += Client_DownloadFileCompleted;
            progressBar2.Style = ProgressBarStyle.Marquee;
            if (DontEvenBother == false)
            {
                // install studio
                label2.Text = "Configuring ARCHBLOX...";
                Directory.CreateDirectory(clientPath);
                wc.DownloadFileAsync(new Uri(@"http://archblox.com/studio/" + version_string + ".zip"), filePath);
                progressBar2.Style = ProgressBarStyle.Blocks;
                handle.WaitOne();
            } else
            {
                // close program
                Thread.Sleep(3000);
                Environment.Exit(0);
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
                // the download has completed, extract.zip, create shortcut and launch! yay!!!
                IsCompleted = true;
                byte[] raw = wc.DownloadData("http://archblox.com/studio/version.txt");
                string webData = Encoding.UTF8.GetString(raw);
                string version_string = webData;
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Archblx\", @"Studio\", @"Versions\");
                string clientPath = Path.Combine(folderPath, version_string + @"\");
                string filePath = Path.Combine(clientPath, Path.GetFileName(@"http://archblox.com/studio/" + version_string + ".zip"));
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
            // update progress bar and text
            progressBar2.Minimum = 0;
            double receive = double.Parse(e.BytesReceived.ToString());
            double total = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = receive / total * 100;
            label2.Text = "Installing ARCHBLOX... (" + Math.Truncate(percentage).ToString() + "% Completed)";
            progressBar2.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
    }
}