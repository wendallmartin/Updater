using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FTPUpdater
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow
    {
        private readonly object _updateLock = new object();
        private readonly object _textLock = new object();
        private UpdateEngine _updater;

        public UpdateWindow(UpdateEngine updater)
        {
            InitializeComponent();
            _updater = updater;
            foreach (DetailVersion detailedVersion in updater.GetUpdateVersions())
            {
                if (detailedVersion.Version <= Program.CurrentVersion) continue;
                VersionBox.Items.Add(detailedVersion.Version.ToString());
            }

            if (VersionBox.Items.Count == 0)
            {
                MessageBox.Show("You are up to date!");
                Close();
                return;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = ((ComboBox) sender).SelectedItem.ToString();
            VersionBox.Text = text;
            UpdateDetails.Text = _updater.GetUpdateVersions().FirstOrDefault(v => v.Version.ToString() == text)?.Details ?? throw new Exception("Version not found!");
            UpdateButton.IsEnabled = true;
        }

        private void UpdateButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(VersionBox.Text)) return;
            ProgressBar.Visibility = Visibility.Visible;
            lock (_updateLock)
            {
                try
                {
                    string version = VersionBox.Text;
                    if (!string.IsNullOrEmpty(version))
                    {
                        new Thread(() =>
                        {
                            Program.StopParentProcess(); // kills the process given at startup threw command line params
                            _updater.UpdateChangedEvent += OnUpdateChanged;
                            _updater.Update(new Version(version));
                            string zipFile = Program.CurrentDirectory + "//" + version + ".zip";
                            using (FileStream zipToOpen = new FileStream(zipFile, FileMode.Open))
                            {
                                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                                {
                                    foreach (ZipArchiveEntry entry in archive.Entries)
                                    {
                                        
                                        if (string.IsNullOrEmpty(entry.Name))
                                            continue;
                                        
                                        DirectoryInfo dir = new DirectoryInfo(Program.CurrentDirectory);
                                        IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories); 
                                        
                                        IEnumerable<FileInfo> fileQuery =  
                                            from file in fileList  
                                            where file.Name == entry.Name  
                                            orderby file.Name  
                                            select file;

                                        foreach (FileInfo info in fileQuery)
                                        {
                                            File.Delete(info.FullName);
                                        }
//                                        Debug.WriteLine("Entry name:" + entry.Name);
//                                        if (File.Exists(Path.Combine(Program.CurrentDirectory, entry.Name)))
//                                        {
//                                            Debug.WriteLine(Path.Combine(Program.CurrentDirectory, entry.Name));
//                                            File.Delete(Path.Combine(Program.CurrentDirectory, entry.Name));
//                                        }
//
//                                        if (Directory.Exists(Path.Combine(Program.CurrentDirectory, entry.Name)))
//                                        {
//                                            Debug.WriteLine(Path.Combine(Program.CurrentDirectory, entry.Name));
//                                            Directory.Delete(Path.Combine(Program.CurrentDirectory, entry.Name), true);
//                                        }
                                    }
                                }
                            }

                            ZipFile.ExtractToDirectory(zipFile, Program.CurrentDirectory);
                            Program.StartParentProcess();
                            Dispatcher.Invoke(Close);
                        }).Start();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void OnUpdateChanged(double recieved, double total)
        {
            double percent = recieved / total * 100;
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = int.Parse(Math.Truncate(percent).ToString());
                BytesLabel.Content = $"{recieved}mb of {total}mb";
            });
        }
    }
}