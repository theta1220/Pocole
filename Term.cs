using System.Linq;

namespace Pocole
{
    public class Term : Runnable
    {
        public new bool Initialize(Block parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            return true;
        }

        public override void OnEntered()
        {
            var methodName = ExtractMethodName(Source);
            var valueName = ExtractValueName(Source);

            if (methodName == "SystemCall")
            {
                var system = new SystemCaller();
                if (!system.Initialize(Parent, Source)) { Log.InitError(); return; }
                Runnables.Add(system);
            }
            else if (Parent.FindMethod(methodName) != null)
            {
                var caller = new MethodCaller();
                if (!caller.Initialize(Parent, Source)) { Log.InitError(); return; }
                Runnables.Add(caller);
            }
            else if (Parent.FindValue(valueName) != null)
            {
                var setter = new ValueSetter();
                if (!setter.Initialize(Parent, Source)) { Log.InitError(); return; }
                Runnables.Add(setter);
            }
            else
            {
                Log.Error("読めないコード:{0}", Source);
                return;
            }
        }

        //! ... Hoge(args)... のような文字列から メソッド名を取り出してくれる
        public static string ExtractMethodName(string source)
        {
            var buf = Util.String.Split(Util.String.Substring(source, '('), ' ');
            return buf.Last();
        }

        //! var hoge = foo; とか hoge = foo; のような文字列から 変数名を取り出してくれる
        public static string ExtractValueName(string source)
        {
            var buf = Util.String.Split(Util.String.Substring(source, '='), ' ');
            return Util.String.Remove(buf.Last(), ' ');
        }
    }
}