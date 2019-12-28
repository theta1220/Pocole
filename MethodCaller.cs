using System;
using System.Collections.Generic;

namespace Pocole
{
    public class MethodCaller : Runnable
    {
        public string Name { get; private set; }
        public string[] Args { get; private set; }

        public new bool Initialize(Block parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            Name = Util.String.SplitOnce(source.Replace(" ", ""), '(')[0];
            Args = Util.String.Remove(Util.String.Extract(source.Replace(" ", ""), '(', ')'), ' ').Split(',');

            Runnables.Add(Parent.FindMethod(Name));
            return true;
        }

        protected override void Activate()
        {
            var method = Parent.FindMethod(Name);
            if (method == null)
            {
                Log.Error("メソッドの呼び出しに失敗しました: {0}", Name);
                return;
            }
            var objs = new List<object>();
            foreach (var arg in Args)
            {
                objs.Add(Util.Calc.Execute(Parent, arg, Value.GetValueType(arg, Parent)));
            }
            method.SetArgs(objs.ToArray());
        }

        protected override void Run()
        {
        }
    }
}