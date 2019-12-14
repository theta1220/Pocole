using System;
using System.Text.RegularExpressions;

namespace Pocole
{
    public class Value
    {
        public enum Type
        {
            Integer,
            String,
        }

        public Type ValueType { get; private set; }
        public string Name { get; private set; }
        public object Object { get; private set; }

        public bool Initialize(string name, string value)
        {
            Name = name;
            Object = value;

            if (Regex.IsMatch(value, "[0-9]"))
            {
                ValueType = Type.Integer;
            }
            else
            {
                ValueType = Type.String;
            }

            switch (ValueType)
            {
                case Type.Integer:
                    Object = int.Parse(value);
                    break;

                case Type.String:
                    Object = value;
                    break;
            }
            return true;
        }

        public void SetValue(object value)
        {
            Object = value;
        }
    }
}