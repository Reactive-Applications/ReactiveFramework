using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ReactiveFramework.Hosting.Abstraction;
public interface IPluginApplicationBuilder
{
    IPluginApplication Build();

    IPluginHostEnvironment Environment { get; }

    IServiceCollection InitializationServices { get; }

    ConfigurationManager Configuration { get; }

    ILoggingBuilder Logging { get; }

    IDictionary<object, object> Properties { get; }

    IPluginApplicationBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate);

    IPluginApplicationBuilder ConfigureAppConfiguration(Action<PluginHostBuilderContext, IConfigurationBuilder> configureDelegate);

    IPluginApplicationBuilder UseRuntimeServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) 
        where TContainerBuilder : notnull;

    IPluginApplicationBuilder UseRuntimeServiceProviderFactory<TContainerBuilder>(Func<PluginHostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        where TContainerBuilder : notnull;

    IPluginApplicationBuilder ConfigureContainer<TContainerBuilder>(Action<PluginHostBuilderContext, TContainerBuilder> configureDelegate);
}
