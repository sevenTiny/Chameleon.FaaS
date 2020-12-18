using SevenTiny.Cloud.ScriptEngine.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.Seventiny.Cloud.ScriptEngine
{
    public class FaaSSettingsConfigTest
    {
        [Fact]
        public void GetReferenceDirs()
        {
            var referenceDirs = FaaSSettingsConfigHelper.GetReferenceDirs();
            Assert.NotEmpty(referenceDirs);
        }
    }
}
