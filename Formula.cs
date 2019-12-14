using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pocole
{
    public class Formula : Runnable
    {
        public enum Type
        {
            None,
            DeclareValue,
            AssignValue,
            RunMethod,
        }

        public Type FormulaType { get; private set; } = Type.None;
        public string Name { get; private set; }

        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text)) { Log.InitError(); return false; }

            var buf = "";
            foreach (var c in text)
            {
                switch (FormulaType)
                {
                    case Type.None:
                        if (c == '(' || c == '=')
                        {
                            var name = buf.Replace(" ", "");
                            if (name.Length == 0)
                            {
                                Log.Error("SyntaxError:名前が定義されていない関数です");
                                return false;
                            }
                            if (name == "var")
                            {
                                FormulaType = Type.DeclareValue;
                            }
                            else if (c == ' ' || c == '=')
                            {
                                FormulaType = Type.AssignValue;
                                Name = buf;
                            }
                            else if (c == '(')
                            {
                                FormulaType = Type.RunMethod;
                                Name = buf;
                            }
                            else
                            {
                                Log.Error("SyntaxError: 関数でも代入でも変数宣言でもないと判断されました");
                                return false;
                            }
                            buf = "";
                            continue;
                        }
                        break;

                    case Type.RunMethod:
                        {
                            if (c == ')')
                            {
                                var args = text.Split(',');
                                foreach (var arg in args)
                                {
                                    var term = new Term();
                                    if (!term.InitializeTerm(parent, arg.Replace(" ", ""))) { Log.InitError(); return false; }
                                    Runnables.Add(term);
                                }
                                continue;
                            }
                        }
                        break;

                    case Type.DeclareValue:
                        {
                            if (c == ' ')
                            {
                                continue;
                            }
                            if (c == '=')
                            {
                                Name = buf;
                                continue;
                            }
                            if (c == ';')
                            {
                                var term = new Term();
                                if (!term.InitializeTerm(Parent, buf.Replace(" ", ""))) { Log.InitError(); return false; }
                                Runnables.Add(term);
                                continue;
                            }
                        }
                        break;

                }
                buf += c;
            }

            return true;
        }

        protected override void Run()
        {
            RunningLog();
        }
    }
}