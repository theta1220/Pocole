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

        public override object Clone() { return new Class(this); }

        public void Extend()
        {
            foreach (var name in _extendNames)
            {
                var def = FindClass(name);
                Using(def);
            }
            ForceExecute();
        }

        public override void OnLeaved()
        {
            // なにもしないのがみそ
        }

        public Class Instantiate(Runnable parent, string name)
        {
            var instance = Clone() as Class;
            instance.Name = name;
            return instance;
        }

        public Value GetMemberValue(string name)
        {
            if (name.Contains("."))
            {
                var instance = name.PoSplitOnce('.').First();
                var member = name.PoSplitOnce('.').Last();
                Log.Debug("{0}/{1}", instance, member);
                return (FindValue(instance).Object as Class).GetMemberValue(member);
            }
            return FindValue(name);
        }

        public MethodDeclarer GetMemberMethod(string name)
        {
            if (name.Contains("."))
            {
                var instanceName = name.PoSplitOnce('.').First();
                var memberName = name.PoSplitOnce('.').Last();
                var value = FindValue(instanceName);
                if (value == null)
                {
                    return FindClass(instanceName).GetMemberMethod(memberName);
                }
                return (value.Object as Class).GetMemberMethod(memberName);
            }
            return FindMethod(name);
        }
    }
}