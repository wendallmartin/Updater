using System.Windows;

namespace Updater.States
{
    public class WelcomeState : UpdateState
    {
        public static WelcomeState State { get; } = new WelcomeState();
        
        public override void Previous(Installer installer)
        {
            
        }

        public override void Next(Installer installer)
        {
            installer.Directory.Visibility = Visibility.Visible;
            installer.State = DirectoryState.State;;
        }
    }
}