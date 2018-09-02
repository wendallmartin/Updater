using System.Windows;
using System.Windows.Forms;

namespace Updater
{
    public class DownloadState : UpdateState
    {
        private static DownloadState _state;
        
        public static DownloadState State
        {
            get{
                if(_state == null)
                    _state = new DownloadState();
                return _state;
            }
        }
        
        public override UpdateState Previous()
        {
            if (Installer == null) return State;

            HideAll();
            Installer.Directory.Visibility = Visibility.Visible;
            return DirectoryState.State;
        }

        public override UpdateState Next()
        {
            if (Installer == null) return State;
            
            HideAll();
            Installer.Finish.Visibility = Visibility.Visible;
            return FinishState.State;
        }
    }
}