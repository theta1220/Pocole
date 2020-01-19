using System.Diagnostics;

namespace Sumi.Util
{
    public static class Reflect
    {
        public static string GetCallerMethodName(int stack = 1)
        {
            return (new StackFrame(stack, true).GetMethod().Name);
        }

        public static string GetCallerClassName(int stack = 1)
        {
            return (new StackFrame(stack, true).GetMethod().ReflectedType.Name);
        }

        public static int GetCallerMethodLineNo(int stack = 1)
        {
            return (new StackFrame(stack, true).GetFileLineNumber());
        }
    }
}