using System.Windows;

namespace Updater.States
{
    public class Updates : UpdateState
    {
        public static UpdateState State { get; } = new Updates();
        
        public override void Previous(Downloader downloader)
        {
            
        }

        public override void Next(Downloader downloader)
        {
            downloader.Download.Visibility = Visibility.Visible;
            downloader.DownloadVersion(downloader.UpdateComboBox.Text);
        }
    }
}