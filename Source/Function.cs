using System.Collections.Generic;
using System.Linq;
using System;
using Sumi.Util;

namespace Sumi
{
    public class Function : Block
    {
        public string[] ArgNames { get; private set; }
        public System.Type ReturnType { get; private set; }
        public Value Caller { get; set; }

        private static string ReadName(string source)
        {
            // func hoge(){ ... }
            var names = source.PoCut('(').Split(' ');
            return names[1];
        }

        public Function(Runnable parent, string source) : base(parent, source.PoExtract('{', '}'), ReadName(source))
        {
            // func hoge(){ ... }
            var names = source.PoCut('(').Split(' ');
            ArgNames = source.PoRemove(' ').PoExtract('(', ')').Split(',');

            var typeName = source.PoCut('{').PoRemove(' ').PoSplit(':').Last();
            if (typeName == "int") ReturnType = typeof(int);
            else if (typeName == "string") ReturnType = typeof(string);
            else if (typeName == "bool") ReturnType = typeof(bool);
            else ReturnType = typeof(Class);

            if (names[0] == "test")
            {
                ReturnType = typeof(bool);
            }
        }

        public Function(Function other) : base(other)
        {
            ArgNames = other.ArgNames.ToArray();
            ReturnType = other.ReturnType;
            Caller = other.Caller;
        }

        public override Runnable Clone() { return new Function(this); }

        public override void OnLeaved()
        {
            base.OnLeaved();
            Caller = null;
        }

        public void SetArgs(object[] args)
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
                if (name.PoMatchHead("@"))
                {
                    name = name.PoRemove('@');
                    isRef = false;
                }

                var value = new Value(name, arg);

                if (isRef) AddValue(value);
                else AddValue(new Value(value));
            }
        }
    }
}