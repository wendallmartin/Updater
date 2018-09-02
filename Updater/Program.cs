using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Updater;

namespace FTPUpdater
{
    static class Program
    {
        private static UpdateEngine _updater;
        
        public static Version CurrentVersion;
        
        /// <summary>
        /// Directory where the update is downloaded to
        /// and the old files and folders are deleted.
        /// </summary>
        public static string CurrentDirectory;

        public static string ParentProcess = "";

        /// <summary>
        /// The fourth arg that equals false if
        /// we don't show "Up to date" dialog if up to date.
        /// </summary>
        public static string UpToDate;

        [STAThread]
        private static void Main(string[] args)
        {
            Installer download = new Installer();
            download.ShowDialog();
            if (args.Length < 2)
            {
                MessageBox.Show("Argument missmatch!! \nArgments are: string current version, string current directory.");
                return;
            }
            
            CurrentVersion = new Version(args[0]);
            CurrentDirectory = args[1];

            if (args.Length >= 3) ParentProcess = args[2];
                
            if (args.Length >= 4) UpToDate = args[3];

            _updater = new HttpEngine();
            
            var versions = new List<DetailVersion>(_updater.GetUpdateVersions().OrderBy(v => v.Version));
            if (versions.Count > 0)
            {
                if (versions.Last().Version > new Version(args[0]))
                {
                    MessageBoxResult result = MessageBox.Show("Update available. Update now?", "Update", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            UpdateWindow window = new UpdateWindow(_updater);
                            window.ShowDialog();
                        }
                        catch (Exception e ) // Other wise if the server is wrong or somethingwe get a hidious error message
                        {
                            // do nothing
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
                MessageBox.Show("Up to date!");
            }
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

        public static void StartParentProcess()
        {
            var startinfo = new ProcessStartInfo();
            startinfo.WorkingDirectory = CurrentDirectory;
            startinfo.FileName = Path.Combine(CurrentDirectory, ParentProcess);
            Process.Start(startinfo);
        }
    }
}
