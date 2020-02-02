using Sumi.Util;
using System;

namespace Sumi
{
    public class LoopBlock : Block
    {
        public bool IsContinuous { get; set; }

        public LoopBlock(Runnable parent, string source) : base(parent, source.PoExtract('{', '}'), "loop")
        {
            IsContinuous = true;
        }

        public LoopBlock(LoopBlock other) : base(other)
        {
            IsContinuous = other.IsContinuous;
        }

        public override Runnable Clone() { return new LoopBlock(this); }

        public override bool CheckContinue()
        {
            return IsContinuous;
        }

        public override void OnLeaved()
        {
            base.OnLeaved();
        }

        public override void OnReset()
        {
            base.OnReset();
            IsContinuous = true;
        }
    }
}