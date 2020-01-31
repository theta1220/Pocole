using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Sumi.Util;

namespace Sumi
{
    public class Block : Runnable
    {
        public string Name { get; protected set; }
        public string FullName
        {
            get
            {
                if (Parent != null)
                {
                    return string.Format("{0}.{1}", GetParentBlock().FullName, Name);
                }
                return Name;
            }
        }
        public List<Value> Values { get; private set; } = new List<Value>();
        public List<Function> Functions { get; private set; } = new List<Function>();
        public List<Function> Tests { get; private set; } = new List<Function>();
        public List<UsingLoader> Usings { get; private set; } = new List<UsingLoader>();
        public bool LastIfResult { get; set; } = false;
        public object ReturnedValue { get; set; }

        public Block(Runnable parent, string text, string name = "") : base(parent, text)
        {
            if (name.Length > 0) Name = name;
            var sources = text.PoSplitSource();

            foreach (var source in sources)
            {
                if (source.PoMatchHead("func")) Functions.Add(new Function(this, source));
                else if (source.PoMatchHead("test")) Tests.Add(new Function(this, source));
                else if (source.PoMatchHead("class"))
                {
                    var def = new Class(this, source);
                    var fullName = string.Format("{0}.{1}", FullName, def.Name);
                    if (!Class.ExistsStaticClass(fullName))
                    {
                        Class.AddStaticClass(fullName, def);
                    }
                    else
                    {
                        Class.GetStaticClass(fullName).Using(def);
                    }
                }
                else if (source.PoMatchHead("extension"))
                {
                    var def = new Class(this, source);
                    if (!Class.ExistsStaticExtension(def.Name))
                    {
                        Class.AddStaticExtension(def);
                    }
                    else
                    {
                        Class.GetStaticExtension(def.Name).Using(def);
                    }
                }
                else if (source.PoMatchHead("using")) Usings.Add(new UsingLoader(this, source));
                else if (source.PoMatchHead("if") ||
                         source.PoMatchHead("else if") ||
                         source.PoMatchHead("else")) Runnables.Add(new If(this, source));
                else if (source.PoMatchHead("count")) Runnables.Add(new Count(this, source));
                else if (source.PoMatchHead("while")) Runnables.Add(new While(this, source));
                else if (source.PoMatchHead("foreach")) Runnables.Add(new Foreach(this, source));
                else if (source.PoMatchHead("for")) Runnables.Add(new For(this, source));
                else if (source.PoMatchHead("return")) Runnables.Add(new Return(this, source));
                else if (source.PoMatchHead("continue")) Runnables.Add(new Continue(this, source));
                else if (source.PoMatchHead("break")) Runnables.Add(new Break(this, source));
                else if (source.PoMatchHead("{")) Runnables.Add(new Block(this, source.PoExtract('{', '}')));
                else Runnables.Add(new Term(this, source));
            }
        }

        public Block(Block other) : base(other)
        {
            Name = other.Name;
            other.Values.ForEach(obj => Values.Add(new Value(obj)));
            foreach (var obj in other.Functions)
            {
                var clone = obj.Clone() as Function;
                clone.Parent = this;
                Functions.Add(clone);
            }
            foreach (var obj in other.Usings)
            {
                var clone = obj.Clone() as UsingLoader;
                clone.Parent = this;
                Usings.Add(clone);
            }
            LastIfResult = other.LastIfResult;
            ReturnedValue = other.ReturnedValue;
        }

        public override Runnable Clone() { return new Block(this); }

        public void ExecuteTest()
        {
            foreach (var test in Tests)
            {
                test.ForceExecute();
                if ((bool)test.ReturnedValue == false)
                {
                    Log.Error("{0}のテストに失敗", test.FullName);
                }
            }
        }

