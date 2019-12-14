using System;
using System.Diagnostics;

namespace Pocole
{
    public class Log
    {
        public static void Info(string text)
        {
            _Print("Info", text, 3, null);
        }
        public static void Warn(string text)
        {
            _Print("Warn", text, 3, null);
        }
        public static void Error(string text)
        {
            _Print("Error", text, 3, null);
        }
        public static void Info(string text, params object[] args)
        {
            _Print("Info", text, 3, args);
        }
        public static void Warn(string text, params object[] args)
        {
            _Print("Warn", text, 3, args);
        }
        public static void Error(string text, params object[] args)
        {
            _Print("Error", text, 3, args);
        }
        private static void _Print(string title, string text, int stack, params object[] args)
        {
            var info = String.Format("[Pocole {0}]:{1}/{2}({3}): ",
                title,
                Util.Reflect.GetCallerClassName(stack),
                Util.Reflect.GetCallerMethodName(stack),
                Util.Reflect.GetCallerMethodLineNo(stack));
            Console.WriteLine(info + text, args);
        }
        public static void InitError()
        {
            _Print("Error", "初期化に失敗しちゃったなり", 3);
        }
    }
}