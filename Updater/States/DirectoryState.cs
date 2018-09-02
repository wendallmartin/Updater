using System.Windows;

namespace Updater
{
    public class DirectoryState : UpdateState
    {
        private static DirectoryState _state;
        
        public static DirectoryState State
        {
            get{
                if(_state == null)
                    _state = new DirectoryState();
                return _state;
            }
        }
        
        public override UpdateState Previous()
        {
            if (Installer == null) return State;
            HideAll();
            Installer.Welcome.Visibility = Visibility.Visible;
            return WelcomeState.State;
        }

        public override UpdateState Next()
        {
            if (Installer == null) return State;;
            HideAll();
            Installer.Download.Visibility = Visibility.Visible;
            return DownloadState.State;
        }
    }
}