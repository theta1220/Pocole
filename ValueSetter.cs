namespace Pocole
{
    public class ValueSetter : Runnable
    {
        public Value Value { get; private set; } = new Value();
        public new bool Initialize(Block parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            try
            {
                var name = source.Split(' ')[0];

                // 宣言
                if (name == "var")
                {
                    name = source.Split(' ')[1].Replace(" ", "").Split('=')[0];
                }
                // 代入
                else
                {
                    name = source.Replace(" ", "").Split('=')[0];
                }
                if (!Value.Initialize(name)) { Log.InitError(); return false; }
            }
            catch
            {
                Log.ParseError();
                return false;
            }
            return true;
        }

        protected override void Run()
        {
            RunningLog();
        }
    }
}