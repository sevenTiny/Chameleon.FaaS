using Fasterflect;
using SevenTiny.Bantina.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SevenTiny.Cloud.ScriptEngine.SandBox
{
    [Serializable]
    internal class RunContainer : MarshalByRefObject
    {
        private static readonly ILog _logger = new LogManager();
        public object ExecuteUntrustedCode(Type type, string methodName, params object[] parameters)
        {
            var obj = Activator.CreateInstance(type);
            try
            {
                return obj.TryCallMethodWithValues(methodName, parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        public object ExecuteUntrustedCode(Type type, string methodName, int millisecondsTimeout, params object[] parameters)
        {
            object result = null;
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var t = Task.Factory.StartNew(() => { result = ExecuteUntrustedCode(type, methodName, parameters); }, token);

            if (!t.Wait(millisecondsTimeout, token))
            {
                tokenSource.Cancel();
                _logger.Error(string.Format("[Tag:{0},Method:{1},Timeout:{2}, execution timed out", type.Assembly.FullName, methodName, millisecondsTimeout));
                return null;
            }
            return result;
        }
    }
}
