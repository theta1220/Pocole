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
            var remove = Util.String.Remove(source, ' ');
            Log.Info(remove);
            var argStr = Util.String.Extract(remove, '(', ')');
            Log.Info(argStr);
            Args = Util.String.Split(argStr, ',');

            Log.Info(Util.String.ArrayToString(Args));
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
            Runnables.Add(method);

            var objs = new List<object>();
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