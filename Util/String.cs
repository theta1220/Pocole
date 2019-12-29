using System.Collections.Generic;

namespace Pocole.Util
{
    public static class String
    {
        public static bool Contains(string text, string chars)
        {
            return text.Contains(chars);
        }

        public static bool ContainsHead(string text, string chars)
        {
            if (text.Length < chars.Length) { return false; }

            for (var i = 0; i < chars.Length; i++)
            {
                if (text[i] != chars[i]) { return false; }
            }
            return true;
        }

        public static bool ContainsAny(string text, params string[] words)
        {
            foreach (var word in words)
            {
                if (Contains(text, word))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny(char target, string chars)
        {
            foreach (var c in chars)
            {
                if (c == target)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny(string source, string chars)
        {
            foreach (var c in source)
            {
                foreach (var target in chars)
                {
                    if (c == target)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string GetFirstSplit(string source, char split)
        {
            return source.Split(split)[0];
        }

        public static string[] SplitOnce(string source, char? splitChar)
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

        // マッチをお尻から検索して一度だけsplitする
        public static string[] SplitOnceTail(string source, char? splitChar)
        {
            if (splitChar == null)
            {
                return new string[] { source };
            }
            var res = new List<string>();
            int match = 0;
            var buf = "";
            for (int i = source.Length - 1; i >= 0; i--)
            {
                var c = source[i];
                if (match == 0 && c == splitChar)
                {
                    match++;
                    res.Add(buf);
                    buf = "";
                    continue;
                }
                if (i == 0)
                {
                    buf = c + buf;
                    res.Add(buf);
                    continue;
                }
                buf = c + buf;
            }
            return res.ToArray();
        }

        public static string[] SplitOnceTail(string source, string splitChar)
        {
            if (!source.Contains(splitChar) || splitChar == "")
            {
                return new string[] { source };
            }
            var res = new List<string>();
            int match = 0;
            var buf = "";
            for (var i = source.Length - 1; i >= 0; i--)
            {
                if (source.Length - i >= splitChar.Length)
                {
                    var find = source.Substring(i, splitChar.Length);
                    if (find == splitChar && match == 0)
                    {
                        match++;
                        res.Add(buf.Substring(splitChar.Length - 1));
                        buf = "";
                        continue;
                    }
                }
                if (i == 0)
                {
                    buf = source[i] + buf;
                    res.Add(buf);
                    buf = "";
                    continue;
                }
                buf = source[i] + buf;
            }
            return res.ToArray();
        }

        public static int MatchCharAnyCount(string source, string chars)
        {
            var res = 0;

            foreach (var c in source)
            {
                foreach (var target in chars)
                {
                    if (c == target)
                    {
                        res++;
                        break;
                    }
                }
            }
            return res;
        }

        public static string[] SplitAny(string source, string chars)
        {
            var res = new List<string>();
            var buf = "";
            var count = 0;
            foreach (var c in source)
            {
                count++;
                if (ContainsAny(c.ToString(), chars) || count == source.Length)
                {
                    if (count == source.Length)
                    {
                        buf += c;
                    }
                    res.Add(buf);
                    buf = "";
                    continue;
                }
                buf += c;
            }
            return res.ToArray();
        }

        public static string Extract(string source, char target)
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

        public static string Extract(string source, char start, char end, bool ignoreString = true)
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
                if (c == start)
                {
                    if (ignoreString)
                    {
                        if (!inString)
                        {
                            blockCount++;

                            if (blockCount == 1)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        blockCount++;

                        if (blockCount == 1)
                        {
                            continue;
                        }
                    }
                }
                if (c == end)
                {

                    if (ignoreString)
                    {
                        if (!inString)
                        {
                            blockCount--;

                            if (blockCount == 0)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        blockCount--;

                        if (blockCount == 0)
                        {
                            break;
                        }
                    }
                }
                if (blockCount > 0)
                {
                    buf += c;
                }
            }

            return buf;
        }

        //! 文字列を意識してスペースや改行を削除してくれる
        public static string Remove(string source, char target)
        {
            var buf = "";
            var inString = false;
            foreach (var c in source)
            {
                if (c == ('"'))
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
    }
}