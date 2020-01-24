using Sumi.Util;
using System;

namespace Sumi
{
    public class While : LoopBlock
    {
        private string _conditionSource;

        public While(Runnable parent, string source) : base(parent, source)
        {
            _conditionSource = source.PoRemove(' ').PoExtract('(', ')');
        }

        public While(While other) : base(other)
        {
            _conditionSource = other._conditionSource;
        }

        public override Runnable Clone() { return new While(this); }

        public override void OnEntered()
        {
            var isContinuous = (bool)Util.Calc.Execute(this, _conditionSource, typeof(bool)).Object;
            if (!isContinuous)
            {
                IsContinuous = false;
                SkipExecute();
            }
        }
    }
}