using Sumi.Util;
using System;

namespace Sumi
{
    public class Return : Runnable
    {
        public string Formula { get; private set; }
        public Return(Runnable parent, string source) : base(parent, source)
        {
            var split = source.PoSplitOnce(' ');
            if (split.Length < 2)
            {
                // NOTE: "return;" としか書かれていない場合は nullを返すことにしておく
                Formula = "null";
                return;
            }
            Formula = split[1];
        }

        public Return(Return other) : base(other)
        {
            Formula = other.Formula;
        }

        public override Runnable Clone() { return new Return(this); }

        protected override void Run()
        {
            var res = Util.Calc.Execute(GetParentBlock(), Formula, GetParentMethod().ReturnType);
            if (res != null)
            {
                GetParentMethod().ReturnedValue = res.Object;
            }
            GetParentMethod().SkipExecute();
        }
    }
}