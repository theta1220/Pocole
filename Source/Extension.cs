using Sumi.Util;

namespace Sumi
{
    public class Extension : Class
    {
        public Extension(Runnable parent, string source) : base(parent, source)
        {

        }

        public Extension(Extension other) : base(other)
        {

        }

        public override Runnable Clone() { return new Extension(this); }
    }
}