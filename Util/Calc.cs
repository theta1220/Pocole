using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Pocole.Util
{
    public class Calc
    {
        public static object Execute(Block parentBlock, string source, System.Type type)
        {
            source = String.Remove(source, ' ');

            // 関数を計算
            var parsed = ExecuteMethod(parentBlock, source, type);

            // 括弧を計算
            parsed = ExecuteCalc(parentBlock, parsed, type);

            // 変数を探す
            if (!Util.String.ContainsAny(source, "+-*/"))
            {
                // 生の配列
                var removeStringStr = Util.String.RemoveString(parsed);
                if (removeStringStr.Length > 0 && Util.String.MatchHead("[", removeStringStr))
                {
                    return ExecuteArrayString(parentBlock, parsed);
                }
                var findValue = parentBlock.FindValue(parsed);
                if (findValue != null) return findValue.Object;
            }

            if (type == typeof(int)) return ExecuteCalcInt(parentBlock, parsed);
            if (type == typeof(bool)) return ExecuteCalcBool(parentBlock, parsed);
            if (type == typeof(string)) return ExecuteCalcString(parentBlock, parsed);

            throw new System.Exception(string.Format("理解できない計算式を演算しようとした:{0}", source));
        }

        public static string ExecuteMethod(Block parentBlock, string source, System.Type type)
        {
            source = Util.String.Remove(source, ' ');
            var formulas = Util.String.SplitAny(source, "+-*/");

            foreach (var formula in formulas)
            {
                if (IsMethodString(formula))
                {
                    var caller = new MethodCaller();
                    if (!caller.Initialize(parentBlock, formula)) { Log.InitError(); return ""; }
                    caller.ForceExecute();
                    source = source.Replace(formula, caller.Method.ReturnedValue.ToString());
                }
            }
            return source;
        }

        public static string ExecuteCalc(Block parentBlock, string source, System.Type type)
        {
            // 括弧があるってことは優先して計算すべき箇所があるってこと
            while (Util.String.RemoveString(source).Contains("("))
            {
                Log.Debug(source);
                var ext = Util.String.Extract(source, '(', ')', true);
                source = source.Replace(ext, Execute(parentBlock, Util.String.Extract(source, '(', ')'), type).ToString());
            }
            return source;
        }

        private static int ExecuteCalcInt(Block parentBlock, string source)
        {
            var ope = GetNextOperator(source);
            var splitedFormula = String.SplitOnceTail(source, ope);
            if (ope != "")
            {
                var r = (int)Execute(parentBlock, splitedFormula[1], typeof(int));
                var l = (int)Execute(parentBlock, splitedFormula[0], typeof(int));
                if (ope == "+") return r + l;
                if (ope == "-") return r - l;
                if (ope == "*") return r * l;
                if (ope == "/") return r / l;
            }
            return int.Parse(source);
        }

        private static bool ExecuteCalcBool(Block parentBlock, string source)
        {
            var ope = GetNextOperator(source);
            var splitedFormula = String.SplitOnceTail(source, ope);
            if (ContainsCompareOperator(ope))
            {
                var rType = Value.GetValueType(splitedFormula[1], parentBlock);
                var lType = Value.GetValueType(splitedFormula[0], parentBlock);

                // 型の違う者同士は比較しないことにする
                if (rType != lType) return false;

                var r = Execute(parentBlock, splitedFormula[1], rType);
                var l = Execute(parentBlock, splitedFormula[0], lType);
                if (rType == typeof(int))
                {
                    if (ope == "==") return (int)r == (int)l;
                    else if (ope == "!=") return (int)r != (int)l;
                    else if (ope == "<") return (int)r < (int)l;
                    else if (ope == ">") return (int)r > (int)l;
                    else if (ope == "<=") return (int)r <= (int)l;
                    else if (ope == ">=") return (int)r >= (int)l;
                    else { Log.Error("{0}型で 比較できる演算子ではない:{1}", rType.ToString(), ope); return false; }
                }
                else if (rType == typeof(string))
                {
                    if (ope == "==") return (string)r == (string)l;
                    else if (ope == "!=") return (string)r != (string)l;
                    else { Log.Error("{0}型で 比較できる演算子ではない:{1}", rType.ToString(), ope); return false; }
                }
                else
                {
                    Log.Error("{0}型は、そもそも比較できない", rType.ToString());
                    return false;
                }
            }
            return bool.Parse(source);
        }

        private static string ExecuteCalcString(Block parentBlock, string source)
        {
            var ope = GetNextOperator(source);
            var splitedFormula = String.SplitOnceTail(source, ope);
            if (ope == "+") return (string)Execute(parentBlock, splitedFormula[1], typeof(string)) + (string)Execute(parentBlock, splitedFormula[0], typeof(string));
            return Util.String.Extract(source, '"');
        }

        private static List<Value> ExecuteArrayString(Block parentBlock, string source)
        {
            var split = Util.String.Split(Util.String.Extract(source, '[', ']'), ',');
            var list = new List<Value>();
            int index = 0;
            foreach (var objSrc in split)
            {
                var obj = Execute(parentBlock, objSrc, Value.GetValueType(objSrc, parentBlock));
                var value = new Value();
                if (!value.Initialize(index.ToString())) { Log.InitError(); return null; }
                value.SetValue(obj);
                list.Add(value);
                index++;
            }
            return list;
        }

        private static string GetNextOperator(string source)
        {
            source = Util.String.RemoveString(source);
            var reverse = new string(source.Reverse().ToArray());

            if (source.Contains("==")) return "==";
            if (source.Contains("!=")) return "!=";
            if (source.Contains(">=")) return ">=";
            if (source.Contains("<=")) return "<=";
            if (source.Contains(">")) return ">";
            if (source.Contains("<")) return "<";

            if (String.ContainsAny(reverse, "+-"))
                foreach (var c in reverse)
                    if (String.ContainsAny(c, "+-")) return c.ToString();

            if (String.ContainsAny(reverse, "*/"))
                foreach (var c in reverse)
                    if (String.ContainsAny(c, "*/")) return c.ToString();

            return "";
        }

        private static bool ContainsCompareOperator(string source)
        {
            if (source.Contains("==") ||
            source.Contains("!=") ||
            source.Contains("<=") ||
            source.Contains(">=") ||
            source.Contains("<") ||
            source.Contains(">")) return true;

            return false;
        }

        private static bool IsMethodString(string source)
        {
            var buf = Util.String.Substring(source, '(');
            if (buf.Length > 0 && Util.String.Contains(source, "("))
            {
                return true;
            }
            return false;
        }
    }
}