using System;
using System.Linq;

namespace Pocole
{
    [System.Serializable]
    public class ClassInstantiator : Runnable
    {
        public ClassInstantiator(Runnable parent, string source) : base(parent, source)
        {
        }

        protected override void Run()
        {
            var className = Util.String.Split(Source, ' ').First();
            var instanceName = Util.String.Split(Source, ' ').Last();
            var instance = GetParentBlock().FindClass(className).Instantiate(Parent, instanceName);
            var value = new Value(instanceName, instance);
            GetParentBlock().AddValue(value);
        }
    }
}