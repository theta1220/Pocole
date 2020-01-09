using System.Linq;

namespace Pocole
{
    public class Term : Runnable
    {
        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            return true;
        }

        public override void OnEntered()
        {
            var methodName = ExtractMethodName(Source);
            var className = ExtractClassName(Source);

            if (methodName == "SystemCall")
            {
                var system = new SystemCaller();
                if (!system.Initialize(this, Source)) { Log.InitError(); return; }
                Runnables.Add(system);
            }
            else if (IsSetter(Source))
            {
                var setter = new ValueSetter();
                if (!setter.Initialize(this, Source)) { Log.InitError(); return; }
                Runnables.Add(setter);
            }
            else if (IsMethod(Source))
            {
                var caller = new MethodCaller();
                if (!caller.Initialize(this, Source)) { Log.InitError(); return; }
                Runnables.Add(caller);
            }
            else if (GetParentBlock().FindClass(className) != null)
            {
                var instantiator = new ClassInstantiator();
                if (!instantiator.Initialize(this, Source)) { Log.InitError(); return; }
                Runnables.Add(instantiator);
            }
            else
            {
                throw new System.Exception(string.Format("理解できないTerm {0}", Source));
            }
        }

        public override void OnLeaved()
        {
            Runnables.Clear();
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

        public static bool IsSetter(string source)
        {
            return Util.String.RemoveString(source).Contains("=");
        }

        public static bool IsMethod(string source)
        {
            return Util.String.RemoveString(source).Contains("(");
        }

        //! Hoge hoge; のような文字列からクラス名を取り出してくれる
        public static string ExtractClassName(string source)
        {
            return Util.String.Remove(Util.String.Split(source, ' ').First(), ' ');
        }
    }
}