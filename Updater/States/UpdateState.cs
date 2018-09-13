namespace Downloader.States
{
    public abstract class UpdateState
    {
        public abstract void Previous(Downloader downloader);
        
        public abstract void Next(Downloader downloader);
    }
}