using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Pocole
{
    public class Block : Runnable
    {
        public List<Value> Values { get; private set; } = new List<Value>();
        public List<MethodDeclarer> Methods { get; private set; } = new List<MethodDeclarer>();
        public List<Class> Classes { get; private set; } = new List<Class>();
        public List<Class> ClassInstances { get; private set; } = new List<Class>();
        public bool LastIfResult { get; set; } = false;

        public new bool Initialize(Runnable parent, string text)
        {
            if (!base.Initialize(parent, text)) { Log.InitError(); return false; }

            var sources = Util.String.SplitSource(text);

            foreach (var source in sources)
            {
                if (Util.String.MatchHead("func ", source))
                {
                    var method = new MethodDeclarer();
                    if (!method.Initialize(this, source)) { Log.InitError(); return false; }
                    Methods.Add(method);
                }
                else if (Util.String.MatchHead("if", source) || Util.String.MatchHead("else if", source) || Util.String.MatchHead("else", source))
                {
                    var process = new Process();
                    if (!process.Initialize(this, source)) { Log.InitError(); return false; }
                    Runnables.Add(process);
                }
                else if (Util.String.MatchHead("class", source))
                {
                    var classDef = new Class();
                    if (!classDef.Initialize(this, source)) { Log.InitError(); return false; }
                    Classes.Add(classDef);
                }
                else if (Util.String.MatchHead("using", source))
                {
                    var loader = new UsingLoader();
                    if (!loader.Initialize(this, source)) { Log.InitError(); return false; }
                    loader.ForceExecute();
                }
                else
                {
                    var term = new Term();
                    if (!term.Initialize(this, source)) { Log.InitError(); return false; }
                    Runnables.Add(term);
                }
            }
            return true;
        }

        public override void OnLeaved()
        {
            Values.Clear();
            ClassInstances.Clear();
        }

        public void AddValue(Value value)
        {
            Values.Add(value);
        }

        public void AddClassInstance(Class instance)
        {
            ClassInstances.Add(instance);
        }

        public virtual Value FindValue(string name)
        {
            if (Util.String.RemoveString(name).Contains("."))
            {
                var split = Util.String.SplitOnce(name, '.');
                return FindClassInstance(split[0]).GetMemberValue(split[1]);
            }
            var target = Values.FirstOrDefault(value => value.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindValue(name);
            }
            return target;
        }

        public Value[] FindValues(string name)
        {
            var target = Values.FindAll(value => value.Name == name);
            if (target.Count == 0 && GetParentBlock() != null)
            {
                target = GetParentBlock().FindValues(name).ToList();
            }
            return target.ToArray();
        }

        public MethodDeclarer FindMethod(string name)
        {
            if (Util.String.RemoveString(name).Contains("."))
            {
                var split = Util.String.SplitOnce(name, '.');
                var instance = FindClassInstance(split[0]);
                if (instance == null)
                {
                    instance = FindClass(split[0]);
                }
                return instance.GetMemberMethod(split[1]);
            }
            var target = Methods.FirstOrDefault(method => method.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindMethod(name);
            }
            return target;
        }

        public Class FindClass(string name)
        {
            var target = Classes.FirstOrDefault(classDef => classDef.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindClass(name);
            }
            return target;
        }

        public Class FindClassInstance(string name)
        {
            var target = ClassInstances.FirstOrDefault(instance => instance.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindClassInstance(name);
            }
            return target;
        }

        public void Using(Block block)
        {
            foreach (var value in block.Values)
            {
                if (FindValue(value.Name) != null) continue;
                Values.Add(value);
            }
            foreach (var method in block.Methods)
            {
                if (FindMethod(method.Name) != null) continue;
                Methods.Add(method);
            }
            foreach (var classDef in block.Classes)
            {
                if (FindClass(classDef.Name) != null) continue;
                Classes.Add(classDef);
            }
            foreach (var instance in block.ClassInstances)
            {
                if (FindClassInstance(instance.Name) != null) continue;
                ClassInstances.Add(instance);
            }
        }
    }
}