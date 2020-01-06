using System.Collections.Generic;
using System.Linq;

namespace Pocole
{
    public class MethodDeclarer : SemanticBlock
    {
        public string Name { get; private set; }
        public string[] ArgNames { get; private set; }

        public new bool Initialize(Block parent, string source)
        {
            if (!base.Initialize(parent, source, SemanticType.MethodDeclarer)) { Log.InitError(); return false; }

            // func hoge(){ ... }
            var header = source.Split(' ')[0];
            if (header != "func")
            {
                Log.Error("ParseError: 関数の宣言じゃないものが渡ってきました : {0}", source);
                return false;
            }
            Name = source.Split('(')[0].Split(' ')[1];
            ArgNames = Util.String.Extract(Util.String.Remove(source, ' '), '(', ')').Split(',');

            var block = new Block();
            if (!block.Initialize(parent, Util.String.Extract(source, '{', '}'))) { Log.InitError(); return false; }
            AddBlock(block);

            return true;
        }

        public bool SetArgs(object[] args)
        {
            Log.Info(Util.String.ArrayToString(args));
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
                var findValue = Parent.FindValue(name);
                if (findValue == null)
                {
                    var value = new Value();
                    if (!value.Initialize(name)) { Log.InitError(); return false; }
                    value.SetValue(arg);
                    Block.AddValue(value);
                }
            }
            return true;
        }

        protected override void Run()
        {
        }
    }
}