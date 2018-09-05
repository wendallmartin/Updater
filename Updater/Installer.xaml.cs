using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Updater.States;

namespace Updater
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for Installer.xaml
    /// </summary>
    public partial class Installer
    {
        public readonly UpdateEngine Updater;

        public UpdateState State;
        
        public void HideAll()
        {
            Welcome.Visibility = Visibility.Hidden;
            Updates.Visibility = Visibility.Hidden;
            Directory.Visibility = Visibility.Hidden;
            Download.Visibility = Visibility.Hidden;
            Finish.Visibility = Visibility.Hidden;
        }

        public Installer(UpdateEngine updater)
        {
            InitializeComponent();

            Updater = updater;
            
            HideAll();

            switch (updater.Type)
            {
                case UpdateEngine.InstanceType.Install:
                    Welcome.Visibility = Visibility.Visible;
                    State = WelcomeState.State;
                    
                    if (Updater.GetUpdateVersions().Count > 0)
                    {
                        VersionLabel.Content = Updater.GetUpdateVersions().OrderBy(v => v.Version).Last().Version.ToString();
                    }

                    break;
                case UpdateEngine.InstanceType.Update:
                    Updates.Visibility = Visibility.Visible;
                    State = global::Updater.States.Updates.State;
                    
                    if (Updater.GetUpdateVersions().Count > 0)
                    {
                        foreach (DetailVersion updateVersion in Updater.GetUpdateVersions())
                        {
                            UpdateComboBox.Items.Add(updateVersion.Version.ToString());
                        }

                        UpdateComboBox.SelectionChanged += OnUpdateSelectionChanged;
                        UpdateComboBox.Text = Updater.GetUpdateVersions().OrderBy(v => v.Version).Last().Version.ToString();
                        UpdateDetails.Text = Updater.GetUpdateVersions().OrderBy(v => v.Version).Last().Details;
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DirectoryTextBlock.Text = Updater.CurrentDirectory;

        }

        private void OnUpdateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDetails.Text = Updater.GetUpdateVersions().FirstOrDefault(v => v.Version.ToString() == UpdateComboBox.SelectedValue.ToString())?.Details;
        }

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            State.Next(this);
        }

        private void Btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            State.Previous(this);
        }

        public void SetDowloadProgress(double recieve, double total)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = recieve / total * 100;
                ProgressText.Content = $"{recieve}mb of {total}mb";
            });
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = @"Install location";
            dialog.SelectedPath = @"C:\\Program Files\\";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                Updater.CurrentDirectory = dialog.SelectedPath;
            }

            DirectoryTextBlock.Text = Updater.CurrentDirectory;
        }

        public void DownloadVersion(string version)
        {
            try
            {
                if (!string.IsNullOrEmpty(version))
                {
                    new Thread(() =>
                    {
                        string oldName = Process.GetCurrentProcess().MainModule.FileName + "_OLD";
                        if (File.Exists(oldName)) File.Delete(oldName);
                        File.Move(Process.GetCurrentProcess().MainModule.FileName, Process.GetCurrentProcess().MainModule.FileName + "_OLD");
                        Program.StopParentProcess(); // kills the process given at startup threw command line params
                        Updater.UpdateChangedEvent += SetDowloadProgress;
                        Updater.Update(new Version(version));
                        string zipFile = Updater.CurrentDirectory + "//" + version + ".zip";
                        using (FileStream zipToOpen = new FileStream(zipFile, FileMode.Open))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                {
                                    if (string.IsNullOrEmpty(entry.Name)) continue;
                                    DirectoryInfo dir = new DirectoryInfo(Updater.CurrentDirectory);
                                    IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
                                    IEnumerable<FileInfo> fileQuery = from file in fileList where file.Name == entry.Name orderby file.Name select file;
                                    foreach (FileInfo info in fileQuery)
                                    {
                                        File.Delete(info.FullName);
                                    }
                                }
                            }
                        }

                        ZipFile.ExtractToDirectory(zipFile, Updater.CurrentDirectory);
                        State = FinishState.State;
                        Dispatcher.Invoke(() =>
                        {
                            Download.Visibility = Visibility.Hidden;
                            Finish.Visibility = Visibility.Visible;
                        });
                    }).Start();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
