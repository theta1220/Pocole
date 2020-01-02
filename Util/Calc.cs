using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Pocole.Util
{
    public class Calc
    {
        private class Node
        {
            public Node Parent { get; private set; }
        }

        public static object Execute(Block parentBlock, string source, System.Type type)
        {
            source = String.Remove(source, ' ');

            // 括弧を計算
            var buf = "";
            var parsed = "";
            int blockCount = 0;
            int count = 0;
            foreach (var c in source)
            {
                count++;
                if (c == '(')
                {
                    blockCount++;
                    if (blockCount == 1)
                    {
                        parsed += buf;
                        buf = "";
                    }
                    continue;
                }
                if (c == ')')
                {
                    blockCount--;
                    if (blockCount == 0)
                    {
                        if (buf.Length == 0)
                        {
                            continue;
                        }
                        var value = Execute(parentBlock, buf, type);
                        parsed += value.ToString();
                        buf = "";
                    }
                    continue;
                }
                if (c == ';')
                {
                    parsed += buf;
                    continue;
                }
                if (count == source.Length)
                {
                    parsed += buf + c;
                    continue;
                }
                buf += c;
            }

            var splitChar = GetNextOperator(parsed);
            var split = String.SplitOnceTail(parsed, splitChar);

            // 変数
            var findValue = parentBlock.FindValue(parsed);
            if (findValue != null)
            {
                return findValue.Object;
            }

            if (type == typeof(int))
            {
                if (splitChar != "")
                {
                    if (split.Length != 2)
                    {
                        Log.Error("右辺と左辺がないと計算できない");
                        return null;
                    }

                    try
                    {
                        var ans = 0;
                        var r = (int)Execute(parentBlock, split[1], type);
                        var l = (int)Execute(parentBlock, split[0], type);
                        if (splitChar == "+") ans = r + l;
                        if (splitChar == "-") ans = r - l;
                        if (splitChar == "*") ans = r * l;
                        if (splitChar == "/") ans = r / l;
                        return ans;
                    }
                    catch
                    {
                        Log.Debug("splitChar:{0}", splitChar);
                        foreach (var sp in split)
                        {
                            Log.Debug(sp.ToString());
                        }
                        if (parentBlock != null)
                        {
                            var value = parentBlock.FindValue(parsed);
                            if (value != null)
                            {
                                Log.Debug("value:{0}", value.Name);
                            }
                        }
                        throw new System.Exception(string.Format("計算中に\"{0}\"をintにParseしようとして失敗", parsed));
                    }
                }
                else
                {
                    try
                    {
                        return int.Parse(parsed);
                    }
                    catch
                    {
                        throw new System.Exception(string.Format("\"{0}\"をintにParseしようとして失敗", parsed));
                    }
                }
            }
            if (type == typeof(bool))
            {
                if (ContainsCompareOperator(splitChar))
                {
                    if (split.Length != 2)
                    {
                        Log.Error("右辺と左辺がないと計算できない");
                        return null;
                    }
                    var ans = false;
                    var rType = Value.GetValueType(split[1], parentBlock);
                    var lType = Value.GetValueType(split[0], parentBlock);
                    // 型の違う者同士は比較しないことにする
                    if (rType != lType)
                    {
                        return false;
                    }
                    var r = Execute(parentBlock, split[1], rType);
                    var l = Execute(parentBlock, split[0], lType);
                    if (rType == typeof(int))
                    {
                        if (splitChar == "==") ans = (int)r == (int)l;
                        else if (splitChar == "!=") ans = (int)r != (int)l;
                        else if (splitChar == "<") ans = (int)r < (int)l;
                        else if (splitChar == ">") ans = (int)r > (int)l;
                        else if (splitChar == "<=") ans = (int)r <= (int)l;
                        else if (splitChar == ">=") ans = (int)r >= (int)l;
                        else { Log.Error("{0}型で 比較できる演算子ではない:{1}", rType.ToString(), splitChar); return false; }
                    }
                    else if (rType == typeof(string))
                    {
                        if (splitChar == "==") ans = (string)r == (string)l;
                        else if (splitChar == "!=") ans = (string)r != (string)l;
                        else { Log.Error("{0}型で 比較できる演算子ではない:{1}", rType.ToString(), splitChar); return false; }
                    }
                    else
                    {
                        Log.Error("{0}型は、そもそも比較できない", rType.ToString());
                        return false;
                    }

                    return ans;
                }
                else
                {
                    Log.Error("比較演算子として厳しいです:{0} split:{1}", splitChar, String.ArrayToString(split));
                    throw new System.Exception(string.Format("比較演算子として正しくないものが渡ってきました{0}", splitChar));
                }
            }
            if (type == typeof(string))
            {
                if (splitChar == "+") return (string)Execute(parentBlock, split[1], type) + (string)Execute(parentBlock, split[0], type);
                else return Util.String.Extract(parsed, '"');
            }
            else
            {
                throw new System.Exception(string.Format("理解できないtypeを演算しようとした:{0}", source));
            }
        }

        public static string GetNextOperator(string source)
        {
            var reverse = new string(source.Reverse().ToArray());

            if (source.Contains("==")) return "==";
            if (source.Contains("!=")) return "!=";
            if (source.Contains(">=")) return ">=";
            if (source.Contains("<=")) return "<=";
            if (source.Contains(">")) return ">";
            if (source.Contains("<")) return "<";

            if (String.ContainsAny(reverse, "+-"))
            {
                foreach (var c in reverse)
                {
                    if (String.ContainsAny(c, "+-"))
                    {
                        return c.ToString();
                    }
                }
            }
            if (String.ContainsAny(reverse, "*/"))
            {
                foreach (var c in reverse)
                {
                    if (String.ContainsAny(c, "*/"))
                    {
                        return c.ToString();
                    }
                }
            }
            return "";
        }

        public static bool ContainsCompareOperator(string source)
        {
            if (source.Contains("==") ||
            source.Contains("!=") ||
            source.Contains("<=") ||
            source.Contains(">=") ||
            source.Contains("<") ||
            source.Contains(">"))
            {
                return true;
            }
            return false;
        }
    }
}