using Sumi.Util;
using System;

namespace Sumi
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

        public override Runnable Clone() { return new ValueSetter(this); }

        protected override void Run()
        {
            Value target = null;
            if (ValueSetterType == ValueSetterType.Declare)
            {
                target = new Value(Name, null);
            }
            else
            {
                target = GetParentBlock().FindValue(Name);
                Log.Assert(target != null, "変数が見つかりませんでした:{0}", Name);
            }
            var valueType = Value.GetValueType(Formula, GetParentBlock());
            var res = Util.Calc.Execute(GetParentBlock(), Formula, valueType);
            target.Object = res.Object;
            if (ValueSetterType == ValueSetterType.Declare)
            {
                GetParentBlock().AddValue(target);
            }
        }
    }
}