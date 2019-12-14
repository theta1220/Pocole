using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pocole
{
    public class Block : Runnable
    {
        public List<Value> Values { get; private set; } = new List<Value>();
        public List<MethodDeclarer> Methods { get; private set; } = new List<MethodDeclarer>();

        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text)) { Log.InitError(); return false; }

            int blockCount = 0;
            bool isLoadingString = false;
            var buf = "";
            var stack = new Stack<SemanticBlock>();

            foreach (var c in text)
            {
                if (c == '"')
                {
                    isLoadingString = !isLoadingString;
                }
                if (isLoadingString)
                {
                    buf += c;
                    continue;
                }
                if (c == '\n')
                {
                    continue;
                }
                if (c == '{')
                {
                    blockCount++;
                    if (blockCount == 1)
                    {
                        if (buf.Length > 0)
                        {
                            // if / else if / else
                            if (Util.String.ContainsAny(buf, "if", "else if", "else"))
                            {
                                var process = new Process();
                                if (!process.Initialize(this, buf)) { Log.InitError(); return false; }
                                stack.Push(process);
                            }
                            // メソッドの宣言
                            else
                            {
                                var commandName = "";
                                var content = "";
                                var start = false;
                                foreach (var _c in buf)
                                {
                                    if (_c == ' ')
                                    {
                                        start = true;
                                        continue;
                                    }
                                    if (start)
                                    {
                                        content += _c;
                                    }
                                    else
                                    {
                                        commandName += _c;
                                    }
                                }
                                if (commandName == "func")
                                {
                                    var declarer = new MethodDeclarer();

                                    if (!declarer.Initialize(this, content)) { Log.InitError(); return false; }
                                    stack.Push(declarer);
                                    Log.Info("メソッドが登録されました:{0}", declarer.Name);
                                }
                                else
                                {
                                    Log.Error("SyntaxError:得も知れぬコマンド:{0}", commandName);
                                    return false;
                                }
                            }
                        }
                        buf = "";
                        continue;
                    }
                }
                if (c == '}')
                {
                    blockCount--;
                    if (blockCount == 0)
                    {
                        var block = new Block();
                        if (!block.Initialize(this, buf)) { Log.InitError(); return false; }

                        if (stack.Count > 0)
                        {
                            var semantic = stack.Pop();
                            semantic.Block = block;
                            if (semantic.SemanticType == SemanticType.MethodDeclarer)
                            {
                                Methods.Add((MethodDeclarer)semantic);
                            }
                            else
                            {
                                Runnables.Add(semantic);
                            }
                        }
                        else
                        {
                            Runnables.Add(block);
                        }
                        buf = "";
                        continue;
                    }
                }
                if (blockCount == 0 && c == ';')
                {
                    var methodName = "";
                    foreach (var _c in buf)
                    {
                        if (_c == ' ') { continue; }
                        if (_c == '(') { break; }
                        methodName += _c;
                    }

                    // 関数の実行
                    if (FindMethod(methodName) != null)
                    {
                        var caller = new MethodCaller();
                        if (!caller.Initialize(this, buf)) { Log.InitError(); return false; }
                        Runnables.Add(caller);
                    }
                    // 変数の宣言か代入
                    else
                    {
                        var setter = new ValueSetter();
                        if (!setter.Initialize(this, buf)) { Log.InitError(); return false; }
                        Runnables.Add(setter);
                    }
                    buf = "";
                    continue;
                }
                buf += c;
            }

            if (blockCount != 0)
            {
                Log.Error("SyntaxError: ブロックの定義がおかしいです blockCount = {0}", blockCount);
                return false;
            }

            return true;
        }

        protected override void Run()
        {
            RunningLog();
        }

        public void AddValue(Value value)
        {
            Values.Add(value);
        }

        public Value FindValue(string name)
        {
            foreach (var value in Values)
            {
                if (value.Name == name)
                {
                    return value;
                }
            }
            return null;
        }

        public MethodDeclarer FindMethod(string name)
        {
            foreach (var method in Methods)
            {
                if (method.Name == name)
                {
                    return method;
                }
            }
            return null;
        }
    }
}