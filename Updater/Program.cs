using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.Windows;

namespace Downloader
{
    static class Program
    {
        private static UpdateEngine _updater;
        
        private static Version _currentVersion;

        private static string _url;
        
        /// <summary>
        /// Directory where the update is downloaded to
        /// and the old files and folders are deleted.
        /// </summary>
        private static string _downloadDirectory;

        /// <summary>
        /// The third arg that equals false if
        /// we don't show "Up to date" dialog if up to date.
        /// </summary>
        public static string UpToDate;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                _updater = new HttpEngine();
                _updater.Type = UpdateEngine.InstanceType.Install;
                Downloader download = new Downloader(_updater);
                download.ShowDialog();
                return;
            }
            
            _currentVersion = new Version(args[0]);
            _downloadDirectory = args[1];
            _url = args[2];
            
            _updater = new HttpEngine(_downloadDirectory, _currentVersion, _url);
            
            if (args.Length >= 4) UpToDate = args[3];
            
            var versions = new List<DetailVersion>(_updater.GetUpdateVersions().OrderBy(v => v.Version));
            if (versions.Count > 0)
            {
                if (versions.Last().Version > new Version(args[0]))
                {
                    try
                    {
                        _updater.Type = UpdateEngine.InstanceType.Update;
                        _updater.DownloadDirectory = _downloadDirectory;
                        Downloader updater = new Downloader(_updater);
                        updater.ShowDialog();
                    }
                    catch (Exception e) // Other wise if the server is wrong or somethingwe get a hidious error message
                    {
                        if (UpToDate != "false")
                        {
                            MessageBox.Show(e.ToString());
                        }   
                    }
                }
                else
                {
                    if (UpToDate != "false")
                    {
                        MessageBox.Show("Up to date!");
                    }       
                }                
            }
            else
            {
                if (UpToDate != "false")
                {
                    MessageBox.Show("Cannot communicate with server!");
                }   
            }
        }

        public static void StartInstall(string updaterCurrentDirectory)
        {
            var startinfo = new ProcessStartInfo();
            startinfo.WorkingDirectory = _downloadDirectory;
            startinfo.FileName = updaterCurrentDirectory;
            Process.Start(startinfo);
        }

        public static bool IsAdminProcess()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        
    }
}
