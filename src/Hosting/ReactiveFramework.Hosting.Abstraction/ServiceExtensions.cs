using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ReactiveFramework.Hosting.Abstraction;
public static class ServiceExtensions
{
    public static IServiceCollection AddStartupActions<TInitializer>(this IServiceCollection services)
        where TInitializer : class, IHostStartupAction
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostStartupAction, TInitializer>());
        return services;
    }

    public static IEnumerable<object> GetServices(this IServiceProvider serviceProvider, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var service = serviceProvider.GetService(type);

            yield return service ?? throw new InvalidOperationException($"{type.FullName} is not registered");
        }
    }

    public static IEnumerable<IHostStartupAction> GetHostStartupActions(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServices<IHostStartupAction>();
    }
}
