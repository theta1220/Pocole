using System;
using System.Collections.Generic;
using System.Linq;
using Sumi.Util;

namespace Sumi
{
    public class Caller : Runnable
    {
        public string Name { get; private set; }
        public string[] Args { get; private set; }
        public Function Function { get; private set; }

        public Caller(Runnable parent, string source) : base(parent, source)
        {
            Name = source.PoRemove(' ').PoCut('(');
            Args = source.PoRemove(' ').PoExtract('(', ')').PoSplit(',');
        }

        public Caller(Caller other) : base(other)
        {
            Name = other.Name;
            Args = other.Args;
            Function = other.Function;
        }

        public override Runnable Clone() { return new Caller(this); }

        public override void OnEntered()
        {
            Function = GetParentBlock().FindFunction(Name);

            if (Function == null)
            {
                Log.Error("メソッドの呼び出しに失敗しました: {0}->{1}", GetParentClass().FullName, Name);
                return;
            }
            Runnables.Add(Function);
            SetCaller();
            SetArgs();
        }

        private void SetCaller()
        {
            if (Name.Contains("."))
            {
                var valueName = Name.PoSplitOnceTail('.')[0];
                Function.Caller =
                    GetParentBlock().FindValue(valueName) ??
                    new Value("", GetParentClass().FindClass(valueName));
            }
            else
            {
                Function.Caller = GetParentBlock().FindValue("this");
            }
            Log.Assert(Function.Caller != null, "呼び出し元を特定できませんでした {0}", Name);
        }

        private void SetArgs()
        {
            var objs = new List<object>();
            foreach (var arg in Args)
            {
                objs.Add(Util.Calc.Execute(GetParentBlock(), arg, Value.GetValueType(arg, GetParentBlock())).Object);
            }
            Function.SetArgs(objs.ToArray());
        }
    }
}