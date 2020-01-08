using System;
using System.Linq;

namespace Pocole
{
    public class ClassInstantiator : Runnable
    {
        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }
            return true;
        }

        protected override void Run()
        {
            var className = Util.String.Split(Source, ' ').First();
            var instanceName = Util.String.Split(Source, ' ').Last();
            var instance = GetParentBlock().FindClass(className).Instantiate(Parent, instanceName);
            instance.ForceExecute();
            GetParentBlock().AddClassInstance(instance);
        }
    }
}