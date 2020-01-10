namespace Pocole
{
    [System.Serializable]
    public abstract class LoopBlock : Block
    {
        public bool IsContinuous { get; protected set; }

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, Util.String.Extract(source, '{', '}'))) { Log.InitError(); return false; }
            IsContinuous = true;
            return true;
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