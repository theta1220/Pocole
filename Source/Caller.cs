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
        public Function Method { get; private set; }

        public Caller(Runnable parent, string source) : base(parent, source)
        {
            Name = source.PoRemove(' ').PoCut('(');
            Args = source.PoRemove(' ').PoExtract('(', ')').PoSplit(',');
        }

        public Caller(Caller other) : base(other)
        {
            Name = other.Name;
            Args = other.Args;
            Method = other.Method;
        }

        public override Runnable Clone() { return new Caller(this); }

        public override void OnEntered()
        {
            Method = GetParentBlock().FindMethod(Name);

            if (Method == null)
            {
                Log.Error("メソッドの呼び出しに失敗しました: {0}", Name);
                return;
            }
            Runnables.Add(Method);

            // 呼び出しもとを特定してセット
            {
                var valueName = Name.PoSplitOnceTail('.')[0];
                if (Name.Contains("."))
                {
                    var value = GetParentBlock().FindValue(valueName);
                    if (value != null)
                    {
                        Method.Caller = value;
                    }
                    else
                    {
                        var classDef = GetParentBlock().FindClass(valueName);
                        if (classDef == null)
                        {
                            Log.Error("呼び出し元を特定できませんでした {0}", valueName);
                        }
                        Method.Caller = new Value("", classDef);
                    }
                }
                else
                {
                    var value = GetParentBlock().FindValue("this");
                    if (value == null)
                    {
                        Log.Error("呼び出し元を特定できませんでした/{0}", Name);
                    }
                    Method.Caller = value;
                }
            }

            // 引数をセット
            {
                var objs = new List<object>();
                foreach (var arg in Args)
                {
                    objs.Add(Util.Calc.Execute(GetParentBlock(), arg, Value.GetValueType(arg, GetParentBlock())).Object);
                }
                Method.SetArgs(objs.ToArray());
            }
        }
    }
}