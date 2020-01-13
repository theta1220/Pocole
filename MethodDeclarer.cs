using System.Collections.Generic;
using System.Linq;
using System;

namespace Pocole
{
    [Serializable]
    public class MethodDeclarer : Block
    {
        public string[] ArgNames { get; private set; }
        public System.Type ReturnType { get; private set; }

        public MethodDeclarer(Runnable parent, string source) : base(parent, Util.String.Extract(source, '{', '}'))
        {
            // func hoge(){ ... }
            Name = source.Split('(')[0].Split(' ')[1];
            ArgNames = Util.String.Extract(Util.String.Remove(source, ' '), '(', ')').Split(',');

            var typeName = Util.String.Split(Util.String.Remove(Util.String.Substring(source, '{'), ' '), ':').Last();
            if (typeName == "int") ReturnType = typeof(int);
            else if (typeName == "string") ReturnType = typeof(string);
            else if (typeName == "bool") ReturnType = typeof(bool);
            else ReturnType = typeof(object);
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
                var isRef = true;
                if (Util.String.MatchHead("@", name))
                {
                    name = Util.String.Remove(name, '@');
                    isRef = false;
                }

                var value = new Value(name, arg);

                if (isRef) AddValue(value);
                else AddValue(Util.Object.DeepCopy(value));
            }
            return true;
        }
    }
}