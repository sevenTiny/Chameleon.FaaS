using Newtonsoft.Json;
using SevenTiny.Cloud.ScriptEngine.Configs;
using System;
using Xunit;

namespace Test.Seventiny.Cloud.ScriptEngine
{
    public class AppSettingsConfigTest
    {
        [Fact]
        public void GetCurrentAppName()
        {
            var currentAppName = AppSettingsConfigHelper.GetAppName();
            Assert.NotNull(currentAppName);
        }
    }
}
