namespace Pocole
{
    public class MethodCaller : Runnable
    {
        public new bool Initialize(Block parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }
            return true;
        }

        protected override void Run()
        {
            RunningLog();
        }


    }
}