using System.Windows;
using System.Windows.Forms.VisualStyles;

namespace Updater
{
    public abstract class UpdateState
    {
        public static Installer Installer;

        public static void HideAll()
        {
            if (Installer == null) return;

            Installer.Welcome.Visibility = Visibility.Hidden;
            Installer.Directory.Visibility = Visibility.Hidden;
            Installer.Download.Visibility = Visibility.Hidden;
            Installer.Finish.Visibility = Visibility.Hidden;
        }
        
        public abstract UpdateState Previous();
        
        public abstract UpdateState Next();
    }
}