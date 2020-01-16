using Pocole.Util;
using System;

namespace Pocole
{
    public enum ValueSetterType
    {
        Invalid,
        Declare,
        Assign,
    }

    [System.Serializable]
    public class ValueSetter : Runnable
    {
        public ValueSetterType ValueSetterType { get; private set; } = ValueSetterType.Invalid;
        public string Name { get; private set; }
        public string Formula { get; private set; }

        public ValueSetter(Runnable parent, string source) : base(parent, source)
        {
            var name = source.Split(' ')[0];

            // 宣言
            if (name == "var")
            {
                var buf = source.PoSplitOnce(' ')[1].PoRemove(' ').Split('=');
                Name = buf[0];
                Formula = buf[1];
                ValueSetterType = ValueSetterType.Declare;
            }
            // 代入
            else
            {
                var buf = source.PoRemove(' ').Split('=');
                Name = buf[0];
                Formula = buf[1];
                ValueSetterType = ValueSetterType.Assign;
            }
        }

        public ValueSetter(ValueSetter other) : base(other)
        {
            ValueSetterType = other.ValueSetterType;
            Name = other.Name;
            Formula = other.Formula;
        }

        public override object Clone() { return new ValueSetter(this); }

        protected override void Run()
        {
            Value target = null;
            if (ValueSetterType == ValueSetterType.Declare)
            {
                target = new Value(Name);
                GetParentBlock().AddValue(target);
            }
            else if (ValueSetterType == ValueSetterType.Assign)
            {
                target = GetParentBlock().FindValue(Name);
                if (target == null)
                {
                    Log.Error("変数が見つかりませんでした:{0}", Name);
                }
            }
            else
            {
                Log.Error("ValueSetterTypeがInvalid");
            }
            var valueType = Value.GetValueType(Formula, GetParentBlock());
            var res = Util.Calc.Execute(GetParentBlock(), Formula, valueType);
            target.Object = res.Object;
        }
    }
}