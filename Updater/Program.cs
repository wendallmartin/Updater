using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Updater
{
    internal static class Program
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
        private static string _upToDate;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length < 3)// Fresh install.
            {
                _updater = new HttpEngine
                {
                    Type = UpdateEngine.InstanceType.Install
                };
                Downloader download = new Downloader(_updater);
                download.ShowDialog();
                return;
            }
            
            _currentVersion = new Version(args[0]);
            _downloadDirectory = args[1];
            _url = args[2];
            
            _updater = new HttpEngine(_downloadDirectory, _url);
            
            if (args.Length >= 4) _upToDate = args[3];
            
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
                    catch (Exception e) // Other wise if the server is wrong or something we get a terrible error message
                    {
                        if (_upToDate != "false")
                        {
                            MessageBox.Show(e.ToString());
                        }   
                    }
                }
                else
                {
                    if (_upToDate != "false")
                    {
                        MessageBox.Show("Up to date!");
                    }       
                }                
            }
            else
            {
                if (_upToDate != "false")
                {
                    MessageBox.Show("Cannot communicate with server!");
                }   
            }
        }

        public static void StartInstall(string updaterCurrentDirectory)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = _downloadDirectory, FileName = updaterCurrentDirectory
            };
            Process.Start(startInfo);
        }        
    }
}
