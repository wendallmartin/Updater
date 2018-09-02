using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms.VisualStyles;

namespace Updater
{
    public class FinishState : UpdateState
    {
        private static FinishState _state;
        
        public static FinishState State
        {
            get{
                if(_state == null)
                    _state = new FinishState();
                return _state;
            }
        }
        
        public override UpdateState Previous()
        {
            if (Installer == null) return State;
            HideAll();
            Installer.Download.Visibility = Visibility.Visible;
            return DownloadState.State;
        }

        public override UpdateState Next()
        {
            Installer.Close();

            return State;
        }
    }
}