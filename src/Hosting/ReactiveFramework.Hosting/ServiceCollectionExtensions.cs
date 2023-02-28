using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReactiveFramework.Hosting.Abstraction;

namespace ReactiveFramework.Hosting;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupService<TInitializer>(this  IServiceCollection services)
        where TInitializer : class, IStartupService
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IStartupService, TInitializer>());
        return services;
    }
}
