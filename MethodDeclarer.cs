namespace Pocole
{
    public class MethodDeclarer : SemanticBlock
    {
        public string Name { get; private set; }
        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text, SemanticType.MethodDeclarer)) { Log.InitError(); return false; }

            // func hoge(){ ... }
            try
            {
                Name = text.Split('(')[0].Split(' ')[1];
                var header = text.Split(' ')[0];
                if(header != "func")
                {
                    Log.Error("ParseError: 関数の宣言じゃないものが渡ってきました : {0}", text);
                    return false;
                }
            }
            catch
            {
                Log.ParseError();
            }

            return true;
        }

        protected override void Run()
        {
            RunningLog();
        }
    }
}