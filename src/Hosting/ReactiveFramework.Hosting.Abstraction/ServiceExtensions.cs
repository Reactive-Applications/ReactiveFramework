using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Hosting;
public static class ServiceExtensions
{
    public static IEnumerable<object> GetServices(this IServiceProvider serviceProvider, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var service = serviceProvider.GetService(type);
            if (service != null)
            {
                yield return service;
            }
        }
    }
}
