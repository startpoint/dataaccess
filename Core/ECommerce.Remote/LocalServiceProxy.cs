using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ECommerce.Core
{
    public class LocalServiceProxy<T> : IServiceProxy<T> where T:class, new()
    {
        private RemoteServiceSettings _remoteServiceSettings;

        public LocalServiceProxy(RemoteServiceSettings remoteServiceSettings) 
            => _remoteServiceSettings = remoteServiceSettings;
    }
}