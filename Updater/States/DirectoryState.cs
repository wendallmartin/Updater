using System.Windows;

namespace Updater.States
{
    public class DirectoryState : UpdateState
    {
        public static DirectoryState State { get; } = new DirectoryState();
        
        public override void Previous(Installer installer)
        {
            installer.Welcome.Visibility = Visibility.Visible;
            installer.State = WelcomeState.State;
        }

        public override void Next(Installer installer)
        {
            installer.Download.Visibility = Visibility.Visible;
            installer.State = DownloadState.State;
            installer.DownloadVersion(installer.VersionLabel.Content.ToString());
        }
    }
}