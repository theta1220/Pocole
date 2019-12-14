using System;
using System.Collections.Generic;

namespace Pocole
{
    public class Loader
    {
        public Runnable Load(string file)
        {
            var text = Util.File.Open(file);
            text = RemoveExtraText(text);
            var block = new Block();
            if (!block.Initialize(null, text)) { return null; }
            return block;
        }

        public string RemoveExtraText(string text)
        {
            text = _RemoveIndent(text);
            text = _RemoveOnlyNewLine(text);
            text = _RemoveComment(text);
            return text;
        }

        private string _RemoveComment(string text)
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
                var replaced = line.Replace("\n", "");
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

        private string _RemoveOnlyNewLine(string text)
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

        private string _RemoveIndent(string text)
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
    }
}