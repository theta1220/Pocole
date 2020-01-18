using System;
using System.Collections.Generic;
using System.Linq;
using Pocole.Util;

namespace Pocole
{
    public class UsingLoader : Runnable
    {
        public string Name { get; private set; }

        private static List<string> UsingedNames = new List<string>();

        public UsingLoader(Runnable parent, string source) : base(parent, source)
        {
            Name = source.PoSplitOnce(' ').Last().PoRemove(' ');
        }

        public UsingLoader(UsingLoader other) : base(other)
        {
            Name = other.Name;
        }

        public override Runnable Clone() { return new UsingLoader(this); }

        protected override void Run()
        {
            var loader = new Loader();
            var file = string.Format("{0}.pcl", Name.Replace('.', '/'));
            if (UsingedNames.Contains(file))
            {
                return;
            }
            UsingedNames.Add(file);
            var block = (Block)loader.Load(file);
            block.ForceExecute();
            GetParentBlock().Using(block);
        }
    }
}