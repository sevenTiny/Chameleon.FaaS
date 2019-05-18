using System.Text.RegularExpressions;

namespace Seventiny.Cloud.ScriptEngine.Toolkit
{
    internal static class ScriptExtension
    {
        private static string emptyLinePattern = @"^\s*$\n|\r";
        private static string commentPattern = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
        private static Regex commentRgx = new Regex(commentPattern);
        private static Regex emptyLineRgx = new Regex(emptyLinePattern, RegexOptions.Multiline);

        public static string ClearScript(this string script)
        {
            return RemoveEmptyLines(StripComments(script).Trim()).Trim();
        }

        public static string StripComments(this string script)
        {
            return commentRgx.Replace(script, "$1");
        }

        public static string RemoveEmptyLines(this string script)
        {
            return emptyLineRgx.Replace(script, "");
        }
    }
}
