using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Updater.States;
using MessageBox = System.Windows.MessageBox;

namespace Updater
{
    /// <summary>
    /// Interaction logic for Installer.xaml
    /// </summary>
    public partial class Downloader
    {
        public readonly UpdateEngine Updater;

        public UpdateState State;
        
        public Downloader(UpdateEngine updater)
        {
            InitializeComponent();

            Updater = updater;
            HideAll();

            switch (updater.Type)
            {
                case UpdateEngine.InstanceType.Install:
                    Welcome.Visibility = Visibility.Visible;
                    State = WelcomeState.State;
                    
                    Updater.DownloadDirectory =
                        DirectoryTextBlock.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    
                    if (Updater.GetUpdateVersions().Count > 0)
                    {
                        VersionLabel.Content = Updater.GetUpdateVersions().OrderBy(v => v.Version).Last().Version.ToString();
                        WelcomeNext.IsEnabled = true;
                    }

                    break;
                case UpdateEngine.InstanceType.Update:
                    Updates.Visibility = Visibility.Visible;
                    State = States.Updates.State;
                    
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

            DirectoryTextBlock.Text = Updater.DownloadDirectory;
        }

        private void HideAll()
        {
            Welcome.Visibility = Visibility.Hidden;
            Updates.Visibility = Visibility.Hidden;
            Directory.Visibility = Visibility.Hidden;
            Download.Visibility = Visibility.Hidden;
            Finish.Visibility = Visibility.Hidden;
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

        private void SetDownloadProgress(double receive, double total)
        {
            if (Math.Abs(receive) < .001 || Math.Abs(total) < .001) return;
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = receive / total * 100;
                ProgressText.Content = $"{receive}mb of {total}mb";
            });
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                Description = @"Install location", SelectedPath = @"C:\\Program Files\\"
            };
            DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                Updater.DownloadDirectory = dialog.SelectedPath;
            }

            DirectoryTextBlock.Text = Updater.DownloadDirectory;
        }

        public void DownloadVersion(string version)
        {
            try
            {
                if (!string.IsNullOrEmpty(version))
                {
                    new Thread(() =>
                    {
                        Updater.UpdateChangedEvent += SetDownloadProgress;
                        Updater.Update(new Version(version));
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
