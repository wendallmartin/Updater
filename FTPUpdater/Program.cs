using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FTPUpdater
{
    static class Program
    {
        public static Version CurrentVersion;
        
        /// <summary>
        /// Directory where the update is downloaded to
        /// and the old files and folders are deleted.
        /// </summary>
        public static string CurrentDirectory;

        public static string ParentProcess = "";
        
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                MessageBox.Show("Argument missmatch!! \nArgments are: string current version, string current directory.");
                return;
            }

            if (args.Length >= 3)
            {
                ParentProcess = args[2];
            }
            
            CurrentVersion = new Version(args[0]);
            CurrentDirectory = args[1];

            try
            {
                UpdateWindow window = new UpdateWindow();
                window.ShowDialog();
            }
            catch (Exception )// Other wise if the server is wrong or somethingwe get a hidious error message
            {
                // do nothing
            }
        }
        
        public static void StopParentProcess()
        {
            if(string.IsNullOrEmpty(Program.ParentProcess))
                return;
            
            foreach (Process process in Process.GetProcessesByName(Program.ParentProcess))
            {
                process.Kill();
            }
        }

        public static void StartParentProcess()
        {
            Process.Start(Path.Combine(Program.CurrentDirectory, Program.ParentProcess));
        }
    }
}
