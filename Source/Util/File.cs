using System;
using System.IO;
using System.Text;

namespace Sumi.Util
{
    public static class File
    {
        public static string Open(string file)
        {
            string text = "";
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch
            {
                Log.Error("ファイルが見つからなかったなり:{0}", file);
            }
            return text;
        }
    }
}