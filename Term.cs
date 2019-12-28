namespace Pocole
{
    public class Term : Runnable
    {
        public Value Result { get; private set; }

        public bool InitializeTerm(Block parent, string text)
        {
            if (!Initialize(parent, text)) { Log.InitError(); return false; }

            return true;
        }

        protected override void Run()
        {
        }
    }
}