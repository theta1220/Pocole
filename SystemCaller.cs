using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pocole
{
    public class SystemCaller : Runnable
    {
        public string Name { get; private set; }
        public string[] Args { get; private set; }

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }
            var args = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ',');

            var count = 0;
            var list = new List<string>();
            foreach (var arg in args)
            {
                count++;

                if (count == 1)
                {
                    Name = Util.String.Extract(arg, '"');
                }
                else
                {
                    list.Add(arg);
                }
            }
            if (Name == "")
            {
                Log.Error("呼び出すメソッド名が不明です:{0}", source);
                throw new Exception("no name system call");
            }
            Args = list.ToArray();
            return true;
        }

        protected override void Run()
        {
            var args = new List<object>();
            foreach (var arg in Args)
            {
                if (arg == "args")
                {
                    var values = GetParentBlock().FindValues(arg);
                    foreach (var val in values)
                    {
                        args.Add(val.Object);
                    }
                }
                else
                {
                    var value = Util.Calc.Execute(GetParentBlock(), arg, Value.GetValueType(arg));
                    args.Add(value);
                }
            }
            MethodInvoke(Name, args.ToArray());
        }

        private object MethodInvoke(string methodName, object[] args)
        {
            var method = typeof(SystemCaller).GetMethod(methodName, new Type[] { typeof(object[]) });
            if (method == null)
            {
                Log.Error("SystemCallメソッドがみつかりませんでした:{0}", methodName);
                throw new Exception("Method not found");
            }
            return method.Invoke(this, new[] { args });
        }

        public void Print(object[] args)
        {
            var list = new List<object>();
            var text = "";
            var count = 0;
            foreach (var arg in args)
            {
                count++;
                if (count == 1)
                {
                    text = arg.ToString();
                    continue;
                }
                list.Add(arg);
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