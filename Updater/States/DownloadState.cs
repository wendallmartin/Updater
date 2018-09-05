namespace Updater.States
{
    public class DownloadState : UpdateState
    {
        public static DownloadState State { get; } = new DownloadState();
        
        public override void Previous(Installer installer)
        {
           
        }

        public override void Next(Installer installer)
        {
           
        }
    }
}