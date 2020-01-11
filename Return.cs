namespace Pocole
{
    [System.Serializable]
    public class Return : Runnable
    {
        public string Formula { get; private set; }
        public Return(Runnable parent, string source) : base(parent, source)
        {
            Formula = Util.String.SplitOnce(source, ' ')[1];
        }

        protected override void Run()
        {
            GetParentBlock().ReturnedValue = Util.Calc.Execute(GetParentBlock(), Formula, GetParentMethod().ReturnType);
            GetParentBlock().SkipExecute();
        }
    }
}