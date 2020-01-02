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
            Args = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ',');

            var method = Parent.FindMethod(Name);
            if (method == null)
            {
                Log.Error("対象のメソッドが見つかりませんでした: {0}", Name);
                return false;
            }
            Runnables.Add(method);
            return true;
        }

        public override void OnEntered()
        {
            var method = Parent.FindMethod(Name);
            if (method == null)
            {
                Log.Error("メソッドの呼び出しに失敗しました: {0}", Name);
                return;
            }
            var objs = new List<object>();
            // Log.Debug("Name:{0} / Args:{1},count{2}", Name, Util.String.ArrayToString(Args), Args.Length);
            foreach (var arg in Args)
            {
                objs.Add(Util.Calc.Execute(Parent, arg, Value.GetValueType(arg, Parent)));
            }
            if (!method.SetArgs(objs.ToArray()))
            {
                Log.Error("SetArgsに失敗");
                return;
            }
        }
    }
}