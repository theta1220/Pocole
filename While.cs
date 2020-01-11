namespace Pocole
{
    [System.Serializable]
    public class While : LoopBlock
    {
        private string _conditionSource;

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            _conditionSource = Util.String.Extract(Util.String.Remove(source, ' '), '(', ')');

            return true;
        }

        public override void OnEntered()
        {
            var isContinuous = (bool)Util.Calc.Execute(this, _conditionSource, typeof(bool));
            if (!isContinuous)
            {
                IsContinuous = false;
                SkipExecute();
            }
        }
    }
}