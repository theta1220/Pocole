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
            source = source.Replace(" ", "");

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
                    var ans = 0;
                    var r = (int)Execute(parentBlock, split[1], type);
                    var l = (int)Execute(parentBlock, split[0], type);
                    if (splitChar == "+") ans = r + l;
                    if (splitChar == "-") ans = r - l;
                    if (splitChar == "*") ans = r * l;
                    if (splitChar == "/") ans = r / l;
                    return ans;
                }
                else
                {
                    try
                    {
                        return int.Parse(parsed);
                    }
                    catch
                    {
                        Log.Error("\"{0}\"をintにParseしようとして失敗", parsed);
                        return null;
                    }
                }
            }
            if (type == typeof(string))
            {
                if (splitChar == "+") return (string)Execute(parentBlock, split[0], type) + (string)Execute(parentBlock, split[1], type);
                else return Util.String.Extract(parsed, '"');
            }
            else
            {
                Log.Error("え？");
                return null;
            }
        }

        private static string GetNextOperator(string source)
        {
            source = new string(source.Reverse().ToArray());

            if (source.Contains("=="))
            {
                return "==";
            }
            if (String.ContainsAny(source, "+-"))
            {
                foreach (var c in source)
                {
                    if (String.ContainsAny(c, "+-"))
                    {
                        return c.ToString();
                    }
                }
            }
            if (String.ContainsAny(source, "*/"))
            {
                foreach (var c in source)
                {
                    if (String.ContainsAny(c, "*/"))
                    {
                        return c.ToString();
                    }
                }
            }
            return "";
        }
    }
}