using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pocole
{
    [Serializable]
    public class SystemCaller : Runnable
    {
        public string ClassName { get; private set; }
        public string Name { get; private set; }
        public string[] Args { get; private set; }

        private Value _returnValue;

        public SystemCaller(Runnable parent, string source) : base(parent, source)
        {
            var args = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ',');

            var count = 0;
            var list = new List<string>();
            foreach (var arg in args)
            {
                if (count == 0)
                {
                    var name = Util.String.Split(Util.String.Extract(arg, '"'), '.');
                    for (var i = 0; i < name.Length - 1; i++)
                    {
                        ClassName += name[i];
                        if (i + 1 < name.Length - 1)
                        {
                            ClassName += ".";
                        }
                    }
                    Name = name.Last();
                }
                else if (count == 1)
                {
                    if (arg != "null") _returnValue = GetParentBlock().FindValue(arg);
                }
                else list.Add(arg);
                count++;
            }
            if (Name == "")
            {
                Log.Error("呼び出すメソッド名が不明です:{0}", source);
                throw new Exception("no name system call");
            }
            Args = list.ToArray();
        }

        protected override void Run()
        {
            var args = new List<Value>();
            foreach (var arg in Args)
            {
                if (arg == "args")
                {
                    var values = GetParentBlock().FindValues(arg);
                    foreach (var val in values)
                    {
                        args.Add(val);
                    }
                }
                else
                {
                    var value = Util.Calc.Execute(GetParentBlock(), arg, Value.GetValueType(arg));
                    args.Add(value);
                }
            }
            var res = MethodInvoke(Name, args.ToArray());
            if (res != null && _returnValue != null)
            {
                _returnValue.Object = res;
            }
        }

        private object MethodInvoke(string methodName, Value[] args)
        {
            var method = Type.GetType(ClassName).GetMethod(methodName);
            if (method == null)
            {
                Log.Error("SystemCallメソッドがみつかりませんでした:{0}.{1}", ClassName, methodName);
                throw new Exception("Method not found");
            }
            if (args.Length > 0) return method.Invoke(null, new[] { args });
            else return method.Invoke(null, new Value[] { });
        }

        public static void Print(Value[] args)
        {
            var list = new List<object>();
            var text = "";
            var count = 0;
            foreach (var arg in args)
            {
                count++;
                if (count == 1)
                {
                    text = arg.Object.ToString();
                    continue;
                }
                list.Add((arg as Value).Object);
            }
            Log.Info(string.Format("{0}", text), list.ToArray());
        }

        public void PrintClassTree(object[] args)
        {
            _PrintClassTree(GetParentBlock(), 0);
        }

        private void _PrintClassTree(Block parent, int tree)
        {
            Log.Info("{0}{1}", GetIndent(tree), parent.Name);
            foreach (var method in parent.Methods)
            {
                Log.Debug("{0}- {1}", GetIndent(tree), method.Name);
            }
            foreach (var classDef in parent.Classes)
            {
                _PrintClassTree(classDef, tree + 1);
            }
        }

        private string GetIndent(int count)
        {
            var space = "";
            for (var i = 0; i < count; i++)
            {
                space += "    ";
            }
            return space;
        }
    }
}