namespace Pocole
{
    public enum ProcessType
    {
        If,
        ElseIf,
        Else,
    }

    public class Process : SemanticBlock
    {
        public string Name { get; private set; }
        public ProcessType ProcessType { get; private set; }
        public string Formula { get; private set; }

        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text, SemanticType.Process)) { Log.InitError(); return false; }

            var name = Util.String.Remove(Util.String.SplitOnce(text, '(')[0], ' ');
            if (name == "if") ProcessType = ProcessType.If;
            else if (name == "elseif") ProcessType = ProcessType.ElseIf;
            else ProcessType = ProcessType.Else;
            Formula = Util.String.Remove(Util.String.Extract(text, '(', ')'), ' ');
            return true;
        }

        public override void OnEntered()
        {
            if (ProcessType == ProcessType.Else)
            {
                if (Parent.LastIfResult)
                {
                    SkipExecute();
                }
            }
            else
            {

                if (ProcessType == ProcessType.If)
                {
                    var res = (bool)Util.Calc.Execute(Parent, Formula, typeof(bool));
                    if (!res)
                    {
                        SkipExecute();
                    }
                    Parent.LastIfResult = res;
                }
                else if (ProcessType == ProcessType.ElseIf)
                {
                    if (Parent.LastIfResult)
                    {
                        SkipExecute();
                    }
                    else
                    {
                        var res = (bool)Util.Calc.Execute(Parent, Formula, typeof(bool));
                        if (!res)
                        {
                            SkipExecute();
                        }
                        Parent.LastIfResult = res;
                    }
                }
            }
        }
    }
}