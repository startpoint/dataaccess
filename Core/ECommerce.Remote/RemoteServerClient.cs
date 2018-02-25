using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ECommerce.Core
{
    public class RemoteServerClient<T> : IServiceProxy<T> where T:class, new()
    {
        private RemoteServiceSettings _remoteServiceSettings;

        public RemoteServerClient(RemoteServiceSettings remoteServiceSettings) 
            => _remoteServiceSettings = remoteServiceSettings;
    }
}