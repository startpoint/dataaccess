using System;
using ECommerce.Core;

namespace ECommerce.Remote
{
    public class RemoteServiceProxy
    {
        private RemoteServiceSettings _remoteServiceSettings;

        public RemoteServiceProxy(RemoteServiceSettings remoteServiceSettings) 
            => _remoteServiceSettings = remoteServiceSettings;

        public ServiceProxy GetInstance(RemoteServiceSettings remoteServiceSettings, Func<ServiceProxy> delFunc)
        {
            return delFunc.Invoke();
        }
    }
}