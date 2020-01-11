namespace Pocole
{
    public enum ProcessType
    {
        If,
        ElseIf,
        Else,
    }

    [System.Serializable]
    public class Process : SemanticBlock
    {
        public string Name { get; private set; }
        public ProcessType ProcessType { get; private set; }
        public string Formula { get; private set; }

        public Process(Runnable parent, string source) : base(parent, source, SemanticType.Process)
        {
            var name = Util.String.Remove(Util.String.SplitOnce(source, '(')[0], ' ');
            if (name == "if") ProcessType = ProcessType.If;
            else if (name == "elseif") ProcessType = ProcessType.ElseIf;
            else ProcessType = ProcessType.Else;
            Formula = Util.String.Remove(Util.String.Extract(source, '(', ')'), ' ');

            var block = new Block(parent, Util.String.Extract(source, '{', '}'));
            AddBlock(block);
        }

        public override void OnEntered()
        {
            if (ProcessType == ProcessType.Else)
            {
                if (GetParentBlock().LastIfResult)
                {
                    SkipExecute();
                }
            }
            else
            {
                if (ProcessType == ProcessType.If)
                {
                    var res = (bool)Util.Calc.Execute(GetParentBlock(), Formula, typeof(bool));
                    if (!res)
                    {
                        SkipExecute();
                    }
                    GetParentBlock().LastIfResult = res;
                }
                else if (ProcessType == ProcessType.ElseIf)
                {
                    if (GetParentBlock().LastIfResult)
                    {
                        SkipExecute();
                    }
                    else
                    {
                        var res = (bool)Util.Calc.Execute(GetParentBlock(), Formula, typeof(bool));
                        if (!res)
                        {
                            SkipExecute();
                        }
                        GetParentBlock().LastIfResult = res;
                    }
                }
            }
        }
    }
}