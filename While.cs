using Pocole.Util;

namespace Pocole
{
    [System.Serializable]
    public class While : LoopBlock
    {
        private string _conditionSource;

        public While(Runnable parent, string source) : base(parent, source)
        {
            _conditionSource = source.PoRemove(' ').PoExtract('(', ')');
        }

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