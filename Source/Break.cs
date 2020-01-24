namespace Sumi
{
    public class Break : Runnable
    {
        public Break(Runnable parent, string source) : base(parent, source)
        {
        }

        public Break(Break other) : base(other)
        {
        }

        public override Runnable Clone() { return new Break(this); }

        protected override void Run()
        {
            GetParentLoopBlock().IsContinuous = false;
            GetParentLoopBlock().SkipExecute();
        }
    }
}