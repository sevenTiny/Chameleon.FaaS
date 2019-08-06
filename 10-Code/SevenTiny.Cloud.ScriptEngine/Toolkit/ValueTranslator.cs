using System;
using System.Collections.Generic;
using System.Text;

namespace SevenTiny.Cloud.ScriptEngine.Toolkit
{
    internal static class ValueTranslator
    {
        public static bool TrueFalse(int value)
        {
            switch (value)
            {
                case 1: return true;
                default:
                    return false;
            }
        }
    }
}
