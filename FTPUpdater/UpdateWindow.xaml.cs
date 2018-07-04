using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<Version> _versions;
        
        public UpdateWindow()
        {
            InitializeComponent();

            _versions = new List<Version>();
            
            try
            {
                _versions = FTPEngine.GetUpdateVersions();
            }
            catch (Exception e)
            {
                MessageBox.Show("There is a problem on our end. Try again later.");
                Close();
                return;
            }

            if (_versions.Count > 0)
            {
                foreach (Version version in _versions)
                {
                    if (version <= Program.CurrentVersion)
                        continue;
                
                    Label versionLabel = new Label() {Content = version};
                    versionLabel.MouseDown += OnVersionSelected;
                    VersionBox.Items.Add(versionLabel);
                }

                if (VersionBox.Items.Count == 0)
                {
                    MessageBox.Show("You are up to date!");
                    Close();
                    return;
                }                
            }
            else
            {
                MessageBox.Show("No updates on server!");
                Close();
                return;
            }
        }

        private void OnVersionSelected(object sender, MouseButtonEventArgs e)
        {
            VersionBox.Text = (string) ((Label) sender).Content;
        }
        
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            lock (_updateLock)
            {
                string version = VersionBox.Text;
                if (!string.IsNullOrEmpty(version) && _versions.Any(v => v.ToString() == version))
                {
                    UpdateIcon.Visibility = Visibility.Visible;
                    new Thread(() =>
                    {
                        Program.StopParentProcess(); // kills the process given at startup threw command line params
                        FTPEngine.Update(new Version(version));
                        UpdateIcon.Dispatcher.Invoke(() => { UpdateIcon.Visibility = Visibility.Hidden;});
                        Program.StartParentProcess();
                        Dispatcher.Invoke(Close);    
                    }).Start();
                }    
            }
        }
    }
}