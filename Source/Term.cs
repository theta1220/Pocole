using System;
using System.Linq;
using Sumi.Util;

namespace Sumi
{
    public class Term : Runnable
    {
        public Term(Runnable parent, string source) : base(parent, source)
        {
        }

        public Term(Term other) : base(other)
        {
        }

        public override Runnable Clone() { return new Term(this); }

        public override void OnEntered()
        {
            var methodName = ExtractMethodName(Source);

            if (methodName == "system_call") Runnables.Add(new SystemCaller(this, Source));
            else if (IsSetter(Source)) Runnables.Add(new ValueSetter(this, Source));
            else Runnables.Add(new Caller(this, Source));
        }

        public override void OnLeaved()
        {
            Runnables.Clear();
        }

        //! ... Hoge(args)... のような文字列から メソッド名を取り出してくれる
        public static string ExtractMethodName(string source)
        {
            var buf = source.PoCut('(').PoSplit(' ');
            return buf.Last();
        }

        public bool IsSetter(string source)
        {
            var name = source.PoRemove(' ').PoCut('=');
            return source.PoMatchHead("var ") || GetParentBlock().FindValue(name) != null;
        }
    }
}