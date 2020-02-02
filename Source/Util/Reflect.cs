using System.Diagnostics;

namespace Sumi.Util
{
    public static class Reflect
    {
        public class Info
        {
            public string MethodName { get; private set; }
            public string ClassName { get; private set; }
            public int MethodLineNo { get; private set; }
            public Info(int stack = 3)
            {
                MethodName = GetCallerMethodName(stack);
                ClassName = GetCallerClassName(stack);
                MethodLineNo = GetCallerMethodLineNo(stack);
            }
        }
        private static string GetCallerMethodName(int stack)
        {
            return (new StackFrame(stack, true).GetMethod().Name);
        }

        private static string GetCallerClassName(int stack)
        {
            return (new StackFrame(stack, true).GetMethod().ReflectedType.Name);
        }

        private static int GetCallerMethodLineNo(int stack)
        {
            return (new StackFrame(stack, true).GetFileLineNumber());
        }
    }
}