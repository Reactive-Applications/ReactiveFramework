using Microsoft.Extensions.Hosting;
using ReactiveFramework.Modularity.Abstraction;
using ReactiveFramework.Modularity.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Modularity.Extensions;
public static class HostBuilderExtensions
{
    public static IHostBuilder UseModuleCatalog<T>(this IHostBuilder builder)
        where T : IModuleCatalog, new()
    {
        builder.Properties.Add("ActiveModuleCatalog", new T());
        return builder;
    }

    public static IHostBuilder UseDefaultModuleCatalog(this IHostBuilder builder)
    {
        builder.Properties.Add("ActiveModuleCatalog", new ModuleCatalog());
        return builder;
    }

    public static async Task RegisterModuleAsync<T>(this IHostBuilder builder)
        where T : IModule, new()
    {
        var catalog = builder.GetModuleCatalog();
        var module = catalog.Add<T>();
        var ctx = builder.GetModuleRegistrationContext();
        await module.RegisterModuleAsync(ctx)
            .ConfigureAwait(false);
    }

    public static IModuleCatalog GetModuleCatalog(this IHostBuilder builder)
    {
        if (!builder.Properties.ContainsKey("ActiveModuleCatalog"))
        {
            builder.UseDefaultModuleCatalog();
        }

        return (IModuleCatalog)builder.Properties["ActiveModuleCatalog"];
    }

    private static IModuleRegistrationContext GetModuleRegistrationContext(this IHostBuilder builder)
    {
        if (!builder.Properties.ContainsKey("ModuleRegistrationContext"))
        {

        }
    }

}
