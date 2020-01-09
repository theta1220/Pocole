using System.Collections.Generic;
using System.Linq;

namespace Pocole
{
    public class MethodDeclarer : Block
    {
        public string Name { get; private set; }
        public string[] ArgNames { get; private set; }
        public System.Type ReturnType { get; private set; }

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, Util.String.Extract(source, '{', '}'))) { Log.InitError(); return false; }

            // func hoge(){ ... }
            Name = source.Split('(')[0].Split(' ')[1];
            ArgNames = Util.String.Extract(Util.String.Remove(source, ' '), '(', ')').Split(',');
            var typeName = Util.String.Split(Util.String.Remove(Util.String.Substring(source, '{'), ' '), ':').Last();
            if (typeName == "int") ReturnType = typeof(int);
            else if (typeName == "string") ReturnType = typeof(string);
            else if (typeName == "bool") ReturnType = typeof(bool);
            else ReturnType = typeof(void);

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
                var findValue = GetParentBlock().FindValue(name);
                if (findValue == null)
                {
                    var value = new Value();
                    if (!value.Initialize(name)) { Log.InitError(); return false; }
                    value.SetValue(arg);
                    AddValue(value);
                }
            }
            return true;
        }
    }
}