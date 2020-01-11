using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Pocole
{
    [Serializable]
    public class Block : Runnable
    {
        public string Name { get; protected set; }
        public List<Value> Values { get; private set; } = new List<Value>();
        public List<MethodDeclarer> Methods { get; private set; } = new List<MethodDeclarer>();
        public List<Class> Classes { get; private set; } = new List<Class>();
        public bool LastIfResult { get; set; } = false;
        public object ReturnedValue { get; set; }

        public Block(Runnable parent, string text, string name = "") : base(parent, text)
        {
            if (name.Length > 0) Name = name;
            var sources = Util.String.SplitSource(text);

            foreach (var source in sources)
            {
                if (Util.String.MatchHead("func ", source)) Methods.Add(new MethodDeclarer(this, source));
                else if (Util.String.MatchHead("class", source))
                {
                    var classDef = new Class(this, source);
                    var already = FindClass(classDef.Name);
                    if (already == null) Classes.Add(classDef);
                    else already.Using(classDef);
                }
                else if (Util.String.MatchHead("if", source) ||
                         Util.String.MatchHead("else if", source) ||
                         Util.String.MatchHead("else", source)) Runnables.Add(new Process(this, source));
                else if (Util.String.MatchHead("count", source)) Runnables.Add(new Count(this, source));
                else if (Util.String.MatchHead("while", source)) Runnables.Add(new While(this, source));
                else if (Util.String.MatchHead("foreach", source)) Runnables.Add(new Foreach(this, source));
                else if (Util.String.MatchHead("for", source)) Runnables.Add(new For(this, source));
                else if (Util.String.MatchHead("using", source)) Runnables.Add(new UsingLoader(this, source));
                else if (Util.String.MatchHead("return", source)) Runnables.Add(new Return(this, source));
                else Runnables.Add(new Term(this, source));
            }
        }

        public override void OnLeaved()
        {
            Values.Clear();
        }

        public void AddValue(Value value)
        {
            Values.Add(value);
        }

        public Value FindValue(string name)
        {
            if (Util.String.RemoveString(name).Contains("."))
            {
                var split = Util.String.SplitOnce(name, '.');
                var instance = (Class)FindValue(split[0]).Object;
                if (instance != null) return instance.GetMemberValue(split[1]);
                return null;
            }
            if (Util.String.RemoveString(name).Contains("["))
            {
                var arrName = Util.String.Substring(name, '[');
                var index = (int)Util.Calc.Execute(this, Util.String.Extract(name, '[', ']'), typeof(int));
                return (FindValue(arrName).Object as List<Value>)[index];
            }
            bool isRef = true;
            // @がついている変数はコピーが作成される
            if (Util.String.MatchHead("@", name))
            {
                name = Util.String.Remove(name, '@');
                isRef = false;
            }
            var target = Values.FirstOrDefault(value => value.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindValue(name);
            }
            if (!isRef)
            {
                target = Util.Object.DeepCopy(target);
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
                var value = FindValue(split[0]);
                if (value == null)
                {
                    return FindClass(split[0]).GetMemberMethod(split[1]);
                }
                return (value.Object as Class).GetMemberMethod(split[1]);
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
            if (Util.String.RemoveString(name).Contains("."))
            {
                var split = Util.String.SplitOnce(name, '.');
                return FindClass(split[0]).FindClass(split[1]);
            }
            var target = Classes.FirstOrDefault(classDef => classDef.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindClass(name);
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
                if (FindClass(classDef.Name) != null)
                {
                    FindClass(classDef.Name).Using(classDef);
                    continue;
                }
                Classes.Add(classDef);
            }
        }
    }
}