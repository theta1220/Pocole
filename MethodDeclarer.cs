using System.Collections.Generic;
using System.Linq;

namespace Pocole
{
    public class MethodDeclarer : SemanticBlock
    {
        public string Name { get; private set; }
        public string[] ArgNames { get; private set; }

        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text, SemanticType.MethodDeclarer)) { Log.InitError(); return false; }

            // func hoge(){ ... }
            try
            {
                var header = text.Split(' ')[0];
                if (header != "func")
                {
                    Log.Error("ParseError: 関数の宣言じゃないものが渡ってきました : {0}", text);
                    return false;
                }
                Name = text.Split('(')[0].Split(' ')[1];
                ArgNames = Util.String.Extract(text.Replace(" ", ""), '(', ')').Split(',');
            }
            catch
            {
                Log.ParseError();
                return false;
            }

            return true;
        }

        public bool SetArgs(object[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var name = "";
                if (i >= ArgNames.Length)
                {
                    name = ArgNames[ArgNames.Length - 1];
                }
                else
                {
                    name = ArgNames[i];
                }
                var value = new Value();
                if (!value.Initialize(name)) { Log.InitError(); return false; }
                value.SetValue(arg);
                Block.AddValue(value);
            }
            return true;
        }

        protected override void Run()
        {
        }
    }
}