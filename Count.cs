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
            var split = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ':');
            _valueName = split[0];
            _maxFormula = split[1];
            _executedInitSource = false;
        }

        public override void OnEntered()
        {
            if (!_executedInitSource)
            {
                _executedInitSource = true;
                _countValue = new Value(_valueName);
                _countValue.SetValue(0);
                AddValue(_countValue);

                _maxValue = new Value();
                _maxValue.SetValue((int)Util.Calc.Execute(GetParentBlock(), _maxFormula, typeof(int)));
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
            _countValue.SetValue((int)_countValue.Object + 1);
        }
    }
}