namespace Pocole
{
    public class Class : SemanticBlock
    {
        public string Name { get; private set; }
        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source, SemanticType.Class)) { Log.InitError(); return false; }

            // class hoge(){ ... }
            try
            {
                var header = source.Split(' ')[0];
                if (header != "class")
                {
                    Log.Error("クラスの宣言じゃないものが渡ってきました : {0}", source);
                    return false;
                }
                Name = source.Split('(')[0].Split(' ')[1];
            }
            catch
            {
                Log.ParseError();
                return false;
            }
            return true;
        }
    }
}