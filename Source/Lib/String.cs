using System.Collections.Generic;

namespace Sumi.Lib
{
    class String
    {
        public static List<Value> ToArray(Value[] args)
        {
            var list = new List<Value>();
            var text = args[0].Object as string;

            foreach (var c in text)
            {
                list.Add(new Value("", c.ToString()));
            }
            return list;
        }
    }
}