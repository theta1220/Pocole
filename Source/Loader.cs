using System;
using System.Linq;
using System.Collections.Generic;

namespace Sumi
{
    public class Loader
    {
        public Loader()
        {
        }

        public Runnable Load(string name, string source)
        {
            source = RemoveExtraText(source);
            source = "class " + GetName(name) + "{" + source + "}";
            Class.Clear();
            var block = new Class(null, source);
            block.Extend();
            block.ClassTest();
            Class.ExtensionTest();
            return block;
        }

        public string RemoveExtraText(string text)
        {
            text = RemoveIndent(text);
            text = RemoveOnlyNewLine(text);
            text = RemoveComment(text);
            return text;
        }

        private string RemoveComment(string text)
        {
            var lines = text.Split('\n');
            var res = "";
            var i = 0;
            foreach (var line in lines)
            {
                if (line.Length >= 2 && line[0] == '/' && line[1] == '/')
                {
                    continue;
                }
                var replaced = line.Replace("\n", "").Replace("\r", "");
                if (i + 1 < lines.Length)
                {
                    res += replaced + '\n';
                }
                else
                {
                    res += replaced;
                }
                i++;
            }
            return res;
        }

        private string RemoveOnlyNewLine(string text)
        {
            var res = "";
            var prev = '\n';
            foreach (var c in text)
            {
                if (c == '\n' && prev == '\n')
                {
                    continue;
                }
                res += c;
                prev = c;
            }
            return res;
        }

        private string RemoveIndent(string text)
        {
            var lines = text.Split('\n');
            var resLines = new List<string>();

            foreach (var line in lines)
            {
                var res = "";
                bool isIndent = true;
                foreach (var c in line)
                {
                    if (isIndent)
                    {
                        if (c == ' ')
                        {
                            continue;
                        }
                        else
                        {
                            isIndent = false;
                        }
                    }
                    res += c;
                }
                res += '\n';
                resLines.Add(res);
            }

            var resText = "";
            foreach (var line in resLines)
            {
                resText += line;
            }
            return resText;
        }

        private string GetName(string file)
        {
            return file.Split('/').Last().Split('.').First();
        }
    }
}