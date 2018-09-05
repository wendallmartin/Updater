namespace Updater.States
{
    public class FinishState : UpdateState
    {
        public static FinishState State { get; } = new FinishState();
        
        public override void Previous(Installer installer)
        {
            
        }

        public override void Next(Installer installer)
        {
            Program.StartParentProcess(installer.Updater.CurrentDirectory);
            installer.Close();
        }
    }
}