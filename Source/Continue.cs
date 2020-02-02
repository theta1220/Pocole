namespace Sumi
{
    public class Continue : Runnable
    {
        public Continue(Runnable parent, string source) : base(parent, source)
        {
        }

        public Continue(Continue other) : base(other)
        {
        }

        public override Runnable Clone() { return new Continue(this); }

        protected override void Run()
        {
            GetParentLoopBlock().ContinueExecute();
        }
    }
}