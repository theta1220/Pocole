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

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }
            var split = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ':');
            _valueName = split[0];
            _maxFormula = split[1];
            _executedInitSource = false;
            return true;
        }

        public override void OnEntered()
        {
            if (!_executedInitSource)
            {
                _executedInitSource = true;
                _countValue = new Value();
                if (!_countValue.Initialize(_valueName)) { Log.InitError(); return; }
                _countValue.SetValue(0);
                AddValue(_countValue);

                _maxValue = new Value();
                if (!_maxValue.Initialize()) { Log.InitError(); return; }
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