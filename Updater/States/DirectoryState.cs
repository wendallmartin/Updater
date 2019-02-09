using System.Windows;

namespace Updater.States
{
    public class DirectoryState : UpdateState
    {
        public static DirectoryState State { get; } = new DirectoryState();
        
        public override void Previous(Downloader downloader)
        {
            downloader.Welcome.Visibility = Visibility.Visible;
            downloader.State = WelcomeState.State;
        }

        public override void Next(Downloader downloader)
        {
            downloader.Download.Visibility = Visibility.Visible;
            downloader.State = DownloadState.State;
            downloader.DownloadVersion(downloader.VersionLabel.Content.ToString());
        }
    }
}