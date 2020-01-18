using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Pocole.Util
{
    public class Calc
    {
        public static string[] Operators = new[] { "+", "-", "*", "/", "==", "&&", "||", "!=", "<=", ">=", "<", ">" };
        public static string[] CompareOpes = new[] { "==", "!=", "&&", "||", "<=", ">=", "<", ">" };

        public static Value Execute(Block parentBlock, string source, System.Type type)
        {
            source = String.PoRemove(source, ' ');

            var splitedFormula = Split(source);

            // 単一項のときは関数か変数か配列か実数（実数はこのifでは処理されない)
            if (splitedFormula.Length == 1)
            {
                var formula = splitedFormula[0];

                if (formula.Length >= 2 && formula.First() == '[' && formula.Last() == ']')
                {
                    return new Value("", ExecuteArray(parentBlock, formula));
                }

                var value = parentBlock.FindValue(formula);
                if (value != null) return value;
            }
            // 括弧を計算
            source = ExecuteBracketCalc(parentBlock, source, type);

            if (source == "null") return null;
            if (type == typeof(int)) return new Value("", ExecuteCalcInt(parentBlock, source));
            if (type == typeof(bool)) return new Value("", ExecuteCalcBool(parentBlock, source));
            if (type == typeof(string)) return new Value("", ExecuteCalcString(parentBlock, source));

            parentBlock.GetParentBlock().PrintBlockTree();

            throw new System.Exception(string.Format("理解できない計算式を演算しようとした:{0} / {1}", source, type.ToString()));
        }

        public static string ExecuteBracketCalc(Block parentBlock, string source, System.Type type)
        {
            var splitedFormula = Split(source);
            var buf = "";
            foreach (var c in source)
            {
                buf += c;
                if (source.Length > 2 && buf.PoMatchAny(splitedFormula) && source.First() == '(' && source.Last() == ')')
                {
                    buf = buf.Replace(source, Execute(parentBlock, source.PoExtract('(', ')'), type).Object.ToString());
                }
            }
            return buf;
        }

        private static int ExecuteCalcInt(Block parentBlock, string source)
        {
            var ope = GetNextOperator(source);
            var splitedFormula = SplitFormula(source, ope);
            if (ope != "")
            {
                var l = (int)Execute(parentBlock, splitedFormula[0], typeof(int)).Object;
                var r = (int)Execute(parentBlock, splitedFormula[1], typeof(int)).Object;
                if (ope == "+") return l + r;
                if (ope == "-") return l - r;
                if (ope == "*") return l * r;
                if (ope == "/") return l / r;
            }
            return int.Parse(source);
        }

        private static bool ExecuteCalcBool(Block parentBlock, string source)
        {
            var ope = GetNextOperator(source);
            var splitedFormula = SplitFormula(source, ope);
            if (ope.PoMatchAny(CompareOpes))
            {
                var lType = Value.GetValueType(splitedFormula[0], parentBlock);
                var rType = Value.GetValueType(splitedFormula[1], parentBlock);

                // 型の違う者同士は比較しないことにする
                if (rType != lType) return false;

                var l = Execute(parentBlock, splitedFormula[0], lType).Object;
                var r = Execute(parentBlock, splitedFormula[1], rType).Object;
                if (rType == typeof(int))
                {
                    if (ope == "==") return (int)l == (int)r;
                    else if (ope == "!=") return (int)l != (int)r;
                    else if (ope == "<") return (int)l < (int)r;
                    else if (ope == ">") return (int)l > (int)r;
                    else if (ope == "<=") return (int)l <= (int)r;
                    else if (ope == ">=") return (int)l >= (int)r;
                    else { Log.Error("{0}型で 比較できる演算子ではない:{1}", rType.ToString(), ope); return false; }
                }
                else if (rType == typeof(string))
                {
                    if (ope == "==") return (string)l == (string)r;
                    else if (ope == "!=") return (string)l != (string)r;
                    else { Log.Error("{0}型で 比較できる演算子ではない:{1}", rType.ToString(), ope); return false; }
                }
                else if (rType == typeof(bool))
                {
                    if (ope == "&&") return (bool)l && (bool)r;
                    if (ope == "||") return (bool)l || (bool)r;
                    throw new System.Exception("比較できない演算子");
                }
                else
                {
                    if (ope == "==") return l == r;
                    if (ope == "!=") return l != r;

                    throw new System.Exception("比較できない");
                }
            }
            return bool.Parse(source);
        }

        private static string ExecuteCalcString(Block parentBlock, string source)
        {
            var ope = GetNextOperator(source);
            var splitedFormula = SplitFormula(source, ope);
            if (ope == "+") return (string)Execute(parentBlock, splitedFormula[0], typeof(string)).Object +
                                    (string)Execute(parentBlock, splitedFormula[1], typeof(string)).Object;
            return source.PoExtract('"');
        }

        private static List<Value> ExecuteArray(Block parentBlock, string source)
        {
            var split = source.PoExtract('[', ']').PoSplit(',');
            var list = new List<Value>();
            int index = 0;
            foreach (var objSrc in split)
            {
                var value = Execute(parentBlock, objSrc, Value.GetValueType(objSrc, parentBlock));
                value.Name = index.ToString();
                list.Add(value);
                index++;
            }
            return list;
        }

        private static string GetNextOperator(string source)
        {
            source = source.PoRemoveString();
            foreach (var formula in Split(source))
            {
                source = source.Replace(formula, " ");
            }
            var opes = source.PoSplit(' ');
            if (opes.Length == 0) return "";

            foreach (var ope in opes)
                if (ope.PoMatchAny(CompareOpes)) return ope;

            foreach (var ope in opes)
                if (ope.PoMatchAny(new[] { "+", "-" })) return ope;

            foreach (var ope in opes)
                if (ope.PoMatchAny(new[] { "*", "/" })) return ope;

            return "";
        }

        public static string[] Split(string source)
        {
            var list = new List<string>();
            var buf = "";
            var blockCount = 0;
            foreach (var c in source)
            {
                buf += c;

                if (c == '(' || c == '[') blockCount++;
                if (c == ')' || c == ']') blockCount--;

                var ope = "";
                if (blockCount == 0 && buf.PoMatchTail(Operators, out ope))
                {
                    buf = buf.Replace(ope, "");
                    list.Add(buf);
                    buf = "";
                    continue;
                }
            }
            if (buf != "") list.Add(buf);
            return list.ToArray();
        }

        private static string[] SplitFormula(string source, string ope)
        {
            if (ope.Length == 0) { return new string[] { source }; }

            var list = new List<string>();
            var buf = "";
            var blockCount = 0;
            bool matched = false;
            foreach (var c in source)
            {
                if (matched)
                {
                    buf += c;
                    continue;
                }

                buf += c;

                if (c == '(' || c == '[') blockCount++;
                if (c == ')' || c == ']') blockCount--;

                if (blockCount == 0 && buf.Contains(ope))
                {
                    buf = buf.Replace(ope, "");
                    list.Add(buf);
                    buf = "";
                    matched = true;
                    continue;
                }
            }
            if (buf != "") list.Add(buf);
            return list.ToArray();
        }
    }
}