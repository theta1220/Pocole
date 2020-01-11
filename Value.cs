using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pocole
{
    [Serializable]
    public class Value
    {
        public Type ValueType { get { return Object.GetType(); } }
        public string Name { get; private set; }
        public object Object { get; private set; }

        public Value()
        {
            Name = "";
            Object = null;
        }

        public Value(string name)
        {
            Name = name;
        }

        public Value(string name, string value)
        {
            Name = name;
            Object = value;

            if (ValueType == typeof(int))
            {
                Object = int.Parse(value);
            }
            else
            {
                Object = value;
            }
        }

        public void SetValue(object value)
        {
            Object = value;
        }

        public static Type GetValueType(string source, Block parentBlock = null)
        {
            source = Util.String.Remove(source, ' ');
            var value = Util.String.SplitAny(source, "+-*/");

            // 配列
            if (Util.String.Contains(source, ",") && Util.String.Contains(source, "["))
            {
                return typeof(List<Value>);
            }

            var resInt = 0;
            var resBool = false;
            if (int.TryParse(value[0], out resInt))
            {
                return typeof(int);
            }
            else if (bool.TryParse(value[0], out resBool))
            {
                return typeof(bool);
            }
            else if (parentBlock != null && parentBlock.FindMethod(Util.String.Substring(value[0], '(')) != null)
            {
                return parentBlock.FindMethod(Util.String.Substring(value[0], '(')).ReturnType;
            }
            else if (parentBlock != null && parentBlock.FindValue(value[0]) != null)
            {
                return parentBlock.FindValue(value[0]).ValueType;
            }
            else if (Util.String.Contains(source, "\""))
            {
                return typeof(string);
            }
            else
            {
                return typeof(object);
            }
        }
    }
}