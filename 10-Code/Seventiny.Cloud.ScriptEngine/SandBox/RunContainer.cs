using Fasterflect;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Seventiny.Cloud.ScriptEngine.SandBox
{
    [Serializable]
    internal class RunContainer : MarshalByRefObject
    {
        public object ExecuteUntrustedCode(Type type, string methodName, params object[] parameters)
        {
            var obj = Activator.CreateInstance(type);
            try
            {
                return obj.TryCallMethodWithValues(methodName, parameters);
            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
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
                //_logger.ErrorFormat("[Tag:{0},Method:{1},Timeout:{2}, execution timed out", type.Assembly.FullName,methodName,millisecondsTimeout);
                return null;
            }
            return result;
        }
    }
}
