namespace Downloader.States
{
    public class DownloadState : UpdateState
    {
        public static DownloadState State { get; } = new DownloadState();
        
        public override void Previous(Downloader downloader)
        {
           
        }

        public override void Next(Downloader downloader)
        {
           
        }
    }
}