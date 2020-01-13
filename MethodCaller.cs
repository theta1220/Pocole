using System;
using System.Collections.Generic;
using System.Linq;
using Pocole.Util;

namespace Pocole
{
    public class MethodCaller : Runnable
    {
        public string Name { get; private set; }
        public string[] Args { get; private set; }
        public MethodDeclarer Method { get { return GetParentBlock().FindMethod(Name); } }

        public MethodCaller(Runnable parent, string source) : base(parent, source)
        {
            Name = source.PoRemove(' ').PoCut('(');
            Args = source.PoRemove(' ').PoExtract('(', ')').PoSplit(',');
        }

        public override void OnEntered()
        {
            if (Method == null)
            {
                Log.Error("メソッドの呼び出しに失敗しました: {0}", Name);
                return;
            }
            Runnables.Add(Method);

            // 呼び出しもとを特定してセット
            {
                var caller = GetParentBlock().FindValue(GetCallerName(Name));
                if (caller != null)
                {
                    Method.Caller = caller;
                }
            }

            // 引数をセット
            {
                var objs = new List<object>();
                foreach (var arg in Args)
                {
                    objs.Add(Util.Calc.Execute(GetParentBlock(), arg, Value.GetValueType(arg, GetParentBlock())).Object);
                }
                if (!Method.SetArgs(objs.ToArray()))
                {
                    Log.Error("SetArgsに失敗");
                    return;
                }
            }
        }

        public static string GetCallerName(string name)
        {
            var names = name.Split('.');
            var count = 0;
            var res = "";
            foreach (var _name in names)
            {
                if (count + 1 < names.Length)
                {
                    res += _name;
                    if (count + 2 < names.Length)
                    {
                        res += ".";
                    }
                }
                count++;
            }
            return res;
        }
    }
}