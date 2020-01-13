using Pocole.Util;

namespace Pocole
{
    [System.Serializable]
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