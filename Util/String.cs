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

        public static string GetFirstSplit(string source, char split)
        {
            return source.Split(split)[0];
        }
    }
}