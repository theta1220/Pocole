namespace Pocole
{
    [System.Serializable]
    public class Return : Runnable
    {
        public string Formula { get; private set; }
        public Return(Runnable parent, string source) : base(parent, source)
        {
            var split = Util.String.SplitOnce(source, ' ');
            if (split.Length < 2)
            {
                // NOTE: "return;" としか書かれていない場合は nullを返すことにしておく
                Formula = "null";
                return;
            }
            Formula = split[1];
        }

        protected override void Run()
        {
            GetParentMethod().ReturnedValue = Util.Calc.Execute(GetParentBlock(), Formula, GetParentMethod().ReturnType);
            GetParentMethod().SkipExecute();
        }
    }
}