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

        public new bool Initialize(Block parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            try
            {
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
            }
            catch (System.Exception e)
            {
                Log.Error(e.Message);
                return false;
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
                Parent.AddValue(target);
            }
            else if (ValueSetterType == ValueSetterType.Assign)
            {
                target = Parent.FindValue(Name);
            }
            else
            {
                Log.Error("ValueSetterTypeがInvalid");
            }
            var valueType = Value.GetValueType(Formula, Parent);
            var value = Util.Calc.Execute(Parent, Formula, valueType);
            target.SetValue(value, valueType);
        }
    }
}