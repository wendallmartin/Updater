namespace Updater.States
{
    public abstract class UpdateState
    {
        public abstract void Previous(Installer installer);
        
        public abstract void Next(Installer installer);
    }
}