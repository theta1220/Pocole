namespace Pocole
{
    public class For : Block
    {
        public string InitSource { get; private set; }
        public string ConditionSource { get; private set; }
        public string RoopSource { get; private set; }

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, Util.String.Extract(source, '{', '}'))) { Log.InitError(); return false; }

            // for(var i=0; i<10; i++) {...} みたいなやつがくる
            var formulas = Util.String.Split(Util.String.Extract(source, '(', ')'), ';');
            InitSource = formulas[0];
            ConditionSource = formulas[1];
            RoopSource = formulas[2];

            return true;
        }

        public override void OnEntered()
        {
            var setter = new ValueSetter();
            if (!setter.Initialize(this, InitSource)) { Log.InitError(); return; }
            setter.ForceExecute();
        }

        public override void OnLeaved()
        {
            var setter = new ValueSetter();
            if (!setter.Initialize(this, RoopSource)) { Log.InitError(); return; }
            setter.ForceExecute();
        }

        public override bool CheckContinue()
        {
            var isContinuous = (bool)Util.Calc.Execute(this, ConditionSource, typeof(bool));
            if (isContinuous)
            {
                return true;
            }
            else
            {
                base.OnLeaved();
                return false;
            }
        }
    }
}