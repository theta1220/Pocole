using System;
using System.Linq;
using Pocole.Util;

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
            var className = Source.Split(' ').First();
            var instanceName = Source.Split(' ').Last();
            var instance = GetParentBlock().FindClass(className).Instantiate(Parent, instanceName);
            var value = new Value(instanceName, instance);
            GetParentBlock().AddValue(value);
        }
    }
}