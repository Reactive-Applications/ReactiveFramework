using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Modularity.Abstraction;
using ReactiveFramework.Modularity.StartupActions;
using ReactiveFramework.Modularity.Internal;

namespace ReactiveFramework.Modularity.Extensions;
public static class HostBuilderExtensions
{
    public static void UseModularity(this IRxHostBuilder builder)
    {
        builder.Services.AddStartupActions<ModuleInitializer>();
    }

    public static void UseModuleCatalog<T>(this IRxHostBuilder builder)
        where T : IModuleCatalog, new()
    {
        builder.Properties[typeof(IModuleCatalog)] = new T();
    }

    public static void UseModuleRegistrationContextBuilder<T>(this IRxHostBuilder builder)
        where T : IModuleRegistrationContextBuilder, new()
    {
        builder.Properties[typeof(IModuleRegistrationContextBuilder)] = new T();
    }

    public static void UseModuleRegistrationContextBuilder<T>(this IRxHostBuilder builder, T ctxBuilder)
        where T : IModuleRegistrationContextBuilder
    {
        builder.Properties[typeof(IModuleRegistrationContextBuilder)] = ctxBuilder;
    }

    public static async Task RegisterModuleAsync<T>(this IRxHostBuilder builder, CancellationToken cancellation)
        where T : IModule, new()
    {
        var catalog = builder.GetModuleCatalog();
        var module = catalog.Add<T>();
        var ctx = builder.GetModuleRegistrationContext();
        await module.RegisterModuleAsync(ctx, cancellation)
            .ConfigureAwait(false);
    }

    private static IModuleCatalog GetModuleCatalog(this IRxHostBuilder builder)
    {
        if(!builder.Properties.TryGetValue(typeof(IModuleCatalog), out var moduleCatalog))
        {
            moduleCatalog = new ModuleCatalog();
            builder.Services.AddSingleton((IModuleCatalog)moduleCatalog);
            builder.Properties[typeof(IModuleCatalog)] = moduleCatalog;
        }

        return (IModuleCatalog)moduleCatalog;
    }

    private static IModuleRegistrationContext GetModuleRegistrationContext(this IRxHostBuilder builder)
    {
        
        if (!builder.Properties.TryGetValue("ContextBuilderBuilded", out var _))
        {
            builder.BuildModuleRegistrationContext();
        }
        return (IModuleRegistrationContext)builder.Properties[typeof(IModuleRegistrationContext)];
    }

    private static void BuildModuleRegistrationContext(this IRxHostBuilder builder)
    {
        if (!builder.Properties.TryGetValue(typeof(IModuleRegistrationContextBuilder), out var contextBuilder))
        {
            contextBuilder = new ModuleRegistrationContextBuilder();
            builder.Properties[typeof(IModuleRegistrationContextBuilder)] = contextBuilder;
        }
        builder.Properties[typeof(IModuleRegistrationContext)] = ((IModuleRegistrationContextBuilder)contextBuilder).Build(builder);
        builder.Properties["ContextBuilderBuilded"] = new object();
    }
}
