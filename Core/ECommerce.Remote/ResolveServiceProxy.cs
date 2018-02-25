using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core
{
    public static class ResolveServiceProxy
    {
        public static IServiceProxy<T> CreateProxy<T>(RemoteServiceSettings remoteServiceSettings, ILoggerFactory logger) 
        where T:class, new()
        {
            if(remoteServiceSettings.IsLocal)
                return new LocalServiceProxy<T>(remoteServiceSettings);
            else
                return new RemoteServerClient<T>(remoteServiceSettings);
        }
    }
}