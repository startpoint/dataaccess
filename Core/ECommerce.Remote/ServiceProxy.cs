using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ECommerce.Core;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Remote
{
    public class ServiceProxy
    {
        private readonly RemoteServiceSettings _settings;

        public ServiceProxy(RemoteServiceSettings settings)
        {
            _settings = settings;
        }

        protected Task<ExecutionResult<T>> InvokeMethod<T>(object service, string methodName, params object[] arguments)
        {
                return (_settings.IsLocal) ? InvokeLocalMethod<T>(service, methodName, arguments)
                    : throw new System.NotImplementedException();
        }

        private static async Task<ExecutionResult<T>> InvokeLocalMethod<T>(object service, string methodName, object[] arguments)
        {
            var methodInfo = service.GetType().GetMethod(methodName, arguments.Select(x => x.GetType()).ToArray());

            if (methodInfo == null)
            {
                throw new System.NotImplementedException($"{service}.{methodName}");
            }

            var isAsynchronous = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null;

            var result =
                isAsynchronous
                    ? InvokeAsyncTask<T>(methodInfo, service, arguments)
                    : InvokeSyncTask<T>(methodInfo, service, arguments);

            return await result;
        }

        private static async Task<ExecutionResult<T>> InvokeSyncTask<T>(MethodInfo methodInfo, object service, object[] arguments)
        {
            return await Task.Run(()=> (Task<ExecutionResult<T>>)methodInfo.Invoke(service, arguments));
        }

        private static Task<ExecutionResult<T>> InvokeAsyncTask<T>(MethodInfo methodInfo, object service, params object[] arguments)
        {
            return ((Task<ExecutionResult<T>>) methodInfo.Invoke(service, arguments));
        }
    }
}