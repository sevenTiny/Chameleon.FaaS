namespace SevenTiny.Cloud.ScriptEngine.Toolkit
{
    internal class Const
    {
        public const string DefaultOutPutDllPath = "SeventinyScriptEngine";
        public const string DefaultProjectName = "SevenTinyCloud";

        public const string ScriptEngine_AssemblyReferenceConfig_SystemAssemblyKey = "System";
        public const string ScriptEngine_AssemblyReferenceConfig_ReferenceDllDirectoryName = "SeventinyCloudReferences";

        //Template
        public const string EndUsing = "//EndUsing";
        public const string Rest = "Rest";
        public const string Name = "name";
        public const string Value = "value";
        public const string CodeId = "code id";
        public const string Code = "code";
        public const string Class = "class";
        public const string Using = "using";
        public const string Public = "public";
        public const string TypeOf = "typeof";
        public const string Package = "import";
        public const string Execute = "execute";
        public const string Methods = "methods";
        public const string Undefined = "undefined";
        public const string InternalContext = "InternalContext";
        public const string ContextObjects = "contextObjects";
        public const string AssemblyName = "GeneratedAssembly";
        public const string OperationContract = "OperationContract";

        public const string MethodTypeName = "CSharpScriptEngine{0}.DynamicScriptExecutor";
        public const string AssemblyScriptKey = "GeneratedAssembly_{0}_{1}_{2}_{3}";

        public const string CSharpCommonUsing =
@"";
        public const string CSharpCommonCode =
@"";
        /// <summary>
        /// C# code template
        /// </summary>
        public const string CsharpScriptTemplate =
@"
using System;
using System.Collections;
using System.Collections.Generic;
[_common_using_]
[_using_]

[_namespaces_]
namespace CSharpScriptEngine[_tenantId_]
{
    public class DynamicScriptExecutor
    {
        [_common_code_]
        [_code_]
    }
}";
    }
}
