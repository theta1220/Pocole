using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sumi.Util;

namespace Sumi
{
    public class Value
    {
        public Type ValueType
        {
            get
            {
                if (Object == null)
                {
                    return typeof(Class);
                }
                return Object.GetType();
            }
        }
        public string Name { get; set; }
        public object Object { get; set; }

        public Value(string name)
        {
            Name = name;
            Object = null;
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

        public Value(Value other)
        {
            Name = other.Name;
            if (other.Object is Runnable)
            {
                Object = (other.Object as Runnable).Clone();
            }
            else Object = other.Object;
        }

        public static Type GetValueType(string source, Block parentBlock = null)
        {
            var valueName = GetFirstCalcSource(source);
            // 比較
            if (Calc.ContainsCompareOpeartor(source)) return typeof(bool);
            // null
            if (valueName == "null") return typeof(Class);
            // 配列
            if ((valueName.Contains(",") && valueName.Contains('[')) || valueName == "[]") return typeof(List<Value>);
            var resInt = 0;
            var resBool = false;
            if (int.TryParse(valueName, out resInt)) return typeof(int);
            if (bool.TryParse(valueName, out resBool)) return typeof(bool);
            if (valueName.PoMatchHead("\"") && valueName.PoMatchTail("\"")) return typeof(string);
            if (parentBlock != null)
            {
                var v = parentBlock.FindValue(valueName);
                if (v != null) return v.ValueType;
                var func = parentBlock.FindFunction(valueName.PoCut('('));
                if (func != null) return func.ReturnType;
            }
            return typeof(Class);
        }

        public static string GetFirstCalcSource(string source)
        {
            return Util.Calc.Split(source)[0];
        }
    }
}