using System;
using System.Text.RegularExpressions;

namespace Pocole
{
    [Serializable]
    public class Value
    {
        public Type ValueType { get { return Object.GetType(); } }
        public string Name { get; private set; }
        public object Object { get; private set; }

        public bool Initialize()
        {
            return true;
        }

        public bool Initialize(string name)
        {
            Name = name;
            return true;
        }

        public bool Initialize(string name, string value)
        {
            if (!Initialize(name)) { Log.InitError(); return false; }
            Object = value;

            if (ValueType == typeof(int))
            {
                Object = int.Parse(value);
            }
            else
            {
                Object = value;
            }
            return true;
        }

        public void SetValue(object value)
        {
            Object = value;
        }

        public static Type GetValueType(string source, Block parentBlock = null)
        {
            source = Util.String.Remove(source, ' ');
            var value = Util.String.SplitAny(source, "+-*/");

            var resInt = 0;
            var resBool = false;
            if (int.TryParse(value[0], out resInt))
            {
                return typeof(int);
            }
            if (bool.TryParse(value[0], out resBool))
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
            else
            {
                return typeof(string);
            }
        }
    }
}