using System;
using System.Linq;

namespace Pocole
{
    [Serializable]
    public class Class : Block
    {
        public new bool Initialize(Runnable parent, string source)
        {
            Name = source.Split('{')[0].Split(' ')[1];
            if (!base.Initialize(parent, Util.String.Extract(source, '{', '}'))) { Log.InitError(); return false; }
            return true;
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
                var instance = Util.String.SplitOnce(name, '.').First();
                var member = Util.String.SplitOnce(name, '.').Last();
                FindClassInstance(instance).GetMemberValue(member);
            }
            return FindValue(name);
        }

        public MethodDeclarer GetMemberMethod(string name)
        {
            if (name.Contains("."))
            {
                var instanceName = Util.String.SplitOnce(name, '.').First();
                var memberName = Util.String.SplitOnce(name, '.').Last();
                var target = FindClassInstance(instanceName);
                if (target == null)
                {
                    target = FindClass(instanceName);
                }
                target.GetMemberMethod(memberName);
            }
            return FindMethod(name);
        }
    }
}