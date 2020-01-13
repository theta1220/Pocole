using System.Collections.Generic;

namespace Pocole.Lib
{
    public class Array
    {
        public static void Push(Value[] args)
        {
            var arr = args[0].Object as List<Value>;
            var item = args[1];
            arr.Add(item);
        }

        public static void Pop(Value[] args)
        {
            var arr = args[0].Object as List<Value>;
            arr.RemoveAt(arr.Count - 1);
        }

        public static int Len(Value[] args)
        {
            var arr = args[0].Object as List<Value>;
            return arr.Count;
        }
    }
}