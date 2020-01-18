using System;
using System.Linq;
using Pocole.Util;

namespace Pocole
{
    public class Class : Block
    {
        private string[] _extendNames = new string[] { };
        public Class(Runnable parent, string source) : base(parent, source.PoExtract('{', '}'))
        {
            Name = source.PoCut('{').PoSplitOnce(' ')[1];
            if (Name.Contains(":"))
            {
                _extendNames = Name.PoRemove(' ').PoSplit(':')[1].PoSplit(',');
                Name = Name.PoRemove(' ').PoCut(':');
            }
        }

        public Class(Class other) : base(other)
        {
            _extendNames = other._extendNames.ToArray();
        }

        public override Runnable Clone() { return new Class(this); }

        public void Extend()
        {
            foreach (var name in _extendNames)
            {
                var def = FindClass(name);
                Using(def);
            }
            foreach (var classDef in Classes)
            {
                classDef.Extend();
            }
        }

        public override void OnLeaved()
        {
            // なにもしないのがみそ
        }
    }
}