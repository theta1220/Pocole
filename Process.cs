namespace Pocole
{
    public enum ProcessType
    {
        If,
        Else,
    }

    public class Process : SemanticBlock
    {

        public string Name { get; private set; }
        public ProcessType ProcessType { get; private set; }

        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text, SemanticType.Process)) { Log.InitError(); return false; }

            return true;
        }

        protected override void Run()
        {
            RunningLog();
        }
    }
}