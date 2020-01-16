using Pocole.Util;
using System;

namespace Pocole
{
    public class Count : LoopBlock
    {
        private string _valueName;
        private string _maxFormula;
        private Value _countValue;
        private Value _maxValue;
        private bool _executedInitSource = false;

        public Count(Runnable parent, string source) : base(parent, source)
        {
            var split = source.Remove(' ').PoExtract('(', ')').Split(':');
            _valueName = split[0];
            _maxFormula = split[1];
            _executedInitSource = false;
        }

        public Count(Count other) : base(other)
        {
            _valueName = other._valueName;
            _maxFormula = other._maxFormula;
            _countValue = new Value(other._countValue);
            _maxValue = new Value(other._maxValue);
            _executedInitSource = other._executedInitSource;
        }

        public override object Clone() { return new Count(this); }

        public override void OnEntered()
        {
            if (!_executedInitSource)
            {
                _executedInitSource = true;
                _countValue = new Value(_valueName, 0);
                AddValue(_countValue);

                _maxValue = Util.Calc.Execute(GetParentBlock(), _maxFormula, typeof(int));
            }

            var isContinuous = (int)_countValue.Object < (int)_maxValue.Object;
            if (!isContinuous)
            {
                IsContinuous = false;
                SkipExecute();
            }
        }

        public override void OnLeaved()
        {
            _countValue.Object = (int)_countValue.Object + 1;
        }
    }
}