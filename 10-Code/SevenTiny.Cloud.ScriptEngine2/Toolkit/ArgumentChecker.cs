using System;
using System.Collections.Generic;
using System.Text;

namespace SevenTiny.Cloud.ScriptEngine.Toolkit
{
    internal static class ArgumentChecker
    {
        public static void NotNullOrEmpty(string arg, string argName)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentNullException(argName, $"argument can not be null or empty.");
            }
        }
    }
}
