using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace ReactiveFramework;

public static class ServiceProviderExtensions
{
    public static T GetUnregisteredService<T>(this IServiceProvider serviceProvider)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance<T>(serviceProvider);
    }

    public static object GetUnregisteredService(this IServiceProvider serviceProvider, Type type)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
    }

    public static T GetServiceWithParameters<T>(this IServiceProvider serviceProvider, params object[] parameters)
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
    }
}
