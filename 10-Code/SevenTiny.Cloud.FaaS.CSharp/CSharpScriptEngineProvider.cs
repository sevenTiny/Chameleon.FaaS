using System;
using System.Collections.Generic;
using System.Text;
using SevenTiny.Bantina;

namespace SevenTiny.Cloud.FaaS.CSharp
{
    public class CSharpScriptEngineProvider : IScriptEngineProvider
    {
        public Result CheckScript(DynamicScriptBase dynamicScript)
        {
            throw new NotImplementedException();
        }

        public Result<T> RunScript<T>(DynamicScriptBase dynamicScript)
        {
            throw new NotImplementedException();
        }
    }
}
