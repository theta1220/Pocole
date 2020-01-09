using System;
using System.Collections.Generic;

namespace Pocole
{
    public class MethodCaller : Runnable
    {
        public string Name { get; private set; }
        public string[] Args { get; private set; }
        public MethodDeclarer Method { get { return GetParentBlock().FindMethod(Name); } }

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            Name = Util.String.Substring(source.Replace(" ", ""), '(');
            Args = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ',');

            return true;
        }

        public override void OnEntered()
        {
            if (Method == null)
            {
                Log.Error("メソッドの呼び出しに失敗しました: {0}", Name);
                return;
            }
            Runnables.Add(Method);

            var objs = new List<object>();
            foreach (var arg in Args)
            {
                objs.Add(Util.Calc.Execute(GetParentBlock(), arg, Value.GetValueType(arg, GetParentBlock())));
            }
            if (!Method.SetArgs(objs.ToArray()))
            {
                Log.Error("SetArgsに失敗");
                return;
            }
        }
    }
}