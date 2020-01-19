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
        public List<Value> Values { get; private set; } = new List<Value>();
        public List<MethodDeclarer> Methods { get; private set; } = new List<MethodDeclarer>();
        public List<Class> Classes { get; private set; } = new List<Class>();
        public List<Extension> Extensions { get; private set; } = new List<Extension>();
        public List<UsingLoader> Usings { get; private set; } = new List<UsingLoader>();
        public bool LastIfResult { get; set; } = false;
        public object ReturnedValue { get; set; }

        public Block(Runnable parent, string text, string name = "") : base(parent, text)
        {
            if (name.Length > 0) Name = name;
            var sources = text.PoSplitSource();

            foreach (var source in sources)
            {
                if (source.PoMatchHead("func ")) Methods.Add(new MethodDeclarer(this, source));
                else if (source.PoMatchHead("class"))
                {
                    var classDef = new Class(this, source);
                    var already = FindClass(classDef.Name);
                    if (already == null) Classes.Add(classDef);
                    else already.Using(classDef);
                }
                else if (source.PoMatchHead("extension")) Extensions.Add(new Extension(this, source));
                else if (source.PoMatchHead("using")) Usings.Add(new UsingLoader(this, source));
                else if (source.PoMatchHead("if") ||
                         source.PoMatchHead("else if") ||
                         source.PoMatchHead("else")) Runnables.Add(new If(this, source));
                else if (source.PoMatchHead("count")) Runnables.Add(new Count(this, source));
                else if (source.PoMatchHead("while")) Runnables.Add(new While(this, source));
                else if (source.PoMatchHead("foreach")) Runnables.Add(new Foreach(this, source));
                else if (source.PoMatchHead("for")) Runnables.Add(new For(this, source));
                else if (source.PoMatchHead("return")) Runnables.Add(new Return(this, source));
                else if (source.PoMatchHead("{")) Runnables.Add(new Block(this, source.PoExtract('{', '}')));
                else Runnables.Add(new Term(this, source));
            }
        }

        public Block(Block other) : base(other)
        {
            Name = other.Name;
            other.Values.ForEach(obj => Values.Add(new Value(obj)));
            foreach (var obj in other.Methods)
            {
                var clone = obj.Clone() as MethodDeclarer;
                clone.Parent = this;
                Methods.Add(clone);
            }
            foreach (var obj in other.Classes)
            {
                var clone = obj.Clone() as Class;
                clone.Parent = this;
                Classes.Add(clone);
            }
            foreach (var obj in other.Extensions)
            {
                var clone = obj.Clone() as Extension;
                clone.Parent = this;
                Extensions.Add(clone);
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

        public override void OnEntered()
        {
            foreach (var use in Usings)
            {
                use.ForceExecute();
            }
            foreach (var classDef in Classes)
            {
                classDef.Extend();
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
            if (Regex.IsMatch(name, "^[0-9.]+$") || name == "null") return null;

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
                if (this is MethodDeclarer) target = (this as MethodDeclarer).Caller;
                else target = GetParentMethod().Caller;
                if (target == null) throw new Exception("this not found");
            }

            if (target == null)
            {
                // ")"で終わるってことは関数の結果を変数として利用したいってこと
                if (name.PoMatchTail(")"))
                {
                    var caller = new MethodCaller(this, name);
                    caller.ForceExecute();
                    target = new Value("", caller.Method.ReturnedValue);
                }
                else
                {
                    if (name.PoRemoveInBlock().Contains('.'))
                    {
                        var split = name.PoSplitOnceTail('.');
                        var instance = FindValue(split[0]);
                        if (instance == null)
                        {
                            Log.Error("instanceの特定に失敗しました{0}", split[0]);
                            PrintBlockTree();
                        }
                        target = (instance.Object as Block).FindValue(split[1]);
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

        public MethodDeclarer FindMethod(string name)
        {
            // 無駄な検索はしない
            if (name.PoMatchHead("[") || name.PoMatchHead("\"") || name.PoMatchHead("(")) return null;
            if (Regex.IsMatch(name, "^[0-9.]+$") || name == "null") return null;

            if (name.PoRemoveInBlock().Contains("."))
            {
                var split = name.PoSplitOnceTail('.');
                var value = FindValue(split[0]);
                if (value == null)
                {
                    return FindClass(split[0]).FindMethod(split[1]);
                }
                var instance = value.Object as Block;
                if (instance != null)
                {
                    return instance.FindMethod(split[1]);
                }
                return FindExtensionMethod(name);
            }
            var target = Methods.FirstOrDefault(method => method.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindMethod(name);
            }
            return target;
        }

        public MethodDeclarer FindExtensionMethod(string name)
        {
            if (name.PoRemoveString().Contains("."))
            {
                var value = FindValue(name.PoCut('.'));
                if (value == null) return null;
                if (value.Object is List<Value>)
                {
                    var ex = FindExtension("Array");
                    if (ex == null)
                    {
                        Log.Error("Extension not found");
                    }
                    return FindExtension("Array").FindMethod(name.PoSplit('.').Last());
                }
            }
            return null;
        }

        public Extension FindExtension(string name)
        {
            var target = Extensions.FirstOrDefault(ex => ex.Name == name);
            if (target == null && GetParentBlock() != null)
            {
                target = GetParentBlock().FindExtension(name);
            }
            return target;
        }

        public Class FindClass(string name)
        {
            // 無駄な検索はしない
            if (name.PoMatchHead("[") || name.PoMatchHead("\"") || name.PoMatchHead("(")) return null;
            if (Regex.IsMatch(name, "^[0-9.]+$") || name == "null") return null;

            var split = name.PoSplitOnce('.');
            if (split.Length > 1)
            {
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
            block.Parent = this;
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
            foreach (var ex in block.Extensions)
            {
                if (FindExtension(ex.Name) != null) continue;
                Extensions.Add(ex);
            }
        }

        public void PrintBlockTree()
        {
            PrintBlockTree(this, 0);
        }

        public void PrintBlockTree(Block parent, int tree)
        {
            Log.Debug(ConsoleColor.DarkBlue, "{0}{1}::{2}", Util.String.GetIndentSpace(tree), parent.GetType(), parent.Name);
            foreach (var value in parent.Values)
            {
                Log.Debug(ConsoleColor.DarkBlue, "{0}var {1}/{2}", Util.String.GetIndentSpace(tree), value.Name, value.Object.ToString());
            }
            foreach (var method in parent.Methods)
            {
                PrintBlockTree(method, tree + 1);
            }
            foreach (var classDef in parent.Classes)
            {
                PrintBlockTree(classDef, tree + 1);
            }
            foreach (var ex in parent.Extensions)
            {
                PrintBlockTree(ex, tree + 1);
            }
        }
    }
}