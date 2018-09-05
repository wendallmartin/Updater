using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.Windows;

namespace Updater
{
    static class Program
    {
        private static UpdateEngine _updater;
        
        private static Version _currentVersion;
        
        /// <summary>
        /// Directory where the update is downloaded to
        /// and the old files and folders are deleted.
        /// </summary>
        private static string _currentDirectory;

        public static string ParentProcess = "";

        /// <summary>
        /// The fourth arg that equals false if
        /// we don't show "Up to date" dialog if up to date.
        /// </summary>
        public static string UpToDate;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                InstallNew();
                return;
            }
            
            _currentVersion = new Version(args[0]);
            _currentDirectory = args[1];
            
            _updater = new HttpEngine(_currentDirectory, _currentVersion);

            if (args.Length >= 3) ParentProcess = args[2];
                
            if (args.Length >= 4) UpToDate = args[3];
            
            var versions = new List<DetailVersion>(_updater.GetUpdateVersions().OrderBy(v => v.Version));
            if (versions.Count > 0)
            {
                if (versions.Last().Version > new Version(args[0]))
                {
                    try
                    {
                        _updater.Type = UpdateEngine.InstanceType.Update;
                        _updater.CurrentDirectory = _currentDirectory;
                        Installer updater = new Installer(_updater);
                        updater.ShowDialog();
                    }
                    catch (Exception e) // Other wise if the server is wrong or somethingwe get a hidious error message
                    {
                        // do nothing
                        MessageBox.Show(e.ToString());
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
                MessageBox.Show("Up to date!");
            }
        }

        private static void InstallNew()
        {
            _updater = new HttpEngine();
            _updater.Type = UpdateEngine.InstanceType.Install;
            Installer download = new Installer(_updater);
            download.ShowDialog();
        }

        public static void StopParentProcess()
        {
            if(string.IsNullOrEmpty(ParentProcess))
                return;
            
            foreach (Process process in Process.GetProcessesByName(ParentProcess))
            {
                process.Kill();
            }
        }

        public static void StartParentProcess(string updaterCurrentDirectory)
        {
            var startinfo = new ProcessStartInfo();
            startinfo.WorkingDirectory = _currentDirectory;
            startinfo.FileName = Path.Combine(updaterCurrentDirectory, "TheTimeApp.exe");
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