        public override void OnEntered()
        {
            foreach (var use in Usings)
            {
                use.ForceExecute();
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
            // 無駄な検索はしない
            if (name.PoMatchHead("[") || name.PoMatchHead("\"") || name.PoMatchHead("(")) return null;
            if (Regex.IsMatch(name, "^[0-9.]+$")) return null;
            if (name == "null") return new Value("", null);

            bool isRef = true;
            // @がついている変数はコピーが作成される
            if (name.PoMatchHead("@"))
            {
                name = name.PoRemove('@');
                isRef = false;
            }
            var target = Values.FirstOrDefault(value => value.Name == name);

            if (name == "this")
            {
                if (this is Function) target = (this as Function).Caller;
                else if (GetParentMethod() != null) target = GetParentMethod().Caller;

                if (target == null)
                {
                    target = new Value("", this);
                }
            }

            if (target == null)
            {
                // ")"で終わるってことは関数の結果を変数として利用したいってこと
                if (name.PoMatchTail(")"))
                {
                    var caller = new Caller(this, name);
                    caller.ForceExecute();
                    target = new Value("", caller.Function.ReturnedValue);
                }
                else
                {
                    if (name.PoRemoveInBlock().Contains('.'))
                    {
                        var split = name.PoSplitOnceTail('.');
                        var instance = FindValue(split[0]);
                        if (instance != null)
                        {
                            target = (instance.Object as Block).FindValue(split[1]);
                        }
                    }
                    else if (name.PoCut('[').Length > 0 && name.PoMatchTail("]"))
                    {
                        var arrName = name.PoCut('[');
                        var source = name.PoExtract('[', ']');
                        var index = (int)Util.Calc.Execute(this, source, typeof(int)).Object;
                        target = (FindValue(arrName).Object as List<Value>)[index];
                    }
                }
            }
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindValue(name);
            }
            if (isRef || target == null) return target;

            if (!isRef && target != null && target.Object != null && target.Object is Class)
            {
                var instance = new Value(target);
                (instance.Object as Runnable).ForceExecute();
                return instance;
            }
            return new Value(target);
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

        public Function FindFunction(string name)
        {
            // 無駄な検索はしない
            if (name.PoMatchHead("[") || name.PoMatchHead("\"") || name.PoMatchHead("(")) return null;
            if (Regex.IsMatch(name, "^[0-9.]+$") || name == "null") return null;

            if (name.PoRemoveInBlock().Contains("."))
            {
                var split = name.PoSplitOnceTail('.');
                var value = FindValue(split[0]);
                if (value == null || value.Object == null)
                {
                    Class parent = null;
                    if (this is Class) parent = this as Class;
                    else parent = GetParentClass();
                    var classDef = parent.FindClass(split[0]);
                    if (classDef != null)
                    {
                        var func = classDef.FindFunction(split[1]);
                        if (func != null) return func;
                    }
                }
                var instance = value.Object as Block;
                if (instance != null)
                {
                    return instance.FindFunction(split[1]);
                }
                return FindExtensionMethod(name);
            }
            var target = Functions.FirstOrDefault(method => method.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindFunction(name);
            }
            return target;
        }

        public Function FindExtensionMethod(string name)
        {
            if (name.PoRemoveString().Contains("."))
            {
                var value = FindValue(name.PoCut('.'));
                if (value == null) return null;
                Class ex = null;
                if (value.Object is List<Value>) ex = Class.FindExtension("array");
                if (value.Object is string) ex = Class.FindExtension("string");
                if (ex == null) Log.Error("Extension not found");
                return ex.FindFunction(name.PoSplit('.').Last());
            }
            return null;
        }

        public void Using(Block block)
        {
            foreach (var value in block.Values)
            {
                if (Values.Find(o => o.Name == value.Name) != null) continue;
                Values.Add(value);
            }
            foreach (var func in block.Functions)
            {
                if (Values.Find(o => o.Name == func.Name) != null) continue;
                var clone = func.Clone() as Function;
                clone.Parent = this;
                Functions.Add(clone);
            }
        }

        public void PrintBlockTree()
        {
            PrintBlockTree(this, 0);
        }

        public void PrintBlockTree(Block parent, int tree)
        {
            Log.Debug(ConsoleColor.DarkBlue, "{0}{1}::{2}", Util.String.GetIndentSpace(tree), parent.GetType(), parent.FullName);
            foreach (var value in parent.Values)
            {
                Log.Debug(ConsoleColor.DarkBlue, "{0}var {1}/{2}", Util.String.GetIndentSpace(tree), value.Name, value.Object.ToString());
            }
            foreach (var method in parent.Functions)
            {
                PrintBlockTree(method, tree + 1);
            }
        }
    }
}