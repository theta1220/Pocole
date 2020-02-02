using System;
using System.Diagnostics;

namespace Sumi
{
    public class Log
    {
        public static void Info(string text) { Info(new Util.Reflect.Info(), text, null); }
        public static void Info(string text, params object[] args) { Info(new Util.Reflect.Info(), text, args); }
        public static void Info(Util.Reflect.Info stackInfo, string text, params object[] args)
        {
            _Print(stackInfo, ConsoleColor.White, "Info", text, args);
        }
        public static void Debug(string text) { Debug(new Util.Reflect.Info(), text, null); }
        public static void Debug(string text, params object[] args) { Debug(new Util.Reflect.Info(), text, args); }
        public static void Debug(Util.Reflect.Info stackInfo, string text, params object[] args)
        {
            _Print(stackInfo, ConsoleColor.DarkGreen, "Debug", text, args);
        }
        public static void Warn(string text) { Warn(new Util.Reflect.Info(), text, null); }
        public static void Warn(string text, params object[] args) { Warn(new Util.Reflect.Info(), text, args); }
        public static void Warn(Util.Reflect.Info stackInfo, string text, params object[] args)
        {
            _Print(stackInfo, ConsoleColor.DarkYellow, "Warn", text, args);
        }
        public static void Error(string text) { Error(new Util.Reflect.Info(), text, null); }
        public static void Error(string text, params object[] args) { Error(new Util.Reflect.Info(), text, args); }
        public static void Error(Util.Reflect.Info stackInfo, string text, params object[] args)
        {
            _Print(stackInfo, ConsoleColor.DarkRed, "Error", text, args);
            throw new Exception(string.Format(text, args));
        }
        public static void Assert(bool condition, string text) { Assert(condition, new Util.Reflect.Info(), text, null); }
        public static void Assert(bool condition, string text, params object[] args) { Assert(condition, new Util.Reflect.Info(), text, args); }
        public static void Assert(bool condition, Util.Reflect.Info stackInfo, string text, params object[] args)
        {
            if (!condition)
            {
                _Print(stackInfo, ConsoleColor.DarkRed, "Assert", text, args);
                throw new Exception(string.Format(text, args));
            }
        }
        private static void _Print(Util.Reflect.Info stackInfo, ConsoleColor color, string title, string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var info = string.Format("[Sumi {0}]:{1}/{2}({3})",
                title,
                stackInfo.ClassName,
                stackInfo.MethodName,
                stackInfo.MethodLineNo);
            var message = string.Format("{0}: \"{1}\"", info, text);
            Console.WriteLine(message, args);
            Console.ResetColor();
        }
    }
}