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

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source, SemanticType.Process)) { Log.InitError(); return false; }

            var name = Util.String.Remove(Util.String.SplitOnce(source, '(')[0], ' ');
            if (name == "if") ProcessType = ProcessType.If;
            else if (name == "elseif") ProcessType = ProcessType.ElseIf;
            else ProcessType = ProcessType.Else;
            Formula = Util.String.Remove(Util.String.Extract(source, '(', ')'), ' ');

            var block = new Block();
            if (!block.Initialize(parent, Util.String.Extract(source, '{', '}'))) { Log.InitError(); return false; }
            AddBlock(block);

            return true;
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