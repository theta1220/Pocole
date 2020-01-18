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
            Console.ResetColor();
        }
        public static void Debug(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            _Print("Debug", text, 3, null);
            Console.ResetColor();
        }
        public static void Warn(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            _Print("Warn", text, 3, null);
            Console.ResetColor();
        }
        public static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            _Print("Error", text, 3, null);
            Console.ResetColor();
        }
        public static void Info(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            _Print("Info", text, 3, args);
            Console.ResetColor();
        }
        public static void Debug(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            _Print("Debug", text, 3, args);
            Console.ResetColor();
        }
        public static void Debug(ConsoleColor color, string text, params object[] args)
        {
            Console.ForegroundColor = color;
            _Print("Debug", text, 3, args);
            Console.ResetColor();
        }
        public static void Warn(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            _Print("Warn", text, 3, args);
            Console.ResetColor();
        }
        public static void Error(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            _Print("Error", text, 3, args);
            Console.ResetColor();
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
        }
    }
}