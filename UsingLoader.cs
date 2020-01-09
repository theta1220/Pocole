using System.Linq;

namespace Pocole
{
    public class UsingLoader : Runnable
    {
        public string Name { get; private set; }
        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            Name = Util.String.Remove(Util.String.SplitOnce(source, ' ').Last(), ' ');
            return true;
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