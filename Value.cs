using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Pocole.Util;

namespace Pocole
{
    [Serializable]
    public class Value
    {
        public Type ValueType
        {
            get
            {
                if (Object == null)
                {
                    return typeof(object);
                }
                return Object.GetType();
            }
        }
        public string Name { get; set; }
        public object Object { get; set; }
        public List<MethodDeclarer> Methods { get; private set; } = new List<MethodDeclarer>();

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

        public Value(string name, object value)
        {
            Name = name;
            Object = value;
        }

        public static Type GetValueType(string source, Block parentBlock = null)
        {
            var value = GetFirstCalcSource(source);

            // 配列
            if ((source.Contains(",") && source.Contains('[')) || source == "[]")
            {
                return typeof(List<Value>);
            }

            var resInt = 0;
            var resBool = false;
            if (int.TryParse(value, out resInt))
            {
                return typeof(int);
            }
            else if (bool.TryParse(value, out resBool))
            {
                return typeof(bool);
            }
            else if (parentBlock != null && parentBlock.FindValue(value) != null)
            {
                return parentBlock.FindValue(value).ValueType;
            }
            else if (parentBlock != null && parentBlock.FindMethod(value.PoCut('(')) != null)
            {
                return parentBlock.FindMethod(value.PoCut('(')).ReturnType;
            }
            else if (source.Contains("\""))
            {
                return typeof(string);
            }
            else
            {
                return typeof(object);
            }
        }

        public static string GetFirstCalcSource(string source)
        {
            return Util.Calc.Split(source)[0];
        }
    }
}