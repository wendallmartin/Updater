using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        
        private void UpdateButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(VersionBox.Text) && _versions.Any(v => v.ToString() == VersionBox.Text))
            {
                Program.StopParentProcess();// kills the process given at startup threw command line params
                FTPEngine.Update(new Version(VersionBox.Text));
                Program.StartParentProcess();
                Close();
                return;
            }
        }
    }
}