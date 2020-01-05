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
        public bool LastIfResult { get; set; } = false;

        public new bool Initialize(Block parent, string text)
        {
            if (!base.Initialize(parent, text)) { Log.InitError(); return false; }

            int blockCount = 0;
            bool isLoadingString = false;
            var buf = "";
            var stack = new Stack<SemanticBlock>();

            // ブロックの中はあとで読みたいので一旦ここにとっておく
            var blocks = new Dictionary<SemanticBlock, string>();

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
                                }
                                else if (commandName == "class")
                                {
                                    var classBlock = new Class();
                                    if (!classBlock.Initialize(this, buf)) { Log.InitError(); return false; }
                                    stack.Push(classBlock);
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
                        if (stack.Count > 0)
                        {
                            var semantic = stack.Pop();
                            if (semantic.SemanticType == SemanticType.MethodDeclarer)
                            {
                                Methods.Add((MethodDeclarer)semantic);
                            }
                            else if (semantic.SemanticType == SemanticType.Class)
                            {
                                Classes.Add((Class)semantic);
                            }
                            else
                            {
                                Runnables.Add(semantic);
                            }
                            blocks.Add(semantic, buf);
                        }
                        else
                        {
                            var block = new Block();
                            if (!block.Initialize(this, buf)) { Log.InitError(); return false; }
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
                        var classDef = FindClass(name);

                        // 変数の宣言
                        if (name == "var")
                        {
                            var setter = new ValueSetter();
                            if (!setter.Initialize(this, buf)) { Log.InitError(); return false; }
                            Runnables.Add(setter);
                        }
                        // クラスインスタンスの宣言
                        else if (classDef != null)
                        {

                        }
                        else
                        {
                            // メソッド
                            var methodName = buf.Replace(" ", "").Split('(')[0];
                            if (methodName == "SystemCall")
                            {
                                var caller = new SystemCaller();
                                if (!caller.Initialize(this, buf)) { Log.InitError(); return false; }
                                Runnables.Add(caller);
                            }
                            else if (FindMethod(methodName) != null)
                            {
                                var caller = new MethodCaller();
                                if (!caller.Initialize(this, buf)) { Log.InitError(); return false; }
                                Runnables.Add(caller);
                            }
                            // やっぱり変数
                            else
                            {
                                var setter = new ValueSetter();
                                if (!setter.Initialize(this, buf)) { Log.InitError(); return false; }
                                Runnables.Add(setter);
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Log.ParseError(e, buf);
                        return false;
                    }
                    buf = "";
                    continue;
                }
                buf += c;
            }

            foreach (var pair in blocks)
            {
                var block = new Block();
                if (!block.Initialize(this, pair.Value)) { Log.InitError(); return false; }
                pair.Key.AddBlock(block);
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
            var target = Values.FirstOrDefault(value => value.Name == name);
            if (target == null && Parent != null)
            {
                target = Parent.FindValue(name);
            }
            return target;
        }

        public Value[] FindValues(string name)
        {
            var target = Values.FindAll(value => value.Name == name);
            if (target.Count == 0 && Parent != null)
            {
                target = Parent.FindValues(name).ToList();
            }
            return target.ToArray();
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

        public Class FindClass(string name)
        {
            var target = Classes.FirstOrDefault(classDef => classDef.Name == name);
            if (target == null && Parent != null)
            {
                target = Parent.FindClass(name);
            }
            return target;
        }
    }
}