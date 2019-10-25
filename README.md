# Seventiny.Cloud.FaaS
（Function as a Service）函数即服务框架是一组灵活，可靠，动态伸缩，沙箱化的脚本服务框架。借助该框架的能力，客户端无需关心业务脚本的运行环境，机器部署，服务扩容等基础设施。而将关注点聚焦在核心功能脚本的编写以及功能脚本的触发时机这两点上。

（Function as a Service）The Function as a Service function is a flexible, reliable, dynamic, and sandboxed script service framework. With the capabilities of the framework, the client does not need to care about the operating environment of the business script, machine deployment, service expansion and other infrastructure. Focus on the writing of core function scripts and the timing of triggering function scripts.

FaaS框架的核心是“动态脚本引擎”，该引擎提供了一种在运行时编译，加载，执行代码脚本的能力。通过这种能力，可以将存储在数据库、磁盘等文件系统的脚本代码实时获取并编译加载到内存中执行。

At the heart of the FaaS framework is the "Dynamic Scripting Engine", which provides the ability to compile, load, and execute code scripts at runtime. Through this capability, script code stored in a file system such as a database or a disk can be acquired and compiled into memory for execution in real time.

动态编译的能力可以轻松应用在PaaS、FaaS、沙箱、插件化等场景，最大限度地满足“动态”、“热插拔”的需求。

The ability to dynamically compile can be easily applied to scenarios such as PaaS, FaaS, sandboxing, and plug-in to maximize the need for "dynamic" and "hot swap".

## 示例代码 Example
```CSharp
IDynamicScriptEngine scriptEngineProvider = new CSharpDynamicScriptEngine();

DynamicScript script = new DynamicScript();

script.TenantId = 0;
script.Language = DynamicScriptLanguage.CSharp;
script.Script =
@"
using System;
public class Test
{
    public int GetA(int a)
    {
        return a;
    }
}
";
script.ClassFullName = "Test";
script.FunctionName = "GetA";
script.Parameters = new object[] { 111 };

var result = scriptEngineProvider.Run<int>(script);

Assert.True(result.IsSuccess);
Assert.Equal(111, result.Data);
```

## 支持模式
1. SDK Nuget 本地模式 | Nuget SDK local mode
2. RPC远程调用模式 | Rpc remote execute mode
3. Restful Api远程调用模式 | Restful api request mode
4. 定时任务模式 | Timed task mode

# Support mode
1. SDK Nuget local mode | Nuget SDK local mode
2. RPC remote call mode | Rpc remote execute mode
3. Restful Api remote call mode | Restful api request mode
4. Timed task mode | Timed task mode

## 使用方式
> 首先需要提供MySql环境，并导入框架的SQL以提供远程配置的能力，祥见使用教程
1. 直接引用对应语言的SDK，开启本地模式使用
2. 部署Grpc服务端，客户端采用 Grpc 远程调用
3. 部署webapi服务端，客户端 http 请求远程调用
4. 部署FaaS站点配置定时任务

## Way of use
> First need to provide the MySql environment, and import the framework's SQL to provide remote configuration capabilities, see Use tutorial
1. Directly reference the SDK of the corresponding language and enable local mode.
2. Deploy the Grpc server, the client uses Grpc remote call
3. Deploy webapi server, client http request remote call
4. Deploy the FaaS site to configure scheduled tasks.

## 使用教程
1. 将 SQL 目录下的 appsettings.json 启动配置文件复制到运行程序的根目录，例如Test项目的 Test.SevenTiny.Cloud.ScriptEngine.CSharp\bin\Debug\netcoreapp3.0 目录
2. 创建数据库 SevenTinyCloudFaaS（名字可自取），然后将该数据库的连接信息配置到步骤1中的 appsettings.json 配置文件中 ConnectionStrings -> SevenTinyConfig 节点中
3. 将 SQL 目录下的数据库表初始化脚本在步骤二中创建的数据库中执行，以生成运行必要的数据库表结构和表数据
4. 如果 appsettings.json 在 Test 项目中已复制并配置好数据库，那么此时可以运行测试用例进行Demo
5. 如果是远程模式，推荐集群部署的模式，每个应用部署一套远程服务，并在 appsettings.json 配置文件中的 SevenTinyCloud -> AppName 节点配置对应的应用（集群）名，因为不同的应用可能需要不同的dll引用，dll引用在 FaaS_CSharpAssemblyReference 中配置
6. 运行期间生成的本地日志，二进制文件，脚本文件，日志信息可以在应用程序的根目录下（会自动创建相应的文件夹）找到

