using System;
using System.Linq;
using System.Windows;
using FTPUpdater;

namespace Updater
{
    /// <summary>
    /// Interaction logic for Installer.xaml
    /// </summary>
    public partial class Installer : Window
    {
        private UpdateEngine _updater;

        private UpdateState _state;

        public Installer()
        {
            InitializeComponent();

            _updater = new HttpEngine();

            _updater.DownloadDirectory = "C:\\Program Files (x86)\\TheTimeApp";
            
            _state = WelcomeState.State;

            UpdateState.Installer = this;
            
            Welcome.Visibility = Visibility.Visible;
            Directory.Visibility = Visibility.Hidden;
            Download.Visibility = Visibility.Hidden;
            Finish.Visibility = Visibility.Hidden;

            DirectoryTextBlock.Text = _updater.DownloadDirectory;

            if (_updater.GetUpdateVersions().Count > 0)
            {
                UpdateVersionLabel.Content = _updater.GetUpdateVersions().OrderBy(v => v.Version).Last().ToString();    
            }
        }

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            _state = _state.Next();
        }

        private void Btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            _state = _state.Previous();
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = @"Install location";
            dialog.RootFolder = Environment.SpecialFolder.ProgramFiles;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                _updater.DownloadDirectory = dialog.SelectedPath;
            }

            DirectoryTextBlock.Text = _updater.DownloadDirectory;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
