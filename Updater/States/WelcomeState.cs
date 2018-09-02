using System.Windows;

namespace Updater
{
    public class WelcomeState : UpdateState
    {
        private static WelcomeState _state;
        
        public static WelcomeState State
        {
            get{
                if(_state == null)
                    _state = new WelcomeState();
                return _state;
            }
        }
        
        public override UpdateState Previous()
        {
            return State;
        }

        public override UpdateState Next()
        {
            if (Installer == null) return State;
            HideAll();
            Installer.Directory.Visibility = Visibility.Visible;
            return DirectoryState.State;
        }
    }
}