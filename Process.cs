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
            if (name == "elseif") ProcessType = ProcessType.ElseIf;
            if (name == "else") ProcessType = ProcessType.Else;
            Formula = Util.String.Remove(Util.String.Extract(text, '(', ')'), ' ');
            return true;
        }

        protected override void Run()
        {
            if (ProcessType == ProcessType.Else)
            {
                if (Parent.LastIfResult)
                {
                    Runnables.Clear();
                }
            }
            else
            {
                var type = Value.GetValueType(Formula);
                var ans = Util.Calc.Execute(Parent, Formula, type);
                var res = false;
                if (type == typeof(int)) { res = (int)ans > 0; }
                if (type == typeof(string)) { res = ((string)ans).Length > 0; }

                if (ProcessType == ProcessType.If)
                {
                    if (!res)
                    {
                        Runnables.Clear();
                    }
                }
                else if (ProcessType == ProcessType.ElseIf)
                {
                    if (!res || Parent.LastIfResult)
                    {
                        Runnables.Clear();
                    }
                }
                Parent.LastIfResult = res;
            }

        }
    }
}