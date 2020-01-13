using System;
using System.Diagnostics;

namespace Pocole
{
    public class Log
    {
        public static void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            _Print("Info", text, 3, null);
        }
        public static void Debug(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            _Print("Debug", text, 3, null);
        }
        public static void Warn(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            _Print("Warn", text, 3, null);
        }
        public static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            _Print("Error", text, 3, null);
        }
        public static void Info(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            _Print("Info", text, 3, args);
        }
        public static void Debug(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            _Print("Debug", text, 3, args);
        }
        public static void Warn(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            _Print("Warn", text, 3, args);
        }
        public static void Error(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            _Print("Error", text, 3, args);
        }
        private static void _Print(string title, string text, int stack, params object[] args)
        {
            var info = string.Format("[Pocole {0}]:{1}/{2}({3})",
                title,
                Util.Reflect.GetCallerClassName(stack),
                Util.Reflect.GetCallerMethodName(stack),
                Util.Reflect.GetCallerMethodLineNo(stack));
            var message = string.Format("{0}: \"{1}\"", info, text);
            Console.WriteLine(message, args);
            Console.ResetColor();
        }
        public static void InitError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            _Print("InitError", "初期化に失敗しちゃったなり", 3, null);
        }
        public static void ParseError()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            _Print("ParseError", "パースに失敗しちゃったなり", 3, null);
        }
        public static void ParseError(string source)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            var message = string.Format("パースに失敗しちゃったなり:{0}", source);
            _Print("ParseError", message, 3, null);
        }
        public static void ParseError(System.Exception e, string source)
        {
            ParseError(source);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            _Print("Exeption", e.Message, 3, null);
        }
    }
}