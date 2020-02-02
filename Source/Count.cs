using Sumi.Util;
using System;

namespace Sumi
{
    public class Count : LoopBlock
    {
        private string valueName;
        private string maxFormula;
        private Value countValue;
        private Value maxValue;
        private bool executedInitSource = false;

        public Count(Runnable parent, string source) : base(parent, source)
        {
            var split = source.Remove(' ').PoExtract('(', ')').Split(':');
            valueName = split[0];
            maxFormula = split[1];
            executedInitSource = false;
        }

        public Count(Count other) : base(other)
        {
            valueName = other.valueName;
            maxFormula = other.maxFormula;
            countValue = new Value(other.countValue);
            maxValue = new Value(other.maxValue);
            executedInitSource = other.executedInitSource;
        }

        public override Runnable Clone() { return new Count(this); }

        public override void OnEntered()
        {
            if (!executedInitSource)
            {
                executedInitSource = true;
                countValue = new Value(valueName, 0);
                AddValue(countValue);

                maxValue = Util.Calc.Execute(GetParentBlock(), maxFormula, typeof(int));
            }

            var isContinuous = (int)countValue.Object < (int)maxValue.Object;
            if (!isContinuous)
            {
                IsContinuous = false;
                SkipExecute();
            }
        }

        public override void OnLeaved()
        {
            countValue.Object = (int)countValue.Object + 1;
        }

        public override void OnReset()
        {
            base.OnReset();
            executedInitSource = false;
        }
    }
}