using System;
using System.Collections.Generic;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine
{
    public class DynamicScript
    {
        public int TenantId { get; set; } = 10000;
        public string ProjectName { get; set; }
        public string Script { get; set; }
        public DynamicScriptLanguage Language { get; set; }
        public OnFailureAction OnFailureAction { get; set; }
    }
}
