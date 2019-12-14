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
                                var commandName = buf.Split(' ')[0];
                                if (commandName == "func")
                                {
                                    var declarer = new MethodDeclarer();

                                    if (!declarer.Initialize(this, buf)) { Log.InitError(); return false; }
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
                    try
                    {
                        var name = buf.Split(' ')[0];

                        // 変数の宣言か代入
                        if (name == "var")
                        {
                            var setter = new ValueSetter();
                            if (!setter.Initialize(this, buf)) { Log.InitError(); return false; }
                            Runnables.Add(setter);
                            Values.Add(setter.Value);
                        }
                        else
                        {
                            var valueName = buf.Replace(" ", "").Split('=')[0];
                            if (FindValue(valueName) != null)
                            {
                                var setter = new ValueSetter();
                                if (!setter.Initialize(this, buf)) { Log.InitError(); return false; }
                                Runnables.Add(setter);
                                Values.Add(setter.Value);
                            }
                            else
                            {
                                // 関数の実行
                                var methodName = buf.Replace(" ", "").Split('(')[0];
                                if (FindMethod(methodName) != null)
                                {
                                    var caller = new MethodCaller();
                                    if (!caller.Initialize(this, buf)) { Log.InitError(); return false; }
                                    Runnables.Add(caller);
                                }
                                // エラー
                                else
                                {
                                    Log.Error("関数がみつかりませんでした: {0}", buf);
                                    return false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        Log.ParseError();
                        return false;
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
            var target = Values.FirstOrDefault(method => method.Name == name);
            if (target == null && Parent != null)
            {
                target = Parent.FindValue(name);
            }
            return target;
        }

        public MethodDeclarer FindMethod(string name)
        {
            var target = Methods.FirstOrDefault(method => method.Name == name);
            if (target == null && Parent != null)
            {
                target = Parent.FindMethod(name);
            }
            return target;
        }
    }
}