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

        public static void RemoveAt(Value[] args)
        {
            var arr = args[0].Object as List<Value>;
            var index = (int)args[1].Object;
            arr.RemoveAt(index);
        }

        public static int Len(Value[] args)
        {
            var arr = args[0].Object as List<Value>;
            return arr.Count;
        }

        public static void Remove(Value[] args)
        {
            var arr = args[0].Object as List<Value>;
            var item = args[1];
            foreach (var value in arr.ToArray())
            {
                if (value.Object == item.Object)
                {
                    arr.Remove(value);
                }
            }
        }
    }
}