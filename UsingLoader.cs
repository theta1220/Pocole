using System;
using System.Linq;

namespace Pocole
{
    [Serializable]
    public class UsingLoader : Runnable
    {
        public string Name { get; private set; }
        public UsingLoader(Runnable parent, string source) : base(parent, source)
        {
            Name = Util.String.Remove(Util.String.SplitOnce(source, ' ').Last(), ' ');
        }

        protected override void Run()
        {
            var loader = new Loader();
            var block = (Block)loader.Load(string.Format("{0}.pcl", Name.Replace('.', '/')));
            block.ForceExecute();
            GetParentBlock().Using(block);
        }
    }
}