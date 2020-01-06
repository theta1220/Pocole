using System;
using System.Collections.Generic;

namespace Pocole
{
    public class MethodCaller : Runnable
    {
        public string Name { get; private set; }
        public string[] Args { get; private set; }

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            Name = Util.String.SplitOnce(source.Replace(" ", ""), '(')[0];
            Args = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ',');

            return true;
        }

        public override void OnEntered()
        {
            var method = GetParentBlock().FindMethod(Name);
            if (method == null)
            {
                Log.Error("メソッドの呼び出しに失敗しました: {0}", Name);
                return;
            }
            Runnables.Add(method);

            var objs = new List<object>();
            foreach (var arg in Args)
            {
                objs.Add(Util.Calc.Execute(GetParentBlock(), arg, Value.GetValueType(arg, GetParentBlock())));
            }
            if (!method.SetArgs(objs.ToArray()))
            {
                Log.Error("SetArgsに失敗");
                return;
            }
        }
    }
}