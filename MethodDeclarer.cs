namespace Pocole
{
    public class MethodDeclarer : SemanticBlock
    {
        public string Name { get; private set; }
        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text, SemanticType.MethodDeclarer)) { Log.InitError(); return false; }

            var methodNameBuf = "";
            foreach (var c in text)
            {
                if (c == ' ') continue;
                if (c == '(') break;
                methodNameBuf += c;
            }
            Name = methodNameBuf;

            return true;
        }

        protected override void Run()
        {
            RunningLog();
        }
    }
}