namespace Pocole
{
    public class Return : Runnable
    {
        public string Formula { get; private set; }
        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            Formula = Util.String.SplitOnce(source, ' ')[1];
            return true;
        }

        protected override void Run()
        {
            GetParentBlock().ReturnedValue = Util.Calc.Execute(GetParentBlock(), Formula, GetParentMethod().ReturnType);
            GetParentBlock().SkipExecute();
        }
    }
}