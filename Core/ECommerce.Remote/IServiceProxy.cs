using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core
{
    public interface IServiceProxy<T> where T:class, new()
    {
        
    }
}