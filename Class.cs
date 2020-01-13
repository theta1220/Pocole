using System;
using System.Linq;
using Pocole.Util;

namespace Pocole
{
    [Serializable]
    public class Class : Block
    {
        public Class(Runnable parent, string source) : base(parent, source.PoExtract('{', '}'))
        {
            Name = source.Split('{')[0].Split(' ')[1];
        }

        public override void OnLeaved()
        {
            // なにもしないのがみそ
        }

        public Class Instantiate(Runnable parent, string name)
        {
            var instance = Util.Object.DeepCopy(this);
            instance.Name = name;
            instance.ForceExecute();
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