using System.IO;

namespace Updater.States
{
    public class FinishState : UpdateState
    {
        public static FinishState State { get; } = new FinishState();
        
        public override void Previous(Downloader downloader)
        {
            
        }

        public override void Next(Downloader downloader)
        {
            Program.StartInstall(Path.Combine(downloader.Updater.DownloadDirectory, downloader.Updater.UpdateVersion + ".exe"));
            downloader.Close();
        }
    }
}