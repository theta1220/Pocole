namespace Pocole
{
    [System.Serializable]
    public abstract class LoopBlock : Block
    {
        public bool IsContinuous { get; protected set; }

        public LoopBlock(Runnable parent, string source) : base(parent, Util.String.Extract(source, '{', '}'))
        {
            IsContinuous = true;
        }

        public override bool CheckContinue()
        {
            return IsContinuous;
        }

        public override void OnLeaved()
        {
            if (!IsContinuous)
            {
                base.OnLeaved();
            }
        }
    }
}