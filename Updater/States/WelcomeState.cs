using System.Windows;

namespace Updater.States
{
    public class WelcomeState : UpdateState
    {
        public static WelcomeState State { get; } = new WelcomeState();
        
        public override void Previous(Downloader downloader)
        {
            
        }

        public override void Next(Downloader downloader)
        {
            downloader.Directory.Visibility = Visibility.Visible;
            downloader.State = DirectoryState.State;;
        }
    }
}