## Use tutorial
1. Copy the appsettings.json startup configuration file in the SQL directory to the root directory of the running program, such as the Test.SevenTiny.Cloud.ScriptEngine.CSharp\bin\Debug\netcoreapp3.0 directory of the Test project.
2. Create the database SevenTinyCloudFaaS (name can be self-fetched), and then configure the connection information of the database into the appsettings.json configuration file in step 1 in the ConnectionStrings -> SevenTinyConfig node
3. Execute the database table initialization script under the SQL directory in the database created in step 2 to generate the necessary database table structure and table data.
4. If appsettings.json has copied and configured the database in the Test project, then you can run the test case for Demo at this point.
5. In the remote mode, the cluster deployment mode is recommended. Each application deploys a set of remote services and configures the corresponding application (cluster) name in the SevenTinyCloud -> AppName node in the appsettings.json configuration file, as different applications may need to be different. Dll reference, dll reference is configured in FaaS_CSharpAssemblyReference
6. Local logs, binary files, script files, log information generated during the run can be found in the root directory of the application (the corresponding folder will be created automatically)

## 数据库表说明
> 需注意，部分数据库表充当着远程配置的角色，例如：FaaS_Settings，FaaS_Settings，Logging

- FaaS_Settings
全局设置，用于设置脚本编译模式、是否输出编译文件、nuget引用目录等信息
- FaaS_CSharpAssemblyReference
CSharp 环境动态脚本需要引用的dll路径配置（该dll应该存在于任意一个FaaS_Settings配置中的nuget引用目录的相对地址）
AppName列是为了区分不同的AppName动态脚本引擎服务需要引用不同的dll而不是全部加载到服务中，可以根据不同的应用配置不同的dll引用。脚本引擎编译时，会加载appsettings.json 配置文件中的 SevenTinyCloud -> AppName 对应的数据库中配置的dll，无论有多少个AppName节点，AppName=System下配置的dll都会被加载，该AppName是全局默认加载的配置信息，当然你可以全部配置在System节点中，但是那样并不好区分）
- Logging
日志组件配置，可以设置脚本引擎服务中不同等级的日志是否输出。例如讲Level_Debug设置为0，则所有的Debug日志将不会输出
- DynamicScript
FaaS 平台下用户配置的脚本将存储在这个数据库表中

## Database Table Description
> Note that some database tables act as remote configuration roles, for example: FaaS_Settings, FaaS_Settings, Logging

- FaaS_Settings
Global settings for setting script compilation mode, outputting compiled files, nuget reference directories, etc.
- FaaS_CSharpAssemblyReference
The CSharp environment dynamic script needs to reference the dll path configuration (the dll should exist in the relative address of the nuget reference directory in any FaaS_Settings configuration)
The AppName column is used to distinguish between different AppName dynamic scripting engine services that need to reference different dlls instead of all loaded into the service. Different dll references can be configured according to different applications. When the script engine compiles, it will load the dll configured in the database corresponding to SevenTinyCloud -> AppName in the appsettings.json configuration file. No matter how many AppName nodes, the dll configured under AppName=System will be loaded. The AppName is the global default load. Configuration information, of course, you can configure all in the System node, but that is not a good distinction)
- Logging
Log component configuration, you can set whether the different levels of logs in the script engine service are output. For example, if Level_Debug is set to 0, all Debug logs will not be output.
- DynamicScript
User-configured scripts under the FaaS platform will be stored in this database table.