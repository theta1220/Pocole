namespace Pocole
{
    public enum ValueSetterType
    {
        Invalid,
        Declare,
        Assign,
    }

    public class ValueSetter : Runnable
    {
        public ValueSetterType ValueSetterType { get; private set; } = ValueSetterType.Invalid;
        public string Name { get; private set; }
        public string Formula { get; private set; }

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            var name = source.Split(' ')[0];

            // 宣言
            if (name == "var")
            {
                var buf = Util.String.SplitOnce(source, ' ')[1].Replace(" ", "").Split('=');
                Name = buf[0];
                Formula = buf[1];
                ValueSetterType = ValueSetterType.Declare;
            }
            // 代入
            else
            {
                var buf = source.Replace(" ", "").Split('=');
                Name = buf[0];
                Formula = buf[1];
                ValueSetterType = ValueSetterType.Assign;
            }
            return true;
        }

        protected override void Run()
        {
            Value target = null;
            if (ValueSetterType == ValueSetterType.Declare)
            {
                target = new Value();
                if (!target.Initialize(Name)) { Log.InitError(); return; }
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
            var value = Util.Calc.Execute(GetParentBlock(), Formula, valueType);
            target.SetValue(value);
        }
    }
}