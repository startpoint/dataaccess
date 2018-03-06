using System;
using ECommerce.Core;

namespace ECommerce.Remote
{
    public static class ResolveServiceProxy
    {
        public static ServiceProxy CreateProxy(RemoteServiceSettings remoteServiceSettings, Func<ServiceProxy> delFunc)
        {
            return !remoteServiceSettings.IsLocal
                ? new LocalServiceProxy(remoteServiceSettings).GetInstance(remoteServiceSettings, delFunc)
                : new RemoteServiceProxy(remoteServiceSettings).GetInstance(remoteServiceSettings, delFunc);
        }
    }
}