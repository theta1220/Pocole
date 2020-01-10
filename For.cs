namespace Pocole
{
    [System.Serializable]
    public class For : LoopBlock
    {
        public string InitSource { get; private set; }
        public string ConditionSource { get; private set; }
        public string LoopSource { get; private set; }

        private bool executedInitSource = false;

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            // for(var i=0; i<10; i++) {...} みたいなやつがくる
            var formulas = Util.String.Split(Util.String.Extract(source, '(', ')'), ';');
            InitSource = formulas[0];
            ConditionSource = formulas[1];
            LoopSource = formulas[2];
            executedInitSource = false;

            return true;
        }

        public override void OnEntered()
        {
            // 初回だけ
            if (!executedInitSource)
            {
                executedInitSource = true;

                var setter = new ValueSetter();
                if (!setter.Initialize(this, InitSource)) { Log.InitError(); return; }
                setter.ForceExecute();
            }

            var isContinuous = (bool)Util.Calc.Execute(this, ConditionSource, typeof(bool));
            if (!isContinuous)
            {
                IsContinuous = false;
                SkipExecute();
            }
        }

        public override void OnLeaved()
        {
            var setter = new ValueSetter();
            if (!setter.Initialize(this, LoopSource)) { Log.InitError(); return; }
            setter.ForceExecute();

            base.OnLeaved();
        }
    }
}