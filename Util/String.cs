using System.Collections.Generic;
using System.Linq;

namespace Pocole.Util
{
    public static class String
    {
        public static bool PoMatchTail(this string source, string[] patterns, out string match)
        {
            match = "";
            foreach (var pattern in patterns)
            {
                if (source.PoMatchTail(pattern))
                {
                    match = pattern;
                    return true;
                }
            }
            return false;
        }

        public static bool PoMatchTail(this string source, string pattern)
        {
            if (source.Length < pattern.Length)
            {
                return false;
            }
            source = new string(source.Reverse().ToArray());
            pattern = new string(pattern.Reverse().ToArray());

            for (var i = 0; i < pattern.Length; i++)
            {
                if (source[i] != pattern[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static string[] PoSplitOnce(this string source, char? splitChar)
        {
            if (splitChar == null)
            {
                return new string[] { source };
            }

            var res = new List<string>();
            int match = 0;
            var buf = "";
            var i = 0;
            foreach (var c in source)
            {
                i++;
                if (match == 0 && c == splitChar)
                {
                    match++;
                    res.Add(buf);
                    buf = "";
                    continue;
                }
                if (i == source.Length)
                {
                    buf += c;
                    res.Add(buf);
                    continue;
                }
                buf += c;
            }
            return res.ToArray();
        }

        public static string PoExtract(this string source, char target)
        {
            var buf = "";
            var blockCount = 0;

            foreach (var c in source)
            {
                if (c == target)
                {
                    if (blockCount == 1)
                    {
                        return buf;
                    }
                    blockCount++;

                    if (blockCount == 1)
                    {
                        continue;
                    }
                }
                if (blockCount > 0)
                {
                    buf += c;
                }
            }
            return buf;
        }

        public static string PoExtract(this string source, char start, char end, bool withBracket = false)
        {
            var buf = "";
            var blockCount = 0;
            var inString = false;
            foreach (var c in source)
            {
                if (c == '"')
                {
                    inString = !inString;
                }
                if (c == start && !inString)
                {
                    blockCount++;

                    if (blockCount == 1 && !withBracket)
                    {
                        continue;
                    }
                }
                if (c == end && !inString)
                {
                    blockCount--;

                    if (blockCount == 0)
                    {
                        if (withBracket)
                        {
                            buf += c;
                        }
                        break;
                    }
                }
                if (blockCount > 0)
                {
                    buf += c;
                }
            }

            return buf;
        }

        //! 文字列の部分を削除
        public static string PoRemoveString(this string source)
        {
            var buf = "";
            var isString = false;
            foreach (var c in source)
            {
                if (c == '"')
                {
                    isString = !isString;
                    continue;
                }
                if (isString)
                {
                    continue;
                }
                buf += c;
            }
            return buf;
        }

        //! 文字列を意識してスペースや改行を削除してくれる
        public static string PoRemove(this string source, char target)
        {
            var buf = "";
            var inString = false;
            foreach (var c in source)
            {
                if (c == '"')
                {
                    inString = !inString;
                }
                if (!inString && c == target)
                {
                    continue;
                }
                buf += c;
            }
            return buf;
        }

        //! 文字列を意識してsplitしてくれる
        public static string[] PoSplit(this string source, char split)
        {
            var list = new List<string>();
            var buf = "";
            var isString = false;
            foreach (var c in source)
            {
                if (c == '"')
                {
                    isString = !isString;
                }
                if (!isString && c == split)
                {
                    list.Add(buf);
                    buf = "";
                    continue;
                }
                buf += c;
            }
            if (buf != "")
            {
                list.Add(buf);
            }
            return list.ToArray();
        }

        public static string ToString<T>(this T[] arr)
        {
            var str = "\n";
            var count = 0;
            foreach (var elm in arr)
            {
                str += string.Format("{0}:{1}", count, elm.ToString());
                if (arr.Length > count + 1)
                {
                    str += '\n';
                }
                count++;
            }
            return str;
        }

        public static string ToString<T>(this List<T> arr)
        {
            return arr.ToArray().ToString();
        }

        //! Blockが読みたい単位で分割してくれる
        public static string[] PoSplitSource(this string source)
        {
            bool inString = false;
            int blockCount = 0;
            int bracketCount = 0;
            var list = new List<string>();
            var buf = "";
            var count = 0;

            foreach (var c in source)
            {
                count++;
                if (c == '"') { inString = !inString; }
                if (c == '{') { blockCount++; }
                if (c == '}') { blockCount--; }
                if (c == '(') { bracketCount++; }
                if (c == ')') { bracketCount--; }

                if (!inString && c == '\n') { continue; }

                if (!inString && bracketCount == 0 && (blockCount == 0 && (c == '}' || c == ';')) || count >= source.Length)
                {
                    if (c == ';') { list.Add(buf); }
                    else { list.Add(buf + c); }

                    buf = "";
                    continue;
                }
                buf += c;
            }
            return list.ToArray();
        }

        //! 先頭がパターンとマッチするか
        public static bool PoMatchHead(this string source, string pattern)
        {
            for (var i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] != source[i])
                {
                    return false;
                }
            }
            return true;
        }

        //! ある文字がヒットするまでの文字を切り取ってくれる （結果にmarkは含まない)
        public static string PoCut(this string source, char mark)
        {
            var res = "";
            foreach (var c in source)
            {
                if (c == mark)
                {
                    return res;
                }
                res += c;
            }
            return res;
        }

        //! どの文字が先にヒットした？
        public static char? PoFirstHit(this string source, char[] chars)
        {
            foreach (var c in source)
            {
                if (PoMatchAny(c, chars)) return c;
            }
            return null;
        }

        public static bool PoMatchAny(this char c, char[] chars)
        {
            foreach (var target in chars)
            {
                if (c == target) return true;
            }
            return false;
        }

        public static bool PoMatchAny(this string source, string[] patterns)
        {
            foreach (var pattern in patterns)
            {
                if (source == pattern) return true;
            }
            return false;
        }

        public static string GetIndentSpace(int count)
        {
            var space = "";
            for (var i = 0; i < count; i++)
            {
                space += "    ";
            }
            return space;
        }

    }
}